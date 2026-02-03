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
    public class HQ_PhieuCanNhapNguyenLieuController : ControllerBase
    { 
        private readonly dbPMScontext _context;

        public HQ_PhieuCanNhapNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetChiTietPhieuCanNhapNguyenLieus(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCanNhapNguyenLieu.GetChiTietPhieuCanNhapNguyenLieus<object>(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetChiTietPhieuCanNhapNguyenLieus(string dateTime, string xuongId)
        {
            DateTime date = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmHQ_PhieuCanNhapNguyenLieu.GetChiTietPhieuCanNhapNguyenLieus<object>(date,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopSanPhamPhieuCanNguyenLieus(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCanNhapNguyenLieu.GetTongHopSanPhamPhieuCanNguyenLieus<object>(from, to, xuongId,_context.Database.GetConnectionString());
            return Ok(items);
        }

        #region xlpc
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCanNhapNguyenLieu>(dataT.Item1);
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
            if (_context.HQ_PhieuCanNhapNguyenLieus.Any(u => u.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new HQ_PhieuCanNhapNguyenLieu
            {
                Id = model.Id,
                STT = model.STT,
                IdPhieuCanNguyenLieu = model.IdPhieuCanNguyenLieu,
                SoPhieuCanNhap = model.SoPhieuCanNhap,
                SoLanSua = model.SoLanSua,
                MaKho = model.MaKho,
                TaiXe = model.TaiXe,
                CCCD = model.CCCD,
                SDT = model.SDT,
                NoiDungGiaoNhan = model.NoiDungGiaoNhan,
                MaPhuongTien = model.MaPhuongTien,
                MaSanPham = model.MaSanPham,
                TrongLuongTong = model.TrongLuongTong,
                TrongLuongXe = model.TrongLuongXe,
                TrongLuongHang = model.TrongLuongHang,
                MaDonVi = model.MaDonVi,
                MaChatLuong = model.MaChatLuong,
                CanHang = model.CanHang,
                TruBi = model.TruBi,
                MaQuyCach = model.MaQuyCach,
                IsPhanLoaiNguyenLieu = model.IsPhanLoaiNguyenLieu
            };
            _context.HQ_PhieuCanNhapNguyenLieus.Add(newItem);
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

            var item = _context.HQ_PhieuCanNhapNguyenLieus.FirstOrDefault(x => x.Id == id);
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
            var model = JsonSerializer.Deserialize<HQ_PhieuCanNhapNguyenLieu>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            var item = await _context.HQ_PhieuCanNhapNguyenLieus.FirstOrDefaultAsync(x => x.Id == id);

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
                item.IdPhieuCanNguyenLieu,
                item.SoPhieuCanNhap,
                item.MaKho,
                item.TaiXe,
                item.CCCD,
                item.SDT,
                item.NoiDungGiaoNhan,
                item.MaPhuongTien,
                item.MaSanPham,
                item.MaDonVi,
                item.MaChatLuong,
                item.CanHang,
                item.TruBi,
                item.MaQuyCach,
                item.IsPhanLoaiNguyenLieu
            }, options);
            item.IdPhieuCanNguyenLieu = model.IdPhieuCanNguyenLieu;
            item.SoPhieuCanNhap = model.SoPhieuCanNhap;
            item.MaKho = model.MaKho;
            item.TaiXe = model.TaiXe;
            item.CCCD = model.CCCD;
            item.SDT = model.SDT;
            item.NoiDungGiaoNhan = model.NoiDungGiaoNhan;
            item.MaPhuongTien = model.MaPhuongTien;
            item.MaSanPham = model.MaSanPham;
            item.MaDonVi = model.MaDonVi;
            item.MaChatLuong = model.MaChatLuong;
            item.CanHang = model.CanHang;
            item.TruBi = model.TruBi;
            item.MaQuyCach = model.MaQuyCach;
            item.IsPhanLoaiNguyenLieu = model.IsPhanLoaiNguyenLieu;
            //item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
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

        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_PhieuCanNhapNguyenLieu>(dataT.Item1);
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            var item = await _context.HQ_PhieuCanNhapNguyenLieus.FirstOrDefaultAsync(x => x.Id == id);

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
                item.TrongLuongHang,
            }, options);

            var newItem = new HQ_PhieuCanNhapNguyenLieu
            {
                Id = model.Id,
                STT = -1,//model.STT,
                IdPhieuCanNguyenLieu = item.IdPhieuCanNguyenLieu,
                SoPhieuCanNhap = item.SoPhieuCanNhap,
                SoLanSua = item.SoLanSua,
                MaKho = item.MaKho,
                TaiXe = item.TaiXe,
                CCCD = item.CCCD,
                SDT = item.SDT,
                NoiDungGiaoNhan = item.NoiDungGiaoNhan,
                MaPhuongTien = item.MaPhuongTien,
                MaSanPham = item.MaSanPham,
                TrongLuongTong = item.TrongLuongTong,
                TrongLuongXe = item.TrongLuongXe,
                TrongLuongHang = item.TrongLuongHang,
                MaDonVi = item.MaDonVi,
                MaChatLuong = item.MaChatLuong,
                CanHang = item.CanHang,
                TruBi = item.TruBi,
                MaQuyCach = item.MaQuyCach,
                IsPhanLoaiNguyenLieu = item.IsPhanLoaiNguyenLieu

                //GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu,
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.HQ_PhieuCanNhapNguyenLieus.Remove(item);
                    _context.HQ_PhieuCanNhapNguyenLieus.Add(newItem);
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



    }
}
