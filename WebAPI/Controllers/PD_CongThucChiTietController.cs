using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class PD_CongThucChiTietController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PD_CongThucChiTietController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{maCongThuc}/{ngay}")]
        [Authorize]
        public IActionResult GetAllsFullField(string maCongThuc, string ngay)
        {
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmPD_CongThuc.GetsFullFieldCongThucChiTiet<object>(maCongThuc, ngayConvert, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.PD_CongThucChiTiet.OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpGet("{maCongThuc}")]
        [Authorize]
        public IActionResult GetAllWithMaCongThucAndNgays(string maCongThuc)
        {
            var items = _context.PD_CongThucChiTiet.Where(x => x.MaCongThuc == maCongThuc).ToList();
            return Ok(items);
        }
        [HttpGet("{stt}/{ngay}/{maCongThuc}")]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maCongThuc)
        {
            //DateTime ngayConvert = DateTime.Parse(ngay);
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PD_CongThucChiTiet.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaCongThuc == maCongThuc);
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
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PD_CongThucChiTiet>(dataT.Item1);
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
            if (_context.PD_CongThucChiTiet.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaCongThuc == model.MaCongThuc))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PD_CongThucChiTiet
            {
                STT = model.STT,
                Ngay = model.Ngay,
                MaCongThuc = model.MaCongThuc,
                MaSanPham = model.MaSanPham,
                SanLuong = model.SanLuong,
                TyLe = model.TyLe,
                BienDo = model.BienDo,
            };
            _context.PD_CongThucChiTiet.Add(newItem);
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
        [HttpPost("{stt}/{ngay}/{maCongThuc}")]
        [Authorize]
        public async Task<IActionResult> Update(int stt, string ngay, string maCongThuc, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PD_CongThucChiTiet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maCongThuc))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PD_CongThucChiTiet.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaCongThuc == maCongThuc);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
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
            item.MaSanPham = model.MaSanPham;
            item.SanLuong = model.SanLuong;
            item.TyLe = model.TyLe;
            item.BienDo = model.BienDo;
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
        [HttpPost("{stt}/{ngay}/{maCongThuc}")]
        [Authorize]
        public async Task<IActionResult> Delete(int stt, string ngay, string maCongThuc)
        {
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maCongThuc))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PD_CongThucChiTiet.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaCongThuc == maCongThuc);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.PD_CongThucChiTiet.Remove(item);

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
