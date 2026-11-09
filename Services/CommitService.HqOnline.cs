using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json;
using Vars.Hubs;

namespace Services;

public partial class CommitService
{
    private async Task<Tuple<bool, string>> CommitHqLite(DataLite data, MayCan machine, string cmd)
    {
        // Capture selectors on receipt, before waiting behind another request.
        var captured = HqScaleSelection.Capture(machine);
        var selected = (captured.Lot, captured.Size, captured.Product, captured.Material);
        using var timing = new HqOnlineTiming(machine.Id, data.Id);
        var queued = Stopwatch.GetTimestamp();
        try
        {
            return await HqOnlineWeightWriter.ForMachine(machine.Id, async () =>
            {
                timing.QueueMs = Stopwatch.GetElapsedTime(queued).TotalMilliseconds;
                try
                {
                    var selectionError = HqScaleSelection.Check(machine, captured);
                    // Identity at non-positive weight is display-only. Verified Id replays must still
                    // work during a pending Set; the writer skips prepare for an existing Id.
                    // For new tickets, check selection inside prepare after the protected Id lookup.
                    var reply = await CommitHqLiteCore(data, machine, cmd, selected, timing, selectionError);
                    timing.Outcome = reply.Item1 ? "COMPLETE" : "REJECTED_OR_PENDING";
                    return reply;
                }
                catch (Exception ex) when (ex is Microsoft.Data.SqlClient.SqlException || ex is DbUpdateException || ex is OperationCanceledException)
                {
                    HqFirmwareSend.Log($"[HQ ONLINE SQL ERROR] Machine={machine.Id}; Id={data.Id}; {ex}");
                    return HqOnlineFailure(machine, cmd, ScaleOnlineMessages.DatabaseError, data, HqOnlineResponse.Unknown);
                }
                catch (Exception ex)
                {
                    HqFirmwareSend.Log($"[HQ ONLINE PROCESS ERROR] Machine={machine.Id}; Id={data.Id}; {ex}");
                    return HqOnlineFailure(machine, cmd, ScaleOnlineMessages.ProcessingError, data, HqOnlineResponse.Unknown);
                }
            });
        }
        catch (HqOnlineWeightWriter.QueueTimeoutException)
        {
            timing.QueueMs = Stopwatch.GetElapsedTime(queued).TotalMilliseconds;
            timing.Outcome = "QUEUE_NOT_SAVED";
            // No write started for this request. Do not clear/mutate the active owner's machine state.
            return Tuple.Create(false, JsonConvert.SerializeObject(HqOnlineResponse.Error(cmd,
                "Cân đang bận. Chưa lưu phiếu", data, machine.Id)));
        }
    }

    protected virtual dbPMScontext CreateHqOnlineDb() => new();

    private static Tuple<bool, string> HqOnlineFailure(MayCan machine, string cmd, string message,
        DataLite? request = null, string saveStatus = HqOnlineResponse.NotSaved)
    {
        if (message is not (ScaleOnlineMessages.ZeroWeight or ScaleOnlineMessages.NegativeWeight or ScaleOnlineMessages.Unstable))
        {
            var card = request?.TheId?.Trim() ?? "";
            HqFirmwareSend.Log($"[HQ LITE REJECT] Machine={machine.Id}; Id={request?.Id}; CardTail={(card.Length > 4 ? card[^4..] : card)}; DeviceTime={request?.NgayGio}; SaveStatus={saveStatus}; Reason={message}");
        }
        // No pending employee/card that could be reused by a later stable sample.
        machine.The = null;
        machine.TheAddDateTime = null;
        machine.TheNhanVien = null;
        machine.TheNhanVienAddDateTime = null;
        machine.MaNhanVien = null;
        machine.MaHoSo = null;
        machine.NhanVienName = null;
        machine.ErrorString = message;
        machine.ThongBao = message;
        machine.ColorString = "red";
        return Tuple.Create(false, JsonConvert.SerializeObject(HqOnlineResponse.Error(cmd, message, request, machine.Id, saveStatus)));
    }

