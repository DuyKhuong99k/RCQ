using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using System.Globalization;

namespace Services;

/// <summary>
/// SignalR endpoint dành riêng cho thiết bị tại xưởng.
/// </summary>
public sealed class WorkshopHub : Hub
{
    public async Task Register(
        string deviceCode,
        string workshopCode,
        string deviceName)
    {
        if (string.IsNullOrWhiteSpace(deviceCode))
            return;

        using var db = new dbPMScontext();

        var device = db.ThietBiChamCongTaiXuongs.Find(deviceCode)
                     ?? new WorkshopDevice
                     {
                         DeviceCode = deviceCode
                     };

        device.WorkshopCode = workshopCode ?? "";
        device.DeviceName = string.IsNullOrWhiteSpace(deviceName)
            ? deviceCode
            : deviceName;

        // Lấy IP nguồn của thiết bị từ kết nối SignalR để giao diện có thể mở trang cấu hình.
        var remoteIp = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrWhiteSpace(remoteIp))
            device.IpAddress = remoteIp;

        device.ConnectionId = Context.ConnectionId;
        device.IsOnline = true;

        // Không dùng DateTime.Now nếu muốn đồng bộ thời gian với SQL Server.
        // Tuy nhiên chỗ này đang Save qua EF nên có thể để thời gian DB tự xử lý
        // nếu property được cấu hình default. Nếu chưa cấu hình thì tạm dùng UTC.
        device.LastSeen = DateTime.Now;

