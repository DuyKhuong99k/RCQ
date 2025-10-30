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

namespace PMS.Controllers.BaoCao.SoCheDinhHinh
{
    [Authorize]
    public class BaoCaoSoCheDinhHinhController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoSoCheDinhHinhController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Sơ Chế Định Hình / Chi Tiết", Func = "Xem Báo Cáo / Sơ Chế Định Hình / Chi Tiết")]
        public IActionResult ChiTietSoCheDinhHinhView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietSoCheDinhHinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Sơ Chế Định Hình";
            return View("~/Views/BaoCao/SoCheDinhHinh/ChiTietSoCheDinhHinhView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/GetPhieuCanSoCheDinhHinhs/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Nhân Viên
        [CustomAuthorize(Fu = "Báo Cáo / Sơ Chế Định Hình / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / Sơ Chế Định Hình / Tổng Hợp Nhân Viên")]
        public IActionResult TongHopNhanVienSoCheDinhHinhView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienSoCheDinhHinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên Sơ Chế Định Hình";
            return View("~/Views/BaoCao/SoCheDinhHinh/TongHopNhanVienSoCheDinhHinhView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopsNhanVien(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/GetPhieuCanTongHopsNhanVien/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Nhân Viên
        [CustomAuthorize(Fu = "Báo Cáo / Sơ Chế Định Hình / Tổng Hợp Thành Phẩm", Func = "Xem Báo Cáo / Sơ Chế Định Hình / Tổng Hợp Thành Phẩm")]
        public IActionResult TongHopThanhPhamSoCheDinhHinhView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamSoCheDinhHinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Thành Phẩm Sơ Chế Định Hình";
            return View("~/Views/BaoCao/SoCheDinhHinh/TongHopThanhPhamSoCheDinhHinhView.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHopsLoaiTP(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/GetPhieuCanTongHopsLoaiTP/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
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
            if (reportType == "ChiTietSoCheDinhHinhView")
            {
                dataSource = await GetChiTiets(fromDate, toDate);
            }
            else if (reportType == "TongHopNhanVienSoCheDinhHinhView")
            {
                dataSource = await GetPhieuCanTongHopsNhanVien(fromDate, toDate);
            }
            else if (reportType == "TongHopThanhPhamSoCheDinhHinhView")
            {
                dataSource = await GetPhieuCanTongHopsLoaiTP(fromDate, toDate);
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
