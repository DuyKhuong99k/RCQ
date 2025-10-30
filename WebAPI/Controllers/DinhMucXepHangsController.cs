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
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DinhMucXepHangsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public DinhMucXepHangsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{year}")]
        [Authorize]
        public IActionResult GetAllsFullFieldByYear(int year)
        {
            var items = Vm.VmDinhMucXepHang.GetsFullFieldByYear<object>(year, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{year}")]
        [Authorize]
        public IActionResult GetAllsByYear(int year)
        {
            var items = _context.DinhMucXepHang.Where(x => x.Year == year).ToList();

            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.DinhMucXepHang.ToList();

            return Ok(items);
        }
        [HttpGet("{maLo}/{maSanPham}/{maXepHang}/{year}")]
        [Authorize]
        public IActionResult GetsByMa(string maLo, string maSanPham, string maXepHang, int year)
        {
            var item = _context.DinhMucXepHang.FirstOrDefault(x => x.MaLo == maLo && x.MaSanPham == maSanPham && x.MaXepHang == maXepHang && x.Year == year);
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
        public async Task<IActionResult> Insert(DinhMucXepHang model)
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
            if (_context.DinhMucXepHang.Any(u => u.MaLo == model.MaLo && u.MaSanPham == model.MaSanPham && u.MaXepHang == model.MaXepHang && u.Year == model.Year))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new DinhMucXepHang
            {
                MaLo = model.MaLo,
                MaSanPham = model.MaSanPham,
                MaXepHang = model.MaXepHang,
                DinhMucUp = model.DinhMucUp,
                Year = model.Year,
                DinhMucDown = model.DinhMucDown,
                DinhMuc = model.DinhMuc
            };
            _context.DinhMucXepHang.Add(newItem);
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
        [HttpPost("{maLo}/{maSanPham}/{maXepHang}/{year}")]
        [Authorize]
        public async Task<IActionResult> Update(string maLo, string maSanPham, string maXepHang, int year, [FromBody] DinhMucXepHang model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maXepHang) || year == null || year == 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra có tồn tại trong cơ sở dữ liệu không
            var item = await _context.DinhMucXepHang.FirstOrDefaultAsync(x => x.MaLo == maLo && x.MaSanPham == maSanPham && x.MaXepHang == maXepHang && x.Year == year);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
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
            item.MaLo = model.MaLo;
            item.MaSanPham = model.MaSanPham;
            item.MaXepHang = model.MaXepHang;
            item.DinhMucUp = model.DinhMucUp;
            item.Year = model.Year;
            item.DinhMucDown = model.DinhMucDown;
            item.DinhMuc = model.DinhMuc;
            try
            {
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
        [HttpPost("{maLo}/{maSanPham}/{maXepHang}/{year}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maLo, string maSanPham, string maXepHang, int year)
        {
            if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maXepHang) || year == null || year == 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Có Vẻ Đang Thiếu Thông Tin! Không Tìm Thấy!"
                });
            }
            var item = await _context.DinhMucXepHang.FirstOrDefaultAsync(x => x.MaLo == maLo && x.MaSanPham == maSanPham && x.MaXepHang == maXepHang && x.Year == year);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.DinhMucXepHang.Remove(item);

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
