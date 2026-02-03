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
    public class HQ_PhieuCanNguyenLieuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_PhieuCanNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }

        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetPhieuCanNguyenLieuByDateAndXuong(string dateTime, string xuongId)
        {
            if (!DateTime.TryParse(dateTime, out DateTime ngayParsed))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Định dạng ngày không hợp lệ!"
                });
            }

            var result = _context.HQ_PhieuCanNguyenLieus
                .Where(p => p.Ngay.Date == ngayParsed.Date && p.MaXuong == xuongId).ToList();

            if (result.Count == 0)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy dữ liệu với điều kiện đã cho."
                });
            }

            return Ok(result);
        }


    }
}
