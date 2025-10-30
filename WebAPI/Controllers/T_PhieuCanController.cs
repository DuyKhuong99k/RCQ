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
    public class T_PhieuCanController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_PhieuCanController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetPhieuCanChiTietsToNgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCan.GetPhieuCanChiTietsToNgayNguyenLieu<object>(date1, date2, xuongId);
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
        public async Task<IActionResult> GetTongHopNhanViensToNgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCan.GetTongHopNhanViensToNgayNguyenLieu<object>(date1, date2, xuongId);
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
        public async Task<IActionResult> GetTongHopNhanViens3ToNgayNguyenLieuNSRC(string fromDate, string dateTime, string xuongId, string khuVuc, bool isNhom, bool isDinhMucBinhThuong = true, bool isFloor = true)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var phieuCanChiTiet = Vm.VmT_PhieuCan.GetTongHopNhanViens3ToNgayNguyenLieuNSRC<object>(date1, date2, xuongId, khuVuc, isNhom, isDinhMucBinhThuong, isFloor);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}/{isNhom}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopNhanViens3DateTimeToDateTimeNSRC(string fromDate, string dateTime, string xuongId, string isNhom)
        {
            try
            {
                bool isNhomcvr = bool.Parse(isNhom);
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetTongHopNhanViens3DateTimeToDateTimeNSRC<object>(date1, date2, xuongId, isNhomcvr);
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
        [HttpGet("{dateTime}")]
        [Authorize]
        public async Task<IActionResult> GetAllWithFullField(string dateTime)
        {
            try
            {
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetAllWithFullFields<object>(date2);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopBaoCaoHoaChatNgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetTongHopBaoCaoHoaChatNgayNguyenLieu<object>(date1,date2,xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopBaoCaoHoaChat(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetTongHopBaoCaoHoaChat<object>(date1,date2,xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetNgayAndNguyenLieuHoaChat(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetNgayAndNguyenLieuHoaChat<object>(date1,date2,xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu<object>(date1,date2,xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopBaoCaoHangNgay(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetTongHopBaoCaoHangNgay<object>(date1,date2,xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu<object>(date1,date2,xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetTongHopBaoCaoSauRaiMayPhanCo(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetTongHopBaoCaoSauRaiMayPhanCo<object>(date1, date2, xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetNgayAndNguyenLieuPhanCo(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetNgayAndNguyenLieuPhanCo<object>(date1, date2, xuongId);
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
        [HttpGet("{fromDate}/{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<IActionResult> GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu(string fromDate, string dateTime, string xuongId)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var items = Vm.VmT_PhieuCan.GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<object>(date1, date2, xuongId);
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
