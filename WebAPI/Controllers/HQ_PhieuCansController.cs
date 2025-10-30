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
    public class HQ_PhieuCansController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_PhieuCansController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetChiTiets(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCan.GetChiTiets<object>(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopNhanViens(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCan.GetTongHopNhanViens<object>(from, to, xuongId,_context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopThanhPhams(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCan.GetTongHopThanhPhams<object>(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopNhanVienTheoCas(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCan.GetTongHopNhanVienTheoCas<object>(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopTinhLuongs(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCan.GetTongHopTinhLuongs(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopThongKeSanXuat(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCan.GetTongHopThongKeSanXuat(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopThanhPhamDashboards(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmDashBoard.ItemTongHopThanhPhamHQ.OfType<dynamic>().ToList();
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopThanhPhamDashboardForGrids(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmDashBoard.ItemTongHopThanhPhamHQGrid.OfType<dynamic>().ToList();
            return Ok(items);
        }
        #region xử lý phiếu cân
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetChiTiets_XLPC(string dateTime, string xuongId)
        {
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
           

            var items = Vm.VmHQ_PhieuCan.GetChiTietXLPCs<object>(date, xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetPhieuCanDeleteXLPCs(string dateTime, string xuongId)
        {
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
           

            var items = Vm.VmHQ_PhieuCan.GetPhieuCanDeleteXLPCs<object>(date, xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetPhieuCanUpdateXLPCs(string dateTime, string xuongId)
        {
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
           

            var items = Vm.VmHQ_PhieuCan.GetPhieuCanUpdateXLPCs<object>(date, xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(string dateTime,string xuongId)
        {
            DateOnly date1 = DateOnly.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.HqPhieuCans.Where(x => x.Ngay == date1 && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
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
            if (_context.HqPhieuCans.Any(u => u.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new HQ_PhieuCan
            {
                Id = model.Id,
                STT = model.STT,
                Ngay = model.Ngay,
                NgayGio = model.NgayGio,
                MayCan = model.MayCan,
                MaLo = model.MaLo,
                MaSize = model.MaSize,
                MaThanhPham = model.MaThanhPham,
                MaLoaiNguyenLieu = model.MaLoaiNguyenLieu,
                MaNhanVien = model.MaNhanVien,
                MaNhanVienPhucVu = model.MaNhanVienPhucVu,
                MaNhanVienBanKiem = model.MaNhanVienBanKiem,
                TrongLuong = model.TrongLuong,
                TrongLuongTare = model.TrongLuongTare,
                ChiSanLuong = model.ChiSanLuong,
                Status = model.Status,
                TheId = model.TheId,
                TheIdNhanVien = model.TheIdNhanVien,
                GhiChu = model.GhiChu,
                MaXuong = model.MaXuong,
            };
            _context.HqPhieuCans.Add(newItem);
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
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetsByMa(string id)
        {

            var item = _context.HqPhieuCans.FirstOrDefault(x => x.Id == id);
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
                item.MaSize,
                item.MaThanhPham,
                item.MaLoaiNguyenLieu,
                item.MaNhanVien,
                item.MaNhanVienPhucVu,
                item.MaNhanVienBanKiem
            }, options);

            item.MaLo = model.MaLo;
            item.MaSize = model.MaSize;
            item.MaThanhPham = model.MaThanhPham;
            item.MaLoaiNguyenLieu = model.MaLoaiNguyenLieu;
            item.MaNhanVien = model.MaNhanVien;
            item.MaNhanVienPhucVu = model.MaNhanVienPhucVu;
            item.MaNhanVienBanKiem = model.MaNhanVienBanKiem;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
                var newItems = new HQ_PhieuCan_U
                {
                    PhieuCanId = item.Id,
                    STT = item.STT,
                    Ngay = item.Ngay,
                    NgayGio = item.NgayGio,
                    MayCan = item.MayCan,
                    MaLo = item.MaLo,
                    MaSize = item.MaSize,
                    MaThanhPham = item.MaThanhPham,
                    MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                    MaNhanVien = item.MaNhanVien,
                    MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                    MaNhanVienBanKiem = item.MaNhanVienBanKiem,
                    TrongLuong = item.TrongLuong,
                    TrongLuongTare = item.TrongLuongTare,
                    ChiSanLuong = item.ChiSanLuong,
                    Status = item.Status,
                    TheId = item.TheId,
                    TheIdNhanVien = item.TheIdNhanVien,
                    GhiChu = item.GhiChu,
                    MaXuong = item.MaXuong
                };
                _context.HqPhieuCanUs.Add(newItems);
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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

            var newItem = new HQ_PhieuCan
            {
                Id = model.Id,
                STT = -1,//model.STT,
                Ngay = item.Ngay,
                NgayGio = item.NgayGio,
                MayCan = item.MayCan,
                MaLo = item.MaLo,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                MaNhanVien = item.MaNhanVien,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                MaNhanVienBanKiem = item.MaNhanVienBanKiem,
                TrongLuong = 0,//model.TrongLuong,
                TrongLuongTare = item.TrongLuongTare,
                ChiSanLuong = item.ChiSanLuong,
                Status = item.Status,
                TheId = item.TheId,
                TheIdNhanVien = item.TheIdNhanVien,

                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu,
                MaXuong = item.MaXuong,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.HqPhieuCans.Remove(item);
                    _context.HqPhieuCans.Add(newItem);
                    var newItemDs = new HQ_PhieuCan_D
                    {
                        PhieuCanId = item.Id,
                        STT = item.STT,
                        Ngay = item.Ngay,
                        NgayGio = item.NgayGio,
                        MayCan = item.MayCan,
                        MaLo = item.MaLo,
                        MaSize = item.MaSize,
                        MaThanhPham = item.MaThanhPham,
                        MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                        MaNhanVien = item.MaNhanVien,
                        MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                        MaNhanVienBanKiem = item.MaNhanVienBanKiem,
                        TrongLuong = item.TrongLuong,
                        TrongLuongTare = item.TrongLuongTare,
                        ChiSanLuong = item.ChiSanLuong,
                        Status = item.Status,
                        TheId = item.TheId,
                        TheIdNhanVien = item.TheIdNhanVien,
                        NgayC = DateTime.Now,
                        GhiChu = item.GhiChu,
                        MaXuong = item.MaXuong
                    };
                    _context.HqPhieuCanDs.Add(newItemDs);
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
                item.MaXuong
            }, options);
            item.MaXuong = model.MaXuong;
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

        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> ChuyenLoaiNguyenLieu(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
                item.MaLoaiNguyenLieu
            }, options);
            item.MaLoaiNguyenLieu = model.MaLoaiNguyenLieu;
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> ChuyenNhanVien(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
                item.MaNhanVien
            }, options);
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
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> ChuyenNhanVienPhucVu(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
                item.MaNhanVienPhucVu
            }, options);
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
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> ChuyenNhanVienBanKiem(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCan>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HqPhieuCans.FirstOrDefaultAsync(x => x.Id == id);

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
                item.MaNhanVienBanKiem
            }, options);
            item.MaNhanVienBanKiem = model.MaNhanVienBanKiem;
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
