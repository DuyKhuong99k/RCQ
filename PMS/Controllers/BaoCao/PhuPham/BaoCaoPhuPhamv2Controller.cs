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

namespace PMS.Controllers.BaoCao.PhuPham
{
    [Authorize]
    public class BaoCaoPhuPhamv2Controller : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoPhuPhamv2Controller(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Phụ Phẩm v2 / Chi Tiết", Func = "Xem Báo Cáo / Phụ Phẩm v2 / Chi Tiết")]
        public IActionResult ChiTietPhuPhamv2View()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietPhuPhamv2View");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Phụ Phẩm v2";
            return View("~/Views/BaoCao/PhuPham/ChiTietPhuPhamv2View.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/GetPhieuCanChiTiets/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Khách hàng
        [CustomAuthorize(Fu = "Báo Cáo / Phụ Phẩm v2 / Tổng Hợp", Func = "Xem Báo Cáo / Phụ Phẩm v2 / Tổng Hợp")]
        public IActionResult TongHopPhuPhamv2View()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopPhuPhamv2View");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Phụ Phẩm v2";
            return View("~/Views/BaoCao/PhuPham/TongHopPhuPhamv2View.cshtml");
        }
        public async Task<IEnumerable<object>> GetPhieuCanTongHops(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/GetPhieuCanTongHops/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}";
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
            if (reportType == "ChiTietPhuPhamv2View")
            {
                dataSource = await GetChiTiets(fromDate, toDate);
            }
            else if (reportType == "TongHopPhuPhamv2View")
            {
                dataSource = await GetPhieuCanTongHops(fromDate, toDate);
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
