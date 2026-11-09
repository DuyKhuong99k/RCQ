using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;

namespace Services;

public sealed record ConfirmedAttendanceAdministrator(int Id, string UserName);

public interface IAttendanceAdministratorVerifier
{
    Task<ConfirmedAttendanceAdministrator?> VerifyAsync(string userName, string password, CancellationToken cancellation);
}

/// <summary>Fresh SQL verification, not the login cache or a client-supplied admin flag.</summary>
public sealed class SqlAttendanceAdministratorVerifier : IAttendanceAdministratorVerifier
{
    public async Task<ConfirmedAttendanceAdministrator?> VerifyAsync(string userName, string password,
        CancellationToken cancellation)
    {
        using var db = new dbPMScontext();
        db.Database.SetCommandTimeout(5);
        var users = await db.NguoiDungs.AsNoTracking().Where(x => x.UserName == userName)
            .Select(x => new NguoiDung { Id = x.Id, UserName = x.UserName, Password = x.Password,
                IsAdmin = x.IsAdmin, IsActive = x.IsActive })
            .Take(2).ToListAsync(cancellation);
        var user = users.Count == 1 ? users[0] : null;
        return user?.IsAdmin == true && LoginService.VerifyPassword(user, password)
            ? new(user.Id, user.UserName) : null;
    }
}

/// <summary>Re-authenticate every change. Never create/change the current web session.</summary>
public sealed class AttendanceModeManager(AttendanceModeStore store, IAttendanceAdministratorVerifier verifier)
{
    private readonly SemaphoreSlim confirming = new(1, 1);
    private DateTimeOffset nextAttempt;

    public async Task<AttendanceModeResult> ChangeAsync(bool enabled, long expectedRevision,
        string userName, string password, CancellationToken cancellation = default)
    {
        try
        {
            if (!await confirming.WaitAsync(0, cancellation)) return new(false, "Đang xác nhận. Vui lòng thử lại.");
        }
        catch (OperationCanceledException) { return new(false, "Đã hủy xác nhận."); }
        try
        {
            if (DateTimeOffset.UtcNow < nextAttempt) return new(false, "Vui lòng chờ 2 giây rồi xác nhận lại.");
            if (string.IsNullOrWhiteSpace(userName) || userName.Length > 50 ||
                string.IsNullOrEmpty(password) || password.Length > 1024)
                return new(false, "Nhập tài khoản quản trị và mật khẩu hợp lệ.");
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            deadline.CancelAfter(TimeSpan.FromSeconds(10));
            var verification = verifier.VerifyAsync(userName.Trim(), password, deadline.Token);
            _ = verification.ContinueWith(task => { _ = task.Exception; }, CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            var administrator = await verification.WaitAsync(deadline.Token);
            if (administrator == null)
            {
                nextAttempt = DateTimeOffset.UtcNow.AddSeconds(2);
                return new(false, "Sai tài khoản, mật khẩu hoặc không có quyền quản trị.");
            }
            return await store.ChangeAsync(enabled, expectedRevision, administrator, deadline.Token);
        }
        catch (OperationCanceledException) { return new(false, "Xác nhận quá hạn hoặc đã hủy. Kiểm tra trạng thái hiện tại."); }
        catch (Exception ex)
        {
            HqFirmwareSend.Log($"[ATTENDANCE MODE ERROR] Error={ex.GetType().Name}; No credentials logged");
            return new(false, "Không đổi được chế độ. Kiểm tra SQL và quyền ghi App_Data.");
        }
        finally { confirming.Release(); }
    }
}
