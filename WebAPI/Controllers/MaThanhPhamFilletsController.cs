using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaThanhPhamFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaThanhPhamFillet.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
        [HttpGet("{codeId}")]
        [Authorize]
        public IActionResult GetAllsByCodeId(string codeId)
        {
            var items = _context.MaThanhPhamFillet
                            .Where(x => x.CodeId == codeId)
                            .OrderByDescending(x => x.Ma)
                            .ToList();

            return Ok(items);
        }
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(string ma)
        {
            var item = _context.MaThanhPhamFillet.FirstOrDefault(x => x.Ma == ma);
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
        public async Task<IActionResult> Insert(MaThanhPhamFillet model)
        {
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
            if (_context.MaThanhPhamFillet.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaThanhPhamFillet
            {
                Ma = model.Ma,
                MaCa = model.MaCa,
                Ten = model.Ten,
                SuDung = model.SuDung,
                Min = model.Min,
                Max = model.Max,
                IsSoChe = model.IsSoChe,
                IsCaMuoi = model.IsCaMuoi,
                DinhMuc = model.DinhMuc,
                BravoId = model.BravoId,
                TrongLuong = model.TrongLuong,
                TrangThaiThanhPham = model.TrangThaiThanhPham,
                TrongLuongHienTai = model.TrongLuongHienTai,
                ThoiGianTren1kgSeconds = model.ThoiGianTren1kgSeconds,
                DinhMucHaoHut = model.DinhMucHaoHut,
                CodeId = model.CodeId, //mã xưởng
                IsNotSetByTime = model.IsNotSetByTime,
                ColorRGB = model.ColorRGB,
                KhongPhanBietSize = model.KhongPhanBietSize,
                IsXeBuom = model.IsXeBuom,
                IsChuyenFillet = model.IsChuyenFillet,
                IsDat = model.IsDat,
                IsNguyenLieuXeBuom = model.IsNguyenLieuXeBuom,
                IsGiaoXepKhuon = model.IsGiaoXepKhuon,
                IsNguyenCon = model.IsNguyenCon,
                IsNguyenConNXB = model.IsNguyenConNXB,
                ThoiGianHT = model.ThoiGianHT
            };
            _context.MaThanhPhamFillet.Add(newItem);
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
        [HttpPost("{ma}")]
        [Authorize]
        public async Task<IActionResult> Update(string ma, [FromBody] MaThanhPhamFillet model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = _context.MaThanhPhamFillet.FirstOrDefault(x => x.Ma == ma);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
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
            item.Ma = model.Ma;
            item.MaCa = model.MaCa;
            item.Ten = model.Ten;
            item.SuDung = model.SuDung;
            item.Min = model.Min;
            item.Max = model.Max;
            item.IsSoChe = model.IsSoChe;
            item.IsCaMuoi = model.IsCaMuoi;
            item.DinhMuc = model.DinhMuc;
            item.BravoId = model.BravoId;
            item.TrongLuong = model.TrongLuong;
            item.TrangThaiThanhPham = model.TrangThaiThanhPham;
            item.TrongLuongHienTai = model.TrongLuongHienTai;
            item.ThoiGianTren1kgSeconds = model.ThoiGianTren1kgSeconds;
            item.DinhMucHaoHut = model.DinhMucHaoHut;
            item.CodeId = model.CodeId; //mã xưởng
            item.IsNotSetByTime = model.IsNotSetByTime;
            item.ColorRGB = model.ColorRGB;
            item.KhongPhanBietSize = model.KhongPhanBietSize;
            item.IsXeBuom = model.IsXeBuom;
            item.IsChuyenFillet = model.IsChuyenFillet;
            item.IsDat = model.IsDat;
            item.IsNguyenLieuXeBuom = model.IsNguyenLieuXeBuom;
            item.IsGiaoXepKhuon = model.IsGiaoXepKhuon;
            item.IsNguyenCon = model.IsNguyenCon;
            item.IsNguyenConNXB = model.IsNguyenConNXB;
            item.ThoiGianHT = model.ThoiGianHT;
            try
            {
                var newItemUs = new MaThanhPhamFillet_U
                {
                    MaThanhPham = item.Ma,
                    MaLoaiCa = item.MaCa,
                    Ten = item.Ten,
                    SuDung = item.SuDung,
                    Min = item.Min,
                    Max = item.Max,
                    IsSoChe = item.IsSoChe,
                    IsCaMuoi = item.IsCaMuoi,
                    DinhMuc = item.DinhMuc,
                    BravoId = item.BravoId,
                    TrongLuong = item.TrongLuong,
                    TrangThaiThanhPham = item.TrangThaiThanhPham,
                    TrongLuongHienTai = item.TrongLuongHienTai,
                    ThoiGianTren1kgSeconds = item.ThoiGianTren1kgSeconds,
                    DinhMucHaoHut = item.DinhMucHaoHut,
                    CodeId = item.CodeId,
                    ColorRGB = item.ColorRGB,
                    IsNotSetByTime = item.IsNotSetByTime,
                    KhongPhanBietSize = item.KhongPhanBietSize,
                    MNgay = DateTime.Now,
                    IsXeBuom = item.IsXeBuom,
                };

                // Thêm vào bảng HqLoaiNguyenLieuUs
                _context.MaThanhPhamFilletUs.Add(newItemUs);
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
        [HttpPost("{ma}")]
        [Authorize]
        public async Task<IActionResult> Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = _context.MaThanhPhamFillet.FirstOrDefault(x => x.Ma == ma);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.MaThanhPhamFillet.Remove(item);

            try
            {
                var newItemUs = new MaThanhPhamFillet_D
                {
                    MaThanhPham = item.Ma,
                    MaLoaiCa = item.MaCa,
                    Ten = item.Ten,
                    SuDung = item.SuDung,
                    Min = item.Min,
                    Max = item.Max,
                    IsSoChe = item.IsSoChe,
                    IsCaMuoi = item.IsCaMuoi,
                    DinhMuc = item.DinhMuc,
                    BravoId = item.BravoId,
                    TrongLuong = item.TrongLuong,
                    TrangThaiThanhPham = item.TrangThaiThanhPham,
                    TrongLuongHienTai = item.TrongLuongHienTai,
                    ThoiGianTren1kgSeconds = item.ThoiGianTren1kgSeconds,
                    DinhMucHaoHut = item.DinhMucHaoHut,
                    CodeId = item.CodeId,
                    ColorRGB = item.ColorRGB,
                    IsNotSetByTime = item.IsNotSetByTime,
                    KhongPhanBietSize = item.KhongPhanBietSize,
                    MNgay = DateTime.Now,
                    IsXeBuom = item.IsXeBuom,
                };

                // Thêm vào bảng HqLoaiNguyenLieuUs
                _context.MaThanhPhamFilletDs.Add(newItemUs);
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

    }
}
