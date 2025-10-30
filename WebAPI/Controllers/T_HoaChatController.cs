using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    public class T_HoaChatController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_HoaChatController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Gets()
        {
            try
            {
                
                var items = Vm.VmT_TyLeHoaChatTheoNhom.Gets<object>();
                if (items == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu phù hợp! Vui lòng kiểm tra lại!."
                    });
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }
    }
}
