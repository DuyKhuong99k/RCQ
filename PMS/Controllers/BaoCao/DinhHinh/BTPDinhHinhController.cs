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

namespace PMS.Controllers.BaoCao.DinhHinh
{
    [Authorize]
    public class BTPDinhHinhController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BTPDinhHinhController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTP Định Hình / Chi Tiết", Func = "Xem Báo Cáo / BTP Định Hình / Chi Tiết")]
        public async Task<ActionResult> ChiTiet()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietBTPDinhHinh");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTP Định Hình | Chi Tiết";
            return View("~/Views/BaoCao/BTPDinhHinh/ChiTietBTPDinhHinh.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTP Định Hình / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / BTP Định Hình / Tổng Hợp Nhân Viên")]
        public async Task<ActionResult> TongHopNhanVien()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienBTPDinHinh");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTP Định Hình | Tổng Hợp Nhân Viên";
            return View("~/Views/BaoCao/BTPDinhHinh/TongHopNhanVienBTPDinhHinhView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTP Định Hình / Tổng Hợp Nhân Viên Phục Vụ", Func = "Xem Báo Cáo / BTP Định Hình / Tổng Hợp Nhân Viên Phục Vụ")]
        public async Task<ActionResult> TongHopNhanVienPhucVu()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienPVBTPDinHinh");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTP Định Hình | Tổng Hợp Nhân Viên Phục Vụ";
            return View("~/Views/BaoCao/BTPDinhHinh/TongHopNhanVienPVBTPDinhHinhView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTP Định Hình / Tổng Hợp Thành Phẩm", Func = "Xem Báo Cáo / BTP Định Hình / Tổng Hợp Thành Phẩm")]
        public async Task<ActionResult> TongHopThanhPham()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamBTPDinHinh");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTP Định Hình | Tổng Hợp Thành Phẩm";
            return View("~/Views/BaoCao/BTPDinhHinh/TongHopThanhPhamBTPDinhHinhView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / BTP Định Hình / Tổng Hợp Lô", Func = "Xem Báo Cáo / BTP Định Hình / Tổng Hợp Lô")]
        public async Task<ActionResult> TongHopLos()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopLoBTPDinHinh");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTP Định Hình | Tổng Hợp Lô";
            return View("~/Views/BaoCao/BTPDinhHinh/TongHopLoBTPDinhHinhView.cshtml");
        }


        [CustomAuthorize(Fu = "Báo Cáo / BTP Định Hình / Chi Tiết Phiếu Cân Chưa Sửa", Func = "Xem Báo Cáo / BTP Định Hình / Chi Tiết Phiếu Cân Chưa Sửa")]
        public async Task<ActionResult> ChiTietPhieuCanChuaSua()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietPhieuCanChuaSua");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BTP Định Hình | Phiếu Cân Chưa Sửa";
            return View("~/Views/BaoCao/BTPDinhHinh/ChiTietPhieuCanChuaSuaView.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true; //trạng tháng action trả về mặc định là true

            if (reportType == "ChiTietBTPDinhHinh")
            {
                dataSource = await GetChiTiets(fromDate, toDate,xuongId);
            }
            else if (reportType == "TongHopNhanVienBTPDinHinh")
            {
                dataSource = await GetTongHopNhanViens(fromDate, toDate,xuongId);
            }
            else if (reportType == "TongHopNhanVienPVBTPDinHinh")
            {
                dataSource = await GetTongHopNhanVienPhucVus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopThanhPhamBTPDinHinh")
            {
                dataSource = await GetTongHopThanhPhams(fromDate, toDate,xuongId);
            }
            else if (reportType == "TongHopLoBTPDinHinh")
            {
                dataSource = await GetTongHopLos(fromDate, toDate, xuongId);
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
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            //xuongId = "1";
#endif
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetChiTietsFromdateTodate/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

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
            //xuongId = "1";
#endif
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetTongHopNhanViens/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }

        public async Task<IEnumerable<object>> GetTongHopNhanVienPhucVus(DateTime? fromDate = null, DateTime? toDate = null, string? xuongId = null)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetTongHopNhanVienPhucVus/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }



        public async Task<IEnumerable<object>> GetTongHopThanhPhams(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetTongHopThanhPhams/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopLos(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetTongHopLos/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetChiTietPhieuCanChuaSuas/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
    }
}
