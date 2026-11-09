using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

/// <summary>Server-side workshop scoping for every read/write, independent of client form fields.</summary>
public partial class HubEmployeeManager(ISessionService session, IHubEmployeeCardCatalog catalog, ILogger<HubEmployeeManager> logger)
{
    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(15);

    public async Task<HubEmployeeList> LoadAsync(CancellationToken cancellation = default)
    {
        using var deadline = Deadline(cancellation);
        using var db = CreateDb();
        var workshop = await RequireWorkshopAsync(db, deadline.Token);
        var employees = await db.NhanVienDaiThanh.AsNoTracking().Where(x => x.Xuong == workshop.Ma)
            .OrderBy(x => x.MaNhanVien).Select(HubEmployeeEdit.Projection).ToListAsync(deadline.Token);
        var allCodes = await db.NhanVienDaiThanh.AsNoTracking().Select(x => x.MaNhanVien).ToListAsync(deadline.Token);
        return new(workshop.Ma, workshop.Ten ?? workshop.Ma, HubEmployeeEdit.SuggestCode(allCodes),
            employees);
    }

    public async Task<HubEmployeeResult> SaveAsync(HubEmployeeEdit input, bool creating, string? originalCode = null,
        CancellationToken cancellation = default)
    {
        try
        {
            using var deadline = Deadline(cancellation);
            using var db = CreateDb();
            var workshop = await RequireWorkshopAsync(db, deadline.Token);
            var form = input.Copy();
            form.MaNhanVien = (form.MaNhanVien ?? "").Trim();
            form.MaHoSo = (form.MaHoSo ?? "").Trim();
            form.Name = (form.Name ?? "").Trim();
            var errors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(form, new ValidationContext(form), errors, true))
                return new(false, errors[0].ErrorMessage ?? "Thông tin nhân viên không hợp lệ.");
            if (!creating && !string.Equals(form.MaNhanVien, originalCode, StringComparison.Ordinal))
                return new(false, "Không được đổi mã nhân viên khi sửa.");

            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, deadline.Token);
            var employee = await FindForWriteAsync(db, form.MaNhanVien, deadline.Token);
            if (creating && employee != null) return new(false, "Mã nhân viên đã tồn tại. Chọn mã khác hoặc tải lại.");
            if (!creating && (employee == null || !string.Equals(employee.Xuong, workshop.Ma, StringComparison.OrdinalIgnoreCase)))
                return new(false, "Nhân viên không tồn tại trong xưởng đang chọn.");
            if (!creating && employee!.MNgay != form.ModifiedAt)
                return new(false, "Hồ sơ đã được người khác sửa. Tải lại trước khi lưu.");

