using System.Text.Json;

namespace Services;

/// <summary>Private, atomic persistence. Only the authenticated manager may change this store.</summary>
public sealed class AttendanceModeStore : IAttendanceMode
{
    private readonly string path;
    private readonly SemaphoreSlim changing = new(1, 1);
    private AttendanceModeState state;
    public AttendanceModeState Current => Volatile.Read(ref state);
    public event Action? Changed;

    public AttendanceModeStore(string path)
    {
        this.path = Path.GetFullPath(path);
        state = File.Exists(this.path) ? Read(this.path) : AttendanceModeState.Default;
    }

    private static AttendanceModeState Read(string path)
    {
        try
        {
            var saved = JsonSerializer.Deserialize<AttendanceModeState>(File.ReadAllText(path));
            if (saved == null || saved.Revision < 1 || saved.ChangedById is not > 0 ||
                string.IsNullOrWhiteSpace(saved.ChangedBy) || saved.ChangedAtUtc == null)
                throw new InvalidDataException();
            return saved;
        }
        catch (Exception ex)
        {
            // Never silently disable attendance because settings are missing/corrupt/unreadable.
            throw new InvalidOperationException("Không đọc được chế độ chấm công. Kiểm tra App_Data/attendance-mode.json.", ex);
        }
    }

    internal async Task<AttendanceModeResult> ChangeAsync(bool enabled, long expectedRevision,
        ConfirmedAttendanceAdministrator administrator, CancellationToken cancellation)
    {
        await changing.WaitAsync(cancellation);
        string? temporary = null;
        try
        {
            var before = Current;
            if (before.Revision != expectedRevision)
                return new(false, "Chế độ vừa được thay đổi. Tải lại và xác nhận lại.");
            if (before.Enabled == enabled) return new(true, "Chế độ đã đúng như yêu cầu.");
            var after = new AttendanceModeState(enabled, checked(before.Revision + 1),
                administrator.Id, administrator.UserName, DateTimeOffset.UtcNow);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            await using (var file = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(file, after, cancellationToken: cancellation);
                await file.FlushAsync(cancellation);
                file.Flush(flushToDisk: true);
            }
            cancellation.ThrowIfCancellationRequested();
            File.Move(temporary, path, overwrite: true);
            // Persistence is committed. Do not allow later cancellation/UI faults to undo the result.
            Volatile.Write(ref state, after);
            HqFirmwareSend.Log($"[ATTENDANCE MODE] Enabled={enabled}; Revision={after.Revision}; AdministratorId={administrator.Id}; UTC={after.ChangedAtUtc:O}");
            foreach (var listener in Changed?.GetInvocationList() ?? Array.Empty<Delegate>())
            {
                try { ((Action)listener)(); }
                catch { /* A disconnected page cannot change persisted settings. */ }
            }
            return new(true, enabled ? "Đã bật chấm công tại cân và xưởng." : "Đã tắt chấm công. Cân sản lượng bình thường.");
        }
        finally
        {
            if (temporary != null)
            {
                try { File.Delete(temporary); }
                catch { /* Do not replace an already committed outcome with a cleanup error. */ }
            }
            changing.Release();
        }
    }
}
