using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using static Services.ScaleAttendanceProcessor;
using OnlineTotals = Dao.Repos.HQ.HQ_PhieuCan.OnlineTotals;

namespace Services;

/// <summary>Ca HQ online theo thành phẩm/nguyên liệu/xưởng, không theo máy quẹt.</summary>
public static class ScaleOnlineAttendanceService
{
    public const string MissingSnapshot = ScaleOnlineMessages.MissingSnapshot;

    public static List<CheckInOut> OpenShifts(dbPMScontext db, string card, DateTime time, int hours) =>
        OpenShiftsQuery(db, card, time, hours).ToList();

    public static Task<List<CheckInOut>> OpenShiftsAsync(dbPMScontext db, string card, DateTime time, int hours,
        CancellationToken cancellation) => OpenShiftsQuery(db, card, time, hours).ToListAsync(cancellation);

    private static IQueryable<CheckInOut> OpenShiftsQuery(dbPMScontext db, string card, DateTime time, int hours) =>
        ScaleAttendanceTransferService.TransferableShifts(db.CheckInOuts.AsNoTracking()
                .Where(x => x.MaChamCong == card), time, hours)
            .Where(x => x.ThoiGian <= time).OrderByDescending(x => x.ThoiGian).Take(3);

    public static ScaleAttendanceScan? Snapshot(dbPMScontext db, string rootId) =>
        db.ScaleAttendanceScans.AsNoTracking().FirstOrDefault(x => x.Id == rootId);

    public static OnlineTotals Totals(dbPMScontext db, HQ_PhieuCan ticket, int hours)
    {
        var day = ticket.NgayGio.Date;
        var context = ScaleWorkContext.Create(ticket.MaThanhPham, ticket.MaLoaiNguyenLieu, ticket.MaXuong);
        var overnight = OvernightShift(TicketShifts(db, ticket, hours).ToList(), day);
        var snapshot = overnight == null ? null : Snapshot(db, overnight.Id);
        var from = TotalsStart(day, context, overnight, snapshot);
        return TotalsQuery(db, ticket, context, from, day.AddDays(1)).FirstOrDefault() ?? new OnlineTotals();
    }

