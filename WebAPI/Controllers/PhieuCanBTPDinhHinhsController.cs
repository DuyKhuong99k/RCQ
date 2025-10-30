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
    public class PhieuCanBTPDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanBTPDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
       
        [HttpGet("{dateTime},{xuongId}")]
        public async Task<ActionResult<IEnumerable<PhieuCanBTPDinhHinh>>> GetChiTiets(DateTime dateTime, string xuongId)
        {
            if (_context.PhieuCanBTPDinhHinh == null)
            {
                return NotFound();
            }
            return Vm.VmPhieuCanBTPDinhHinh.GetChiTiets<PhieuCanBTPDinhHinh>(dateTime, xuongId);
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTietsFromdateTodate(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var items = Vm.VmPhieuCanBTPDinhHinh.GetChiTiets<object>(fromDate, toDate, xuongId);

                if (items == null)
                {
                    return NotFound("Không có dữ liệu phù hợp.");
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ và ghi log nếu cần
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }

        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTietPhieuCanChuaSuas(DateTime fromDate, DateTime toDate, string xuongId)
        {
            try
            {
                var items = Vm.VmPhieuCanBTPDinhHinh.GetChiTietPhieuCanChuaSuas<object>(fromDate, toDate, xuongId);

                if (items == null)
                {
                    return NotFound("Không có dữ liệu phù hợp.");
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ và ghi log nếu cần
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }

        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanViens(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPDinhHinh.GetTongHops<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanVienPhucVus(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopNhanVienPhucVus<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLos(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopLos<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamsDB_St(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamBTPDinhHinh.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamsDB(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }






        #region Xử Lý Phiếu Cân
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert_XLPC(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPDinhHinh>(dataT.Item1);
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
            if (_context.PhieuCanBTPDinhHinh.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaXuong == model.MaXuong && u.MaMayCan == model.MaMayCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanBTPDinhHinh
            {
                STT = model.STT,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaUserCan = model.MaUserCan,
                MaMayCan = model.MaMayCan,
                MaLoaiCa = model.MaLoaiCa,
                MaMau = model.MaMau,
                MaSize = model.MaSize,
                MaThanhPham = model.MaThanhPham,
                MaLo = model.MaLo,
                MaThe = model.MaThe,
                MaNhanVien = model.MaNhanVien,
                MaMayLangDa = model.MaMayLangDa,
                TrongLuong = model.TrongLuong,
                IsEnabled = model.IsEnabled,
                MaXuong = model.MaXuong,
                CaTra = model.CaTra,
                GhiChu = model.GhiChu,
                ChiSanLuong = model.ChiSanLuong,
                TrongLuongTare = model.TrongLuongTare,
                TrongLuongBu = model.TrongLuongBu,
                IsOffline = model.IsOffline,
                Id = model.Id
            };
            _context.PhieuCanBTPDinhHinh.Add(newItem);
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
        [HttpGet("{sttBTP}/{ngay}/{maNhanVien}/{maMayCanBTP}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(int sttBTP, string ngay, string maNhanVien, string maMayCanBTP, string maXuong)
        {
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanBTPDinhHinh.FirstOrDefault(x => x.STT == sttBTP && x.Ngay == ngayConvert && x.MaNhanVien == maNhanVien && x.MaMayCan == maMayCanBTP && x.MaXuong == maXuong);
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
        [HttpPost("{sttBTP}/{ngay}/{maMayCanBTP}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update_XLPC(int sttBTP, string ngay, string maMayCanBTP, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPDinhHinh>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCanBTP) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPDinhHinh.FirstOrDefaultAsync(x => x.STT == sttBTP && x.Ngay == ngayConvert && x.MaMayCan == maMayCanBTP && x.MaXuong == maXuong);

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
            var oldData = JsonSerializer.Serialize(new
            {
                item.Gio,
                item.MaLoaiCa,
                item.MaMau,
                item.MaSize,
                item.MaThanhPham,
                item.MaLo,
                item.MaNhanVien,
                //item.MaMayLangDa,
                item.TrongLuong,
                item.MaXuong,
                //item.CaTra,
                item.GhiChu
            }, options);

            item.Gio = model.Gio;
            item.MaLoaiCa = model.MaLoaiCa;
            item.MaMau = model.MaMau;
            item.MaSize = model.MaSize;
            item.MaThanhPham = model.MaThanhPham;
            item.MaLo = model.MaLo;
            item.MaNhanVien = model.MaNhanVien;
            //item.MaMayLangDa = model.MaMayLangDa;
            item.TrongLuong = model.TrongLuong;
            item.MaXuong = model.MaXuong;
            //item.CaTra = model.CaTra;
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
        public async Task<IActionResult> Delete_DinhHinh(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.Gio,
                item.MaLoaiCa,
                item.MaMau,
                item.MaSize,
                item.MaThanhPham,
                item.MaLo,
                item.MaNhanVien,
                item.MaMayLangDa,
                item.TrongLuong,
                item.MaXuong,
                item.CaTra,
                item.GhiChu
            }, options);

            var newItem = new PhieuCanBTPDinhHinh
            {
                STT = model.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaUserCan = item.MaUserCan,
                MaMayCan = item.MaMayCan,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaLo = item.MaLo,
                MaThe = item.MaThe,
                MaNhanVien = item.MaNhanVien,
                MaMayLangDa = item.MaMayLangDa,
                TrongLuong = model.TrongLuong,
                IsEnabled = item.IsEnabled,
                MaXuong = item.MaXuong,
                CaTra = item.CaTra,
                ChiSanLuong = item.ChiSanLuong,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuongBu = item.TrongLuongBu,
                IsOffline = item.IsOffline,
                Id = model.Id,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanBTPDinhHinh.Add(newItem);
                    _context.PhieuCanBTPDinhHinh.Remove(item);
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
        public async Task<IActionResult> Delete_XLPC(int stt, string ngay, string maMayCan, string maXuong)
        {
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.PhieuCanBTPDinhHinh.Remove(item);

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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong_DinhHinh(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaXuong
            }, options);

            var newItem = new PhieuCanBTPDinhHinh
            {
                STT = item.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaUserCan = item.MaUserCan,
                MaMayCan = item.MaMayCan,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaLo = item.MaLo,
                MaThe = item.MaThe,
                MaNhanVien = item.MaNhanVien,
                MaMayLangDa = item.MaMayLangDa,
                TrongLuong = item.TrongLuong,
                IsEnabled = item.IsEnabled,
                MaXuong = model.MaXuong,
                CaTra = item.CaTra,
                ChiSanLuong = item.ChiSanLuong,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuongBu = item.TrongLuongBu,
                IsOffline = item.IsOffline,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanBTPDinhHinh.Add(newItem);
                    _context.PhieuCanBTPDinhHinh.Remove(item);
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
        public async Task<IActionResult> ChuyenSize_DinhHinh(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaSize
            }, options);

            var newItem = new PhieuCanBTPDinhHinh
            {
                STT = item.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaUserCan = item.MaUserCan,
                MaMayCan = item.MaMayCan,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaSize = model.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaLo = item.MaLo,
                MaThe = item.MaThe,
                MaNhanVien = item.MaNhanVien,
                MaMayLangDa = item.MaMayLangDa,
                TrongLuong = item.TrongLuong,
                IsEnabled = item.IsEnabled,
                MaXuong = item.MaXuong,
                CaTra = item.CaTra,
                ChiSanLuong = item.ChiSanLuong,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuongBu = item.TrongLuongBu,
                IsOffline = item.IsOffline,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanBTPDinhHinh.Add(newItem);
                    _context.PhieuCanBTPDinhHinh.Remove(item);
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
        [HttpPost("{sttBTP}/{ngay}/{maMayCanBTP}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize_XLPC(int sttBTP, string ngay, string maMayCanBTP, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPDinhHinh>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCanBTP) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPDinhHinh.FirstOrDefaultAsync(x => x.STT == sttBTP && x.Ngay == ngayConvert && x.MaMayCan == maMayCanBTP && x.MaXuong == maXuong);

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
        [HttpPost("{sttBTP}/{ngay}/{maMayCanBTP}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham_XLPC(int sttBTP, string ngay, string maMayCanBTP, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPDinhHinh>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCanBTP) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPDinhHinh.FirstOrDefaultAsync(x => x.STT == sttBTP && x.Ngay == ngayConvert && x.MaMayCan == maMayCanBTP && x.MaXuong == maXuong);

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
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaThanhPham
            }, options);

            item.MaThanhPham = model.MaThanhPham;
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
