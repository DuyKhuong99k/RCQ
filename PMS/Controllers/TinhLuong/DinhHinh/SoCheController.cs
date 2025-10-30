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
    public class SoCheController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SoCheController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Tính Lương / Định Hình / Tổ Kiểm", Func = "Xem / Tính Lương / Định Hình / Tổ Kiểm")]
        public IActionResult SoCheView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "SoCheView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Tính Lương Định Hình Sơ Chế";
            return View("~/Views/TinhLuong/DinhHinh/SoCheView.cshtml");
        }
        [CustomAuthorize(Fu = "Tính Lương / Định Hình / Sơ Chế", Func = "Cài Đặt Nhân Viên / Tính Lương / Định Hình / Sơ Chế")]
        public IActionResult CaiDatNhanVienSoCheView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CaiDatNhanVienSoCheView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var xuongId = HttpContext.Session.GetString("XuongId");

            var apiNhomSoCheUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomSoChes/GetNhomSoChe/{xuongId}";
            using var helperNhomSoChe = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhomsoches = helperNhomSoChe.GetAsync<IEnumerable<object>>(HttpContext, apiNhomSoCheUrl);
            ViewBag.listNhomSoChe = nhomsoches.Result.ToList();

            ViewBag.titile = "Cài Đặt Nhân Viên Sơ Chế";
            return View("~/Views/TinhLuong/DinhHinh/CaiDatNhanVienSoCheView.cshtml");
        }

        public async Task<IEnumerable<object>> ReloadSanLuongSoChe(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/ReloadSanLuongSoChe/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> KetChuyenSanLuongSoChe(DateTime dateTime, string maXuong, string listSanLuongSoChe)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/KetChuyenSanLuongSoChe/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listSanLuongSoChe));
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
        public async Task<IEnumerable<object>> LoadBoTriNhomSoChe(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomSoChes/LoadBoTriNhomSoChe/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> LoadNhanVienNhomSoChe(DateTime dateTime, string xuongId, string nhomSoChe)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomSoChes/NhomSoCheSelectionChanged/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomSoChe}";
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
                    case "ReloadSanLuongSoChe":
                        dataSource = await ReloadSanLuongSoChe(dateTime, xuongId);
                        break;
                    case "LoadBoTriNhomSoChe":
                        dataSource = await LoadBoTriNhomSoChe(dateTime, xuongId);
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
        public async Task<IActionResult> MoveTo(DateTime dateTime,string xuongId,string nhomSoChe, string listNhanVienKiemSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomSoChes/MoveTo/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomSoChe}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listNhanVienKiemSelectedItems));
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
        public async Task<IActionResult> RemoveTo(DateTime dateTime,string xuongId,string nhomSoChe, string listNhanVienSoCheNhomSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomSoChes/Remove/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomSoChe}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listNhanVienSoCheNhomSelectedItems));
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
        public async Task<IActionResult> AddNhanVienDaiThanhTo(DateTime dateTime,string xuongId,string nhomSoChe, string listNhanVienToSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhomSoChes/Add/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{nhomSoChe}";
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
