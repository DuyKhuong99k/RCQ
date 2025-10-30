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
    public class MapThanhPhamFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MapThanhPhamFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmThanhPhamFilletMap.GetsFullField<object>(_context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MapThanhPhamFillet.ToList();

            return Ok(items);
        }
        [HttpGet("{maLoaiCa}/{maThanhPham}/{maSize}/{isTangCa}")]
        [Authorize]
        public IActionResult GetsByMa(string maLoaiCa, string maThanhPham, string maSize, string isTangCa)
        {

            bool booleanValue = bool.Parse(isTangCa);
            var item = _context.MapThanhPhamFillet.FirstOrDefault(x => x.MaLoaiCaFillet == maLoaiCa && x.MaTPFillet == maThanhPham && x.MaSize == maSize && x.IsTangCa == booleanValue);
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
        public async Task<IActionResult> Insert(MapThanhPhamFillet model)
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
            if (_context.MapThanhPhamFillet.Any(x => x.MaLoaiCaFillet == model.MaLoaiCaFillet && x.MaTPFillet == model.MaTPFillet && x.MaSize == model.MaSize && x.MaBravoFillet == model.MaBravoFillet && x.IsTangCa == model.IsTangCa))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MapThanhPhamFillet
            {

                MaLoaiCaFillet = model.MaLoaiCaFillet,
                MaTPFillet = model.MaTPFillet,
                MaSize = model.MaSize,
                MaBravoFillet = model.MaBravoFillet,
                IsTangCa = model.IsTangCa
            };
            _context.MapThanhPhamFillet.Add(newItem);
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
        [HttpPost("{maLoaiCa}/{maThanhPham}/{maSize}/{isTangCa}")]
        [Authorize]
        public async Task<IActionResult> Update(string maLoaiCa, string maThanhPham, string maSize, string isTangCa, [FromBody] MapThanhPhamFillet model)
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
            bool booleanValue = bool.Parse(isTangCa);
            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var items = await _context.MapThanhPhamFillet.Where(x => x.MaLoaiCaFillet == maLoaiCa && x.MaTPFillet == maThanhPham && x.MaSize == maSize && x.IsTangCa == booleanValue).ToListAsync();
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
            item.MaLoaiCaFillet = model.MaLoaiCaFillet;
            item.MaTPFillet = model.MaTPFillet;
            item.MaSize = model.MaSize;
            item.MaBravoFillet = model.MaBravoFillet;
            item.IsTangCa = model.IsTangCa;
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
        [HttpPost("{maLoaiCa}/{maThanhPham}/{maSize}/{isTangCa}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maLoaiCa, string maThanhPham, string maSize, string isTangCa)
        {
            if (string.IsNullOrEmpty(maLoaiCa))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
          
            bool booleanValue = bool.Parse(isTangCa);
            var item = await _context.MapThanhPhamFillet.Where(x => x.MaLoaiCaFillet == maLoaiCa && x.MaTPFillet == maThanhPham && x.MaSize == maSize && x.IsTangCa == booleanValue).FirstAsync();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            _context.MapThanhPhamFillet.Remove(item);
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
