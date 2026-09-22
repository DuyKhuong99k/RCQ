using Microsoft.AspNetCore.Mvc;
using Services;

namespace PMSHub.Controllers;

[ApiController]
[Route("api/chamcongtaixuong")]
public sealed class ChamCongTaiXuongController : ControllerBase
{
    [HttpPost("post")]
    public IActionResult Post([FromBody] Request request)
    {
        var maTheTu = request.MaTheTu?.Trim() ?? "";
        var deviceCode = request.Device.ToString();
        var time = request.Time?.Trim() ?? "";
        var xuong = request.Xuong?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(maTheTu) ||
            string.IsNullOrWhiteSpace(deviceCode) ||
            string.IsNullOrWhiteSpace(time))
        {
            return BadRequest(new
            {
                message = "Thiếu MaTheTu, time hoặc Device."
            });
        }

        return Ok(
            AttendanceProcessor.Save(
                deviceCode,
                maTheTu,
                request.TrangThai,
                time,
                xuong));
    }

    public sealed class Request
    {
        public string? MaTheTu { get; set; }
        public string? Time { get; set; }
        public string? Xuong { get; set; }
        public long Device { get; set; }
        public string? TrangThai { get; set; }
    }
}