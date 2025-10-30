using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using Models.Repos.SoketModels;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanSoCheDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanSoCheDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanSoCheDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanSoCheDinhHinh.GetTongHopThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamsDB(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanSoCheDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamSoCheDinhHinh.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object
            return items;
        }

        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanSoCheDinhHinhs(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanSoCheDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanSoCheDinhHinhs<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopsNhanVien(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanSoCheDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanTongHopsNhanVien<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopsLoaiTP(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanSoCheDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanTongHopsLoaiTP<object>(fromDate, toDate, xuongId);
            return items;
        }
        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanSoCheDinhHinh == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCan_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanSoCheDinhHinh.Where(x => x.Ngay == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSoCheDinhHinh>(dataT.Item1);
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
            if (_context.PhieuCanSoCheDinhHinh.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanSoCheDinhHinh
            {
                STT = model.STT,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaLo = model.MaLo,
                MaLoaiCa = model.MaLoaiCa,
                MaThanhPham = model.MaThanhPham,
                MaNhanVien = model.MaNhanVien,
                MaXuong = model.MaXuong,
                MaMayCan = model.MaMayCan,
                TrongLuong = model.TrongLuong,
                MaMayLangDa = model.MaMayLangDa,
                MaUserCan = model.MaUserCan,
                GhiChu = model.GhiChu,
            };
            _context.PhieuCanSoCheDinhHinh.Add(newItem);
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
            var item = _context.PhieuCanSoCheDinhHinh.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
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
            var model = JsonSerializer.Deserialize<PhieuCanSoCheDinhHinh>(dataT.Item1);
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
            var item = await _context.PhieuCanSoCheDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaLoaiCa,
                item.MaThanhPham,
                item.MaMayLangDa,
                item.MaNhanVien
            }, options);

            item.MaLo = model.MaLo;
            item.MaLoaiCa = model.MaLoaiCa;
            item.MaThanhPham = model.MaThanhPham;
            item.MaMayLangDa = model.MaMayLangDa;
            item.MaNhanVien = model.MaNhanVien;
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
            var model = JsonSerializer.Deserialize<PhieuCanSoCheDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanSoCheDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

            var newItem = new PhieuCanSoCheDinhHinh
            {
                STT = model.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaThanhPham = item.MaThanhPham,
                MaNhanVien = item.MaNhanVien,
                MaXuong = item.MaXuong,
                MaMayCan = item.MaMayCan,
                TrongLuong = model.TrongLuong,
                MaMayLangDa = item.MaMayLangDa,
                MaUserCan = item.MaUserCan,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanSoCheDinhHinh.Remove(item);
                    _context.PhieuCanSoCheDinhHinh.Add(newItem);
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
            var model = JsonSerializer.Deserialize<PhieuCanSoCheDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanSoCheDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

            var newItem = new PhieuCanSoCheDinhHinh
            {
                STT = item.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaThanhPham = item.MaThanhPham,
                MaNhanVien = item.MaNhanVien,
                MaXuong = model.MaXuong,
                MaMayCan = item.MaMayCan,
                TrongLuong = item.TrongLuong,
                MaMayLangDa = item.MaMayLangDa,
                MaUserCan = item.MaUserCan,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanSoCheDinhHinh.Remove(item);
                    _context.PhieuCanSoCheDinhHinh.Add(newItem);
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

        //[HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        //[Authorize]
        //public async Task<IActionResult> ChuyenSize(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        //{
        //    var model = JsonSerializer.Deserialize<PhieuCanSoCheDinhHinh>(dataT.Item1);
        //    Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
        //    if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Vui lòng cung cấp đủ thông tin!."
        //        });
        //    }
        //    DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        //    var item = await _context.PhieuCanSoCheDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

        //    if (item == null)
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Mục không tồn tại."
        //        });
        //    }

        //    Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors)
        //                                      .Select(e => e.ErrorMessage)
        //                                      .ToList();

        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Dữ liệu không hợp lệ.",
        //            Errors = errors
        //        });
        //    }
        //    var options = new JsonSerializerOptions
        //    {
        //        WriteIndented = true
        //    };
        //    Lưu trạng thái cũ của đối tượng trước khi thay đổi
        //   var oldData = JsonSerializer.Serialize(new
        //   {
        //       item.MaSize
        //   }, options);
        //    item.MaSize = model.MaSize;
        //    item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
        //            Errors = new List<string> { ex.Message }
        //        });
        //    }
        //    return Ok(new ApiResponse
        //    {
        //        Success = true,
        //        Message = "Cập nhật thông tin thành công!"
        //    });
        //}
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanSoCheDinhHinh>(dataT.Item1);
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
            var item = await _context.PhieuCanSoCheDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

        #region Tính Lương
        #region sơ chế
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> ReloadSanLuongSoChe(string dateTime, string xuongId)
        {
            if (_context.NhanVienDaiThanh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.ReloadSanLuongSoChe(date1, xuongId);
            return items;
        }
        public async Task<IActionResult> KetChuyenSanLuongSoChe(string dateTime, string xuongId, Tuple<string> dataT)
        {
            try
            {
                var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                var sanPhamBravo = Vm.VmSanPhamBravo.GetSanPhamKiem();
                if (Vm.VmNhanVien.SanLuongNhanVienKiems.Any())
                {
                    string listSanLuongNhanVienSoCheSelectedItems = dataT.Item1;
                    string[] sanLuongNhanVienSoChes = listSanLuongNhanVienSoCheSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    var sanLuongNhanVienKiemSelectedItems = new List<BravoModelV1.Model.SanLuongNhanVienSoChe>();
                    foreach (var sanLuongNhanVienSoCheSelectedItem in sanLuongNhanVienSoChes)
                    {
                        // Làm sạch chuỗi JSON escape
                        string cleanedJson = sanLuongNhanVienSoCheSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                        // Deserialize JSON thành đối tượng NhanVienKiem
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var sanLuongNhanVienSoChe = JsonSerializer.Deserialize<BravoModelV1.Model.SanLuongNhanVienSoChe>(cleanedJson, options);

                        // Thêm đối tượng vào danh sách items nếu nó không null
                        if (sanLuongNhanVienSoChe != null)
                        {
                            sanLuongNhanVienKiemSelectedItems.Add((BravoModelV1.Model.SanLuongNhanVienSoChe)sanLuongNhanVienSoChe);
                        }
                    }
                    var items = sanLuongNhanVienKiemSelectedItems.Cast<BravoModelV1.Model.SanLuongNhanVienSoChe>().ToList();
                    try
                    {
                        var phieuCanSoCheDinhHinhs = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>(Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanSoCheTinhLuongs(date1, items, 3));
                        var ids = phieuCanSoCheDinhHinhs.Select(x => x.MaSanPham);
                        bool isF = false;
                        //foreach(var phieuCanDinhHinh in itemsP)
                        //{
                        //    var ps = PhieuCanVm.PhieuCanSoCheDinhHinhs
                        //        .Where(x => x.MaNhanVien == phieuCanDinhHinh.MaNhanVien);
                        //    if(ps.Any())
                        //    {
                        //        isF = true;
                        //        break;
                        //    }
                        //}
                        var sanPhamIds = phieuCanSoCheDinhHinhs
                            .Select(x => x.MaSanPham)
                            .Distinct()
                            .ToList();
                        if (sanPhamIds != null)
                        {
                            foreach (var item in sanPhamIds)
                            {
                                var log = Vm.VmLogKetChuyen.Get(
                                    date1,
                                    xuongId,
                                    item,
                                    3);
                                if (log != null)
                                {
                                    isF = true;
                                    break;
                                }
                            }
                        }

                        if (isF == false)
                        {
                            var rows = 0;
                            await Task.Delay(1000);
                            await Task.Run(
                                () =>
                                {
                                    rows = Vm.VmPhieuCanTPDinhHinh.Insert(phieuCanSoCheDinhHinhs);
                                    foreach (var item in sanPhamIds)
                                    {
                                        Vm.VmLogKetChuyen.Insert(
                                            new LogKetChuyenBravo()
                                            {
                                                Gio = DateTime.Now.TimeOfDay,
                                                MaSanPham = item,
                                                MaXuong = xuongId,
                                                Ngay = date1.Date,
                                                tab = 3,
                                                NgayChuyen = DateTime.Now
                                            });
                                    }
                                });
                            await Task.Delay(1000);
                            await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");
                        }
                        else
                        {
                            await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Thao tác đã thực hiện, Không thể thực hiện lại!");
                        }

                        // PhieuCanVm.IsBusy = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Đã xảy ra lỗi.",
                            Errors = new List<string> { ex.Message }
                        });
                    }

                }
                else
                {
                    //MessageBox.Show("Không có dữ liệu");
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Không có dữ liệu!");

                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        #endregion
        #region Kiểm Sơ Chế
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> ReloadSanLuongKiemSoChe(string dateTime, string xuongId)
        {
            if (_context.PhieuCanSoCheDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.ReloadSanLuongKiemSoChe(date1, xuongId);
            return items;
        }
        public async Task<IActionResult> KetChuyenSanLuongKiemSoChe(string dateTime, string xuongId, Tuple<string> dataT)
        {
            try
            {
                var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                string listSanLuongNhanVienToKiemSoCheSelectedItems = dataT.Item1;
                string[] sanLuongNhanVienToKiemSoChes = listSanLuongNhanVienToKiemSoCheSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var sanLuongNhanVienKiemSoCheSelectedItems = new List<BravoModelV1.Model.SanLuongNhanVienKiemSoChe>();
                foreach (var sanLuongNhanVienToKiemSoCheSelectedItem in sanLuongNhanVienToKiemSoChes)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = sanLuongNhanVienToKiemSoCheSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var sanLuongNhanVienToKiemSoChe = JsonSerializer.Deserialize<BravoModelV1.Model.SanLuongNhanVienKiemSoChe>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (sanLuongNhanVienToKiemSoChe != null)
                    {
                        sanLuongNhanVienKiemSoCheSelectedItems.Add((BravoModelV1.Model.SanLuongNhanVienKiemSoChe)sanLuongNhanVienToKiemSoChe);
                    }
                }

                var items = sanLuongNhanVienKiemSoCheSelectedItems.Cast<BravoModelV1.Model.SanLuongNhanVienKiemSoChe>().ToList();

                try
                {

                    var phieuCanSoCheDinhHinhs = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>(Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanSoCheTinhLuongs(date1, items, 4));

                    var ids = phieuCanSoCheDinhHinhs.Select(x => x.MaSanPham);
                    bool isF = false;
                    //foreach(var phieuCanDinhHinh in itemsP)
                    //{
                    //    var ps = PhieuCanVm.PhieuCanSoCheDinhHinhs
                    //        .Where(x => x.MaNhanVien == phieuCanDinhHinh.MaNhanVien);
                    //    if(ps.Any())
                    //    {
                    //        isF = true;
                    //        break;
                    //    }
                    //}
                    var sanPhamIds = phieuCanSoCheDinhHinhs
                        .Select(x => x.MaSanPham)
                        .Distinct()
                        .ToList();
                    if (sanPhamIds != null)
                    {
                        foreach (var item in sanPhamIds)
                        {
                            var log = Vm.VmLogKetChuyen.Get(date1, xuongId, item, 4);
                            if (log != null)
                            {
                                isF = true;
                                break;
                            }
                        }
                    }

                    if (isF == false)
                    {
                        var rows = 0;
                        await Task.Delay(1000);
                        await Task.Run(
                           () =>
                           {
                               rows = Vm.VmPhieuCanTPDinhHinh.Insert(phieuCanSoCheDinhHinhs.ToList());
                               foreach (var item in sanPhamIds)
                               {
                                   Vm.VmLogKetChuyen.Insert(
                                       new LogKetChuyenBravo()
                                       {
                                           Gio = DateTime.Now.TimeOfDay,
                                           MaSanPham = item,
                                           MaXuong = xuongId,
                                           Ngay = date1.Date,
                                           tab = 4,
                                           NgayChuyen = DateTime.Now
                                       });
                               }
                           });
                        Task.Delay(1000);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Thành công!"
                        });
                    }
                    else
                    {
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Thao tác đã thực hiện, Không thể thực hiện lại!");
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Thao tác đã thực hiện, Không thể thực hiện lại!",
                           // Errors = new List<string> { ex.Message }
                        });
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // throw;
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi.",
                        Errors = new List<string> { ex.Message }
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        #endregion
        #endregion
    }
}
