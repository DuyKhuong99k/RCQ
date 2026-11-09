using Models.Repos.Models;

namespace ViewModels.Repos.Staff;

public class NhanVienImportRow
{
    public int RowNumber { get; set; }
    public string MaNhanVien { get; set; } = "";
    public string MaHoSo { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public string Message { get; set; } = "";
}

public class NhanVienImportRequest
{
    public List<NhanVienImportRow> Rows { get; set; } = new();
    public string? Xuong { get; set; }
    public bool PreviewOnly { get; set; } = true;
}

public class NhanVienImportResult
{
    public bool IsSuccess { get; set; }
    public string Messages { get; set; } = "";
    public int ReadyCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public int ImportedCount { get; set; }
    public List<NhanVienImportRow> Rows { get; set; } = new();
}

public static class NhanVienImport
{
    public const int MaxRows = 1000;

    // Used at preview and again immediately before saving. Never trust client row statuses.
    public static NhanVienImportResult Validate(IEnumerable<NhanVienImportRow> source,
        IEnumerable<NhanVienDaiThanh> existing)
    {
        var rows = source.Select(x => new NhanVienImportRow
        {
            RowNumber = x.RowNumber,
            MaNhanVien = x.MaNhanVien?.Trim() ?? "",
            MaHoSo = x.MaHoSo?.Trim() ?? "",
            Name = x.Name?.Trim() ?? ""
        }).ToList();
        var result = new NhanVienImportResult { Rows = rows };
        if (rows.Count == 0 || rows.Count > MaxRows)
        {
            result.Messages = $"File phải có từ 1 đến {MaxRows} nhân viên.";
            return result;
        }

        var duplicateCodes = rows.GroupBy(x => x.MaNhanVien, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1).Select(g => g.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var duplicateFiles = rows.GroupBy(x => x.MaHoSo, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1).Select(g => g.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var employees = existing.ToList();
        var codes = employees.Select(x => x.MaNhanVien.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var files = employees.Where(x => !string.IsNullOrWhiteSpace(x.MaHoSo))
            .Select(x => x.MaHoSo!.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows)
        {
            var errors = new List<string>();
            if (row.MaNhanVien.Length == 0 || row.MaHoSo.Length == 0 || row.Name.Length == 0)
                errors.Add("Thiếu mã nhân viên, mã hồ sơ hoặc tên nhân viên.");
            if (row.MaNhanVien.Length > 50 || row.MaHoSo.Length > 50 || row.Name.Length > 100)
                errors.Add("Mã tối đa 50 ký tự; tên nhân viên tối đa 100 ký tự.");
            if (duplicateCodes.Contains(row.MaNhanVien)) errors.Add("Mã nhân viên bị trùng trong file.");
            if (duplicateFiles.Contains(row.MaHoSo)) errors.Add("Mã hồ sơ bị trùng trong file.");
            if (errors.Count > 0)
            {
                row.Status = "Error";
                row.Message = string.Join(" ", errors);
                result.ErrorCount++;
            }
            else if (codes.Contains(row.MaNhanVien))
            {
                row.Status = "Skipped";
                row.Message = "Mã nhân viên đã tồn tại; bỏ qua, giữ nguyên dữ liệu hiện có.";
                result.SkippedCount++;
            }
            else if (files.Contains(row.MaHoSo))
            {
                row.Status = "Error";
                row.Message = "Mã hồ sơ đã được sử dụng bởi nhân viên khác.";
                result.ErrorCount++;
            }
            else
            {
                row.Status = "Ready";
                row.Message = "Sẵn sàng thêm mới.";
                result.ReadyCount++;
            }
        }
        result.IsSuccess = result.ErrorCount == 0;
        result.Messages = $"{result.ReadyCount} nhân viên mới, {result.SkippedCount} bỏ qua, {result.ErrorCount} dòng lỗi."
            + (result.ErrorCount > 0 ? " Vui lòng sửa file và kiểm tra lại trước khi nhập." : "");
        return result;
    }

    public static NhanVienDaiThanh CreateEmployee(NhanVienImportRow row, string? xuong) => new()
    {
        MaNhanVien = row.MaNhanVien,
        MaHoSo = row.MaHoSo,
        Name = row.Name,
        Xuong = xuong,
        FirstWorkingDate = "Đang Làm",
        IsContracting = true,
        IsHuman = true,
        IsShowDinhMuc = true,
        LoaiSanLuong = 1
    };
}

