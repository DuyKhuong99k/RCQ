using Models.Repos;
using Models.Repos.Models;
using Microsoft.EntityFrameworkCore;

namespace Services;

/// <summary>Nhân viên và thẻ nhóm đều cần ca Vào hợp lệ để ghi sản lượng online.</summary>
public static class ScaleProductionAttendancePolicy
{
    private const string InvalidProductionCard = "Thẻ không thuộc nhân viên hoặc nhóm sản xuất hợp lệ.";
    private const string BeforeLatestAttendance = "Giờ cân trước lượt chấm công. Kiểm tra giờ máy.";

    // Chỉ tin cờ trong danh mục SQL, không tin mã/tên nhóm do firmware gửi lên.
    public static bool IsProductionCard(NhanVienDaiThanh? employee) => employee != null
        && (employee.IsHuman || employee.IsNhom) && !employee.IsChucNang && !employee.IsPhucVu;

    // Same online policy, but no synchronous SQL waits or redundant employee lookup.
    public static async Task<string?> GetOnlineErrorAsync(dbPMScontext db, string card, DateTime time,
        int maxShiftHours, NhanVienDaiThanh employee, ScaleWorkContext selected, CancellationToken cancellation)
    {
        if (!IsProductionCard(employee)) return InvalidProductionCard;
        var latest = await AttendanceTimes(db, card).MaxAsync(cancellation);
        if (latest > time)
        {
            HqFirmwareSend.Log($"[HQ ATTENDANCE REJECT] Time={time:O}; Latest={latest:O}; Product={selected.Product}; Material={selected.Material}; Workshop={selected.Workshop}; Reason=BEFORE_LAST_SCAN");
            return BeforeLatestAttendance;
        }
        var shifts = await ScaleOnlineAttendanceService.OpenShiftsAsync(db, card, time, maxShiftHours, cancellation);
        var ids = shifts.Select(x => x.Id).ToArray();
        var snapshots = ids.Length == 0 ? new List<ScaleAttendanceScan>() :
            await InitialSnapshots(db, ids).ToListAsync(cancellation);
        var error = ValidateOnlineAtCurrentTime(shifts, snapshots, selected, time, maxShiftHours);
        if (error != null)
            HqFirmwareSend.Log($"[HQ ATTENDANCE REJECT] Time={time:O}; Product={selected.Product}; Material={selected.Material}; Workshop={selected.Workshop}; Roots={string.Join(',', shifts.Select(x => x.Id))}; Snapshots={string.Join(';', snapshots.Select(x => $"{x.Id}:{x.MaThanhPham}/{x.MaLoaiNguyenLieu}/{x.MaXuong}"))}; Reason={error}");
        return error;
    }

