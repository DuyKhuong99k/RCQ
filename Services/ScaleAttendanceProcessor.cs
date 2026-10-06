using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vars.Hubs;

namespace Services;

/// <summary>Chấm công online tại cân; độc lập với phiếu chấm công tại xưởng.</summary>
public static class ScaleAttendanceProcessor
{
    public static (bool Success, string Json) Process(ClientInfo client, string payload, int maxShiftHours = 16)
    {
        if (maxShiftHours is < 1 or > 24)
            return Failure("INVALID_CONFIGURATION", "Giới hạn ca chấm công phải từ 1 đến 24 giờ.");
        Request? request;
        try { request = JsonConvert.DeserializeObject<Request>(payload); }
        catch (JsonException) { return Failure("INVALID_JSON", "JSON chấm công không hợp lệ."); }
        if (request is null) return Failure("INVALID_JSON", "JSON chấm công không hợp lệ.");

        request.TheId = request.TheId?.Trim() ?? "";
        request.ChamCongId = request.ChamCongId?.Trim() ?? "";
        request.TrangThai = request.TrangThai?.Trim() ?? "";
        if (request.TheId.Length is 0 or > 50)
            return Failure("INVALID_CARD", "Thiếu hoặc sai mã thẻ.", request);
        if (!DateTime.TryParseExact(request.NgayGio, "yyyyMMddHHmmss",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            return Failure("INVALID_TIME", "NgayGio phải có định dạng yyyyMMddHHmmss.", request);
        if (!int.TryParse(client.Id, NumberStyles.None, CultureInfo.InvariantCulture, out var machine) || machine <= 0)
            return Failure("INVALID_DEVICE", "Kết nối chưa xác định được mã máy cân.", request);
        var device = machine.ToString(CultureInfo.InvariantCulture);
        var id = AttendanceRecordId.Create(device, time);
        if (request.ChamCongId != id)
            return Failure("INVALID_ID", "ChamCongId không khớp máy đang kết nối hoặc NgayGio.", request);
        if (request.TrangThai is not ("Vào" or "Ra"))
            return Failure("INVALID_STATUS", "TrangThai chỉ được là Vào hoặc Ra.", request);

        try
        {
            using var db = new dbPMScontext();
            var card = db.TheTu.AsNoTracking().FirstOrDefault(x => x.MaTheTu == request.TheId);
            var employee = string.IsNullOrWhiteSpace(card?.MaNhanVien) ? null :
                db.NhanVienDaiThanh.AsNoTracking().FirstOrDefault(x => x.MaNhanVien == card.MaNhanVien);
            if (employee is null)
                return Failure("CARD_NOT_REGISTERED", "Thẻ chưa đăng ký", request);
            var name = db.MayCans.AsNoTracking().Where(x => x.Id == device)
                .Select(x => x.DisplayName).FirstOrDefault();
            name = string.IsNullOrWhiteSpace(name) ? device : name.Trim();
            if (name.Length > 200)
                return Failure("INVALID_DEVICE_NAME", "Tên máy vượt quá độ dài cho phép của CheckInOut.TenMay.", request);

            using var transaction = db.Database.BeginTransaction();
            // Khóa tại SQL theo máy: chống đua trạng thái và đua Id cùng giây.
            var resource = "ScaleAttendance:device:" + device;
            db.Database.ExecuteSqlInterpolated($@"
                DECLARE @result int;
                EXEC @result = sys.sp_getapplock @Resource = {resource},
                    @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 5000;
                IF @result < 0 THROW 51001, N'Không thể khóa chấm công máy cân.', 1;");

            var existing = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (existing != null)
            {
                if (existing.MaChamCong != request.TheId || existing.TrangThai != request.TrangThai)
                    return Failure("ID_CONFLICT", "Id đã được dùng cho lượt quẹt khác. Giữ nguyên bản ghi đã lưu.", request);
                var duplicate = Success(db, request, existing.ThoiGian, machine, device, employee, name, true, maxShiftHours);
                transaction.Commit();
                return duplicate;
            }

            var dayStart = time.Date;
            var dayEnd = dayStart.AddDays(1);
            // Sang ngày mới bắt đầu lại trạng thái, không ghép với ngày trước.
            var latest = db.CheckInOuts.AsNoTracking()
                .Where(x => x.MaChamCong == request.TheId && x.MaSoMay == machine &&
                    x.ThoiGian >= dayStart && x.ThoiGian < dayEnd &&
                    (x.TrangThai == "Vào" || x.TrangThai == "Ra"))
                .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id)
                .Select(x => new { x.Id, x.ThoiGian, x.TrangThai }).FirstOrDefault();
            if (latest != null && latest.ThoiGian > time)
                return Failure("OUT_OF_ORDER", "Lượt quẹt cũ hơn lịch sử. Kiểm tra giờ thiết bị.", request);
            var previousStatus = latest?.TrangThai;
            var expiredOpenShift = latest != null && previousStatus == "Vào" &&
                IsShiftExpired(latest.ThoiGian, time, maxShiftHours);
            if (expiredOpenShift && request.TrangThai == "Ra")
                return ExpiredShift(request, latest!.ThoiGian, time, maxShiftHours);
            if (!expiredOpenShift && latest != null && previousStatus == request.TrangThai)
            {
                // Quẹt lại cùng hướng: trả ca đã lưu, tuyệt đối không lấy giờ
                // quẹt lặp làm giờ Vào/Ra hoặc kéo dài tổng thời gian của ca.
                var recorded = Success(db, request, latest.ThoiGian, machine, device, employee, name, false, maxShiftHours);
                transaction.Commit();
                return recorded.Success
                    ? (false, MarkRepeated(recorded.Json, request.TrangThai))
                    : recorded;
            }
            // Ca cũ quá hạn không chặn nhân viên bắt đầu một ca Vào mới.
            var error = ValidateTransition(expiredOpenShift ? null : previousStatus, request.TrangThai);
            if (error != null) return Failure("INVALID_TRANSITION", error, request);

            // Tính kết quả trước khi ghi: lỗi tra sản lượng không tạo phiếu nửa chừng.
            var response = Success(db, request, time, machine, device, employee, name, false, maxShiftHours);
            if (!response.Success) return response;
            if (expiredOpenShift)
                response = (true, MarkRestarted(response.Json, latest!.ThoiGian));
            db.CheckInOuts.Add(new CheckInOut
            {
                Id = id, MaChamCong = request.TheId, ThoiGian = time, MaSoMay = machine,
                TenMay = name, CongViec = name, Isforget = 0,
                TrangThai = request.TrangThai
            });
            try { db.SaveChanges(); transaction.Commit(); }
            catch (DbUpdateException errorDb) when (AttendanceRecordId.IsDuplicate(errorDb))
            {
                transaction.Rollback();
                var saved = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (saved is null) throw;
                if (saved.MaChamCong != request.TheId || saved.TrangThai != request.TrangThai)
                    return Failure("ID_CONFLICT", "Id đã được dùng cho lượt quẹt khác.", request);
                return Success(db, request, saved.ThoiGian, machine, device, employee, name, true, maxShiftHours);
            }
            return response;
        }
        catch (Exception ex)
        {
            // Không đưa thông tin kết nối SQL/stack trace về firmware.
            Console.Error.WriteLine($"[CHAMCONG] Máy={device}; Id={id}; Lỗi={ex.GetType().Name}");
            return Failure("SERVER_ERROR", "Không thể xử lý chấm công. Vui lòng thử lại.", request);
        }
    }

    private static (bool Success, string Json) Success(dbPMScontext db, Request request,
        DateTime time, int machine, string device, NhanVienDaiThanh employee, string job, bool duplicated,
        int maxShiftHours)
    {
        var totalBaskets = 0;
        var totalWeight = 0m;
        var start = time;
        DateTime? end = null;
        if (request.TrangThai == "Ra")
        {
            var dayStart = time.Date;
            var checkIn = db.CheckInOuts.AsNoTracking()
                .Where(x => x.MaChamCong == request.TheId && x.MaSoMay == machine &&
                    x.ThoiGian >= dayStart && x.ThoiGian <= time &&
                    x.TrangThai == "Vào")
                .OrderByDescending(x => x.ThoiGian).Select(x => (DateTime?)x.ThoiGian)
                .FirstOrDefault();
            if (checkIn is null)
                return Failure("MISSING_CHECK_IN", "Chưa có lượt Vào trong ngày. Vui lòng quẹt Vào trước.", request);
            start = checkIn.Value;
            // Kiểm tra cả dữ liệu đã lưu/gửi lại để phiếu Ra sai trước đây
            // không tiếp tục trả tổng thời gian hoặc sản lượng nhiều ngày.
            if (IsShiftExpired(start, time, maxShiftHours))
                return ExpiredShift(request, start, time, maxShiftHours);
            end = time;
            var totals = db.HqPhieuCans.AsNoTracking()
                .Where(x => x.MayCan == device && x.TheIdNhanVien == request.TheId &&
                    x.NgayGio >= start && x.NgayGio <= time && x.ChiSanLuong && x.Status == 1)
                .GroupBy(x => 1)
                .Select(g => new { Count = g.Count(), Weight = g.Sum(x => x.TrongLuong - x.TrongLuongTare) })
                .FirstOrDefault();
            totalBaskets = totals?.Count ?? 0;
            totalWeight = totals?.Weight ?? 0m;
        }
        var timing = BuildTiming(start, end);
        return (true, JsonConvert.SerializeObject(new
        {
            request.ChamCongId, request.TheId, MaNhanVien = employee.MaNhanVien,
            TenNhanVien = employee.Name ?? "", TongSoRo = totalBaskets,
            TongTrongLuong = totalWeight, TenCongViec = job,
            timing.ThoiGianVao, timing.ThoiGianRa,
            timing.TongThoiGianGiay, timing.TongThoiGian,
            request.TrangThai, IsValid = true, Duplicated = duplicated,
            MessStr = duplicated ? $"Đã quẹt {request.TrangThai} rồi." : "Chấm công hợp lệ."
        }));
    }

    internal static bool IsShiftExpired(DateTime start, DateTime end, int maxShiftHours) =>
        start.Date != end.Date || end - start > TimeSpan.FromHours(maxShiftHours);

    internal static (bool Success, string Json) ExpiredShift(Request request, DateTime start,
        DateTime scanTime, int maxShiftHours) =>
        (false, JsonConvert.SerializeObject(new
        {
            request.ChamCongId, request.TheId, request.TrangThai,
            IsValid = false, Code = "SHIFT_EXPIRED", CanBoSungGioRa = true,
            ThoiGianVao = start.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            ThoiGianQuet = scanTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            ThoiGianRa = (string?)null, TongThoiGianGiay = (long?)null,
            TongThoiGian = (string?)null, MaxShiftHours = maxShiftHours,
            MessStr = $"Ca đã quá {maxShiftHours} giờ, cần kiểm tra giờ Ra. Quẹt Vào để bắt đầu ca mới."
        }));

    internal static string MarkRestarted(string responseJson, DateTime previousStart)
    {
        var result = JObject.Parse(responseJson);
        result["WarningCode"] = "PREVIOUS_SHIFT_MISSING_OUT";
        result["CanBoSungGioRa"] = true;
        result["ThoiGianVaoCaTruoc"] = previousStart.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        result["MessStr"] = "Đã quẹt Vào ca mới. Ca trước thiếu giờ Ra, cần bổ sung.";
        return result.ToString(Formatting.None);
    }

    internal static string MarkRepeated(string recordedJson, string status)
    {
        var result = JObject.Parse(recordedJson);
        result["IsValid"] = false;
        result["AlreadyRecorded"] = true;
        result["Code"] = status == "Vào" ? "ALREADY_CHECKED_IN" : "ALREADY_CHECKED_OUT";
        result["MessStr"] = $"Đã quẹt {status} rồi.";
        return result.ToString(Formatting.None);
    }

    // Chính sách theo ngày: tuyệt đối không cộng giờ từ ngày hôm trước.
    internal static AttendanceTiming BuildTiming(DateTime start, DateTime? end)
    {
        if (end.HasValue && end.Value < start)
            throw new ArgumentException("Giờ Ra không được trước giờ Vào.");
        if (end.HasValue && end.Value.Date != start.Date)
            throw new ArgumentException("Không tính thời gian chấm công qua ngày.");
        var seconds = end.HasValue ? (end.Value - start).Ticks / TimeSpan.TicksPerSecond : 0L;
        return new AttendanceTiming(
            start.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            end?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            seconds,
            string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}",
                seconds / 3600, seconds % 3600 / 60, seconds % 60));
    }

    internal sealed record AttendanceTiming(
        string ThoiGianVao, string? ThoiGianRa, long TongThoiGianGiay, string TongThoiGian);

    internal static string? ValidateTransition(string? previous, string current) =>
        current == "Ra" && previous != "Vào" ? "Chưa có lượt Vào trong ngày. Vui lòng quẹt Vào trước." :
        current == "Vào" && previous == "Vào" ? "Đã chấm Vào, chưa chấm Ra." : null;

    private static (bool Success, string Json) Failure(string code, string message, Request? request = null) =>
        (false, JsonConvert.SerializeObject(new
        {
            ChamCongId = request?.ChamCongId ?? "", TheId = request?.TheId ?? "",
            IsValid = false, Code = code, MessStr = message
        }));

    public sealed class Request
    {
        public string? ChamCongId { get; set; }
        public string? TheId { get; set; }
        public string? NgayGio { get; set; }
        public string? TrangThai { get; set; }
    }
}
