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
    public class CongViecTinhLuongXepKhuonTheoLoaiThanhPhamsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public CongViecTinhLuongXepKhuonTheoLoaiThanhPhamsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        // GET: api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CongViecTinhLuongXepKhuonTheoLoaiThanhPham>>> GetCongViecTinhLuongXepKhuonTheoLoaiThanhPham()
        {
            if (_context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham == null)
            {
                return NotFound();
            }
            return await _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.ToListAsync();
        }

        // GET: api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CongViecTinhLuongXepKhuonTheoLoaiThanhPham>> GetCongViecTinhLuongXepKhuonTheoLoaiThanhPham(string id)
        {
            if (_context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham == null)
            {
                return NotFound();
            }
            var congViecTinhLuongXepKhuonTheoLoaiThanhPham = await _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.FindAsync(id);

            if (congViecTinhLuongXepKhuonTheoLoaiThanhPham == null)
            {
                return NotFound();
            }

            return congViecTinhLuongXepKhuonTheoLoaiThanhPham;
        }

        // PUT: api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCongViecTinhLuongXepKhuonTheoLoaiThanhPham(string id, CongViecTinhLuongXepKhuonTheoLoaiThanhPham congViecTinhLuongXepKhuonTheoLoaiThanhPham)
        {
            if (id != congViecTinhLuongXepKhuonTheoLoaiThanhPham.MaCongViec)
            {
                return BadRequest();
            }

            _context.Entry(congViecTinhLuongXepKhuonTheoLoaiThanhPham).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CongViecTinhLuongXepKhuonTheoLoaiThanhPhamExists(id))
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

        // POST: api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CongViecTinhLuongXepKhuonTheoLoaiThanhPham>> PostCongViecTinhLuongXepKhuonTheoLoaiThanhPham(CongViecTinhLuongXepKhuonTheoLoaiThanhPham congViecTinhLuongXepKhuonTheoLoaiThanhPham)
        {
            if (_context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham == null)
            {
                return Problem("Entity set 'dbPMScontext.CongViecTinhLuongXepKhuonTheoLoaiThanhPham'  is null.");
            }
            _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.Add(congViecTinhLuongXepKhuonTheoLoaiThanhPham);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CongViecTinhLuongXepKhuonTheoLoaiThanhPhamExists(congViecTinhLuongXepKhuonTheoLoaiThanhPham.MaCongViec))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCongViecTinhLuongXepKhuonTheoLoaiThanhPham", new { id = congViecTinhLuongXepKhuonTheoLoaiThanhPham.MaCongViec }, congViecTinhLuongXepKhuonTheoLoaiThanhPham);
        }

        // DELETE: api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCongViecTinhLuongXepKhuonTheoLoaiThanhPham(string id)
        {
            if (_context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham == null)
            {
                return NotFound();
            }
            var congViecTinhLuongXepKhuonTheoLoaiThanhPham = await _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.FindAsync(id);
            if (congViecTinhLuongXepKhuonTheoLoaiThanhPham == null)
            {
                return NotFound();
            }

            _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.Remove(congViecTinhLuongXepKhuonTheoLoaiThanhPham);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CongViecTinhLuongXepKhuonTheoLoaiThanhPhamExists(string id)
        {
            return (_context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham?.Any(e => e.MaCongViec == id)).GetValueOrDefault();
        }





        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmCongViecTinhLuongTheoLoaiThanhPham.GetsFullField<object>();
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.ToList();

            return Ok(items);
        }
        //[HttpGet("{maCongThuc}")]
        //[Authorize]
        //public IActionResult GetAllWithMaCongThucAndNgays(string maCongThuc)
        //{
        //    var items = _context.PD_CongThucChiTiet.Where(x => x.MaCongThuc == maCongThuc).ToList();
        //    return Ok(items);
        //}
        [HttpGet("{maCongViec}/{maThanhPhamDinhHinh}/{maThanhPhamPhuXepKhuon}/{maThanhPhamChinhXepKhuon}/{maThanhPhamBlockXepKhuon}/{maThanhPhamKHCXepKhuon}/{maThanhPhamTaiChe}/{maThanhPhamSoChe}/{maCongViecTaiChe}/{maCongDoan}/{maChieuXaChinhXepKhuon}")]
        [Authorize]
        public IActionResult GetsByMa(string maCongViec, string maThanhPhamDinhHinh, string maThanhPhamPhuXepKhuon, string maThanhPhamChinhXepKhuon, string maThanhPhamBlockXepKhuon, string maThanhPhamKHCXepKhuon, string maThanhPhamTaiChe, string maThanhPhamSoChe, string maCongViecTaiChe, string maCongDoan, string maChieuXaChinhXepKhuon)
        {

            var item = _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.FirstOrDefault(x => x.MaCongViec == maCongViec && x.MaThanhPhamDinhHinh == maThanhPhamDinhHinh && x.MaThanhPhamPhuXepKhuon == maThanhPhamPhuXepKhuon && x.MaThanhPhamChinhXepKhuon == maThanhPhamChinhXepKhuon && x.MaThanhPhamBlockXepKhuon == maThanhPhamBlockXepKhuon && x.MaThanhPhamKHCXepKhuon == maThanhPhamKHCXepKhuon && x.MaThanhPhamTaiChe == maThanhPhamTaiChe && x.MaThanhPhamSoChe == maThanhPhamSoChe && x.MaCongViecTaiChe == maCongViecTaiChe && x.MaCongDoan == maCongDoan && x.MaChieuXaChinhXepKhuon ==  maChieuXaChinhXepKhuon);
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
            var model = JsonSerializer.Deserialize<CongViecTinhLuongXepKhuonTheoLoaiThanhPham>(dataT.Item1);
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
            if (_context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.Any(x => x.MaCongViec == model.MaCongViec && x.MaThanhPhamDinhHinh == model.MaThanhPhamDinhHinh && x.MaThanhPhamPhuXepKhuon == model.MaThanhPhamPhuXepKhuon && x.MaThanhPhamChinhXepKhuon == model.MaThanhPhamChinhXepKhuon && x.MaThanhPhamBlockXepKhuon == model.MaThanhPhamBlockXepKhuon && x.MaThanhPhamKHCXepKhuon == model.MaThanhPhamKHCXepKhuon && x.MaThanhPhamTaiChe == model.MaThanhPhamTaiChe && x.MaThanhPhamSoChe == model.MaThanhPhamSoChe && x.MaCongViecTaiChe == model.MaCongViecTaiChe && x.MaCongDoan == model.MaCongDoan && x.MaChieuXaChinhXepKhuon ==  model.MaChieuXaChinhXepKhuon))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new CongViecTinhLuongXepKhuonTheoLoaiThanhPham
            {
               MaCongViec = model.MaCongViec,
               MaThanhPhamDinhHinh = model.MaThanhPhamDinhHinh,
               MaThanhPhamPhuXepKhuon = model.MaThanhPhamPhuXepKhuon,
               MaThanhPhamChinhXepKhuon = model.MaThanhPhamChinhXepKhuon,
               MaThanhPhamBlockXepKhuon = model.MaThanhPhamBlockXepKhuon,
               MaThanhPhamKHCXepKhuon = model.MaThanhPhamKHCXepKhuon,
               MaThanhPhamTaiChe = model.MaThanhPhamTaiChe,
               MaThanhPhamSoChe = model.MaThanhPhamSoChe,
               MaCongViecTaiChe = model.MaCongViecTaiChe,
               MaCongDoan = model.MaCongDoan,
               MaChieuXaChinhXepKhuon = model.MaChieuXaChinhXepKhuon
            };
            _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.Add(newItem);
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
        [HttpPost("{maCongViec}/{maThanhPhamDinhHinh}/{maThanhPhamPhuXepKhuon}/{maThanhPhamChinhXepKhuon}/{maThanhPhamBlockXepKhuon}/{maThanhPhamKHCXepKhuon}/{maThanhPhamTaiChe}/{maThanhPhamSoChe}/{maCongViecTaiChe}/{maCongDoan}/{maChieuXaChinhXepKhuon}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maCongViec, string maThanhPhamDinhHinh, string maThanhPhamPhuXepKhuon, string maThanhPhamChinhXepKhuon, string maThanhPhamBlockXepKhuon, string maThanhPhamKHCXepKhuon, string maThanhPhamTaiChe, string maThanhPhamSoChe, string maCongViecTaiChe, string maCongDoan, string maChieuXaChinhXepKhuon)
        {
            if (string.IsNullOrEmpty(maCongViec.ToString()) || string.IsNullOrEmpty(maThanhPhamDinhHinh))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
           
            var item = await _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.FirstOrDefaultAsync(x => x.MaCongViec == maCongViec && x.MaThanhPhamDinhHinh == maThanhPhamDinhHinh && x.MaThanhPhamPhuXepKhuon == maThanhPhamPhuXepKhuon && x.MaThanhPhamChinhXepKhuon == maThanhPhamChinhXepKhuon && x.MaThanhPhamBlockXepKhuon == maThanhPhamBlockXepKhuon && x.MaThanhPhamKHCXepKhuon == maThanhPhamKHCXepKhuon && x.MaThanhPhamTaiChe == maThanhPhamTaiChe && x.MaThanhPhamSoChe == maThanhPhamSoChe && x.MaCongViecTaiChe == maCongViecTaiChe && x.MaCongDoan == maCongDoan && x.MaChieuXaChinhXepKhuon ==  maChieuXaChinhXepKhuon);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.CongViecTinhLuongXepKhuonTheoLoaiThanhPham.Remove(item);

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
