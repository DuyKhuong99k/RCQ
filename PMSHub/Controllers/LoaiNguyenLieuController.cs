using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoaiNguyenLieuController(IMainService mainService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay,int PageIndex,int PageSize)
        {
         
            var items = mainService.VmNguyenLieuHq.Items.Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetUs(string Ngay,int PageIndex,int PageSize)
        {
            var items = mainService.VmNguyenLieuHq.GetUs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetDs(string Ngay,int PageIndex,int PageSize)
        {
            var items = mainService.VmNguyenLieuHq.GetDs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
    }
}
