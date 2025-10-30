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
using ViewModels.Repos.HQ;

namespace PMS.Controllers.DanhMuc.DinhHinh
{
    [Authorize]
    public class ThanhPhamTyLeDinhHinhController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ThanhPhamTyLeDinhHinhController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm Tỷ Lệ", Func = "Xem Định Hình / Thành Phẩm Tỷ Lệ")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ThanhPhamTyLeDinhHinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();
            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();
            ViewBag.TitlePage = "Thành Phẩm Tỷ Lệ Định Hình";
            return View("~/Views/DanhMuc/DinhHinh/ThanhPhamTyLeDinhHinhView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinh_TyLe/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<MaThanhPhamDinhHinh_TyLe>> GetAlls()
        {
            IEnumerable<MaThanhPhamDinhHinh_TyLe> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinh_TyLe/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamDinhHinh_TyLe>>(HttpContext, apiUrl);
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
                    NgayGio = DateTime.Now,
                    TyLeDau = 0,
                    TyLeRot = 0,
                    Id = $@"{DateTime.Now.ToString("yyyyMMdd.HHmmmss.fffffff")}",
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm Tỷ Lệ", Func = "Thêm Định Hình / Thành Phẩm Tỷ Lệ")]
        public async Task<IActionResult> DoInsert(string id, DateTime ngayGio, string maThanhPham, decimal tyLeDau, decimal tyLeRot, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinh_TyLe/Insert";
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaThanhPhamDinhHinh_TyLe
                {
                    Id = id,
                    NgayGio = ngayGio,
                    MaThanhPham = maThanhPham,
                    TyLeDau = tyLeDau,
                    TyLeRot = tyLeRot,
                    MaXuong = maXuong
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
        public async Task<IActionResult> GetsByMa(string id)
        {
            if (string.IsNullOrEmpty(id))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinh_TyLe/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaThanhPhamDinhHinh_TyLe>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        NgayGio = item.NgayGio,
                        MaThanhPham = item.MaThanhPham,
                        TyLeDau = item.TyLeDau,
                        TyLeRot = item.TyLeRot,
                        MaXuong = item.MaXuong
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm Tỷ Lệ", Func = "Sửa Định Hình / Thành Phẩm Tỷ Lệ")]
        public async Task<IActionResult> DoUpDate(string id, DateTime ngayGio, string maThanhPham, decimal tyLeDau, decimal tyLeRot, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinh_TyLe/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaThanhPhamDinhHinh_TyLe
                {
                    Id = id,
                    NgayGio = ngayGio,
                    MaThanhPham = maThanhPham,
                    TyLeDau = tyLeDau,
                    TyLeRot = tyLeRot,
                    MaXuong = maXuong
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
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm Tỷ Lệ", Func = "Xoá Định Hình / Thành Phẩm Tỷ Lệ")]
        public async Task<IActionResult> DoDelete(string id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinh_TyLe/Delete/{id}";
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
