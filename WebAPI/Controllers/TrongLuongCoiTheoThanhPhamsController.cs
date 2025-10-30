using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class TrongLuongCoiTheoThanhPhamsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TrongLuongCoiTheoThanhPhamsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.TrongLuongCoiTheoThanhPhams.OrderByDescending(x => x.MaCoi).ToList();

            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmTrongLuongCoiTheoThanhPham.GetAllsFullField<object>();
            return Ok(items);
        }
        [HttpGet("{maCoi}/{maThanhPham}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(string maCoi, string maThanhPham,string maXuong)
        {
            var item = _context.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == maCoi && x.MaThanhPham == maThanhPham && x.MaXuong == maXuong);
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
             var model = JsonSerializer.Deserialize<TrongLuongCoiTheoThanhPham>(dataT.Item1);
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
            if (_context.TrongLuongCoiTheoThanhPhams.Any(x => x.MaCoi == model.MaCoi && x.MaThanhPham == model.MaThanhPham && x.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new TrongLuongCoiTheoThanhPham
            {
                MaCoi = model.MaCoi,
                MaThanhPham = model.MaThanhPham,
                MaXuong = model.MaXuong,
                TrongLuongMax = model.TrongLuongMax
            };
            
            _context.TrongLuongCoiTheoThanhPhams.Add(newItem);
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
        [HttpPost("{maCoi}/{maThanhPham}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update(string maCoi, string maThanhPham,string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<TrongLuongCoiTheoThanhPham>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = _context.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == maCoi && x.MaThanhPham == maThanhPham && x.MaXuong == maXuong);
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
            item.TrongLuongMax = model.TrongLuongMax;
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
        [HttpPost("{maCoi}/{maThanhPham}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete(string maCoi, string maThanhPham,string maXuong)
        {
            if (string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = _context.TrongLuongCoiTheoThanhPhams.FirstOrDefault(x => x.MaCoi == maCoi && x.MaThanhPham == maThanhPham && x.MaXuong == maXuong);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.TrongLuongCoiTheoThanhPhams.Remove(item);

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
