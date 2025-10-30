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
    public class MaThanhPhamFillet_HanMucTrongLuongController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamFillet_HanMucTrongLuongController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{ngay}/{maXuong}")]
        [Authorize]
        public IActionResult GetAllsFullField(string ngay, string maXuong)
        {
            DateTime dateTime = DateTime.Parse(ngay);
            var items = Vm.VmThanhPhamFillet_HanMucTrongLuong.GetsFullField<object>(dateTime, maXuong, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaThanhPhamFillet_HanMucTrongLuong.ToList();

            return Ok(items);
        }
        [HttpGet("{stt}/{ngay}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(string stt, string ngay, string maXuong)
        {
            int number = int.Parse(stt);
            DateTime dateTime = DateTime.Parse(ngay);
            var item = _context.MaThanhPhamFillet_HanMucTrongLuong.FirstOrDefault(x => x.STT == number && x.Ngay == dateTime && x.MaXuong == maXuong);
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
        public async Task<IActionResult> Insert(MaThanhPhamFillet_HanMucTrongLuong model)
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
            if (_context.MaThanhPhamFillet_HanMucTrongLuong.Any(x => x.STT == model.STT && x.Ngay == model.Ngay && x.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaThanhPhamFillet_HanMucTrongLuong
            {

                STT = model.STT,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaLo = model.MaLo,
                MaThanhPham = model.MaThanhPham,
                TrongLuong = model.TrongLuong,
                MaXuong = model.MaXuong
            };
            _context.MaThanhPhamFillet_HanMucTrongLuong.Add(newItem);
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
        [HttpPost("{stt}/{ngay}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update(string stt, string ngay, string maXuong, [FromBody] MaThanhPhamFillet_HanMucTrongLuong model)
        {
            //// Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            //if (string.IsNullOrEmpty(stt))
            //{
            //    return BadRequest(new ApiResponse
            //    {
            //        Success = false,
            //        Message = "Mã này không hợp lệ."
            //    });
            //}
            int number = int.Parse(stt);
            DateTime dateTime = DateTime.Parse(ngay);
            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var items = await _context.MaThanhPhamFillet_HanMucTrongLuong.Where(x => x.STT == number && x.Ngay == dateTime && x.MaXuong == maXuong).ToListAsync();
            var item = items.FirstOrDefault();
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại."
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
            item.STT = model.STT;
            item.Ngay = model.Ngay;
            item.Gio = model.Gio;
            item.MaLo = model.MaLo;
            item.MaThanhPham = model.MaThanhPham;
            item.TrongLuong = model.TrongLuong;
            item.MaXuong = model.MaXuong;
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
        [HttpPost("{stt}/{ngay}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(string stt, string ngay, string maXuong)
        {
            if (string.IsNullOrEmpty(stt))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }

            int number = int.Parse(stt);
            DateTime dateTime = DateTime.Parse(ngay);
            var item = await _context.MaThanhPhamFillet_HanMucTrongLuong.Where(x => x.STT == number && x.Ngay == dateTime && x.MaXuong == maXuong).FirstAsync();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            _context.MaThanhPhamFillet_HanMucTrongLuong.Remove(item);
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
