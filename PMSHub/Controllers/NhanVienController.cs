using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NhanVienController(IMainService mainService,IMayCansService mayCansService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay,int PageIndex,int PageSize)
        {
           
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
            {
                //
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay.Date >= date.Date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                var json = new
                {
                    data = items,
                    total = items.Count,
                    PageIndex 
                };

                return Ok(json);
            }

            var _data = Ngay.Substring(index + 1).Split(',');
            var mayCanId = _data.FirstOrDefault() ?? "";
            var mNgay = _data.LastOrDefault() ?? "";
            date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var mayCan = mayCansService.Find(mayCanId);
            if (mayCan != null)

            {
                var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay.Date >= date.Date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                var json = new
                {
                    data = items,
                    total = items.Count,
                    PageIndex 
                };

                return Ok(json);
                //break;
            }

            return NotFound("Not Found");
        }
        [HttpGet]
        public IActionResult GetUs(string Ngay,int PageIndex,int PageSize)
        {
            
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
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

            var _data = Ngay.Substring(index + 1).Split(',');
            var mayCanId = _data.FirstOrDefault() ?? "";
            var mNgay = _data.LastOrDefault() ?? "";
            date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var mayCan = mayCansService.Find(mayCanId);
            if (mayCan != null)

            {
                var items = mainService.VmNhanVienHq.GetUs(mNgay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                    total = items.Count,
                    PageIndex 
                };

                return Ok(json);
                //break;
            }

            return NotFound("Not Found");
        }
        [HttpGet]
        public IActionResult GetDs(string Ngay,int PageIndex,int PageSize)
        {
            
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
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

            var _data = Ngay.Substring(index + 1).Split(',');
            var mayCanId = _data.FirstOrDefault() ?? "";
            var mNgay = _data.LastOrDefault() ?? "";
            date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var mayCan = mayCansService.Find(mayCanId);
            if (mayCan != null)

            {
                var items = mainService.VmNhanVienHq.GetDs(mNgay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                    total = items.Count,
                    PageIndex 
                };

                return Ok(json);
                //break;
            }

            return NotFound("Not Found");
        }
    }
}
