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
    public class RolePermistionController : Controller
    {
        private readonly dbPMScontext _context;
        private readonly AppSetting _appSettings;
        public RolePermistionController(dbPMScontext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public IActionResult GetAllsFullField()
        {
            var items = Vm.VmRolePermistion.GetsFullField<object>(_context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{roleId}")]
        [Authorize]
        public IActionResult GetAllsFullField(int roleId)
        {
            var items = Vm.VmRolePermistion.GetsFullField<object>(roleId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAllRolePermistion()
        {
            //var RolePermistion = _context.RolePermistion.OrderByDescending(x => x.Id).ToList();
            var RolePermistion = Vm.VmRolePermistion.GetAllRolePermissions();
            return Ok(RolePermistion);
        }
        [HttpGet("{permistionId}")]
        [Authorize]
        public IActionResult GetAllRolePermistionById(int permistionId)
        {
            try
            {
                //var rolePermission = _context.RolePermistion
                //    .Where(x => x.Id == permistionId)
                //    .First();
                //var rolePermission = Vm.VmRolePermistion.GetAllRolePermistionById(permistionId,_context.Database.GetConnectionString());
                var rolePermission = Vm.VmRolePermistion.Items.Where(x => x.Id == permistionId).FirstOrDefault();
                // Kiểm tra xem có dữ liệu không
                if (rolePermission != null)
                {
                    return Ok(rolePermission);
                }
                else
                {
                    // Thông báo không tìm thấy dữ liệu
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy dữ liệu phù hợp!."
                    });
                }
            }
            catch (Exception ex)
            {
                // In ra thông báo lỗi nếu có
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        [HttpGet("{roleId}")]
        [Authorize]
        public IActionResult GetAllRolePermistionByRoleId(int roleId)
        {
            try
            {
                //var rolePermission = _context.RolePermistion
                //    .Where(x => x.RoleId == roleId)
                //    .ToList();
                var rolePermission = Vm.VmRolePermistion.GetAllRolePermistionByRoleId(roleId, _context.Database.GetConnectionString());

                // Kiểm tra xem có dữ liệu không
                if (rolePermission != null)
                {
                    return Ok(rolePermission);
                }
                else
                {
                    // Thông báo không tìm thấy dữ liệu
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy dữ liệu phù hợp!."
                    });
                }
            }
            catch (Exception ex)
            {
                // In ra thông báo lỗi nếu có
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        /// <summary>
        /// Không xài
        /// </summary>
        /// <param name="ma"></param>
        /// <returns></returns>
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMa(int ma)
        {
            var item = _context.RolePermistion.FirstOrDefault(x => x.Id == ma);
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
        /// <summary>
        /// Không xài
        /// </summary>
        /// <param name="ma"></param>
        /// <returns></returns>
        [HttpGet("{ma}")]
        [Authorize]
        public IActionResult GetsByMaRole(int ma)
        {
            var item = _context.RolePermistion.FirstOrDefault(x => x.RoleId == ma);
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
        [HttpGet]
        [Authorize]
        public IActionResult GetRolePermistions([FromQuery] List<int> roleIds)
        {
            if (roleIds == null || roleIds.Count == 0)
            {
                // Xử lý khi không có roleIds được truyền vào
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Danh sách roleIds không hợp lệ."
                });
            }
            var item = Vm.VmRolePermistion.GetRolePermistions(roleIds);
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
        [HttpGet]
        [Authorize]
        public IActionResult GetRolePermissions(string fu, string func)
        {
            var rolePermissions = Vm.VmRolePermistion.GetRolePermissions(fu, func,_context);

            return Ok(rolePermissions);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert(RolePermistion model)
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
            if (Vm.VmRolePermistion.Items.Any(x => x.Id == model.Id))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new RolePermistion
            {
                RoleId = model.RoleId,
                Fu = model.Fu,
                Func = model.Func,
                CreatedDateTime = model.CreatedDateTime,
                Status = model.Status
            };
            //_context.RolePermistion.Add(newItem);
            try
            {
                //await _context.SaveChangesAsync();
                Vm.VmRolePermistion.Insert(newItem, _context.Database.GetConnectionString());
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


        [HttpPost]
        [Authorize]
        public IActionResult InsertList([FromBody] List<RolePermistion> rolePermissionList)
        {
            if (rolePermissionList == null || !rolePermissionList.Any())
            {
                return BadRequest("Danh sách RolePermission trống hoặc không hợp lệ.");
            }

            try
            {
                // Thêm danh sách RolePermission vào Context và lưu vào CSDL
                //_context.RolePermistion.AddRange(rolePermissionList);
                //_context.SaveChanges();
                Vm.VmRolePermistion.Insert(rolePermissionList, _context.Database.GetConnectionString());
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Danh sách RolePermission đã được chèn thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = $"Lỗi khi chèn danh sách RolePermission: {ex.Message}"
                });
            }
        }

        [HttpPost("{permistionId}")]
        [Authorize]
        public async Task<IActionResult> Update(int permistionId, [FromBody] RolePermistion model)
        {
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (permistionId == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không hợp lệ."
                });
            }

            // Kiểm tra xem nhân viên có tồn tại trong cơ sở dữ liệu không
            //var item = await _context.RolePermistion.FindAsync(permistionId);
            var item = Vm.VmRolePermistion.Items.Where(x => x.Id == permistionId).First();
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
            //item.Status = model.Status;
            try
            {
                //await _context.SaveChangesAsync();
                Vm.VmRolePermistion.Update(model, _context.Database.GetConnectionString());
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
        [HttpPost("{permistionId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int permistionId)
        {
            if (permistionId == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = _context.RolePermistion.Where(x => x.Id == permistionId).FirstOrDefault();
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã Không tồn tại."
                });
            }

            _context.RolePermistion.Remove(item);
            //Vm.VmRolePermistion.Delete(item, _context.Database.GetConnectionString());

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