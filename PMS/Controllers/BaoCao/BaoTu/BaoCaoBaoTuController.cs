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

namespace PMS.Controllers.BaoCao.BaoTu
{
    [Authorize]
    public class BaoCaoBaoTuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoBaoTuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region chi tiết
        [CustomAuthorize(Fu = "Báo Cáo / Bao Tử Cá / Chi Tiết", Func = "Xem Báo Cáo / Bao Tử Cá / Chi Tiết")]
        public IActionResult ChiTietBaoTuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietBaoTuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Bao Tử";
            return View("~/Views/BaoCao/BaoTu/ChiTietBaoTuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanBaoTus(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/GetsChiTiet/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp Bao Tửt
        [CustomAuthorize(Fu = "Báo Cáo / Bao Tử Cá / Tổng Hợp", Func = "Xem Báo Cáo / Bao Tử Cá / Tổng Hợp")]
        public IActionResult TongHopBaoTuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopBaoTuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Bao Tử";
            return View("~/Views/BaoCao/BaoTu/TongHopBaoTuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopBaoTu(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/GetTongHops/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
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
            if (reportType == "ChiTietBaoTuView")
            {
                dataSource = await GetPhieuCanBaoTus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopBaoTuView")
            {
                dataSource = await GetPhieuCanTongHopBaoTu(fromDate, toDate, xuongId);
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
