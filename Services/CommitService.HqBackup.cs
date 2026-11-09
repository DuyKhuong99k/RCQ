using Models.Repos.Models;
using Newtonsoft.Json;
using Vars.Hubs;

namespace Services;

public partial class CommitService
{
    private Tuple<bool, string> CommitHqBackup(IReadOnlyList<DataCoummunication> records,
        ClientInfo client, string cmd, bool batch)
    {
        var result = HqWeightBackupProcessor.Process(records, client, cmd,
            attendanceOptions.Value.MaxShiftHours, batch, saved =>
        {
            var machine = mayCansService.Find(client.Id);
            if (machine == null) return;
            foreach (var item in saved)
            {
                if (!machine.Items.OfType<HQ_PhieuCan>().Any(current => current.Id == item.Id))
                    mayCansService.AddItem(machine, item);
            }
            var today = ScaleWorkshopHistory.BusinessNow().Date;
            if (saved.Any(item => item.NgayGio.Date == today))
            {
                var totals = mainService.VmPhieuCanHq.GetOnlineTotals(today, machine.Id, machine.MaNhanVien);
                machine.TongSoRo = totals.MachineCount;
                machine.TongTrongLuong = totals.MachineWeight;
            }
            machine.ThongBao = $"Đã xác nhận {saved.Count} phiếu backup trong SQL (bao gồm phiếu đã có).";
            machine.ErrorString = null;
            mayCansService.NotifyItemChanged(machine);
        }, attendanceEnabled: attendanceMode?.Current.Enabled ?? true);
        if (!result.Item1)
        {
            var machine = mayCansService.Find(client.Id);
            if (machine != null)
            {
                var error = JsonConvert.DeserializeObject<DataError>(result.Item2)?.ErrorString ?? "Backup HQ thất bại.";
                machine.ErrorString = error;
                machine.ThongBao = error;
                machine.ColorString = "red";
                mayCansService.NotifyItemChanged(machine);
            }
        }
        return result;
    }
}

