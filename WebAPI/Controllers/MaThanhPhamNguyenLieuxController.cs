using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaThanhPhamNguyenLieuxController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamNguyenLieuxController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaThanhPhamNguyenLieu.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(string ma)
        {
            var item = _context.MaThanhPhamNguyenLieu.FirstOrDefault(x => x.Ma == ma);
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
        public async Task<IActionResult> Insert(MaThanhPhamNguyenLieu model)
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
            if (_context.MaThanhPhamNguyenLieu.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaThanhPhamNguyenLieu
            {
                MaCa = model.MaCa,
                Ma = model.Ma,
                Ten = model.Ten,
                SuDung = model.SuDung,
                Min = model.Min,
                Max = model.Max,
                IsSNL = model.IsSNL,
                IsNgopGhe = model.IsNgopGhe,
                IsNgopGheMuoi = model.IsNgopGheMuoi,
                IsMuoiGhePhuPham = model.IsMuoiGhePhuPham,
                IsNgopAoMuoi = model.IsNgopAoMuoi,
                IsNgopAoPhuPham = model.IsNgopAoPhuPham,
                IsDatNho = model.IsDatNho,
                IsPhuPhamCaTap = model.IsPhuPhamCaTap,
                IsCaCanTin = model.IsCaCanTin,
                IsCaNgopXeMuoi = model.IsCaNgopXeMuoi,
                IsNgopGhePhuPham = model.IsNgopGhePhuPham,
                IsNgopXePhuPham = model.IsNgopXePhuPham,
                IsManh = model.IsManh,
                TyLeNuoc = model.TyLeNuoc,
                IsCaNgopGheTuoiBanNgoai = model.IsCaNgopGheTuoiBanNgoai,
                IsCaNgopGheAoBanNgoai = model.IsCaNgopGheAoBanNgoai,
                IsTareThung = model.IsTareThung,
                CTTYLE = model.CTTYLE,

            };
            _context.MaThanhPhamNguyenLieu.Add(newItem);
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
        public async Task<IActionResult> Update(string ma, [FromBody] MaThanhPhamNguyenLieu model)
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
           var item = _context.MaThanhPhamNguyenLieu.FirstOrDefault(x => x.Ma == ma);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại."
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
                item.MaCa = model.MaCa;
                item.Ma = model.Ma;
                item.Ten = model.Ten;
                item.SuDung = model.SuDung;
                item.Min = model.Min;
                item.Max = model.Max;
                item.IsSNL = model.IsSNL;
                item.IsNgopGhe = model.IsNgopGhe;
                item.IsNgopGheMuoi = model.IsNgopGheMuoi;
                item.IsMuoiGhePhuPham = model.IsMuoiGhePhuPham;
                item.IsNgopAoMuoi = model.IsNgopAoMuoi;
                item.IsNgopAoPhuPham = model.IsNgopAoPhuPham;
                item.IsDatNho = model.IsDatNho;
                item.IsPhuPhamCaTap = model.IsPhuPhamCaTap;
                item.IsCaCanTin = model.IsCaCanTin;
                item.IsCaNgopXeMuoi = model.IsCaNgopXeMuoi;
                item.IsNgopGhePhuPham = model.IsNgopGhePhuPham;
                item.IsNgopXePhuPham = model.IsNgopXePhuPham;
                item.IsManh = model.IsManh;
                item.TyLeNuoc = model.TyLeNuoc;
                item.IsCaNgopGheTuoiBanNgoai = model.IsCaNgopGheTuoiBanNgoai;
                item.IsCaNgopGheAoBanNgoai = model.IsCaNgopGheAoBanNgoai;
                item.IsTareThung = model.IsTareThung;
                item.CTTYLE = model.CTTYLE;
            try
            {
                var newItemUs = new MaThanhPhamNguyenLieu_U
                {
                    MaThanhPham = item.Ma,
                    MaLoaiCa = item.MaCa,
                    Ten = item.Ten,
                    SuDung = item.SuDung,
                    Min = item.Min,
                    Max = item.Max,
                    IsSNL = item.IsSNL,
                    IsNgopGhe = item.IsNgopGhe,
                    IsNgopGheMuoi = item.IsNgopGheMuoi,
                    IsMuoiGhePhuPham = item.IsMuoiGhePhuPham,
                    IsNgopAoMuoi = item.IsNgopAoMuoi,
                    IsNgopAoPhuPham = item.IsNgopAoPhuPham,
                    IsDatNho = item.IsDatNho,
                    IsPhuPhamCaTap = item.IsPhuPhamCaTap,
                    IsCaCanTin = item.IsCaCanTin,
                    IsCaNgopXeMuoi = item.IsCaNgopXeMuoi,
                    IsNgopGhePhuPham = item.IsNgopGhePhuPham,
                    IsManh = item.IsManh,
                    TyLeNuoc = item.TyLeNuoc,
                    IsCaNgopGheAoBanNgoai = item.IsCaNgopGheAoBanNgoai,
                    IsCaNgopGheTuoiBanNgoai = item.IsCaNgopGheTuoiBanNgoai,
                    MNgay = DateTime.Now,
                    IsNgopXePhuPham = item.IsNgopXePhuPham,
                };

                // Thêm vào bảng HqLoaiNguyenLieuUs
                _context.MaThanhPhamNguyenLieuUs.Add(newItemUs);
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
            var item = _context.MaThanhPhamNguyenLieu.FirstOrDefault(x => x.Ma == ma);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.MaThanhPhamNguyenLieu.Remove(item);

            try
            {
                var newItemUs = new MaThanhPhamNguyenLieu_D
                {
                    MaThanhPham = item.Ma,
                    MaLoaiCa = item.MaCa,
                    Ten = item.Ten,
                    SuDung = item.SuDung,
                    Min = item.Min,
                    Max = item.Max,
                    IsSNL = item.IsSNL,
                    IsNgopGhe = item.IsNgopGhe,
                    IsNgopGheMuoi = item.IsNgopGheMuoi,
                    IsMuoiGhePhuPham = item.IsMuoiGhePhuPham,
                    IsNgopAoMuoi = item.IsNgopAoMuoi,
                    IsNgopAoPhuPham = item.IsNgopAoPhuPham,
                    IsDatNho = item.IsDatNho,
                    IsPhuPhamCaTap = item.IsPhuPhamCaTap,
                    IsCaCanTin = item.IsCaCanTin,
                    IsCaNgopXeMuoi = item.IsCaNgopXeMuoi,
                    IsNgopGhePhuPham = item.IsNgopGhePhuPham,
                    IsManh = item.IsManh,
                    TyLeNuoc = item.TyLeNuoc,
                    IsCaNgopGheAoBanNgoai = item.IsCaNgopGheAoBanNgoai,
                    IsCaNgopGheTuoiBanNgoai = item.IsCaNgopGheTuoiBanNgoai,
                    MNgay = DateTime.Now,
                    IsNgopXePhuPham = item.IsNgopXePhuPham,
                };

                // Thêm vào bảng HqLoaiNguyenLieuUs
                _context.MaThanhPhamNguyenLieuDs.Add(newItemUs);
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
