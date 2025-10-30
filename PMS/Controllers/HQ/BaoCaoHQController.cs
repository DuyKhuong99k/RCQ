using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Attrs;
using Syncfusion.EJ2.Base;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMS.Controllers.HQ
{
    [Authorize]
    public class BaoCaoHQController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoHQController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region CHi Tiết
        [CustomAuthorize(Fu = "Báo Cáo HQ / Chi Tiết", Func = "Xem Báo Cáo HQ / Chi Tiết")]
        public IActionResult ChiTietHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết";
            return View("~/Views/BaoCaoHQ/ChiTietHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetChiTiets/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp NV
        [CustomAuthorize(Fu = "Báo Cáo HQ / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo HQ / Tổng Hợp Nhân Viên")]
        public IActionResult TongHopNhanVienHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên";
            return View("~/Views/BaoCaoHQ/TongHopNhanVienHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopNhanViens(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopNhanViens/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp tp
        [CustomAuthorize(Fu = "Báo Cáo HQ / Tổng Hợp Thành Phẩm", Func = "Xem Báo Cáo HQ / Tổng Hợp Thành Phẩm")]
        public IActionResult TongHopThanhPhamHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên";
            return View("~/Views/BaoCaoHQ/TongHopNhanVienHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopThanhPhams(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhams/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietHQView")
            {
                dataSource = await GetChiTiets(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopNhanVienHQView")
            {
                dataSource = await GetTongHopNhanViens(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopThanhPhamHQView")
            {
                dataSource = await GetTongHopThanhPhams(fromDate, toDate, xuongId);
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
