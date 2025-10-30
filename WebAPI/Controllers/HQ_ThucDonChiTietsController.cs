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
    public class HQ_ThucDonChiTietsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_ThucDonChiTietsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = Vm.VmHq_ThucDonChiTiet.Items.OrderByDescending(x => x.Id).ToList();
            return Ok(items);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetsByMa(long id)
        {
            var item = await Vm.VmHq_ThucDonChiTiet.FindAsync(id);
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
            var model = JsonSerializer.Deserialize<HQ_ThucDonChiTiet>(dataT.Item1);
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
            //if (await Vm.VmHq_ThucDonChiTiet.FindAsync(model.Id) != null)
            //{
            //    return BadRequest(new ApiResponse
            //    {
            //        Success = false,
            //        Message = "Mã này đã tồn tại.",
            //    });
            //}
            var newItem = new HQ_ThucDonChiTiet
            {
                ThucDonId = model.ThucDonId,
                MonAnId = model.MonAnId,
                GhiChu = model.GhiChu,

            };
            try
            {
                await Vm.VmHq_ThucDonChiTiet.InsertAsync(newItem);
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
            var model = JsonSerializer.Deserialize<HQ_ThucDonChiTiet>(dataT.Item1);
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
            var item = await Vm.VmHq_ThucDonChiTiet.FindAsync(id);
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
            item.ThucDonId = model.ThucDonId;
            item.MonAnId = model.MonAnId;
            item.GhiChu = model.GhiChu;

            try
            {
                await Vm.VmHq_ThucDonChiTiet.UpdateAsync(item);
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
        [HttpPost("{thucDonId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int thucDonId)
        {
            if (thucDonId <= 0 || thucDonId == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var item = await Vm.VmHq_ThucDonChiTiet.FindAsync(thucDonId);
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
                await Vm.VmHq_ThucDonChiTiet.DeleteAsync(item);
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
        [HttpPost("{listIdThucDonChiTietRs}")]
        [Authorize]
        public async Task<IActionResult> DeleteThucDonChiTietByListId(string listIdThucDonChiTietRs)
        {
            if (string.IsNullOrEmpty(listIdThucDonChiTietRs))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }

            string[] partIdThucDonChiTiets = listIdThucDonChiTietRs.TrimEnd('|').Split('|');
            foreach (string idThucDonChiTiet in partIdThucDonChiTiets)
            {
                var item = await Vm.VmHq_ThucDonChiTiet.FindAsync(int.Parse(idThucDonChiTiet));
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
                    await Vm.VmHq_ThucDonChiTiet.DeleteAsync(item);
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
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đã xoá!"
            });
        }
        [HttpPost("{thucDonId}")]
        [Authorize]
        public async Task<IActionResult> DeleteByThucDonId(int thucDonId)
        {
            if (string.IsNullOrEmpty(thucDonId.ToString()))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            var items = await Vm.VmHq_ThucDonChiTiet.FindByThucDonIdAsync(thucDonId);
            if (items == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tồn tại."
                });
            }
            try
            {
                await Vm.VmHq_ThucDonChiTiet.DeleteByThucDonIdsAsync(thucDonId);
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
