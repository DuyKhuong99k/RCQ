using Microsoft.AspNetCore.Mvc;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class XiNghiepsController : ControllerBase
{
    private readonly dbPMScontext _context;
    private readonly AppSetting _appSettings;
    public XiNghiepsController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;

    [HttpGet]
    public IActionResult GetAllXuongNoCheckAuth()
    {
        var xuonggs = _context.XiNghiep.OrderByDescending(x => x.Ma).ToList();
        return Ok(xuonggs);
    }
    //[HttpGet]
    //public IActionResult GetConn()
    //{
    //    //_context.Database.GetConnectionString();
    //    return Ok(_context.Database.GetConnectionString());
    //}
    [HttpGet]
    [Authorize]
    public IActionResult GetAllXuong()
    {
        var xuonggs = _context.XiNghiep.OrderByDescending(x => x.Ma).ToList();
        return Ok(xuonggs);
    }
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetXuongById(string id)
    {
        var xuongg = _context.XiNghiep.FirstOrDefault(u => u.Ma == id);
        if (xuongg == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Xưởng không tồn tại."
            });
        }

        return Ok(xuongg);
    }
}