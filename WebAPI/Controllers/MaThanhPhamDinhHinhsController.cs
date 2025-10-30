using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Humanizer;
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
    public class MaThanhPhamDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet()]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmThanhPhamDinhHinh.GetsFullField<object>(_context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaThanhPhamDinhHinh.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
        [HttpGet("{codeId}")]
        [Authorize]
        public IActionResult GetAllsByCodeId(string codeId)
        {
            var items = _context.MaThanhPhamDinhHinh
                            .Where(x => x.CodeId == codeId)
                            .OrderByDescending(x => x.Ma)
                            .ToList();

            return Ok(items);
        }
        [HttpGet("{ma}/{maCa}")]
        [Authorize]
        public IActionResult GetsByMa(string ma, string maCa)
        {
            var item = _context.MaThanhPhamDinhHinh.FirstOrDefault(x => x.Ma == ma && x.MaCa == maCa);
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
        public async Task<IActionResult> Insert(MaThanhPhamDinhHinh model)
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
            if (_context.MaThanhPhamDinhHinh.Any(u => u.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaThanhPhamDinhHinh
            {
                Ma = model.Ma,
                MaCa = model.MaCa,
                Ten = model.Ten,
                SuDung = model.SuDung,
                DinhMuc = model.DinhMuc,
                Min = model.Min,
                Max = model.Max,
                BravoId = model.BravoId,
                TyLeDinhMucDau = model.TyLeDinhMucDau,
                TyLeDinhMucRot = model.TyLeDinhMucRot,
                TrongLuongTare = model.TrongLuongTare,
                IsDauVaoBatBuoc = model.IsDauVaoBatBuoc,
                DinhMucKhongDauVao = model.DinhMucKhongDauVao,
                IsDisplay = model.IsDisplay,
                CodeId = model.CodeId,
                DinhMucCaTra = model.DinhMucCaTra,
                BaoCaoDauRot = model.BaoCaoDauRot,
                IsSuDungThoiGianGiuaLoaiThanhPham = model.IsSuDungThoiGianGiuaLoaiThanhPham,
                MinOut = model.MinOut,
                MaxOut = model.MaxOut,
                IsCaDa = model.IsCaDa,
                ThoiGianHT = model.ThoiGianHT,
            };
            _context.MaThanhPhamDinhHinh.Add(newItem);
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
        [HttpPost("{ma}/{maCa}")]
        [Authorize]
        public async Task<IActionResult> Update(string ma, string maCa, [FromBody] MaThanhPhamDinhHinh model)
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
            var item = await _context.MaThanhPhamDinhHinh.Where(x => x.Ma == ma && x.MaCa == maCa).FirstAsync();
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
            item.MaCa = model.MaCa;
            item.Ten = model.Ten;
            item.SuDung = model.SuDung;
            item.DinhMuc = model.DinhMuc;
            item.Min = model.Min;
            item.Max = model.Max;
            item.BravoId = model.BravoId;
            item.TyLeDinhMucDau = model.TyLeDinhMucDau;
            item.TyLeDinhMucRot = model.TyLeDinhMucRot;
            item.TrongLuongTare = model.TrongLuongTare;
            item.IsDauVaoBatBuoc = model.IsDauVaoBatBuoc;
            item.DinhMucKhongDauVao = model.DinhMucKhongDauVao;
            item.IsDisplay = model.IsDisplay;
            item.CodeId = model.CodeId;
            item.DinhMucCaTra = model.DinhMucCaTra;
            item.BaoCaoDauRot = model.BaoCaoDauRot;
            item.IsSuDungThoiGianGiuaLoaiThanhPham = model.IsSuDungThoiGianGiuaLoaiThanhPham;
            item.MinOut = model.MinOut;
            item.MaxOut = model.MaxOut;
            item.IsCaDa = model.IsCaDa;
            item.ThoiGianHT = model.ThoiGianHT;
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
        [HttpPost("{ma}/{maCa}")]
        [Authorize]
        public async Task<IActionResult> Delete(string ma, string maCa)
        {
            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(maCa))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await _context.MaThanhPhamDinhHinh.Where(x => x.Ma == ma && x.MaCa == maCa).FirstAsync();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhà cung cấp không tồn tại."
                });
            }

            _context.MaThanhPhamDinhHinh.Remove(item);

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
