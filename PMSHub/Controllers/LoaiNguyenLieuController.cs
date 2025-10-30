using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoaiNguyenLieuController(IMainService mainService,IMayCansService mayCansService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay,int PageIndex,int PageSize)
        {
            
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                DateTime date = new DateTime();
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw;
                }
                var items = mainService.VmNguyenLieuHq.Items.Where(x=>x.MNgay.Date >= date.Date).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList();
                var json = new
                {
                    data = items,
                    total = items.Count,
                    PageIndex 
                };

                return Ok(json);
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                DateTime date = new DateTime();
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    var itemshq = mainService.VmNguyenLieuHq.Items.Where(x=>x.MNgay.Date >= date.Date).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList();
                    var jsonhq = new
                    {
                        data = itemshq,
                        total = itemshq.Count,
                        PageIndex 
                    };

                    return Ok(jsonhq);
                }
            }

            return NotFound("Not Found");
        }
        [HttpGet]
        public IActionResult GetUs(string Ngay,int PageIndex,int PageSize)
        {
            
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                var items = mainService.VmNguyenLieuHq.GetUs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x=>new {Id=x.LoaiNguyenLieuId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                    total = items.Count,
                    PageIndex 
                };

                return Ok(json);
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                DateTime date = new DateTime();
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    var itemshq = mainService.VmNguyenLieuHq.GetUs(mNgay, PageIndex, PageSize);
                    var jsonhq = new
                    {
                        data = itemshq.Select(x=>new {Id=x.LoaiNguyenLieuId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                        total = itemshq.Count,
                        PageIndex 
                    };

                    return Ok(jsonhq);
                }
            }

            return NotFound("Not Found");
        }
        [HttpGet]
        public IActionResult GetDs(string Ngay,int PageIndex,int PageSize)
        {
            
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                var items = mainService.VmNguyenLieuHq.GetDs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x=>new {Id=x.LoaiNguyenLieuId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                    total = items.Count,
                    PageIndex 
                };

                return Ok(json);
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                DateTime date = new DateTime();
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    var itemshq = mainService.VmNguyenLieuHq.GetDs(mNgay, PageIndex, PageSize);
                    var jsonhq = new
                    {
                        data = itemshq.Select(x=>new {Id=x.LoaiNguyenLieuId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                        total = itemshq.Count,
                        PageIndex 
                    };

                    return Ok(jsonhq);
                }
            }

            return NotFound("Not Found");
        }
    }
}
