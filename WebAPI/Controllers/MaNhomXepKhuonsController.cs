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
    public class MaNhomXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaNhomXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaNhomXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaNhomXepKhuon>>> Gets()
        {
            if (_context.MaNhomXepKhuon == null)
            {
                return NotFound();
            }
            return await _context.MaNhomXepKhuon.ToListAsync();
        }

        // GET: api/MaNhomXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaNhomXepKhuon>> Get(string id)
        {
            if (_context.MaNhomXepKhuon == null)
            {
                return NotFound();
            }
            var maNhomXepKhuon = await _context.MaNhomXepKhuon.FindAsync(id);

            if (maNhomXepKhuon == null)
            {
                return NotFound();
            }

            return maNhomXepKhuon;
        }

        // PUT: api/MaNhomXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaNhomXepKhuon maNhomXepKhuon)
        {
            if (id != maNhomXepKhuon.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maNhomXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaNhomXepKhuonExists(id))
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

        // POST: api/MaNhomXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaNhomXepKhuon>> Pos(MaNhomXepKhuon maNhomXepKhuon)
        {
            if (_context.MaNhomXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.MaNhomXepKhuon'  is null.");
            }
            _context.MaNhomXepKhuon.Add(maNhomXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaNhomXepKhuonExists(maNhomXepKhuon.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maNhomXepKhuon.Ma }, maNhomXepKhuon);
        }

        //// DELETE: api/MaNhomXepKhuons/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (_context.MaNhomXepKhuon == null)
        //    {
        //        return NotFound();
        //    }
        //    var maNhomXepKhuon = await _context.MaNhomXepKhuon.FindAsync(id);
        //    if (maNhomXepKhuon == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.MaNhomXepKhuon.Remove(maNhomXepKhuon);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool MaNhomXepKhuonExists(string id)
        {
            return (_context.MaNhomXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }

        [HttpGet("{maXuong}")]
        [Authorize]
        public IActionResult GetAllsByMaXuong(string maXuong)
        {
            var items = _context.MaNhomXepKhuon.Where(m => m.MaXuong == maXuong).ToList();

            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaNhomXepKhuon.ToList();

            return Ok(items);
        }

        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(string ma)
        {

            var item = _context.MaNhomXepKhuon.FirstOrDefault(x => x.Ma == ma);
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
        public async Task<IActionResult> Insert(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<MaNhomXepKhuon>(dataT.Item1);
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
            if (_context.MaNhomXepKhuon.Any(x => x.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaNhomXepKhuon
            {
                Ma = model.Ma,
                Ten = model.Ten,
                MaXuong = model.MaXuong,
                GhiChu = model.GhiChu
            };
            _context.MaNhomXepKhuon.Add(newItem);
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
        public async Task<IActionResult> Update(string ma, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<MaNhomXepKhuon>(dataT.Item1);
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
            var item = await _context.MaNhomXepKhuon.FindAsync(ma);
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
            item.Ten = model.Ten;
            item.GhiChu = model.GhiChu;

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
