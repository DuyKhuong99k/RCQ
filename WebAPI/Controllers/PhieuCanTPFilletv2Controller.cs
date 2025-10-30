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
using ViewModels.Repos.API;
using WebAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppViewModels;
using ViewModels.Repos.HQ;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanTPFilletv2Controller : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanTPFilletv2Controller(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTiets2HN(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFilletv2.GetChiTiets2HN<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLoaiThanhPhamsHN(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFilletv2.GetTongHopLoaiThanhPhamsHN<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanViensHN(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFilletv2.GetTongHopNhanViensHN<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanViens2(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFilletv2.GetTongHopNhanViens2<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanViens(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPFilletv2.GetTongHopNhanViens<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLoaiThanhPhamFillet(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFilletv2.GetTongHopThanhPhamFillet<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLoaiThanhPhamFilletDB(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamTPFilletv2.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object

            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanVienPhucVu(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFilletv2.GetTongHopNhanVienPhucVu<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopGioLamViec(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFilletv2.GetTongHopGioLamViec<object>(date1, date2, xuongId);
            return items;
        }

        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTPFillet_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanBTPFilletv2 == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFilletv2.GetPhieuCanTPFillet_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{maMayCan}/{maXuong}")]
        [Authorize]
        public IActionResult GetMaxSTT(string dateTime, string maMayCan, string maXuong)
        {
            // Chuyển đổi ngày từ chuỗi sang DateTime
            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            // Lấy danh sách các STT phù hợp với điều kiện
            var sttList = _context.PhieuCanTPFilletv2
                .Where(x => x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong)
                .Select(x => x.STT)
                .ToList();

            // Tìm giá trị STT lớn nhất theo yêu cầu
            int maxSTT = sttList
                .Select(stt => Math.Abs(stt)) // Lấy giá trị tuyệt đối của STT
                .DefaultIfEmpty(0) // Đảm bảo không có phần tử nào thì trả về 0
                .Max();

            return Ok(new
            {
                MaxSTT = maxSTT
            });
        }
        [HttpGet("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maMayCan, string maXuong)
        {
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanTPFilletv2.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
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
        [HttpGet("{ngay}/{maNhanVien}/{maXuong}/{maMayCan}/{sttBTP}/{thePhieuSanLuongId}")]
        [Authorize]
        public IActionResult GetSTT(string ngay, string maNhanVien, string maXuong, string maMayCan, int sttBTP, string thePhieuSanLuongId)
        {
            DateTime ngayConvert;
            if (!DateTime.TryParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ngayConvert))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Ngày không đúng định dạng!"
                });
            }
            var item = _context.PhieuCanTPFilletv2
                .FirstOrDefault(p => p.MaNhanVien == maNhanVien &&
                                     p.Ngay == ngayConvert &&
                                     p.MaXuong == maXuong &&
                                     p.MaMayCan == maMayCan &&
                                     p.STTBTP == sttBTP &&
                                     p.ThePhieuSanLuongId == thePhieuSanLuongId);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại!"
                });
            }

            // Return the STT
            return Ok(new
            {
                Success = true,
                STT = item.STT // Assuming STT is a property of the item
            });
        }
        [HttpGet("{maBan}/{idThePhieuSLFillet}")]
        [Authorize]
        public IActionResult GetDanhSachNhanVienBanTrongLuongByThePhieuSLFillet(string maBan, string idThePhieuSLFillet)
        {
            var item = Vm.VmPhieuCanTPFilletv2.GetDanhSachNhanVienBanTrongLuongByThePhieuSLFillet<object>(maBan, idThePhieuSLFillet);
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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFilletv2>(dataT.Item1);
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
            if (_context.PhieuCanTPFilletv2.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanTPFilletv2
            {
                STT = model.STT,
                CaTra = model.CaTra,
                Gio = model.Gio,
                MaLo = model.MaLo,
                MaLoaiCa = model.MaLoaiCa,
                MaMau = model.MaMau,
                MaMayCan = model.MaMayCan,
                MaSize = model.MaSize,
                MaThanhPham = model.MaThanhPham,
                MaThe = model.MaThe,
                MaUserCan = model.MaUserCan,
                MaXuong = model.MaXuong,
                Ngay = model.Ngay,
                TrongLuongTare = model.TrongLuongTare,
                TrongLuongTra = model.TrongLuongTra,
                STTBTP = model.STTBTP,
                DinhMucThucTe = model.DinhMucThucTe,
                DinhMucYeuCau = model.DinhMucThucTe,
                MaMayCanBTP = model.MaMayCanBTP,
                TrongLuongNhan = model.TrongLuongNhan,
                MaNhanVien = model.MaNhanVien,
                MaBan = model.MaBan,
                ThePhieuSanLuongId = model.ThePhieuSanLuongId,
                GhiChu = model.GhiChu,
                MaNhanVienPhucVu = model.MaNhanVienPhucVu
            };
            _context.PhieuCanTPFilletv2.Add(newItem);
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
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFilletv2>(dataT.Item1);
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
            var item = await _context.PhieuCanTPFilletv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.MaMau,
                item.MaThanhPham,
                item.MaSize,
                item.MaNhanVien,
                item.MaNhanVienPhucVu,
                //item.TrongLuongTra,
                item.DinhMucThucTe,
            }, options);

            item.MaLo = model.MaLo;
            item.MaLoaiCa = model.MaLoaiCa;
            item.MaMau = model.MaMau;
            item.MaThanhPham = model.MaThanhPham;
            item.MaSize = model.MaSize;
            item.MaNhanVien = model.MaNhanVien;
            item.MaNhanVienPhucVu = model.MaNhanVienPhucVu;
            //item.TrongLuongTra = model.TrongLuongTra;
            item.DinhMucThucTe = model.DinhMucThucTe;
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
        [HttpGet("{thePhieuSanLuongId}")]
        [Authorize]
        public IActionResult GetAllByThePhieuSLIds(string thePhieuSanLuongId)
        {
            var item = _context.PhieuCanTPFilletv2.Where(x => x.ThePhieuSanLuongId == thePhieuSanLuongId).ToList();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại!."
                });
            }
            return Ok(new
            {
                Success = true,
                Message = "Lấy thông tin thành công!.",
                Data = item.ToList()
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPFilletv2>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPFilletv2.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

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
                item.TrongLuongTra,
                item.TrongLuongTare
            }, options);

            var newItem = new PhieuCanTPFilletv2
            {
                STT = model.STT,
                CaTra = item.CaTra,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaLoaiCa = item.MaLoaiCa,
                MaMau = item.MaMau,
                MaMayCan = item.MaMayCan,
                MaSize = item.MaSize,
                MaThanhPham = item.MaThanhPham,
                MaThe = item.MaThe,
                MaUserCan = item.MaUserCan,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuongTra = item.TrongLuongTra,
                STTBTP = item.STTBTP,
                DinhMucThucTe = item.DinhMucThucTe,
                DinhMucYeuCau = item.DinhMucThucTe,
                MaMayCanBTP = item.MaMayCanBTP,
                TrongLuongNhan = item.TrongLuongNhan,
                MaNhanVien = item.MaNhanVien,
                MaBan = item.MaBan,
                ThePhieuSanLuongId = item.ThePhieuSanLuongId,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanTPFilletv2.Add(newItem);
                    _context.PhieuCanTPFilletv2.Remove(item);
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

        #region Tính Lương Phụ Fillet
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadSanLuongPhuFillet(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.ReloadSanLuongPhuFillet(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> KetChuyenSanLuongCommand(string dateTime, string xuongId, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                string listSanLuongPhuFilletSelectedItems = dataT.Item1;
                string[] sanLuongPhuFillets = listSanLuongPhuFilletSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var sanLuongPhuFilletSelectedItems = new List<BravoModelV1.Model.SanLuongPhuFillet>();
                foreach (var sanLuongNhanVienKiemSelectedItem in sanLuongPhuFillets)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = sanLuongNhanVienKiemSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var sanLuongPhuFillet = JsonSerializer.Deserialize<BravoModelV1.Model.SanLuongPhuFillet>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (sanLuongPhuFillet != null)
                    {
                        sanLuongPhuFilletSelectedItems.Add((BravoModelV1.Model.SanLuongPhuFillet)sanLuongPhuFillet);
                    }
                }
                var items = new List<BravoModelV1.Model.SanLuongPhuFillet>();
                foreach (var item in sanLuongPhuFilletSelectedItems)
                {
                    var phieuSanLuong = (BravoModelV1.Model.SanLuongPhuFillet)item;
                    items.Add(phieuSanLuong);
                }


                try
                {
                    var phieuCans = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>(
                        Vm.VmNhanVien.GetPhieuCanPhuFilletTinhLuongs(items.ToList(), 6, date1));
                    var ids = phieuCans.Select(x => x.MaSanPham).Distinct().ToList();
                    bool isF = false;
                    if (ids != null)
                    {
                        foreach (var item in ids)
                        {
                            var log = Vm.VmLogKetChuyen.Get(date1, xuongId, item, 6);
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
                                {
                                    Vm.VmLogKetChuyen.Insert(
                                        new LogKetChuyenBravo()
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1.Date,
                                            NgayChuyen = DateTime.Now,
                                            tab = 6
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
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadCaiDatNhanVienBanLinePhuFilletTL(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.ReloadNhanVienBanLine(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{maHoSo}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> FindNhanVienByMaHoSoNhanVienBanLine(string dateTime, string xuongId, string maHoSo)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.FindNhanVienByMaHoSoNhanVienBanLine(date1, xuongId, maHoSo);
            return items;
        }

        [HttpGet("{dateTime}/{xuongId}/{maBanCatTiet}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> AddNhanVienTheoBanPhuFilletTL(string dateTime, string xuongId, string maBanCatTiet, Tuple<string> dataT)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.AddNhanVienTheoBanPhuFilletTL(date1, xuongId, maBanCatTiet, dataT);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{maBanCatTiet}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> RemoveNhanVienTheoBanPhuFilletTL(string dateTime, string xuongId, string maBanCatTiet, Tuple<string> dataT)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.RemoveNhanVienTheoBanPhuFilletTL(date1, xuongId, maBanCatTiet, dataT);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{maLine}/{maThanhPham}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> AddNhanVienTheoLinePhuFilletTL(string dateTime, string xuongId, string maLine, string maThanhPham, Tuple<string> dataT)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.AddNhanVienTheoLinePhuFilletTL(date1, xuongId, maLine, maThanhPham, dataT);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{maLine}/{maThanhPham}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> RemoveNhanVienTheoLinePhuFilletTL(string dateTime, string xuongId, string maLine, string maThanhPham, Tuple<string> dataT)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.RemoveNhanVienTheoLinePhuFilletTL(date1, xuongId, maLine, maThanhPham, dataT);
            return items;
        }
        #endregion
        #region Tính Lương Filletv2
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadTongHopTinhLuong(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFilletv2.LoadTongHopTinhLuong(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> KetChuyenTinhLuongFilletv2(string dateTime, string xuongId, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                string listPhieuCanTongHopTinhLuongSelectedItems = dataT.Item1;
                string[] phieuCanTongHopTinhLuongs = listPhieuCanTongHopTinhLuongSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var phieuCanTongHopTinhLuongSelectedItems = new List<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>();
                foreach (var phieuCanTongHopTinhLuongSelectedItem in phieuCanTongHopTinhLuongs)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = phieuCanTongHopTinhLuongSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var phieuCanTongHopTinhLuong = JsonSerializer.Deserialize<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (phieuCanTongHopTinhLuong != null)
                    {
                        phieuCanTongHopTinhLuongSelectedItems.Add((BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong)phieuCanTongHopTinhLuong);
                    }
                }
                var _items = phieuCanTongHopTinhLuongSelectedItems.Cast<BravoModelV1.Model.FilletV2_PhieuCanTongHopTinhLuong>().ToList();
                //convert to filletTp ver1 bravo

                var items = new List<PhieuCanDinhHinh>();
                foreach (var phieuCanTongHopTinhLuong in _items)
                {
                    var phieuCan = new PhieuCanDinhHinh
                    {
                        Ngay = date1.Date,
                        MaNhanVien = phieuCanTongHopTinhLuong.MaNhanVien,
                        DinhMucYeuCau = phieuCanTongHopTinhLuong.DinhMucYeuCau,
                        DanhGia = phieuCanTongHopTinhLuong.DanhGia,
                        DinhMucThucTe = phieuCanTongHopTinhLuong.DinhMucThucTe,
                        SoRo = phieuCanTongHopTinhLuong.SoRo,
                        SuDung = true,
                        TrongLuongNhan = phieuCanTongHopTinhLuong.TrongLuongNhan,
                        TrongLuongTra = phieuCanTongHopTinhLuong.TrongLuongTra,
                        _Status = 0,
                        MaSanPham = phieuCanTongHopTinhLuong.MaSanPham,
                        TenSanPham = phieuCanTongHopTinhLuong.ThanhPhamName,
                        CaLamViec = "CT04"
                    };

                    items.Add(phieuCan);
                }

                //var rl = MessageBox.Show(
                //    $@"Thực hiện thao tác chuyển dữ liệu lên CSDL tính lương trên {items.Count} dòng dữ liệu?",
                //    "Thông Báo",
                //    MessageBoxButton.YesNo,
                //    MessageBoxImage.Warning);
                //if (rl == MessageBoxResult.Yes)
                //try
                //{
                //AppViewModel.Ins.IsBusy = true;
                var isF = false;
                //foreach(var phieuCanDinhHinh in itemsP)
                //{
                //    var ps = items.Where(x => x.MaNhanVien == phieuCanDinhHinh.MaNhanVien);
                //    if(ps.Any())
                //    {
                //        isF = true;
                //        break;
                //    }
                //}
                var sanPhamIds = items.Select(x => x.MaSanPham).Distinct().ToList();
                if (sanPhamIds != null)
                    foreach (var item in sanPhamIds)
                    {
                        var log = Vm.VmLogKetChuyen.Get(date1, xuongId, item, 9);
                        if (log != null)
                        {
                            isF = true;
                            break;
                        }
                    }

                if (isF == false)
                {
                    var rows = 0;
                    await Task.Delay(1000);
                    await Task.Run(
                        () =>
                        {
                            rows = Vm.VmBV_PhieuCanDinhHinh.Insert(items);
                            foreach (var item in sanPhamIds)
                                Vm.VmLogKetChuyen.Insert(
                                        new LogKetChuyenBravo
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1,
                                            tab = 9,
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

        [HttpPost("{dateTime}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> TPSoftAction_Filletv2(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            // Tạo một tham chiếu đến SignalR Hub
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            try
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", "Khởi tạo TPSoft");
                await Task.Delay(50).ConfigureAwait(false);
                var phieuCans = Vm.VmPhieuCanTPFilletv2.GetsTPSoft<BravoModelV1.EF.PMS_DataRecord>(date1, xuongId);
                var phieuCanTPSoftServer = Vm.VmPMS_Record.Gets<BravoModelV1.EF.PMS_DataRecord>(date1, "09", xuongId);
                var idTPSofts = phieuCanTPSoftServer.Select(x => x.ID)
                    .DefaultIfEmpty(string.Empty)
                    .Distinct()
                    .ToList();
                var insItems = phieuCans.Where(x => !idTPSofts.Contains(x.ID)).ToList();
                var _udaItems = phieuCans.Where(x => idTPSofts.Contains(x.ID)).ToList();

                var udaItems = (from _ups in _udaItems
                                from tpS in phieuCanTPSoftServer
                                where _ups.ID == tpS.ID
                                select new BravoModelV1.EF.PMS_DataRecord
                                {
                                    CMND = _ups.CMND,
                                    ID = _ups.ID,
                                    CongDoanID = _ups.CongDoanID,
                                    DateCreate = tpS.DateCreate,
                                    DateSync = _ups.DateSync,
                                    Status = _ups.Status,
                                    ThoiGian = _ups.ThoiGian,
                                    TrongLuong = _ups.TrongLuong
                                }).ToList();
                udaItems.All(
                    x =>
                    {
                        x.DateSync = new DateTime(1900, 01, 01, 0, 0, 0);
                        return true;
                    });
                insItems.All(
                    x =>
                    {
                        x.DateCreate = DateTime.Now;
                        return true;
                    });
                if (insItems.Any())
                {
                    var batches = Vm.VmPMS_Record.GetSqlsInBatches(insItems);
                    var rows = 0;
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", "Bắt đầu thêm");

                    await Task.Delay(50).ConfigureAwait(false);
                    foreach (var batche in batches)
                    {
                        rows += Vm.VmPMS_Record.Execute(batche);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Đã thực hiện {rows}/{insItems.Count}");
                        await Task.Delay(50).ConfigureAwait(false);
                    }
                }

                if (udaItems.Any())
                {
                    var batches = Vm.VmPMS_Record.GetSqlsInBatches(udaItems);
                    var rows = 0;
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", "Bắt đầu cập nhật");
                    await Task.Delay(50).ConfigureAwait(false);
                    Vm.VmPMS_Record.Delete(udaItems.Select(x => x.ID).ToList());

                    foreach (var batche in batches)
                    {
                        rows += Vm.VmPMS_Record.Execute(batche);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Đã thực hiện {rows}/{udaItems.Count}");
                        await Task.Delay(50).ConfigureAwait(false);
                    }
                }

                await hubContext.Clients.All.SendAsync("ReceiveProgress", "Thực hiện xong!");
                await Task.Delay(500).ConfigureAwait(false);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception exception)
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Có lỗi xảy ra: {exception.Message}");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { exception.Message }
                });
            }

        }

        #endregion

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetsTongHopThangDinhMuc(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPFilletv2 == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPFilletv2.GetsTongHopThangDinhMuc(date1, date2, xuongId);
            return items;
        }
    }
}

