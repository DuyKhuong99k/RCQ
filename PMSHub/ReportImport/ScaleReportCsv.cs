using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;

namespace PMSHub.ReportImport;

public static class ScaleReportCsv
{
    public const int MaximumBytes = 10 * 1024 * 1024;
    private static readonly string[] RequiredColumns =
    {
        "Id", "Ngày Giờ", "Lô", "Mã Nhân Viên", "Nguyên Liệu", "Thành Phẩm", "Size",
        "Trọng Lượng", "Trọng Lượng Tare", "Mã Thẻ"
    };

    public static string TemplateDownloadUrl { get; } =
        "data:text/csv;charset=utf-8," + Uri.EscapeDataString("\uFEFF" + string.Join(",", RequiredColumns) + "\r\n");

    public sealed record Row(string Id, DateTime Time, string Lot, string Employee, string Material, string Product,
        string Size, decimal Net, decimal Tare, string Card);
    public sealed record Document(string Machine, DateOnly Day, IReadOnlyList<Row> Rows, string Original);

    public static Document Read(string text)
    {
        if (System.Text.Encoding.UTF8.GetByteCount(text) > MaximumBytes) throw new ArgumentException("File vượt quá 10 MB.");
        using var reader = new TextFieldParser(new StringReader(text.TrimStart('\uFEFF')))
        { TextFieldType = FieldType.Delimited, HasFieldsEnclosedInQuotes = true, TrimWhiteSpace = false };
        reader.SetDelimiters(",");
        var header = reader.ReadFields() ?? throw new ArgumentException("File CSV rỗng.");
        if (header.Select(name => name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != header.Length) throw new ArgumentException("CSV trùng tên cột.");
        var fields = header.Select((name, i) => (name: name.Trim(), i)).ToDictionary(x => x.name, x => x.i, StringComparer.Ordinal);
        if (!fields.ContainsKey("Nguyên Liệu"))
            throw new ArgumentException("CSV thiếu cột Nguyên Liệu. Hãy xuất lại báo cáo chi tiết trên cân đã cập nhật.");
        if (RequiredColumns.Any(x => !fields.ContainsKey(x))) throw new ArgumentException("Chọn CSV từ Báo cáo → Chi tiết → Xuất báo cáo trên cân. Không dùng file tổng hợp hoặc file Excel.");
        var rows = new List<Row>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        string? machine = null;
        DateOnly? day = null;
        while (!reader.EndOfData)
        {
            var values = reader.ReadFields()!;
            if (values.Length != header.Length)
                throw new ArgumentException($"Dòng {rows.Count + 2} sai số cột. Có thể tên chứa dấu phẩy nhưng firmware chưa đặt trong dấu nháy; không thể tự tách an toàn.");
            string Value(string key)
            {
                var value = values[fields[key]].Trim();
                // Firmware wraps text such as card numbers in Excel-style text literals.
                if (value.StartsWith("=\"", StringComparison.Ordinal) && value.EndsWith('"') && value.Length >= 3)
                    return value[2..^1].Replace("\"\"", "\"").Trim();
                return value.StartsWith("'", StringComparison.Ordinal) ? value[1..] : value;
            }
            var id = Value("Id");
            var split = id.IndexOf('.');
            if (split <= 0 || !ids.Add(id)) throw new ArgumentException($"Dòng {rows.Count + 2}: sai hoặc trùng Id.");
            var currentMachine = id[..split];
            if (!DateTime.TryParseExact(Value("Ngày Giờ"), new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss", "yyyyMMddHHmmss" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                throw new ArgumentException($"Dòng {rows.Count + 2}: ngày giờ phải giữ đúng định dạng xuất từ cân (yyyy-MM-dd HH:mm:ss).");
            var currentDay = DateOnly.FromDateTime(time);
            machine ??= currentMachine; day ??= currentDay;
            if (machine != currentMachine || day != currentDay) throw new ArgumentException("File phải chứa đúng một cân và một ngày.");
            decimal Number(string key)
            {
                if (!decimal.TryParse(Value(key), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out var number)) throw new ArgumentException($"Dòng {rows.Count + 2}: sai {key}.");
                return number;
            }
            var material = Value("Nguyên Liệu");
            if (string.IsNullOrWhiteSpace(material))
                throw new ArgumentException($"Dòng {rows.Count + 2}: phiếu {id} thiếu nguyên liệu.");
            rows.Add(new(id, time, Value("Lô"), Value("Mã Nhân Viên"), material, Value("Thành Phẩm"), Value("Size"),
                Number("Trọng Lượng"), Number("Trọng Lượng Tare"), Value("Mã Thẻ")));
            if (rows.Count > 200_000) throw new ArgumentException("File vượt quá 200.000 phiếu.");
        }
        if (rows.Count == 0) throw new ArgumentException("File không có phiếu; không dùng file rỗng để xóa SQL.");
        return new(machine!, day!.Value, rows, text);
    }

    public static IReadOnlyDictionary<string, string> ResolveMaterials(Document file, IEnumerable<(string Id, string Name)> catalog)
    {
        var codes = catalog.ToLookup(x => x.Id, StringComparer.Ordinal);
        var names = catalog.ToLookup(x => x.Name.Trim(), StringComparer.OrdinalIgnoreCase);
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var material in file.Rows.Select(x => x.Material).Distinct(StringComparer.Ordinal))
        {
            var matches = codes[material].Concat(names[material]).Select(x => x.Id).Distinct(StringComparer.Ordinal).ToArray();
            if (matches.Length == 0)
                throw new ArgumentException($"Nguyên liệu «{material}» trong CSV chưa có trong danh mục trên server.");
            if (matches.Length > 1)
                throw new ArgumentException($"Nguyên liệu «{material}» trong CSV khớp nhiều mã trong danh mục trên server.");
            result.Add(material, matches[0]);
        }
        return result;
    }
    private static string Code(IReadOnlyDictionary<string, string> map, string key, string caption) =>
        map.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value :
            throw new ArgumentException($"Chưa chọn mã {caption}: {key}.");

    public static ScaleReportSync.Report Build(Document file, IReadOnlyDictionary<string, string> products,
        IReadOnlyDictionary<string, string> sizes, IReadOnlyDictionary<string, string> materials)
    {
        var rows = new JArray(file.Rows.Select(row => new JObject
        {
            ["Id"] = row.Id, ["MayCan"] = file.Machine, ["Ngay"] = file.Day.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
            ["NgayGio"] = row.Time.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture), ["STT"] = "0",
            ["MaLo"] = row.Lot, ["MaNhanVien"] = row.Employee, ["TheId"] = row.Card,
            ["MaThanhPham"] = Code(products, row.Product, "thành phẩm"), ["MaSize"] = Code(sizes, row.Size, "size"),
            ["MaLoaiNguyenLieu"] = Code(materials, row.Material, "nguyên liệu trong CSV"),
            ["TrongLuongNhan"] = row.Net.ToString(CultureInfo.InvariantCulture),
            ["TrongLuongTare"] = row.Tare.ToString(CultureInfo.InvariantCulture),
            ["ChiSanLuong"] = "1", ["MaNhanVienPhucVu"] = "", ["MaNhanVienBanKiem"] = ""
        }));
        var report = new ScaleReportSync.Report(file.Machine, file.Day, rows, file.Rows.Sum(x => x.Net)) { OriginalCsv = file.Original };
        ScaleReportSync.Validate(report);
        return report;
    }
}
