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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.SignalR;
using ViewModels.Repos.HQ;


namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BT_PhieuCanController : ControllerBase
    {
        private readonly dbPMScontext _context;
        private MainViewModel Vm => MainViewModel.Instance;
        public BT_PhieuCanController(dbPMScontext context)
        {
            _context = context;
        }



        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime, string xuongId)
        {
            if (_context.BT_PhieuCan == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmBT_PhieuCan.GetPhieuCan_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.BT_PhieuCan.Where(x => x.Ngay == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<BT_PhieuCan>(dataT.Item1);
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
            if (_context.BT_PhieuCan.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new BT_PhieuCan
            {
                STT = model.STT,
                Ngay = model.Ngay,
                MaMayCan = model.MaMayCan,
                MaXuong = model.MaXuong,
                Gio = model.Gio,
                MaLoaiCa = model.MaLoaiCa,
                MaThanhPham = model.MaThanhPham,
                MaKhachHang = model.MaKhachHang,
                MaNhanVien = model.MaNhanVien,
                TrongLuong = model.TrongLuong,
                SuDung = model.SuDung,
                CreateDateTime = model.CreateDateTime,
                CreateBy = model.CreateBy,
                ModifiedDateTime = model.ModifiedDateTime,
                ModifiedBy = model.ModifiedBy,
                GhiChu = model.GhiChu,
            };
            _context.BT_PhieuCan.Add(newItem);
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
            var item = _context.BT_PhieuCan.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
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
            var model = JsonSerializer.Deserialize<BT_PhieuCan>(dataT.Item1);
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
            var item = await _context.BT_PhieuCan.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaThanhPham,
                item.MaKhachHang,
                item.MaNhanVien,
                item.ModifiedDateTime,
                item.ModifiedBy,
            }, options);

            item.MaLoaiCa = model.MaLoaiCa;
            item.MaThanhPham = model.MaThanhPham;
            item.MaKhachHang = model.MaKhachHang;
            item.MaNhanVien = model.MaNhanVien;
            item.ModifiedDateTime = model.ModifiedDateTime;
            item.ModifiedBy = model.ModifiedBy;
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
            var model = JsonSerializer.Deserialize<BT_PhieuCan>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.BT_PhieuCan.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

            var newItem = new BT_PhieuCan
            {
                STT = model.STT,
                Ngay = item.Ngay,
                MaMayCan = item.MaMayCan,
                MaXuong = item.MaXuong,
                Gio = item.Gio,
                MaLoaiCa = item.MaLoaiCa,
                MaThanhPham = item.MaThanhPham,
                MaKhachHang = item.MaKhachHang,
                MaNhanVien = item.MaNhanVien,
                TrongLuong = model.TrongLuong,
                SuDung = item.SuDung,
                CreateDateTime = item.CreateDateTime,
                CreateBy = item.CreateBy,
                ModifiedDateTime = item.ModifiedDateTime,
                ModifiedBy = item.ModifiedBy,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.BT_PhieuCan.Remove(item);
                    _context.BT_PhieuCan.Add(newItem);
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
            var model = JsonSerializer.Deserialize<BT_PhieuCan>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.BT_PhieuCan.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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

            var newItem = new BT_PhieuCan
            {
                STT = item.STT,
                Ngay = item.Ngay,
                MaMayCan = item.MaMayCan,
                MaXuong = model.MaXuong,
                Gio = item.Gio,
                MaLoaiCa = item.MaLoaiCa,
                MaThanhPham = item.MaThanhPham,
                MaKhachHang = item.MaKhachHang,
                MaNhanVien = item.MaNhanVien,
                TrongLuong = item.TrongLuong,
                SuDung = item.SuDung,
                CreateDateTime = item.CreateDateTime,
                CreateBy = item.CreateBy,
                ModifiedDateTime = item.ModifiedDateTime,
                ModifiedBy = item.ModifiedBy,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.BT_PhieuCan.Remove(item);
                    _context.BT_PhieuCan.Add(newItem);
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
        //    var model = JsonSerializer.Deserialize<BT_PhieuCan>(dataT.Item1);
        //    // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
        //    if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Vui lòng cung cấp đủ thông tin!."
        //        });
        //    }
        //    DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        //    var item = await _context.BT_PhieuCan.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

        //    if (item == null)
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Mục không tồn tại."
        //        });
        //    }

        //    // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
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
        //    // Lưu trạng thái cũ của đối tượng trước khi thay đổi
        //    var oldData = JsonSerializer.Serialize(new
        //    {
        //        item.MaSize
        //    }, options);
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
            var model = JsonSerializer.Deserialize<BT_PhieuCan>(dataT.Item1);
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
            var item = await _context.BT_PhieuCan.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
        public async Task<ActionResult<IEnumerable<object>>> LoadTongHopTinhLuongBaoTu(string dateTime, string xuongId)
        {
            if (_context.PhieuSanLuongRaCoiTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.CommandReloadTinhLuongBaoTu(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> KetChuyen(string dateTime, string xuongId)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var tab = 10;
            try
            {
                //PMSMSharedv1.ViewModel.AppViewModel.Ins.IsBusy = true;
                var SanLuongTinhLuongs = Vm.VmNhanVien.CommandReloadTinhLuongBaoTu(date1, xuongId);
                var phieuCans = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>(
                    Vm.VmBV_PhieuCanKiemDinhHinh.GetPhieuCanTinhLuongs(SanLuongTinhLuongs.ToList(), tab));
                var ids = phieuCans.Select(x => x.MaSanPham).Distinct().ToList();
                bool isF = false;
                if (ids != null)
                {
                    foreach (var item in ids)
                    {
                        var log = LogKetChuyenViewModel.Instance
                            .Get(date1, xuongId, item, tab);
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
                            rows = Vm.VmPhieuCanTPDinhHinh.Insert(phieuCans);
                            foreach (var item in ids)
                                Vm.VmLogKetChuyen.Insert(
                                        new LogKetChuyenBravo()
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1,
                                            tab = tab,
                                            NgayChuyen = DateTime.Now
                                        });
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
            catch (Exception ex)
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Lỗi" + ex.ToString());
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                });
            }

        }
        #endregion
        #region Báo Cáo
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetsChiTiet(string fromDate, string toDate, string xuongId)
        {
            if (_context.BT_PhieuCan == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmBT_PhieuCan.GetChiTiets<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHops(string fromDate, string toDate, string xuongId)
        {
            if (_context.BT_PhieuCan == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmBT_PhieuCan.GetTongHops<object>(date1, date2, xuongId);
            return items;
        }
        #endregion
    }
}
