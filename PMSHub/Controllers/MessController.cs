using Handlers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MessController(IMayCansService mayCansService, IMainService mainService) : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] object data)
    {
        var result = ErrorHandler.Handle(() =>
        {
            var jsonString = data.ToString() ?? "";
            var jObject = JObject.Parse(jsonString);
            var maycan = (string)(jObject["MayCan"] ?? "")!;
            var content = (string)(jObject["Content"] ?? "")!;
            var mayCan = mayCansService.Find(maycan ?? "");
               

            if (mayCan != null)
            {
                mayCan.ThongBao = $"{content} - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            }

            mayCansService.NotifyItemChanged(mayCan);
        });
        if (!result.IsSuccess)
            //switch (errorCode)
            return BadRequest(new { message = $"Error: {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
        //} 
        // Process the received data
        return Ok(new
        {
            message = $"Data received: {DateTime.Now.ToString("yyyyMMddHHmmss")}"
        }); //Ok(new { Status = $"Data received {DateTime.Now.ToString("yyyyMMddHHmmss")}" });
    }
}