    private async Task<Tuple<bool, string>> IdentifyHqEmployeeAtNonPositiveWeight(DataLite data, MayCan machine, string cmd)
    {
        var error = HqOnlineWeightWriter.IdentityRequestError(data);
        if (error != null) return HqOnlineFailure(machine, cmd, error, data);

        // Clear all pending-swipe fields first. Only the displayed identity is restored below.
        var weightMessage = data.TrongLuong < 0 ? ScaleOnlineMessages.NegativeWeight : ScaleOnlineMessages.ZeroWeight;
        var notSaved = HqOnlineFailure(machine, cmd, weightMessage, data);
        var employee = FindHqEmployee(data.TheId);
        if (employee == null) return HqOnlineFailure(machine, cmd, ScaleOnlineMessages.UnknownEmployee, data);
        if (!ScaleProductionAttendancePolicy.IsProductionCard(employee))
            return HqOnlineFailure(machine, cmd, "HQ cần thẻ nhân viên hoặc nhóm sản xuất.", data);

        try
        {
            // Existing firmware displays these fields on SETNHANVIEN, not on a failed LITE payload.
            var sent = await SendHqIdentity(machine, employee);
            if (!sent) return HqOnlineFailure(machine, cmd, ScaleOnlineMessages.IdentitySendFailed, data);
        }
        catch (Exception ex)
        {
            HqFirmwareSend.Log($"[HQ NONPOSITIVE WEIGHT DISPLAY ERROR] Machine={machine.Id}; Id={data.Id}; {ex}");
            return HqOnlineFailure(machine, cmd, ScaleOnlineMessages.IdentitySendFailed, data);
        }
        machine.MaNhanVien = employee.MaNhanVien;
        machine.MaHoSo = employee.MaHoSo;
        machine.NhanVienName = employee.Name;
        // No ticket lookup, attendance change, totals, INSERT, or success acknowledgement.
        return notSaved;
    }

