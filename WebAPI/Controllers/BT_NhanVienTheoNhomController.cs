using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppViewModels;
using ViewModels.Repos.HQ;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BT_NhanVienTheoNhomController : ControllerBase
{
    private readonly dbPMScontext _context;

    public BT_NhanVienTheoNhomController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;
    [HttpGet]
    [Authorize]
    public IActionResult GetAlls()
    {
        var items = _context.BT_NhanVienTheoNhom.ToList();

        return Ok(items);
    }
    [HttpGet("{maNhom}/{dateTime}/{maXuong}")]
    [Authorize]
    public IActionResult GetAllByMaNhoms(string maNhom, string dateTime, string maXuong)
    {
        DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var items = _context.BT_NhanVienTheoNhom
                    .Where(x => x.MaNhom == maNhom && x.Ngay == date1 && x.MaXuong == maXuong)
                    .ToList();

        return Ok(items);
    }
    [HttpGet("{maNhanVien}/{maNhom}/{dateTime}/{maXuong}")]
    [Authorize]
    public IActionResult GetsByMa(string maNhanVien, string maNhom, string dateTime,string maXuong)
    {
        DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var item = _context.BT_NhanVienTheoNhom.FirstOrDefault(x => x.MaNhanVien == maNhanVien && x.MaNhom == maNhom && x.Ngay == date1 && x.MaXuong == maXuong);
        if (item == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Mã này không tồn tại!."
            });
        }
        return Ok(item);
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Insert( Tuple<string> dataT)
    {
        var model = JsonSerializer.Deserialize<BT_NhanVienTheoNhom>(dataT.Item1);
        //kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
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
        // ... kiểm tra mã nhân viên đã tồn tại chưa ...
        if (_context.BT_NhanVienTheoNhom.Any(x => x.MaNhanVien == model.MaNhanVien && x.MaNhom == model.MaNhom && x.Ngay == model.Ngay))
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Mã này đã tồn tại.",
            });
        }
        var newItem = new BT_NhanVienTheoNhom
        {
            MaNhanVien = model.MaNhanVien,
            MaNhom = model.MaNhom,
            Ngay = model.Ngay,
            TyLeHuong = model.TyLeHuong,
            TyLeTru = model.TyLeTru,
            SoGio = model.SoGio,
        };
        _context.BT_NhanVienTheoNhom.Add(newItem);
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
    //[HttpPost("{maNhanVien}/{maNhom}/{dateTime}")]
    //[Authorize]
    //public async Task<IActionResult> Update(string maNhanVien, string maNhom, string dateTime, Tuple<string> dataT)
    //{
    //    DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    //    var model = JsonSerializer.Deserialize<BT_NhanVienTheoNhom>(dataT.Item1);
    //    // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
    //    if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maNhom) || string.IsNullOrEmpty(dateTime))
    //    {
    //        return BadRequest(new ApiResponse
    //        {
    //            Success = false,
    //            Message = "Mã này không hợp lệ."
    //        });
    //    }

    //    // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
    //    var item = await _context.BT_NhanVienTheoNhom.FirstOrDefaultAsync(x => x.MaNhanVien == maNhanVien && x.MaNhom == maNhom && x.Ngay == date1);
    //    if (item == null)
    //    {
    //        return BadRequest(new ApiResponse
    //        {
    //            Success = false,
    //            Message = "Mã này không tồn tại."
    //        });
    //    }

    //    // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
    //    if (!ModelState.IsValid)
    //    {
    //        var errors = ModelState.Values.SelectMany(v => v.Errors)
    //                                      .Select(e => e.ErrorMessage)
    //                                      .ToList();

    //        return BadRequest(new ApiResponse
    //        {
    //            Success = false,
    //            Message = "Dữ liệu không hợp lệ.",
    //            Errors = errors
    //        });
    //    }
    //    item.TyLeHuong = model.TyLeHuong;
    //    item.TyLeTru = model.TyLeTru;
    //    item.SoGio = model.SoGio;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new ApiResponse
    //        {
    //            Success = false,
    //            Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
    //            Errors = new List<string> { ex.Message }
    //        });
    //    }
    //    return Ok(new ApiResponse
    //    {
    //        Success = true,
    //        Message = "Cập nhật thông tin thành công!"
    //    });
    //}
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete([FromBody] List<BT_NhanVienTheoNhom> maList)
    {
        if (maList == null || !maList.Any())
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Bạn chưa chọn thông tin!"
            });
        }

        var itemsToDelete = await _context.BT_NhanVienTheoNhom
            .Where(x => maList.Any(m => m.MaNhanVien == x.MaNhanVien && m.MaNhom == x.MaNhom && m.Ngay == x.Ngay && x.MaXuong == m.MaXuong))
            .ToListAsync();

        if (!itemsToDelete.Any())
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Không có mục nào tồn tại."
            });
        }

        _context.BT_NhanVienTheoNhom.RemoveRange(itemsToDelete);

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
            Message = "Đã xoá các mục!"
        });
    }

}