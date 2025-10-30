using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Repos.Hubs.IServices;

namespace PMSHub.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class MauTheController(IMainService mainService, IMayCansService mayCansService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetDs(string Ngay, int PageIndex, int PageSize)
    {
        
        var index = Ngay.IndexOf("=", StringComparison.Ordinal);
        var date = new DateTime();
        if (index == -1)
        {
            //
            var items = mainService.VmColorCode.GetDs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                        { Id = x.Code, Ten = x.Name, x.Code, x.Code2, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" })
                    .ToList(),
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
            //c
            var items = mainService.VmColorCode.GetDs(mNgay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                        { Id = x.Code, Ten = x.Name, x.Code, x.Code2, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" })
                    .ToList(),
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
        var date = new DateTime();
        if (index == -1)
        {
            //
            date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var items = mainService.VmColorCode.Items.Where(x => x.MNgay.Date >= date.Date).OrderBy(x => x.Code)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                    { Id = x.Code, Ten = x.Name, x.Code, x.Code2, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" })
                .ToList();
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
            //c
            var items = mainService.VmColorCode.Items.Where(x => x.MNgay.Date >= date.Date).OrderBy(x => x.Code)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new
                    { Id = x.Code, Ten = x.Name, x.Code, x.Code2, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" })
                .ToList();
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
    public IActionResult GetUs(string Ngay, int PageIndex, int PageSize)
    {
       
        var index = Ngay.IndexOf("=", StringComparison.Ordinal);
        var date = new DateTime();
        if (index == -1)
        {
            //
            var items = mainService.VmColorCode.GetUs(Ngay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                        { Id = x.Code, Ten = x.Name, x.Code, x.Code2, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" })
                    .ToList(),
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
            //c
            var items = mainService.VmColorCode.GetUs(mNgay, PageIndex, PageSize);
            var json = new
            {
                data = items.Select(x => new
                        { Id = x.Code, Ten = x.Name, x.Code, x.Code2, MNgay = $"{x.MNgay.ToString("yyyyMMddHHmmss")}" })
                    .ToList(),
                total = items.Count,
                PageIndex
            };

            return Ok(json);
            //break;
        }

        return NotFound("Not Found");
    }
}