    private async Task<Tuple<bool, string>> CommitHqLiteCore(DataLite data, MayCan machine, string cmd,
        (string? Lot, string? Size, string? Product, string? Material) selected, HqOnlineTiming timing,
        string? selectionError = null)
    {
        Tuple<bool, string> Fail(string message) => HqOnlineFailure(machine, cmd, message, data);

        machine.TrongLuong = data.TrongLuong;
        machine.TrongLuongTare = data.TrongLuongTare;
        machine.TheView = data.TheId;
        machine.IsStated = data.Stated == "ST" && data.TrongLuong > 0;
        machine.ErrorString = null;
        machine.ColorString = null;
        if (data.Stated == "ST" && data.TrongLuong <= 0 && !data.IsWaiting)
            return await IdentifyHqEmployeeAtNonPositiveWeight(data, machine, cmd);
        var requestError = HqOnlineWeightWriter.RequestError(data);
        if (requestError != null) return Fail(requestError);
        var time = DateTime.ParseExact(data.NgayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        if (!int.TryParse(machine.Id, NumberStyles.None, CultureInfo.InvariantCulture, out _))
            return Fail("Mã máy cân không hợp lệ.");
        NhanVienDaiThanh? employee = null;
        var useAttendance = attendanceMode?.Current.Enabled ?? true;
        var saving = Stopwatch.GetTimestamp();
        HqOnlineWeightWriter.Result result;
        try
        {
            using var attendanceDb = CreateHqOnlineDb();
            result = await HqOnlineWeightWriter.SaveAsync(attendanceDb, data, machine.Id, async cancellation =>
            {
                HqOnlineWeightWriter.Result Reject(string message) => new(null, false, message);
                if (selectionError != null) return Reject(selectionError);
                // This runs ONLY for a new Id, after the single protected lookup. Replay never
                // depends on today's selectors/card cache or opens a second DAO connection.
                employee = FindHqEmployee(data.TheId);
                if (employee == null) return Reject("Không tìm thấy thẻ nhân viên hoặc nhân viên.");
                if (!ScaleProductionAttendancePolicy.IsProductionCard(employee))
                    return Reject("HQ cần thẻ nhân viên hoặc nhóm sản xuất.");
                if (string.IsNullOrWhiteSpace(selected.Lot)) return Reject("Chưa chọn lô.");
                if (string.IsNullOrWhiteSpace(selected.Size)) return Reject("Chưa chọn size.");
                if (string.IsNullOrWhiteSpace(selected.Product)) return Reject("Chưa chọn thành phẩm.");
                if (string.IsNullOrWhiteSpace(selected.Material)) return Reject("Chưa chọn loại nguyên liệu.");
                var workshop = await ScaleWorkshopHistory.ResolveAsync(attendanceDb, machine.Id, time, cancellation);
                if (string.IsNullOrWhiteSpace(workshop))
                    return Reject("Chưa xác định xưởng tại giờ cân. Kiểm tra lịch sử chuyển xưởng.");
                var prepared = new HQ_PhieuCan
                {
                    Id = data.Id, STT = 0, Ngay = DateOnly.FromDateTime(time), NgayGio = time, MayCan = machine.Id,
                    MaLo = selected.Lot, MaSize = selected.Size, MaThanhPham = selected.Product, MaLoaiNguyenLieu = selected.Material,
                    MaNhanVien = employee.MaNhanVien, MaNhanVienPhucVu = "", MaNhanVienBanKiem = "", MaXuong = workshop,
                    TrongLuong = data.TrongLuong, TrongLuongTare = data.TrongLuongTare,
                    TheId = data.TheId, TheIdNhanVien = data.TheId, ChiSanLuong = true, Status = 1, GhiChu = ""
                };
                return new(prepared, false, null);
            }, (item, cancellation) => useAttendance ? ScaleProductionAttendancePolicy.GetOnlineErrorAsync(attendanceDb,
                data.TheId, time, attendanceOptions.Value.MaxShiftHours, employee!,
                ScaleWorkContext.Create(item.MaThanhPham, item.MaLoaiNguyenLieu, item.MaXuong), cancellation)
                : Task.FromResult<string?>(null));
        }
        finally { timing.SqlMs = Stopwatch.GetElapsedTime(saving).TotalMilliseconds; }
        if (result.Error != null) return Fail(result.Error);
        var item = result.Item!;
        if (result.Inserted)
        {
            // UI rendering/listeners run separately; committed SQL remains the source of truth.
            HqOnlineUi.Add(machine, item, row => mayCansService.AddItem(machine, row));
        }
        var calculating = Stopwatch.GetTimestamp();
        Tuple<bool, string> response;
        try { response = await HqResponse(item, machine, cmd, employee, useAttendance); }
        finally { timing.TotalsMs = Stopwatch.GetElapsedTime(calculating).TotalMilliseconds; }
        if (employee == null) return response; // verified replay: no repeated SETNHANVIEN side effect
        var identifying = Stopwatch.GetTimestamp();
        try
        {
            machine.MaNhanVien = item.MaNhanVien;
            machine.TheNhanVien = item.TheIdNhanVien;
            machine.MaHoSo = employee.MaHoSo;
            machine.NhanVienName = employee.Name;
            machine.TheNhanVienAddDateTime = null; // display only; not a pending swipe
            if (!data.IsWaiting)
                await SendHqIdentity(machine, employee);
        }
        catch (Exception ex) { HqFirmwareSend.Log($"[HQ ONLINE DISPLAY ERROR] Id={item.Id}; {ex}"); }
        finally { timing.IdentityMs = Stopwatch.GetElapsedTime(identifying).TotalMilliseconds; }
        return response;
    }

    // Cache-only lookup, used after request validation; saved-Id replay never calls it.
    private NhanVienDaiThanh? FindHqEmployee(string cardId)
    {
        var card = mainService.VmThe.Items.FirstOrDefault(x => x.MaTheTu == cardId);
        return card == null ? null : mainService.VmNhanVien.Items
            .FirstOrDefault(x => x.MaNhanVien == card.MaNhanVien);
    }

    private Task<bool> SendHqIdentity(MayCan machine, NhanVienDaiThanh employee) =>
        HqFirmwareSend.IdentityAsync(machine, (connection, cancellation) =>
            mayCansService is IHqFirmwareIdentitySender sender
                ? sender.SendHqIdentityAsync(machine.Id, connection, employee.MaNhanVien,
                    employee.MaHoSo ?? "", employee.Name ?? "", cancellation)
                : mayCansService.CommandSetNhanVien(machine.Id, employee.MaNhanVien,
                    employee.MaHoSo ?? "", employee.Name ?? ""));

    private async Task<Tuple<bool, string>> HqResponse(HQ_PhieuCan item, MayCan machine, string cmd,
        NhanVienDaiThanh? employee = null, bool useAttendance = true)
    {
        var reply = await HqOnlineResponse.ForSavedAsync(item, cmd, async cancellation =>
        {
            using var totalsDb = CreateHqOnlineDb();
            return await ScaleOnlineAttendanceService.TotalsAsync(totalsDb, item,
                attendanceOptions.Value.MaxShiftHours, cancellation, useAttendance);
        }, totals =>
        {
            machine.TongSoRo = totals.MachineCount;
            machine.TongTrongLuong = totals.MachineWeight;
        }, employee);
        try
        {
            machine.ThongBao = reply.Item1 ? "ĐÃ HOÀN THÀNH" : ScaleOnlineMessages.SavedTotalsError;
            machine.ErrorString = reply.Item1 ? null : ScaleOnlineMessages.SavedTotalsError;
            machine.ColorString = reply.Item1 ? "greenyellow" : "red";
        }
        catch (Exception ex) { HqFirmwareSend.Log($"[HQ ONLINE STATUS UI ERROR] Id={item.Id}; {ex}"); }
        return reply;
    }
}

