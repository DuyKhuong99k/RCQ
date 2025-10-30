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
    public class BaoCaoSanPhamChinhXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoSanPhamChinhXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết")]
        public IActionResult ChiTietChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/ChiTietChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanChinhXepKhuonsByFromDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanChinhXepKhuonsByFromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region chi tiết nhân viên
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết Nhân Viên", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết Nhân Viên")]
        public IActionResult ChiTietNhanVienChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietNhanVienChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Nhân Viên Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/ChiTietNhanVienChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopNhanVienFromdateToDates(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHopNhanVienFromdateToDates/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp nhân viên
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Nhân Viên")]
        public IActionResult TongHopNhanVienChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/TongHopNhanVienChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopNhanViens2FromDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHopNhanViens2FromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp 2
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp 2", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp 2")]
        public IActionResult TongHop2ChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHop2ChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp 2 Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/TongHop2ChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHops2FromDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHops2FromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region chi tiết cối
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết Cối", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết Cối")]
        public IActionResult ChiTietCoiChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietCoiChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Cối Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/ChiTietCoiChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopCoisFromDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHopCoisFromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region chi tiết cối chưa quay
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết Cối Chưa Quay", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Chi Tiết Cối Chưa Quay")]
        public IActionResult ChiTietCoiChuaQuayChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietCoiChuaQuayChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Cối Chưa Quay Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/ChiTietCoiChuaQuayChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopCoisChuaQuayFromDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHopCoisChuaQuayFromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp cối
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Cối", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Cối")]
        public IActionResult TongHopCoiChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopCoiChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Cối Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/TongHopCoiChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopCois2FromDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHopCois2FromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp chi tiết cối
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Chi Tiết Cối", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Chi Tiết Cối")]
        public IActionResult TongHopChiTietCoiChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopChiTietCoiChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Chi Tiết Cối Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/TongHopChiTietCoiChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopChiTietCoisFromDateToDate(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHopChiTietCoisFromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp cối xưởng
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Cối Xưởng", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Cối Xưởng")]
        public IActionResult TongHopCoiXuongChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopCoiXuongChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Cối Xưởng Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/TongHopCoiXuongChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopCoisXuongFromDateToDate(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetTongHopCoisXuongFromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp thời gian sắp ra cối xưởng
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Thời Gian Sắp Ra Cối Xưởng", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Thời Gian Sắp Ra Cối Xưởng")]
        public IActionResult TongHopThoiGianSapRaCoiXuongChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThoiGianSapRaCoiXuongChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Thời Gian Sắp Ra Cối Xưởng Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/TongHopThoiGianSapRaCoiXuongChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopCoiGanRaFromDateToDate(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetTongHopCoiGanRaFromDateToDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp thời gian lượt ra cối xưởng
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Thời Gian Lượt Ra Cối Xưởng", Func = "Xem Báo Cáo / Xếp Khuôn / Sản Phẩm Chính / Tổng Hợp Thời Gian Lượt Ra Cối Xưởng")]
        public IActionResult TongHopThoiGianLuotRaCoiXuongChinhXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThoiGianLuotRaCoiXuongChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Thời Gian Lượt Ra Cối Xưởng Chính Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/Chinh/TongHopThoiGianLuotRaCoiXuongChinhXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopThoiGianLuotRaCoiFromDate(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetTongHopThoiGianLuotRaCoiFromDate/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
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
            if (reportType == "ChiTietChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanChinhXepKhuonsByFromDateToDate(fromDate, toDate,xuongId);
            }
            else if (reportType == "ChiTietNhanVienChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopNhanVienFromdateToDates(fromDate, toDate,xuongId);
            }
            else if (reportType == "TongHopNhanVienChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopNhanViens2FromDateToDate(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHop2ChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHops2FromDateToDate(fromDate, toDate, xuongId);
            }
            else if (reportType == "ChiTietCoiChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopCoisFromDateToDate(fromDate, toDate, xuongId);
            }
            else if (reportType == "ChiTietCoiChuaQuayChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopCoisChuaQuayFromDateToDate(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopCoiChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopCois2FromDateToDate(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopChiTietCoiChinhXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopChiTietCoisFromDateToDate(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopCoiXuongChinhXepKhuonView")
            {
                dataSource = await GetTongHopCoisXuongFromDateToDate(fromDate, toDate);
            }
            else if (reportType == "TongHopThoiGianSapRaCoiXuongChinhXepKhuonView")
            {
                dataSource = await GetTongHopCoiGanRaFromDateToDate(fromDate, toDate);
            }
            else if (reportType == "TongHopThoiGianLuotRaCoiXuongChinhXepKhuonView")
            {
                dataSource = await GetTongHopThoiGianLuotRaCoiFromDate(fromDate, toDate);
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
