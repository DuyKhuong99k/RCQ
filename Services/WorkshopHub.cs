using Microsoft.AspNetCore.SignalR;
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

        // ----------------------------------------------------
        // 4. Nếu không gửi TrangThai thì tự xác định Vào / Ra
        // ----------------------------------------------------
        if (string.IsNullOrWhiteSpace(status))
        {
            var startOfDay = when.Date;
            var endOfDay = startOfDay.AddDays(1);

            var count = db.ChamCongTaiXuongs.Count(x =>
                x.MaTheTu == maTheTu &&
                x.ThoiGian >= startOfDay &&
                x.ThoiGian < endOfDay);

            status = isRegistered
                ? (count % 2 == 0 ? "Vào" : "Ra")
                : "Thẻ chưa đăng ký";
        }

        // ----------------------------------------------------
        // 5. Kiểm tra bản ghi trùng
        // ----------------------------------------------------
        var existing = db.ChamCongTaiXuongs.FirstOrDefault(x =>
            x.MaTheTu == maTheTu &&
            x.ThietBi == deviceCode &&
            x.ThoiGian == when);

        if (existing != null)
        {
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

        // ----------------------------------------------------
        // 6. Tạo bản ghi mới
        // ----------------------------------------------------
        var item = new WorkshopAttendance
        {
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
        db.SaveChanges();

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
}
