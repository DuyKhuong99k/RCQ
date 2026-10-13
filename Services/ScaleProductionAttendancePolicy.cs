using Models.Repos;
using Models.Repos.Models;
using Microsoft.EntityFrameworkCore;

namespace Services;

/// <summary>Phiếu cân online phải thuộc ca Vào còn mở tại chính máy cân.</summary>
public static class ScaleProductionAttendancePolicy
{
    public static string? GetError(dbPMScontext db, string card, int machine, DateTime time, int maxShiftHours)
    {
        var latestTime = db.CheckInOuts.AsNoTracking()
            .Where(x => x.MaChamCong == card && (x.TrangThai == "Vào" || x.TrangThai == "Ra"))
            .Select(x => (DateTime?)x.ThoiGian).Max();
        if (latestTime > time)
            return "Giờ cân trước lượt chấm công. Kiểm tra giờ máy.";
        var latestAtMachine = ScaleAttendanceTransferService.OperationalEvents(db, card, machine, time, maxShiftHours)
            .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.Id)
            .FirstOrDefault();
        var shifts = ScaleAttendanceTransferService.OpenShifts(db, card);
        var error = Validate(shifts, machine, time, maxShiftHours, latestAtMachine);
        if (error != null) return error;
        // Máy chậm giờ không được dùng giờ cũ để tiếp tục ca đã hết hạn.
        var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime;
        return Validate(shifts, machine, now > time ? now : time, maxShiftHours, latestAtMachine);
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
            return "Có ca mở tại nhiều máy. Nhờ quản lý kiểm tra.";
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
