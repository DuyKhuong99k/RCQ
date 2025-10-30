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

namespace PMS.Controllers.BaoCao.NhanSu_KeToan
{
    [Authorize]
    public class BaoCaoNhanSu_KeToanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoNhanSu_KeToanController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Toàn Công Ty
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Toàn Công Ty", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Toàn Công Ty")]
        public IActionResult ToanCongTyNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ToanCongTyNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Toàn Công Ty Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/ToanCongTyNhanSuKeToanView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanKeToansPV(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanKeToans/GetPhieuCanKeToansPV/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Nhân Viên - Toàn Công Ty
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Nhân Viên - Toàn Công Ty", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Nhân Viên - Toàn Công Ty")]
        public IActionResult NhanVienToanCongTyNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhanVienToanCongTyNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Nhân Viên Toàn Công Ty Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/NhanVienToanCongTyNhanSuKeToanView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanKeToansNhanVienPV(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanKeToans/GetPhieuCanKeToansNhanVienPV/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Định Hình
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Định Hình", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Định Hình")]
        public IActionResult DinhHinhNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "DinhHinhNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Định Hình Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/DinhHinhNhanSuKeToanView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopSanPhamPhieuCanDinhHinhsPV(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanKeToans/GetTongHopSanPhamPhieuCanDinhHinhsPV/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Fillet
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Fillet", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Fillet")]
        public IActionResult FilletNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "FilletNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Fillet Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/FilletNhanSuKeToanView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanFilletsPV(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanKeToans/GetPhieuCanFilletsPV/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Xếp Khuôn Trực Tiếp", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Xếp Khuôn Trực Tiếp")]
        public IActionResult XepKhuonTrucTiepNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XepKhuonTrucTiepNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Xếp Khuôn Trực Tiếp Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/XepKhuonTrucTiepNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Xếp Khuôn Gián Tiếp", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Xếp Khuôn Gián Tiếp")]
        public IActionResult XepKhuonGianTiepNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XepKhuonGianTiepNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Xếp Khuôn Gián Tiếp Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/XepKhuonGianTiepNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Kiểm Định Hình", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Kiểm Định Hình")]
        public IActionResult KiemDinhHinhNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "KiemDinhHinhNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Kiểm Định Hình Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/KiemDinhHinhNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Sơ Chế", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Sơ Chế")]
        public IActionResult SoCheNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "SoCheNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Sơ Chế Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/SoCheNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Kiểm Sơ Chế", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Kiểm Sơ Chế")]
        public IActionResult KiemSoCheNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "KiemSoCheNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Kiểm Sơ Chế Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/KiemSoCheNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Phục Vụ Định Hình", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Phục Vụ Định Hình")]
        public IActionResult PhucVuDinhHinhNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhucVuDinhHinhNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Phục Vụ Định Hình Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/PhucVuDinhHinhNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Phụ Fillet", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Phụ Fillet")]
        public IActionResult PhuFilletNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhuFilletNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Phụ Fillet Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/PhuFilletNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Bao Tử", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Bao Tử")]
        public IActionResult BaoTuNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BaoTuNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Bao Tử Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/BaoTuNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Phụ Phẩm", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Phụ Phẩm")]
        public IActionResult PhuPhamNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhuPhamNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Phụ Phẩm Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/PhuPhamNhanSuKeToanView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Nhân Sự - Kế Toán / Lạng Da", Func = "Xem Báo Cáo / Nhân Sự - Kế Toán / Lạng Da")]
        public IActionResult LangDaNhanSuKeToanView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "LangDaNhanSuKeToanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Lạng Da Nhân Sự - Kế Toán";
            return View("~/Views/BaoCao/NhanSu-KeToan/LangDaNhanSuKeToanView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(DateTime? fromDate = null, DateTime? toDate = null, int? khuVucId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanKeToans/GetTongHopSanPhamPhieuCanKiemDinhhinhsPV/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{khuVucId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ToanCongTyNhanSuKeToanView")
            {
                dataSource = await GetPhieuCanKeToansPV(fromDate, toDate);
            }
            else if (reportType == "NhanVienToanCongTyNhanSuKeToanView")
            {
                dataSource = await GetPhieuCanKeToansNhanVienPV(fromDate, toDate);
            }
            else if (reportType == "DinhHinhNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanDinhHinhsPV(fromDate, toDate);
            }
            else if (reportType == "FilletNhanSuKeToanView")
            {
                dataSource = await GetPhieuCanFilletsPV(fromDate, toDate);
            }
            else if (reportType == "XepKhuonTrucTiepNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 8);
            }
            else if (reportType == "XepKhuonGianTiepNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 7);
            }
            else if (reportType == "KiemDinhHinhNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 2);
            }
            else if (reportType == "SoCheNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 3);
            }
            else if (reportType == "KiemSoCheNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 4);
            }
            else if (reportType == "PhucVuDinhHinhNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 5);
            }
            else if (reportType == "PhuFilletNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 6);
            }
            else if (reportType == "BaoTuNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 10);
            }
            else if (reportType == "PhuPhamNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 11);
            }
            else if (reportType == "LangDaNhanSuKeToanView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(fromDate, toDate, 12);
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
