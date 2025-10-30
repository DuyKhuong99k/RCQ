using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Globalization;
using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SizeController(IMainService mainService,IMayCansService mayCansService) : ControllerBase
    {
        [HttpGet]
        public IActionResult Gets(string Ngay,int PageIndex,int PageSize)
        {
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
         
                var items = mainService.VmSizeHq.Items.Where(x=>x.MNgay.Date >=date.Date).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList();
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
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
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
                            var itemNL = mainService.VmSizeNguyenLieu.Items.Where(x => x.SuDung == true).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                            var jsonNL = new
                            {
                                data = itemNL,
                                total = itemNL.Count,
                                PageIndex
                            };
                            return Ok(jsonNL);
                            break;
                        case AppKV.BTPFillet:
                        case AppKV.TPFillet:
                        case AppKV.BTPFilletv2:
                        case AppKV.TPFilletv2:
                            var itemfl = mainService.VmSizeFillet.Items.Where(x => x.SuDung == true).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                            var jsonfl = new
                            {
                                data = itemfl,
                                total = itemfl.Count,
                                PageIndex
                            };
                            return Ok(jsonfl);
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            var itemdinhhinhs = mainService.VmSizeDinhHinh.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.Ten).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Ma,Ten=x.Ten,x.SuDung,MNgay =$"{DateTime.Now.ToString("yyyyMMddHHmmss")}"}).ToList();
                            var jsondinhhinh = new
                            {
                                data = itemdinhhinhs,
                                total = itemdinhhinhs.Count,
                                PageIndex
                            };
                            return Ok(jsondinhhinh);
                            break;
                        case AppKV.XepKhuon:
                            var itemXks = mainService.VmSizeChinhXepKhuon.Items.Where(x => x.SuDung == true).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                            var jsoChinh = new
                            {
                                data = itemXks,
                                total = itemXks.Count,
                                PageIndex
                            };
                            return Ok(jsoChinh);
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            goto case AppKV.XepKhuon;
                        case AppKV.XepKhuonBlock:
                            goto case AppKV.XepKhuon;
                        case AppKV.XepKhuonKXL:
                        case AppKV.XepKhuonPhu:
                            var itemphs = mainService.VmSizePhuXepKhuon.Items.Where(x => x.SuDung == true).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new { Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList();
                            var jsonph = new
                            {
                                data = itemphs,
                                total = itemphs.Count,
                                PageIndex
                            };
                            return Ok(jsonph);
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var itemHqs = mainService.VmSizeHq.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Id,Ten=x.Ten,x.SuDung,MNgay =$"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList();
                            var json = new
                            {
                                data = itemHqs,
                                total = itemHqs.Count,
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
        public IActionResult GetUs(string Ngay,int PageIndex,int PageSize)
        {
           
             var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
            {
         
                var items = mainService.VmSizeHq.GetUs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x=>new {Id=x.SizeId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
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
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
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
                            var itemNL = mainService.VmSizeNguyenLieu.GetUs(mNgay, PageIndex, PageSize);
                            var jsonNL = new
                            {
                                data = itemNL.Select(x=>new {Id=x.MaSize,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                                total = itemNL.Count,
                                PageIndex
                            };
                            return Ok(jsonNL);
                            break;
                        case AppKV.BTPFillet:
                        case AppKV.TPFillet:
                        case AppKV.BTPFilletv2:
                        case AppKV.TPFilletv2:
                            var itemfl = mainService.VmSizeFillet.GetUs(mNgay, PageIndex, PageSize);
                            var jsonfl = new
                            {
                                data = itemfl.Select(x=>new {Id=x.MaSize,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                                total = itemfl.Count,
                                PageIndex
                            };
                            return Ok(jsonfl);
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            //var itemdinhhinhs = mainService.VmSizeDinhHinh.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.Ten).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Ma,Ten=x.Ten,x.SuDung,MNgay =$"{DateTime.Now.ToString("yyyyMMddHHmmss")}"}).ToList();
                            //var jsondinhhinh = new
                            //{
                            //    data = itemdinhhinhs,
                            //    total = itemdinhhinhs.Count,
                            //    PageIndex
                            //};
                            //return Ok(jsondinhhinh);
                            break;
                        case AppKV.XepKhuon:
                            var itemXks = mainService.VmSizeChinhXepKhuon.GetUs(mNgay, PageIndex, PageSize);
                            var jsoChinh = new
                            {
                                data = itemXks.Select(x=>new {Id=x.MaSize,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                                total = itemXks.Count,
                                PageIndex
                            };
                            return Ok(jsoChinh);
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            break;
                       
                        case AppKV.XepKhuonKXL:
                        case AppKV.XepKhuonPhu:
                            var itemphs = mainService.VmSizePhuXepKhuon.GetUs(mNgay, PageIndex, PageSize);
                            var jsonph = new
                            {
                                data = itemphs.Select(x => new { Id = x.MaSize, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
                                total = itemphs.Count,
                                PageIndex
                            };
                            return Ok(jsonph);
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var items = mainService.VmSizeHq.GetUs(mNgay, PageIndex, PageSize);
                            var json = new
                            {
                                data = items.Select(x=>new {Id=x.SizeId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
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
        public IActionResult GetDs(string Ngay,int PageIndex,int PageSize)
        {
             var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            var date = new DateTime();
            if (index == -1)
            {
         
                var items = mainService.VmSizeHq.GetDs(Ngay, PageIndex, PageSize);
                var json = new
                {
                    data = items.Select(x=>new {Id=x.SizeId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
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
                date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
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
                            var itemNL = mainService.VmSizeNguyenLieu.GetDs(mNgay, PageIndex, PageSize);
                            var jsonNL = new
                            {
                                data = itemNL.Select(x=>new {Id=x.MaSize,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                                total = itemNL.Count,
                                PageIndex
                            };
                            return Ok(jsonNL);
                            break;
                        case AppKV.BTPFillet:
                        case AppKV.TPFillet:
                        case AppKV.BTPFilletv2:
                        case AppKV.TPFilletv2:
                            var itemfl = mainService.VmSizeFillet.GetDs(mNgay, PageIndex, PageSize);
                            var jsonfl = new
                            {
                                data = itemfl.Select(x=>new {Id=x.MaSize,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                                total = itemfl.Count,
                                PageIndex
                            };
                            return Ok(jsonfl);
                            break;
                        case AppKV.PhuPham:
                            break;
                        case AppKV.BTPDinhHinh:

                        case AppKV.TPDinhHinh:
                            //var itemdinhhinhs = mainService.VmSizeDinhHinh.Items.Where(x=>x.SuDung == true).OrderByDescending(x=>x.Ten).Skip((PageIndex -1)* PageSize).Take(PageSize).Select(x=>new {Id=x.Ma,Ten=x.Ten,x.SuDung,MNgay =$"{DateTime.Now.ToString("yyyyMMddHHmmss")}"}).ToList();
                            //var jsondinhhinh = new
                            //{
                            //    data = itemdinhhinhs,
                            //    total = itemdinhhinhs.Count,
                            //    PageIndex
                            //};
                            //return Ok(jsondinhhinh);
                            break;
                        case AppKV.XepKhuon:
                            var itemXks = mainService.VmSizeChinhXepKhuon.GetDs(mNgay, PageIndex, PageSize);
                            var jsoChinh = new
                            {
                                data = itemXks.Select(x=>new {Id=x.MaSize,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
                                total = itemXks.Count,
                                PageIndex
                            };
                            return Ok(jsoChinh);
                            break;
                        case AppKV.BaoTu:
                            break;
                        case AppKV.CaoThit:
                            break;
                        case AppKV.XepKhuonRaCoi:
                            break;
                       
                        case AppKV.XepKhuonKXL:
                        case AppKV.XepKhuonPhu:
                            var itemphs = mainService.VmSizePhuXepKhuon.GetDs(mNgay, PageIndex, PageSize);
                            var jsonph = new
                            {
                                data = itemphs.Select(x => new { Id = x.MaSize, Ten = x.Ten, x.SuDung, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" }).ToList(),
                                total = itemphs.Count,
                                PageIndex
                            };
                            return Ok(jsonph);
                            break;
                        case AppKV.PhuPhamv2:
                            break;
                        case AppKV.Hq:
                            var items = mainService.VmSizeHq.GetDs(mNgay, PageIndex, PageSize);
                            var json = new
                            {
                                data = items.Select(x=>new {Id=x.SizeId,Ten=x.Ten,x.SuDung,MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"}).ToList(),
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
