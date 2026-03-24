using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
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
    public class HQ_QuyCachNguyenLieuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_QuyCachNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.HQ_QuyCachNguyenLieus.OrderByDescending(x => x.MaNguyenLieu).ThenBy(x => x.Ten).ToList();

            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetsByMa(int id)
        {
            var item = _context.HQ_QuyCachNguyenLieus.FirstOrDefault(x => x.Id == id);
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
        [HttpGet("{tenSize}")]
        [Authorize]
        public IActionResult GetsByTen(string tenSize)
        {
            var item = _context.HQ_QuyCachNguyenLieus.Where(x => x.Ten.Trim() == tenSize)
                .Select(x => x.Id.ToString())
                .FirstOrDefault();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Tên Size này không tồn tại!."
                });
            }
            return Ok(new { Ma = item });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(HQ_QuyCachNguyenLieu model)
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
            if (_context.HQ_QuyCachNguyenLieus.Any(u => u.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new HQ_QuyCachNguyenLieu
            {
                Id = model.Id,
                Ten = model.Ten,
                SuDung = model.SuDung,
                MNgay = model.MNgay,
                Index = model.Index,
                NhomQuyCach = model.NhomQuyCach,
                MaNguyenLieu = model.MaNguyenLieu,
                DonViTinh = model.DonViTinh
            };
            _context.HQ_QuyCachNguyenLieus.Add(newItem);
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
        public async Task<IActionResult> Update(long id, [FromBody] HQ_QuyCachNguyenLieu model)
        {

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await _context.HQ_QuyCachNguyenLieus.FindAsync(id);
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

            var MNgay = DateTime.Now;
            item.Id = model.Id;
            item.Ten = model.Ten;
            item.SuDung = model.SuDung;
            item.MNgay = model.MNgay;
            item.Index = model.Index;
            item.NhomQuyCach = model.NhomQuyCach;
            item.MaNguyenLieu = model.MaNguyenLieu;
            item.DonViTinh = model.DonViTinh;
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {

            var item = await _context.HQ_QuyCachNguyenLieus.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhà cung cấp không tồn tại."
                });
            }

            _context.HQ_QuyCachNguyenLieus.Remove(item);

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
