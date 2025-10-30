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
namespace PMS.Controllers.DanhMuc.SanPhamTinhLuong
{
    [Authorize]
    public class DinhMucXepHangSPTLController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DinhMucXepHangSPTLController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Định Mức Xếp Hạng", Func = "Xem Sản Phẩm Tính Lương / Định Mức Xếp Hạng")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "DinhMucXepHangView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Định Mức Xếp Hạng";
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            ViewBag.listMaLos = items.Result.ToList();
            return View("~/Views/DanhMuc/SanPhamTinhLuong/DinhMucXepHangView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullFieldByYear(int year)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucXepHangs/GetAllsFullFieldByYear/{year}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<DinhMucXepHang>> GetAllsByYear(int year)
        {
            IEnumerable<DinhMucXepHang> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucXepHangs/GetAllsByYear/{year}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<DinhMucXepHang>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<DinhMucXepHang>> GetAlls()
        {
            IEnumerable<DinhMucXepHang> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucXepHangs/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<DinhMucXepHang>>(HttpContext, apiUrl);
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
                    DinhMucDown = 0,
                    DinhMucUp = 0,
                    Year = DateTime.Now.Year,
                    DinhMuc = 0
                });


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Định Mức Xếp Hạng", Func = "Thêm Sản Phẩm Tính Lương / Định Mức Xếp Hạng")]
        public async Task<IActionResult> DoInsert(string maLo, string maSanPham, string maXepHang, int year, decimal dinhMucUp, decimal dinhMucDown, decimal dinhMuc)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucXepHangs/Insert";
            try
            {
                if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maXepHang))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new DinhMucXepHang
                {
                    MaLo = maLo,
                    MaSanPham = maSanPham,
                    MaXepHang = maXepHang,
                    DinhMucUp = dinhMucUp,
                    Year = year,
                    DinhMucDown = dinhMucDown,
                    DinhMuc = dinhMuc
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
        public async Task<IActionResult> GetsByMa(string maLo, string maSanPham, string maXepHang, int year)
        {
            if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maXepHang) || year == null || year == 0)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucXepHangs/GetsByMa/{maLo}/{maSanPham}/{maXepHang}/{year}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<DinhMucXepHang>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaLo = item.MaLo,
                        MaSanPham = item.MaSanPham,
                        MaXepHang = item.MaXepHang,
                        DinhMucUp = item.DinhMucUp,
                        Year = item.Year,
                        DinhMucDown = item.DinhMucDown,
                        DinhMuc = item.DinhMuc
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Định Mức Xếp Hạng", Func = "Sửa Sản Phẩm Tính Lương / Định Mức Xếp Hạng")]
        public async Task<IActionResult> DoUpDate(string maLo, string maSanPham, string maXepHang, int year, decimal dinhMucUp, decimal dinhMucDown, decimal dinhMuc)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucXepHangs/Update/{maLo}/{maSanPham}/{maXepHang}/{year}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maXepHang) || year == null || year == 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new DinhMucXepHang
                {
                    MaLo = maLo,
                    MaSanPham = maSanPham,
                    MaXepHang = maXepHang,
                    DinhMucUp = dinhMucUp,
                    Year = year,
                    DinhMucDown = dinhMucDown,
                    DinhMuc = dinhMuc
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
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Định Mức Xếp Hạng", Func = "Xoá Sản Phẩm Tính Lương / Định Mức Xếp Hạng")]
        public async Task<IActionResult> DoDelete(string maLo, string maSanPham, string maXepHang, int year)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucXepHangs/Delete/{maLo}/{maSanPham}/{maXepHang}/{year}";
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
