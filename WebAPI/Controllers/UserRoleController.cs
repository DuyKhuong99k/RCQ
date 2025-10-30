using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViewModels.Repos.API;
using System.Security.Cryptography;
using WebAPI.Models;
using AppModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.InteropServices;
using static Dapper.SqlMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Globalization;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly dbPMScontext _context;
        private readonly AppSetting _appSettings;
        public UserRoleController(dbPMScontext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{userId}")]
        [Authorize]
        public IActionResult GetAllsFullField(int userId)
        {
            var items = Vm.VmUserRole.GetsFullField<object>(userId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{userRoleId}")]
        [Authorize]
        public IActionResult GetAllsFullFieldById(int userRoleId)
        {
            var items = Vm.VmUserRole.GetsFullFieldById<object>(userRoleId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpPost]
        [Authorize]
        public IActionResult InsertList([FromBody] List<UserRole> userRoleList)
        {
            if (userRoleList == null || !userRoleList.Any())
            {
                return BadRequest("Danh sách UserRoles trống hoặc không hợp lệ.");
            }

            try
            {
                _context.UserRoles.AddRange(userRoleList);
                _context.SaveChanges();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Danh sách UserRoles đã được chèn thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = $"Lỗi khi chèn danh sách UserRoles: {ex.Message}"
                });
            }
        }
         [HttpPost("{userRoleId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int userRoleId)
        {
            if (userRoleId == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn thông tin!."
                });
            }
            var userRol = await _context.UserRoles.FindAsync(userRoleId);
            if (userRol == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Vai trò không tồn tại."
                });
            }

            _context.UserRoles.Remove(userRol);

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
                Message = "Huỷ vai trò thành công!"
            });
        }
    }
}
