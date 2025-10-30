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

namespace PMS.Controllers.DanhMuc.Fillet
{
    [Authorize]
    public class ThanhPhamMacDinhFilletController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ThanhPhamMacDinhFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm Mặc Định", Func = "Xem Fillet / Thành Phẩm Mặc Định")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "MaThanhPhamFillet_ThanhPhamMacDinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
            using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl);
            ViewBag.listTPFLs = tpfls.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();


            ViewBag.TitlePage = "Thành Phẩm Mặc Định Fillet";
            return View("~/Views/DanhMuc/Fillet/ThanhPhamMacDinhFilletView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField(string ngay, string maXuong)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_ThanhPhamMacDinh/GetAllsFullField/{ngay}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<MaThanhPhamFillet_ThanhPhamMacDinh>> GetAlls()
        {
            IEnumerable<MaThanhPhamFillet_ThanhPhamMacDinh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_ThanhPhamMacDinh/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamFillet_ThanhPhamMacDinh>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew(DateTime ngay, string maXuong)
        {
            try
            {
                var dataSource = await GetAlls();
                if (dataSource != null)
                {

                    var maxId = dataSource.Where(x => x.Ngay.Date == ngay.Date && x.MaXuong == maXuong)
                    .Select(x => x.STT)
                    .DefaultIfEmpty(0)
                    .Max();
                    var id = maxId + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = id,
                        Ngay = ngay.Date,
                        MaXuong = maXuong,
                        Gio = DateTime.Now
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Lỗi!"
                    });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm Mặc Định", Func = "Thêm Fillet / Thành Phẩm Mặc Định")]
        public async Task<IActionResult> DoInsert (int stt, DateTime ngay, string maLo, string maThanhPham, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_ThanhPhamMacDinh/Insert";
            try
            {
                if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaThanhPhamFillet_ThanhPhamMacDinh
                {
                    STT = stt,
                    Ngay = ngay,
                    Gio = DateTime.Now.TimeOfDay,
                    MaLo = maLo,
                    MaThanhPham = maThanhPham,
                    MaXuong = maXuong,
                   
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
        public async Task<IActionResult> GetsByMa(string stt, string ngay,string maXuong)
        {
            if (string.IsNullOrEmpty(stt) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_ThanhPhamMacDinh/GetsByMa/{stt}/{ngay}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaThanhPhamFillet_ThanhPhamMacDinh>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaLo = item.MaLo,
                        MaThanhPham = item.MaThanhPham,
                        MaXuong = item.MaXuong,
                      
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm Mặc Định", Func = "Sửa Fillet / Thành Phẩm Mặc Định")]
        public async Task<IActionResult> DoUpDate(string stt, string ngay, string maLo, string maThanhPham, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_ThanhPhamMacDinh/Update/{stt}/{ngay}/{maXuong}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                int number = int.Parse(stt);
                DateTime dateTime = DateTime.Parse(ngay);
                var model = new MaThanhPhamFillet_ThanhPhamMacDinh
                {
                    STT = number,
                    Ngay = dateTime,
                    Gio = DateTime.Now.TimeOfDay,
                    MaLo = maLo,
                    MaThanhPham = maThanhPham,
                    MaXuong = maXuong,
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm Mặc Định", Func = "Xoá Fillet / Thành Phẩm Mặc Định")]
        public async Task<IActionResult> DoDelete(string stt, string ngay, string maXuong)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_ThanhPhamMacDinh/Delete/{stt}/{ngay}/{maXuong}";
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
