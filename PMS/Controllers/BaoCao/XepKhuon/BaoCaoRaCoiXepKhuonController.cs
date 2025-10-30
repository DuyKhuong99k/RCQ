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
    public class BaoCaoRaCoiXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoRaCoiXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Ra Cối / Chi Tiết", Func = "Xem Báo Cáo / Xếp Khuôn / Ra Cối / Chi Tiết")]
        public IActionResult ChiTietRaCoiXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietRaCoiXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Ra Cối";
            return View("~/Views/BaoCao/XepKhuon/RaCoi/ChiTietRaCoiXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTietRaCois(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanRaCois/GetChiTietRaCois/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Cối
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Ra Cối / Tổng Hợp Cối", Func = "Xem Báo Cáo / Xếp Khuôn / Ra Cối / Tổng Hợp Cối")]
        public IActionResult TongHopCoiRaCoiXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopCoiRaCoiXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Cối Ra Cối";
            return View("~/Views/BaoCao/XepKhuon/RaCoi/TongHopCoiRaCoiXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopCoiRaCois(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanRaCois/GetTongHopCoiRaCois/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Nhân Viên
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Ra Cối / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / Xếp Khuôn / Ra Cối / Tổng Hợp Nhân Viên")]
        public IActionResult TongHopNhanVienCoiXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienCoiXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên Ra Cối";
            return View("~/Views/BaoCao/XepKhuon/RaCoi/TongHopNhanVienCoiXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVienRaCois(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanRaCois/GetTongHopNhanVienRaCois/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Tỷ Lệ Tăng Trọng Cối
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Ra Cối / Tổng Hợp Tỷ Lệ Tăng Trọng Cối", Func = "Xem Báo Cáo / Xếp Khuôn / Ra Cối / Tổng Hợp Tỷ Lệ Tăng Trọng Cối")]
        public IActionResult TongHopTyLeTangTrongCoiXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopTyLeTangTrongCoiXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Tỷ Lệ Tăng Trọng Cối";
            return View("~/Views/BaoCao/XepKhuon/RaCoi/TongHopTyLeTangTrongCoiXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopTyLeTangTrongCois(DateTime? toDate = null)
        {
            if (toDate == null) toDate = DateTime.Now;
            //decimal chiSo = AppViewModels.AppViewModel.Instance.ChiSoTyLeTangTrongRaCoi;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanRaCois/GetTongHopTyLeTangTrongCoiRaCois/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate,string? xuongId = null)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietRaCoiXepKhuonView")
            {
                dataSource = await GetChiTietRaCois(fromDate, toDate,xuongId);
            }
            else if (reportType == "TongHopCoiRaCoiXepKhuonView")
            {
                dataSource = await GetTongHopCoiRaCois(fromDate, toDate,xuongId);
            }
            else if (reportType == "TongHopNhanVienCoiXepKhuonView")
            {
                dataSource = await GetTongHopNhanVienRaCois(fromDate, toDate,xuongId);
            }
            else if (reportType == "TongHopTyLeTangTrongCoiXepKhuonView")
            {
                dataSource = await GetTongHopTyLeTangTrongCois(toDate);
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
