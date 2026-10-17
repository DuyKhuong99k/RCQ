using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using static Services.ScaleAttendanceProcessor;

namespace Services;

/// <summary>Lưu một ca tính giờ và các lượt tham gia ca trên nhiều cân HQ.</summary>
internal static class ScaleSharedAttendanceService
{
    internal static void LockMachines(dbPMScontext db, IEnumerable<int> machines)
    {
        foreach (var machine in machines.Distinct().OrderBy(x => x))
        {
            var resource = "ScaleAttendance:device:" + machine.ToString(CultureInfo.InvariantCulture);
            db.Database.ExecuteSqlInterpolated($@"
                DECLARE @result int;
                EXEC @result = sys.sp_getapplock @Resource = {resource},
                    @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 5000;
                IF @result < 0 THROW 51001, N'Không thể khóa máy cân.', 1;");
        }
    }

    internal static DateTime? LatestScanTime(dbPMScontext db, string card) =>
        (from scan in db.ScaleAttendanceScans.AsNoTracking()
         join shift in db.CheckInOuts.AsNoTracking() on scan.VaoId equals shift.Id
         where shift.MaChamCong == card
         select (DateTime?)scan.ThoiGian).Max();

    internal static string? ProductAtStart(dbPMScontext db, CheckInOut shift, DateTime time)
    {
        var product = db.ScaleAttendanceScans.AsNoTracking().Where(x => x.Id == shift.Id)
            .Select(x => x.MaThanhPham).FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(product)) return product;
        // Ca trước khi nâng cấp: chỉ nhận lịch sử sản lượng có đúng một mã.
        var device = shift.MaSoMay.ToString(CultureInfo.InvariantCulture);
        var products = db.HqPhieuCans.AsNoTracking().Where(x => x.MayCan == device &&
                x.TheIdNhanVien == shift.MaChamCong && x.NgayGio >= shift.ThoiGian &&
                x.NgayGio <= time && x.ChiSanLuong && x.Status == 1)
            .Select(x => x.MaThanhPham).Distinct().Take(2).ToList();
        return products.Count == 1 ? products[0].Trim() : null;
    }

