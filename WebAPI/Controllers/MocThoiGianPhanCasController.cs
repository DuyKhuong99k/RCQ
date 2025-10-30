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
    public class MocThoiGianPhanCasController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MocThoiGianPhanCasController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/MocThoiGianPhanCas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MocThoiGianPhanCa>>> Gets()
        {
            if (_context.MocThoiGianPhanCa == null)
            {
                return NotFound();
            }
            return await _context.MocThoiGianPhanCa.ToListAsync();
        }

        // GET: api/MocThoiGianPhanCas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MocThoiGianPhanCa>> Get(DateTime id)
        {
            if (_context.MocThoiGianPhanCa == null)
            {
                return NotFound();
            }
            var mocThoiGianPhanCa = await _context.MocThoiGianPhanCa.FindAsync(id);

            if (mocThoiGianPhanCa == null)
            {
                return NotFound();
            }

            return mocThoiGianPhanCa;
        }

        // PUT: api/MocThoiGianPhanCas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(DateTime id, MocThoiGianPhanCa mocThoiGianPhanCa)
        {
            if (id != mocThoiGianPhanCa.Ngay)
            {
                return BadRequest();
            }

            _context.Entry(mocThoiGianPhanCa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MocThoiGianPhanCaExists(id))
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

        // POST: api/MocThoiGianPhanCas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MocThoiGianPhanCa>> Post(MocThoiGianPhanCa mocThoiGianPhanCa)
        {
            if (_context.MocThoiGianPhanCa == null)
            {
                return Problem("Entity set 'dbPMScontext.MocThoiGianPhanCa'  is null.");
            }
            _context.MocThoiGianPhanCa.Add(mocThoiGianPhanCa);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MocThoiGianPhanCaExists(mocThoiGianPhanCa.Ngay))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = mocThoiGianPhanCa.Ngay }, mocThoiGianPhanCa);
        }

        // DELETE: api/MocThoiGianPhanCas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(DateTime id)
        {
            if (_context.MocThoiGianPhanCa == null)
            {
                return NotFound();
            }
            var mocThoiGianPhanCa = await _context.MocThoiGianPhanCa.FindAsync(id);
            if (mocThoiGianPhanCa == null)
            {
                return NotFound();
            }

            _context.MocThoiGianPhanCa.Remove(mocThoiGianPhanCa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MocThoiGianPhanCaExists(DateTime id)
        {
            return (_context.MocThoiGianPhanCa?.Any(e => e.Ngay == id)).GetValueOrDefault();
        }

        [HttpGet("{dateTime}/{maXuong}")]
        [Authorize]
        public IActionResult GetAllsByNgayMaXuong(string dateTime, string maXuong)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var items = _context.MocThoiGianPhanCa.Where(m => m.Ngay == date1 && m.MaXuong == maXuong).ToList();

            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MocThoiGianPhanCa.ToList();

            return Ok(items);
        }

        //[HttpGet("{dateTime}/{maXuong}")]
        //[Authorize]
        //public IActionResult GetsByMa(string dateTime, string gio, string maXuong)
        //{
        //    DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    var item = _context.MocThoiGianPhanCa.FirstOrDefault(x => x.Ngay == date1 && x.MaXuong == maXuong);
        //    if (item == null)
        //    {
        //        return NotFound(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Mã này không tồn tại!."
        //        });
        //    }
        //    return Ok(item);
        //}
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<MocThoiGianPhanCa>(dataT.Item1);
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
            //// ... kiểm tra mã nhân viên đã tồn tại chưa ...
            if (_context.MocThoiGianPhanCa.Any(x => x.Ngay == model.Ngay && x.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MocThoiGianPhanCa
            {
                Ngay = model.Ngay,
                Gio = model.Gio,
                CreateDateTime = model.CreateDateTime,
                CreateBy = model.CreateBy,
                ModifyDateTime = model.ModifyDateTime,
                ModifyBy = model.ModifyBy,
                MaXuong = model.MaXuong,
            };
            _context.MocThoiGianPhanCa.Add(newItem);
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
        [HttpPost("{gio}")]
        [Authorize]
        public async Task<IActionResult> Update(string gio, Tuple<string> dataT)
        {
            TimeSpan timeSpan = TimeSpan.Parse(gio);
            var model = JsonSerializer.Deserialize<MocThoiGianPhanCa>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(gio))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await _context.MocThoiGianPhanCa.FirstOrDefaultAsync(x => x.Ngay == model.Ngay && x.Gio == model.Gio && x.MaXuong == model.MaXuong);
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
            item.Gio = timeSpan;
            item.ModifyDateTime = model.ModifyDateTime;
            item.ModifyBy = model.ModifyBy;

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

            var item = await _context.MaNhomXepKhuon.FirstOrDefaultAsync(x => x.Ma == ma);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.MaNhomXepKhuon.Remove(item);

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
