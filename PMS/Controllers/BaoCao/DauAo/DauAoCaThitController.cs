using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Models;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Security.Policy;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using Azure.Core;
using Models.Repos.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using Models.Repos;
using System;
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;

namespace PMS.Controllers.BaoCao.DauAo
{
    [Authorize]
    public class DauAoCaThitController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DauAoCaThitController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Đầu Ao Cá Thịt / Chi Tiết", Func = "Xem Báo Cáo / Đầu Ao Cá Thịt / Chi Tiết")]
        public IActionResult ChiTietDauAoCaThitView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietDauAoCaThitView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Cá Thịt";
            return View("~/Views/BaoCao/DauAo/ChiTietDauAoCaThitView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetChiTiets/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp
        [CustomAuthorize(Fu = "Báo Cáo / Đầu Ao Cá Thịt / Tổng Hợp", Func = "Xem Báo Cáo / Đầu Ao Cá Thịt / Tổng Hợp")]
        public IActionResult TongHopDauAoCaThitView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopDauAoCaThitView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Cá Thịt";
            return View("~/Views/BaoCao/DauAo/TongHopDauAoCaThitView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHops(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetTongHops/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Ghe - Ngày Bắt Cá
        [CustomAuthorize(Fu = "Báo Cáo / Đầu Ao Cá Thịt / Ghe Ngày Bắt Cá", Func = "Xem Báo Cáo / Đầu Ao Cá Thịt / Ghe Ngày Bắt Cá")]
        public IActionResult TongHopGheNgayBatCaDauAoCaThitView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopGheNgayBatCaDauAoCaThitView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Ghe - Ngày Bắt Cá";
            return View("~/Views/BaoCao/DauAo/TongHopGheNgayBatCaDauAoCaThitView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHops_NgayBatCa(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetTongHops_NgayBatCa/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Ghe - Ngày Nhập Xưởng
        [CustomAuthorize(Fu = "Báo Cáo / Đầu Ao Cá Thịt / Ghe Ngày Nhập Xưởng", Func = "Xem Báo Cáo / Đầu Ao Cá Thịt / Ghe Ngày Nhập Xưởng")]
        public IActionResult TongHopGheNgayNhapXuongDauAoCaThitView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopGheNgayNhapXuongDauAoCaThitView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Ghe - Ngày Nhập Xưởng";
            return View("~/Views/BaoCao/DauAo/TongHopGheNgayNhapXuongDauAoCaThitView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHops_NgayNhapXuong(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetTongHops_NgayNhapXuong/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietDauAoCaThitView")
            {
                dataSource = await GetChiTiets(fromDate, toDate);
            }
            else if (reportType == "TongHopDauAoCaThitView")
            {
                dataSource = await GetTongHops(fromDate, toDate);
            }
            else if (reportType == "TongHopGheNgayBatCaDauAoCaThitView")
            {
                dataSource = await GetTongHops_NgayBatCa(fromDate, toDate);
            }
            else if (reportType == "TongHopGheNgayNhapXuongDauAoCaThitView")
            {
                dataSource = await GetTongHops_NgayNhapXuong(fromDate, toDate);
            }
            if (dataSource == null || !dataSource.Any())
            {
                success = false; // Đặt trạng thái thành không thành công nếu dataSource là null hoặc không có dữ liệu.
            }
            var result = new
            {
                Success = success, // Trạng thái thành công
                Messages = success ? "Lấy dữ liệu Thành Công." : "Vui lòng kiểm tra lại!.", // Thông báo tùy thuộc vào trạng thái
                Data = dataSource // Dữ liệu từ GetChiTiets, GetTongHopNhanViens hoặc GetTongHopThanhPhams
            };
            ViewBag.datasource = dataSource;
            return Json(result);
        }
    }
}
