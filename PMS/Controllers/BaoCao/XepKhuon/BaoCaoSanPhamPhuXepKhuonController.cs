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
    public class BaoCaoSanPhamPhuXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoSanPhamPhuXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Phụ / Chi Tiết", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Phụ / Chi Tiết")]
        public IActionResult ChiTietPhuXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietPhuXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Phụ Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Phu/ChiTietPhuXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanPhuXepKhuons(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/GetPhieuCanPhuXepKhuons/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Phụ / Tổng Hợp", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Phụ / Tổng Hợp")]
        public IActionResult TongHopPhuXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopPhuXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Phụ Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Phu/TongHopPhuXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopPhus(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/GetPhieuCanTongHopPhus/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp 2
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Phụ / Tổng Hợp 2", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Phụ / Tổng Hợp 2")]
        public IActionResult TongHop2PhuXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHop2PhuXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp 2 Phụ Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Phu/TongHop2PhuXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopPhus2(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/GetPhieuCanTongHopPhus2/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
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
            if (reportType == "ChiTietPhuXepKhuonView")
            {
                dataSource = await GetPhieuCanPhuXepKhuons(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopPhuXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopPhus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHop2PhuXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopPhus2(fromDate, toDate, xuongId);
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
