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
using Syncfusion.EJ2.Gantt;

namespace PMS.Controllers.DanhMuc.NguyenLieu
{
    [Authorize]
    public class MauCaGiongVungNuoiController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MauCaGiongVungNuoiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Vùng Nuôi / Mẫu Cá Giống", Func = "Xem Vùng Nuôi / Mẫu Cá Giống")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "MauCaGiongVungNuoiView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiUrlGheVungNuoi = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaGheVungNuois/GetAlls";
            using var helperGheVungNuoi = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var itemGheVungNuois = helperGheVungNuoi.GetAsync<IEnumerable<MaGheVungNuoi>>(HttpContext, apiUrlGheVungNuoi);
            ViewBag.listGheVungNuois = itemGheVungNuois.Result.ToList();
            ViewBag.TitlePage = "Mẫu Cá Giống Vùng Nuôi";
            return View("~/Views/DanhMuc/NguyenLieu/MauCaGiongVungNuoiView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauCaGiongVungNuois/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew()
        {
            return Json(new
            {
                isSuccess = true,
                Ngay = DateTime.Now,
            });
        }

        [CustomAuthorize(Fu = "Danh Mục / Vùng Nuôi / Mẫu Cá Giống", Func = "Thêm Vùng Nuôi / Mẫu Cá Giống")]
        public async Task<IActionResult> DoInsert(DateTime ngay, string maGhe, decimal soLuong, decimal trongLuongDonVi, decimal trongLuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauCaGiongVungNuois/Insert";
            try
            {
                if (ngay == null || string.IsNullOrEmpty(maGhe) || soLuong == null || trongLuongDonVi == null || trongLuong == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaMauCaGiongVungNuoi
                {
                    Ngay = ngay,
                    MaGhe = maGhe,
                    SoLuong = soLuong,
                    TrongLuongDonVi = trongLuongDonVi,
                    TrongLuong = trongLuong
                };
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (response.Success)
                {
                    // Đăng ký thành công
                    return Json(new
                    {
                        isSuccess = response.Success,
                        Messages = response.Message
                    });
                }

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
                    Messages = "Đã xảy ra lỗi: " + ex.Message.ToString()
                });
            }
        }
        public async Task<IActionResult> GetsByMa(DateTime ngay, string maGhe)
        {
            if (ngay == null || string.IsNullOrEmpty(maGhe))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn thông tin!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauCaGiongVungNuois/GetsByMa/{ngay.ToString("yyyy-MM-dd")}/{maGhe}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaMauCaGiongVungNuoi>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Ngay = item.Ngay,
                        MaGhe = item.MaGhe,
                        SoLuong = item.SoLuong,
                        TrongLuongDonVi = item.TrongLuongDonVi,
                        TrongLuong = item.TrongLuong
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Vùng Nuôi / Mẫu Cá Giống", Func = "Sửa Vùng Nuôi / Mẫu Cá Giống")]
        public async Task<IActionResult> DoUpDate(DateTime ngay, string maGhe, decimal soLuong, decimal trongLuongDonVi, decimal trongLuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauCaGiongVungNuois/Update/{ngay.ToString("yyyy-MM-dd")}/{maGhe}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (ngay == null || string.IsNullOrEmpty(maGhe) || soLuong == null || trongLuongDonVi == null || trongLuong == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaMauCaGiongVungNuoi
                {
                    SoLuong = soLuong,
                    TrongLuongDonVi = trongLuongDonVi,
                    TrongLuong = trongLuong
                };
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                if (rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = rl.Success,
                        Messages = rl.Message
                    });
                }
                return Json(new
                {
                    isSuccess = rl.Success,
                    Messages = rl.Message
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
        [CustomAuthorize(Fu = "Danh Mục / Vùng Nuôi / Mẫu Cá Giống", Func = "Xóa Vùng Nuôi / Mẫu Cá Giống")]
        public async Task<IActionResult> DoDelete(DateTime ngay, string maGhe)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauCaGiongVungNuois/Delete/{ngay.ToString("yyyy-MM-dd")}/{maGhe}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = rl.Success,
                        Messages = rl.Message
                    });
                }
                return Json(new
                {
                    isSuccess = rl.Success,
                    Messages = rl.Message
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
