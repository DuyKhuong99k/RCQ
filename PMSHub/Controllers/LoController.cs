using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoController(IMainService mainService)  : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay,int PageIndex,int PageSize)
        {

            var items = mainService.VmLoHq.Gets(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=> new {x.Id,MNgay=x.MNgay.ToString("yyyyMMdd"),x.SuDung,NgayNguyenLieu=$"{x.MNgay.ToString("yyyyMMddHHmmss")}"}),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetUs(string Ngay,int PageIndex,int PageSize)
        {
            var items = mainService.VmLoHq.GetUs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=> new {x.Id,MNgay=x.MNgay.ToString("yyyyMMdd"),x.SuDung,NgayNguyenLieu=$"{x.MNgay.ToString("yyyyMMddHHmmss")}"}),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetDs(string Ngay,int PageIndex,int PageSize)
        {
            var items = mainService.VmLoHq.GetDs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=> new {x.Id,MNgay=x.MNgay.ToString("yyyyMMdd"),x.SuDung,NgayNguyenLieu=$"{x.MNgay.ToString("yyyyMMddHHmmss")}"}),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
    }
}
