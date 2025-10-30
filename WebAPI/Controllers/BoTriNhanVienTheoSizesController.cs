using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class BoTriNhanVienTheoSizesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BoTriNhanVienTheoSizesController(dbPMScontext context)
        {
            _context = context;
        }

        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{ngay}/{maLo}/{maXuong}")]
        [Authorize]
        public IActionResult GetAllsFullField(string ngay, string maLo, string maXuong)
        {
            DateTime dateTime = DateTime.Parse(ngay);
            var items = Vm.VmBoTriNhanVienTheoSize.GetsFullField<object>(dateTime, maLo, maXuong, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.BoTriNhanVienTheoSize.ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(BoTriNhanVienTheoSize model)
        {
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
            if (_context.BoTriNhanVienTheoSize.Any(x => x.Ngay == model.Ngay && x.MaNhanVien == model.MaNhanVien && x.MaSize == model.MaSize && x.ThoiGianBatDau == model.ThoiGianBatDau && x.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new BoTriNhanVienTheoSize
            {

                Ngay = model.Ngay,
                MaNhanVien = model.MaNhanVien,
                MaLo = model.MaLo,
                MaSize = model.MaSize,
                ThoiGianBatDau = model.ThoiGianBatDau,
                MaXuong = model.MaXuong,
                SuDung = model.SuDung,

            };
            _context.BoTriNhanVienTheoSize.Add(newItem);
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
        [HttpPost("{maNhanVien}/{ngay}/{maLo}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update(string maNhanVien, string ngay, string maLo, string maXuong, [FromBody] BoTriNhanVienTheoSize model)
        {
            DateTime dateTime = DateTime.Parse(ngay);

            // Lấy danh sách item cũ
            var items = await _context.BoTriNhanVienTheoSize
                .Where(x => x.MaNhanVien == maNhanVien
                         && x.Ngay == dateTime
                         && x.MaLo == maLo
                         && x.MaXuong == maXuong)
                .ToListAsync();

            if (!items.Any())
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại."
                });
            }

            // Kiểm tra model hợp lệ
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

            try
            {
                // Xóa toàn bộ bản ghi cũ
                _context.BoTriNhanVienTheoSize.RemoveRange(items);

                // Thêm mới lại bản ghi với dữ liệu đã thay đổi
                var newItem = new BoTriNhanVienTheoSize
                {
                    Ngay = model.Ngay,
                    MaNhanVien = model.MaNhanVien,
                    MaLo = model.MaLo,
                    MaSize = model.MaSize,  // chỉ thay đổi
                    ThoiGianBatDau = model.ThoiGianBatDau, // chỉ thay đổi
                    MaXuong = model.MaXuong,
                    SuDung = model.SuDung
                };

                _context.BoTriNhanVienTheoSize.Add(newItem);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maNhanVien}/{ngay}/{maLo}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maNhanVien, string ngay, string maLo, string maXuong)
        {
            if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            DateTime dateTime = DateTime.Parse(ngay);
            var item = await _context.BoTriNhanVienTheoSize.Where(x => x.MaNhanVien == maNhanVien && x.Ngay == dateTime && x.MaLo == maLo && x.MaXuong == maXuong).FirstAsync();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            _context.BoTriNhanVienTheoSize.Remove(item);
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

