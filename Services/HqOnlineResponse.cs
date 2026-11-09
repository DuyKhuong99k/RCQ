using System.Globalization;
using Models.Repos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vars.Hubs;
using OnlineTotals = Dao.Repos.HQ.HQ_PhieuCan.OnlineTotals;

namespace Services;

/// <summary>HQ LITE responses. A totals failure must never masquerade as an INSERT failure.</summary>
public static class HqOnlineResponse
{
    public const string Saved = "SAVED";
    public const string NotSaved = "NOT_SAVED";
    public const string Unknown = "UNKNOWN";

    private static string? Bounded(string? value, int limit) =>
        !string.IsNullOrWhiteSpace(value) && value.Length <= limit ? value : null;

    public static DataError Error(string cmd, string message, DataLite? request, string? machine,
        string saveStatus = NotSaved) => new()
    {
        Cmd = cmd, ErrorString = message, SaveStatus = saveStatus,
        Id = Bounded(request?.Id, 100), TheId = Bounded(request?.TheId, 50),
        NgayGio = Bounded(request?.NgayGio, 14), MayCan = Bounded(machine, 50)
    };

    // Only read bounded string fields. Never echo arbitrary payloads or attempt to recover malformed JSON.
    public static DataLite? Correlation(string payload)
    {
        try
        {
            var json = JObject.Parse(payload);
            string? Read(string field, int limit) => json[field]?.Type == JTokenType.String
                ? Bounded(json[field]!.Value<string>(), limit) : null;
            return new DataLite { Id = Read("Id", 100)!, TheId = Read("TheId", 50)!, NgayGio = Read("NgayGio", 14)! };
        }
        catch (JsonException) { return null; }
    }

    // Call ONLY after COMMIT or after a verified SQL lookup of the same saved Id/content.
    // Wait for complete totals as requested; do not emit an early ACK or invented/stale counters.
    public const int MaxTotalsAttempts = 3;
    public static readonly TimeSpan TotalsBudget = TimeSpan.FromSeconds(4);
    public static readonly TimeSpan TotalsRetryDelay = TimeSpan.FromMilliseconds(150);

    public static async Task<Tuple<bool, string>> ForSavedAsync(HQ_PhieuCan item, string cmd,
        Func<CancellationToken, Task<OnlineTotals>> readTotals,
        Action<OnlineTotals>? onTotals = null, NhanVienDaiThanh? employee = null,
        TimeSpan? retryDelay = null, TimeSpan? budget = null)
    {
        var delay = retryDelay ?? TotalsRetryDelay;
        var window = budget ?? TotalsBudget;
        if (delay < TimeSpan.Zero || window <= TimeSpan.Zero || window > TotalsBudget)
            throw new ArgumentOutOfRangeException(nameof(budget));
        using var deadline = new CancellationTokenSource(window);
        OnlineTotals? totals = null;
        // The delegate is read-only; INSERT/attendance validation must never be retried here.
        // All attempts keep the original committed ticket/Id. Each production attempt opens a fresh DbContext.
        for (var attempt = 1; attempt <= MaxTotalsAttempts; attempt++)
        {
            try
            {
                deadline.Token.ThrowIfCancellationRequested();
                var reading = readTotals(deadline.Token);
                // Observe failures even if the reader takes longer to acknowledge cancellation.
                _ = reading.ContinueWith(t => { _ = t.Exception; }, CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                totals = await reading.WaitAsync(deadline.Token)
                    ?? throw new InvalidOperationException("Totals reader returned null.");
                break;
            }
            catch (Exception ex)
            {
                HqFirmwareSend.Log($"[HQ ONLINE TOTALS RETRY] Machine={item.MayCan}; Id={item.Id}; Attempt={attempt}; SQL ticket already saved; {ex}");
                if (attempt == MaxTotalsAttempts || deadline.IsCancellationRequested) break;
                try { await Task.Delay(delay, deadline.Token); }
                catch (OperationCanceledException) { break; }
            }
        }
        if (totals == null)
        {
            var request = new DataLite { Id = item.Id, TheId = item.TheIdNhanVien,
                NgayGio = item.NgayGio.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) };
            return Tuple.Create(false, JsonConvert.SerializeObject(Error(cmd, ScaleOnlineMessages.SavedTotalsError,
                request, item.MayCan, Saved)));
        }
        var response = new DataLiteRes
        {
            Id = item.Id, STT = item.STT, TheId = item.TheIdNhanVien,
            SaveStatus = Saved, IsDataAction = 1, MessStr = "ĐÃ HOÀN THÀNH", MayCan = item.MayCan,
            Ngay = item.Ngay.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
            NgayGio = item.NgayGio.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            MaLo = item.MaLo, MaSize = item.MaSize, MaThanhPham = item.MaThanhPham,
            MaLoaiNguyenLieu = item.MaLoaiNguyenLieu, MaNhanVien = item.MaNhanVien,
            MaHoSo = employee?.MaHoSo, NhanVienName = employee?.Name,
            TrongLuongNhan = item.TrongLuong, TrongLuongTare = item.TrongLuongTare,
            ChiSanLuong = item.ChiSanLuong ? 1 : 0, Status = item.Status,
            TongSoRoCan = totals.MachineCount, TongSoRo = totals.EmployeeCount,
            TongTrongLuongCan = totals.MachineWeight, TongTrongLuong = totals.EmployeeWeight
        };
        // UI failures cannot change a successful SQL/response result.
        try { onTotals?.Invoke(totals); }
        catch (Exception ex) { HqFirmwareSend.Log($"[HQ ONLINE TOTALS UI ERROR] Id={item.Id}; {ex}"); }
        return Tuple.Create(true, JsonConvert.SerializeObject(response));
    }
}
