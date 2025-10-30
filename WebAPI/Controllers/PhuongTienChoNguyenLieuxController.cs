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
    public class PhuongTienChoNguyenLieuxController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhuongTienChoNguyenLieuxController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PhuongTienChoNguyenLieux
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(PhuongTienChoNguyenLieu model)
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
            if (_context.PhuongTienChoNguyenLieu.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện đã tồn tại.",
                });
            }
            var newPhuongTienChoNguyenLieu = new PhuongTienChoNguyenLieu
            {
                Ten = model.Ten,
                Ma = model.Ma,
                SuDung = model.SuDung,
                IsHD = model.IsHD,
                IsGhe = model.IsGhe,
                VungNuoiId = model.VungNuoiId,
                SoGhe = model.SoGhe
            };
            _context.PhuongTienChoNguyenLieu.Add(newPhuongTienChoNguyenLieu);
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
                Message = "Thêm phương tiện chở nguyên liệu thành công!"
            });
        }
        [HttpPost("{ma}")]
        [Authorize]
        public async Task<IActionResult> Update(string ma, [FromBody] PhuongTienChoNguyenLieu model)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện nguyên liệu không hợp lệ."
                });
            }
            var phuongTienNguyenLieu = await _context.PhuongTienChoNguyenLieu.FindAsync(ma);
            if (phuongTienNguyenLieu == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện nguyên liệu không tồn tại."
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
            phuongTienNguyenLieu.Ma = model.Ma;
            phuongTienNguyenLieu.Ten = model.Ten;
            phuongTienNguyenLieu.SuDung = model.SuDung;
            phuongTienNguyenLieu.IsHD = model.IsHD;
            phuongTienNguyenLieu.IsGhe = model.IsGhe;
            phuongTienNguyenLieu.VungNuoiId = model.VungNuoiId;
            phuongTienNguyenLieu.SoGhe = model.SoGhe;
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
                Message = "Cập nhật thông tin phương tiện nguyên thành công!"
            });
        }
        [HttpPost("{ma}")]
        [Authorize]
        public async Task<IActionResult> Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn phương tiện nguyên liệu!."
                });
            }
            var phuongTienNguyenLieu = await _context.PhuongTienChoNguyenLieu.FindAsync(ma);
            if (phuongTienNguyenLieu == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Phương tiện nguyên liệu không tồn tại."
                });
            }

            _context.PhuongTienChoNguyenLieu.Remove(phuongTienNguyenLieu);

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
                Message = "Xóa nhà cung cấp thành công!"
            });
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var phuongTienNguyenLieus = _context.PhuongTienChoNguyenLieu.OrderByDescending(x => x.Ma).ToList();
            var connectionstring = _context.Database.GetConnectionString();
            return Ok(phuongTienNguyenLieus);
        }
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetByMa(string ma)
        {
            var phuongTienNguyenLieu = _context.PhuongTienChoNguyenLieu.FirstOrDefault(x => x.Ma == ma);
            if (phuongTienNguyenLieu == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phương tiện không tồn tại!."
                });
            }
            return Ok(phuongTienNguyenLieu);
        }
    }
}
