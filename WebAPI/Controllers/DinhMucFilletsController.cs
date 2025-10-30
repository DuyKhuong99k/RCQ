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
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DinhMucFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public DinhMucFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmDinhMucFillet.GetsFullField<object>(_context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.DinhMucFillet.OrderByDescending(x => x.STT).ToList();

            return Ok(items);
        }
        [HttpGet("{stt}/{ngay}/{gio}/{maLo}/{maLoaiCa}/{maMau}/{maSize}/{maThanhPham}/{maXuong}/{caTra}")]
        [Authorize]
        public IActionResult GetsByMa(string stt, string ngay, string gio, string maLo, string maLoaiCa, string maMau, string maSize, string maThanhPham, string maXuong, string caTra)
        {
            int number = int.Parse(stt);
            DateTime dateTime = DateTime.Parse(ngay);
            TimeSpan timeSpan = TimeSpan.Parse(gio);
            bool booleanValue = bool.Parse(caTra);
            var item = _context.DinhMucFillet.Where(x => x.STT == number && x.Ngay == dateTime && x.Gio == timeSpan && x.MaLo == maLo && x.MaLoaiCa == maLoaiCa && x.MaMau == maMau && x.MaSize == maSize && x.MaThanhPham == maThanhPham && x.MaXuong == maXuong && x.CaTra == booleanValue).First();
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
        public async Task<IActionResult> Insert(DinhMucFillet model)
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
            if (_context.DinhMucFillet.Any(x => x.STT == model.STT && x.Ngay == model.Ngay && x.Gio == model.Gio && x.MaLo == model.MaLo && x.MaLoaiCa == model.MaLoaiCa && x.MaMau == model.MaMau && x.MaSize == model.MaSize && x.MaThanhPham == model.MaThanhPham && x.MaXuong == model.MaXuong && x.CaTra == model.CaTra))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new DinhMucFillet
            {
                STT = model.STT,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaLo = model.MaLo,
                MaLoaiCa = model.MaLoaiCa,
                MaMau = model.MaMau,
                MaSize = model.MaSize,
                MaThanhPham = model.MaThanhPham,
                MaXuong = model.MaXuong,
                CaTra = model.CaTra,
                DinhMuc = model.DinhMuc,
                SuDung = model.SuDung
            };
            _context.DinhMucFillet.Add(newItem);
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
        [HttpPost("{stt}/{ngay}/{gio}/{maLo}/{maLoaiCa}/{maMau}/{maSize}/{maThanhPham}/{maXuong}/{caTra}")]
        [Authorize]
        public async Task<IActionResult> Update(string stt, string ngay, string gio, string maLo, string maLoaiCa, string maMau, string maSize, string maThanhPham, string maXuong, string caTra, [FromBody] DinhMucFillet model)
        {
            //// Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            //if (string.IsNullOrEmpty(stt))
            //{
            //    return BadRequest(new ApiResponse
            //    {
            //        Success = false,
            //        Message = "Mã này không hợp lệ."
            //    });
            //}
            int number = int.Parse(stt);
            DateTime dateTime = DateTime.Parse(ngay);
            TimeSpan timeSpan = TimeSpan.Parse(gio);
            bool booleanValue = bool.Parse(caTra);
            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var items = await _context.DinhMucFillet.Where(x => x.STT == number && x.Ngay == dateTime && x.Gio == timeSpan && x.MaLo == maLo && x.MaLoaiCa == maLoaiCa && x.MaMau == maMau && x.MaSize == maSize && x.MaThanhPham == maThanhPham && x.MaXuong == maXuong && x.CaTra == booleanValue).ToListAsync();
            var item = items.FirstOrDefault();
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
            item.STT = model.STT;
            item.Ngay = model.Ngay;
            item.Gio = model.Gio;
            item.MaLo = model.MaLo;
            item.MaLoaiCa = model.MaLoaiCa;
            item.MaMau = model.MaMau;
            item.MaSize = model.MaSize;
            item.MaThanhPham = model.MaThanhPham;
            item.MaXuong = model.MaXuong;
            item.CaTra = model.CaTra;
            item.DinhMuc = model.DinhMuc;
            item.SuDung = model.SuDung;
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
        [HttpPost("{stt}/{ngay}/{gio}/{maLo}/{maLoaiCa}/{maMau}/{maSize}/{maThanhPham}/{maXuong}/{caTra}")]
        [Authorize]
        public async Task<IActionResult> Delete(string stt, string ngay, string gio, string maLo, string maLoaiCa, string maMau, string maSize, string maThanhPham, string maXuong, string caTra)
        {
            if (string.IsNullOrEmpty(maLo))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            int number = int.Parse(stt);
            DateTime dateTime = DateTime.Parse(ngay);
            TimeSpan timeSpan = TimeSpan.Parse(gio);
            bool booleanValue = bool.Parse(caTra);
            var item = await _context.DinhMucFillet.Where(x => x.STT == number && x.Ngay == dateTime && x.Gio == timeSpan && x.MaLo == maLo && x.MaLoaiCa == maLoaiCa && x.MaMau == maMau && x.MaSize == maSize && x.MaThanhPham == maThanhPham && x.MaXuong == maXuong && x.CaTra == booleanValue).FirstAsync();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            _context.DinhMucFillet.Remove(item);
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
