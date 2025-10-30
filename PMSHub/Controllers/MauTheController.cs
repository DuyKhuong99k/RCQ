using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MauTheController(IMainService mainService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay,int PageIndex,int PageSize)
        {
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
            var items = mainService.VmColorCode.Items.Where(x=>x.MNgay >date).OrderBy(x=>x.Code) .Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Code,Ten=x.Name,Code = x.Code,Code2 = x.Code2,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList();
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
            var items = mainService.VmColorCode.GetUs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=>new {Id=x.Code,Ten=x.Name,Code = x.Code,Code2 = x.Code2,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetDs(string Ngay,int PageIndex,int PageSize)
        {
            var items = mainService.VmColorCode.GetDs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=>new {Id=x.Code,Ten=x.Name,Code = x.Code,Code2 = x.Code2,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
       
    }
}
