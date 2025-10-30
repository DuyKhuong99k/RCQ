using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Attrs;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PMS.Controllers.BaoCao.Fillet
{
    [Authorize]
    public class BTPFilletv2Controller : Controller
    {
         private readonly IHttpClientFactory _httpClientFactory;
        public BTPFilletv2Controller(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTPFilletv2 / Chi Tiết", Func = "Xem Báo Cáo / BTPFilletv2 / Chi Tiết")]
        public async Task<ActionResult> ChiTiet()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietBTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTPFillet | Chi Tiết";

            return View("~/Views/BaoCao/Fillet/ChiTietBTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTPFilletv2 / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / BTPFilletv2 / Tổng Hợp Nhân Viên")]
        public async Task<ActionResult> TongHopNhanVien()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienBTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTPFillet | Tổng Hợp Nhân Viên";
            return View("~/Views/BaoCao/Fillet/TongHopNhanVienBTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTPFilletv2 / Tổng Hợp Nhân Viên Phục Vụ", Func = "Xem Báo Cáo / BTPFilletv2 / Tổng Hợp Nhân Viên Phục Vụ")]
        public async Task<ActionResult> TongHopNhanVienPhucVu()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienPhucVuBTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTPFillet | Tổng Hợp Nhân Viên Phục Vụ";
            return View("~/Views/BaoCao/Fillet/TongHopNhanVienPhucVuBTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTPFilletv2 / Chi Tiết", Func = "Xem Báo Cáo / BTPFilletv2 / Chi Tiết")]
        public async Task<ActionResult> ChiTietPhieuCanChuaSua()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietPhieuCanChuaSua");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTPFillet | Phiếu Cân Chưa Sửa";
            return View("~/Views/BaoCao/Fillet/chitietphieucanchuasuaview.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTPFilletv2 / Tổng Hợp Thành Phẩm", Func = "Xem Báo Cáo / BTPFilletv2 / Tổng Hợp Thành Phẩm")]
        public async Task<ActionResult> TongHopThanhPham()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamBTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTPFillet | Tổng Hợp Thành Phẩm";
            return View("~/Views/BaoCao/Fillet/TongHopThanhPhamBTPFilletv2View.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTPFilletv2 / Tổng Hợp Lô", Func = "Xem Báo Cáo / BTPFilletv2 / Tổng Hợp Lô")]
        public async Task<ActionResult> TongHopLo()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopLoBTPFilletv2");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTPFillet | Tổng Hợp Lô";
            return View("~/Views/BaoCao/Fillet/TongHopLoBTPFilletv2View.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            var xuongId = "1"; // Mặc định là 1, có thể thay đổi theo yêu cầu
            bool success = true; //trạng tháng action trả về mặc định là true
            if (reportType == "ChiTietBTPFilletv2")
            {
                dataSource = await GetChiTiets(fromDate, toDate);
            }
            else if (reportType == "TongHopNhanVienBTPFilletv2")
            {
                dataSource = await GetTongHopNhanViens(fromDate, toDate);
            }
            else if (reportType == "TongHopNhanVienPhucVuBTPFilletv2")
            {
                dataSource = await GetTongHopPhucVus(fromDate, toDate);
            }
            else if (reportType == "TongHopThanhPhamBTPFilletv2")
            {
                dataSource = await GetTongHopThanhPhams(fromDate, toDate);
            }
            else if (reportType == "TongHopLoBTPFilletv2")
            {
                dataSource = await GetTongHopLos(fromDate, toDate);
            }
            else if (reportType == "ChiTietPhieuCanChuaSua")
            {
                dataSource = await GetChiTietPhieuCanChuaSuas(fromDate, toDate, xuongId);
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
        public async Task<IEnumerable<object>> GetChiTietPhieuCanChuaSuas(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            //xuongId = "1";
#endif
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetChiTietPhieuCanChuaSuas/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetChiTiets/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanViens(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            xuongId = "1";
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetTongHopNhanViens/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopPhucVus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            xuongId = "1";
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetTongHopPhucVus/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopThanhPhams(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            xuongId = "1";
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetTongHopThanhPhams/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopLos(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //fromDate = new DateTime(2022, 05, 25);//2022 - 05 - 25;
            //toDate = new DateTime(2022, 05, 25);
            xuongId = "1";
#endif
            xuongId = "1";
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetTongHopLos/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
    }
}
