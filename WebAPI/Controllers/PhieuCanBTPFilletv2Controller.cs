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
    public class PhieuCanBTPFilletv2Controller : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanBTPFilletv2Controller(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]// cho tất cả quyền truy cập
        public async Task<ActionResult<IEnumerable<object>>> GetChiTiets(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPFilletv2.GetChiTiets<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]// cho tất cả quyền truy cập
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanViens(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPFilletv2.GetTongHopNhanViens<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]// cho tất cả quyền truy cập
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopPhucVus(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPFilletv2.GetTongHopPhucVus<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]// cho tất cả quyền truy cập
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanBTPFilletv2.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]// cho tất cả quyền truy cập
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamsDB(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamBTPFilletv2.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object
            return items;
        }


        //[HttpGet("{fromDate}/{toDate}/{xuongId}")]
        //[Authorize]// cho tất cả quyền truy cập
        //public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamsTest(string fromDate, string toDate, string xuongId)
        //{
        //    if (_context.PhieuCanBTPFilletv2 == null)
        //    {
        //        return NotFound();
        //    }
        //    DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    var items = Vm.
        //    return items;
        //}
        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanBTPFillet_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanBTPFilletv2.GetPhieuCanBTPFillet_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanBTPFilletv2.Where(x => x.Ngay == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPFilletv2>(dataT.Item1);
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
            if (_context.PhieuCanBTPFilletv2.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanBTPFilletv2
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
                TrongLuongTare = model.TrongLuongTare,
                MaNhanVienPhucVu = model.MaNhanVienPhucVu,
            };
            _context.PhieuCanBTPFilletv2.Add(newItem);
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
            var item = _context.PhieuCanBTPFilletv2.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
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
        public async Task<IActionResult> Update(int sttBTP, string ngay, string maMayCanBTP, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPFilletv2>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCanBTP) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPFilletv2.FirstOrDefaultAsync(x => x.STT == sttBTP && x.Ngay == ngayConvert && x.MaMayCan == maMayCanBTP && x.MaXuong == maXuong);

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
                item.MaThanhPham,
                item.MaSize,
                item.MaNhanVien,
                item.MaNhanVienPhucVu,
            }, options);

            item.MaLo = model.MaLo;
            item.MaThanhPham = model.MaThanhPham;
            item.MaSize = model.MaSize;
            item.MaNhanVien = model.MaNhanVien;
            item.MaNhanVienPhucVu = model.MaNhanVienPhucVu;
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
        [HttpPost("{sttBTP}/{ngay}/{maMayCanBTP}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(int sttBTP, string ngay, string maMayCanBTP, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPFilletv2>(dataT.Item1);
            if (string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCanBTP) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPFilletv2.FirstOrDefaultAsync(x => x.STT == sttBTP && x.Ngay == ngayConvert && x.MaMayCan == maMayCanBTP && x.MaXuong == maXuong);

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

            var newItem = new PhieuCanBTPFilletv2
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
                TrongLuongTare = model.TrongLuongTare,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanBTPFilletv2.Add(newItem);
                    _context.PhieuCanBTPFilletv2.Remove(item);
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
            var model = JsonSerializer.Deserialize<PhieuCanBTPFilletv2>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanBTPFilletv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

            var newItem = new PhieuCanBTPFilletv2
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
                TrongLuong = model.TrongLuong,
                IsEnabled = item.IsEnabled,
                MaXuong = model.MaXuong,
                CaTra = item.CaTra,
                TrongLuongTare = item.TrongLuongTare,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanBTPFilletv2.Add(newItem);
                    _context.PhieuCanBTPFilletv2.Remove(item);
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
            var model = JsonSerializer.Deserialize<PhieuCanBTPFilletv2>(dataT.Item1);
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
            var item = await _context.PhieuCanBTPFilletv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanBTPFilletv2>(dataT.Item1);
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
            var item = await _context.PhieuCanBTPFilletv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
