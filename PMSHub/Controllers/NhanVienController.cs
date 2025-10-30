using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NhanVienController(IMainService mainService) : ControllerBase
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
            var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
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
            var items = mainService.VmNhanVienHq.GetUs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetDs(string Ngay,int PageIndex,int PageSize)
        {
            var items = mainService.VmNhanVienHq.GetDs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                total = items.Count,
                PageIndex 
            };

            return Ok(json);
        }
    }
}
