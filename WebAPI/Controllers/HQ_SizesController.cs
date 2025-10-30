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
    public class HQ_SizesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_SizesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.HqSizes.OrderByDescending(x => x.Id).ToList();

            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetsByMa(string id)
        {
            var item = _context.HqSizes.FirstOrDefault(x => x.Id == id);
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
            var item = _context.HqSizes.Where(x => x.Ten.Trim() == tenSize)
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
        public async Task<IActionResult> Insert(HQ_Size model)
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
            if (_context.HqSizes.Any(u => u.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new HQ_Size
            {
                Id = model.Id,
                Ten = model.Ten,
                SuDung = model.SuDung,
                MNgay = DateTime.Now//model.MNgay,
            };
            _context.HqSizes.Add(newItem);
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
        public async Task<IActionResult> Update(string id, [FromBody] HQ_Size model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await _context.HqSizes.FindAsync(id);
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
            item.MNgay = MNgay;
            try
            {
                // Lưu thay đổi cập nhật vào bảng HqLoaiNguyenLieus
                await _context.SaveChangesAsync();

                // Tạo bản ghi mới cho bảng HqLoaiNguyenLieuUs
                var newItemUs = new HQ_Size_U
                {
                    SizeId = item.Id,
                    Ten = item.Ten,
                    SuDung = item.SuDung,
                    MNgay = MNgay,
                    Ngay = DateTime.Now // Ngày hiện tại khi tạo mới
                };

                // Thêm vào bảng HqLoaiNguyenLieuUs
                _context.HqSizeUs.Add(newItemUs);


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
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await _context.HqSizes.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhà cung cấp không tồn tại."
                });
            }

            _context.HqSizes.Remove(item);

            try
            {
                var MNgay = DateTime.Now;
                // Tạo bản ghi mới cho bảng HqLoaiNguyenLieuUs để lưu lịch sử
                var newItemUs = new HQ_Size_D
                {
                    SizeId = item.Id,
                    Ten = item.Ten,
                    SuDung = item.SuDung,
                    MNgay = MNgay,
                    Ngay = DateTime.Now // Ngày hiện tại khi tạo bản ghi
                };

                // Thêm bản ghi vào bảng HqLoaiNguyenLieuUs
                _context.HqSizeDs.Add(newItemUs);
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