    public static string? GetError(dbPMScontext db, string card, int machine, DateTime time, int maxShiftHours,
        NhanVienDaiThanh? preloadedEmployee = null, string? selectedProductCode = null,
        string? selectedMaterialCode = null, string? selectedWorkshop = null)
    {
        var employee = preloadedEmployee;
        if (employee == null)
        {
            var employeeCode = db.TheTu.AsNoTracking().Where(x => x.MaTheTu == card)
                .Select(x => x.MaNhanVien).FirstOrDefault();
            employee = string.IsNullOrWhiteSpace(employeeCode) ? null : db.NhanVienDaiThanh.AsNoTracking()
                .FirstOrDefault(x => x.MaNhanVien == employeeCode);
        }
        if (!IsProductionCard(employee)) return InvalidProductionCard;

        if (AttendanceTimes(db, card).Max() > time) return BeforeLatestAttendance;
        if (selectedMaterialCode != null || selectedWorkshop != null)
        {
            var context = ScaleWorkContext.Create(selectedProductCode, selectedMaterialCode, selectedWorkshop);
            var onlineShifts = ScaleOnlineAttendanceService.OpenShifts(db, card, time, maxShiftHours);
            var ids = onlineShifts.Select(x => x.Id).ToArray();
            var initial = ids.Length == 0 ? new List<ScaleAttendanceScan>() : InitialSnapshots(db, ids).ToList();
            return ValidateOnlineAtCurrentTime(onlineShifts, initial, context, time, maxShiftHours);
        }
        var latestAtMachine = ScaleAttendanceTransferService.OperationalEvents(db, card, machine, time, maxShiftHours)
            .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id)
            .FirstOrDefault();
        var shifts = ScaleAttendanceTransferService.OpenShifts(db, card);
        var rootIds = shifts.Select(x => x.Id).ToArray();
        var scans = db.ScaleAttendanceScans.AsNoTracking().Where(x => rootIds.Contains(x.VaoId) &&
            x.MaSoMay == machine && x.TrangThai == "Vào" && x.ThoiGian <= time).ToList();
        var error = ValidateShared(shifts, scans, machine, time, maxShiftHours, latestAtMachine, selectedProductCode);
        if (error != null) return error;
        // Máy chậm giờ không được dùng giờ cũ để tiếp tục ca đã hết hạn.
        var now = ScaleWorkshopHistory.BusinessNow();
        return ValidateShared(shifts, scans, machine, now > time ? now : time, maxShiftHours, latestAtMachine, selectedProductCode);
    }

    // One SQL round trip for both timelines. UNION ALL preserves both ordering guards.
    private static IQueryable<DateTime?> AttendanceTimes(dbPMScontext db, string card)
    {
        var events = db.CheckInOuts.AsNoTracking().Where(x => x.MaChamCong == card &&
            (x.TrangThai == "Vào" || x.TrangThai == "Ra")).Select(x => (DateTime?)x.ThoiGian);
        var scans = from scan in db.ScaleAttendanceScans.AsNoTracking()
                    join root in db.CheckInOuts.AsNoTracking() on scan.VaoId equals root.Id
                    where root.MaChamCong == card
                    select (DateTime?)scan.ThoiGian;
        return events.Concat(scans);
    }

    private static IQueryable<ScaleAttendanceScan> InitialSnapshots(dbPMScontext db, string[] rootIds) =>
        db.ScaleAttendanceScans.AsNoTracking().Where(x => rootIds.Contains(x.Id));

    private static string? ValidateOnlineAtCurrentTime(IReadOnlyList<CheckInOut> shifts,
        IReadOnlyList<ScaleAttendanceScan> snapshots, ScaleWorkContext selected, DateTime time, int maxShiftHours)
    {
        var error = ValidateOnline(shifts, snapshots, selected, time, maxShiftHours);
        if (error != null) return error;
        // Validate the device timestamp AND current business time; a slow clock cannot extend a shift.
        var now = ScaleWorkshopHistory.BusinessNow();
        return ValidateOnline(shifts, snapshots, selected, now > time ? now : time, maxShiftHours);
    }

    public static string? ValidateOnline(IReadOnlyList<CheckInOut> shifts,
        IReadOnlyList<ScaleAttendanceScan> snapshots, ScaleWorkContext selected, DateTime time, int maxShiftHours)
    {
        if (maxShiftHours is < 1 or > 24) return ScaleOnlineMessages.InvalidConfiguration;
        if (!selected.IsComplete) return ScaleOnlineMessages.IncompleteContext(selected);
        if (shifts.Any(x => x.ThoiGian > time)) return ScaleOnlineMessages.BeforeCheckIn;
        var active = shifts.Where(x => !ScaleAttendanceTransferService.IsExpired(x, time, maxShiftHours)).ToList();
        if (active.Count > 1) return ScaleOnlineMessages.MultipleShifts;
        if (active.Count == 0) return ScaleOnlineMessages.MissingCheckIn;
        var shift = active[0];
        if (shift.Isforget != 0) return ScaleOnlineMessages.InvalidShift;
        var snapshot = snapshots.FirstOrDefault(x => x.Id == shift.Id);
        var original = ScaleWorkContext.FromSnapshot(snapshot);
        if (original == null) return ScaleOnlineAttendanceService.MissingSnapshot;
        return ScaleSharedAttendancePolicy.CanUseContext(shift, snapshot, time, selected) ? null :
            ScaleOnlineMessages.ContextMismatch(original, selected);
    }

    internal static string? ValidateShared(IReadOnlyList<CheckInOut> shifts, IReadOnlyList<ScaleAttendanceScan> scans,
        int machine, DateTime time, int maxShiftHours, CheckInOut? latestAtMachine, string? product)
    {
        var active = shifts.Where(x => !ScaleAttendanceTransferService.IsExpired(x, time, maxShiftHours)).ToList();
        if (active.Count == 1)
        {
            if (!ScaleSharedAttendancePolicy.CanUseMachine(active[0], scans, machine, time, product))
                return "Chưa quẹt Vào ca chung với thành phẩm này tại cân.";
            machine = active[0].MaSoMay;
        }
        return Validate(shifts, machine, time, maxShiftHours, latestAtMachine);
    }

    internal static string? Validate(IReadOnlyList<CheckInOut> openShifts, int machine, DateTime time,
        int maxShiftHours, CheckInOut? latestAtMachine = null)
    {
        if (maxShiftHours is < 1 or > 24)
            return "Cấu hình ca không hợp lệ. Nhờ quản lý kiểm tra.";
        if (openShifts.Any(x => x.ThoiGian > time))
            return "Giờ cân trước lượt Vào. Kiểm tra giờ máy.";
        openShifts = openShifts.Where(x => ScaleAttendanceTransferService.IsInOperationalDay(x, time, maxShiftHours)).ToArray();
        if (latestAtMachine != null && !ScaleAttendanceTransferService.IsInOperationalDay(latestAtMachine, time, maxShiftHours))
            latestAtMachine = null;
        var active = openShifts.Where(x => !ScaleAttendanceTransferService.IsExpired(x, time, maxShiftHours)).ToList();
        if (active.Count > 1)
            return "Trùng ca nhiều cân. Báo quản lý.";
        if (active.Count == 0)
        {
            // Ca cũ tại máy khác vẫn còn để đối soát, nhưng không dùng làm
            // trạng thái của máy đang cân. Không xóa ca hoặc tự tạo giờ Ra.
            if (latestAtMachine?.MaSoMay == machine && latestAtMachine.TrangThai == "Ra")
                return "Quẹt Vào lại để tiếp tục cân.";
            return openShifts.Any(x => x.MaSoMay == machine)
                ? "Ca đã hết hạn. Quẹt Vào lại tại cân."
                : "Chưa quẹt Vào tại cân.";
        }
        if (active[0].MaSoMay != machine)
            return ScaleAttendanceProcessor.OtherMachineMessage(active[0].MaSoMay);
        if (active[0].Isforget != 0)
            return "Ca đang lỗi. Nhờ quản lý kiểm tra.";
        return null;
    }
}
