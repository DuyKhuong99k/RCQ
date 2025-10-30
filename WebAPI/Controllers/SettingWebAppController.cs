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
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettingWebAppController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public SettingWebAppController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var item = _context.SettingDashboard.First();

            return Ok(item);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> InsertSDB(SettingDashboard model)
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
            if (_context.SettingDashboard.Any(u => u.NumberDate == model.NumberDate))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new SettingDashboard
            {
                NumberDate = model.NumberDate
            };
            _context.SettingDashboard.Add(newItem);
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
        [HttpPost("{numberDate}")]
        [Authorize]
        public async Task<IActionResult> UpdateSDB(int numberDate, [FromBody] SettingDashboard model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (numberDate == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }
            //var item = _context.SettingDashboard.First();
            //_context.SettingDashboard.Remove(item);
            //await _context.SaveChangesAsync();

            //var newItem = new SettingDashboard
            //{
            //    NumberDate = numberDate
            //};
            //_context.SettingDashboard.Add(newItem);
            _=_context.Database.ExecuteSql($"Update SettingDashboard set NumberDate = {numberDate}");
           
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
    }
}
