using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Vars.Hubs;

namespace Services;

/// <summary>Online LITE only. Offline/Sync keep their separate validation rules.</summary>
public static class HqOnlineWeightWriter
{
    public const string UnstableError = ScaleOnlineMessages.Unstable;
    public const string ConflictError = ScaleOnlineMessages.WeightConflict;
    public const string WaitingError = ScaleOnlineMessages.OldWaiting;
    // Keys come from registered machines, not arbitrary payload Ids. Keep gates for
    // the process lifetime; removing a gate while a waiter exists can split a lock.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> MachineGates = new(StringComparer.Ordinal);
    public static readonly TimeSpan QueueBudget = TimeSpan.FromSeconds(15);
    public static readonly TimeSpan WriteBudget = TimeSpan.FromSeconds(30);
    public const int CommandTimeoutSeconds = 15;
    public sealed class QueueTimeoutException() : TimeoutException("Hàng đợi cân quá hạn.");

    public static async Task<T> ForMachine<T>(string machine, Func<Task<T>> action, TimeSpan? waitBudget = null)
    {
        var gate = MachineGates.GetOrAdd(machine, _ => new SemaphoreSlim(1, 1));
        var window = waitBudget ?? QueueBudget;
        if (window <= TimeSpan.Zero || window > QueueBudget) throw new ArgumentOutOfRangeException(nameof(waitBudget));
        if (!await gate.WaitAsync(window)) throw new QueueTimeoutException();
        try { return await action(); }
        finally { gate.Release(); }
    }

    public static string? RequestError(DataLite data) => ValidateRequest(data, identityOnly: false);

    // ST with non-positive net weight can identify a card, but must never INSERT a ticket.
    public static string? IdentityRequestError(DataLite data)
    {
        if (data.Stated != "ST" || data.TrongLuong > 0) return UnstableError;
        if (data.IsWaiting) return WaitingError;
        return ValidateRequest(data, identityOnly: true);
    }

    private static string? ValidateRequest(DataLite data, bool identityOnly)
    {
        if (!DateTime.TryParseExact(data.NgayGio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _)) return ScaleOnlineMessages.InvalidTime;
        if (string.IsNullOrWhiteSpace(data.Id) || data.Id.Length > 100) return ScaleOnlineMessages.InvalidId;
        if (data.Stated != "ST") return UnstableError;
        if (!identityOnly && data.TrongLuong < 0) return ScaleOnlineMessages.NegativeWeight;
        if (!identityOnly && data.TrongLuong == 0) return ScaleOnlineMessages.ZeroWeight;
        if (data.TrongLuongTare < 0) return ScaleOnlineMessages.InvalidTare;
        if (!HqWeightValues.WithinStorageRange(data.TrongLuong) || !HqWeightValues.WithinStorageRange(data.TrongLuongTare))
            return ScaleOnlineMessages.InvalidWeightRange;
        // SQL stores decimal(18,3): never silently round a request and then
        // report a conflicting payload when the identical request is retried.
        if (!HqWeightValues.HasStoragePrecision(data.TrongLuong) || !HqWeightValues.HasStoragePrecision(data.TrongLuongTare))
            return ScaleOnlineMessages.InvalidPrecision;
        if (string.IsNullOrWhiteSpace(data.TheId) || data.TheId.Length > 50) return ScaleOnlineMessages.InvalidCard;
        return null;
    }

