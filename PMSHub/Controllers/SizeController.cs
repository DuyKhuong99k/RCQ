using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SizeController(IMainService mainService) : ControllerBase
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
         
            var items = mainService.VmSizeHq.Items.Where(x=>x.MNgay >date).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList();
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
            var items = mainService.VmSizeHq.GetUs(Ngay, PageIndex, PageSize);
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
            var items = mainService.VmSizeHq.GetDs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
    }
    //var items = mainService.VmSizeDinhHinh.Items.ToList();
    //var json = new
    //{
    //    data = items,
    //    total = items.Count
    //};

    //return Ok(json);
}
