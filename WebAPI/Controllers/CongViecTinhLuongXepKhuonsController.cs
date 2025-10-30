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
    public class CongViecTinhLuongXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public CongViecTinhLuongXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/CongViecTinhLuongXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CongViecTinhLuongXepKhuon>>> GetCongViecTinhLuongXepKhuon()
        {
            if (_context.CongViecTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            return await _context.CongViecTinhLuongXepKhuon.ToListAsync();
        }

        // GET: api/CongViecTinhLuongXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CongViecTinhLuongXepKhuon>> GetCongViecTinhLuongXepKhuon(string id)
        {
            if (_context.CongViecTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            var congViecTinhLuongXepKhuon = await _context.CongViecTinhLuongXepKhuon.FindAsync(id);

            if (congViecTinhLuongXepKhuon == null)
            {
                return NotFound();
            }

            return congViecTinhLuongXepKhuon;
        }

        // PUT: api/CongViecTinhLuongXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCongViecTinhLuongXepKhuon(string id, CongViecTinhLuongXepKhuon congViecTinhLuongXepKhuon)
        {
            if (id != congViecTinhLuongXepKhuon.Ma)
            {
                return BadRequest();
            }

            _context.Entry(congViecTinhLuongXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CongViecTinhLuongXepKhuonExists(id))
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

        // POST: api/CongViecTinhLuongXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CongViecTinhLuongXepKhuon>> PostCongViecTinhLuongXepKhuon(CongViecTinhLuongXepKhuon congViecTinhLuongXepKhuon)
        {
            if (_context.CongViecTinhLuongXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.CongViecTinhLuongXepKhuon'  is null.");
            }
            _context.CongViecTinhLuongXepKhuon.Add(congViecTinhLuongXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CongViecTinhLuongXepKhuonExists(congViecTinhLuongXepKhuon.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCongViecTinhLuongXepKhuon", new { id = congViecTinhLuongXepKhuon.Ma }, congViecTinhLuongXepKhuon);
        }

        // DELETE: api/CongViecTinhLuongXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCongViecTinhLuongXepKhuon(string id)
        {
            if (_context.CongViecTinhLuongXepKhuon == null)
            {
                return NotFound();
            }
            var congViecTinhLuongXepKhuon = await _context.CongViecTinhLuongXepKhuon.FindAsync(id);
            if (congViecTinhLuongXepKhuon == null)
            {
                return NotFound();
            }

            _context.CongViecTinhLuongXepKhuon.Remove(congViecTinhLuongXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CongViecTinhLuongXepKhuonExists(string id)
        {
            return (_context.CongViecTinhLuongXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.CongViecTinhLuongXepKhuon.ToList();

            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetLoaiDuLieus()
        {
            var items = Vm.VmLoaiDuLieu.LoaiDuLieus;
            return Ok(items);
        }
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(string ma)
        {

            var item = _context.CongViecTinhLuongXepKhuon.FirstOrDefault(x => x.Ma == ma);
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
            var model = JsonSerializer.Deserialize<CongViecTinhLuongXepKhuon>(dataT.Item1);
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
            if (_context.CongViecTinhLuongXepKhuon.Any(x => x.Ma == model.Ma))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new CongViecTinhLuongXepKhuon
            {
                Ma = model.Ma,
                Ten = model.Ten,
                BravoId = model.BravoId,
                BravoIdDem = model.BravoIdDem,
                SuDung = model.SuDung,
                LoaiDuLieu = model.LoaiDuLieu,
            };
            _context.CongViecTinhLuongXepKhuon.Add(newItem);
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
            var model = JsonSerializer.Deserialize<CongViecTinhLuongXepKhuon>(dataT.Item1);
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
            var item = await _context.CongViecTinhLuongXepKhuon.FindAsync(ma);
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
            item.BravoId = model.BravoId;
            item.BravoIdDem = model.BravoIdDem;
            item.SuDung = model.SuDung;
            item.LoaiDuLieu = model.LoaiDuLieu;
           
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

            var item = await _context.CongViecTinhLuongXepKhuon.FirstOrDefaultAsync(x => x.Ma == ma);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.CongViecTinhLuongXepKhuon.Remove(item);

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
