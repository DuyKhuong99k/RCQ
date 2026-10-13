using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using System.Globalization;

namespace Services;

/// <summary>Đóng ca máy cũ, mở ca độc lập tại máy mới; lưu người xác nhận và lý do.</summary>
public static class ScaleAttendanceTransferService
{
    public static bool IsExpired(CheckInOut row, DateTime time, int hours) =>
        ScaleAttendanceProcessor.IsShiftExpired(row.ThoiGian, time, hours, row.CaDem);

    // Chỉ dùng cho thao tác online hiện tại. Không giới hạn lịch sử quản lý,
    // đồng bộ offline hoặc truy vấn OpenShifts phục vụ đối soát.
    public static IQueryable<CheckInOut> OperationalEvents(dbPMScontext db, string card,
        int machine, DateTime time, int hours)
    {
        var dayStart = time.Date;
        var nightStart = time.AddHours(-hours);
        return db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == card
            && x.MaSoMay == machine && (x.TrangThai == "Vào" || x.TrangThai == "Ra")
            && (x.ThoiGian >= dayStart || (x.CaDem && x.ThoiGian >= nightStart)));
    }

    public static bool IsInOperationalDay(CheckInOut row, DateTime time, int hours) =>
        row.ThoiGian >= time.Date || (row.CaDem && row.ThoiGian >= time.AddHours(-hours));
    public static void LockCard(dbPMScontext db, string card)
    {
        var resource = "ScaleAttendance:card:" + card.Trim().ToUpperInvariant();
        db.Database.ExecuteSqlInterpolated($@"
            DECLARE @result int;
            EXEC @result = sys.sp_getapplock @Resource = {resource},
                @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 5000;
            IF @result < 0 THROW 51001, N'Không thể khóa ca chấm công.', 1;");
    }

    public static List<CheckInOut> OpenShifts(dbPMScontext db, string? card = null) =>
        db.CheckInOuts.AsNoTracking().Where(x => x.TrangThai == "Vào" &&
            (card == null || x.MaChamCong == card) &&
            !db.CheckInOuts.Any(next =>
                next.VaoId == x.Id ||
                (next.MaChamCong == x.MaChamCong && next.MaSoMay == x.MaSoMay &&
                 (next.TrangThai == "Vào" || next.TrangThai == "Ra") &&
                 (next.ThoiGian > x.ThoiGian || (next.ThoiGian == x.ThoiGian && next.Id.CompareTo(x.Id) > 0)))))
            .OrderByDescending(x => x.ThoiGian).ToList();

    public static string Transfer(int userId, string openId, int targetMachine, DateTime time,
        string reason, int maxShiftHours)
    {
        using var db = new dbPMScontext();
        var user = db.NguoiDungs.AsNoTracking().FirstOrDefault(x => x.Id == userId);
        if (user?.IsAdmin != true || user.IsActive == false)
            throw new InvalidOperationException("Chỉ tài khoản quản trị được xác nhận chuyển máy.");
        reason = reason.Trim();
        if (reason.Length is 0 or > 500) throw new ArgumentException("Nhập lý do chuyển máy, tối đa 500 ký tự.");
        time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime;
        if (time > now) throw new ArgumentException("Giờ chuyển không được nằm trong tương lai.");
        if (maxShiftHours is < 1 or > 24) throw new ArgumentException("Giới hạn ca không hợp lệ.");
        var original = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == openId);
        if (original == null) throw new InvalidOperationException("Không tìm thấy phiếu Vào.");

        using var transaction = db.Database.BeginTransaction();
        LockCard(db, original.MaChamCong);
        // Cùng thứ tự khóa card -> device với xử lý quẹt thẻ.
        foreach (var machine in new[] { original.MaSoMay, targetMachine }.Distinct().OrderBy(x => x))
        {
            var resource = "ScaleAttendance:device:" + machine.ToString(CultureInfo.InvariantCulture);
            db.Database.ExecuteSqlInterpolated($@"
                DECLARE @result int;
                EXEC @result = sys.sp_getapplock @Resource = {resource},
                    @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 5000;
                IF @result < 0 THROW 51001, N'Không thể khóa máy cân.', 1;");
        }
        var active = OpenShifts(db, original.MaChamCong);
        var source = active.FirstOrDefault(x => x.Id == openId);
        if (source == null) throw new InvalidOperationException("Ca đã thay đổi hoặc đã Ra. Tải lại danh sách.");
        if (source.Isforget != 0) throw new InvalidOperationException("Ca đang bị đánh dấu lỗi. Cần xử lý giờ Ra thực tế trước khi chuyển máy.");
        var rootStart = source.ThoiGian;
        if (time <= source.ThoiGian || ScaleAttendanceProcessor.IsShiftExpired(rootStart, time, maxShiftHours, source.CaDem) ||
            ScaleAttendanceProcessor.IsShiftExpired(rootStart, now, maxShiftHours, source.CaDem))
            throw new InvalidOperationException("Ca đã hết hạn hoặc giờ chuyển không sau giờ Vào. Cần xử lý phiếu lỗi.");
        if (active.Any(x => x.Id != source.Id && !ScaleAttendanceProcessor.IsShiftExpired(
            x.ThoiGian, time, maxShiftHours, x.CaDem)))
            throw new InvalidOperationException("Có nhiều ca đang mở. Cần đối chiếu trước khi chuyển máy.");
        if (source.MaSoMay == targetMachine) throw new ArgumentException("Máy mới phải khác máy đang làm.");
        if (db.CheckInOuts.Any(x => x.MaChamCong == source.MaChamCong && x.ThoiGian > time))
            throw new InvalidOperationException("Đã có lượt quẹt sau giờ chuyển. Cần tải lại và kiểm tra.");
        var target = db.MayCans.AsNoTracking().FirstOrDefault(x => x.Id == targetMachine.ToString());
        if (target == null || target.WKv != Vars.AppKV.Hq)
            throw new ArgumentException("Máy mới phải là máy cân HQ đã đăng ký.");
        var outId = AttendanceRecordId.Create(source.MaSoMay.ToString(CultureInfo.InvariantCulture), time);
        var inId = AttendanceRecordId.Create(targetMachine.ToString(CultureInfo.InvariantCulture), time);
        if (db.CheckInOuts.Any(x => x.Id == outId || x.Id == inId))
            throw new InvalidOperationException("Đã có phiếu tại giây chuyển này. Chọn thời điểm khác.");
        var targetName = string.IsNullOrWhiteSpace(target.DisplayName) ? target.Id : target.DisplayName.Trim();
        if (targetName.Length > 200) throw new ArgumentException("Tên máy mới vượt quá 200 ký tự.");
        var actor = user.UserName;
        db.CheckInOuts.AddRange(
            new CheckInOut
            {
                Id = outId, MaChamCong = source.MaChamCong, MaSoMay = source.MaSoMay,
                ThoiGian = time, TenMay = source.TenMay, CongViec = source.CongViec,
                TrangThai = "Ra", CaDem = source.CaDem, VaoId = source.Id,
                NguoiXacNhan = actor, LyDo = reason
            },
            new CheckInOut
            {
                Id = inId, MaChamCong = source.MaChamCong, MaSoMay = targetMachine,
                ThoiGian = time, TenMay = targetName, CongViec = targetName,
                TrangThai = "Vào", CaDem = source.CaDem,
                NguoiXacNhan = actor, LyDo = reason
            });
        db.SaveChanges();
        transaction.Commit();
        return $"Đã chuyển thẻ {source.MaChamCong} từ {source.MaSoMay} sang {targetMachine} lúc {time:dd/MM HH:mm:ss}.";
    }
}
