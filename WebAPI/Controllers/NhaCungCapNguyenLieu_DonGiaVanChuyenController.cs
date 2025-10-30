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
    public class NhaCungCapNguyenLieu_DonGiaVanChuyenController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhaCungCapNguyenLieu_DonGiaVanChuyenController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmNhaCungCapNguyenLieuDonGiaVanChuyen.GetsFullField<object>();
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var donGiaVanChuyens = _context.NhaCungCapNguyenLieu_DonGiaVanChuyen.OrderByDescending(x => x.MaNhaCC).ToList();

            return Ok(donGiaVanChuyens);
        }
        [HttpGet("{maNhaCC}/{ngayApDung}")]
        [Authorize]
        public IActionResult GetByMaNhaCCAndNgayApDung(string maNhaCC, string ngayApDung)
        {
            DateTime date = DateTime.ParseExact(ngayApDung, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var donGiaVanChuyen = _context.NhaCungCapNguyenLieu_DonGiaVanChuyen.FirstOrDefault(x => x.MaNhaCC == maNhaCC && x.NgayApDung == date);
            if (donGiaVanChuyen == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp đơn giá không tồn tại!."
                });
            }
            return Ok(donGiaVanChuyen);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(NhaCungCapNguyenLieu_DonGiaVanChuyen model)
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
            if (_context.NhaCungCapNguyenLieu_DonGiaVanChuyen.Any(u => u.MaNhaCC == model.MaNhaCC && u.NgayApDung == model.NgayApDung))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp theo ngày áp dụng đã tồn tại.",
                });
            }
            var newDonGiaVanChuyen = new NhaCungCapNguyenLieu_DonGiaVanChuyen
            {
                
                MaNhaCC = model.MaNhaCC,
                NgayApDung = model.NgayApDung,
                DonGia = model.DonGia,
                GhiChu =model.GhiChu
            };
            _context.NhaCungCapNguyenLieu_DonGiaVanChuyen.Add(newDonGiaVanChuyen);
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
                Message = "Thêm nhà cung cấp đơn giá thành công!"
            });
        }
        [HttpPost("{maNhaCC}/{ngayApDung}")]
        [Authorize]
        public async Task<IActionResult> Update(string maNhaCC,string ngayApDung, [FromBody] NhaCungCapNguyenLieu_DonGiaVanChuyen model)
         {
            if (string.IsNullOrEmpty(maNhaCC) && ngayApDung == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp đơn giá không hợp lệ."
                });
            }
            DateTime date = DateTime.ParseExact(ngayApDung, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var donGiaVanChuyen = await _context.NhaCungCapNguyenLieu_DonGiaVanChuyen.FirstOrDefaultAsync(x => x.MaNhaCC == maNhaCC);
            if (donGiaVanChuyen == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã nhà cung cấp đơn giá không tồn tại."
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
            donGiaVanChuyen.MaNhaCC = model.MaNhaCC;
            donGiaVanChuyen.NgayApDung = model.NgayApDung;
            donGiaVanChuyen.DonGia = model.DonGia;
            donGiaVanChuyen.GhiChu = model.GhiChu;
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
                Message = "Cập nhật thông tin nhà cung cấp đơn giá thành công!"
            });
        }
        [HttpPost("{maNhaCC}/{ngayApDung}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maNhaCC,string ngayApDung)
        {
            if (string.IsNullOrEmpty(maNhaCC) && string.IsNullOrEmpty(ngayApDung))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn nhà cung cấp đơn giá!."
                });
            }
            DateTime date = DateTime.ParseExact(ngayApDung, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var donGiaVanChuyen = await _context.NhaCungCapNguyenLieu_DonGiaVanChuyen.FirstOrDefaultAsync(x => x.MaNhaCC == maNhaCC && x.NgayApDung == date);
            if (donGiaVanChuyen == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhà cung cấp đơn giá không tồn tại."
                });
            }

            _context.NhaCungCapNguyenLieu_DonGiaVanChuyen.Remove(donGiaVanChuyen);

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
                Message = "Xóa nhà cung cấp đơn giá thành công!"
            });
        }
    }
}
