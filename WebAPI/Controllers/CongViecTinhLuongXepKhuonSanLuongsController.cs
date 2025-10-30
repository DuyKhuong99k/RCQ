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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CongViecTinhLuongXepKhuonSanLuongsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public CongViecTinhLuongXepKhuonSanLuongsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/CongViecTinhLuongXepKhuonSanLuongs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CongViecTinhLuongXepKhuonSanLuong>>> GetCongViecTinhLuongXepKhuonSanLuong()
        {
            if (_context.CongViecTinhLuongXepKhuonSanLuong == null)
            {
                return NotFound();
            }
            return await _context.CongViecTinhLuongXepKhuonSanLuong.ToListAsync();
        }

        // GET: api/CongViecTinhLuongXepKhuonSanLuongs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CongViecTinhLuongXepKhuonSanLuong>> GetCongViecTinhLuongXepKhuonSanLuong(string id)
        {
            if (_context.CongViecTinhLuongXepKhuonSanLuong == null)
            {
                return NotFound();
            }
            var congViecTinhLuongXepKhuonSanLuong = await _context.CongViecTinhLuongXepKhuonSanLuong.FindAsync(id);

            if (congViecTinhLuongXepKhuonSanLuong == null)
            {
                return NotFound();
            }

            return congViecTinhLuongXepKhuonSanLuong;
        }

        // PUT: api/CongViecTinhLuongXepKhuonSanLuongs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCongViecTinhLuongXepKhuonSanLuong(string id, CongViecTinhLuongXepKhuonSanLuong congViecTinhLuongXepKhuonSanLuong)
        {
            if (id != congViecTinhLuongXepKhuonSanLuong.MaCongViec)
            {
                return BadRequest();
            }

            _context.Entry(congViecTinhLuongXepKhuonSanLuong).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CongViecTinhLuongXepKhuonSanLuongExists(id))
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

        // POST: api/CongViecTinhLuongXepKhuonSanLuongs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CongViecTinhLuongXepKhuonSanLuong>> PostCongViecTinhLuongXepKhuonSanLuong(CongViecTinhLuongXepKhuonSanLuong congViecTinhLuongXepKhuonSanLuong)
        {
            if (_context.CongViecTinhLuongXepKhuonSanLuong == null)
            {
                return Problem("Entity set 'dbPMScontext.CongViecTinhLuongXepKhuonSanLuong'  is null.");
            }
            _context.CongViecTinhLuongXepKhuonSanLuong.Add(congViecTinhLuongXepKhuonSanLuong);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CongViecTinhLuongXepKhuonSanLuongExists(congViecTinhLuongXepKhuonSanLuong.MaCongViec))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCongViecTinhLuongXepKhuonSanLuong", new { id = congViecTinhLuongXepKhuonSanLuong.MaCongViec }, congViecTinhLuongXepKhuonSanLuong);
        }

        // DELETE: api/CongViecTinhLuongXepKhuonSanLuongs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCongViecTinhLuongXepKhuonSanLuong(string id)
        {
            if (_context.CongViecTinhLuongXepKhuonSanLuong == null)
            {
                return NotFound();
            }
            var congViecTinhLuongXepKhuonSanLuong = await _context.CongViecTinhLuongXepKhuonSanLuong.FindAsync(id);
            if (congViecTinhLuongXepKhuonSanLuong == null)
            {
                return NotFound();
            }

            _context.CongViecTinhLuongXepKhuonSanLuong.Remove(congViecTinhLuongXepKhuonSanLuong);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CongViecTinhLuongXepKhuonSanLuongExists(string id)
        {
            return (_context.CongViecTinhLuongXepKhuonSanLuong?.Any(e => e.MaCongViec == id)).GetValueOrDefault();
        }





        [HttpGet("{dateTime}/{maXuong}")]
        [Authorize]
        public IActionResult GetAllsFullField(string dateTime, string maXuong)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmCongViecTinhLuongSanLuong.GetsFullField<object>(date1, maXuong);
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetCas()
        {
            var items = Vm.VmCa.Gets();

            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.CongViecTinhLuongXepKhuonSanLuong.ToList();

            return Ok(items);
        }
        //[HttpGet("{maCongThuc}")]
        //[Authorize]
        //public IActionResult GetAllWithMaCongThucAndNgays(string maCongThuc)
        //{
        //    var items = _context.PD_CongThucChiTiet.Where(x => x.MaCongThuc == maCongThuc).ToList();
        //    return Ok(items);
        //}
        [HttpGet("{maCongViec}/{dateTime}/{maCa}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(string maCongViec, string dateTime, string maCa, string maXuong)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.CongViecTinhLuongXepKhuonSanLuong.FirstOrDefault(x => x.MaCongViec == maCongViec && x.Ngay == date1 && x.MaCa == maCa && x.MaXuong == maXuong);
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
            var model = JsonSerializer.Deserialize<CongViecTinhLuongXepKhuonSanLuong>(dataT.Item1);
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
            if (_context.CongViecTinhLuongXepKhuonSanLuong.Any(x => x.MaCongViec == model.MaCongViec && x.Ngay == model.Ngay && x.MaCa == model.MaCa && x.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new CongViecTinhLuongXepKhuonSanLuong
            {
                MaCongViec = model.MaCongViec,
                Ngay = model.Ngay,
                MaCa = model.MaCa,
                MaXuong = model.MaXuong,
                SanLuong = model.SanLuong,
            };
            _context.CongViecTinhLuongXepKhuonSanLuong.Add(newItem);
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
        //[HttpPost("{stt}/{ngay}/{maCongThuc}")]
        //[Authorize]
        //public async Task<IActionResult> Update(int stt, string ngay, string maCongThuc, Tuple<string> dataT)
        //{
        //    var model = JsonSerializer.Deserialize<PD_CongThucChiTiet>(dataT.Item1);
        //    // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
        //    if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maCongThuc))
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Mã này không hợp lệ."
        //        });
        //    }
        //    DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        //    var item = await _context.PD_CongThucChiTiet.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaCongThuc == maCongThuc);

        //    if (item == null)
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Mục không tồn tại."
        //        });
        //    }

        //    // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors)
        //                                      .Select(e => e.ErrorMessage)
        //                                      .ToList();

        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Dữ liệu không hợp lệ.",
        //            Errors = errors
        //        });
        //    }
        //    item.MaSanPham = model.MaSanPham;
        //    item.SanLuong = model.SanLuong;
        //    item.TyLe = model.TyLe;
        //    item.BienDo = model.BienDo;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
        //            Errors = new List<string> { ex.Message }
        //        });
        //    }
        //    return Ok(new ApiResponse
        //    {
        //        Success = true,
        //        Message = "Cập nhật thông tin thành công!"
        //    });
        //}
        [HttpPost("{maCongViec}/{dateTime}/{maCa}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maCongViec, string dateTime, string maCa, string maXuong)
        {
            if (string.IsNullOrEmpty(maCongViec.ToString()) || string.IsNullOrEmpty(dateTime))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
             DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.CongViecTinhLuongXepKhuonSanLuong.FirstOrDefault(x => x.MaCongViec == maCongViec && x.Ngay == date1 && x.MaCa == maCa && x.MaXuong == maXuong);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.CongViecTinhLuongXepKhuonSanLuong.Remove(item);

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
