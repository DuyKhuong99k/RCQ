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
    public class HQ_NhanVienTheoNhomsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_NhanVienTheoNhomsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = Vm.VmHQ_NhanVienTheoNhom.Gets<HQ_NhanVienTheoNhom>();
            return Ok(items);
        }
        [HttpGet("{dateTime}")]
        [Authorize]
        public IActionResult GetAllListNews(string dateTime)
        {
            DateTime ngayConvert = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmHQ_NhanVienTheoNhom.Gets<object>(ngayConvert);
            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetsByMa(long id)
        {
            var item = Vm.VmHQ_NhanVienTheoNhom.GetByMa(id);
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
        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> Insert(HQ_NhanVienTheoNhom model)
        //{
        //    //kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
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
        //    // ... kiểm tra mã nhân viên đã tồn tại chưa ...
        //    if (_context.HqNhanVienTheoNhoms.Any(u => u.Id == model.Id))
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Mã này đã tồn tại.",
        //        });
        //    }
        //    var newItem = new HQ_NhanVienTheoNhom
        //    {
        //       MaNhanVien = model.MaNhanVien,
        //       MaNhom = model.MaNhom,
        //       NgayGioBatDau = model.NgayGioBatDau,
        //       HeSo = model.HeSo,
        //    };
        //    _context.HqNhanVienTheoNhoms.Add(newItem);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //        //Vm.VmHQ_NhanVienTheoNhom.Insert(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Đã xảy ra lỗi khi lưu dữ liệu." + ex.Message.ToString(),
        //            Errors = new List<string> { ex.Message.ToString() }
        //        });
        //    }
        //    return Ok(new ApiResponse
        //    {
        //        Success = true,
        //        Message = "Thêm thành công!"
        //    });
        //}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(HQ_NhanVienTheoNhom model)
        {
            // 1️⃣ Kiểm tra dữ liệu đầu vào
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

            // Làm tròn thời gian đến phút
            var ngayGioPhut = new DateTime(
                model.NgayGioBatDau.Year,
                model.NgayGioBatDau.Month,
                model.NgayGioBatDau.Day,
                model.NgayGioBatDau.Hour,
                model.NgayGioBatDau.Minute,
                0
            );

            // Tìm bản ghi trùng theo MaNhanVien + thời điểm
            var duplicateRecord = await (
                from nv in _context.HqNhanVienTheoNhoms
                join nhom in _context.HqNhoms on nv.MaNhom equals nhom.Id into gj
                from nhom in gj.DefaultIfEmpty()
                where nv.MaNhanVien == model.MaNhanVien
                   && nv.NgayGioBatDau.Year == ngayGioPhut.Year
                   && nv.NgayGioBatDau.Month == ngayGioPhut.Month
                   && nv.NgayGioBatDau.Day == ngayGioPhut.Day
                   && nv.NgayGioBatDau.Hour == ngayGioPhut.Hour
                   && nv.NgayGioBatDau.Minute == ngayGioPhut.Minute
                select new
                {
                    nv.MaNhom,
                    TenNhom = nhom != null ? nhom.Ten : null,
                    nv.NgayGioBatDau
                }
            ).FirstOrDefaultAsync();

            if (duplicateRecord != null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = $"Nhân viên này tồn tại trong nhóm: {duplicateRecord.MaNhom}" +(string.IsNullOrEmpty(duplicateRecord.TenNhom) ? "" : $" ({duplicateRecord.TenNhom})") +$" lúc {duplicateRecord.NgayGioBatDau:HH:mm dd/MM/yyyy}"
                    
                });
            }

            var newItem = new HQ_NhanVienTheoNhom
            {
                MaNhanVien = model.MaNhanVien,
                MaNhom = model.MaNhom,
                NgayGioBatDau = model.NgayGioBatDau,
                HeSo = model.HeSo,
            };

            _context.HqNhanVienTheoNhoms.Add(newItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message,
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm thành công!"
            });
        }
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(long id, [FromBody] HQ_NhanVienTheoNhom model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }


            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            //var item = await _context.HqNhanVienTheoNhoms.FindAsync(id);
            var item = Vm.VmHQ_NhanVienTheoNhom.GetByMa(id);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có
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

            //// Cập nhật thông tin vào item
            //item.Id = model.Id;
            //item.NgayNguyenLieu = model.NgayNguyenLieu;
            //item.SuDung = model.SuDung;
            ////item.MNgay = model.MNgay;

            try
            {
                // Lưu thay đổi cập nhật vào bảng HqLoaiNguyenLieus
                //await _context.SaveChangesAsync();

                //// Tạo bản ghi mới cho bảng HqLoaiNguyenLieuUs
                //var newItemUs = new HQ_Lo_U
                //{
                //    LoId = item.Id,
                //    NgayNguyenLieu = item.NgayNguyenLieu,
                //    SuDung = item.SuDung,
                //    MNgay = item.MNgay,
                //    Ngay = DateTime.Now // Ngày hiện tại khi tạo mới
                //};

                //// Thêm vào bảng HqLoaiNguyenLieuUs
                //_context.HqLoUs.Add(newItemUs);

                //// Lưu thay đổi vào cơ sở dữ liệu
                //await _context.SaveChangesAsync();
                Vm.VmHQ_NhanVienTheoNhom.Update(model);
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
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await _context.HqNhanVienTheoNhoms.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhóm không tồn tại."
                });
            }

            _context.HqNhanVienTheoNhoms.Remove(item);

            try
            {
                await _context.SaveChangesAsync();
                //Vm.VmHQ_NhanVienTheoNhom.Delete(id);
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
