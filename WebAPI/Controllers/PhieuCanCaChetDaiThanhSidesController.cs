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
using ToolsEx;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanCaChetDaiThanhSidesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanCaChetDaiThanhSidesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{fromDate}/{toDate}/{timeString}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTiets(string fromDate, string toDate, string timeString)
        {
            if (_context.PhieuCanCaChetDaiThanhSide == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanCaChetDaiThanhSide.GetChiTiets<object>(from, to, timeString);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHops(string fromDate, string toDate)
        {
            if (_context.PhieuCanCaChetDaiThanhSide == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanCaChetDaiThanhSide.GetTongHops<object>(from, to);
            return items;
        }

        #region xử lý phiếu cân
        [HttpGet("{dateTime}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime)
        {
            if (_context.PhieuCanCaChetDaiThanhSide == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanCaChetDaiThanhSide.Where(x => x.Ngay == date1).OrderByDescending(x => x.Ngay).ToList();
            return items;
        }
        [HttpGet("{dateTime}")]
        [Authorize]
        public IActionResult GetAllsWithDate(string dateTime)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanCaChetDaiThanhSide.Where(x => x.Ngay == date1).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanCaChetDaiThanhSide>(dataT.Item1);
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
            if (_context.PhieuCanCaChetDaiThanhSide.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaMayCan == model.MaMayCan && u.BiosId == model.BiosId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanCaChetDaiThanhSide
            {
                STT = model.STT,
                Ngay = model.Ngay,
                MaMayCan = model.MaMayCan,
                Gio = model.Gio,
                MaUserCan = model.MaUserCan,
                GhiChu = model.GhiChu,
                TenLoaiCa = model.TenLoaiCa,
                TenKhachHang = model.TenKhachHang,
                TenThongKe = model.TenThongKe,
                TenAo = model.TenAo,
                TrongLuong = model.TrongLuong,
                TrongLuongBinhQuan = model.TrongLuongBinhQuan,
                TrongLuongTare = model.TrongLuongTare,
                BiosId = model.BiosId,
                GioTai = model.GioTai,
                NgayTai = model.NgayTai,

            };
            _context.PhieuCanCaChetDaiThanhSide.Add(newItem);
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

        [HttpGet("{stt}/{ngay}/{maMayCan}/{sanitizedBiosId}")]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maMayCan, string sanitizedBiosId)
        {

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            //var item = _context.PhieuCanCaChetDaiThanhSide.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId == AppViewModels.AppViewModel.Instance.DecryBiosId(sanitizedBiosId));
            var item = _context.PhieuCanCaChetDaiThanhSide.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId ==sanitizedBiosId.HexStringToString());
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{sanitizedBiosId}")]
        [Authorize]
        public async Task<IActionResult> Update(int stt, string ngay, string maMayCan, string sanitizedBiosId, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanCaChetDaiThanhSide>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(sanitizedBiosId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanCaChetDaiThanhSide.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId == sanitizedBiosId.HexStringToString());

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
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.NgayTai,
                item.GioTai,
                item.TenLoaiCa,
                item.TenKhachHang,
                item.TenThongKe,
                item.TenAo
            }, options);

            item.NgayTai = model.NgayTai;
            item.GioTai = model.GioTai;
            item.TenLoaiCa = model.TenLoaiCa;
            item.TenKhachHang = model.TenKhachHang;
            item.TenThongKe = model.TenThongKe;
            item.TenAo = model.TenAo;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
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
                    Errors = new List<string> { ex.Message
}
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{sanitizedBiosId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int stt, string ngay, string maMayCan, string sanitizedBiosId, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanCaChetDaiThanhSide>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(sanitizedBiosId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanCaChetDaiThanhSide.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId == sanitizedBiosId.HexStringToString());

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
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

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var oldData = JsonSerializer.Serialize(new
            {
                item.STT,
                item.TrongLuong,
                item.TrongLuongBinhQuan,
                item.TrongLuongTare
            }, options);

            var newItem = new PhieuCanCaChetDaiThanhSide
            {
                STT = model.STT,
                Ngay = item.Ngay,
                MaMayCan = item.MaMayCan,
                Gio = item.Gio,
                MaUserCan = item.MaUserCan,
                TenLoaiCa = item.TenLoaiCa,
                TenKhachHang = item.TenKhachHang,
                TenThongKe = item.TenThongKe,
                TenAo = item.TenAo,
                TrongLuong = model.TrongLuong,
                TrongLuongBinhQuan = model.TrongLuongBinhQuan,
                TrongLuongTare = model.TrongLuongTare,
                BiosId = item.BiosId,
                GioTai = item.GioTai,
                NgayTai = item.NgayTai,

                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanCaChetDaiThanhSide.Add(newItem);
                    _context.PhieuCanCaChetDaiThanhSide.Remove(item);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        #endregion
    }
}
