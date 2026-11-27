using System.Data;
using System.Globalization;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;

namespace PMSHub.ReportImport;

/// <summary>Uses the same paged API as the firmware's detailed-report export.</summary>
public sealed class ScaleReportSync(IHttpClientFactory clients, IWebHostEnvironment environment)
{
    public sealed record Report(string Machine, DateOnly Day, JArray Rows, decimal Net)
    {
        public string? OriginalCsv { get; init; }
    }
    public sealed record Result(int Removed, int Imported, decimal Net, string Backup);
    private const int PageSize = 400;
    private const int MaximumRows = 200_000;

    public async Task<Report> DownloadAsync(string machine, string address, DateOnly day,
        CancellationToken cancellation = default)
    {
        if (!IPAddress.TryParse(address, out var ip) || IPAddress.IsLoopback(ip) ||
            ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            throw new ArgumentException("IP cân không hợp lệ. Chọn đúng cân đã đăng ký.");
        var bytes = ip.GetAddressBytes();
        if (!(bytes[0] == 10 || bytes[0] == 192 && bytes[1] == 168 ||
              bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31))
            throw new ArgumentException("Chỉ tải báo cáo từ IP cân trong mạng nội bộ.");
        if (day > DateOnly.FromDateTime(ScaleWorkshopHistory.BusinessNow()))
            throw new ArgumentException("Không đồng bộ ngày tương lai.");
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        deadline.CancelAfter(TimeSpan.FromMinutes(10));
        using var http = clients.CreateClient("ScaleReportSync");
        http.Timeout = TimeSpan.FromSeconds(30);
        http.MaxResponseContentBufferSize = 8 * 1024 * 1024;
        var root = $"http://{ip}/api/phieucan/gets?";
        var date = day.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        async Task<JArray> Get(string query) => ReadEnvelope(
            await http.GetStringAsync(root + query, deadline.Token));
        var before = ReadSummary(await Get($"Path=getsophieuchitiet&Ngay={date}"));
        var rows = new JArray();
        for (var page = 1; rows.Count < before.Count; page++)
        {
            var batch = await Get($"Path=getschitiet&ngay={date}&pageSize={PageSize}&pageIndex={page}");
            if (batch.Count != Math.Min(PageSize, before.Count - rows.Count))
                throw new InvalidOperationException("Báo cáo tải thiếu hoặc thay đổi số phiếu. Chưa thay dữ liệu SQL.");
            foreach (var row in batch) rows.Add(row.DeepClone());
        }
        var after = ReadSummary(await Get($"Path=getsophieuchitiet&Ngay={date}"));
        if (before != after)
            throw new InvalidOperationException("Cân đang thay đổi báo cáo. Dừng cân tạm thời rồi đồng bộ lại.");
        var report = new Report(machine, day, rows, before.Net);
        Validate(report);
        return report;
    }

    public static JArray ReadEnvelope(string json)
    {
        var envelope = JObject.Parse(json);
        if (envelope["success"]?.Type != JTokenType.Boolean || envelope["success"]!.Value<bool>() != true ||
            envelope["data"] is not JArray rows)
            throw new InvalidOperationException("API báo cáo cân không thành công. Chưa thay dữ liệu SQL.");
        return rows;
    }

    private static (int Count, decimal Net) ReadSummary(JArray rows)
    {
        if (rows.Count != 1 || rows[0] is not JObject summary ||
            !int.TryParse(summary["SoPhieu"]?.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out var count) ||
            count < 0 || count > MaximumRows)
            throw new InvalidOperationException("Số phiếu trong báo cáo không hợp lệ.");
        var weight = summary["TrongLuongNhan"];
        // SQLite SUM returns NULL for a day with no tickets.
        var net = count == 0 && (weight == null || weight.Type == JTokenType.Null || weight.ToString() == "")
            ? 0 : Number(summary, "TrongLuongNhan");
        return (count, net);
    }

    public static List<HQ_PhieuCan> Validate(Report report)
    {
        if (report.Day > DateOnly.FromDateTime(ScaleWorkshopHistory.BusinessNow()))
            throw new ArgumentException("Không nạp phiếu ngày tương lai.");
        if (report.OriginalCsv != null && System.Text.Encoding.UTF8.GetByteCount(report.OriginalCsv) > ScaleReportCsv.MaximumBytes)
            throw new ArgumentException("File CSV vượt quá dung lượng cho phép.");
        if (string.IsNullOrWhiteSpace(report.Machine) || report.Machine.Length > 50 || report.Rows.Count > MaximumRows)
            throw new ArgumentException("Sai mã cân hoặc số phiếu báo cáo.");
        var items = new List<HQ_PhieuCan>(report.Rows.Count);
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var token in report.Rows)
        {
            if (token is not JObject row) throw new ArgumentException("Sai dòng báo cáo cân.");
            string Text(string key, int limit = 50, bool required = true)
            {
                var value = row[key];
                if (value?.Type != JTokenType.String) throw new ArgumentException($"Thiếu/sai cột {key}.");
                var text = value.Value<string>()!;
                if (text.Length > limit || text.Any(char.IsControl) || required && string.IsNullOrWhiteSpace(text))
                    throw new ArgumentException($"Sai cột {key}.");
                return text;
            }
            var machine = Text("MayCan");
            var id = Text("Id", 100);
            var timeText = Text("NgayGio");
            if (machine != report.Machine || !id.StartsWith(machine + ".", StringComparison.Ordinal) || !ids.Add(id) ||
                !DateTime.TryParseExact(timeText, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time) ||
                DateOnly.FromDateTime(time) != report.Day || time > ScaleWorkshopHistory.BusinessNow() ||
                Text("Ngay") != report.Day.ToString("yyyyMMdd", CultureInfo.InvariantCulture))
                throw new ArgumentException("Báo cáo trùng Id, sai cân hoặc sai ngày. Chưa thay dữ liệu SQL.");
            var net = Number(row, "TrongLuongNhan");
            var tare = Number(row, "TrongLuongTare");
            if (!HqWeightValues.IsValidTicket(net, tare) ||
                !int.TryParse(row["STT"]?.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out var ordinal))
                throw new ArgumentException("Sai trọng lượng, bì hoặc STT báo cáo.");
            var card = Text("TheId");
            var production = Text("ChiSanLuong");
            if (production is not ("0" or "1")) throw new ArgumentException("Sai ChiSanLuong báo cáo.");
            items.Add(new HQ_PhieuCan
            {
                Id = id, MayCan = machine, Ngay = report.Day, NgayGio = time, STT = ordinal,
                MaLo = Text("MaLo", required: false), MaSize = Text("MaSize", required: false),
                MaThanhPham = Text("MaThanhPham", required: false), MaLoaiNguyenLieu = Text("MaLoaiNguyenLieu", required: false),
                MaNhanVien = Text("MaNhanVien"), TheId = card, TheIdNhanVien = card,
                MaNhanVienPhucVu = Text("MaNhanVienPhucVu", required: false),
                MaNhanVienBanKiem = Text("MaNhanVienBanKiem", required: false),
                TrongLuong = net, TrongLuongTare = tare, ChiSanLuong = production == "1", Status = 1,
                MaXuong = "", GhiChu = ""
            });
        }
        if (items.Sum(x => x.TrongLuong) != report.Net)
            throw new ArgumentException("Tổng kg chi tiết khác tổng kg báo cáo. Chưa thay dữ liệu SQL.");
        return items;
    }

