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

namespace PMS.Controllers.DanhMuc.XepKhuon.Chinh
{
    [Authorize]
    public class ChiTietRaCoiXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ChiTietRaCoiXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Chi Tiết Ra Cối", Func = "Xem Xếp Khuôn / Chi Tiết Ra Cối")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietRaCoiChinhXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiCoiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAlls";
            using var helperCoi = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var cois = helperCoi.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiUrl);
            ViewBag.listCois = cois.Result.ToList();
            ViewBag.TitlePage = "Chi Tiết Ra Cối";
            return View("~/Views/DanhMuc/XepKhuon/Chinh/ChiTietRaCoiXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ChiTietRaCois/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<ChiTietRaCoi>> GetAlls()
        {
            IEnumerable<ChiTietRaCoi> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ChiTietRaCois/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<ChiTietRaCoi>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew()
        {
            try
            {
                var dataSource = await GetAlls();
                if (dataSource != null)
                {

                    var maxId = dataSource.Select(x => x.Id).DefaultIfEmpty(0).Max();
                    var id = $"{(maxId + 1).ToString("000000")}";
                    var idd = int.TryParse(id, out var rl);
                    return Json(new
                    {
                        isSuccess = true,
                        Id = rl,
                        Ngay = DateTime.Today,
                        Gio = DateTime.Now.TimeOfDay,
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
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Chi Tiết Ra Cối", Func = "Thêm Xếp Khuôn / Chi Tiết Ra Cối")]
        public async Task<IActionResult> DoInsert(long id, DateTime ngay, TimeSpan gio, string maCoi, int luot, bool isDone, DateTime ngayNguyenLieu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ChiTietRaCois/Insert";
            try
            {
                if (string.IsNullOrEmpty(maCoi))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new ChiTietRaCoi
                {
                    Id = id,
                    Ngay = ngay,
                    Gio = gio,
                    MaCoi = maCoi,
                    Luot = luot,
                    IsDone = isDone,
                    NgayNguyenLieu = ngayNguyenLieu

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
        public async Task<IActionResult> GetsByMa(long id)
        {
            if (id == null || id == 0)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ChiTietRaCois/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<ChiTietRaCoi>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaCoi = item.MaCoi,
                        Luot = item.Luot,
                        IsDone = item.IsDone,
                        NgayNguyenLieu = item.NgayNguyenLieu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Chi Tiết Ra Cối", Func = "Sửa Xếp Khuôn / Chi Tiết Ra Cối")]
        public async Task<IActionResult> DoUpDate(long id, DateTime ngay, TimeSpan gio, string maCoi, int luot, bool isDone, DateTime ngayNguyenLieu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ChiTietRaCois/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (id == null || id == 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new ChiTietRaCoi
                {
                    Id = id,
                    Ngay = ngay,
                    Gio = gio,
                    MaCoi = maCoi,
                    Luot = luot,
                    IsDone = isDone,
                    NgayNguyenLieu = ngayNguyenLieu
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
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Chi Tiết Ra Cối", Func = "Xóa Xếp Khuôn / Chi Tiết Ra Cối")]
        public async Task<IActionResult> DoDelete(long id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ChiTietRaCois/Delete/{id}";
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
