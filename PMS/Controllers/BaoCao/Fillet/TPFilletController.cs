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
            ViewBag.TitlePage = "Fillet | Chi Tiết";
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
            ViewBag.TitlePage = "Fillet | Tổng Hợp Nhân Viên";
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
            ViewBag.TitlePage = "Fillet | Tổng Hợp Thành Phẩm";
            return View("~/Views/BaoCao/Fillet/TongHopThanhPhamTPFilletView.cshtml");
        }

        #region Xẻ Bướm
        #region BTP
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Chi Tiết BTP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Chi Tiết BTP Xẻ Bướm")]
        public async Task<ActionResult> ChiTietBTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietBTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/ChiTietBTPXeBuomView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Thành Phẩm BTP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Thành Phẩm BTP Xẻ Bướm")]
        public async Task<ActionResult> TongHopThanhPhamBTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamBTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopThanhPhamBTPXeBuomView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Lô Thành Phẩm Size BTP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Lô Thành Phẩm Size BTP Xẻ Bướm")]
        public async Task<ActionResult> TongHopLoThanhPhamSizeBTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopLoThanhPhamSizeBTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopLoThanhPhamSizeBTPXeBuomView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Tổng Hợp Nhân Viên BTP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Tổng Hợp Nhân Viên BTP Xẻ Bướm")]
        public async Task<ActionResult> TongHopNhanVienBTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienBTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopNhanVienBTPXeBuomView.cshtml");
        }

        #region  xử lý data
        public async Task<IEnumerable<object>> GetChiTietBTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetChiTietBTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopThanhPhamBTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopThanhPhamBTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopLTPSBTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopLTPSBTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
         public async Task<IEnumerable<object>> GetTongHopNhanVienBTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopNhanVienBTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        #endregion
        


        #endregion

        #region TP
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Chi Tiết TP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Chi Tiết TP Xẻ Bướm")]
        public async Task<ActionResult> ChiTietTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/ChiTietTPXeBuomView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Thành Phẩm TP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Thành Phẩm TP Xẻ Bướm")]
        public async Task<ActionResult> TongHopThanhPhamTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopThanhPhamTPXeBuomView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Lô Thành Phẩm Size TP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Lô Thành Phẩm Size TP Xẻ Bướm")]
        public async Task<ActionResult> TongHopLoThanhPhamSizeTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopLoThanhPhamSizeTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopLoThanhPhamSizeTPXeBuomView.cshtml");
        }

        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Tổng Hợp Nhân Viên TP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Tổng Hợp Nhân Viên TP Xẻ Bướm")]
        public async Task<ActionResult> TongHopNhanVienTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopNhanVienTPXeBuomView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TPFillet / Tổng Hợp Định Mức TP Xẻ Bướm", Func = "Xem Báo Cáo / TPFillet / Tổng Hợp Định Mức TP Xẻ Bướm")]
        public async Task<ActionResult> TongHopDinhMucTPXeBuom()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopDinhMucTPXeBuomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/Fillet/TongHopDinhMucTPXeBuomView.cshtml");
        }
        #region  xử lý data
        public async Task<IEnumerable<object>> GetChiTietTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetChiTietTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopLTPSTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopLTPSTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopThanhPhamTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopThanhPhamTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVienTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopNhanVienTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopDinhMucTPXeBuoms(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;

            if (toDate == null) toDate = DateTime.Now;

            IEnumerable<object>
                dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopDinhMucTPXeBuoms/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;

            }

            return dataSource;
        }
        #endregion
        
        #endregion
        #endregion
        [HttpPost]
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string xuongId)
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
            else if (reportType == "ChiTietBTPXeBuomView")
            {
                dataSource = await GetChiTietBTPXeBuoms(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopThanhPhamBTPXeBuomView")
            {
                dataSource = await GetTongHopThanhPhamBTPXeBuoms(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopLoThanhPhamSizeBTPXeBuomView")
            {
                dataSource = await GetTongHopLTPSBTPXeBuoms(fromDate, toDate, xuongId);
            }
             else if (reportType == "TongHopNhanVienBTPXeBuomView")
            {
                dataSource = await GetTongHopNhanVienBTPXeBuoms(fromDate, toDate, xuongId);
            }
            else if (reportType == "ChiTietTPXeBuomView")
            {
                dataSource = await GetChiTietTPXeBuoms(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopThanhPhamTPXeBuomView")
            {
                dataSource = await GetTongHopThanhPhamTPXeBuoms(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopLoThanhPhamSizeTPXeBuomView")
            {
                dataSource = await GetTongHopLTPSTPXeBuoms(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopNhanVienTPXeBuomView")
            {
                dataSource = await GetTongHopNhanVienTPXeBuoms(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopDinhMucTPXeBuomView")
            {
                dataSource = await GetTongHopDinhMucTPXeBuoms(fromDate, toDate, xuongId);
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
