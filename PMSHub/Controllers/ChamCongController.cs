using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
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
        // MaSoMay là số nếu firmware gửi số; nếu không, vẫn lưu được tên máy.
        _ = int.TryParse(request.MaThietBi, NumberStyles.Integer,
            CultureInfo.InvariantCulture, out var maSoMay);

        // Firmware mới gửi ChamCongId là khóa duy nhất của lượt chấm công.
        // Giữ fallback về Id để vẫn nhận được dữ liệu từ firmware cũ.
        var maChamCong = string.IsNullOrWhiteSpace(request.ChamCongId)
            ? request.Id.Trim()
            : request.ChamCongId.Trim();
        var congViec = request.CongViec?.Trim();
        var ghiChu = $"Id={request.Id};TrangThai={request.TrangThai ?? ""};NgayTao={request.NgayTao ?? ""}";
        using var db = new dbPMScontext();

        var banGhiDaTonTai = db.CheckInOuts.FirstOrDefault(x =>
            x.MaChamCong == maChamCong &&
            x.MaSoMay == maSoMay &&
            x.ThoiGian == thoiGian &&
            x.CongViec == congViec);
        if (banGhiDaTonTai != null)
        {
            var isforgetThayDoi = banGhiDaTonTai.Isforget != request.Isforget;
            if (isforgetThayDoi)
            {
                banGhiDaTonTai.Isforget = request.Isforget;
                db.SaveChanges();
            }

            logger.LogInformation(
                "[CHẤM CÔNG BỎ QUA] Bản ghi trùng. ChamCongId={ChamCongId}; Id={Id}; Máy={MaThietBi}; ThờiGian={ThoiGian}; CôngViệc={CongViec}; Isforget={Isforget}; IsforgetĐãCậpNhật={IsforgetDaCapNhat}",
                maChamCong,
                request.Id,
                request.MaThietBi,
                thoiGian,
                congViec,
                request.Isforget,
                isforgetThayDoi);
            return Ok(new
            {
                message = isforgetThayDoi
                    ? "Bản ghi đã tồn tại, Isforget đã được cập nhật."
                    : "Bản ghi đã tồn tại.",
                chamCongId = maChamCong,
                isforget = request.Isforget,
                duplicated = true
            });
        }

        db.CheckInOuts.Add(new CheckInOut
        {
            MaChamCong = maChamCong,
            ThoiGian = thoiGian,
            MaSoMay = maSoMay,
            TenMay = request.MaThietBi,
            GhiChu = ghiChu,
            CongViec = congViec,
            Isforget = request.Isforget
        });
        db.SaveChanges();

        logger.LogInformation(
            "[CHẤM CÔNG ĐÃ LƯU] ChamCongId={ChamCongId}; Id={Id}; Máy={MaThietBi}; ThờiGian={ThoiGian}; TrạngThái={TrangThai}; CôngViệc={CongViec}; Isforget={Isforget}",
            maChamCong,
            request.Id,
            request.MaThietBi,
            thoiGian,
            request.TrangThai,
            congViec,
            request.Isforget);

        return Ok(new
        {
            message = "Đã nhận chấm công.",
            chamCongId = maChamCong,
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
