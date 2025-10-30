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

namespace PMS.Controllers.XuLyPhieuCan
{
    [Authorize]
    public class XuLyPhieuCanTomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public XuLyPhieuCanTomController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tôm", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Tôm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhuongTienTaiTrongView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            return View("~/Views/XuLyPhieuCan/XuLyPhieuCanTomView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAlls(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetAllWithFullField/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<ActionResult> Reload(DateTime dateTime)
        {
            IEnumerable<object> dataSource = await GetAlls(dateTime);
            bool success = true; //trạng tháng action trả về mặc định là true

            if (dataSource == null || !dataSource.Any())
            {
                success = false;
            }
            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu Thành Công." : "Vui lòng kiểm tra lại!.",
                Data = dataSource
            };
            ViewBag.datasource = dataSource;
            return Json(result);
        }
    }
}
