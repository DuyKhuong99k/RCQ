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

namespace PMS.Controllers.DanhMucHQ
{
    [Authorize]
    public class HQ_SanPhamNguyenLieuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HQ_SanPhamNguyenLieuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Nguyên Liệu HQ", Func = "Xem Sản Phẩm Nguyên Liệu HQ")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "HQ_SanPhamNguyenLieu");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiQCHQUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_QuyCachNguyenLieu/GetAlls";
            using var helperQCHQ = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var qcs = helperQCHQ.GetAsync<IEnumerable<HQ_QuyCachNguyenLieu>>(HttpContext, apiQCHQUrl);
            ViewBag.listQuyCach = qcs.Result.ToList();


            ViewBag.TitlePage = "Sản Phẩm Nguyên Liệu";
            return View("~/Views/DanhMucHQ/HQ_SanPhamNguyenLieu/HQ_SanPhamNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAlls()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_SanPhamNguyenLieu/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew()
        {
            try
            {

                return Json(new
                {
                    isSuccess = true,
                    SuDung = true,
                    DiaChi = "",
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Nguyên Liệu HQ", Func = "Thêm Sản Phẩm Nguyên Liệu HQ")]
        public async Task<IActionResult> DoInsert(string ten, bool suDung, decimal min, decimal max, int maQuyCach)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_SanPhamNguyenLieu/Insert";
            try
            {
                if (string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_SanPhamNguyenLieu
                {
                    Ten = ten,
                    SuDung = suDung,
                    MNgay = DateTime.Now,
                    Min = min,
                    Max = max,
                    MaQuyCach = maQuyCach,
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
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
        public async Task<IActionResult> GetsByMa(int id)
        {
            if (id <= 0)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_SanPhamNguyenLieu/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<HQ_SanPhamNguyenLieu>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        SuDung = item?.SuDung,
                        Id = item?.Id,
                        Ten = item?.Ten,
                        Min = item?.Min,
                        Max = item?.Max,
                        MaQuyCach = item?.MaQuyCach,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Nguyên Liệu HQ", Func = "Sửa Sản Phẩm Nguyên Liệu HQ")]
        public async Task<IActionResult> DoUpDate(int id, string ten, bool suDung, decimal min, decimal max, int maQuyCach)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_SanPhamNguyenLieu/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (id <= 0 || string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_SanPhamNguyenLieu
                {
                    Id = id,
                    Ten = ten,
                    SuDung = suDung,
                    Min = min,
                    Max = max,
                    MaQuyCach = maQuyCach,
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
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
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Nguyên Liệu HQ", Func = "Xoá Sản Phẩm Nguyên Liệu HQ")]
        public async Task<IActionResult> DoDelete(int id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_SanPhamNguyenLieu/Delete/{id}";
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
