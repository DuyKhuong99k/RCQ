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
    public class PhucVuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public PhucVuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Tính Lương / Định Hình / Phục Vụ", Func = "Xem / Tính Lương / Định Hình / Phục Vụ")]
        public IActionResult PhucVuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhucVuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var xuongId = HttpContext.Session.GetString("XuongId");

            var apiNhomKiemUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetNhomKiemsWithType2/{xuongId}";
            using var helperNhomKiem = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhomkiems = helperNhomKiem.GetAsync<IEnumerable<object>>(HttpContext, apiNhomKiemUrl);
            ViewBag.listNhomKiem = nhomkiems.Result.ToList();

            ViewBag.titile = "Tính Lương Định Hình Phục Vụ";
            return View("~/Views/TinhLuong/DinhHinh/PhucVuView.cshtml");
        }
        [CustomAuthorize(Fu = "Tính Lương / Định Hình / Phục Vụ", Func = "Cài Đặt Nhân Viên / Tính Lương / Định Hình / Phục Vụ")]
        public IActionResult CaiDatNhanVienPhucVuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CaiDatNhanVienPhucVuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var xuongId = HttpContext.Session.GetString("XuongId");

            var apiNhomKiemUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetNhomKiemsWithType2/{xuongId}";
            using var helperNhomKiem = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhomkiems = helperNhomKiem.GetAsync<IEnumerable<object>>(HttpContext, apiNhomKiemUrl);
            ViewBag.listNhomKiem = nhomkiems.Result.ToList();

            ViewBag.titile = "Cài Đặt Nhân Viên Phục Vụ";
            return View("~/Views/TinhLuong/DinhHinh/CaiDatNhanVienPhucVuView.cshtml");
        }

        public async Task<IEnumerable<object>> ReloadSanLuongPhucVu(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/ReloadSanLuongPhucVu/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> ReloadDanhSachMacDinhCaiDatNhanVienTL(DateTime dateTime, string xuongId, int type)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/ReloadDanhSachMacDinhCaiDatNhanVienTL/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{type}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> LoadNhanVienDaiThanhTo(string nhomKiem)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/LoadGridNhanVienDaiThanhTo/{nhomKiem}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> ReloadDanhSachMacDinhCaiDatNhanVienTL2()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/ReloadDanhSachMacDinhCaiDatNhanVienTL2";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> MoveTo(string nhomKiem, string listNhanVienKiemSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/MoveTo/{nhomKiem}";
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
        public async Task<IActionResult> RemoveTo(string nhomKiem, string listNhanVienKiemToSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/RemoveTo/{nhomKiem}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listNhanVienKiemToSelectedItems));
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
        public async Task<IActionResult> SaveCaiDatNhanVien(DateTime dateTime, string xuongId)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/SaveCaiDatNhanVien/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{2}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var response = await helper.PostAsync(HttpContext, apiUrl, null);
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
        public async Task<IActionResult> AddNhanVienDaiThanhTo(string nhomKiem, string listNhanVienToSelectedItems)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/AddNhanVienDaiThanhTo/{nhomKiem}";
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
        public async Task<ActionResult> Reload(DateTime dateTime, string xuongId, string typeReload)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;

            try
            {
                switch (typeReload)
                {
                    case "ReloadSanLuongPhucVu":
                        dataSource = await ReloadSanLuongPhucVu(dateTime, xuongId);
                        break;
                    case "ReloadDanhSachMacDinhCaiDatNhanVienTL":
                        dataSource = await ReloadDanhSachMacDinhCaiDatNhanVienTL(dateTime, xuongId, 2);
                        break;
                    case "ReloadDanhSachMacDinhCaiDatNhanVienTL2":
                        dataSource = await ReloadDanhSachMacDinhCaiDatNhanVienTL2();
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
        public async Task<IActionResult> KetChuyenSanLuongPhucVu(DateTime dateTime, string maXuong, string listSanLuongToKiemSoChe)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/KetChuyenSanLuongPhucVu/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
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
    }
}
