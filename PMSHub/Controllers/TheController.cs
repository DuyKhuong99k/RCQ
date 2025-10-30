using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Dao.Repos.HQ;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TheController (IMainService mainService): ControllerBase
    {
        [HttpGet]
        public IActionResult GetTheZeros(string Ngay, int PageIndex, int PageSize)
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
            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items.Where(x => x.NgayGio > date && x.IsZero == true).OrderBy(x=>x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.MaThe,MaNhanVien = "",MaThanhPham ="",MaSize="",ColorId = "",MaLoaiNguyenLieu="",TrongLuongTare ="-999",Zero = true, NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}" }).ToList();   
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetTheTares(string Ngay, int PageIndex, int PageSize)
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
            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items.Where(x => x.NgayGio > date && (x.TrongLuongTare > 0 || x.TrongLuongTare == -1)).OrderBy(x=>x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.MaThe,MaNhanVien = "",MaThanhPham ="",MaSize="",ColorId = "",MaLoaiNguyenLieu="",TrongLuongTare = x.TrongLuongTare,Zero = false, NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}" }).ToList();   
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetTheRos(string Ngay, int PageIndex, int PageSize)
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
            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheRo.Items.Where(x => x.CreatedDateTime > date ).OrderBy(x=>x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.MaThe,MaNhanVien = "",MaThanhPham ="",MaSize="",ColorId = x.ColorCode,MaLoaiNguyenLieu="",TrongLuongTare = "-999",Zero = false, NgayGio = $"{x.CreatedDateTime.ToString("yyyyMMddHHmmss")}" }).ToList();   
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetTheNhanViens(string Ngay, int PageIndex, int PageSize)
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
            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmThe.Items.Where(x => x.NgayGio > date ).OrderBy(x=>x.MaNhanVien).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.MaTheTu,MaNhanVien = x.MaNhanVien,MaThanhPham ="",MaSize="",ColorId = "",MaLoaiNguyenLieu="",TrongLuongTare = "-999",Zero = false, NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}" }).ToList();   
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetTheSizes(string Ngay, int PageIndex, int PageSize)
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
            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items.Where(x => x.NgayGio > date ).OrderBy(x=>x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.MaThe,MaNhanVien = "",MaThanhPham ="",MaSize=x.MaSize,ColorId = "",MaLoaiNguyenLieu="",TrongLuongTare = "-999",Zero = false, NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}" }).ToList();   
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetTheThanhPhams(string Ngay, int PageIndex, int PageSize)
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
            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items.Where(x => x.NgayGio > date ).OrderBy(x=>x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.MaThe,MaNhanVien = "",MaThanhPham =x.MaThanhPham,MaSize="",ColorId = "",MaLoaiNguyenLieu="",TrongLuongTare = "-999",Zero = false, NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}" }).ToList();   
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetTheLoaiNguyenLieus(string Ngay, int PageIndex, int PageSize)
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
            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items.Where(x => x.NgayGio > date ).OrderBy(x=>x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.MaThe,MaNhanVien = "",MaThanhPham ="",MaSize=x.MaSize,ColorId = "",MaLoaiNguyenLieu=x.MaLoaiNguyenLieu,TrongLuongTare = "-999",Zero = false, NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}" }).ToList();   
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }
        [HttpGet]
        public IActionResult GetTheDs(string Ngay, int PageIndex, int PageSize)
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

            var items = mainService.VmThe.GetDs(Ngay, PageIndex, PageSize);
            var items2 = mainService.VmTheThanhPham.GetDs(Ngay, PageIndex, PageSize);
            items.AddRange(items2);
            var items3 = mainService.VmTheRo.GetDs(Ngay, PageIndex, PageSize);
            items.AddRange(items3);
            var _items = items.Select(x=>new {Id = x}).Distinct().ToList();
            var json = new
            {
                data = _items,
                total = _items.Count,
                PageIndex
            };
            return Ok(json);
        }
    }
}
