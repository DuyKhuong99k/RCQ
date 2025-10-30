using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class MaThanhPhamSoCheDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamSoCheDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaThanhPhamSoCheDinhHinh.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(string ma)
        {
            var item = _context.MaThanhPhamSoCheDinhHinh.FirstOrDefault(x => x.Ma == ma);
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
        public async Task<IActionResult> Insert(MaThanhPhamSoCheDinhHinh model)
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
            if (_context.MaThanhPhamSoCheDinhHinh.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaThanhPhamSoCheDinhHinh
            {
                Ten = model.Ten,
                Ma = model.Ma,
                SuDung = model.SuDung,
                X = model.X,
                Y = model.Y,
                Min = model.Min,
                Max = model.Max,
                TinhKiem = model.TinhKiem,
                TinhPhucVu = model.TinhPhucVu,
                CaMuoi = model.CaMuoi,
                NguyenLieu = model.NguyenLieu,
                Ban09 = model.Ban09,
                BravoId = model.BravoId,
                LoaiGui = model.LoaiGui,
                TruocLangDa = model.TruocLangDa,
                SauLangDa = model.SauLangDa,
                Nhan = model.Nhan,
                TinhGio = model.TinhGio,
                BatCO = model.BatCO,
                IsNhapTay = model.IsNhapTay,
                Createdby = model.Createdby,
                CreatedDateTime = model.CreatedDateTime,
                Modifiedby = model.Modifiedby,
                ModifiedDateTime = model.ModifiedDateTime
            };
            _context.MaThanhPhamSoCheDinhHinh.Add(newItem);
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
        public async Task<IActionResult> Update(string ma, [FromBody] MaThanhPhamSoCheDinhHinh model)
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
            var item = await _context.MaThanhPhamSoCheDinhHinh.FindAsync(ma);
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
            item.Ma = model.Ma;
            item.Ten = model.Ten;
            item.SuDung = model.SuDung;
            item.X = model.X;
            item.Y = model.Y;
            item.Min = model.Min;
            item.Max = model.Max;
            item.TinhKiem = model.TinhKiem;
            item.TinhPhucVu = model.TinhPhucVu;
            item.CaMuoi = model.CaMuoi;
            item.NguyenLieu = model.NguyenLieu;
            item.Ban09 = model.Ban09;
            item.BravoId = model.BravoId;
            item.LoaiGui = model.LoaiGui;
            item.TruocLangDa = model.TruocLangDa;
            item.SauLangDa = model.SauLangDa;
            item.Nhan = model.Nhan;
            item.TinhGio = model.TinhGio;
            item.BatCO = model.BatCO;
            item.IsNhapTay = model.IsNhapTay;
            item.Createdby = model.Createdby;
            item.CreatedDateTime = model.CreatedDateTime;
            item.Modifiedby = model.Modifiedby;
            item.ModifiedDateTime = model.ModifiedDateTime;
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
            var item = await _context.MaThanhPhamSoCheDinhHinh.FindAsync(ma);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.MaThanhPhamSoCheDinhHinh.Remove(item);

            try
            {
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
