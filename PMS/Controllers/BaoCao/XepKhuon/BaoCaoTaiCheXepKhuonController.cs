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
    public class BaoCaoTaiCheXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoTaiCheXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết ra đông
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Tái Chế / Chi Tiết Rã Đông", Func = "Xem Báo Cáo / Xếp Khuôn / Tái Chế / Chi Tiết Rã Đông")]
        public IActionResult ChiTietRaDongTaiCheXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietRaDongTaiCheXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Tái Chế Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/TaiChe/ChiTietRaDongTaiCheXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanRaDongXepKhuonsByDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/GetPhieuCanRaDongXepKhuonsByDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region chi tiết tai chế
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Tái Chế / Chi Tiết Tái Chế", Func = "Xem Báo Cáo / Xếp Khuôn / Tái Chế / Chi Tiết Tái Chế")]
        public IActionResult ChiTietTaiCheXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietTaiCheXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Tái Chế Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/TaiChe/ChiTietTaiCheXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTaiChes(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/GetPhieuCanTaiChes/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp tái chết
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Tái Chế / Tổng Hợp Tái Chế", Func = "Xem Báo Cáo / Xếp Khuôn / Tái Chế / Tổng Hợp Tái Chế")]
        public IActionResult TongHopTaiCheXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopTaiCheXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Tái Chế Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/TaiChe/TongHopTaiCheXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopTaiChe(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/GetPhieuCanTongHopTaiChe/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
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
            if (reportType == "ChiTietRaDongTaiCheXepKhuonView")
            {
                dataSource = await GetPhieuCanRaDongXepKhuonsByDateToDate(fromDate, toDate, xuongId);
            }
            else if (reportType == "ChiTietTaiCheXepKhuonView")
            {
                dataSource = await GetPhieuCanTaiChes(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopTaiCheXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopTaiChe(fromDate, toDate, xuongId);
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
