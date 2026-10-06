using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using PMSHub.Middlewares;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

/// <summary>
/// Nhận dữ liệu chấm công đồng bộ từ firmware của máy cân.
/// Firmware gửi POST /api/chamcong/post với các trường id, maThietBi,
/// thoiGian, trangThai và ngayTao.
/// </summary>
[ApiController]
[Route("api/chamcong")]
public sealed class ChamCongController(
    IMayCansService mayCansService,
    ILogger<ChamCongController> logger) : ControllerBase
{
    [HttpPost("post")]
    public IActionResult Post([FromBody] ChamCongPostRequest request)
    {
        var ipNguon = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "không xác định";
        var rawPayload = HttpContext.Items[ChamCongRawBodyMiddleware.RawPayloadItemKey]?.ToString()
            ?? "<không đọc được request body>";

        logger.LogInformation(
            "[CHẤM CÔNG JSON GỐC] IP={IpNguon} | {RawPayload}",
            ipNguon,
            rawPayload);

        if (string.IsNullOrWhiteSpace(request.Id) ||
            string.IsNullOrWhiteSpace(request.MaThietBi) ||
            string.IsNullOrWhiteSpace(request.ThoiGian))
        {
            logger.LogWarning(
                "[CHẤM CÔNG KHÔNG HỢP LỆ] Thiếu id, maThietBi hoặc thoiGian. IP={IpNguon}",
                ipNguon);
            return BadRequest(new { message = "Thiếu id, maThietBi hoặc thoiGian." });
        }

        if (!TryParseDate(request.ThoiGian, out var thoiGian))
        {
            logger.LogWarning(
                "[CHẤM CÔNG KHÔNG HỢP LỆ] thoiGian={ThoiGian} sai định dạng. Id={Id}",
                request.ThoiGian,
                request.Id);
            return BadRequest(new { message = "thoiGian không đúng định dạng." });
        }

        if (request.Isforget is not 0 and not 1)
        {
            logger.LogWarning(
                "[CHẤM CÔNG KHÔNG HỢP LỆ] Isforget={Isforget} không phải 0 hoặc 1. Id={Id}",
                request.Isforget,
                request.Id);
            return BadRequest(new { message = "isforget chỉ nhận giá trị 0 hoặc 1." });
        }

        // CheckInOut là bảng chấm công dùng chung trong PMS_HQ.
        if (!int.TryParse(request.MaThietBi, NumberStyles.Integer,
            CultureInfo.InvariantCulture, out var maSoMay) || maSoMay <= 0)
            return BadRequest(new { message = "Mã máy phải là số nguyên dương." });
        var recordId = AttendanceRecordId.Create(maSoMay.ToString(CultureInfo.InvariantCulture), thoiGian);

        // Id là mã thẻ; ChamCongId là mã lượt quẹt. Firmware cũ có thể gửi
        // Id dạng mãThẻ.yyyyMMddHHmmss, nên chuẩn hóa trước khi lưu.
        var chamCongId = string.IsNullOrWhiteSpace(request.ChamCongId)
            ? request.Id.Trim()
            : request.ChamCongId.Trim();
        var maChamCong = LayMaThe(request.Id);
        if (string.IsNullOrWhiteSpace(maChamCong) || maChamCong.Length > 50)
            return BadRequest(new { message = "Mã chấm công phải có từ 1 đến 50 ký tự." });
        var congViec = request.CongViec?.Trim();
        var trangThai = request.TrangThai?.Trim();
        if (trangThai is not "Vào" and not "Ra")
            return BadRequest(new { message = "TrangThai phải là Vào hoặc Ra." });
        using var db = new dbPMScontext();
        var maThietBi = request.MaThietBi.Trim();
        var displayName = db.MayCans
            .Where(x => x.Id == maThietBi)
            .Select(x => x.DisplayName)
            .FirstOrDefault();
        var tenMay = string.IsNullOrWhiteSpace(displayName) ? maThietBi : displayName.Trim();

        if (db.CheckInOuts.Any(x => x.Id == recordId))
            return Ok(new { message = "Bản ghi đã tồn tại.", chamCongId,
                isforget = request.Isforget, duplicated = true });

        db.CheckInOuts.Add(new CheckInOut
        {
            Id = recordId,
            MaChamCong = maChamCong,
            ThoiGian = thoiGian,
            MaSoMay = maSoMay,
            TenMay = tenMay,
            TrangThai = trangThai,
            CongViec = congViec,
            Isforget = request.Isforget
        });
        try
        {
            db.SaveChanges();
        }
        catch (DbUpdateException error) when (AttendanceRecordId.IsDuplicate(error))
        {
            // PK bảo vệ cả trường hợp hai request cùng kiểm tra trước khi insert.
            if (!db.CheckInOuts.AsNoTracking().Any(x => x.Id == recordId)) throw;
            return Ok(new { message = "Bản ghi đã tồn tại.", chamCongId,
                isforget = request.Isforget, duplicated = true });
        }

        logger.LogInformation(
            "[CHẤM CÔNG ĐÃ LƯU] ChamCongId={ChamCongId}; Id={Id}; Máy={MaThietBi}; ThờiGian={ThoiGian}; TrạngThái={TrangThai}; CôngViệc={CongViec}; Isforget={Isforget}",
            chamCongId,
            request.Id,
            request.MaThietBi,
            thoiGian,
            request.TrangThai,
            congViec,
            request.Isforget);

        return Ok(new
        {
            message = "Đã nhận chấm công.",
            chamCongId,
            isforget = request.Isforget
        });
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync(
        [FromBody] ChamCongSyncRequest? request,
        CancellationToken cancellationToken)
    {
        var ngay = DateTime.Today;
        if (!string.IsNullOrWhiteSpace(request?.Ngay) &&
            !DateTime.TryParseExact(request.Ngay, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out ngay))
        {
            return BadRequest(new { message = "Ngày đồng bộ phải có định dạng yyyyMMdd." });
        }

        var thietBiOnline = mayCansService.Items.Values
            .Where(x => x.IsConnected && !string.IsNullOrWhiteSpace(x.ConnectionId))
            .ToList();

        foreach (var thietBi in thietBiOnline)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await mayCansService.CommandChamCongSync(thietBi.Id, ngay);
        }

        if (thietBiOnline.Count == 0)
        {
            logger.LogWarning("[SYNC CHẤM CÔNG] Không có máy nào đang kết nối PMSHub.");
            return Conflict(new { deviceCount = 0, message = "Không có máy nào đang kết nối PMSHub." });
        }

        logger.LogInformation(
            "[SYNC CHẤM CÔNG] Đã gửi sự kiện chamcongsync ngày {Ngay:yyyyMMdd} đến {SoThietBi} máy: {DanhSachMay}",
            ngay,
            thietBiOnline.Count,
            string.Join(", ", thietBiOnline.Select(x => x.Id)));

        return Ok(new
        {
            deviceCount = thietBiOnline.Count,
            message = $"Đã gửi lệnh chamcongsync đến {thietBiOnline.Count} máy."
        });
    }

    private static string LayMaThe(string value)
    {
        var id = value.Trim();
        var dauCham = id.IndexOf('.');
        // Cùng quy tắc với báo cáo CheckInOut: phần trước dấu chấm là mã thẻ.
        return dauCham >= 0 ? id.Substring(0, dauCham).Trim() : id;
    }

    private static bool TryParseDate(string value, out DateTime result)
    {
        var formats = new[]
        {
            "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss",
            "yyyy/MM/dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss"
        };
        return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture,
                   DateTimeStyles.AllowWhiteSpaces, out result) ||
               DateTime.TryParse(value, CultureInfo.InvariantCulture,
                   DateTimeStyles.AllowWhiteSpaces, out result);
    }

    public sealed class ChamCongPostRequest
    {
        public string? ChamCongId { get; set; }
        public string Id { get; set; } = "";
        public string MaThietBi { get; set; } = "";
        public string ThoiGian { get; set; } = "";
        public string? TrangThai { get; set; }
        public string? NgayTao { get; set; }
        public string? CongViec { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Isforget { get; set; } = 0;
    }

    public sealed class ChamCongSyncRequest
    {
        public string? Ngay { get; set; }
    }
}
