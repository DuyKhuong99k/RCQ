using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class LoController(IMainService mainService, IMayCansService mayCansService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetDs(string Ngay, int PageIndex, int PageSize)
    {
        
        var index = Ngay.IndexOf("=", StringComparison.Ordinal);
        if (index == -1)
        {
            
            var items = mainService.VmLoHq.GetDs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                {
                   Id = x.LoId, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
                    NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
                }),
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        var _data = Ngay.Substring(index + 1).Split(',');
        var mayCanId = _data.FirstOrDefault() ?? "";
        var mNgay = _data.LastOrDefault() ?? "";
        DateTime date = new DateTime();
        date = DateTime.ParseExact(mNgay, "yyyyMMdd", CultureInfo.InvariantCulture);
        var mayCan = mayCansService.Find(mayCanId);
        if (mayCan != null)
            
        {
            //case AppKV.Main:
            //case AppKV.DauAo:
            //case AppKV.NguyenLieu:
            //case AppKV.BTPFillet:
            //case AppKV.TPFillet:
            //case AppKV.BTPFilletv2:
            //case AppKV.TPFilletv2:
            //case AppKV.PhuPham:
            //case AppKV.BTPDinhHinh:
            //case AppKV.TPDinhHinh:
            //case AppKV.XepKhuon:
            //case AppKV.BaoTu:
            //case AppKV.CaoThit:
            //case AppKV.XepKhuonRaCoi:
            //case AppKV.XepKhuonPhu:
            //case AppKV.XepKhuonKXL:
            //case AppKV.PhuPhamv2:
            //    var itemsppv2 = mainService.VmLoHq.Gets(Ngay, PageIndex, PageSize);
            //    var jsonppv2 = new
            //    {
            //        data = itemsppv2.Select(x => new
            //        {
            //            x.Id, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
            //            NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
            //        }),
            //        total = itemsppv2.Count,
            //        PageIndex
            //    };

            //    return Ok(jsonppv2);
            //    break;
            //case AppKV.Hq:
            var items = mainService.VmLoHq.GetDs(mNgay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                {
                    Id =  x.LoId, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
                    NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
                }),
                total = items.Count,
                PageIndex
            };

            return Ok(json);
            //break;
        }

        return NotFound("Not Found");
    }

    [HttpGet]
    public IActionResult Gets(string Ngay, int PageIndex, int PageSize)
    {
        var index = Ngay.IndexOf("=", StringComparison.Ordinal);
        if (index == -1)
        {
            
            var items = mainService.VmLoHq.Gets(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                {
                    x.Id, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
                    NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
                }),
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        var _data = Ngay.Substring(index + 1).Split(',');
        var mayCanId = _data.FirstOrDefault() ?? "";
        var mNgay = _data.LastOrDefault() ?? "";
        DateTime date = new DateTime();
        date = DateTime.ParseExact(mNgay, "yyyyMMdd", CultureInfo.InvariantCulture);
        var mayCan = mayCansService.Find(mayCanId);
        if (mayCan != null)
            
        {
            // case AppKV.Main:
            // case AppKV.DauAo:
            // case AppKV.NguyenLieu:
            // case AppKV.BTPFillet:
            // case AppKV.TPFillet:
            // case AppKV.BTPFilletv2:
            // case AppKV.TPFilletv2:
            // case AppKV.PhuPham:
            // case AppKV.BTPDinhHinh:
            // case AppKV.TPDinhHinh:
            // case AppKV.XepKhuon:
            // case AppKV.BaoTu:
            // case AppKV.CaoThit:
            // case AppKV.XepKhuonRaCoi:
            // case AppKV.XepKhuonPhu:
            // case AppKV.XepKhuonKXL:
            // case AppKV.PhuPhamv2:
            //     var itemsppv2 = mainService.VmLoHq.Gets(Ngay, PageIndex, PageSize);
            //     var jsonppv2 = new
            //     {
            //         data = itemsppv2.Select(x => new
            //         {
            //             x.Id, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
            //             NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
            //         }),
            //         total = itemsppv2.Count,
            //         PageIndex
            //     };
            //
            //     return Ok(jsonppv2);
            //     break;
            // case AppKV.Hq:
            switch (mainService.VmApp.LoKv)
            {
                case AppKV.Main:
                case AppKV.DauAo:
                case AppKV.NguyenLieu:
                {
                    var items = mainService.VmLoHq.GetsLastWithSize(mayCan.MaXuong); //(mNgay, PageIndex, PageSize);
                    var json = new
                    {
                        data = items.Select(x => new
                        {
                            Id =  x.Item2, MNgay = x.Item1?.ToString("yyyyMMdd"), SuDung = true,
                            NgayNguyenLieu = $"{x.Item1?.ToString("yyyyMMddHHmmss")}"
                        }),
                        total = items.Count,
                        PageIndex
                    };

                    return Ok(json);
                }
                case AppKV.BTPFillet:
                case AppKV.TPFillet:
                case AppKV.BTPFilletv2:
                case AppKV.TPFilletv2:
                case AppKV.PhuPham:
                case AppKV.BTPDinhHinh:
                case AppKV.TPDinhHinh:
                case AppKV.XepKhuon:
                case AppKV.BaoTu:
                case AppKV.CaoThit:
                case AppKV.XepKhuonRaCoi:
                case AppKV.XepKhuonPhu:
                case AppKV.XepKhuonKXL:
                case AppKV.PhuPhamv2:
                {
                    var itemsppv2 = mainService.VmLoHq.Gets(mNgay, PageIndex, PageSize);
                    var jsonppv2 = new
                    {
                        data = itemsppv2.Select(x => new
                        {
                            x.Id, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
                            NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
                        }),
                        total = itemsppv2.Count,
                        PageIndex
                    };

                    return Ok(jsonppv2);
                    break;
                }
                   
                case AppKV.Hq:
                {
                    var items = mainService.VmLoHq.Gets(mNgay, PageIndex, PageSize);
                    var json = new
                    {
                        data = items.Select(x => new
                        {
                            Id =  x.Id, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
                            NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
                        }),
                        total = items.Count,
                        PageIndex
                    };

                    return Ok(json);
                }
                   
            }
            
            //break;
        }

        return NotFound("Not Found");
        //switch (Ngay)
        //{
        //    case nameof(AppKV.Hq):
        //        var items = mainService.VmLoHq.Gets(Ngay, PageIndex, PageSize);
        //        var json = new
        //        {
        //            data = items.Select(x => new
        //            {
        //                x.Id, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
        //                NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
        //            }),
        //            total = items.Count,
        //            PageIndex
        //        };

        //        return Ok(json);
        //        break;
        //    default:
        //        var itemdefauts = mainService.VmLo.Gets(mainService.VmXiNghiep.SelectedItem?.Ma ?? "")
        //            .Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        //        var jsondefauts = new
        //        {
        //            data = itemdefauts.Select(x => new
        //            {
        //                Id = x, MNgay = DateTime.Now.ToString("yyyyMMdd"), SuDung = true,
        //                NgayNguyenLieu = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}"
        //            }),
        //            total = itemdefauts.Count,
        //            PageIndex
        //        };
        //        return Ok(jsondefauts);
        //        break;
        //}

        //return NotFound("Not Found");
    }

    [HttpGet]
    public IActionResult GetUs(string Ngay, int PageIndex, int PageSize)
    {
       
        var index = Ngay.IndexOf("=", StringComparison.Ordinal);
        if (index == -1)
        {
            
            var items = mainService.VmLoHq.GetUs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                {
                    Id =  x.LoId, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
                    NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
                }),
                total = items.Count,
                PageIndex
            };

            return Ok(json);
        }

        var _data = Ngay.Substring(index + 1).Split(',');
        var mayCanId = _data.FirstOrDefault() ?? "";
        var mNgay = _data.LastOrDefault() ?? "";
        DateTime date = new DateTime();
        date = DateTime.ParseExact(mNgay, "yyyyMMdd", CultureInfo.InvariantCulture);
        var mayCan = mayCansService.Find(mayCanId);
        if (mayCan != null)
            
        {
            //case AppKV.Main:
            //case AppKV.DauAo:
            //case AppKV.NguyenLieu:
            //case AppKV.BTPFillet:
            //case AppKV.TPFillet:
            //case AppKV.BTPFilletv2:
            //case AppKV.TPFilletv2:
            //case AppKV.PhuPham:
            //case AppKV.BTPDinhHinh:
            //case AppKV.TPDinhHinh:
            //case AppKV.XepKhuon:
            //case AppKV.BaoTu:
            //case AppKV.CaoThit:
            //case AppKV.XepKhuonRaCoi:
            //case AppKV.XepKhuonPhu:
            //case AppKV.XepKhuonKXL:
            //case AppKV.PhuPhamv2:
            //    var itemsppv2 = mainService.VmLoHq.Gets(Ngay, PageIndex, PageSize);
            //    var jsonppv2 = new
            //    {
            //        data = itemsppv2.Select(x => new
            //        {
            //            x.Id, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
            //            NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
            //        }),
            //        total = itemsppv2.Count,
            //        PageIndex
            //    };

            //    return Ok(jsonppv2);
            //    break;
            //case AppKV.Hq:
            var items = mainService.VmLoHq.GetUs(mNgay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                {
                    Id =  x.LoId, MNgay = x.MNgay.ToString("yyyyMMdd"), x.SuDung,
                    NgayNguyenLieu = $"{x.MNgay.ToString("yyyyMMddHHmmss")}"
                }),
                total = items.Count,
                PageIndex
            };

            return Ok(json);
            //break;
        }

        return NotFound("Not Found");
    }
}