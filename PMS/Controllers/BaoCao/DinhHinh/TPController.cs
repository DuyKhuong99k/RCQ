using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Attrs;
using Syncfusion.EJ2.Base;
using static NuGet.Packaging.PackagingConstants;

namespace PMS.Controllers.BaoCao.DinhHinh
{
    [Authorize]
    public class TPController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public TPController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Báo Cáo / TP Định Hình / Chi Tiết", Func = "Xem Báo Cáo / TP Định Hình / Chi Tiết")]
        public async Task<ActionResult> ChiTiet()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietTPDinhHinh");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/DinhHinh/ChiTietView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TP Định Hình / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / TP Định Hình / Tổng Hợp Nhân Viên")]
        public async Task<ActionResult> TongHopNhanVien()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienTPDinhHinh");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/DinhHinh/TongHopNhanVienTPDinhHinhView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / TP Định Hình / Tổng Hợp Thành Phẩm", Func = "Xem Báo Cáo / TP Định Hình / Tổng Hợp Thành Phẩm")]
        public async Task<ActionResult> TongHopLoaiThanhPham()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopLoaiThanhPham");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/DinhHinh/TongHopLoaiThanhPhamTPDinhHinhView.cshtml");
        }
        #region đánh giá định mức trên chi tiết
        [CustomAuthorize(Fu = "Báo Cáo / TP Định Hình / Tổng Hợp Đánh Giá Định Mức Trên Chi Tiết", Func = "Xem Báo Cáo / TP Định Hình / Tổng Hợp Đánh Giá Định Mức Trên Chi Tiết")]
        public async Task<ActionResult> TongHopDanhGiaDinhMucTrenChiTietView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopDanhGiaDinhMucTrenChiTietView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/DinhHinh/TongHopDanhGiaDinhMucTrenChiTietView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopDanhGiaDinhMucTrenChiTiet(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); //Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopDanhGiaDinhMucTrenChiTiet/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region đánh giá định mức
        [CustomAuthorize(Fu = "Báo Cáo / TP Định Hình / Tổng Hợp Đánh Giá Định Mức", Func = "Xem Báo Cáo / TP Định Hình / Tổng Hợp Đánh Giá Định Mức")]
        public async Task<ActionResult> TongHopDanhGiaDinhMucView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopDanhGiaDinhMucView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/DinhHinh/TongHopDanhGiaDinhMucView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopDanhGiaDinhMuc(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopDanhGiaDinhMuc/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp nhân viên phục vụ
        [CustomAuthorize(Fu = "Báo Cáo / TP Định Hình / Tổng Hợp Nhân Viên Phục Vụ", Func = "Xem Báo Cáo / TP Định Hình / Tổng Hợp Nhân Viên Phục Vụ")]
        public async Task<ActionResult> TongHopNhanVienPhucVuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienPhucVuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/DinhHinh/TongHopNhanVienPhucVuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVienPhucVu(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopNhanVienPhucVu/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region tổng hợp nhân viên bàn kiểm
        [CustomAuthorize(Fu = "Báo Cáo / TP Định Hình / Tổng Hợp Nhân Viên Bàn Kiểm", Func = "Xem Báo Cáo / TP Định Hình / Tổng Hợp Nhân Viên Bàn Kiểm")]
        public async Task<ActionResult> TongHopNhanVienBanKiemView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienBanKiemView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            return View("~/Views/BaoCao/DinhHinh/TongHopNhanVienBanKiemView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVienBanKiem(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopNhanVienBanKiem/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        [HttpPost]
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietTPDinhHinh")
            {
                dataSource = await GetChiTietsTPDinhHinh(fromDate, toDate);
            }
            else if (reportType == "TongHopNhanVienTPDinhHinh")
            {
                dataSource = await GetTongHopNhanVien(fromDate, toDate);
            }
            else if (reportType == "TongHopLoaiThanhPham")
            {
                dataSource = await GetTongHopLoaiThanhPhams(fromDate, toDate);
            }
            else if (reportType == "TongHopDanhGiaDinhMucTrenChiTietView")
            {
                dataSource = await GetTongHopDanhGiaDinhMucTrenChiTiet(fromDate, toDate);
            }
            else if (reportType == "TongHopDanhGiaDinhMucView")
            {
                dataSource = await GetTongHopDanhGiaDinhMuc(fromDate, toDate);
            }
            else if (reportType == "TongHopNhanVienPhucVuView")
            {
                dataSource = await GetTongHopNhanVienPhucVu(fromDate, toDate);
            }
            else if (reportType == "TongHopNhanVienBanKiemView")
            {
                dataSource = await GetTongHopNhanVienBanKiem(fromDate, toDate);
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
        // GET: TPController/Create


        public async Task<IEnumerable<object>> GetChiTietsTPDinhHinh(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            xuongId = "1";
#endif
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetChiTietsFromdateTodate/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVien(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            xuongId = "1";
#endif
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopNhanVien/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopLoaiThanhPhams(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = "1")
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
#if DEBUG
            xuongId = "1";
#endif
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopLoaiThanhPhams/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        //public async Task<IActionResult> GetTongHopLoaiThanhPhamsByMaTP(DateTime? fromDate = null, DateTime? toDate = null, string maTP)
        //{
        //    if (fromDate == null) fromDate = DateTime.Now;
        //    if (toDate == null) toDate = DateTime.Now;
        //    var xuongId = HttpContext.Session.GetString("XuongId");
        //    IEnumerable<object> dataSource = ViewBag.dataSource;
        //    var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
        //    if (dataSource == null)
        //    {
        //        var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopLoaiThanhPhams/{fromDate?.ToString("yyyy-MM-dd")},{toDate?.ToString("yyyy-MM-dd")},{xuongId}";
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //        ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
        //        dataSource = ViewBag.dataSource;
        //        return Json(new
        //        {
        //            isSuccess = true,
        //            //Mesages = "Lỗi!"
        //        });
        //    }
        //    else
        //    {
        //        return Json(new
        //        {
        //            isSuccess = false,
        //            Mesages = "Không có dữ liệu!"
        //        });
        //    }

        //}
        // GET: TPController

        // POST: TPController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

