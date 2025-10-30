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
    public class MaThanhPham_PhoiTronController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPham_PhoiTronController(dbPMScontext context)
        {
            _context = context;
        }

        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaThanhPham_PhoiTron.OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpGet("{dateTime}/{maKhuVuc}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetAllWithMaKhuVucs(string dateTime, string maKhuVuc)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            IEnumerable<object> items = null;

            if (maKhuVuc == "DH")
            {
                items = Vm.VmThanhPhamPhoiTron.GetsAllByMaKhuVucDH<object>(date1, maKhuVuc);
            }
            else if (maKhuVuc == "FL")
            {
                items = Vm.VmThanhPhamPhoiTron.GetsAllByMaKhuVucFL<object>(date1, maKhuVuc);
            }

            return Ok(items);
        }
        [HttpGet("{dateTime}/{maThanhPhamOrg}/{maThanhPhamDes}/{maKhuVuc}/{maXuong}/{maLo}")]
        [Authorize]
        public IActionResult GetsByMa(string dateTime, string maThanhPhamOrg, string maThanhPhamDes, string maKhuVuc, string maXuong, string maLo)
        {
            DateTime ngay = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.MaThanhPham_PhoiTron.FirstOrDefault(x => x.Ngay == ngay && x.MaThanhPhamOrg == maThanhPhamOrg && x.MaThanhPhamDes == maThanhPhamDes && x.MaKhuVuc == maKhuVuc && x.MaXuong == maXuong && x.MaLo == maLo);
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
            var model = JsonSerializer.Deserialize<MaThanhPham_PhoiTron>(dataT.Item1);
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
            if (_context.MaThanhPham_PhoiTron.Any(x => x.Ngay == model.Ngay && x.MaThanhPhamOrg == model.MaThanhPhamOrg && x.MaThanhPhamDes == model.MaThanhPhamDes && x.MaKhuVuc == model.MaKhuVuc && x.MaXuong == model.MaXuong && x.MaLo == model.MaLo))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new MaThanhPham_PhoiTron
            {
                Ngay = model.Ngay,
                MaThanhPhamOrg = model.MaThanhPhamOrg,
                MaThanhPhamDes = model.MaThanhPhamDes,
                MaKhuVuc = model.MaKhuVuc,
                MaXuong = model.MaXuong,
                MaLo = model.MaLo,
                TyLe = model.TyLe,
                CreateBy = model.CreateBy,
                CreateDateTime = model.CreateDateTime,
                ModifyBy = model.ModifyBy,
                ModifyDateTime = model.ModifyDateTime,
            };
            _context.MaThanhPham_PhoiTron.Add(newItem);
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
        [HttpPost("{dateTime}/{maThanhPhamOrg}/{maThanhPhamDes}/{maKhuVuc}/{maXuong}/{maLo}")]
        [Authorize]
        public async Task<IActionResult> Update(string dateTime, string maThanhPhamOrg, string maThanhPhamDes, string maKhuVuc, string maXuong, string maLo, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<MaThanhPham_PhoiTron>(dataT.Item1);

            DateTime ngay = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.MaThanhPham_PhoiTron.FirstOrDefault(x => x.Ngay == ngay && x.MaThanhPhamOrg == maThanhPhamOrg && x.MaThanhPhamDes == maThanhPhamDes && x.MaKhuVuc == maKhuVuc && x.MaXuong == maXuong && x.MaLo == maLo);

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
            item.TyLe = model.TyLe;
            item.ModifyBy = model.ModifyBy;
            item.ModifyDateTime = model.ModifyDateTime;
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
        [HttpPost("{dateTime}/{maThanhPhamOrg}/{maThanhPhamDes}/{maKhuVuc}/{maXuong}/{maLo}")]
        [Authorize]
        public async Task<IActionResult> Delete(string dateTime, string maThanhPhamOrg, string maThanhPhamDes, string maKhuVuc, string maXuong, string maLo)
        {
            DateTime ngay = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.MaThanhPham_PhoiTron.FirstOrDefault(x => x.Ngay == ngay && x.MaThanhPhamOrg == maThanhPhamOrg && x.MaThanhPhamDes == maThanhPhamDes && x.MaKhuVuc == maKhuVuc && x.MaXuong == maXuong && x.MaLo == maLo);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.MaThanhPham_PhoiTron.Remove(item);

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