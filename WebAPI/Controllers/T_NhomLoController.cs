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
    public class T_NhomLoController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_NhomLoController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;


        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.T_NhomLo.OrderByDescending(x => x.Ma).ToList();
            return Ok(items);
        }
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(string ma)
        {
            var item = _context.T_NhomLo.FirstOrDefault(x => x.Ma == ma);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã lô không tồn tại!."
                });
            }
            return Ok(item);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(T_NhomLo model)
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
            if (_context.T_NhomLo.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã lô đã tồn tại.",
                });
            }
            var newItem = new T_NhomLo
            {
                Ten = model.Ten,
                Ma = model.Ma,
                NgayTao = model.NgayTao,
                SuDung = model.SuDung,
                NgayBatDau = model.NgayBatDau,
                NgayKetThuc = model.NgayKetThuc,
                NgayNguyenLieu = model.NgayNguyenLieu,
                DaKetThuc = model.DaKetThuc,
                GhiChu = model.GhiChu
            };
            _context.T_NhomLo.Add(newItem);
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
                Message = "Thêm lô thành công!"
            });
        }
        [HttpPost("{ma}")]
        [Authorize]
        public async Task<IActionResult> Update(string ma, [FromBody] T_NhomLo model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã lô không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await _context.T_NhomLo.FindAsync(ma);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã lô không tồn tại."
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
            item.Ten = model.Ten;
            item.Ma = model.Ma;
            item.NgayTao = model.NgayTao;
            item.SuDung = model.SuDung;
            item.NgayBatDau = model.NgayBatDau;
            item.NgayKetThuc = model.NgayKetThuc;
            item.NgayNguyenLieu = model.NgayNguyenLieu;
            item.DaKetThuc = model.DaKetThuc;
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
                Message = "Cập nhật thông tin lô thành công!"
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
                    Message = "Vui lòng chọn lô!."
                });
            }
            var item = await _context.T_NhomLo.FindAsync(ma);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Lô không tồn tại."
                });
            }

            _context.T_NhomLo.Remove(item);

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
                    Message = "Đã xảy ra lỗi khi xóa lô.",
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa lô thành công!"
            });
        }
    }
}
