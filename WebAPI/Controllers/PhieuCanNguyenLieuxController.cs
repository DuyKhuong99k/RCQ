using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using AppViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using Vars;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanNguyenLieuxController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanNguyenLieuxController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;


        private class BaoCaoChiTietModel
        {
            public DateTime Ngay { get; set; }
            public DateTime ThoiGian { get; set; }
            public string MaPhuongTien { get; set; }
            public int Chuyen { get; set; }
            public string TenPhuongTien { get; set; }
            public string TenNCC { get; set; }
            public string MaLo { get; set; }
            public string MaAo { get; set; }
            public string TenLoaiCa { get; set; }
            public string MaLoaiThanhPham { get; set; }   // mới thêm
            public string TenThanhPham { get; set; }
            public string MaSize { get; set; }             // mới thêm
            public string TenSize { get; set; }
            public string TenMau { get; set; }
            public decimal TrongLuong { get; set; }
            public decimal TrongLuongTare { get; set; }
            public decimal TyLeNuoc { get; set; }
            public decimal TrongLuongOrg { get; set; }
            public int Pheu { get; set; }
            public string MaBanCatTiet { get; set; }
            public string TenBanCatTiet { get; set; }
            public string MaMayTinhCan { get; set; }
            public int MaXuongSanXuat { get; set; }
            public string TenXuong { get; set; }

            // Trường bổ sung
            public bool IsTap { get; set; }
        }
        private class BaoCaoTongHopPhuongTienModel
        {
            public DateTime Ngay { get; set; }
            public int MaXuongSanXuat { get; set; }
            public string TenXuong { get; set; }
            public string MaPhuongTien { get; set; }
            public string TenPhuongTien { get; set; }
            public string MaLo { get; set; }
            public string MaAo { get; set; }
            public string? TenLoaiCa { get; set; }
            public string TenThanhPham { get; set; }
            public string? MaLoaiThanhPham { get; set; }    // Thêm mã loại cho thanh phẩm
            public string TenSize { get; set; }
            public string? MaSize { get; set; }             // Thêm mã cho size
            public string? TenMau { get; set; }
            public decimal TrongLuong { get; set; }
            public DateTime BatDau { get; set; }
            public DateTime KetThuc { get; set; }
            public bool IsTap { get; set; }

        }
        public class ThanhPhamTheoLoModel
        {
            public DateTime Ngay { get; set; }
            public string MaLo { get; set; }
            public string MaLoaiThanhPham { get; set; }
            public string TenThanhPham { get; set; }
            public string MaSize { get; set; }
            public string TenSize { get; set; }
            public decimal TrongLuong { get; set; }
            public bool IsTap { get; set; }  // Cờ xác định có phải là Táp hay không
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamDashBoard(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetTongHopThanhPhamDashBoard<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuKhongGhiNhan(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetChiTietPhieuCanKhongTheGhiNhanDuLieu<object>(date1, date2);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamDashBoardDB_St(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamNguyenLieu.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopThanhPhamDashBoardDB(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetTongHopThanhPhamDashBoard<object>(date1, date2, xuongId);
            return items;
        }


        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongQuanDB(string fromDate, string toDate, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetTongQuans<object>(date1, date2);
            return items;
        }

        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanChiTiets(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }

            try
            {
                DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var items = Vm.VmPhieuCanNguyenLieu.GetPhieuCanChiTiets<object>(from, to);
                var itemThanhPhamNguyenLieu = Vm.VmThanhPhamNguyenLieu.Items;
                string comName = Vm.VmApp.ComName ?? "HL";

                switch (comName.ToUpper())
                {
                    case nameof(ComNames.HL):
                        var result = items.Cast<dynamic>().Select(x =>
                        {
                            try
                            {
                                string maLoaiThanhPham = x.MaLoaiThanhPham?.ToString();
                                var matchedTP = itemThanhPhamNguyenLieu.FirstOrDefault(tp => tp.Ma == maLoaiThanhPham);
                                bool isTap = matchedTP?.IsPhuPhamCaTap == true;

                                return new BaoCaoChiTietModel
                                {
                                    Ngay = Convert.ToDateTime(x.Ngay),
                                    ThoiGian = Convert.ToDateTime(x.ThoiGian),
                                    MaPhuongTien = x.MaPhuongTien?.ToString(),
                                    Chuyen = Convert.ToInt32(x.Chuyen),
                                    TenPhuongTien = x.TenPhuongTien?.ToString(),
                                    TenNCC = x.TenNCC?.ToString(),
                                    MaLo = x.MaLo?.ToString(),
                                    MaAo = x.MaAo?.ToString(),
                                    TenLoaiCa = x.TenLoaiCa?.ToString(),
                                    MaLoaiThanhPham = maLoaiThanhPham,
                                    TenThanhPham = x.TenThanhPham?.ToString(),
                                    MaSize = x.MaSize?.ToString(),
                                    TenSize = x.TenSize?.ToString(),
                                    TenMau = x.TenMau?.ToString(),
                                    TrongLuong = isTap ? -1 * Convert.ToDecimal(x.TrongLuong) : Convert.ToDecimal(x.TrongLuong),
                                    TrongLuongTare = Convert.ToDecimal(x.TrongLuongTare),
                                    TyLeNuoc = Convert.ToDecimal(x.TyLeNuoc),
                                    TrongLuongOrg = Convert.ToDecimal(x.TrongLuongOrg),
                                    Pheu = Convert.ToInt32(x.Pheu),
                                    MaBanCatTiet = x.MaBanCatTiet?.ToString(),
                                    TenBanCatTiet = x.TenBanCatTiet?.ToString(),
                                    MaMayTinhCan = x.MaMayTinhCan?.ToString(),
                                    MaXuongSanXuat = Convert.ToInt32(x.MaXuongSanXuat),
                                    TenXuong = x.TenXuong?.ToString(),
                                    IsTap = isTap
                                };
                            }
                            catch (Exception exInner)
                            {
                                // Có thể log lỗi exInner ở đây
                                return null; // Bỏ qua bản ghi lỗi
                            }
                        })
                        .Where(r => r != null)
                        .ToList();

                        return Ok(result);

                    default:
                        return Ok(items);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return BadRequest($"Lỗi khi xử lý dữ liệu: {ex.Message}");
            }
        }

        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopNCCs(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetPhieuCanTongHopNCCs<object>(from, to);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopBCTs(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetPhieuCanTongHopBCTs<object>(from, to);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopPhuongTiens(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetPhieuCanTongHopPhuongTiens<object>(from, to);
            var itemThanhPhamNguyenLieu = Vm.VmThanhPhamNguyenLieu.Items;

            string comName = Vm.VmApp.ComName ?? "HL";
            switch (comName.ToUpper())
            {
                case nameof(ComNames.HL):
                    var result = items.Cast<dynamic>().Select(x =>
                    {
                        string maLoaiThanhPham = x.MaLoaiThanhPham?.ToString();
                        var matchedTP = itemThanhPhamNguyenLieu.FirstOrDefault(tp => tp.Ma == maLoaiThanhPham);
                        bool isTap = matchedTP?.IsPhuPhamCaTap == true;

                        return new BaoCaoTongHopPhuongTienModel
                        {
                            Ngay = Convert.ToDateTime(x.Ngay),
                            MaXuongSanXuat = Convert.ToInt32(x.MaXuongSanXuat),
                            TenXuong = x.TenXuong?.ToString(),
                            MaPhuongTien = x.MaPhuongTien?.ToString(),
                            TenPhuongTien = x.TenPhuongTien?.ToString(),
                            MaLo = x.MaLo?.ToString(),
                            MaAo = x.MaAo?.ToString(),
                            TenLoaiCa = x.TenLoaiCa?.ToString(),
                            MaLoaiThanhPham = maLoaiThanhPham,
                            TenThanhPham = x.TenThanhPham?.ToString(),
                            MaSize = x.MaSize?.ToString(),
                            TenSize = x.TenSize?.ToString(),
                            TenMau = x.TenMau?.ToString(),
                            TrongLuong = isTap ? -1 * Convert.ToDecimal(x.TrongLuong) : Convert.ToDecimal(x.TrongLuong),
                            BatDau = Convert.ToDateTime(x.BatDau),
                            KetThuc = Convert.ToDateTime(x.KetThuc),
                            IsTap = isTap
                        };
                    }).ToList();

                    return Ok(result);

                default:
                    return Ok(items);
            }
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopLos(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetPhieuCanTongHopLos<object>(from, to);
            var itemThanhPhamNguyenLieu = Vm.VmThanhPhamNguyenLieu.Items;

            string comName = Vm.VmApp.ComName ?? "HL";
            switch (comName.ToUpper())
            {
                case nameof(ComNames.HL):
                    var result = items.Cast<dynamic>().Select(x =>
                    {
                        string maLoaiThanhPham = x.MaLoaiThanhPham?.ToString();
                        var matchedTP = itemThanhPhamNguyenLieu.FirstOrDefault(tp => tp.Ma == maLoaiThanhPham);
                        bool isTap = matchedTP?.IsPhuPhamCaTap == true;

                        return new ThanhPhamTheoLoModel
                        {
                            IsTap = isTap,
                            Ngay = Convert.ToDateTime(x.Ngay),
                            MaLo = x.MaLo?.ToString(),
                            MaLoaiThanhPham = maLoaiThanhPham,
                            TenThanhPham = x.TenThanhPham?.ToString(),
                            MaSize = x.MaSize?.ToString(),
                            TenSize = x.TenSize?.ToString(),
                            TrongLuong = isTap ? -1 * Convert.ToDecimal(x.TrongLuong) : Convert.ToDecimal(x.TrongLuong)

                        };
                    }).ToList();

                    return Ok(result);

                default:
                    return Ok(items);
            }
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetsCaTra(string dateTime, string xuongId)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetsCaTra<object>(date, xuongId);
            return items;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertNhapCaTra(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanNguyenLieu>(dataT.Item1);
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
            if (model == null || string.IsNullOrEmpty(model.MaMayTinhCan) || string.IsNullOrEmpty(model.MaUserCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Thông tin không hợp lệ."
                });
            }

            // Kiểm tra phiếu cân có tồn tại chưa
            bool phieuCanTonTai = _context.PhieuCanNguyenLieu
                .Any(u => u.MaMayTinhCan == model.MaMayTinhCan
                       && u.MaUserCan == model.MaUserCan
                       && u.ThoiGianCan == model.ThoiGianCan);

            if (phieuCanTonTai)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phiếu cân đã tồn tại."
                });
            }
            var newItem = new PhieuCanNguyenLieu
            {
                TrongLuong = model.TrongLuong,
                MaPhuongTien = model.MaPhuongTien,
                TyLeNuoc = model.TyLeNuoc,
                TrongLuongOrg = model.TrongLuongOrg,
                MaBanCatTiet = model.MaBanCatTiet,
                NhaCC = model.NhaCC,
                MSL = model.MSL,
                MaAo = model.MaAo,
                MaLoaiThanhPham = model.MaLoaiThanhPham,
                MaSize = model.MaSize,
                SuDung = model.SuDung,
                MaXuongSanXuat = model.MaXuongSanXuat,
                MaMayTinhCan = model.MaMayTinhCan,
                Ngay = model.Ngay,
                ThoiGianCan = model.ThoiGianCan,
                MaUserCan = model.MaUserCan,
                MaMau = model.MaMau,
                TrongLuongTare = model.TrongLuongTare,
                Pheu = model.Pheu,
                Chuyen = model.Chuyen,
                MaLoaiCa = model.MaLoaiCa,
                GhiChu = model.GhiChu,
            };
            _context.PhieuCanNguyenLieu.Add(newItem);
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
        
        #region Báo Cáo Thành Phẩm 2
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTietThanhPham2s(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetChiTietThanhPham2s<object>(date1, date2);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopSanPhamThanhPham2(string fromDate, string toDate)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetTongHopSanPhamThanhPham2<object>(date1, date2);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTyLeThuHoiThanhPham2(string fromDate, string toDate,string xuongId)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetTyLeThuHoiThanhPham2<object>(date1, date2,xuongId);
            return items;
        }
        #endregion
        //[HttpGet("{fromDate}/{toDate}/{xuongId}")]
        //[Authorize]
        //public async Task<ActionResult<IEnumerable<object>>> GetChiTietPhieuCanKhongTheGhiNhanDuLieu(string fromDate, string toDate,string xuongId)
        //{
        //    if (_context.PhieuCanNguyenLieu == null)
        //    {
        //        return NotFound();
        //    }
        //    DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    var items = Vm.VmPhieuCanNguyenLieu.GetChiTietPhieuCanKhongTheGhiNhanDuLieu<object>(date1, date2);
        //    return items;
        //}
        #region xử lý phiếu cân
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCan_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanNguyenLieu.GetPhieuCan<object>(date1, xuongId);
            return items;
        }
        [HttpGet("{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAos(string xuongId)
        {
            if (_context.PhieuCanNguyenLieu == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanNguyenLieu.GetAos(xuongId);
            return items;
        }
        [HttpGet("{dateTime}")]
        [Authorize]
        public IActionResult GetAllsWithDate(string dateTime)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = _context.PhieuCanNguyenLieu.Where(x => x.Ngay == date1).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanNguyenLieu>(dataT.Item1);
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
            if (_context.PhieuCanNguyenLieu.Any(u => u.MaMayTinhCan == model.MaMayTinhCan && u.MaUserCan == model.MaUserCan && u.ThoiGianCan == model.ThoiGianCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanNguyenLieu
            {
                MaMayTinhCan = model.MaMayTinhCan,
                MaUserCan = model.MaUserCan,
                ThoiGianCan = model.ThoiGianCan,
                Ngay = model.Ngay,
                NhaCC = model.NhaCC,
                MSL = model.MSL,
                MaAo = model.MaAo,
                MaPhuongTien = model.MaPhuongTien,
                MaLoaiCa = model.MaLoaiCa,
                MaLoaiThanhPham = model.MaLoaiThanhPham,
                MaSize = model.MaSize,
                MaMau = model.MaMau,
                MaBanCatTiet = model.MaBanCatTiet,
                TrongLuong = model.TrongLuong,
                SuDung = model.SuDung,
                GhiChu = model.GhiChu,
                MaXuongSanXuat = model.MaXuongSanXuat,
                TyLeNuoc = model.TyLeNuoc,
                TrongLuongOrg = model.TrongLuongOrg,
                TrongLuongTare = model.TrongLuongTare,
                Pheu = model.Pheu,
                Chuyen = model.Chuyen,
                Id = model.Id
            };
            _context.PhieuCanNguyenLieu.Add(newItem);
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

            if (sanitizedBiosId.Contains("_"))
            {
                // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
                BiosIdEncry = sanitizedBiosId.Replace("_", "/");
            }
            else
            {
                // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
                BiosIdEncry = sanitizedBiosId;
            }
            var biosIdDecry = Security.Crypt.ED.DecryptString(BiosIdEncry);
            return biosIdDecry;
        }
        [HttpGet("{ngay}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan}")]
        [Authorize]
        public IActionResult GetsByMa(string ngay, string xuongId, string maMayTinhCan, string maUserCan, string thoiGianCan)
        {

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime timeConvert = DateTime.ParseExact(thoiGianCan, "HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanNguyenLieu.FirstOrDefault(u => u.Ngay == ngayConvert && u.MaXuongSanXuat == xuongId && u.MaMayTinhCan == maMayTinhCan && u.MaUserCan == maUserCan && u.ThoiGianCan == timeConvert);
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
        [HttpPost("{dateTime}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan}")]
        [Authorize]
        public async Task<IActionResult> Update(string dateTime, string xuongId, string maMayTinhCan, string maUserCan, string thoiGianCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanNguyenLieu>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maMayTinhCan.ToString()) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime timeConvert = DateTime.ParseExact(thoiGianCan, "HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanNguyenLieu.FirstOrDefaultAsync(u => u.Ngay == ngayConvert && u.MaXuongSanXuat == xuongId && u.MaMayTinhCan == maMayTinhCan && u.MaUserCan == maUserCan && u.ThoiGianCan == timeConvert);

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
                item.ThoiGianCan,
                item.MaLoaiCa,
                item.NhaCC,
                item.MaLoaiThanhPham,
                item.MSL,
                item.MaSize,
                item.MaPhuongTien,
                item.MaMau,
                item.MaAo,
                item.MaBanCatTiet,
                item.SuDung,
            }, options);

            item.ThoiGianCan = model.ThoiGianCan;
            item.MaLoaiCa = model.MaLoaiCa;
            item.NhaCC = model.NhaCC;
            item.MaLoaiThanhPham = model.MaLoaiThanhPham;
            item.MSL = model.MSL;
            item.MaSize = model.MaSize;
            item.MaPhuongTien = model.MaPhuongTien;
            item.MaMau = model.MaMau;
            item.MaAo = model.MaAo;
            item.MaBanCatTiet = model.MaBanCatTiet;
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
        [HttpPost("{dateTime}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan}")]
        [Authorize]
        public async Task<IActionResult> Delete(string dateTime, string xuongId, string maMayTinhCan, string maUserCan, string thoiGianCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanNguyenLieu>(dataT.Item1);
            if (string.IsNullOrEmpty(maMayTinhCan.ToString()) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime timeConvert = DateTime.ParseExact(thoiGianCan, "HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanNguyenLieu.FirstOrDefaultAsync(u => u.Ngay == ngayConvert && u.MaXuongSanXuat == xuongId && u.MaMayTinhCan == maMayTinhCan && u.MaUserCan == maUserCan && u.ThoiGianCan == timeConvert);

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
                item.TyLeNuoc,
                item.TrongLuongTare,
                item.TrongLuongOrg,
                item.Pheu
            }, options);

            var newItem = new PhieuCanNguyenLieu
            {
                MaMayTinhCan = item.MaMayTinhCan,
                MaUserCan = item.MaUserCan,
                ThoiGianCan = item.ThoiGianCan,
                Ngay = item.Ngay,
                NhaCC = item.NhaCC,
                MSL = item.MSL,
                MaAo = item.MaAo,
                MaPhuongTien = item.MaPhuongTien,
                MaLoaiCa = item.MaLoaiCa,
                MaLoaiThanhPham = item.MaLoaiThanhPham,
                MaSize = item.MaSize,
                MaMau = item.MaMau,
                MaBanCatTiet = item.MaBanCatTiet,
                TrongLuong = model.TrongLuong,
                SuDung = model.SuDung,
                MaXuongSanXuat = item.MaXuongSanXuat,
                TyLeNuoc = model.TyLeNuoc,
                TrongLuongOrg = model.TrongLuongOrg,
                TrongLuongTare = model.TrongLuongTare,
                Pheu = model.Pheu,
                Chuyen = item.Chuyen,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanNguyenLieu.Remove(item);
                    _context.PhieuCanNguyenLieu.Add(newItem);
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
        [HttpPost("{dateTime}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong(string dateTime, string xuongId, string maMayTinhCan, string maUserCan, string thoiGianCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanNguyenLieu>(dataT.Item1);

            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime timeConvert = DateTime.ParseExact(thoiGianCan, "HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanNguyenLieu.FirstOrDefaultAsync(u => u.Ngay == ngayConvert && u.MaXuongSanXuat == xuongId && u.MaMayTinhCan == maMayTinhCan && u.MaUserCan == maUserCan && u.ThoiGianCan == timeConvert);

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
        [HttpPost("{dateTime}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize(string dateTime, string xuongId, string maMayTinhCan, string maUserCan, string thoiGianCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanNguyenLieu>(dataT.Item1);

            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime timeConvert = DateTime.ParseExact(thoiGianCan, "HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanNguyenLieu.FirstOrDefaultAsync(u => u.Ngay == ngayConvert && u.MaXuongSanXuat == xuongId && u.MaMayTinhCan == maMayTinhCan && u.MaUserCan == maUserCan && u.ThoiGianCan == timeConvert);

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
        [HttpPost("{dateTime}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham(string dateTime, string xuongId, string maMayTinhCan, string maUserCan, string thoiGianCan, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanNguyenLieu>(dataT.Item1);

            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime timeConvert = DateTime.ParseExact(thoiGianCan, "HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanNguyenLieu.FirstOrDefaultAsync(u => u.Ngay == ngayConvert && u.MaXuongSanXuat == xuongId && u.MaMayTinhCan == maMayTinhCan && u.MaUserCan == maUserCan && u.ThoiGianCan == timeConvert);

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
