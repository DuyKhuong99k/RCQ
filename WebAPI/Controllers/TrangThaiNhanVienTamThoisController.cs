using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TrangThaiNhanVienTamThoisController : ControllerBase
{
    private readonly dbPMScontext _context;

    public TrangThaiNhanVienTamThoisController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;

    [HttpGet("{dateTime}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<object>>> GetDanhSachNhanVienTrangThaiTamThois(string dateTime)
    {
        if (_context.TrangThaiNhanVienTamThoi == null)
        {
            return NotFound();
        }
        // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        var items = Vm.VmTrangThaiNhanVienTamThoi.Gets<object>(date);
        return items;
    }
}