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

namespace PMS.Controllers.BaoCao.NguyenLieu
{
    [Authorize]
    public class BaoCaoNguyenLieuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoNguyenLieuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Chi Tiết", Func = "Xem Báo Cáo / Nguyên Liệu / Chi Tiết")]
        public IActionResult ChiTietNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Nguyên Liệu";
            return View("~/Views/BaoCao/NguyenLieu/ChiTietNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanChiTiets/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Nhà CC
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp NCC", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp NCC")]
        public IActionResult TongHopNhaCCNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhaCCNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhà Cung Cấp";
            return View("~/Views/BaoCao/NguyenLieu/TongHopNhaCCNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopNCCs(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanTongHopNCCs/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Bàn Cắt Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp BCT", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp BCT")]
        public IActionResult TongHopBanCatTietNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopBanCatTietNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Bàn Cắt Tiết";
            return View("~/Views/BaoCao/NguyenLieu/TongHopBanCatTietNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopBCTs(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanTongHopBCTs/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Phương Tiện
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp Ghe", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp Ghe")]
        public IActionResult TongHopPhuongTienNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopPhuongTienNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Phương Tiện";
            return View("~/Views/BaoCao/NguyenLieu/TongHopPhuongTienNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopPhuongTiens(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanTongHopPhuongTiens/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Kế Toán
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp Kế Toán", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp Kế Toán")]
        public IActionResult TongHopKeToanNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopKeToanNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Kế Toán";
            return View("~/Views/BaoCao/NguyenLieu/TongHopKeToanNguyenLieuView.cshtml");
        }
        #endregion
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietNguyenLieuView")
            {
                dataSource = await GetChiTiets(fromDate, toDate);
            }
            else if (reportType == "TongHopNhaCCNguyenLieuView")
            {
                dataSource = await GetPhieuCanTongHopNCCs(fromDate, toDate);
            }
            else if (reportType == "TongHopBanCatTietNguyenLieuView")
            {
                dataSource = await GetPhieuCanTongHopBCTs(fromDate, toDate);
            }
            else if (reportType == "TongHopPhuongTienNguyenLieuView")
            {
                dataSource = await GetPhieuCanTongHopPhuongTiens(fromDate, toDate);
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
