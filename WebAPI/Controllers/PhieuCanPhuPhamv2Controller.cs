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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanPhuPhamv2Controller : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanPhuPhamv2Controller(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
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
            var items = Vm.VmPhieuCanPhuPhamv2.GetChiTiets<object>(from, to);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHops(string fromDate, string toDate)
        {
            if (_context.PhieuCanPhuPham == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanPhuPhamv2.GetTongHops<object>(from, to);
            return items;
        }
        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanPhuPhamv2 == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanPhuPhamv2.GetPhieuCan_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanPhuPhamv2.Where(x => x.Ngay == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPhamv2>(dataT.Item1);
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
            if (_context.PhieuCanPhuPhamv2.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanPhuPhamv2
            {
                STT = model.STT,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaLo = model.MaLo,
                MaThanhPham = model.MaThanhPham,
                MaNhanVien = model.MaNhanVien,
                MaXuong = model.MaXuong,
                MaMayCan = model.MaMayCan,
                TrongLuong = model.TrongLuong,
                GhiChu = model.GhiChu,
                MaThe = model.MaThe,
                SuDung = model.SuDung
            };
            _context.PhieuCanPhuPhamv2.Add(newItem);
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
            var item = _context.PhieuCanPhuPhamv2.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
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
            var model = JsonSerializer.Deserialize<PhieuCanPhuPhamv2>(dataT.Item1);
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
            var item = await _context.PhieuCanPhuPhamv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaNhanVien,
                item.SuDung
            }, options);

            item.MaLo = model.MaLo;
            item.MaThanhPham = model.MaThanhPham;
            item.MaNhanVien = model.MaNhanVien;
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanPhuPhamv2>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanPhuPhamv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

            var newItem = new PhieuCanPhuPhamv2
            {
                STT = model.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaThanhPham = item.MaThanhPham,
                MaNhanVien = item.MaNhanVien,
                MaXuong = item.MaXuong,
                MaMayCan = item.MaMayCan,
                TrongLuong = model.TrongLuong,
                MaThe = item.MaThe,
                SuDung = model.SuDung,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanPhuPhamv2.Remove(item);
                    _context.PhieuCanPhuPhamv2.Add(newItem);
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
            var model = JsonSerializer.Deserialize<PhieuCanPhuPhamv2>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanPhuPhamv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

            var newItem = new PhieuCanPhuPhamv2
            {
                STT = item.STT,
                Ngay = item.Ngay,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaThanhPham = item.MaThanhPham,
                MaNhanVien = item.MaNhanVien,
                MaXuong = model.MaXuong,
                MaMayCan = item.MaMayCan,
                TrongLuong = item.TrongLuong,
                MaThe = item.MaThe,
                SuDung = item.SuDung,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanPhuPhamv2.Remove(item);
                    _context.PhieuCanPhuPhamv2.Add(newItem);
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
        //    var model = JsonSerializer.Deserialize<PhieuCanPhuPhamv2>(dataT.Item1);
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
        //    var item = await _context.PhieuCanPhuPhamv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
            var model = JsonSerializer.Deserialize<PhieuCanPhuPhamv2>(dataT.Item1);
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
            var item = await _context.PhieuCanPhuPhamv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadTongHopTinhLuong(string dateTime, string xuongId)
        {
            if (_context.PhieuCanPhuPhamv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanPhuPhamv2.CommandReloadTinhLuong(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> CommandKetChuyenTinhLuong(string dateTime, string xuongId, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {

                string listPhieuCanTongHopTinhLuongSelectedItems = dataT.Item1;
                string[] phieuCanTongHopTinhLuongs = listPhieuCanTongHopTinhLuongSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var phieuCanTongHopTinhLuongSelectedItems = new List<object>();
                foreach (var phieuCanTongHopTinhLuongSelectedItem in phieuCanTongHopTinhLuongs)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = phieuCanTongHopTinhLuongSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var phieuCanTongHopTinhLuong = JsonSerializer.Deserialize<object>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (phieuCanTongHopTinhLuong != null)
                    {
                        phieuCanTongHopTinhLuongSelectedItems.Add(phieuCanTongHopTinhLuong);
                    }
                }



                var items = phieuCanTongHopTinhLuongSelectedItems.Cast<dynamic>().ToList();
                var ids = items.Where(x => x.MaSanPham != null)
                    .Select(x => x.MaSanPham)
                    .Distinct()
                    .Cast<string>()
                    .ToList();
                bool isF = false;
                if (ids != null)
                {
                    foreach (var item in ids)
                    {
                        var log = Vm.VmLogKetChuyen.Get(date1, xuongId, item, 11);
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
                            rows = Vm.VmBV_PhieuCanKiemDinhHinh.Insert(phieuCanTongHopTinhLuongSelectedItems.ToList());
                            foreach (var item in ids)
                            {
                                Vm.VmLogKetChuyen
                                    .Insert(
                                        new LogKetChuyenBravo()
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1.Date,
                                            NgayChuyen = DateTime.Now,
                                            tab = 11
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
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });

            }
            catch (Exception exception)
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Lỗi" + exception.ToString());
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { exception.Message }
                });
            }
        }
        #endregion
    }
}
