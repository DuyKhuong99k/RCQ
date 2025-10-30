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

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NhanVienTheoLinesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanVienTheoLinesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.NhanVienTheoLine.ToList();

            return Ok(items);
        }


        [HttpGet("{ngay}/{maLine}/{maViTri}")]
        [Authorize]
        public IActionResult GetAllsFullField(string ngay, string maLine, string maViTri)
        {
            DateTime date = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVienTheoLine.GetsFullField<object>(date, maLine, maViTri);
            return Ok(items);
        }
        [HttpGet("{ngay}/{maLine}/{maViTri}")]
        [Authorize]
        public IActionResult GetsFullFieldLastNew(string ngay, string maLine, string maViTri)
        {
            DateTime date = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVienTheoLine.GetsFullFieldLastNew<object>(date, maLine, maViTri);
            return Ok(items);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<NhanVienTheoLine>(dataT.Item1);
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
            if (_context.NhanVienTheoLine.Any(x =>x.MaNhanVien == model.MaNhanVien && x.CodeId == model.CodeId && x.Ngay == model.Ngay && x.MaLine == model.MaLine && x.MaViTri == model.MaViTri && x.Gio == model.Gio))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new NhanVienTheoLine
            {
                Id = model.Id,
                CodeId = model.CodeId,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaNhanVien = model.MaNhanVien,
                MaLine = model.MaLine,
                MaViTri = model.MaViTri,
            };
            _context.NhanVienTheoLine.Add(newItem);
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
            var item = await _context.NhanVienTheoLine.Where(x => x.Id == id).FirstAsync();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            _context.NhanVienTheoLine.Remove(item);
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
}
