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
    public class T_PhieuCanThuMuasController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_PhieuCanThuMuasController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetChiTietsDateTimeToDateTimeNgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCanThuMua.GetChiTietsDateTimeToDateTimeNgayNguyenLieu<object>(date1, date2, xuongId);
                if (phieuCanChiTiet == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu phù hợp! Vui lòng kiểm tra lại!."
                    });
                }
                return Ok(phieuCanChiTiet);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetChiTietsDateTimeToDateTime(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCanThuMua.GetChiTietsDateTimeToDateTime<object>(date1, date2, xuongId);
                if (phieuCanChiTiet == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu phù hợp! Vui lòng kiểm tra lại!."
                    });
                }
                return Ok(phieuCanChiTiet);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCanThuMua.GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu<object>(date1, date2, xuongId);
                if (phieuCanChiTiet == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu phù hợp! Vui lòng kiểm tra lại!."
                    });
                }
                return Ok(phieuCanChiTiet);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopBaoCaoSauRaiMayPhanCo(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCanThuMua.GetTongHopBaoCaoSauRaiMayPhanCo<object>(date1, date2, xuongId);
                if (phieuCanChiTiet == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu phù hợp! Vui lòng kiểm tra lại!."
                    });
                }
                return Ok(phieuCanChiTiet);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetNgayAndNguyenLieuPhanCo(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCanThuMua.GetNgayAndNguyenLieuPhanCo<object>(date1, date2, xuongId);
                if (phieuCanChiTiet == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu phù hợp! Vui lòng kiểm tra lại!."
                    });
                }
                return Ok(phieuCanChiTiet);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCanThuMua.GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<object>(date1, date2, xuongId);
                if (phieuCanChiTiet == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không có dữ liệu phù hợp! Vui lòng kiểm tra lại!."
                    });
                }
                return Ok(phieuCanChiTiet);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi server: " + ex.Message);
            }
        }
    }
}