        if (db.Entry(device).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
        {
            db.ThietBiChamCongTaiXuongs.Add(device);
        }

        db.SaveChanges();

        await Clients.Caller.SendAsync(
            "REGISTERED",
            new
            {
                deviceCode,
                workshopCode
            });
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        using var db = new dbPMScontext();

        var device = db.ThietBiChamCongTaiXuongs
            .FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

        if (device != null)
        {
            device.IsOnline = false;
            device.ConnectionId = null;

            db.SaveChanges();
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task CheckIn(
    string deviceCode,
    string cardId,
    string status,
    string time,
    string xuong)
    {
        var result = AttendanceProcessor.Save(
            deviceCode,
            cardId,
            status,
            time,
            xuong);

        await Clients.Caller.SendAsync(
            "CHAMCONG_RESULT",
            result);
    }
}

public static class AttendanceProcessor
{
    public static object Save(
        string deviceCode,
        string maTheTu,
        string? status,
        string time,
        string xuong)
    {
        // ----------------------------------------------------
        // 1. Parse thời gian từ firmware
        // ----------------------------------------------------
        var formats = new[]
        {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy/MM/dd HH:mm:ss",
            "yyyyMMddHHmmss"
        };

        if (!DateTime.TryParseExact(
                time,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out var when) &&
            !DateTime.TryParse(
                time,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out when))
        {
            return new
            {
                success = false,
                message = "Thời gian không hợp lệ."
            };
        }

        // ----------------------------------------------------
        // 2. Tìm mã thẻ -> mã nhân viên
        // ----------------------------------------------------
        deviceCode = deviceCode.Trim();
        maTheTu = maTheTu.Trim();
        var recordId = AttendanceRecordId.Create(deviceCode, when);
        using var db = new dbPMScontext();

        var card = db.TheTu
    .FirstOrDefault(x => x.MaTheTu == maTheTu);

        var employeeId = card?.MaNhanVien;

        // ----------------------------------------------------
        // 3. Tìm mã nhân viên -> tên nhân viên
        // ----------------------------------------------------
        var employee = !string.IsNullOrWhiteSpace(employeeId)
            ? db.NhanVienDaiThanh.FirstOrDefault(x => x.MaNhanVien == employeeId)
            : null;

        var isRegistered = card is not null && employee is not null;

        // Xác nhận đã nhận lượt quẹt, nhưng không ghi phiếu nếu chưa có
        // thẻ và nhân viên hợp lệ trong database (không tin tên do thiết bị gửi).
        // Return trước truy vấn lịch sử/Add/SaveChanges: cả HTTP và SignalR.
        if (!isRegistered)
        {
            return new
            {
                message = "Thẻ chưa đăng ký",
                maTheTu
            };
        }

        // Khóa theo mã thẻ trong SQL, dùng được khi chạy nhiều instance PMSHub.
        // Giữ khóa từ lúc đọc trạng thái đến khi commit để hai request không
        // cùng đọc một lượt trước đó và cùng ghi Vào (hoặc cùng ghi Ra).
        using var transaction = db.Database.BeginTransaction();
        var lockResource = "WorkshopAttendance:card:" + maTheTu.ToUpperInvariant();
        db.Database.ExecuteSqlInterpolated($@"
            DECLARE @lockResult int;
            EXEC @lockResult = sys.sp_getapplock
                @Resource = {lockResource}, @LockMode = 'Exclusive',
                @LockOwner = 'Transaction', @LockTimeout = 5000;
            IF @lockResult < 0
                THROW 51001, N'Không thể khóa lượt chấm công. Vui lòng thử lại.', 1;");

        // Kiểm tra trùng trước khi suy ra trạng thái; gửi lại không đảo Vào/Ra.
        var existing = db.ChamCongTaiXuongs.AsNoTracking()
            .FirstOrDefault(x => x.Id == recordId);

        if (existing != null)
        {
            transaction.Commit();
            return new
            {
                success = true,
                duplicated = true,
                maTheTu,
                maNhanVien = employeeId ?? "",
                tenNhanVien = employee?.Name ?? "",
                thoiGian = when,
                xuong,
                thietBi = deviceCode,
                trangThai = existing.TrangThai
            };
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            // Không reset lúc 0 giờ. Chỉ xét các lượt Vào/Ra hợp lệ gần nhất,
            // không tính bản ghi cũ "Thẻ chưa đăng ký" hoặc trạng thái khác.
            var latest = db.ChamCongTaiXuongs.AsNoTracking()
                .Where(x => x.MaTheTu == maTheTu &&
                    (x.TrangThai == "Vào" || x.TrangThai == "Ra"))
                .OrderByDescending(x => x.ThoiGian)
                .ThenByDescending(x => x.Id)
                .Select(x => new { x.ThoiGian, x.TrangThai })
                .FirstOrDefault();

            // Dữ liệu đến sai thứ tự không đủ thông tin để suy ra hướng.
            // Không chèn giữa lịch sử rồi làm sai trạng thái các lượt đã lưu.
            if (latest != null && latest.ThoiGian > when)
            {
                return new
                {
                    success = false,
                    message = "Thời gian lượt quẹt cũ hơn lượt đã lưu. Kiểm tra giờ thiết bị.",
                    maTheTu
                };
            }

            status = NextStatus(latest?.TrangThai);
        }
        else
        {
            status = status.Trim();
            if (status != "Vào" && status != "Ra")
            {
                return new
                {
                    success = false,
                    message = "Trạng thái chấm công chỉ được là Vào hoặc Ra.",
                    maTheTu
                };
            }
        }

        // Tạo bản ghi mới sau khi xác định trạng thái.
        var item = new WorkshopAttendance
        {
            Id = recordId,
            ThoiGian = when,
            Xuong = xuong,
            ThietBi = deviceCode,
            MaTheTu = maTheTu,
            MaNhanVien = employeeId,
            TenNhanVien = employee?.Name,
            TrangThai = status
        };

        // Debug
        Console.WriteLine(
            System.Text.Json.JsonSerializer.Serialize(
                item,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                }));

        db.ChamCongTaiXuongs.Add(item);
        try
        {
            db.SaveChanges();
            transaction.Commit();
        }
        catch (DbUpdateException error) when (AttendanceRecordId.IsDuplicate(error))
        {
            transaction.Rollback();
            var saved = db.ChamCongTaiXuongs.AsNoTracking().FirstOrDefault(x => x.Id == recordId);
            if (saved is null) throw;
            return new
            {
                success = true, duplicated = true, maTheTu,
                maNhanVien = employeeId ?? "", tenNhanVien = employee?.Name ?? "",
                thoiGian = when, xuong, thietBi = deviceCode, trangThai = saved.TrangThai
            };
        }

        return new
        {
            success = true,
            duplicated = false,
            maTheTu,
            maNhanVien = employeeId ?? "",
            tenNhanVien = employee?.Name ?? "",
            thoiGian = when,
            xuong,
            thietBi = deviceCode,
            trangThai = status
        };
    }

    internal static string NextStatus(string? previousStatus) =>
        previousStatus == "Vào" ? "Ra" : "Vào";
}