    public static async Task<OnlineTotals> TotalsAsync(dbPMScontext db,
        HQ_PhieuCan ticket, int hours, CancellationToken cancellation = default, bool useAttendance = true)
    {
        var day = ticket.NgayGio.Date;
        var context = ScaleWorkContext.Create(ticket.MaThanhPham, ticket.MaLoaiNguyenLieu, ticket.MaXuong);
        var from = day;
        if (useAttendance)
        {
            var roots = await TicketShifts(db, ticket, hours).ToListAsync(cancellation);
            var overnight = OvernightShift(roots, day);
            var snapshot = overnight == null ? null : await db.ScaleAttendanceScans.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == overnight.Id, cancellation);
            from = TotalsStart(day, context, overnight, snapshot);
        }
        return await TotalsQuery(db, ticket, context, from, day.AddDays(1)).FirstOrDefaultAsync(cancellation)
            ?? new OnlineTotals();
    }

    private static IQueryable<CheckInOut> TicketShifts(dbPMScontext db, HQ_PhieuCan ticket, int hours)
    {
        var day = ticket.NgayGio.Date;
        return ScaleSharedAttendancePolicy.ShiftsAtTime(db.CheckInOuts.AsNoTracking(),
                ticket.TheIdNhanVien, ticket.NgayGio)
            .Where(x => x.ThoiGian >= ticket.NgayGio.AddHours(-hours) &&
                (x.CaDem || x.ThoiGian >= day)).Take(2);
    }

    private static CheckInOut? OvernightShift(IReadOnlyList<CheckInOut> roots, DateTime day) =>
        roots.Count == 1 && roots[0].CaDem && roots[0].ThoiGian.Date < day ? roots[0] : null;

    // Same net-kg/day rule as offline, across all scales sharing the context.
    // A single matching night shift extends the totals window across midnight.
    private static DateTime TotalsStart(DateTime day, ScaleWorkContext context,
        CheckInOut? overnight, ScaleAttendanceScan? snapshot) =>
        overnight != null && context.Matches(ScaleWorkContext.FromSnapshot(snapshot)) ? overnight.ThoiGian : day;

    private static IQueryable<OnlineTotals> TotalsQuery(dbPMScontext db,
        HQ_PhieuCan ticket, ScaleWorkContext context, DateTime from, DateTime dayEnd)
    {
        return db.HqPhieuCans.AsNoTracking().Where(x => x.NgayGio >= from && x.NgayGio < dayEnd &&
            x.MaThanhPham == context.Product && x.MaLoaiNguyenLieu == context.Material &&
            x.MaXuong == context.Workshop && x.ChiSanLuong && x.Status == 1)
            .GroupBy(_ => 1).Select(g => new OnlineTotals
        {
            // Legacy JSON names; totals cover the work context/workshop, not a single scale.
            MachineCount = g.Count(), MachineWeight = g.Sum(x => x.TrongLuong),
            EmployeeCount = g.Count(x => x.MaNhanVien == ticket.MaNhanVien),
            EmployeeWeight = g.Sum(x => x.MaNhanVien == ticket.MaNhanVien ? x.TrongLuong : 0m)
        });
    }

    // Caller giữ transaction và khóa card -> device. Không gọi từ API đồng bộ offline.
    public static (bool Success, string Json) Process(dbPMScontext db, Request request,
        int machine, string name, DateTime time, ScaleWorkContext selected,
        NhanVienDaiThanh employee, string job, int maxShiftHours)
    {
        var card = request.TheId!;
        var id = request.ChamCongId!;
        var device = machine.ToString(CultureInfo.InvariantCulture);
        var saved = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == id);
        var scan = db.ScaleAttendanceScans.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (saved != null || scan != null)
        {
            var rootId = scan?.VaoId ?? (saved!.TrangThai == "Vào" ? saved.Id : saved.VaoId);
            var root = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == rootId);
            if (root == null || root.MaChamCong != card ||
                (saved != null && (saved.MaChamCong != card || saved.TrangThai != request.TrangThai)) ||
                (scan != null && (scan.MaSoMay != machine || scan.TrangThai != request.TrangThai)))
                return Failure("ID_CONFLICT", ScaleOnlineMessages.ScanConflict, request);
            request.Cadem = root.CaDem;
            if (request.TrangThai == "Vào")
            {
                var originalProduct = Snapshot(db, root.Id)?.MaThanhPham;
                job = ResolveJobName(originalProduct, code => db.HqThanhPhams.AsNoTracking()
                    .Where(x => x.Id == code).Select(x => x.Ten).FirstOrDefault());
            }
            // Gửi lại cùng Id xác nhận kết quả gốc dù cấu hình cân đã đổi.
            return Success(db, request, scan?.ThoiGian ?? saved!.ThoiGian, machine, device,
                employee, job, true, maxShiftHours, root);
        }

        var newest = db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == card &&
                (x.TrangThai == "Vào" || x.TrangThai == "Ra"))
            .Select(x => (DateTime?)x.ThoiGian).Max();
        if (newest > time || ScaleSharedAttendanceService.LatestScanTime(db, card) > time)
            return Failure("OUT_OF_ORDER", ScaleOnlineMessages.OutOfOrder, request);

        var active = OpenShifts(db, card, time, maxShiftHours);
        if (active.Count > 1)
            return Failure("MULTIPLE_OPEN_SHIFTS", ScaleOnlineMessages.MultipleShifts, request);
        var current = active.SingleOrDefault();
        if (request.TrangThai == "Ra")
        {
            if (current == null)
            {
                var last = db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == card &&
                        (x.TrangThai == "Vào" || x.TrangThai == "Ra") && x.ThoiGian <= time)
                    .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id).FirstOrDefault();
                if (last?.TrangThai == "Ra" && last.VaoId != null)
                {
                    var previous = db.CheckInOuts.AsNoTracking().FirstOrDefault(x => x.Id == last.VaoId);
                    if (previous != null && !ScaleAttendanceTransferService.IsExpired(previous, time, maxShiftHours))
                    {
                        request.Cadem = previous.CaDem;
                        var response = Success(db, request, last.ThoiGian, machine, device, employee,
                            job, false, maxShiftHours, previous);
                        return response.Success ? (false, MarkRepeated(response.Json, "Ra")) : response;
                    }
                }
                return Failure("MISSING_CHECK_IN", ScaleOnlineMessages.MissingCheckIn, request);
            }
            if (current.Isforget != 0)
                return Failure("INVALID_SHIFT", ScaleOnlineMessages.InvalidShift, request);
            if (time <= current.ThoiGian)
                return Failure("INVALID_TIME", ScaleOnlineMessages.CheckoutTime, request);
            request.Cadem = current.CaDem;
            var result = Success(db, request, time, machine, device, employee, job, false, maxShiftHours, current);
            if (!result.Success) return result;
            var checkout = ScaleSharedAttendanceService.Checkout(current, id, time);
            db.CheckInOuts.Add(checkout);
            // MaSoMay trong CheckInOut giữ máy gốc để báo cáo cũ ghép giờ;
            // máy quẹt Ra thực tế luôn được lưu trong ScaleAttendanceScan.
            db.ScaleAttendanceScans.Add(CreateScan(id, current.Id, machine, time, "Ra",
                ScaleWorkContext.FromSnapshot(Snapshot(db, current.Id))));
            db.SaveChanges();
            return result;
        }

        if (!selected.IsComplete)
            return Failure("MISSING_WORK_CONTEXT", ScaleOnlineMessages.IncompleteContext(selected), request);
        if (!db.HqThanhPhams.AsNoTracking().Any(x => x.Id == selected.Product && x.SuDung))
            return Failure("INVALID_WORK_CONTEXT", ScaleOnlineMessages.InvalidProduct, request);
        if (!db.HqLoaiNguyenLieus.AsNoTracking().Any(x => x.Id == selected.Material && x.SuDung))
            return Failure("INVALID_WORK_CONTEXT", ScaleOnlineMessages.InvalidMaterial, request);

        if (current != null)
        {
            if (current.Isforget != 0)
                return Failure("INVALID_SHIFT", ScaleOnlineMessages.InvalidShift, request);
            var initial = ScaleWorkContext.FromSnapshot(Snapshot(db, current.Id));
            if (initial == null) return Failure("MISSING_SHIFT_SNAPSHOT", MissingSnapshot, request);
            request.Cadem = current.CaDem;
            if (ScaleSharedAttendancePolicy.SelectContextAction(initial, selected) ==
                ScaleSharedAttendancePolicy.CheckInAction.Repeat)
            {
                var repeated = Success(db, request, time, machine, device, employee, job, false, maxShiftHours, current);
                return repeated.Success ? (false, MarkRepeated(repeated.Json, "Vào")) : repeated;
            }
            if (time <= current.ThoiGian)
                return Failure("INVALID_TIME", ScaleOnlineMessages.SwitchTime, request);
            var outId = current.Id + ".AUTO." + time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            if (db.CheckInOuts.Any(x => x.Id == outId) || db.ScaleAttendanceScans.Any(x => x.Id == outId))
                return Failure("ID_CONFLICT", ScaleOnlineMessages.SwitchConflict, request);
            var checkout = ScaleSharedAttendanceService.Checkout(current, outId, time);
            checkout.LyDo = "Tự đóng ca khi quẹt Vào khác mã thành phẩm, nguyên liệu hoặc xưởng.";
            db.CheckInOuts.Add(checkout);
            db.ScaleAttendanceScans.Add(CreateScan(outId, current.Id, machine, time, "Ra", initial));
        }

        var newShift = new CheckInOut
        {
            Id = id, MaChamCong = card, ThoiGian = time, MaSoMay = machine, TenMay = name,
            CongViec = name, Isforget = 0, TrangThai = "Vào", CaDem = request.Cadem, MaXuong = selected.Workshop
        };
        var started = Success(db, request, time, machine, device, employee, job, false, maxShiftHours, newShift);
        if (!started.Success) return started;
        db.CheckInOuts.Add(newShift);
        db.ScaleAttendanceScans.Add(CreateScan(id, id, machine, time, "Vào", selected));
        db.SaveChanges();
        return started;
    }

    internal static ScaleAttendanceScan CreateScan(string id, string rootId, int machine,
        DateTime time, string status, ScaleWorkContext? context) => new()
    {
        Id = id, VaoId = rootId, MaSoMay = machine, ThoiGian = time, TrangThai = status,
        MaThanhPham = context?.Product, MaLoaiNguyenLieu = context?.Material, MaXuong = context?.Workshop
    };
}
