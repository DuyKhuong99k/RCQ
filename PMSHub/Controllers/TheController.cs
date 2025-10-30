using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Dao.Repos.HQ;
using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TheController(IMainService mainService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTheZeros(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items.Where(x => x.NgayGio.Date >= date.Date && x.IsZero == true)
                .OrderBy(x => x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x =>
                    new
                    {
                        Id = x.MaThe, MaNhanVien = "", MaThanhPham = "", MaSize = "", ColorId = "",
                        MaLoaiNguyenLieu = "", TrongLuongTare = "-999", Zero = true,
                        NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}"
                    }).ToList();
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
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date >= date.Date && (x.TrongLuongTare > 0 || x.TrongLuongTare == -1))
                .OrderBy(x => x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x =>
                    new
                    {
                        Id = x.MaThe, MaNhanVien = "", MaThanhPham = "", MaSize = "", ColorId = "",
                        MaLoaiNguyenLieu = "", TrongLuongTare = x.TrongLuongTare, Zero = false,
                        NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}"
                    }).ToList();
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
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheRo.Items.Where(x => x.CreatedDateTime.Date >= date.Date).OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                {
                    Id = x.MaThe, MaNhanVien = "", MaThanhPham = "", MaSize = "", ColorId = x.ColorCode,
                    MaLoaiNguyenLieu = "", TrongLuongTare = "-999", Zero = false,
                    NgayGio = $"{x.CreatedDateTime.ToString("yyyyMMddHHmmss")}"
                }).ToList();
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
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmThe.Items.Where(x => x.NgayGio.Date >= date.Date).OrderBy(x => x.MaNhanVien)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                {
                    Id = x.MaTheTu, MaNhanVien = x.MaNhanVien, MaThanhPham = "", MaSize = "", ColorId = "",
                    MaLoaiNguyenLieu = "", TrongLuongTare = "-999", Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}"
                }).ToList();
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
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date > date.Date && (x.MaSize != null && x.MaSize?.Trim() != ""))
                .OrderBy(x => x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x =>
                    new
                    {
                        Id = x.MaThe, MaNhanVien = "", MaThanhPham = "", MaSize = x.MaSize, ColorId = "",
                        MaLoaiNguyenLieu = "", TrongLuongTare = "-999", Zero = false,
                        NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}"
                    }).ToList();
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
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            if (mainService.VmApp.ComName?.ToUpper().Trim() == nameof(ComNames.RCQTG))
            {
                var items = mainService.VmTheThanhPham.Items
                    .Where(x => x.NgayGio.Date > date.Date && (x.MaThanhPham != null && x.MaThanhPham?.Trim() != "") &&
                                x.MaLoaiNguyenLieu != null && x.MaLoaiNguyenLieu?.Trim() != "").OrderBy(x => x.MaThe)
                    .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                    {
                        Id = x.MaThe, MaNhanVien = "", MaThanhPham = x.MaThanhPham, MaSize = x.MaSize, ColorId = "",
                        MaLoaiNguyenLieu = x.MaLoaiNguyenLieu, TrongLuongTare = "-999", Zero = false,
                        NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}"
                    }).ToList();
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
                var items = mainService.VmTheThanhPham.Items
                    .Where(x => x.NgayGio.Date > date.Date && (x.MaThanhPham != null && x.MaThanhPham?.Trim() != ""))
                    .OrderBy(x => x.MaThe).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x =>
                        new
                        {
                            Id = x.MaThe, MaNhanVien = "", MaThanhPham = x.MaThanhPham, MaSize = "", ColorId = "",
                            MaLoaiNguyenLieu = "", TrongLuongTare = "-999", Zero = false,
                            NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}"
                        }).ToList();
                var json = new
                {
                    data = items,
                    total = items.Count,
                    PageIndex
                };

                return Ok(json);
            }
        }

        [HttpGet]
        public IActionResult GetTheLoaiNguyenLieus(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            //var items = mainService.VmNhanVien.Items.Where(x=>x.MNgay > date).OrderBy(x=>x.MaNhanVien).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id =x.MaNhanVien,MaSo= x.MaHoSo,Ten = x.Name, IsPhucVu = x.IsPhucVu, IsBanKiem = x.IsBanKiem,NgayGioTao = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
            //var json = new
            //{
            //    data = items,
            //    total = items.Count,
            //    PageIndex 
            //};
            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date >= date.Date &&
                            (x.MaLoaiNguyenLieu != null && x.MaLoaiNguyenLieu?.Trim() != "")).OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                {
                    Id = x.MaThe, MaNhanVien = "", MaThanhPham = "", MaSize = x.MaSize, ColorId = "",
                    MaLoaiNguyenLieu = x.MaLoaiNguyenLieu, TrongLuongTare = "-999", Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}"
                }).ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        [HttpGet]
        public IActionResult GetTheThanhPhamChinhXepKhuons(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date >= date.Date && x.MaThanhPhamChinhXepKhuon != null)
                .OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Select(x => new
                {
                    Id = x.MaThe,
                    MaNhanVien = "",
                    MaThanhPham = x.MaThanhPhamChinhXepKhuon,
                    MaSize = "",
                    ColorId = "",
                    MaLoaiNguyenLieu = "",
                    TrongLuongTare = "-999",
                    Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}",
                    LoLevel = 0,
                    MaCoi = "",
                    MaChieuXa = "",
                    MaChatLuong = ""
                })
                .ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        [HttpGet]
        public IActionResult GetTheLoLevels(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            var items = mainService.VmTheThanhPham.Items.Where(x => x.NgayGio.Date >= date.Date && x.LoLevel > 0)
                .OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Select(x => new
                {
                    Id = x.MaThe,
                    MaNhanVien = "",
                    MaThanhPham = "",
                    MaSize = "",
                    ColorId = "",
                    MaLoaiNguyenLieu = "",
                    TrongLuongTare = "-999",
                    Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}",
                    LoLevel = x.LoLevel,
                    MaCoi = "",
                    MaChieuXa = "",
                    MaChatLuong = ""
                })
                .ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        [HttpGet]
        public IActionResult GetTheSizeChinhXepKhuons(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date >= date.Date && x.MaSizeChinhXepKhuon != null)
                .OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Select(x => new
                {
                    Id = x.MaThe,
                    MaNhanVien = "",
                    MaThanhPham = "",
                    MaSize = x.MaSizeChinhXepKhuon,
                    ColorId = "",
                    MaLoaiNguyenLieu = "",
                    TrongLuongTare = "-999",
                    Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}",
                    LoLevel = 0,
                    MaCoi = "",
                    MaChieuXa = "",
                    MaChatLuong = ""
                })
                .ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        [HttpGet]
        public IActionResult GetTheThanhPhamPhuXepKhuons(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date >= date.Date && x.MaThanhPhamChinhXepKhuon != null)
                .OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Select(x => new
                {
                    Id = x.MaThe,
                    MaNhanVien = "",
                    MaThanhPham = x.MaThanhPhamPhuXepKhuon,
                    MaSize = "",
                    ColorId = "",
                    MaLoaiNguyenLieu = "",
                    TrongLuongTare = "-999",
                    Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}",
                    LoLevel = 0,
                    MaCoi = "",
                    MaChieuXa = "",
                    MaChatLuong = ""
                })
                .ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        [HttpGet]
        public IActionResult GetTheSizePhuXepKhuons(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date >= date.Date && x.MaSizePhuXepKhuon != null)
                .OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Select(x => new
                {
                    Id = x.MaThe,
                    MaNhanVien = "",
                    MaThanhPham = "",
                    MaSize = x.MaSizePhuXepKhuon,
                    ColorId = "",
                    MaLoaiNguyenLieu = "",
                    TrongLuongTare = "-999",
                    Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}",
                    LoLevel = 0,
                    MaCoi = "",
                    MaChieuXa = "",
                    MaChatLuong = ""
                })
                .ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        [HttpGet]
        public IActionResult GetTheChatLuongXepKhuons(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }


            var items = mainService.VmTheThanhPham.Items
                .Where(x => x.NgayGio.Date >= date.Date && x.MaChatLuongChinhXepKhuon != null)
                .OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize)
                .Select(x => new
                {
                    Id = x.MaThe,
                    MaNhanVien = "",
                    MaThanhPham = "",
                    MaSize = "",
                    ColorId = "",
                    MaLoaiNguyenLieu = "",
                    TrongLuongTare = "-999",
                    Zero = false,
                    NgayGio = $"{x.NgayGio.ToString("yyyyMMddHHmmss")}",
                    LoLevel = 0,
                    MaCoi = "",
                    MaChieuXa = "",
                    MaChatLuong = x.MaChatLuongChinhXepKhuon
                })
                .ToList();
            var json = new
            {
                data = items,
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        [HttpGet]
        public IActionResult GetDs(string Ngay, int PageIndex, int PageSize)
        {
           


            var items = mainService.VmThe.GetDs(Ngay, PageIndex, PageSize);
            var items2 = mainService.VmTheThanhPham.GetDs(Ngay, PageIndex, PageSize);
            items.AddRange(items2);
            var items3 = mainService.VmTheRo.GetDs(Ngay, PageIndex, PageSize);
            items.AddRange(items3);
            var _items = items.Select(x => new { Id = x }).Distinct().ToList();
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