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

namespace PMS.Controllers.TinhLuong.DinhHinh
{
    [Authorize]
    public class ToKiemSoCheController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ToKiemSoCheController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Tính Lương / Định Hình / Tổ Kiểm Sơ Chế", Func = "Xem / Tính Lương / Định Hình / Tổ Kiểm Sơ Chế")]
        public IActionResult ToKiemSoCheView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ToKiemSoCheView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Tính Lương Định Hình Sơ Chế";
            return View("~/Views/TinhLuong/DinhHinh/ToKiemSoCheView.cshtml");
        }
        [CustomAuthorize(Fu = "Tính Lương / Định Hình / Tổ Kiểm Sơ Chế", Func = "Cài Đặt Nhân Viên / Tính Lương / Định Hình / Tổ Kiểm Sơ Chế")]
        public IActionResult CaiDatNhanVienToKiemSoCheView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CaiDatNhanVienToKiemSoCheView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var xuongId = HttpContext.Session.GetString("XuongId");

            var apiNhomSoCheUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomKiemSoChes/GetNhomKiemSoChe/{xuongId}";
            using var helperNhomSoChe = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhomsoches = helperNhomSoChe.GetAsync<IEnumerable<object>>(HttpContext, apiNhomSoCheUrl);
            ViewBag.listNhomKiemSoChe = nhomsoches.Result.ToList();

            ViewBag.titile = "Cài Đặt Nhân Viên Kiểm Sơ Chế";
            return View("~/Views/TinhLuong/DinhHinh/CaiDatNhanVienToKiemSoCheView.cshtml");
        }
        public async Task<IEnumerable<object>> ReloadSanLuongKiemSoChe(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/ReloadSanLuongKiemSoChe/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> KetChuyenSanLuongKiemSoChe(DateTime dateTime, string maXuong, string listSanLuongToKiemSoChe)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/KetChuyenSanLuongKiemSoChe/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listSanLuongToKiemSoChe));
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
        public async Task<IEnumerable<object>> LoadBoTriNhomKiemSoChe(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomKiemSoChes/LoadBoTriNhomKiemSoChe/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> LoadNhanVienNhomKiemSoChe(DateTime dateTime, string xuongId, string nhomKiemSoChe)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomKiemSoChes/NhomSoCheSelectionChanged/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomKiemSoChe}";
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
                    case "ReloadSanLuongKiemSoChe":
                        dataSource = await ReloadSanLuongKiemSoChe(dateTime, xuongId);
                        break;
                    case "LoadBoTriNhomKiemSoChe":
                        dataSource = await LoadBoTriNhomKiemSoChe(dateTime, xuongId);
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

        public async Task<IActionResult> MoveTo(DateTime dateTime, string xuongId, string nhomKiemSoChe, string listNhanVienKiemSoCheSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomKiemSoChes/MoveTo/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomKiemSoChe}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listNhanVienKiemSoCheSelectedItems));
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
        public async Task<IActionResult> RemoveTo(DateTime dateTime, string xuongId, string nhomKiemSoChe, string listNhanVienToSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomKiemSoChes/Remove/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomKiemSoChe}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listNhanVienToSelectedItems));
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
        public async Task<IActionResult> AddNhanVienDaiThanhTo(DateTime dateTime, string xuongId, string nhomKiemSoChe, string listNhanVienToSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomKiemSoChes/Add/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomKiemSoChe}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listNhanVienToSelectedItems));
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
    }
}
