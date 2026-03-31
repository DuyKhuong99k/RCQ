using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Attrs;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Charts;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;
using PMS.Models;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Security.Policy;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Models.Repos.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using Models.Repos;
using System;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PMS.Controllers.HQ
{
    [Authorize]
    public class BaoCaoCanNguyenLieuHQController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoCanNguyenLieuHQController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Nhập Nguyên Liệu
        #region Chi Tiết Phiếu Cân Nhập Nguyên Liệu
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ", Func = "Xem Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ")]
        public IActionResult ChiTietNLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietNLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết NL Nhập";
            return View("~/Views/BaoCaoNguyenLieuHQ/ChiTietNLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTietPhieuCanNhapNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/GetChiTietPhieuCanNhapNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Sant Phẩm Phiếu Cân Nhập Nguyên Liệu
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ", Func = "Xem Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ")]
        public IActionResult TongHopSanPhamNLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopSanPhamNLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Sản Phẩm NL Nhập";
            return View("~/Views/BaoCaoNguyenLieuHQ/TongHopSanPhamNLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopSanPhamPhieuCanNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/GetTongHopSanPhamPhieuCanNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #endregion

        #region Xuất Nguyên Liệu
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Chi Tiết HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Chi Tiết HQ")]
        public IActionResult ChiTietXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/ChiTietXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTietPhieuCanXuatNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetChiTietPhieuCaXuatNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Sản Phẩm
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Tổng Hợp Sản Phẩm HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Tổng Hợp Sản Phẩm HQ")]
        public IActionResult TongHopSanPhamXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopSanPhamXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Sản Phẩm NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/TongHopSanPhamXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopSanPhamPhieuCanXuatNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetTongHopSanPhamPhieuCanXuatNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tồn kho
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Tồn Kho HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Tồn Kho HQ")]
        public IActionResult TonKhoXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TonKhoXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tồn Kho NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/TonKhoXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTonKhoSanPhamPhieuCanXuatNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetTonKhoSanPhamPhieuCanXuatNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tỷ Lệ Hao Hụt
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Tỷ Lệ Hao Hụt HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Tỷ Lệ Hao Hụt HQ")]
        public IActionResult TyLeHaoHutXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TyLeHaoHutXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tỷ Lệ Hao Hụt NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/TyLeHaoHutXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTyLeNguyenLieuHaoHut(DateTime? fromDate = null)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetTyLeNguyenLieuHaoHut/{fromDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion


        #region Khối Lượng Xuất Xưởng Theo Lô

        public class ListLo
        {

            public string MaLo { get; set; }

        }


        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ")]
        public IActionResult KhoiLuongXuatLoTheoXuongXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "KhoiLuongXuatLoTheoXuongXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            //var apiListLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetListLo";
            //using var helperListLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            //var listLo = helperListLo.GetAsync<IEnumerable<ListLo>>(HttpContext, apiListLoUrl);
            //ViewBag.listLo = listLo.Result.ToList();


            var apiListLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetListLo";

            using var helperListLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            var listLo = helperListLo
                            .GetAsync<IEnumerable<ListLo>>(HttpContext, apiListLoUrl)
                            .Result;

            ViewBag.listLo = listLo?.ToList() ?? new List<ListLo>();


            ViewBag.TitlePage = "Báo Cáo Khối Lượng Xuất Lô Theo Xưởng";
            return View("~/Views/BaoCaoNguyenLieuHQ/KhoiLuongXuatLoTheoXuongXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetKhoiLuongXuatLoTheoXuong(string maLo = null)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetKhoiLuongXuatLoTheoXuong/{maLo}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        public async Task<ActionResult> ReloadKhoiLuongXuatLoTheoXuong(string maLo)
        {

            if (maLo == "Chọn Lô" || maLo == null)
            {
                maLo = "'";
            }
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "KhoiLuongXuatLoTheoXuongXLNHQView")
            {
                dataSource = await GetKhoiLuongXuatLoTheoXuong(maLo);
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
        #endregion
        #endregion

        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietNLNHQView")
            {
                dataSource = await GetChiTietPhieuCanNhapNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopSanPhamNLNHQView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "ChiTietXLNHQView")
            {
                dataSource = await GetChiTietPhieuCanXuatNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopSanPhamXLNHQView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanXuatNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TonKhoXLNHQView")
            {
                dataSource = await GetTonKhoSanPhamPhieuCanXuatNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TyLeHaoHutXLNHQView")
            {
                dataSource = await GetTyLeNguyenLieuHaoHut(fromDate);
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
