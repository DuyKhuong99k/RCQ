using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BoTriLoSizeThanhPhamsController : ControllerBase
{
    private readonly dbPMScontext _context;

    public BoTriLoSizeThanhPhamsController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;

    [HttpGet]
    [Authorize]
    public IActionResult GetAlls()
    {
        var items = _context.BoTriLoSizeThanhPham.ToList();

        return Ok(items);
    }

    [HttpGet("{ngay}/{maLo}")]
    [Authorize]
    public IActionResult GetAllsFullField(string ngay, string maLo)
    {
        DateTime date = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        var items = Vm.VmBoTriLoSizeThanhPham.GetsFullField<object>(date, maLo);
        return Ok(items);
    }
    [HttpGet("{ngay}/{maLo}")]
    [Authorize]
    public IActionResult GetsFullFieldLastNew(string ngay, string maLo)
    {
        DateTime date = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        var items = Vm.VmBoTriLoSizeThanhPham.GetsFullFieldLastNew<object>(date, maLo);
        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Insert(Tuple<string> dataT)
    {
        var model = JsonSerializer.Deserialize<BoTriLoSizeThanhPham>(dataT.Item1);
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();

            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ.",
                Errors = errors
            });
        }
        if (_context.BoTriLoSizeThanhPham.Any(x => x.Id == model.Id))
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Mã này đã tồn tại.",
            });
        }
        var newItem = new BoTriLoSizeThanhPham
        {
            Id = model.Id,
            CodeId = model.CodeId,
            MaLo = model.MaLo,
            MaViTri = model.MaViTri,
            MaSize = model.MaSize,
            MaThanhPham = model.MaThanhPham,
            Ngay = model.Ngay,
            Gio = model.Gio,
            MaSizePhu = model.MaSizePhu
        };

        _context.BoTriLoSizeThanhPham.Add(newItem);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi lưu dữ liệu." + ex.Message.ToString(),
                Errors = new List<string> { ex.Message.ToString() }
            });
        }
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Thêm thành công!"
        });
    }

    [HttpPost("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == null || id <= 0)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Bạn chưa chọn thông tin!."
            });
        }
        var item = await _context.BoTriLoSizeThanhPham.Where(x => x.Id == id).FirstAsync();
        if (item == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Không tồn tại."
            });
        }
        _context.BoTriLoSizeThanhPham.Remove(item);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi xóa.",
                Errors = new List<string> { ex.Message }
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Đã xoá!"
        });
    }
}
