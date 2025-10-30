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
    public class PhieuCanCaGiongVungNuoiDaiThanhSidesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanCaGiongVungNuoiDaiThanhSidesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTiets(string fromDate, string toDate)
        {
            if (_context.PhieuCanCaGiongVungNuoiDaiThanhSide == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanCaGiong.GetChiTiets<object>(from, to);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHops(string fromDate, string toDate)
        {
            if (_context.PhieuCanCaGiongVungNuoiDaiThanhSide == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanCaGiong.GetTongHops<object>(from, to);
            return items;
        }
        #region xử lý phiếu cân
        [HttpGet("{dateTime}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime)
        {
            if (_context.PhieuCanCaGiongVungNuoiDaiThanhSide == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanCaGiong.GetPhieuCan<object>(date1);
            return items;
        }
        [HttpGet("{dateTime}")]
        [Authorize]
        public IActionResult GetAllsWithDate(string dateTime)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanCaGiongVungNuoiDaiThanhSide.Where(x => x.Ngay == date1).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanCaGiongVungNuoiDaiThanhSide>(dataT.Item1);
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
            if (_context.PhieuCanCaGiongVungNuoiDaiThanhSide.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaMayCan == model.MaMayCan && u.BiosId == model.BiosId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanCaGiongVungNuoiDaiThanhSide
            {
                STT = model.STT,
                Ngay = model.Ngay,
                MaMayCan = model.MaMayCan,
                BiosId = model.BiosId,
                NgayTai = model.NgayTai,
                GioTai = model.GioTai,
                Gio = model.Gio,
                MaUserCan = model.MaUserCan,
                GhiChu = model.GhiChu,
                MaLoaiCaDaiThanhId = model.MaLoaiCaDaiThanhId,
                TenLoaiCa = model.TenLoaiCa,
                MaGhe = model.MaGhe,
                TenAo = model.TenAo,
                TenChuAo = model.TenChuAo,
                TenCongDoan = model.TenCongDoan,
                TenThongKeDauAo = model.TenThongKeDauAo,
                TrongLuongTare = model.TrongLuongTare,
                TrongLuong = model.TrongLuong,

            };
            _context.PhieuCanCaGiongVungNuoiDaiThanhSide.Add(newItem);
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
        public string DecryBiosId(string sanitizedBiosId)
        {
            var BiosIdEncry = "";

            //if (sanitizedBiosId.Contains("_"))
            //{
            //    // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
            //    BiosIdEncry = sanitizedBiosId.Replace("_", "/");
            //}
            //else
            //{
            //    // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
            //    BiosIdEncry = sanitizedBiosId;
            //}
            //BiosIdEncry = sanitizedBiosId;
            //var biosIdDecry = BiosIdEncry.Replace(Environment.NewLine, "");  //Security.Crypt.ED.DecryptString(BiosIdEncry);
            //return biosIdDecry;
            return sanitizedBiosId.HexStringToString();
        }
        [HttpGet("{stt}/{ngay}/{maMayCan}/{sanitizedBiosId}")]
        //[Authorize]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maMayCan, string sanitizedBiosId)
        {

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanCaGiongVungNuoiDaiThanhSide.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId == DecryBiosId(sanitizedBiosId));
            //var item = _context.PhieuCanCaGiongVungNuoiDaiThanhSide.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId ==sanitizedBiosId);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = $"Mã này khôngtại!"
                });
            }
            return Ok(item);
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{sanitizedBiosId}")]
        [Authorize]
        public async Task<IActionResult> Update(int stt, string ngay, string maMayCan, string sanitizedBiosId, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanCaGiongVungNuoiDaiThanhSide>(dataT.Item1);
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
            var item = await _context.PhieuCanCaGiongVungNuoiDaiThanhSide.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId == DecryBiosId(sanitizedBiosId));

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
                item.TrongLuong,

                item.TenAo,
                item.TenChuAo,
                item.TenCongDoan,
                item.TenThongKeDauAo,
                item.MaLoaiCaDaiThanhId,
                item.TenLoaiCa,

            }, options);

            item.NgayTai = model.NgayTai;
            item.GioTai = model.GioTai;
            item.TrongLuong = model.TrongLuong;
            item.TenChuAo = model.TenChuAo;
            item.TenAo = model.TenAo;
            item.TenCongDoan = model.TenCongDoan;
            item.TenThongKeDauAo = model.TenThongKeDauAo;
            item.MaLoaiCaDaiThanhId = model.MaLoaiCaDaiThanhId;
            item.TenLoaiCa = model.TenLoaiCa;
            item.MaGhe = model.MaGhe;

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
            var model = JsonSerializer.Deserialize<PhieuCanCaGiongVungNuoiDaiThanhSide>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(sanitizedBiosId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanCaGiongVungNuoiDaiThanhSide.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.BiosId == DecryBiosId(sanitizedBiosId));

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
                item.TrongLuongTare
            }, options);

            var newItem = new PhieuCanCaGiongVungNuoiDaiThanhSide
            {
                STT = model.STT,
                Ngay = item.Ngay,
                MaMayCan = item.MaMayCan,
                BiosId = item.BiosId,
                NgayTai = item.NgayTai,
                GioTai = item.GioTai,
                Gio = item.Gio,

                MaUserCan = item.MaUserCan,
                MaLoaiCaDaiThanhId = item.MaLoaiCaDaiThanhId,
                TenLoaiCa = item.TenLoaiCa,

                MaGhe = item.MaGhe,
                TenChuAo = item.TenChuAo,
                TenAo = item.TenAo,
                TenCongDoan = item.TenCongDoan,
                TenThongKeDauAo = item.TenThongKeDauAo,
                TrongLuongTare = model.TrongLuongTare,
                TrongLuong = model.TrongLuong,

                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanCaGiongVungNuoiDaiThanhSide.Add(newItem);
                    _context.PhieuCanCaGiongVungNuoiDaiThanhSide.Remove(item);
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