    public static bool MatchesRequest(HQ_PhieuCan saved, DataLite data, string machine) =>
        saved.Id == data.Id && saved.MayCan == machine && saved.TheIdNhanVien == data.TheId
        && saved.TheId == data.TheId
        && saved.NgayGio.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) == data.NgayGio
        && saved.NgayGio.Ticks % TimeSpan.TicksPerSecond == 0
        && saved.TrongLuong == data.TrongLuong && saved.TrongLuongTare == data.TrongLuongTare
        && saved.Status == 1 && saved.ChiSanLuong;

    public sealed record Result(HQ_PhieuCan? Item, bool Inserted, string? Error);

    public static async Task<Result> SaveAsync(dbPMScontext db, DataLite data, string machine,
        Func<CancellationToken, Task<Result>> prepare,
        Func<HQ_PhieuCan, CancellationToken, Task<string?>> validate, TimeSpan? writeBudget = null)
    {
        var error = RequestError(data);
        if (error != null) return new(null, false, error);
        var window = writeBudget ?? WriteBudget;
        if (window <= TimeSpan.Zero || window > WriteBudget) throw new ArgumentOutOfRangeException(nameof(writeBudget));
        using var deadline = new CancellationTokenSource(window);
        var token = deadline.Token;
        db.Database.SetCommandTimeout(CommandTimeoutSeconds);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        await ScaleAttendanceTransferService.LockCardAsync(db, data.TheId, token);
        var machineResource = "HQOnlineMachine:" + machine;
        var idResource = "HQWeightBackup:" + data.Id;
        // Preserve lock order, batch two lock commands in one round trip.
        await db.Database.ExecuteSqlInterpolatedAsync($@"DECLARE @result int;
            EXEC @result=sys.sp_getapplock @Resource={machineResource},@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=5000;
            IF @result<0 THROW 51001,N'Không khóa được máy cân.',1;
            EXEC @result=sys.sp_getapplock @Resource={idResource},@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=5000;
            IF @result<0 THROW 51001,N'Không khóa được phiếu cân.',1;", token);
        // The ONLY Id lookup for normal online processing. Retry works even if current selectors/cache changed.
        var saved = await db.HqPhieuCans.AsNoTracking().SingleOrDefaultAsync(x => x.Id == data.Id, token);
        if (saved != null)
        {
            if (!MatchesRequest(saved, data, machine)) return new(null, false, ConflictError);
            await tx.CommitAsync(token);
            return new(saved, false, null);
        }
        if (data.IsWaiting) return new(null, false, WaitingError);
        var prepared = await prepare(token);
        if (prepared.Error != null) return new(null, false, prepared.Error);
        var item = prepared.Item ?? throw new InvalidOperationException("Missing prepared ticket.");
        if (!MatchesRequest(item, data, machine)) return new(null, false, ConflictError);
        error = await validate(item, token);
        if (error != null) return new(null, false, error);
        token.ThrowIfCancellationRequested();
        db.HqPhieuCans.Add(item);
        await db.SaveChangesAsync(token);
        // Never retry INSERT automatically. If commit/cancellation is ambiguous, caller returns UNKNOWN.
        await tx.CommitAsync(token);
        return new(item, true, null);
    }

    public static Result Save(dbPMScontext db, DataLite data, string machine,
        Func<HQ_PhieuCan> create, Func<HQ_PhieuCan, string?> validate)
    {
        var error = RequestError(data);
        if (error != null) return new(null, false, error);
        using var tx = db.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        // Card first, matching attendance/transfer lock order. SQL machine lock
        // also serializes different Hub processes; SemaphoreSlim covers UI/awaits.
        ScaleAttendanceTransferService.LockCard(db, data.TheId);
        foreach (var resource in new[] { "HQOnlineMachine:" + machine, "HQWeightBackup:" + data.Id })
            db.Database.ExecuteSqlInterpolated($@"DECLARE @result int;
                EXEC @result=sys.sp_getapplock @Resource={resource},@LockMode='Exclusive',
                    @LockOwner='Transaction',@LockTimeout=5000;
                IF @result<0 THROW 51001,N'Cân bận. Quẹt lại',1;");
        var saved = db.HqPhieuCans.AsNoTracking().SingleOrDefault(x => x.Id == data.Id);
        if (saved != null)
        {
            if (!MatchesRequest(saved, data, machine)) return new(null, false, ConflictError);
            tx.Commit();
            return new(saved, false, null);
        }
        // No pending-swipe state exists in this online flow. An unsolicited or
        // delayed response to an old SETWAITINGSTABLE must not create a new ticket.
        if (data.IsWaiting) return new(null, false, WaitingError);
        var item = create();
        if (!MatchesRequest(item, data, machine)) return new(null, false, ConflictError);
        error = validate(item);
        if (error != null) return new(null, false, error);
        db.HqPhieuCans.Add(item);
        db.SaveChanges();
        // The attendance check and INSERT use the SAME connection/transaction.
        // Never return an acknowledgement before Commit succeeds.
        tx.Commit();
        return new(item, true, null);
    }
}
