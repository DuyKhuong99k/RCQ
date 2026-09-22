using Dao.Repos;
using Handlers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DeviceController(
    IMayCansService mayCansService,
    Database database) : Controller
{
    [HttpPost]
    public IActionResult PostInfo([FromBody] object data)
    {
        var errorCode = "";

        var result = ErrorHandler.Handle(() =>
        {
            var jsonString = data.ToString() ?? "";
            var jObject = JObject.Parse(jsonString);

            var ip = (string)(jObject["ip"] ?? "192.168.1.1")!;
            var mayCanId = (string)(jObject["id"] ?? "")!;
            var displayName = (string)(jObject["displayname"] ?? "")!;

            var mayCan = mayCansService.Find(mayCanId);

            if (mayCan == null)
            {
                throw new Exception(errorCode);
            }

            mayCan.IPAddr = ip;

            if (displayName != "")
            {
                mayCan.DisplayName = displayName;
            }

            mayCansService.NotifyItemChanged(mayCan);
        });

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                Status = $"Error: {DateTime.Now:yyyyMMddHHmmss}"
            });
        }

        return Ok(new
        {
            Status = $"Data received {DateTime.Now:yyyyMMddHHmmss}"
        });
    }

    [HttpPost]
    public IActionResult Heartbeat([FromBody] HeartbeatRequest request)
    {
        var updated = database.UpdateDeviceHeartbeat(request.Device);

        if (!updated)
        {
            return NotFound(new
            {
                Status = "Device not found",
                Device = request.Device
            });
        }

        return Ok(new
        {
            Status = "OK"
        });
    }

    public sealed class HeartbeatRequest
    {
        public long Device { get; set; }
    }
}