using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using ViewModels.Repos.Staff;

namespace PMSHub.EmployeeImport;

public static class HubEmployeeExcelReader
{
    public static List<NhanVienImportRow> Read(Stream stream)
    {
        ValidateArchive(stream);
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheets.FirstOrDefault()
            ?? throw new InvalidDataException("File không có trang tính.");
        var required = new[] { "ma nhan vien", "ma ho so", "ten nhan vien" };
        var columns = new int[required.Length];
        foreach (var cell in sheet.Row(1).CellsUsed())
        {
            var index = Array.IndexOf(required, NormalizeHeader(cell.GetString()));
            if (index < 0) continue;
            if (columns[index] != 0) throw new InvalidDataException("File có tiêu đề cột bị trùng.");
            columns[index] = cell.Address.ColumnNumber;
        }
        if (columns.Any(x => x == 0))
            throw new InvalidDataException("Dòng đầu tiên phải có đủ 3 cột: Mã nhân viên, Mã hồ sơ, Tên nhân viên.");

        var rows = new List<NhanVienImportRow>();
        // Ignore formatting-only rows, but retain the original Excel row numbers in errors.
        foreach (var row in sheet.RowsUsed(XLCellsUsedOptions.Contents).Where(x => x.RowNumber() > 1))
        {
            var cells = columns.Select(column => row.Cell(column)).ToArray();
            if (cells.All(cell => cell.IsEmpty())) continue;
            if (cells.Any(cell => cell.HasFormula || cell.DataType == XLDataType.Error))
                throw new InvalidDataException($"Dòng {row.RowNumber()}: vui lòng dùng giá trị văn bản, không dùng công thức hoặc ô lỗi.");
            rows.Add(new NhanVienImportRow
            {
                RowNumber = row.RowNumber(),
                MaNhanVien = cells[0].GetFormattedString(CultureInfo.InvariantCulture).Trim(),
                MaHoSo = cells[1].GetFormattedString(CultureInfo.InvariantCulture).Trim(),
                Name = cells[2].GetFormattedString(CultureInfo.InvariantCulture).Trim()
            });
            if (rows.Count > NhanVienImport.MaxRows)
                throw new InvalidDataException($"Chỉ hỗ trợ tối đa {NhanVienImport.MaxRows} nhân viên mỗi lần nhập.");
        }
        if (rows.Count == 0) throw new InvalidDataException("File không có dữ liệu nhân viên.");
        return rows;
    }

    private static void ValidateArchive(Stream stream)
    {
        // Bound expanded workbook size before ClosedXML allocates its object model.
        if (!stream.CanSeek) throw new InvalidDataException("Không đọc được file Excel.");
        stream.Position = 0;
        using (var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read, true))
        {
            if (archive.Entries.Count > 1000 || archive.Entries.Sum(entry => entry.Length) > 25 * 1024 * 1024)
                throw new InvalidDataException("File Excel quá lớn sau giải nén. Chỉ giữ trang danh sách cần nhập.");
        }
        stream.Position = 0;
    }

    private static string NormalizeHeader(string value)
    {
        var text = new string(value.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray())
            .Replace('đ', 'd').Replace('Đ', 'D').ToLowerInvariant();
        return string.Join(" ", text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}

