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
    public class ThePhieuSanLuongFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public ThePhieuSanLuongFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/ThePhieuSanLuongFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThePhieuSanLuongFillet>>> Gets()
        {
            if (_context.ThePhieuSanLuongFillet == null)
            {
                return NotFound();
            }
            return Vm.VmThePhieuSanLuongFillet.Items;
        }

        // GET: api/ThePhieuSanLuongFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ThePhieuSanLuongFillet>> Gets(string id)
        {
            if (_context.ThePhieuSanLuongFillet == null)
            {
                return NotFound();
            }
            var thePhieuSanLuongFillet = Vm.VmThePhieuSanLuongFillet.Find(id);

            if (thePhieuSanLuongFillet == null)
            {
                return NotFound();
            }

            return thePhieuSanLuongFillet;
        }

        // PUT: api/ThePhieuSanLuongFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, ThePhieuSanLuongFillet thePhieuSanLuongFillet)
        {
            if (id != thePhieuSanLuongFillet.Id)
            {
                return BadRequest();
            }

            if (!Vm.VmThePhieuSanLuongFillet.Exists(thePhieuSanLuongFillet))
            {
                return NotFound();
            }
            Vm.VmThePhieuSanLuongFillet.Update_Command.Execute(thePhieuSanLuongFillet);
            return NoContent();
        }

        // POST: api/ThePhieuSanLuongFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ThePhieuSanLuongFillet>> Post(ThePhieuSanLuongFillet thePhieuSanLuongFillet)
        {
            if (_context.ThePhieuSanLuongFillet == null)
            {
                return Problem("Entity set 'dbPMScontext.ThePhieuSanLuongFillet'  is null.");
            }
            if (Vm.VmThePhieuSanLuongFillet.Exists(thePhieuSanLuongFillet))
            {
                return Conflict();
            }
            Vm.VmThePhieuSanLuongFillet.Insert_Command.Execute(thePhieuSanLuongFillet);
            return CreatedAtAction("Get", new { id = thePhieuSanLuongFillet.Id }, thePhieuSanLuongFillet);
        }

        //DELETE: api/ThePhieuSanLuongFillets/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (_context.ThePhieuSanLuongFillet == null)
        //    {
        //        return NotFound();
        //    }
        //    var thePhieuSanLuongFillet = Vm.VmThePhieuSanLuongFillet.Find(id);
        //    if (thePhieuSanLuongFillet == null)
        //    {
        //        return NotFound();
        //    }

        //    Vm.VmThePhieuSanLuongFillet.Delete_Command.Execute(thePhieuSanLuongFillet);

        //    return NoContent();
        //}

        private bool ThePhieuSanLuongFilletExists(string id)
        {
            return (_context.ThePhieuSanLuongFillet?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(ThePhieuSanLuongFillet model)
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
            if (_context.ThePhieuSanLuongFillet.Any(u => u.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new ThePhieuSanLuongFillet
            {
                Id = model.Id,
                Ngay = model.Ngay,
                Gio = model.Gio,
                MaLoaiCa = model.MaLoaiCa,
                MaMau = model.MaMau,
                MaSize = model.MaSize,
                MaThanhPham = model.MaThanhPham,
                MaLo = model.MaLo,
                CaTra = model.CaTra,
                MaXuong = model.MaXuong,
                STT_PC = model.STT_PC,
                MayCan_PC = model.MayCan_PC,
                MaThe = model.MaThe,
                TrongLuongTare = model.TrongLuongTare,
                MaBan = model.MaBan,
                MaNhanVien = model.MaNhanVien,
                IsDone = model.IsDone,
                STT_PC_BTP = model.STT_PC_BTP,
                MayCan_PC_BTP = model.MayCan_PC_BTP,
                TrongLuongNhan = model.TrongLuongNhan,
                GhiChu = model.GhiChu,
            };
            _context.ThePhieuSanLuongFillet.Add(newItem);
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
        [HttpPost("{thePhieuSanLuongId}")]
        [Authorize]
        public async Task<IActionResult> Update(string thePhieuSanLuongId, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<ThePhieuSanLuongFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(thePhieuSanLuongId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            var item = await _context.ThePhieuSanLuongFillet.FirstOrDefaultAsync(x => x.Id == thePhieuSanLuongId);

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
                item.MaNhanVien,
                item.MaLo,
                item.MaLoaiCa,
                item.MaThanhPham,
                item.MaSize,
                item.MaMau,
            }, options);
            item.MaNhanVien = model?.MaNhanVien;
            item.MaLo = model?.MaLo;
            item.MaLoaiCa = model?.MaLoaiCa;
            item.MaMau = model?.MaMau;
            item.MaSize = model?.MaSize;
            item.MaThanhPham = model?.MaThanhPham;
            item.GhiChu = item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;

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
        [HttpPost("{thePhieuSanLuongId}")]
        [Authorize]
        public async Task<IActionResult> Delete(string thePhieuSanLuongId)
        {
            if (string.IsNullOrEmpty(thePhieuSanLuongId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await _context.ThePhieuSanLuongFillet.FirstOrDefaultAsync(x => x.Id == thePhieuSanLuongId);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.ThePhieuSanLuongFillet.Remove(item);

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
        [HttpPost("{thePhieuSanLuongId}")]
        [Authorize]
        public async Task<IActionResult> Delete_XLPC(string thePhieuSanLuongId, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<ThePhieuSanLuongFillet>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(thePhieuSanLuongId))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            var item = await _context.ThePhieuSanLuongFillet.FirstOrDefaultAsync(x => x.Id == thePhieuSanLuongId);

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
            item.GhiChu = item.GhiChu = item.GhiChu + "," + model.GhiChu;

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
