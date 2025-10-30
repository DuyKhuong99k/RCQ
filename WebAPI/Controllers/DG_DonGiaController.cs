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
    public class DG_DonGiaController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public DG_DonGiaController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullFieldByYear()
        {
            var items = Vm.VmDG_DonGia.GetsFullFieldByYear<object>(_context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.DG_DonGia.OrderByDescending(x => x.Id).ToList();

            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetsByMa(string id)
        {
            var item = _context.DG_DonGia.FirstOrDefault(x => x.Id == id);
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
        public async Task<IActionResult> Insert(DG_DonGia model)
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
            if (_context.DG_DonGia.Any(u => u.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new DG_DonGia
            {
                Id = model.Id,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaSanPham = model.MaSanPham,
                MaLoaiDonGia = model.MaLoaiDonGia,
                DanhGia = model.DanhGia,
                DinhMucDown = model.DinhMucDown,
                DinhMucUp = model.DinhMucUp,
                HeSo = model.HeSo,
                DonGia = model.DonGia,
                CreateBy = model.CreateBy,
                CreateDateTime = model.CreateDateTime,
                ModifiedBy = model.ModifiedBy,
                ModifiedDateTime = model.ModifiedDateTime,
                GhiChu = model.GhiChu,
                HeSoRot = model.HeSoRot,
                IsUsedHeSoRot = model.IsUsedHeSoRot,
                Range = model.Range,
                MaSizeDinhHinh = model.MaSizeDinhHinh,
                DonGiaGiaCong = model.DonGiaGiaCong,
                MaXepHang = model.MaXepHang,
                MaThanhPham = model.MaThanhPham,
                LoaiCan = model.LoaiCan,
                MaSizeFillet = model.MaSizeFillet
            };
            _context.DG_DonGia.Add(newItem);
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
        public async Task<IActionResult> Update(string id, [FromBody] DG_DonGia model)
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
            var item = await _context.DG_DonGia.FindAsync(id);
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
            item.Ngay = model.Ngay;
            item.Gio = model.Gio;
            item.MaSanPham = model.MaSanPham;
            item.MaLoaiDonGia = model.MaLoaiDonGia;
            item.DanhGia = model.DanhGia;
            item.DinhMucDown = model.DinhMucDown;
            item.DinhMucUp = model.DinhMucUp;
            item.HeSo = model.HeSo;
            item.DonGia = model.DonGia;
            item.CreateBy = model.CreateBy;
            item.CreateDateTime = model.CreateDateTime;
            item.ModifiedBy = model.ModifiedBy;
            item.ModifiedDateTime = model.ModifiedDateTime;
            item.GhiChu = model.GhiChu;
            item.HeSoRot = model.HeSoRot;
            item.IsUsedHeSoRot = model.IsUsedHeSoRot;
            item.Range = model.Range;
            item.MaSizeDinhHinh = model.MaSizeDinhHinh;
            item.DonGiaGiaCong = model.DonGiaGiaCong;
            item.MaXepHang = model.MaXepHang;
            item.MaThanhPham = model.MaThanhPham;
            item.LoaiCan = model.LoaiCan;
            item.MaSizeFillet = model.MaSizeFillet;
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
            var item = await _context.DG_DonGia.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.DG_DonGia.Remove(item);

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
