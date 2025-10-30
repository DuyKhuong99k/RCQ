using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserAreaController : ControllerBase
    {
        private readonly dbPMScontext _context;
        private readonly AppSetting _appSettings;
        public UserAreaController(dbPMScontext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{userId}")]
        [Authorize]
        public IActionResult GetAllsFullField(int userId)
        {
            var items = Vm.VmUserArea.Gets<object>(userId, _context.Database.GetConnectionString());
            return Ok(items);
        }
       
        [HttpPost]
        [Authorize]
        public IActionResult Insert([FromBody] UserArea userArea)
        {
            try
            {
                var userAreaExist = _context.UserAreas.FirstOrDefault(x => x.UserId == userArea.UserId && x.WKv == userArea.WKv);
                if(userAreaExist != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Khu vực đã được phân bổ."
                    });
                }

                var item = new UserArea()
                {
                    UserId = userArea.UserId,
                    NgayGio = DateTime.Now,
                    WKv = userArea.WKv
                };
                _context.UserAreas.Add(item);
                _context.SaveChanges();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thêm thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = $"Lỗi khi thêm UserArea: {ex.Message}"
                });
            }
        }
         [HttpPost("{UserAreaId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int userAreaId)
        {
            if (userAreaId == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng chọn thông tin!."
                });
            }
            var userArea = _context.UserAreas.FirstOrDefault(x => x.Id == userAreaId);
            if (userArea == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }

            _context.UserAreas.Remove(userArea);

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
                Message = "Xóa thành công!"
            });
        }
    }
    
}
