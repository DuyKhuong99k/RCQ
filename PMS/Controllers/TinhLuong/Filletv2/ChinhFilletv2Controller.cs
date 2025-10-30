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
using Models.Repos.AppModel;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using Models.Repos;
using System;
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;
using AppViewModels;
using System.Transactions;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Syncfusion.EJ2.Layouts;
using Syncfusion.EJ2.Navigations;
using ViewModels.Repos.HQ;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using Models.Repos.A_Model;
namespace PMS.Controllers.TinhLuong.Filletv2
{
    [Authorize]
    public class ChinhFilletv2Controller : Controller
    {
        private readonly IHubContext<PMS.Hubs.ProgressHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChinhFilletv2Controller(IHubContext<PMS.Hubs.ProgressHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xem / Tính Lương / Phụ Fillet / Chính")]
        public IActionResult ChinhView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();
            var apiThanhPhamFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAllsByCodeId/{codeId}";
            using var helperThanhPhamFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamFLs = helperThanhPhamFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiThanhPhamFLUrl);
            ViewBag.listThanhPhamFLs = thanhPhamFLs.Result.ToList();
            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiKhuVucUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/KhuVucs/Gets";
            using var helperKhuVuc = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khuvuc = helperKhuVuc.GetAsync<IEnumerable<object>>(HttpContext, apiKhuVucUrl);
            ViewBag.listKhuVucs = khuvuc.Result.ToList();

            ViewBag.token = HttpContext.Session.GetString("JWTTokenEncry");
            ViewBag.titile = "Tính Lương Filletv2 Chính";
            return View("~/Views/TinhLuong/Filletv2/ChinhView.cshtml");
        }
        public async Task<IEnumerable<object>> LoadTongHopTinhLuongFilletv2(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/LoadTongHopTinhLuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<ActionResult> Reload(DateTime dateTime, string xuongId, string typeReload)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;

            try
            {
                switch (typeReload)
                {
                    case "LoadTongHopTinhLuongFilletv2":
                        dataSource = await LoadTongHopTinhLuongFilletv2(dateTime, xuongId);
                        break;

                }

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

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> KetChuyen(DateTime dateTime, string maXuong, string listSanLuongPhuFillet)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/KetChuyenTinhLuongFilletv2/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listSanLuongPhuFillet));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                //var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                return Json(new
                {
                    isSuccess = response.Success,
                    Messages = response.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Đã xảy ra lỗi: " + ex.Message
                });
            }

        }
        public IActionResult GetSignalRConnectionInfo()
        {
            var url = AppViewModels.AppViewModel.Instance.ApiHostUrl + "/progressHub";
            var token = HttpContext.Session.GetString("JWTToken");

            // Mã hóa URL và Token
            var encryptedUrl = Security.Crypt.ED.EncryptString(url);
            var encryptedToken = Security.Crypt.ED.EncryptString(token);

            return Json(new { url = encryptedUrl, token = encryptedToken });
        }
        public async Task<IActionResult> ExecuteTPSoftAction(DateTime dateTime, string maXuong)
        {
            //var progress = new Progress<string>(async message =>
            //{
            //    await _hubContext.Clients.All.SendAsync("ReceiveProgress", message);
            //});

            await CallExecuteTPSoftActionApi(dateTime, maXuong);
            return Ok();
        }

        private async Task CallExecuteTPSoftActionApi(DateTime dateTime, string maXuong)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/TPSoftAction_Filletv2/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";

                //var model = new ProgressTPSoftcs
                //{
                //    DateTime = dateTime,
                //    MaXuong = maXuong
                //};
                //var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                //var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                var response = await helper.PostAsync(HttpContext, apiUrl, null);
                //if (response.Success)
                //{
                //    progress.Report("Thực hiện xong!");
                //}
                //else
                //{
                //    progress.Report($"Error: {response.Message}");
                //}
            }
            catch (Exception ex)
            {
                //progress.Report("Có lỗi xảy ra: " + ex.ToString());
            }
        }
    }
}
