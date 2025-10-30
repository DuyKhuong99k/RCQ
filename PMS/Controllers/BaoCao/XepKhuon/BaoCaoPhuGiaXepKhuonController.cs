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
    public class BaoCaoPhuGiaXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoPhuGiaXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region chi tiết tai chế
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Phụ Gia / Chi Tiết", Func = "Xem Báo Cáo / Xếp Khuôn / Phụ Gia / Chi Tiết")]
        public IActionResult ChiTietPhuGiaXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietPhuGiaXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Phụ Gia Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/PhuGia/ChiTietPhuGiaXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanPhuGias(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/GetsChiTiet/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp Phụ Giat
        [CustomAuthorize(Fu = "Báo Cáo / Xếp Khuôn / Phụ Gia / Tổng Hợp", Func = "Xem Báo Cáo / Xếp Khuôn / Phụ Gia / Tổng Hợp")]
        public IActionResult TongHopPhuGiaXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopPhuGiaXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Phụ Gia Xếp Khuôn";
            return View("~/Views/BaoCao/XepKhuon/PhuGia/TongHopPhuGiaXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopPhuGia(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/GetsTongHop/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
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
            if (reportType == "ChiTietPhuGiaXepKhuonView")
            {
                dataSource = await GetPhieuCanPhuGias(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopPhuGiaXepKhuonView")
            {
                dataSource = await GetPhieuCanTongHopPhuGia(fromDate, toDate, xuongId);
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
