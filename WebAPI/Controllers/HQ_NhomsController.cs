using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HQ_NhomsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_NhomsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = Vm.VmHQ_Nhom.Gets<HQ_Nhom>();
            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetsByMa(string id)
        {
            var item = Vm.VmHQ_Nhom.GetByMa(id);
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
        public async Task<IActionResult> Insert(HQ_Nhom model)
        {
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
            if (_context.HqNhoms.Any(u => u.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            //var newItem = new HQ_Lo
            //{
            //    Id = model.Id,
            //    NgayNguyenLieu = model.NgayNguyenLieu,
            //    SuDung = model.SuDung,
            //    MNgay = model.MNgay,
            //};
            //_context.HqLos.Add(newItem);
            try
            {
                //await _context.SaveChangesAsync();
                Vm.VmHQ_Nhom.Insert(model);
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
        public async Task<IActionResult> Update(string id, [FromBody] HQ_Nhom model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            var item = await _context.HqNhoms.FindAsync(id);
            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có
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

            //// Cập nhật thông tin vào item
            //item.Id = model.Id;
            //item.NgayNguyenLieu = model.NgayNguyenLieu;
            //item.SuDung = model.SuDung;
            ////item.MNgay = model.MNgay;

            try
            {
                // Lưu thay đổi cập nhật vào bảng HqLoaiNguyenLieus
                //await _context.SaveChangesAsync();

                //// Tạo bản ghi mới cho bảng HqLoaiNguyenLieuUs
                //var newItemUs = new HQ_Lo_U
                //{
                //    LoId = item.Id,
                //    NgayNguyenLieu = item.NgayNguyenLieu,
                //    SuDung = item.SuDung,
                //    MNgay = item.MNgay,
                //    Ngay = DateTime.Now // Ngày hiện tại khi tạo mới
                //};

                //// Thêm vào bảng HqLoaiNguyenLieuUs
                //_context.HqLoUs.Add(newItemUs);

                //// Lưu thay đổi vào cơ sở dữ liệu
                //await _context.SaveChangesAsync();
                Vm.VmHQ_Nhom.Update(model);
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
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await _context.HqNhoms.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Nhóm không tồn tại."
                });
            }

            _context.HqNhoms.Remove(item);

            try
            {
                //Vm.VmHQ_Nhom.Delete(id);
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
