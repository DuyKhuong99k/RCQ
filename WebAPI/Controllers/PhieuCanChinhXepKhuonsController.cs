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
    public class PhieuCanChinhXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanChinhXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;


        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopChinhXepKhuons(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHopChinhXepKhuons<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopChinhXepKhuonsDB_St(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamChinhXepKhuon.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopChinhXepKhuonsDB(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHopChinhXepKhuons<object>(date1, date2, xuongId);
            return items;
        }
        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCan_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanChinhXepKhuon.Where(x => x.NgayNguyenLieu == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanChinhXepKhuon>(dataT.Item1);
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
            if (_context.PhieuCanChinhXepKhuon.Any(u => u.STT == model.STT && u.NgayNguyenLieu == model.NgayNguyenLieu && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanChinhXepKhuon
            {
                STT = model.STT,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaCoiTam = model.MaCoiTam,
                MaCoiChinh = model.MaCoiChinh,
                DaQuay = model.DaQuay,
                ThoiGianBatDauQuay = model.ThoiGianBatDauQuay,
                ThoiGianRaCoi = model.ThoiGianRaCoi,
                Forced = model.Forced,
                ThoiGianQuay = model.ThoiGianQuay,
                TrongLuong = model.TrongLuong,
                MaLo = model.MaLo,
                MaLoaiCa = model.MaLoaiCa,
                MaSizeChinh = model.MaSizeChinh,
                MaMau = model.MaMau,
                MaChatLuong = model.MaChatLuong,
                MaThanhPhamChinh = model.MaThanhPhamChinh,
                MaKhuVuc = model.MaKhuVuc,
                MaNhanVien = model.MaNhanVien,
                MaNhom = model.MaNhom,
                MaChieuXa = model.MaChieuXa,
                TaiChe = model.TaiChe,
                MaUserCan = model.MaUserCan,
                MaXuong = model.MaXuong,
                MaMayCan = model.MaMayCan,
                GhiChu = model.GhiChu,
                LuotQuay = model.LuotQuay,
                ChuyenXuong = model.ChuyenXuong,
                MaNhanVienPvPhanCo = model.MaNhanVienPvPhanCo,
                TrongLuongTare = model.TrongLuongTare,
                NgayNguyenLieu = model.NgayNguyenLieu,
                Id = model.Id,
            };
            _context.PhieuCanChinhXepKhuon.Add(newItem);
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
        [HttpGet("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maMayCan, string maXuong)
        {
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanChinhXepKhuon.FirstOrDefault(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanChinhXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanChinhXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaLo,
                item.MaThanhPhamChinh,
                item.MaChieuXa,
                item.MaSizeChinh,
                item.MaMau,
                item.MaChatLuong,
                item.MaKhuVuc,
                item.MaCoiTam,
                item.MaCoiChinh,
                item.TaiChe,
                item.ChuyenXuong,
                item.ThoiGianQuay,
                item.MaNhanVien,
                item.ThoiGianBatDauQuay,
                item.MaNhanVienPvPhanCo,
                item.ThoiGianRaCoi,
            }, options);

            item.MaLo = model.MaLo;
            item.MaThanhPhamChinh = model.MaThanhPhamChinh;
            item.MaChieuXa = model.MaChieuXa;
            item.MaSizeChinh = model.MaSizeChinh;
            item.MaMau = model.MaMau;
            item.MaChatLuong = model.MaChatLuong;
            item.MaKhuVuc = model.MaKhuVuc;
            item.MaCoiTam = model.MaCoiTam;
            item.MaCoiChinh = model.MaCoiChinh;
            item.TaiChe = model.TaiChe;
            item.ChuyenXuong = model.ChuyenXuong;
            item.ThoiGianQuay = model.ThoiGianQuay;
            item.MaNhanVien = model.MaNhanVien;
            item.ThoiGianBatDauQuay = model.ThoiGianBatDauQuay;
            item.MaNhanVienPvPhanCo = model.MaNhanVienPvPhanCo;
            item.ThoiGianRaCoi = model.ThoiGianRaCoi;
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanChinhXepKhuon>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanChinhXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
            }, options);

            var newItem = new PhieuCanChinhXepKhuon
            {
                STT = model.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaCoiTam = item.MaCoiTam,
                MaCoiChinh = item.MaCoiChinh,
                DaQuay = item.DaQuay,
                ThoiGianBatDauQuay = item.ThoiGianBatDauQuay,
                ThoiGianRaCoi = item.ThoiGianRaCoi,
                Forced = item.Forced,
                ThoiGianQuay = item.ThoiGianQuay,
                TrongLuong = model.TrongLuong,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaSizeChinh = item.MaSizeChinh,
                MaMau = item.MaMau,
                MaChatLuong = item.MaChatLuong,
                MaThanhPhamChinh = item.MaThanhPhamChinh,
                MaKhuVuc = item.MaKhuVuc,
                MaNhanVien = item.MaNhanVien,
                MaNhom = item.MaNhom,
                MaChieuXa = item.MaChieuXa,
                TaiChe = item.TaiChe,
                MaUserCan = item.MaUserCan,
                MaXuong = item.MaXuong,
                MaMayCan = item.MaMayCan,
                LuotQuay = item.LuotQuay,
                ChuyenXuong = item.ChuyenXuong,
                MaNhanVienPvPhanCo = item.MaNhanVienPvPhanCo,
                TrongLuongTare = item.TrongLuongTare,
                NgayNguyenLieu = item.NgayNguyenLieu,
                NgayRaCoi = item.NgayRaCoi,
                NgayBatDauQuay = item.NgayBatDauQuay,
                IdMonitor = item.IdMonitor,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanChinhXepKhuon.Remove(item);
                    _context.PhieuCanChinhXepKhuon.Add(newItem);
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanChinhXepKhuon>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanChinhXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaXuong,
                item.ChuyenXuong
            }, options);

            var newItem = new PhieuCanChinhXepKhuon
            {
                STT = item.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaCoiTam = item.MaCoiTam,
                MaCoiChinh = item.MaCoiChinh,
                DaQuay = item.DaQuay,
                ThoiGianBatDauQuay = item.ThoiGianBatDauQuay,
                ThoiGianRaCoi = item.ThoiGianRaCoi,
                Forced = item.Forced,
                ThoiGianQuay = item.ThoiGianQuay,
                TrongLuong = item.TrongLuong,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaSizeChinh = item.MaSizeChinh,
                MaMau = item.MaMau,
                MaChatLuong = item.MaChatLuong,
                MaThanhPhamChinh = item.MaThanhPhamChinh,
                MaKhuVuc = item.MaKhuVuc,
                MaNhanVien = item.MaNhanVien,
                MaNhom = item.MaNhom,
                MaChieuXa = item.MaChieuXa,
                TaiChe = item.TaiChe,
                MaUserCan = item.MaUserCan,
                MaXuong = model.MaXuong,
                MaMayCan = item.MaMayCan,
                LuotQuay = item.LuotQuay,
                ChuyenXuong = model.ChuyenXuong,
                MaNhanVienPvPhanCo = item.MaNhanVienPvPhanCo,
                TrongLuongTare = item.TrongLuongTare,
                NgayNguyenLieu = item.NgayNguyenLieu,
                NgayRaCoi = item.NgayRaCoi,
                NgayBatDauQuay = item.NgayBatDauQuay,
                IdMonitor = item.IdMonitor,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanChinhXepKhuon.Remove(item);
                    _context.PhieuCanChinhXepKhuon.Add(newItem);
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

        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanChinhXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanChinhXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaSizeChinh
            }, options);
            item.MaSizeChinh = model.MaSizeChinh;
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
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanChinhXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanChinhXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaThanhPhamChinh
            }, options);
            item.MaThanhPhamChinh = model.MaThanhPhamChinh;
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
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenNgayNguyenLieu(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanChinhXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanChinhXepKhuon.FirstOrDefaultAsync(x => x.STT == stt && x.NgayNguyenLieu == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.NgayNguyenLieu
            }, options);
            item.NgayNguyenLieu = model.NgayNguyenLieu;
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
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        #endregion
        #region Báo Cáo
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanChinhXepKhuonsByFromDateToDate(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanChinhXepKhuonsByFromDateToDate<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopNhanVienFromdateToDates(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHopNhanVienFromdateToDates(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopNhanViens2FromDateToDate(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHopNhanViens2FromDateToDate(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHops2FromDateToDate(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHops2FromDateToDate(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopCoisFromDateToDate(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetChiTietCoiChinh<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopCoisChuaQuayFromDateToDate(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHopCoisChuaQuayFromDateToDate(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopCois2FromDateToDate(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHopCois2FromDateToDate(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopChiTietCoisFromDateToDate(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanTongHopChiTietCoisFromDateToDate(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopCoisXuongFromDateToDate(string fromDate, string toDate)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopCoisXuongFromDateToDate<object>(date1, date2);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopCoiGanRaFromDateToDate(string fromDate, string toDate)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopCoiGanRaFromDateToDate<object>(date1, date2);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThoiGianLuotRaCoiFromDate(string fromDate, string toDate)
        {
            if (_context.PhieuCanChinhXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopThoiGianLuotRaCoiFromDate<object>(date1, date2);
            return items;
        }
        #endregion
    }
}
