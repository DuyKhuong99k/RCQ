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
    public class NhaCungCapNguyenLieuxController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhaCungCapNguyenLieuxController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var nhaCungCaps = _context.NhaCungCapNguyenLieu.OrderByDescending(x => x.Ma).ToList();

            return Ok(nhaCungCaps);
        }
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(string ma)
        {
            var nhaCungCap = _context.NhaCungCapNguyenLieu.FirstOrDefault(x => x.Ma == ma);
            if (nhaCungCap == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp không tồn tại!."
                });
            }
            return Ok(nhaCungCap);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(NhaCungCapNguyenLieu model)
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
            if (_context.NhaCungCapNguyenLieu.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp đã tồn tại.",
                });
            }
            var newNhaCungCap = new NhaCungCapNguyenLieu
            {
                
                Ten = model.Ten,
                Ma = model.Ma,
                SuDung = model.SuDung,
            };
            _context.NhaCungCapNguyenLieu.Add(newNhaCungCap);
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
                Message = "Thêm nhà cung cấp thành công!"
            });
        }
        [HttpPost("{ma}")]
        [Authorize]
        public async Task<IActionResult> Update(string ma, [FromBody] NhaCungCapNguyenLieu model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var nhaCungCap = await _context.NhaCungCapNguyenLieu.FindAsync(ma);
            if (nhaCungCap == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp không tồn tại."
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
            nhaCungCap.Ma = model.Ma;
            nhaCungCap.Ten = model.Ten;
            nhaCungCap.SuDung = model.SuDung;
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
                Message = "Cập nhật thông tin nhà cung cấp thành công!"
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
                    Message = "Vui lòng chọn nhà cung cấp!."
                });
            }
            var nhaCungCap = await _context.NhaCungCapNguyenLieu.FindAsync(ma);
            if (nhaCungCap == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhà cung cấp không tồn tại."
                });
            }

            _context.NhaCungCapNguyenLieu.Remove(nhaCungCap);

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
    }
}
