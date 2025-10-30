using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class BanCatTietController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BanCatTietController(dbPMScontext context)
        {
            _context = context;
        }

        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAllBanCatTiets()
        {
            var banCatTiets = _context.BanCatTiet.OrderByDescending(x => x.Ma).ToList();

            return Ok(banCatTiets);
        }
        [HttpGet("{maBanCatTiet}")]
        [Authorize]
        public IActionResult GetBanCatTietByMaBanCatTiet(string maBanCatTiet)
        {
            var banCatTiet = _context.BanCatTiet.FirstOrDefault(x => x.Ma == maBanCatTiet);
            if (banCatTiet == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã bàn cắt tiết không tồn tại!."
                });
            }
            return Ok(banCatTiet);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertBanCatTiet(BanCatTiet model)
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
            if (_context.BanCatTiet.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã bàn cắt tiết đã tồn tại.",
                });
            }
            var newBanCatTiet = new BanCatTiet
            {
                SuDung = model.SuDung,
                IsShowX2 = model.IsShowX2,
                Y2 = model.Y2,
                Y1 = model.Y1,
                X2 = model.X2,
                X1 = model.X1,
                ColSpanX2 = model.ColSpanX2,
                ColSpanX1 = model.ColSpanX1,
                IsShowX1 = model.IsShowX1,
                DoUuTienX1 = model.DoUuTienX1,
                DoUuTienX2 = model.DoUuTienX2,
                ThoiGianQuangDuongPheu2X2 = model.ThoiGianQuangDuongPheu2X2,
                ThoiGianQuangDuongPheu2X1 = model.ThoiGianQuangDuongPheu2X1,
                ThoiGianQuangDuongPheu1X2 = model.ThoiGianQuangDuongPheu1X2,
                ThoiGianQuangDuongPheu1X1 = model.ThoiGianQuangDuongPheu1X1,
                SoLanChiaCaX2 = model.SoLanChiaCaX2,
                SoLanChiaCaX1 = model.SoLanChiaCaX1,
                IsKhoaX2 = model.IsKhoaX2,
                IsDetect = model.IsDetect,
                IsFalse = model.IsFalse,
                IsKhoaX1 = model.IsKhoaX1,
                Ma = model.Ma,
                TimeOpen = model.TimeOpen,
                PlcAdr = model.PlcAdr,
                PlcValue = model.PlcValue,
                PlcYValue = model.PlcYValue,
                PlcYadr = model.PlcYadr,
                VongChiaCa = model.VongChiaCa,
                ViTri_HX1 = model.ViTri_HX1,
                ViTri_HX2 = model.ViTri_HX2,
                ThoiGianNhanCaX1 = model.ThoiGianNhanCaX1,
                ThoiGianNhanCaX2 = model.ThoiGianNhanCaX2,
                PlcOffAdr = model.PlcOffAdr,
                PlcOffValue = model.PlcOffValue,
                IsCaMuoiX1 = model.IsCaMuoiX1,
                IsCaMuoiX2 = model.IsCaMuoiX2,
                Ten = model.Ten
            };
            _context.BanCatTiet.Add(newBanCatTiet);
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
                Message = "Thêm bàn cắt tiết thành công!"
            });
        }
        [HttpPost("{maBanCatTiet}")]
        [Authorize]
        public async Task<IActionResult> UpdateBanCatTiet(string maBanCatTiet, [FromBody] BanCatTiet model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maBanCatTiet))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã bàn cắt tiết không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var banCatTiet = await _context.BanCatTiet.FindAsync(maBanCatTiet);
            if (banCatTiet == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã bàn cắt tiết không tồn tại."
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
            banCatTiet.SuDung = model.SuDung;
            banCatTiet.IsShowX2 = model.IsShowX2;
            banCatTiet.Y2 = model.Y2;
            banCatTiet.Y1 = model.Y1;
            banCatTiet.X2 = model.X2;
            banCatTiet.X1 = model.X1;
            banCatTiet.ColSpanX2 = model.ColSpanX2;
            banCatTiet.ColSpanX1 = model.ColSpanX1;
            banCatTiet.IsShowX1 = model.IsShowX1;
            banCatTiet.DoUuTienX1 = model.DoUuTienX1;
            banCatTiet.DoUuTienX2 = model.DoUuTienX2;
            banCatTiet.ThoiGianQuangDuongPheu2X2 = model.ThoiGianQuangDuongPheu2X2;
            banCatTiet.ThoiGianQuangDuongPheu2X1 = model.ThoiGianQuangDuongPheu2X1;
            banCatTiet.ThoiGianQuangDuongPheu1X2 = model.ThoiGianQuangDuongPheu1X2;
            banCatTiet.ThoiGianQuangDuongPheu1X1 = model.ThoiGianQuangDuongPheu1X1;
            banCatTiet.SoLanChiaCaX2 = model.SoLanChiaCaX2;
            banCatTiet.SoLanChiaCaX1 = model.SoLanChiaCaX1;
            banCatTiet.IsKhoaX2 = model.IsKhoaX2;
            banCatTiet.IsDetect = model.IsDetect;
            banCatTiet.IsFalse = model.IsFalse;
            banCatTiet.IsKhoaX1 = model.IsKhoaX1;
            banCatTiet.Ma = model.Ma;
            banCatTiet.TimeOpen = model.TimeOpen;
            banCatTiet.PlcAdr = model.PlcAdr;
            banCatTiet.PlcValue = model.PlcValue;
            banCatTiet.PlcYValue = model.PlcYValue;
            banCatTiet.PlcYadr = model.PlcYadr;
            banCatTiet.VongChiaCa = model.VongChiaCa;
            banCatTiet.ViTri_HX1 = model.ViTri_HX1;
            banCatTiet.ViTri_HX2 = model.ViTri_HX2;
            banCatTiet.ThoiGianNhanCaX1 = model.ThoiGianNhanCaX1;
            banCatTiet.ThoiGianNhanCaX2 = model.ThoiGianNhanCaX2;
            banCatTiet.PlcOffAdr = model.PlcOffAdr;
            banCatTiet.PlcOffValue = model.PlcOffValue;
            banCatTiet.IsCaMuoiX1 = model.IsCaMuoiX1;
            banCatTiet.IsCaMuoiX2 = model.IsCaMuoiX2;
            banCatTiet.Ten = model.Ten;
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
                Message = "Cập nhật thông tin bàn cắt tiết thành công!"
            });
        }
        [HttpPost("{maBanCatTiet}")]
        [Authorize]
        public async Task<IActionResult> DeleteBanCatTiet(string maBanCatTiet)
        {
            if (string.IsNullOrEmpty(maBanCatTiet))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn bàn cắt tiết!."
                });
            }
            var banCatTiet = await _context.BanCatTiet.FindAsync(maBanCatTiet);
            if (banCatTiet == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Bàn cắt tiết không tồn tại."
                });
            }

            _context.BanCatTiet.Remove(banCatTiet);

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
                    Message = "Đã xảy ra lỗi khi xóa bàn cắt tiết.",
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa bàn cắt tiết thành công!"
            });
        }
    }
}

