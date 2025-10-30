using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Attrs;
using Syncfusion.EJ2.Base;
using static NuGet.Packaging.PackagingConstants;

namespace PMS.Controllers.BaoCao.Fillet
{
    [Authorize]
    public class TPFilletController : Controller
    {
         private readonly IHttpClientFactory _httpClientFactory;
        public TPFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Chi Tiết", Func = "Xem Báo Cáo / TPFillet / Chi Tiết")]
        public async Task<ActionResult> ChiTiet()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietTPFillet");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            return View("~/Views/BaoCao/Fillet/ChiTietTPFilletView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / TPFillet / Tổng Hợp Nhân Viên")]
        public async Task<ActionResult> TongHopNhanVien()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienTPFillet");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            return View("~/Views/BaoCao/Fillet/TongHopNhanVienTPFilletView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Tổng Hợp Thành Phẩm", Func = "Xem Báo Cáo / TPFillet / Tổng Hợp Thành Phẩm")]
        public async Task<ActionResult> TongHopThanhPham()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamTPFillet");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopThanhPhamTPFilletView.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietTPFillet")
            {
                dataSource = await GetPhieuCanChiTiets(fromDate, toDate);
            }
            else if (reportType == "TongHopNhanVienTPFillet")
            {
                dataSource = await GetPhieuCanTongHopNhanViens(fromDate, toDate);
            }
            else if (reportType == "TongHopThanhPhamTPFillet")
            {
                dataSource = await GetPhieuCanTonghopThanhPhams(fromDate, toDate);
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
        public async Task<IEnumerable<object>> GetPhieuCanChiTiets(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetPhieuCanChiTiets/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopNhanViens(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetPhieuCanTongHopNhanViens/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanTonghopThanhPhams(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetPhieuCanTonghopThanhPhams/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }

    }
}
