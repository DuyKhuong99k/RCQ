using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    public class PhuongTien_TaiTrongController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhuongTien_TaiTrongController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            try
            {
                var items = Vm.VmPhuongTienTaiTrong.GetsFullField<object>();
                return Ok(items);
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.PhuongTien_TaiTrong.OrderByDescending(x => x.MaPhuongTien).ToList();

            return Ok(items);
        }

        [HttpGet("{maPhuongTien}/{ngayApDung}")]
        [Authorize]
        public IActionResult GetByMaPhuongTienAndNgayApDung(string maPhuongTien, string ngayApDung)
        {
            DateTime date = DateTime.ParseExact(ngayApDung, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var item = _context.PhuongTien_TaiTrong.FirstOrDefault(x => x.MaPhuongTien == maPhuongTien && x.NgayApDung == date);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện không tồn tại!."
                });
            }
            return Ok(item);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(PhuongTien_TaiTrong model)
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
            if (_context.PhuongTien_TaiTrong.Any(u => u.MaPhuongTien == model.MaPhuongTien && u.NgayApDung == model.NgayApDung))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện và ngày áp dụng đã tồn tại.",
                });
            }
            var newItem = new PhuongTien_TaiTrong
            {
                MaPhuongTien = model.MaPhuongTien,
                NgayApDung = model.NgayApDung,
                TaiTrong = model.TaiTrong,
                GhiChu = model.GhiChu
            };
            _context.PhuongTien_TaiTrong.Add(newItem);
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
                Message = "Thêm phương tiện tải trọng thành công!"
            });
        }
        [HttpPost("{maPhuongTien}/{ngayApDung}")]
        [Authorize]
        public async Task<IActionResult> Update(string maPhuongTien,string ngayApDung, [FromBody] PhuongTien_TaiTrong model)
         {
            if (string.IsNullOrEmpty(maPhuongTien) && string.IsNullOrEmpty(ngayApDung))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện tải trọng không hợp lệ."
                });
            }
            DateTime date = DateTime.ParseExact(ngayApDung, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var item = await _context.PhuongTien_TaiTrong.FirstOrDefaultAsync(x => x.MaPhuongTien == maPhuongTien);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện tải trọng không tồn tại."
                });
            }
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
            item.MaPhuongTien = model.MaPhuongTien;
            item.TaiTrong = model.TaiTrong;
            item.NgayApDung = model.NgayApDung;
            item.GhiChu = model.GhiChu;
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
                Message = "Cập nhật thông tin phương tiện tải trọng thành công!"
            });
        }
        [HttpPost("{maPhuongTien}/{ngayApDung}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maPhuongTien,string ngayApDung)
        {
            if (string.IsNullOrEmpty(maPhuongTien) && string.IsNullOrEmpty(ngayApDung))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn phương tiện tải trọng!."
                });
            }
            DateTime date = DateTime.ParseExact(ngayApDung, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var item = await _context.PhuongTien_TaiTrong.FirstOrDefaultAsync(x => x.MaPhuongTien == maPhuongTien && x.NgayApDung == date);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Phương tiện tải trọng không tồn tại."
                });
            }

            _context.PhuongTien_TaiTrong.Remove(item);

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
                    Message = "Đã xảy ra lỗi khi xóa nhà cung cấp.",
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa Phương tiện tải trọng thành công!"
            });
        }
    }
}
