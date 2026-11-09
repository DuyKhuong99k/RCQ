using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewModels.Repos.Staff;

namespace Services;

public partial class HubEmployeeManager
{
    public async Task<NhanVienImportResult> ImportAsync(IReadOnlyList<NhanVienImportRow> rows,
        string expectedWorkshop, bool previewOnly = true, CancellationToken cancellation = default)
    {
        if (rows.Count is 0 or > NhanVienImport.MaxRows || rows.Any(row => row == null))
            return new() { Messages = $"Danh sách phải có từ 1 đến {NhanVienImport.MaxRows} nhân viên." };

        // Snapshot caller-owned fields before the first await. Client statuses never authorize a write.
        var input = rows.Select(row => new NhanVienImportRow
        {
            RowNumber = row.RowNumber, MaNhanVien = row.MaNhanVien, MaHoSo = row.MaHoSo, Name = row.Name
        }).ToArray();
        NhanVienImportResult result;
        try
        {
            using var deadline = Deadline(cancellation);
            using var db = CreateDb();
            var workshop = await RequireWorkshopAsync(db, deadline.Token);
            if (!string.Equals(workshop.Ma, expectedWorkshop, StringComparison.Ordinal))
                return new() { Messages = "Xưởng đăng nhập đã thay đổi. Đóng cửa sổ và kiểm tra lại file." };

            // Short, atomic batch: recheck SQL even after a successful preview.
            // Serializable protects missing keys and file-code predicates from concurrent imports/writers.
            await using var transaction = previewOnly ? null :
                await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, deadline.Token);
            var codes = input.Select(row => (row.MaNhanVien ?? "").Trim()).Distinct().ToArray();
            var files = input.Select(row => (row.MaHoSo ?? "").Trim()).Distinct().ToArray();
            var existing = await db.NhanVienDaiThanh.AsNoTracking()
                .Where(employee => codes.Contains(employee.MaNhanVien) || files.Contains(employee.MaHoSo!))
                .Select(employee => new Models.Repos.Models.NhanVienDaiThanh
                { MaNhanVien = employee.MaNhanVien, MaHoSo = employee.MaHoSo }).ToListAsync(deadline.Token);
            result = NhanVienImport.Validate(input, existing);
            if (!result.IsSuccess || previewOnly || result.ReadyCount == 0) return result;

            var newRows = result.Rows.Where(row => row.Status == "Ready").ToArray();
            var now = ScaleWorkshopHistory.BusinessNow();
            db.NhanVienDaiThanh.AddRange(newRows.Select(row =>
            {
                var employee = NhanVienImport.CreateEmployee(row, workshop.Ma);
                employee.MNgay = now;
                return employee;
            }));
            await db.SaveChangesAsync(deadline.Token);
            await transaction!.CommitAsync(deadline.Token);
            foreach (var row in newRows)
            {
                row.Status = "Imported";
                row.Message = "Đã thêm nhân viên.";
            }
            result.ImportedCount = newRows.Length;
            result.ReadyCount = 0;
            result.Messages = $"Đã thêm {result.ImportedCount} nhân viên; bỏ qua {result.SkippedCount} nhân viên đã tồn tại.";
        }
        catch (UnauthorizedAccessException error) { return new() { Messages = error.Message }; }
        catch (Exception error) when (error.GetBaseException() is SqlException { Number: 2601 or 2627 or 1205 })
        {
            logger.LogWarning(error, "Dữ liệu thay đổi trong lúc nhập nhân viên hàng loạt.");
            return new() { Messages = "Có thay đổi đồng thời. Bấm Kiểm tra danh sách rồi nhập lại; không ghi đè hồ sơ cũ." };
        }
        catch (Exception error)
        {
            logger.LogError(error, "Không xác nhận được kết quả nhập nhân viên trên Hub.");
            return new() { Messages = "Chưa xác nhận được kết quả. Kiểm tra lại danh sách trước khi nhập lại; mã đã có sẽ được bỏ qua." };
        }

        // A cache error after commit must never be reported as a failed insertion.
        var refresh = await RefreshAfterCommitAsync(result.Messages, cancellation);
        result.Messages = refresh.Message;
        return result;
    }
}
