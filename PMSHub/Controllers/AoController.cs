using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vars;
using ViewModels.Repos.Hubs.IServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMSHub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AoController(IMainService mainService, IMayCansService mayCansService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay, int PageIndex, int PageSize)
        {
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                DateTime date = new DateTime();
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw;
                }
                var items = mainService.VmAoNguyenLieu.ItemsAo.Where(x => x.MNgay.Date >= date.Date).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
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
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw;
                }
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    switch (mayCan.WKv)
                    {
                        case AppKV.Main:
                            break;
                        case AppKV.DauAo:
                           
                            break;
                        case AppKV.NguyenLieu:
                            var itemNL = mainService.VmAoNguyenLieu.ItemsAo.Where(x => x.SuDung == true ).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                            var jsonNL = new
                            {
                                data = itemNL,
                                total = itemNL.Count,
                                PageIndex
                            };

                            return Ok(jsonNL);
                            break;
                        case AppKV.BTPFillet:
                            break;
                        case AppKV.TPFillet:
                            break;
                        case AppKV.BTPFilletv2:
                            break;
                        case AppKV.TPFilletv2:
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            break;
                        case AppKV.XepKhuon:
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            break;
                        case AppKV.XepKhuonPhu:
                            break;
                        case AppKV.XepKhuonKXL:
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var items = mainService.VmAoNguyenLieu.ItemsAo.Where(x => x.MNgay.Date >= date.Date).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                            var json = new
                            {
                                data = items,
                                total = items.Count,
                                PageIndex
                            };
                            return Ok(json);
                            break;
                    }
                }
            }
            return NotFound("Not Found");
        }
        [HttpGet]
        public IActionResult GetUs(string Ngay, int PageIndex, int PageSize)
        {
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                var items = mainService.VmAoNguyenLieu.GetUs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x => new { Id = x.MaAo, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
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
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw;
                }
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    switch (mayCan.WKv)
                    {
                        case AppKV.Main:
                            break;
                        case AppKV.DauAo:
                           
                            break;
                        case AppKV.NguyenLieu:
                            var itemNLs = mainService.VmAoNguyenLieu.GetUs(mNgay, PageIndex, PageSize);
                            var jsonNL = new
                            {
                                data = itemNLs.Select(x => new { Id = x.MaAo, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
                                total = itemNLs.Count,
                                PageIndex
                            };

                            return Ok(jsonNL);
                            break;
                        case AppKV.BTPFillet:
                            break;
                        case AppKV.TPFillet:
                            break;
                        case AppKV.BTPFilletv2:
                            break;
                        case AppKV.TPFilletv2:
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            break;
                        case AppKV.XepKhuon:
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            break;
                        case AppKV.XepKhuonPhu:
                            break;
                        case AppKV.XepKhuonKXL:
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var items = mainService.VmAoNguyenLieu.GetUs(mNgay, PageIndex, PageSize);
                            var json = new
                            {
                                data = items.Select(x => new { Id = x.MaAo, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
                                total = items.Count,
                                PageIndex
                            };

                            return Ok(json);
                            break;
                    }
                }
            }
            return NotFound("Not Found");
           
        }
        [HttpGet]
        public IActionResult GetDs(string Ngay, int PageIndex, int PageSize)
        {
           
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                var items = mainService.VmAoNguyenLieu.GetDs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x => new { Id = x.MaAo, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
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
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw;
                }
                var mayCan = mayCansService.Find(mayCanId);
                if (mayCan != null)
                {
                    switch (mayCan.WKv)
                    {
                        case AppKV.Main:
                            break;
                        case AppKV.DauAo:
                           
                            break;
                        case AppKV.NguyenLieu:
                            var itemNLs = mainService.VmAoNguyenLieu.GetDs(mNgay, PageIndex, PageSize);
                            var jsonNL = new
                            {
                                data = itemNLs.Select(x => new { Id = x.MaAo, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
                                total = itemNLs.Count,
                                PageIndex
                            };

                            return Ok(jsonNL);
                            break;
                        case AppKV.BTPFillet:
                            break;
                        case AppKV.TPFillet:
                            break;
                        case AppKV.BTPFilletv2:
                            break;
                        case AppKV.TPFilletv2:
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            break;
                        case AppKV.XepKhuon:
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            break;
                        case AppKV.XepKhuonPhu:
                            break;
                        case AppKV.XepKhuonKXL:
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var items = mainService.VmAoNguyenLieu.GetDs(mNgay, PageIndex, PageSize);
                            var json = new
                            {
                                data = items.Select(x => new { Id = x.MaAo, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
                                total = items.Count,
                                PageIndex
                            };

                            return Ok(json);
                            break;
                    }
                }
            }
            return NotFound("Not Found");
        }
    }
}
