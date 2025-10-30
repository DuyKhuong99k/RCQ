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


namespace PMS.Controllers.DanhMuc.NguyenLieu
{
    [Authorize]
    public class DonGiaVanChuyenController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DonGiaVanChuyenController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Đơn Giá Vận Chuyển", Func = "Xem Nguyên Liệu / Đơn Giá Vận Chuyển")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "DonGiaVanChuyenView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieux/GetAlls";
            //var listNhaCungCaps = GetApiAsync<NhaCungCapNguyenLieu>(apiUrl);
            //ViewBag.listNhaCCs = listNhaCungCaps.Result.ToList();
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<IEnumerable<NhaCungCapNguyenLieu>>(HttpContext, apiUrl);
            ViewBag.listNhaCCs = items.Result.ToList();
            ViewBag.TitlePage = "Đơn Giá Vận Chuyển";
            return View("~/Views/DanhMuc/NguyenLieu/DonGiaVanChuyenView.cshtml");
        }
        private async Task<List<T>> GetApiAsync<T>(string apiurl)
        {
            IEnumerable<object> dataSource = null;

            var apiUrl = apiurl;

            if (dataSource == null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWTToken"));
                    // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var sourceObjects = JsonConvert.DeserializeObject<IEnumerable<object>>(data);
                        if (sourceObjects != null) dataSource = sourceObjects;
                    }
                }
            }
            var jsonString = JsonConvert.SerializeObject(dataSource);
            var apiData = JsonConvert.DeserializeObject<List<T>>(jsonString)?.ToList();
            return apiData;
        }
        public async Task<IEnumerable<object>> GetAlls()
        {
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieu_DonGiaVanChuyen/GetAllsFullField";
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
                    DonGia = 0,
                    NgayApDung = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Lỗi!" + ex.Message.ToString()
                });
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Đơn Giá Vận Chuyển", Func = "Thêm Nguyên Liệu / Đơn Giá Vận Chuyển")]
        public async Task<IActionResult> DoInsert(string maNhaCC, DateTime ngayApDung, decimal donGia, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieu_DonGiaVanChuyen/Insert";
            try
            {
                if (string.IsNullOrEmpty(maNhaCC) || ngayApDung == null || donGia == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new NhaCungCapNguyenLieu_DonGiaVanChuyen
                {
                    MaNhaCC = maNhaCC,
                    NgayApDung = ngayApDung,
                    DonGia = donGia,
                    GhiChu = ghiChu
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
                        Messages = response.Message,
                    });
                }

                return Json(new
                {
                    isSuccess = response.Success,
                    Messages = response.Message,
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
        public async Task<IActionResult> GetByMaNhaCCAndNgayApDung(string maNhaCC, DateTime ngayApDung)
        {
            if (string.IsNullOrEmpty(maNhaCC) || ngayApDung == null)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn nhà cung cấp đơn giá!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieu_DonGiaVanChuyen/GetByMaNhaCCAndNgayApDung/{maNhaCC}/{ngayApDung.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<NhaCungCapNguyenLieu_DonGiaVanChuyen>(HttpContext, apiUrl);

                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaNhaCC = item?.MaNhaCC,
                        NgayApDung = item?.NgayApDung.ToString("yyyy-MM-dd"),
                        DonGia = item?.DonGia,
                        GhiChu = item?.GhiChu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Đơn Giá Vận Chuyển", Func = "Sửa Nguyên Liệu / Đơn Giá Vận Chuyển")]
        public async Task<IActionResult> DoUpDate(string maNhaCC, DateTime ngayApDung, decimal? donGia, string? ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieu_DonGiaVanChuyen/Update/{maNhaCC}/{ngayApDung.ToString("yyyy-MM-dd")}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maNhaCC) || ngayApDung == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new NhaCungCapNguyenLieu_DonGiaVanChuyen
                {
                    NgayApDung = ngayApDung,
                    GhiChu = ghiChu,
                    DonGia = donGia,

                    MaNhaCC = maNhaCC

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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Đơn Giá Vận Chuyển", Func = "Xoá Nguyên Liệu / Đơn Giá Vận Chuyển")]
        public async Task<IActionResult> DoDelete(string maNhaCC, DateTime ngayApDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieu_DonGiaVanChuyen/Delete/{maNhaCC}/{ngayApDung.ToString("yyyy-MM-dd")}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var rl = await helper.PostAsync(HttpContext, apiUrl, null);
            try
            {
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
