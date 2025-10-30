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
    public class PhieuCanPhuPhamsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanPhuPhamsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanPhuPham.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamsDB_St(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamPhuXepKhuon.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamsDB(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanPhuPham.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }

        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanChiTiets(string fromDate, string toDate)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanPhuPham.GetPhieuCanChiTiets<object>(from, to);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopNhaMuaHangs(string fromDate, string toDate)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanPhuPham.GetPhieuCanTongHopNhaMuaHangs<object>(from, to);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopThanhPhams(string fromDate, string toDate)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanPhuPham.GetPhieuCanTongHopThanhPhams<object>(from, to);
            return items;
        }
        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanPhuPham.GetPhieuCan_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanPhuPham.Where(x => x.Ngay == date1 && x.MaXuongSanXuat == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPham>(dataT.Item1);
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
            if (_context.PhieuCanPhuPham.Any(u => u.MaMayTinhCan == model.MaMayTinhCan && u.MaUserCan == model.MaUserCan && u.ThoiGianCan == model.ThoiGianCan && u.ThoiGianCan == model.NgayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanPhuPham
            {
                MaMayTinhCan = model.MaMayTinhCan,
                MaUserCan = model.MaUserCan,
                ThoiGianCan = model.ThoiGianCan,
                Ngay = model.Ngay,
                NhaMuaHang = model.NhaMuaHang,
                MSL = model.MSL,
                MaPhuongTien = model.MaPhuongTien,
                MaLoaiCa = model.MaLoaiCa,
                MaLoaiThanhPham = model.MaLoaiThanhPham,
                MaSize = model.MaSize,
                MaMau = model.MaMau,
                TrongLuong = model.TrongLuong,
                SuDung = model.SuDung,
                GhiChu = model.GhiChu,
                MaXuongSanXuat = model.MaXuongSanXuat,
                NgayCan = model.NgayCan,
                TrongLuongTare = model.TrongLuongTare,
            };
            _context.PhieuCanPhuPham.Add(newItem);
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
        [HttpGet("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngayCan}")]
        [Authorize]
        public IActionResult GetsByMa(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngayCan)
        {
            DateTime thoiGianCanConvert = DateTime.ParseExact(thoiGianCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime ngayCanConvert = DateTime.ParseExact(ngayCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanPhuPham.FirstOrDefault(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCanConvert && x.NgayCan == ngayCanConvert);
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
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngayCan}")]
        [Authorize]
        public async Task<IActionResult> Update(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngayCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPham>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan) || string.IsNullOrEmpty(ngayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime thoiGianCanConvert = DateTime.ParseExact(thoiGianCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime ngayCanConvert = DateTime.ParseExact(ngayCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanPhuPham.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCanConvert && x.NgayCan == ngayCanConvert);

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
                item.MaLoaiCa,
                item.NhaMuaHang,
                item.MaSize,
                item.MaPhuongTien,
                item.MaMau,
                item.SuDung,
            }, options);

            item.MaLoaiCa = model.MaLoaiCa;
            item.NhaMuaHang = model.NhaMuaHang;
            item.MaSize = model.MaSize;
            item.MaPhuongTien = model.MaPhuongTien;
            item.MaMau = model.MaMau;
            item.SuDung = model.SuDung;
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
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngayCan}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngayCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPham>(dataT.Item1);
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan) || string.IsNullOrEmpty(ngayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            DateTime thoiGianCanConvert = DateTime.ParseExact(thoiGianCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime ngayCanConvert = DateTime.ParseExact(ngayCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanPhuPham.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCanConvert && x.NgayCan == ngayCanConvert);

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
                item.TrongLuong,
                item.TrongLuongTare
            }, options);

            item.TrongLuong = model.TrongLuong;
            item.TrongLuongTare = model.TrongLuongTare;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
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
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngayCan}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngayCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPham>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan) || string.IsNullOrEmpty(ngayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime thoiGianCanConvert = DateTime.ParseExact(thoiGianCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime ngayCanConvert = DateTime.ParseExact(ngayCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanPhuPham.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCanConvert && x.NgayCan == ngayCanConvert);

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
                item.MaXuongSanXuat
            }, options);
            item.MaXuongSanXuat = model.MaXuongSanXuat;
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
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngayCan}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngayCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPham>(dataT.Item1);
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan) || string.IsNullOrEmpty(ngayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime thoiGianCanConvert = DateTime.ParseExact(thoiGianCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime ngayCanConvert = DateTime.ParseExact(ngayCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanPhuPham.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCanConvert && x.NgayCan == ngayCanConvert);

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
                item.MaSize
            }, options);
            item.MaSize = model.MaSize;
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
        [HttpPost("{maMayTinhCan}/{maUserCan}/{thoiGianCan}/{ngayCan}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham(string maMayTinhCan, string maUserCan, string thoiGianCan, string ngayCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPham>(dataT.Item1);
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan) || string.IsNullOrEmpty(ngayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime thoiGianCanConvert = DateTime.ParseExact(thoiGianCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime ngayCanConvert = DateTime.ParseExact(ngayCan, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanPhuPham.FirstOrDefaultAsync(x => x.MaMayTinhCan == maMayTinhCan && x.MaUserCan == maUserCan && x.ThoiGianCan == thoiGianCanConvert && x.NgayCan == ngayCanConvert);

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
                item.MaLoaiThanhPham
            }, options);
            item.MaLoaiThanhPham = model.MaLoaiThanhPham;
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
    }
}
