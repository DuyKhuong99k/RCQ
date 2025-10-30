using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Vars;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MauController(IMainService mainService, IMayCansService mayCansService) : ControllerBase
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
                //throw ex;
            }

            var items = mainService.VmMauBlockXepKhuon.Items.Where(x => x.SuDung == true)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                {
                    Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}"
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
            var _data = Ngay.Substring(index + 1).Split(',');
            var mayCanId = _data.FirstOrDefault() ?? "";
            var mNgay = _data.LastOrDefault() ?? "";
            DateTime date = new DateTime();
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
                    {
                        var items = mainService.VmMauBlockXepKhuon.Items.Where(x => x.SuDung == true)
                            .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                            {
                                Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}"
                            }).ToList();
                        var json = new
                        {
                            data = items,
                            total = items.Count,
                            PageIndex
                        };

                        return Ok(json);
                    }
                        break;
                    case AppKV.BaoTu:
                        break;
                    case AppKV.CaoThit:
                        break;
                    case AppKV.XepKhuonRaCoi:
                        goto case AppKV.XepKhuon;
                    case AppKV.XepKhuonPhu:
                        goto case AppKV.XepKhuon;
                    case AppKV.XepKhuonKXL:
                        break;
                    case AppKV.PhuPhamv2:
                        break;
                    case AppKV.XepKhuonBlock:
                        goto case AppKV.XepKhuon;
                    case AppKV.Hq:
                    {
                        var itemshq = mainService.VmMauBlockXepKhuon.Items.Where(x => x.SuDung == true)
                            .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                            {
                                Id = x.Ma, Ten = x.Ten, x.SuDung, MNgay = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}"
                            }).ToList();
                        var jsonhq = new
                        {
                            data = itemshq,
                            total = itemshq.Count,
                            PageIndex
                        };

                        return Ok(jsonhq);
                    }
                        break;
                }
            }
        }

        return NotFound("Not Found");
    }
}