    private static decimal Number(JObject row, string key)
    {
        var token = row[key];
        if (token?.Type is not (JTokenType.String or JTokenType.Float or JTokenType.Integer) ||
            !decimal.TryParse(token.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out var number) || !HqWeightValues.WithinStorageRange(number))
            throw new ArgumentException($"Sai cột {key} báo cáo.");
        return number;
    }

    public async Task<Result> ReplaceAsync(Report report, string workshop, CancellationToken cancellation = default)
    {
        await using var db = new dbPMScontext();
        var machine = await db.Set<MayCan>().AsNoTracking().SingleAsync(x => x.Id == report.Machine, cancellation);
        if (machine.WKv != Vars.AppKV.Hq || machine.MaXuong != workshop || string.IsNullOrWhiteSpace(workshop))
            throw new InvalidOperationException("Cân không thuộc xưởng đăng nhập hoặc không phải cân HQ.");
        return await ReplaceAsync(db, report, workshop,
            Path.Combine(environment.ContentRootPath, "App_Data", "ScaleReportSync"), cancellation);
    }

    public static async Task<Result> ReplaceAsync(dbPMScontext db, Report report, string workshop,
        string backupRoot, CancellationToken cancellation = default)
    {
        var items = Validate(report);
        if (items.Count == 0) throw new ArgumentException("Báo cáo rỗng. Không thay dữ liệu SQL.");
        if (string.IsNullOrWhiteSpace(workshop) || workshop.Length > 50)
            throw new ArgumentException("Chưa xác định xưởng.");
        var history = await db.LichSuXuongMayCans.AsNoTracking().Where(x => x.MayCan == report.Machine)
            .OrderByDescending(x => x.TuThoiGian).ToListAsync(cancellation);
        foreach (var item in items)
        {
            item.MaXuong = history.Count == 0 ? workshop : history.FirstOrDefault(x => x.TuThoiGian <= item.NgayGio)?.MaXuong ?? "";
            if (item.MaXuong != workshop)
                throw new InvalidOperationException("Ngày báo cáo khác lịch sử xưởng đăng nhập. Không thay dữ liệu.");
        }
        // Range locks serialize replacement with live inserts. Failure anywhere rolls back DELETE + INSERT.
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellation);
        var existing = db.HqPhieuCans.Where(x => x.MayCan == report.Machine && x.Ngay == report.Day);
        var old = await existing.AsNoTracking().ToListAsync(cancellation);
        if (old.Any(x => x.MaXuong != workshop))
            throw new InvalidOperationException("Phiếu SQL hiện có thuộc xưởng khác. Không thay dữ liệu.");
        Directory.CreateDirectory(backupRoot);
        // GUID filename never uses user/machine input; backups stay outside wwwroot.
        var backup = Guid.NewGuid().ToString("N") + ".json";
        await File.WriteAllTextAsync(Path.Combine(backupRoot, backup), JsonConvert.SerializeObject(new
        {
            CreatedUtc = DateTime.UtcNow, report.Machine, report.Day, Workshop = workshop,
            Original = old, Source = report.Rows, report.Net, report.OriginalCsv
        }, Formatting.Indented), cancellation);
        // Keep a lossless CSV alongside the rollback archive. Unlike the browser export,
        // it includes original size/product/material codes, not only display names.
        var columns = new[] { "Id", "Ngay", "NgayGio", "MayCan", "STT", "MaLo", "MaSize", "MaThanhPham",
            "MaLoaiNguyenLieu", "MaNhanVien", "MaNhanVienPhucVu", "MaNhanVienBanKiem", "TheId",
            "TrongLuongNhan", "TrongLuongTare", "ChiSanLuong" };
        static string Csv(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";
        var csv = new System.Text.StringBuilder("\uFEFF").AppendLine(string.Join(",", columns));
        foreach (var row in report.Rows)
            csv.AppendLine(string.Join(",", columns.Select(key => Csv(row[key]!.ToString()))));
        await File.WriteAllTextAsync(Path.Combine(backupRoot, Path.GetFileNameWithoutExtension(backup) + ".source.csv"),
            csv.ToString(), cancellation);
        await existing.ExecuteDeleteAsync(cancellation);
        db.HqPhieuCans.AddRange(items);
        await db.SaveChangesAsync(cancellation);
        var stored = await db.HqPhieuCans.AsNoTracking().Where(x => x.MayCan == report.Machine && x.Ngay == report.Day)
            .Select(x => x.TrongLuong).ToListAsync(cancellation);
        if (stored.Count != items.Count || stored.Sum() != report.Net)
            throw new InvalidOperationException("Đối chiếu SQL sau nạp không khớp; đã hủy thay đổi.");
        await transaction.CommitAsync(cancellation);
        return new(old.Count, items.Count, report.Net, backup);
    }
}
