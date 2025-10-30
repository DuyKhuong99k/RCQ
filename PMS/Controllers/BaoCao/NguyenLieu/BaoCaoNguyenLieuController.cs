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

namespace PMS.Controllers.BaoCao.NguyenLieu
{
    [Authorize]
    public class BaoCaoNguyenLieuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoNguyenLieuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Chi Tiết", Func = "Xem Báo Cáo / Nguyên Liệu / Chi Tiết")]
        public IActionResult ChiTietNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Nguyên Liệu";
            return View("~/Views/BaoCao/NguyenLieu/ChiTietNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanChiTiets/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Nhà CC
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp NCC", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp NCC")]
        public IActionResult TongHopNhaCCNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhaCCNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhà Cung Cấp";
            return View("~/Views/BaoCao/NguyenLieu/TongHopNhaCCNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopNCCs(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanTongHopNCCs/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Bàn Cắt Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp BCT", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp BCT")]
        public IActionResult TongHopBanCatTietNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopBanCatTietNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Bàn Cắt Tiết";
            return View("~/Views/BaoCao/NguyenLieu/TongHopBanCatTietNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopBCTs(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanTongHopBCTs/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Phương Tiện
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp Ghe", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp Ghe")]
        public IActionResult TongHopPhuongTienNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopPhuongTienNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Phương Tiện";
            return View("~/Views/BaoCao/NguyenLieu/TongHopPhuongTienNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopPhuongTiens(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanTongHopPhuongTiens/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Kế Toán
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp Kế Toán", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp Kế Toán")]
        public IActionResult TongHopKeToanNguyenLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopKeToanNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Kế Toán";
            return View("~/Views/BaoCao/NguyenLieu/TongHopKeToanNguyenLieuView.cshtml");
        }
        #endregion
        #region Tổng Hợp Lô
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Tổng Hợp Lô", Func = "Xem Báo Cáo / Nguyên Liệu / Tổng Hợp Lô")]
        public IActionResult TongHopLoView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopLoView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Lô";
            return View("~/Views/BaoCao/NguyenLieu/TongHopLoView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopLos(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCanTongHopLos/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Thành Phẩm 2 Hoàng Long (Phụ Phẩm)
        [CustomAuthorize(Fu = "Báo Cáo / Thành Phẩm 2 / Chi Tiết", Func = "Xem Báo Cáo / Thành Phẩm 2 / Chi Tiết")]
        public IActionResult ChiTietThanhPham2View()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietThanhPham2View");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết";
            return View("~/Views/BaoCao/TP2/Chitiettp2.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanChiTietThanhPham2(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetChiTietThanhPham2s/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        [CustomAuthorize(Fu = "Báo Cáo / Thành Phẩm 2 / Tổng Hợp Sản Phẩm", Func = "Xem Báo Cáo / Thành Phẩm 2 / Tổng Hợp Sản Phẩm")]
        public IActionResult TongHopSanPhamThanhPham2View()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopSanPhamThanhPham2View");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Sản Phẩm";
            return View("~/Views/BaoCao/TP2/tonghopsptp2.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopSanPhamThanhPham2(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetTongHopSanPhamThanhPham2/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        [CustomAuthorize(Fu = "Báo Cáo / Thành Phẩm 2 / Tỷ Lệ Thu Hồi", Func = "Xem Báo Cáo / Thành Phẩm 2 / Tỷ Lệ Thu Hồi")]
        public IActionResult TyLeThuHoiThanhPham2View()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TyLeThuHoiThanhPham2View");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tỷ Lệ Thu Hồi";
            return View("~/Views/BaoCao/TP2/tonghoptylethuhoitp2.cshtml");
        }
        public async Task<IEnumerable<object>> GetTyLeThuHoiThanhPham2(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            string xuongId = HttpContext.Session.GetString("XuongId");
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetTyLeThuHoiThanhPham2/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion


        #region Phieu Cân Không Ghi Nhận Trọng Lương
        [CustomAuthorize(Fu = "Báo Cáo / Nguyên Liệu / Phiếu Cân Không Thể Ghi Nhận", Func = "Xem Báo Cáo / Nguyên Liệu / Phiếu Cân Không Thể Ghi Nhận")]
        public IActionResult ChiTietPhieuCanKhongGhiNhanDuLieuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietPhieuCanKhongGhiNhanDuLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Phiếu Cân Không Ghi Nhận Dữ Liệu";
            return View("~/Views/BaoCao/NguyenLieu/ChiTietPhieuCanKhongGhiNhanDuLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTietPhieuCanKhongGhiNhanDuLieu(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuKhongGhiNhan/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion

        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietNguyenLieuView")
            {
                dataSource = await GetChiTiets(fromDate, toDate);
            }
            else if (reportType == "TongHopNhaCCNguyenLieuView")
            {
                dataSource = await GetPhieuCanTongHopNCCs(fromDate, toDate);
            }
            else if (reportType == "TongHopBanCatTietNguyenLieuView")
            {
                dataSource = await GetPhieuCanTongHopBCTs(fromDate, toDate);
            }
            else if (reportType == "TongHopPhuongTienNguyenLieuView")
            {
                dataSource = await GetPhieuCanTongHopPhuongTiens(fromDate, toDate);
            }
            else if (reportType == "TongHopLoView")
            {
                dataSource = await GetPhieuCanTongHopLos(fromDate, toDate);
            }
            else if (reportType == "ChiTietThanhPham2View")
            {
                dataSource = await GetPhieuCanChiTietThanhPham2(fromDate, toDate);
            }
            else if (reportType == "TongHopSanPhamThanhPham2View")
            {
                dataSource = await GetTongHopSanPhamThanhPham2(fromDate, toDate);
            }
            else if (reportType == "TyLeThuHoiThanhPham2View")
            {
                dataSource = await GetTyLeThuHoiThanhPham2(fromDate, toDate);
            }
            else if (reportType == "ChiTietPhieuCanKhongGhiNhanDuLieuView")
            {
                dataSource = await GetChiTietPhieuCanKhongGhiNhanDuLieu(fromDate, toDate);
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
