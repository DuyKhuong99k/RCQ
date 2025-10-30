using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppViewModels;
using ViewModels.Repos.HQ;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoTriTinhLuonXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BoTriTinhLuonXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/BoTriTinhLuonXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoTriTinhLuonXepKhuon>>> GetBoTriTinhLuonXepKhuon()
        {
            if (_context.BoTriTinhLuonXepKhuon == null)
            {
                return NotFound();
            }
            return await _context.BoTriTinhLuonXepKhuon.ToListAsync();
        }

        // GET: api/BoTriTinhLuonXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoTriTinhLuonXepKhuon>> GetBoTriTinhLuonXepKhuon(string id)
        {
            if (_context.BoTriTinhLuonXepKhuon == null)
            {
                return NotFound();
            }
            var boTriTinhLuonXepKhuon = await _context.BoTriTinhLuonXepKhuon.FindAsync(id);

            if (boTriTinhLuonXepKhuon == null)
            {
                return NotFound();
            }

            return boTriTinhLuonXepKhuon;
        }

        // PUT: api/BoTriTinhLuonXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoTriTinhLuonXepKhuon(string id, BoTriTinhLuonXepKhuon boTriTinhLuonXepKhuon)
        {
            if (id != boTriTinhLuonXepKhuon.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(boTriTinhLuonXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoTriTinhLuonXepKhuonExists(id))
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

        // POST: api/BoTriTinhLuonXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BoTriTinhLuonXepKhuon>> PostBoTriTinhLuonXepKhuon(BoTriTinhLuonXepKhuon boTriTinhLuonXepKhuon)
        {
            if (_context.BoTriTinhLuonXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.BoTriTinhLuonXepKhuon'  is null.");
            }
            _context.BoTriTinhLuonXepKhuon.Add(boTriTinhLuonXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BoTriTinhLuonXepKhuonExists(boTriTinhLuonXepKhuon.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBoTriTinhLuonXepKhuon", new { id = boTriTinhLuonXepKhuon.MaNhanVien }, boTriTinhLuonXepKhuon);
        }

        // DELETE: api/BoTriTinhLuonXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoTriTinhLuonXepKhuon(string id)
        {
            if (_context.BoTriTinhLuonXepKhuon == null)
            {
                return NotFound();
            }
            var boTriTinhLuonXepKhuon = await _context.BoTriTinhLuonXepKhuon.FindAsync(id);
            if (boTriTinhLuonXepKhuon == null)
            {
                return NotFound();
            }

            _context.BoTriTinhLuonXepKhuon.Remove(boTriTinhLuonXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoTriTinhLuonXepKhuonExists(string id)
        {
            return (_context.BoTriTinhLuonXepKhuon?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }
        #region Tính lương cài đặt nhân viên theo công việc xếp khuôn
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.BoTriTinhLuonXepKhuon.ToList();

            return Ok(items);
        }
        [HttpGet("{maCongViec}/{dateTime}/{maXuong}")]
        [Authorize]
        public IActionResult GetAllByMaCongViecs(string maCongViec, string dateTime, string maXuong)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var items = _context.BoTriTinhLuonXepKhuon
                        .Where(x => x.MaCongViec == maCongViec && x.Ngay == date1 && x.MaXuong == maXuong)
                        .ToList();

            return Ok(items);
        }
        [HttpGet("{maNhanVien}/{maCongViec}/{dateTime}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(string maNhanVien, string maCongViec, string dateTime, string maXuong)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var item = _context.BoTriTinhLuonXepKhuon.FirstOrDefault(x => x.MaNhanVien == maNhanVien && x.MaCongViec == maCongViec && x.Ngay == date1 && x.MaXuong == maXuong);
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
        public async Task<IActionResult> Insert(BoTriTinhLuonXepKhuon model)
        {
            //var model = JsonSerializer.Deserialize<MaNhanVienTheoNhomXepKhuon>(dataT.Item1);
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
            if (_context.BoTriTinhLuonXepKhuon.Any(x => x.MaNhanVien == model.MaNhanVien && x.MaCongViec == model.MaCongViec && x.Ngay == model.Ngay && x.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new BoTriTinhLuonXepKhuon
            {
                MaNhanVien = model.MaNhanVien,
                Ngay = model.Ngay,
                MaCongViec = model.MaCongViec,
                MaXuong = model.MaXuong,
                IsNhom = model.IsNhom,
                TyLeHuong = model.TyLeHuong,
                TyLeTru = model.TyLeTru,
                SoGio = model.SoGio,
            };
            _context.BoTriTinhLuonXepKhuon.Add(newItem);
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
        public async Task<IActionResult> Update(string maNhanVien, string maCongViec, string dateTime, string maXuong, Tuple<string> dataT)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var model = JsonSerializer.Deserialize<BoTriTinhLuonXepKhuon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(dateTime) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await _context.BoTriTinhLuonXepKhuon.FirstOrDefaultAsync(x => x.MaNhanVien == maNhanVien && x.MaCongViec == maCongViec && x.Ngay == date1 && x.MaXuong == maXuong);
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

            item.MaXuong = model.MaXuong;
            item.IsNhom = model.IsNhom;
            item.TyLeHuong = model.TyLeHuong;
            item.TyLeTru = model.TyLeTru;
            item.SoGio = model.SoGio;

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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] List<BoTriTinhLuonXepKhuon> maList)
        {
            if (maList == null || !maList.Any())
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!"
                });
            }

            var itemsToDelete = await _context.BoTriTinhLuonXepKhuon
                .Where(x => maList.Any(m => m.MaNhanVien == x.MaNhanVien && m.MaCongViec == x.MaCongViec && m.Ngay == x.Ngay && m.MaXuong == x.MaXuong))
                .ToListAsync();

            if (!itemsToDelete.Any())
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Không có mục nào tồn tại."
                });
            }

            _context.BoTriTinhLuonXepKhuon.RemoveRange(itemsToDelete);

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
                Message = "Đã xoá các mục!"
            });
        }
        #endregion
    }
}
