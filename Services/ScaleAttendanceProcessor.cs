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
    public static (bool Success, string Json) Process(ClientInfo client, string payload, int maxShiftHours = 16,
        string? selectedProductCode = null)
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
            var registeredDevice = db.MayCans.AsNoTracking().FirstOrDefault(x => x.Id == device);
            var name = registeredDevice?.DisplayName;
            name = string.IsNullOrWhiteSpace(name) ? device : name.Trim();
            if (name.Length > 200)
                return Failure("INVALID_DEVICE_NAME", "Tên máy vượt quá độ dài cho phép của CheckInOut.TenMay.", request);
            // Chỉ đổi nhãn Công việc gửi xuống WSS. Không lấy DisplayName của
            // máy làm tên thành phẩm, không sửa tên máy hoặc lịch sử chấm công.
            var jobName = ResolveJobName(selectedProductCode, code => db.HqThanhPhams.AsNoTracking()
                .Where(product => product.Id == code).Select(product => product.Ten).FirstOrDefault());

            using var transaction = db.Database.BeginTransaction();
            ScaleAttendanceTransferService.LockCard(db, request.TheId);
            if (registeredDevice?.WKv == Vars.AppKV.Hq)
            {
                var machines = ScaleAttendanceTransferService.OpenShifts(db, request.TheId)
                    .Where(x => !ScaleAttendanceTransferService.IsExpired(x, time, maxShiftHours))
                    .Select(x => x.MaSoMay).Append(machine);
                ScaleSharedAttendanceService.LockMachines(db, machines);
                var shared = ScaleSharedAttendanceService.Process(db, request, machine, name, time,
                    selectedProductCode, employee, job: jobName, maxShiftHours: maxShiftHours);
                if (shared.Success) transaction.Commit();
                return shared;
            }
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
                request.Cadem = existing.CaDem;
                var duplicate = Success(db, request, existing.ThoiGian, existing.MaSoMay,
                    existing.MaSoMay.ToString(CultureInfo.InvariantCulture), employee, jobName, true, maxShiftHours);
                transaction.Commit();
                return duplicate;
            }

            // Kiểm tra ca trên mọi máy trước khi mở ca mới.
            var newestTime = db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == request.TheId &&
                (x.TrangThai == "Vào" || x.TrangThai == "Ra"))
                .Select(x => (DateTime?)x.ThoiGian).Max();
            if (newestTime > time)
                return Failure("OUT_OF_ORDER", "Lượt quẹt cũ hơn lịch sử. Kiểm tra giờ thiết bị.", request);
            var openShifts = ScaleAttendanceTransferService.OpenShifts(db, request.TheId);
            if (openShifts.Any(x => x.ThoiGian > time))
                return Failure("OUT_OF_ORDER", "Lượt quẹt cũ hơn ca đang mở. Kiểm tra giờ thiết bị.", request);
            var active = openShifts.Where(x => !IsShiftExpired(x.ThoiGian,
                time, maxShiftHours, x.CaDem)).ToList();
            if (active.Count > 1)
                return Failure("MULTIPLE_OPEN_SHIFTS", "Trùng ca nhiều cân. Báo quản lý.", request);
            // Cả Vào và Ra đều phải thực hiện tại máy có ca đang mở.
            if (active.Any(x => x.MaSoMay != machine))
                return Failure("SHIFT_ON_OTHER_MACHINE", OtherMachineMessage(active[0].MaSoMay), request);

            // Ca thiếu Ra ngày cũ dành cho quản lý, không nhắc lại khi Vào ngày mới.
            // Giữ ca đêm còn hạn để không ngắt ca hợp lệ qua nửa đêm.
            var latest = ScaleAttendanceTransferService.OperationalEvents(db, request.TheId, machine, time, maxShiftHours)
                .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id)
                .Select(x => new { x.Id, x.ThoiGian, x.TrangThai, x.CaDem, x.MaXuong }).FirstOrDefault();
            if (latest != null && latest.ThoiGian > time)
                return Failure("OUT_OF_ORDER", "Lượt quẹt cũ hơn lịch sử. Kiểm tra giờ thiết bị.", request);
            var previousStatus = latest?.TrangThai == "Ra" && latest.ThoiGian.Date != time.Date
                ? null : latest?.TrangThai;
            var expiredOpenShift = latest != null && previousStatus == "Vào" &&
                IsShiftExpired(latest.ThoiGian, time, maxShiftHours, latest.CaDem);
            if (expiredOpenShift && request.TrangThai == "Ra")
                return ExpiredShift(request, latest!.ThoiGian, time, maxShiftHours);
            if (!expiredOpenShift && latest != null && previousStatus == request.TrangThai)
            {
                request.Cadem = latest.CaDem;
                // Quẹt lại cùng hướng: trả ca đã lưu, tuyệt đối không lấy giờ
                // quẹt lặp làm giờ Vào/Ra hoặc kéo dài tổng thời gian của ca.
                var recorded = Success(db, request, latest.ThoiGian, machine, device, employee, jobName, false, maxShiftHours);
                transaction.Commit();
                return recorded.Success
                    ? (false, MarkRepeated(recorded.Json, request.TrangThai))
                    : recorded;
            }
            // Ca cũ quá hạn không chặn nhân viên bắt đầu một ca Vào mới.
            var error = ValidateTransition(expiredOpenShift ? null : previousStatus, request.TrangThai);
            if (error != null) return Failure("INVALID_TRANSITION", error, request);

            // Không dùng cờ lúc Ra để thay đổi chính sách của ca đã mở.
            if (request.TrangThai == "Ra") request.Cadem = latest!.CaDem;

            // Tính kết quả trước khi ghi: lỗi tra sản lượng không tạo phiếu nửa chừng.
            var response = Success(db, request, time, machine, device, employee, jobName, false, maxShiftHours);
            if (!response.Success) return response;
            if (expiredOpenShift)
                response = (true, MarkRestarted(response.Json, latest!.ThoiGian));
            db.CheckInOuts.Add(new CheckInOut
            {
                Id = id, MaChamCong = request.TheId, ThoiGian = time, MaSoMay = machine,
                TenMay = name, CongViec = name, Isforget = 0,
                MaXuong = request.TrangThai == "Ra" ? latest!.MaXuong : ScaleWorkshopHistory.Resolve(db, device, time),
                TrangThai = request.TrangThai, CaDem = request.Cadem,
                VaoId = request.TrangThai == "Ra" ? latest!.Id : null
            });
            try { db.SaveChanges(); transaction.Commit(); }
            catch (DbUpdateException errorDb) when (AttendanceRecordId.IsDuplicate(errorDb))
            {
                transaction.Rollback();
                var saved = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == id);
                if (saved is null) throw;
                if (saved.MaChamCong != request.TheId || saved.TrangThai != request.TrangThai)
                    return Failure("ID_CONFLICT", "Id đã được dùng cho lượt quẹt khác.", request);
                request.Cadem = saved.CaDem;
                return Success(db, request, saved.ThoiGian, machine, device, employee, jobName, true, maxShiftHours);
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

    internal static string ResolveJobName(string? selectedProductCode, Func<string, string?> findName)
    {
        var code = selectedProductCode?.Trim();
        if (string.IsNullOrEmpty(code)) return string.Empty;
        return findName(code)?.Trim() ?? string.Empty;
    }

    internal static string ResolveShiftJobNames(IEnumerable<string?> productCodes, Func<string, string?> findName) =>
        string.Join(", ", productCodes.Select(code => code?.Trim() ?? string.Empty)
            .Where(code => code.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(code => ResolveJobName(code, findName)).Where(name => name.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase));

    internal static (bool Success, string Json) Success(dbPMScontext db, Request request,
        DateTime time, int machine, string device, NhanVienDaiThanh employee, string job, bool duplicated,
        int maxShiftHours, CheckInOut? sharedShift = null)
    {
        var totalBaskets = 0;
        var totalWeight = 0m;
        var start = time;
        var shiftStart = time;
        DateTime? end = null;
        var stored = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == request.ChamCongId)
            ?? db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == request.TheId && x.MaSoMay == machine &&
                x.ThoiGian == time && x.TrangThai == request.TrangThai)
                .OrderByDescending(x => x.Id).FirstOrDefault();
        if (stored != null) shiftStart = stored.ThoiGian;
        if (sharedShift != null) start = shiftStart = sharedShift.ThoiGian;
        if (request.TrangThai == "Ra")
        {
            var checkIn = sharedShift ?? (stored?.VaoId != null
                ? db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == stored.VaoId)
                : db.CheckInOuts.AsNoTracking()
                .Where(x => x.MaChamCong == request.TheId && x.MaSoMay == machine &&
                    x.ThoiGian <= time && x.Id != (stored != null ? stored.Id : request.ChamCongId) &&
                    (x.TrangThai == "Vào" || x.TrangThai == "Ra"))
                .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id)
                .FirstOrDefault());
            if (checkIn is null || checkIn.TrangThai != "Vào")
                return Failure("MISSING_CHECK_IN", "Chưa có ca Vào đang mở. Vui lòng quẹt Vào trước.", request);
            start = checkIn.ThoiGian;
            shiftStart = start;
            request.Cadem = checkIn.CaDem;
            // Kiểm tra cả dữ liệu đã lưu/gửi lại để phiếu Ra sai trước đây
            // không tiếp tục trả tổng thời gian hoặc sản lượng nhiều ngày.
            if (IsShiftExpired(start, time, maxShiftHours, checkIn.CaDem))
                return ExpiredShift(request, start, time, maxShiftHours);
            end = time;
        }
        if (request.TrangThai == "Ra" || (sharedShift != null && time > start))
        {
            // Cùng tập phiếu với tổng số rổ/kg; gom mã theo thứ tự phát sinh
            // trong ca, không lấy thành phẩm đang chọn lúc Ra hoặc của người khác.
            var weights = sharedShift == null ? db.HqPhieuCans.AsNoTracking()
                .Where(x => x.MayCan == device && x.TheIdNhanVien == request.TheId &&
                    x.NgayGio >= start && x.NgayGio <= time && x.ChiSanLuong && x.Status == 1)
                : ScaleSharedAttendancePolicy.ShiftWeights(db.HqPhieuCans.AsNoTracking(),
                    db.ScaleAttendanceScans.AsNoTracking(), sharedShift, time, db.CheckInOuts.AsNoTracking());
            var products = weights
                .GroupBy(x => x.MaThanhPham)
                .Select(g => new { Code = g.Key, FirstTime = g.Min(x => x.NgayGio),
                    Count = g.Count(), Weight = g.Sum(x => x.TrongLuong - x.TrongLuongTare) })
                .OrderBy(x => x.FirstTime).ThenBy(x => x.Code).ToList();
            totalBaskets = products.Sum(x => x.Count);
            totalWeight = products.Sum(x => x.Weight);
            var codes = products.Select(x => x.Code?.Trim()).Where(code => !string.IsNullOrEmpty(code))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            var names = db.HqThanhPhams.AsNoTracking().Where(product => codes.Contains(product.Id))
                .Select(product => new { product.Id, product.Ten }).ToList()
                .GroupBy(product => product.Id.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().Ten, StringComparer.OrdinalIgnoreCase);
            if (request.TrangThai == "Ra")
                job = ResolveShiftJobNames(products.Select(x => x.Code), code => names.GetValueOrDefault(code));
        }
        var timing = BuildTiming(start, end, request.Cadem);
        var shiftTiming = BuildTiming(shiftStart, end, request.Cadem);
        return (true, JsonConvert.SerializeObject(new
        {
            request.ChamCongId, request.TheId, MaNhanVien = employee.MaNhanVien,
            TenNhanVien = employee.Name ?? "", TongSoRo = totalBaskets,
            TongTrongLuong = totalWeight, TenCongViec = job,
            timing.ThoiGianVao, timing.ThoiGianRa,
            timing.TongThoiGianGiay, timing.TongThoiGian,
            ThoiGianBatDauCa = shiftTiming.ThoiGianVao,
            TongThoiGianCaGiay = shiftTiming.TongThoiGianGiay,
            TongThoiGianCa = shiftTiming.TongThoiGian,
            // Giữ khóa JSON cũ để firmware tương thích; không cần cột SQL MayQuetRa.
            MayDangLam = machine, MayQuetRa = (int?)null, WarningCode = (string?)null,
            request.TrangThai, request.Cadem, IsValid = true, Duplicated = duplicated,
            MessStr = duplicated
                ? $"Đã quẹt {request.TrangThai}{(request.Cadem ? " ca đêm" : "")} rồi."
                : $"Đã chấm công {request.TrangThai}{(request.Cadem ? " ca đêm" : "")} thành công."
        }));
    }

    internal static bool IsShiftExpired(DateTime start, DateTime end, int maxShiftHours, bool caDem = false) =>
        (!caDem && start.Date != end.Date) || end - start > TimeSpan.FromHours(maxShiftHours);

    internal static string OtherMachineMessage(int machine) =>
        $"Đang ở cân {machine}. Ra cân cũ.";

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
            MessStr = $"Ca đã hết hạn (ca thường chốt cuối ngày, tối đa {maxShiftHours} giờ). Cần bổ sung giờ Ra; quẹt Vào để bắt đầu ca mới."
        }));

    internal static string MarkRestarted(string responseJson, DateTime previousStart)
    {
        var result = JObject.Parse(responseJson);
        result["WarningCode"] = "PREVIOUS_SHIFT_MISSING_OUT";
        result["CanBoSungGioRa"] = true;
        result["ThoiGianVaoCaTruoc"] = previousStart.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        result["MessStr"] = result.Value<bool>("Cadem")
            ? "Đã quẹt Vào ca đêm mới. Ca trước thiếu giờ Ra, cần bổ sung."
            : "Đã quẹt Vào ca mới. Ca trước thiếu giờ Ra, cần bổ sung.";
        return result.ToString(Formatting.None);
    }

    internal static string MarkRepeated(string recordedJson, string status)
    {
        var result = JObject.Parse(recordedJson);
        result["IsValid"] = false;
        result["AlreadyRecorded"] = true;
        result["Code"] = status == "Vào" ? "ALREADY_CHECKED_IN" : "ALREADY_CHECKED_OUT";
        result["MessStr"] = $"Đã quẹt {status}{(result.Value<bool>("Cadem") ? " ca đêm" : "")} rồi.";
        return result.ToString(Formatting.None);
    }

    internal static AttendanceTiming BuildTiming(DateTime start, DateTime? end, bool caDem = false)
    {
        if (end.HasValue && end.Value < start)
            throw new ArgumentException("Giờ Ra không được trước giờ Vào.");
        if (!caDem && end.HasValue && end.Value.Date != start.Date)
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
        current == "Ra" && previous != "Vào" ? "Chưa Vào. Quẹt Vào trước." :
        current == "Vào" && previous == "Vào" ? "Đã chấm Vào, chưa chấm Ra." : null;

    internal static (bool Success, string Json) Failure(string code, string message, Request? request = null) =>
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
        public bool Cadem { get; set; }
    }
}
