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
    public class MaMauCaGiongVungNuoisController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaMauCaGiongVungNuoisController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmMauCaGiongVungNuoi.GetsFullField<object>();
            return Ok(items);
        }


        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaMauCaGiongVungNuoi.OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpGet("{maGhe}")]
        [Authorize]
        public IActionResult GetByMaGhes(string maGhe)
        {
            var items = _context.MaMauCaGiongVungNuoi.Where(x => x.MaGhe == maGhe).ToList();
            return Ok(items);
        }
        [HttpGet("{ngay}/{maGhe}")]
        [Authorize]
        public IActionResult GetsByMa(string ngay, string maGhe)
        {
            //DateTime ngayConvert = DateTime.Parse(ngay);
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.MaMauCaGiongVungNuoi.FirstOrDefault(x => x.Ngay == ngayConvert && x.MaGhe == maGhe);
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
            var model = JsonSerializer.Deserialize<MaMauCaGiongVungNuoi>(dataT.Item1);
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
            if (_context.MaMauCaGiongVungNuoi.Any(u => u.Ngay == model.Ngay && u.MaGhe == model.MaGhe))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaMauCaGiongVungNuoi
            {
                Ngay = model.Ngay,
                MaGhe = model.MaGhe,
                SoLuong = model.SoLuong,
                TrongLuongDonVi = model.TrongLuongDonVi,
                TrongLuong = model.TrongLuong
            };
            _context.MaMauCaGiongVungNuoi.Add(newItem);
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
        [HttpPost("{ngay}/{maGhe}")]
        [Authorize]
        public async Task<IActionResult> Update(string ngay, string maGhe, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<MaMauCaGiongVungNuoi>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maGhe))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.MaMauCaGiongVungNuoi.FirstOrDefaultAsync(x => x.Ngay == ngayConvert && x.MaGhe == maGhe);

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
            item.SoLuong = model.SoLuong;
            item.TrongLuongDonVi = model.TrongLuongDonVi;
            item.TrongLuong = model.TrongLuong;
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
        [HttpPost("{ngay}/{maGhe}")]
        [Authorize]
        public async Task<IActionResult> Delete(string ngay, string maGhe)
        {
            if (string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maGhe))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.MaMauCaGiongVungNuoi.FirstOrDefaultAsync(x => x.Ngay == ngayConvert && x.MaGhe == maGhe);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.MaMauCaGiongVungNuoi.Remove(item);

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