            var now = creating ? ScaleWorkshopHistory.BusinessNow() : NextModifiedAt(employee!.MNgay);
            if (creating)
            {
                employee = new NhanVienDaiThanh { MaNhanVien = form.MaNhanVien, Xuong = workshop.Ma };
                db.NhanVienDaiThanh.Add(employee);
            }
            Apply(form, employee!);
            employee!.MNgay = now;
            if (!creating)
            {
                var change = new HQ_NhanVien_U();
                db.HqNhanVienUs.Add(change);
                db.Entry(change).CurrentValues.SetValues(employee);
                change.Ngay = change.MNgay = now;
            }
            // Profile and update journal are committed together, never two independent saves.
            await db.SaveChangesAsync(deadline.Token);
            await transaction.CommitAsync(deadline.Token);
        }
        catch (UnauthorizedAccessException error) { return new(false, error.Message); }
        catch (DbUpdateException error) when (error.GetBaseException() is SqlException { Number: 2601 or 2627 })
        { return new(false, "Mã nhân viên đã tồn tại. Chọn mã khác hoặc tải lại."); }
        catch (Exception error)
        {
            logger.LogError(error, "Không lưu được hồ sơ nhân viên trên Hub.");
            return new(false, "Chưa xác nhận lưu được hồ sơ. Tải lại để kiểm tra, không bấm lưu liên tục.");
        }
        return await RefreshAfterCommitAsync(creating ? "Đã thêm nhân viên." : "Đã sửa nhân viên.", cancellation);
    }

    public async Task<HubEmployeeResult> DeleteAsync(string employeeCode, DateTime expectedModified,
        CancellationToken cancellation = default)
    {
        try
        {
            using var deadline = Deadline(cancellation);
            using var db = CreateDb();
            var workshop = await RequireWorkshopAsync(db, deadline.Token);
            // Match Report: delete only the selected profile and journal it atomically.
            // Historical references are not blockers; database constraints still apply.
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, deadline.Token);
            var employee = await FindForWriteAsync(db, employeeCode, deadline.Token);
            if (employee == null || !string.Equals(employee.Xuong, workshop.Ma, StringComparison.OrdinalIgnoreCase))
                return new(false, "Nhân viên không tồn tại trong xưởng đang chọn.");
            if (employee.MNgay != expectedModified) return new(false, "Hồ sơ đã thay đổi. Tải lại trước khi xóa.");
            var now = NextModifiedAt(employee.MNgay);
            var deleted = new HQ_NhanVien_D();
            db.HqNhanVienDs.Add(deleted);
            db.Entry(deleted).CurrentValues.SetValues(employee);
            deleted.Ngay = deleted.MNgay = now;
            db.NhanVienDaiThanh.Remove(employee);
            await db.SaveChangesAsync(deadline.Token);
            await transaction.CommitAsync(deadline.Token);
        }
        catch (UnauthorizedAccessException error) { return new(false, error.Message); }
        catch (DbUpdateException error) when (error.GetBaseException() is SqlException { Number: 547 })
        {
            logger.LogWarning(error, "SQL chặn xóa hồ sơ nhân viên do ràng buộc dữ liệu.");
            return new(false, "SQL không cho xóa do ràng buộc dữ liệu. Giữ hồ sơ hoặc tắt Trạng thái làm việc; không xóa dữ liệu liên quan.");
        }
        catch (Exception error)
        {
            logger.LogError(error, "Không xóa được hồ sơ nhân viên trên Hub.");
            return new(false, "Chưa xác nhận xóa được hồ sơ. Tải lại để kiểm tra; không xóa dữ liệu liên quan.");
        }
        return await RefreshAfterCommitAsync("Đã xóa nhân viên.", cancellation);
    }

    private async Task<XiNghiep> RequireWorkshopAsync(dbPMScontext db, CancellationToken cancellation)
    {
        if (!int.TryParse(session.Get("UserId"), out var userId) || userId <= 0 ||
            string.IsNullOrWhiteSpace(session.Get("XuongId")))
            throw new UnauthorizedAccessException("Phiên đăng nhập không hợp lệ. Đăng nhập lại và chọn xưởng.");
        db.Database.SetCommandTimeout(5);
        if (!await db.NguoiDungs.AsNoTracking().AnyAsync(x => x.Id == userId && x.IsActive == true, cancellation))
            throw new UnauthorizedAccessException("Tài khoản không hoạt động. Vui lòng đăng nhập lại.");
        var workshopCode = session.Get("XuongId");
        var workshop = await db.XiNghiep.AsNoTracking().Where(x => x.Ma == workshopCode && x.SuDung == true)
            .Select(x => new XiNghiep { Ma = x.Ma, Ten = x.Ten, SuDung = x.SuDung }).FirstOrDefaultAsync(cancellation);
        return workshop ?? throw new UnauthorizedAccessException("Xưởng đang chọn không còn hợp lệ. Đăng nhập lại.");
    }

    private static Task<NhanVienDaiThanh?> FindForWriteAsync(dbPMScontext db, string code, CancellationToken cancellation) =>
        db.NhanVienDaiThanh.FromSqlInterpolated($"SELECT * FROM dbo.NhanVienDaiThanh WITH (UPDLOCK, HOLDLOCK) WHERE MaNhanVien = {code}")
            .SingleOrDefaultAsync(cancellation);

    private async Task<HubEmployeeResult> RefreshAfterCommitAsync(string message, CancellationToken cancellation)
    {
        try { await catalog.RefreshAsync(cancellation); return new(true, message); }
        catch (Exception error)
        {
            logger.LogWarning(error, "Hồ sơ đã lưu SQL nhưng chưa làm mới được danh sách nhân viên/thẻ.");
            return new(true, message + " Chưa cập nhật được danh sách Hub; bấm Tải lại hoặc đăng nhập lại.", true);
        }
    }

    private static void Apply(HubEmployeeEdit form, NhanVienDaiThanh employee)
    {
        employee.MaHoSo = form.MaHoSo;
        employee.Name = form.Name;
        employee.MaChamCong = string.IsNullOrWhiteSpace(form.MaChamCong) ? null : form.MaChamCong.Trim();
        employee.DeptName0 = string.IsNullOrWhiteSpace(form.DeptName0) ? null : form.DeptName0.Trim();
        employee.LoaiSanLuong = form.LoaiSanLuong;
        employee.IsContracting = form.IsContracting;
        employee.IsHuman = form.IsHuman;
        employee.IsPhucVu = form.IsPhucVu;
        employee.IsGiaCong = form.IsGiaCong;
        employee.IsBanKiem = form.IsBanKiem;
        employee.IsChucNang = form.IsChucNang;
        employee.IsNhom = form.IsNhom;
        employee.IsNhanVienCat = form.IsNhanVienCat;
        employee.IsShowDinhMuc = form.IsShowDinhMuc;
    }

    private static CancellationTokenSource Deadline(CancellationToken cancellation)
    {
        var source = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        source.CancelAfter(OperationTimeout);
        return source;
    }

    private static DateTime NextModifiedAt(DateTime previous)
    {
        var now = ScaleWorkshopHistory.BusinessNow();
        return now > previous ? now : previous.AddTicks(1);
    }

    protected virtual dbPMScontext CreateDb() => new();
}
