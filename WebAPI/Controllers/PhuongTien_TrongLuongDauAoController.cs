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
    public class PhuongTien_TrongLuongDauAoController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhuongTien_TrongLuongDauAoController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<string>>> GetThuKys()
        {
            if (_context.PhuongTien_TrongLuongDauAo == null)
            {
                return NotFound("Context is null.");
            }

            var items = Vm.VmPhuongTien_TrongLuongDauAo.GetThuKys();



            return Ok(items);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<string>>> GetApTais()
        {
            if (_context.PhuongTien_TrongLuongDauAo == null)
            {
                return NotFound("Context is null.");
            }

            var items = Vm.VmPhuongTien_TrongLuongDauAo.GetApTais();



            return Ok(items);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<int>>> GetTais()
        {
            if (_context.PhuongTien_TrongLuongDauAo == null)
            {
                return NotFound("Context is null.");
            }

            var items = Vm.VmPhuongTien_TrongLuongDauAo.GetTais();


            return Ok(items);
        }
        [HttpGet("{dateTime}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetsNgayBatCas(string dateTime)
        {
            if (_context.PhuongTien_TrongLuongDauAo == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhuongTien_TrongLuongDauAo.Gets_NgayBatCa<object>(date);
            return items;
        }
        [HttpGet("{dateTime}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetsNgayNhapXuong(string dateTime)
        {
            if (_context.PhuongTien_TrongLuongDauAo == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhuongTien_TrongLuongDauAo.Gets_NgayNX<object>(date);
            return items;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertNhapDauAo(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhuongTien_TrongLuongDauAo>(dataT.Item1);
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

            // Kiểm tra phiếu cân có tồn tại chưa
            bool phieuCanTonTai = _context.PhuongTien_TrongLuongDauAo
                .Any(u => u.MaPhuongTien == model.MaPhuongTien
                       && u.Ngay == model.Ngay
                       && u.MaAo == model.MaAo
                       && u.MaPhuongTien == model.MaNhaCungCap
                       && u.Chuyen == model.Chuyen);

            if (phieuCanTonTai)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã phiếu cân đã tồn tại."
                });
            }
            var newItem = new PhuongTien_TrongLuongDauAo
            {
                MaPhuongTien = model.MaPhuongTien,
                Ngay = model.Ngay,
                MaAo = model.MaAo,
                CaManh = model.CaManh,
                CaNgopAoGhe = model.CaNgopAoGhe,
                CaNgopAoXe = model.CaNgopAoXe,
                TongHam = model.TongHam,
                CaNgayTruoc = model.CaNgayTruoc,
                CaConLai = model.CaConLai,
                ThuKy = model.ThuKy,
                ApTai = model.ApTai,
                STTChuyen = model.STTChuyen,
                TyLeMoi = model.TyLeMoi,
                GhiChu = model.GhiChu,
                NgayXuatPhat = model.NgayXuatPhat,
                GioXuatPhat = model.GioXuatPhat,
                NgayBatCa = model.NgayBatCa,
                CreateDateTime = model.CreateDateTime,
                CreateBy = model.CreateBy,
                ModifiedDateTime = model.ModifiedDateTime,
                ModifiedBy = model.ModifiedBy,
                IsVungNuoiBlocked = model.IsVungNuoiBlocked,
                MaNhaCungCap = model.MaNhaCungCap,
                CaNgopAoBanNgoai = model.CaNgopAoBanNgoai,
                Chuyen = model.Chuyen
            };
            _context.PhuongTien_TrongLuongDauAo.Add(newItem);
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
        [HttpGet("{phuongTien}/{ngay}/{maAo}/{maNhaCC}/{chuyen}")]
        [Authorize]
        public IActionResult GetsByMa(string phuongTien, string ngay, string maAo, string maNhaCC, int chuyen)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = Vm.VmPhuongTien_TrongLuongDauAo.Find<PhuongTien_TrongLuongDauAo>(phuongTien, dateTime, maAo, maNhaCC, chuyen);
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

        [HttpGet("{phuongTien}/{ngay}/{maAo}/{maNhaCC}/{chuyen}")]
        [Authorize]
        public IActionResult GetsByMa2(string phuongTien, string ngay, string maAo, string maNhaCC, int chuyen)
        {
            DateTime dateTime = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var item = _context.PhuongTien_TrongLuongDauAo.FirstOrDefault(
                       x => x.MaPhuongTien == phuongTien
                         && x.Ngay == dateTime
                         && x.MaAo == maAo
                         && x.MaNhaCungCap == maNhaCC
                         && x.Chuyen == chuyen
                         );
            
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

        [HttpPost("{phuongTien}/{ngay}/{maAo}/{maNhaCC}/{chuyen}")]
        [Authorize]
        public async Task<IActionResult> Update(string phuongTien, string ngay, string maAo, string maNhaCC, int chuyen, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhuongTien_TrongLuongDauAo>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(chuyen.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(phuongTien))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
             var item = _context.PhuongTien_TrongLuongDauAo.FirstOrDefault(
                       x => x.MaPhuongTien == phuongTien
                         && x.Ngay == ngayConvert
                         && x.MaAo == maAo
                         && x.MaNhaCungCap == maNhaCC
                         && x.Chuyen == chuyen
                         );

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
            //item.MaPhuongTien = model.MaPhuongTien;
            //item.Ngay = model.Ngay;
            //item.MaAo = model.MaAo;
            item.CaManh = model.CaManh;
            item.CaNgopAoGhe = model.CaNgopAoGhe;
            item.CaNgopAoXe = model.CaNgopAoXe;
            item.TongHam = model.TongHam;
            item.CaNgayTruoc = model.CaNgayTruoc;
            item.CaConLai = model.CaConLai;
            item.ThuKy = model.ThuKy;
            item.ApTai = model.ApTai;
            item.STTChuyen = model.STTChuyen;
            item.TyLeMoi = model.TyLeMoi;
            item.GhiChu = model.GhiChu;
            item.NgayXuatPhat = model.NgayXuatPhat;
            item.GioXuatPhat = model.GioXuatPhat;
            item.NgayBatCa = model.NgayBatCa;
            item.ModifiedDateTime = model.ModifiedDateTime;
            item.ModifiedBy = model.ModifiedBy;
            item.IsVungNuoiBlocked = model.IsVungNuoiBlocked;
            //item.MaNhaCungCap = model.MaNhaCungCap;
            item.CaNgopAoBanNgoai = model.CaNgopAoBanNgoai;
            //item.Chuyen = model.Chuyen;
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
    }
}
