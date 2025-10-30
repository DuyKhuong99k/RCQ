using Handlers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class DeviceController(IMayCansService mayCansService) : Controller
{
    [HttpPost]
    public IActionResult PostInfo([FromBody] object data)
    {
        var errorCode = "";
        var result = ErrorHandler.Handle(() =>
        {
            var jsonString = data.ToString()??"";
            var jObject = JObject.Parse(jsonString);
            var ip = (string)(jObject["ip"]??"192.168.1.1")!;
            var mayCanId = (string)(jObject["id"]??"")!;
            var displayName = (string)(jObject["displayname"] ?? "")!;
            var mayCan = mayCansService.Find(mayCanId);
            if(mayCan == null)
            {
                throw new Exception(errorCode);
            }
            else
            {
                mayCan.IPAddr = ip;
                if (displayName != "")
                {
                    mayCan.DisplayName = displayName;
                }
                mayCansService.NotifyItemChanged(mayCan);
            }
        });
        if (!result.IsSuccess)
            return BadRequest(new { Status = $"Error: {DateTime.Now:yyyyMMddHHmmss}" });
        return Ok(new { Status = $"Data received {DateTime.Now:yyyyMMddHHmmss}" });
    }
}