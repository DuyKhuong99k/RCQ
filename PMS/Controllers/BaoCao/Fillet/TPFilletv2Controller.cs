using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Attrs;
using System.Globalization;
using System.Net.Http.Headers;

namespace PMS.Controllers.BaoCao.Fillet
{
    [Authorize]
    public class TPFilletv2Controller : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TPFilletv2Controller(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFilletv2 / Chi Tiết", Func = "Xem Báo Cáo / TPFilletv2 / Chi Tiết")]
        public async Task<ActionResult> ChiTiet()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TPFillet | Chi Tiết";
            return View("~/Views/BaoCao/Fillet/ChiTietTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFilletv2 / Tổng Hợp Thành Phẩm", Func = "Xem Báo Cáo / TPFilletv2 / Tổng Hợp Thành Phẩm")]
        public async Task<ActionResult> TongHopThanhPham()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopthanhPhamTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TPFillet | Tổng Hợp Thành Phẩm";
            return View("~/Views/BaoCao/Fillet/TongHopthanhPhamTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFilletv2 / Tổng Hợp Lô", Func = "Xem Báo Cáo / TPFilletv2 / Tổng Hợp Lô")]
        public async Task<ActionResult> TongHopLo()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopLoTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TPFillet | Tổng Hợp Lô";
            return View("~/Views/BaoCao/Fillet/TongHopLoTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFilletv2 / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / TPFilletv2 / Tổng Hợp Nhân Viên")]
        public async Task<ActionResult> TongHopNhanVien()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TPFillet | Tổng Hợp Nhân Viên";
            return View("~/Views/BaoCao/Fillet/TongHopNhanVienTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFilletv2 / Tổng Hợp Nhân Viên 2", Func = "Xem Báo Cáo / TPFilletv2 / Tổng Hợp Nhân Viên 2")]
        public async Task<ActionResult> TongHopNhanVien2()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVien2TPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            return View("~/Views/BaoCao/Fillet/TongHopNhanVien2TPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFilletv2 / Tổng Hợp Nhân Viên Phục Vụ", Func = "Xem Báo Cáo / TPFilletv2 / Tổng Hợp Nhân Viên Phục Vụ")]
        public async Task<ActionResult> TongHopNhanVienPhucVuTPFilletv2View()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienPhucVuTPFilletv2View");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TPFillet | Tổng Hợp Nhân Viên Phục Vụ";
            return View("~/Views/BaoCao/Fillet/TongHopNhanVienPhucVuTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFilletv2 / Tổng Hợp Giờ Làm Việc", Func = "Xem Báo Cáo / TPFilletv2 / Tổng Hợp Giờ Làm Việc")]
        public async Task<ActionResult> GetTongHopGioLamViecTPFilletv2View()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "GetTongHopGioLamViecTPFilletv2View");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TPFillet | Tổng Hợp Giờ Làm Việc";
            return View("~/Views/BaoCao/Fillet/GetTongHopGioLamViecTPFilletv2View.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietTPFilletv2")
            {
                dataSource = await GetChiTiets2HN(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopthanhPhamTPFilletv2")
            {
                dataSource = await GetTongHopLoaiThanhPhamsHN(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopNhanVienTPFilletv2")
            {
                dataSource = await GetTongHopNhanViensHN(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopNhanVien2TPFilletv2")
            {
                dataSource = await GetTongHopNhanVien2HoangLong(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopNhanVienPhucVuTPFilletv2View")
            {
                dataSource = await GetTongHopNhanVienPhucVu(fromDate, toDate, xuongId);
            }
            else if (reportType == "GetTongHopGioLamViecTPFilletv2View")
            {
                dataSource = await GetTongHopGioLamViec(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopLoTPFilletv2")
            {
                dataSource = await GetTongHopLo(fromDate, toDate, xuongId);
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
        public async Task<IEnumerable<object>> GetChiTiets2HN(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
            if (xuongId == null) xuongId = "1";

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetChiTiets2HN/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopLoaiThanhPhamsHN(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
            if (xuongId == null) xuongId = "1";
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopLoaiThanhPhamsHN/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopLo(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
            if (xuongId == null) xuongId = "1";
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopLo/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanViensHN(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
            if (xuongId == null) xuongId = "1";
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopNhanViensHN/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVien2HoangLong(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
            if (xuongId == null) xuongId = "1";
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopNhanVien2HoangLong/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVienPhucVu(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
            if (xuongId == null) xuongId = "1";
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopNhanVienPhucVu/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopGioLamViec(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
            if (xuongId == null) xuongId = "1";
            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopGioLamViec/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
    }
}
