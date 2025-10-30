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
    public class PhanBoNhanVienTheoCongViecPhuFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhanBoNhanVienTheoCongViecPhuFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PhanBoNhanVienTheoCongViecPhuFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhanBoNhanVienTheoCongViecPhuFillet>>> Gets()
        {
            if (_context.PhanBoNhanVienTheoCongViecPhuFillet == null)
            {
                return NotFound();
            }
            return await _context.PhanBoNhanVienTheoCongViecPhuFillet.ToListAsync();
        }

        // GET: api/PhanBoNhanVienTheoCongViecPhuFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhanBoNhanVienTheoCongViecPhuFillet>> Get(string id)
        {
            if (_context.PhanBoNhanVienTheoCongViecPhuFillet == null)
            {
                return NotFound();
            }
            var phanBoNhanVienTheoCongViecPhuFillet = await _context.PhanBoNhanVienTheoCongViecPhuFillet.FindAsync(id);

            if (phanBoNhanVienTheoCongViecPhuFillet == null)
            {
                return NotFound();
            }

            return phanBoNhanVienTheoCongViecPhuFillet;
        }

        // PUT: api/PhanBoNhanVienTheoCongViecPhuFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, PhanBoNhanVienTheoCongViecPhuFillet phanBoNhanVienTheoCongViecPhuFillet)
        {
            if (id != phanBoNhanVienTheoCongViecPhuFillet.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(phanBoNhanVienTheoCongViecPhuFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhanBoNhanVienTheoCongViecPhuFilletExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PhanBoNhanVienTheoCongViecPhuFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhanBoNhanVienTheoCongViecPhuFillet>> Post(PhanBoNhanVienTheoCongViecPhuFillet phanBoNhanVienTheoCongViecPhuFillet)
        {
            if (_context.PhanBoNhanVienTheoCongViecPhuFillet == null)
            {
                return Problem("Entity set 'dbPMScontext.PhanBoNhanVienTheoCongViecPhuFillet'  is null.");
            }
            _context.PhanBoNhanVienTheoCongViecPhuFillet.Add(phanBoNhanVienTheoCongViecPhuFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhanBoNhanVienTheoCongViecPhuFilletExists(phanBoNhanVienTheoCongViecPhuFillet.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhanBoNhanVienTheoCongViecPhuFillet", new { id = phanBoNhanVienTheoCongViecPhuFillet.MaNhanVien }, phanBoNhanVienTheoCongViecPhuFillet);
        }

        // DELETE: api/PhanBoNhanVienTheoCongViecPhuFillets/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (_context.PhanBoNhanVienTheoCongViecPhuFillet == null)
        //    {
        //        return NotFound();
        //    }
        //    var phanBoNhanVienTheoCongViecPhuFillet = await _context.PhanBoNhanVienTheoCongViecPhuFillet.FindAsync(id);
        //    if (phanBoNhanVienTheoCongViecPhuFillet == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.PhanBoNhanVienTheoCongViecPhuFillet.Remove(phanBoNhanVienTheoCongViecPhuFillet);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool PhanBoNhanVienTheoCongViecPhuFilletExists(string id)
        {
            return (_context.PhanBoNhanVienTheoCongViecPhuFillet?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(PhanBoNhanVienTheoCongViecPhuFillet model)
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
            if (_context.PhanBoNhanVienTheoCongViecPhuFillet.Any(x => x.MaNhanVien == model.MaNhanVien && x.MaCongViecPhuFillet == model.MaCongViecPhuFillet && x.Ngay == model.Ngay && x.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhanBoNhanVienTheoCongViecPhuFillet
            {

                MaNhanVien = model.MaNhanVien,
                MaCongViecPhuFillet = model.MaCongViecPhuFillet,
                Ngay = model.Ngay,
                MaXuong = model.MaXuong,
                TyLeHuong = model.TyLeHuong,
                TyLeTru = model.TyLeTru
            };
            _context.PhanBoNhanVienTheoCongViecPhuFillet.Add(newItem);
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
        [HttpPost("{maNhanVien}/{maCongViec}/{dateTime}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maNhanVien, string maCongViec, string dateTime, string maXuong)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(dateTime) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }

            var item = await _context.PhanBoNhanVienTheoCongViecPhuFillet.Where(x => x.MaNhanVien == maNhanVien && x.MaCongViecPhuFillet == maCongViec && x.Ngay == date1 && x.MaXuong == maXuong).FirstAsync();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            _context.PhanBoNhanVienTheoCongViecPhuFillet.Remove(item);
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