    // Caller giữ transaction và khóa card -> các máy theo thứ tự tăng dần.
    internal static (bool Success, string Json) Process(dbPMScontext db, Request request,
        int machine, string name, DateTime time, string? selectedProduct,
        NhanVienDaiThanh employee, string job, int maxShiftHours)
    {
        var id = request.ChamCongId!;
        var card = request.TheId!;
        var device = machine.ToString(CultureInfo.InvariantCulture);
        var product = selectedProduct?.Trim();
        if (product?.Length > 50)
            return Failure("INVALID_PRODUCT", "Mã thành phẩm vượt quá 50 ký tự.", request);
        var saved = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == id);
        var recorded = db.ScaleAttendanceScans.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (saved != null || recorded != null)
        {
            var rootId = recorded?.VaoId ?? (saved!.TrangThai == "Vào" ? saved.Id : saved.VaoId);
            var root = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == rootId);
            if ((saved != null && (saved.MaChamCong != card || saved.TrangThai != request.TrangThai)) ||
                (recorded != null && (recorded.MaSoMay != machine || recorded.TrangThai != request.TrangThai ||
                    root?.MaChamCong != card)))
                return Failure("ID_CONFLICT", "Id đã được dùng cho lượt quẹt khác.", request);
            request.Cadem = root?.CaDem ?? saved!.CaDem;
            if (request.TrangThai == "Vào" && recorded != null)
                job = ResolveJobName(recorded.MaThanhPham, code => db.HqThanhPhams.AsNoTracking()
                    .Where(x => x.Id == code).Select(x => x.Ten).FirstOrDefault());
            return Success(db, request, recorded?.ThoiGian ?? saved!.ThoiGian, machine, device,
                employee, job, true, maxShiftHours, root);
        }

        var latestTime = db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == card &&
                (x.TrangThai == "Vào" || x.TrangThai == "Ra"))
            .Select(x => (DateTime?)x.ThoiGian).Max();
        if (latestTime > time || LatestScanTime(db, card) > time)
            return Failure("OUT_OF_ORDER", "Lượt quẹt cũ hơn lịch sử. Kiểm tra giờ thiết bị.", request);
        var open = ScaleAttendanceTransferService.OpenShifts(db, card);
        var active = open.Where(x => !ScaleAttendanceTransferService.IsExpired(x, time, maxShiftHours)).ToList();
        if (active.Count > 1)
            return Failure("MULTIPLE_OPEN_SHIFTS", "Trùng ca nhiều cân. Báo quản lý.", request);
        var shift = active.SingleOrDefault();

        if (shift != null)
        {
            if (shift.Isforget != 0)
                return Failure("INVALID_SHIFT", "Ca đang lỗi. Nhờ quản lý kiểm tra.", request);
            if (request.TrangThai == "Ra")
            {
                var participated = shift.MaSoMay == machine || db.ScaleAttendanceScans.Any(x =>
                    x.VaoId == shift.Id && x.MaSoMay == machine && x.TrangThai == "Vào" && x.ThoiGian <= time);
                if (!participated)
                    return Failure("MISSING_CHECK_IN", "Chưa quẹt Vào ca chung tại cân này.", request);
                request.Cadem = shift.CaDem;
                var response = Success(db, request, time, machine, device, employee, job, false, maxShiftHours, shift);
                if (!response.Success) return response;
                db.CheckInOuts.Add(Checkout(shift, id, time));
                db.ScaleAttendanceScans.Add(Scan(request, shift.Id, machine, time, ProductAtStart(db, shift, time)));
                db.SaveChanges();
                return response;
            }
            var oldProduct = ProductAtStart(db, shift, time);
            if (shift.MaSoMay == machine &&
                (string.IsNullOrWhiteSpace(oldProduct) || string.IsNullOrWhiteSpace(product) ||
                 ScaleSharedAttendancePolicy.SameProduct(oldProduct, product)))
            {
                request.Cadem = shift.CaDem;
                var repeated = Success(db, request, time, machine, device, employee, job, false, maxShiftHours, shift);
                return repeated.Success ? (false, MarkRepeated(repeated.Json, "Vào")) : repeated;
            }
            var error = ScaleSharedAttendancePolicy.ValidateContinuation(shift, oldProduct, product, time, maxShiftHours);
            if (error != null) return Failure("INVALID_TRANSFER", error, request);
            if (!db.HqThanhPhams.AsNoTracking().Any(x => x.Id == product))
                return Failure("INVALID_PRODUCT", "Thành phẩm đang chọn không có trong danh mục HQ.", request);
            RememberLegacyProduct(db, shift, oldProduct!);
            request.Cadem = shift.CaDem;
            if (ScaleSharedAttendancePolicy.SameProduct(oldProduct, product))
            {
                var response = Success(db, request, time, machine, device, employee, job, false, maxShiftHours, shift);
                if (!response.Success) return response;
                db.ScaleAttendanceScans.Add(Scan(request, shift.Id, machine, time, oldProduct));
                db.SaveChanges();
                return response;
            }
            // Id riêng cho Ra tự động: cho phép đóng/mở cùng giây tại máy gốc,
            // không chiếm Id lượt quẹt thật của nhân viên khác trên cân đó.
            var outId = shift.Id + ".AUTO." + time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            if (db.CheckInOuts.Any(x => x.Id == outId) || db.ScaleAttendanceScans.Any(x => x.Id == outId))
                return Failure("ID_CONFLICT", "Cân cũ đã có lượt tại giây chuyển. Thử lại sau một giây.", request);
            var checkout = Checkout(shift, outId, time);
            checkout.LyDo = "Tự động đóng ca khi quẹt Vào cân khác mã thành phẩm.";
            db.CheckInOuts.Add(checkout);
        }
        else if (request.TrangThai == "Ra")
        {
            var last = (from scan in db.ScaleAttendanceScans.AsNoTracking()
                        join root in db.CheckInOuts.AsNoTracking() on scan.VaoId equals root.Id
                        where root.MaChamCong == card && scan.MaSoMay == machine && scan.ThoiGian <= time
                        orderby scan.ThoiGian descending, scan.Id descending
                        select scan).FirstOrDefault();
            var previous = last == null ? null : db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == last.VaoId);
            if (last?.TrangThai == "Ra" && previous != null &&
                ScaleAttendanceTransferService.IsInOperationalDay(previous, time, maxShiftHours))
            {
                request.Cadem = previous.CaDem;
                var repeated = Success(db, request, last.ThoiGian, machine, device, employee, job, false, maxShiftHours, previous);
                return repeated.Success ? (false, MarkRepeated(repeated.Json, "Ra")) : repeated;
            }
            if (last == null)
            {
                var legacy = ScaleAttendanceTransferService.OperationalEvents(db, card, machine, time, maxShiftHours)
                    .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id).FirstOrDefault();
                if (legacy?.TrangThai == "Ra")
                {
                    request.Cadem = legacy.CaDem;
                    var repeated = Success(db, request, legacy.ThoiGian, machine, device, employee, job, false, maxShiftHours);
                    return repeated.Success ? (false, MarkRepeated(repeated.Json, "Ra")) : repeated;
                }
            }
            var expired = open.FirstOrDefault(x => x.MaSoMay == machine || x.Id == last?.VaoId);
            if (expired != null && ScaleAttendanceTransferService.IsInOperationalDay(expired, time, maxShiftHours))
                return ExpiredShift(request, expired.ThoiGian, time, maxShiftHours);
            return Failure("MISSING_CHECK_IN", "Chưa Vào. Quẹt Vào trước.", request);
        }

        var newShift = new CheckInOut
        {
            Id = id, MaChamCong = card, ThoiGian = time, MaSoMay = machine,
            TenMay = name, CongViec = name, Isforget = 0, TrangThai = "Vào", CaDem = request.Cadem,
            MaXuong = ScaleWorkshopHistory.Resolve(db, device, time)
        };
        var started = Success(db, request, time, machine, device, employee, job, false, maxShiftHours, newShift);
        if (!started.Success) return started;
        var expiredHere = open.FirstOrDefault(x => x.MaSoMay == machine && x.ThoiGian.Date == time.Date);
        if (shift == null && expiredHere != null) started = (true, MarkRestarted(started.Json, expiredHere.ThoiGian));
        db.CheckInOuts.Add(newShift);
        db.ScaleAttendanceScans.Add(Scan(request, id, machine, time, product));
        db.SaveChanges();
        return started;
    }

    internal static CheckInOut Checkout(CheckInOut shift, string id, DateTime time) => new()
    {
        // Máy tính giờ giữ theo ca gốc; máy quẹt thực tế nằm trong ScaleAttendanceScan.
        Id = id, MaChamCong = shift.MaChamCong, MaSoMay = shift.MaSoMay,
        TenMay = shift.TenMay, CongViec = shift.CongViec, ThoiGian = time,
        TrangThai = "Ra", CaDem = shift.CaDem, VaoId = shift.Id, MaXuong = shift.MaXuong
    };

    private static void RememberLegacyProduct(dbPMScontext db, CheckInOut shift, string product)
    {
        var snapshot = db.ScaleAttendanceScans.FirstOrDefault(x => x.Id == shift.Id);
        if (snapshot == null)
            db.ScaleAttendanceScans.Add(new ScaleAttendanceScan { Id = shift.Id, VaoId = shift.Id,
                MaSoMay = shift.MaSoMay, ThoiGian = shift.ThoiGian, TrangThai = "Vào", MaThanhPham = product });
        else if (string.IsNullOrWhiteSpace(snapshot.MaThanhPham))
            snapshot.MaThanhPham = product;
    }

    private static ScaleAttendanceScan Scan(Request request, string rootId, int machine, DateTime time, string? product) => new()
    {
        Id = request.ChamCongId!, VaoId = rootId, MaSoMay = machine,
        ThoiGian = time, TrangThai = request.TrangThai!, MaThanhPham = product
    };
}
