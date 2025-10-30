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


namespace PMS.Controllers.BaoCao.XepKhuon
{
    [Authorize]
    public class BaoCaoKXLXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoKXLXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Không Xử Lý / Chi Tiết", Func = "Xem Báo Cáo / Xếp Khuôn / Không Xử Lý / Chi Tiết")]
        public IActionResult ChiTietKXLXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietKXLXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết KXL Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/KXL/ChiTietKXLXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanXepKhuonKHCs(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/GetPhieuCanXepKhuonKHCs/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Không Xử Lý / Tổng Hợp", Func = "Xem Báo Cáo / Xếp Khuôn / Không Xử Lý / Tổng Hợp")]
        public IActionResult TongHopKXLXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopKXLXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp KXL Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/KXL/TongHopKXLXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopKHCs(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/GetPhieuCanTongHopKHCs/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp SL CÔNG NHÂN
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Không Xử Lý / Tổng Hợp Sản Lượng Công Nhân", Func = "Xem Báo Cáo / Xếp Khuôn / Không Xử Lý / Tổng Hợp Sản Lượng Công Nhân")]
        public IActionResult TongHopSLCongNhanKXLXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopSLCongNhanKXLXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp SL Công Nhân KXL Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/KXL/TongHopSLCongNhanKXLXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopKHCsTheoNhanVien(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/GetPhieuCanTongHopKHCsTheoNhanVien/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string? xuongId = null)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietKXLXepKhuonView")
            {
                dataSource = await GetPhieuCanXepKhuonKHCs(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopKXLXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopKHCs(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopSLCongNhanKXLXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopKHCsTheoNhanVien(fromDate, toDate, xuongId);
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
