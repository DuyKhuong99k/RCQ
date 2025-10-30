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
    public class HQ_ThucDonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_ThucDonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = Vm.VmHq_ThucDon.Items.OrderByDescending(x => x.Id).ToList();
            return Ok(items);
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult GetMaxId()
        {
            int id = Vm.VmHq_ThucDon.GetMaxId();
            return Ok(id);
        }

        [HttpGet("{ngay}")]
        [Authorize]
        public IActionResult GetThucDonByDate(string ngay)
        {
            DateTime date = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmHq_ThucDon.GetDanhSachThucDon<object>(date);
            return Ok(items);
        }
        [HttpGet("{thucDonId}")]
        [Authorize]
        public IActionResult GetChiTietTheoThucDon(int thucDonId)
        {
            var items = Vm.VmHq_ThucDon.GetChiTietTheoThucDon<object>(thucDonId);
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetDanhSachTrangThaiThucDonTungNgay()
        {
            var items = Vm.VmHq_ThucDon.GetDanhSachTrangThaiThucDonTungNgay<object>();
            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetsByMa(long id)
        {
            var item = await Vm.VmHq_ThucDon.FindAsync(id);
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
             var model = JsonSerializer.Deserialize<HQ_ThucDon>(dataT.Item1);
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
            //if (await Vm.VmHq_ThucDon.FindAsync(model.Id) != null)
            //{
            //    return BadRequest(new ApiResponse
            //    {
            //        Success = false,
            //        Message = "Mã này đã tồn tại.",
            //    });
            //}
            var newItem = new HQ_ThucDon
            {
                //Id = 0,
                Ngay = model.Ngay,
                NgayTao = DateTime.Now,
                NguoiTao = model.NguoiTao,
                Ten = model.Ten,
                ThietBi = model.ThietBi,
                GhiChu = model.GhiChu,

            };
            try
            {
                await Vm.VmHq_ThucDon.InsertAsync(newItem);
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(long id, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<HQ_ThucDon>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await Vm.VmHq_ThucDon.FindAsync(id);
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
            item.Ngay = model.Ngay;
            item.NguoiTao = model.NguoiTao;
            item.NgayTao = model.NgayTao;
            item.Ten = model.Ten;
            item.ThietBi = model.ThietBi;
            item.GhiChu = model.GhiChu;

            try
            {
                await Vm.VmHq_ThucDon.UpdateAsync(item);
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
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await Vm.VmHq_ThucDon.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            try
            {
                await Vm.VmHq_ThucDon.DeleteAsync(item);
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
