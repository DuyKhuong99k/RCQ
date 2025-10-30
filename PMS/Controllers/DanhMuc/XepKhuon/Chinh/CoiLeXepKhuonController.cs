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
    public class CoiLeXepKhuonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CoiLeXepKhuonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CoiLeXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiCoiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAllCoiChinhs";
            using var helperCoi = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var Cois = helperCoi.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiUrl);
            ViewBag.Cois = Cois.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamChinhXepKhuons/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var ThanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamChinhXepKhuon>>(HttpContext, apiThanhPhamUrl);
            ViewBag.ThanhPhams = ThanhPhams.Result.ToList();

            ViewBag.TitlePage = "Cối Lẻ Xếp Khuôn";
            return View("~/Views/DanhMuc/XepKhuon/Chinh/CoiLeXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<CoiLeXepKhuon>> GetAlls()
        {
            IEnumerable<CoiLeXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CoiLeXepKhuons/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<CoiLeXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllsFullField(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CoiLeXepKhuons/GetAllsFullField/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Cối Lẻ", Func = "Thêm Xếp Khuôn / Cối Lẻ")]
        public async Task<IActionResult> DoInsert(string maCoi,string maSanPham, decimal trongLuong,DateTime ngayGio, DateTime ngayNguyenLieu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CoiLeXepKhuons/Insert";
            try
            {
                if (string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maSanPham) || trongLuong  == 0 || trongLuong <0 || trongLuong == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var id = 1;
                var dataSource = await GetAlls();
                if (dataSource != null)
                {
                    var maxId = dataSource.Select(x => x.Id)
                        .DefaultIfEmpty(0)
                        .Max();
                    id = maxId + 1;
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Lỗi!"
                    });
                }
                var model = new CoiLeXepKhuon
                {
                    Id = id,
                    MaCoi = maCoi,
                    MaSanPham = maSanPham,
                    TrongLuong = trongLuong,
                    NgayGio = ngayGio,
                    NgayNguyenLieu = ngayNguyenLieu
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
        public async Task<IActionResult> GetsByMa(int id)
        {
            if (id <= 0 || id == null)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CoiLeXepKhuons/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<CoiLeXepKhuon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",

                        MaCoi = item.MaCoi,
                        MaSanPham = item.MaSanPham,
                        TrongLuong = item.TrongLuong,
                        NgayGio = item.NgayGio, NgayNguyenLieu = item.NgayNguyenLieu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Cối Lẻ", Func = "Sửa Xếp Khuôn / Cối Lẻ")]
        public async Task<IActionResult> DoUpDate(int id,string maCoi,string maSanPham, decimal trongLuong,DateTime ngayGio , DateTime ngayNguyenLieu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CoiLeXepKhuons/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maSanPham) || trongLuong  == 0 || trongLuong <0 || trongLuong == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new CoiLeXepKhuon
                {
                    Id = id,
                    MaCoi = maCoi,
                    MaSanPham = maSanPham,
                    TrongLuong = trongLuong,
                    NgayGio = ngayGio, NgayNguyenLieu = ngayNguyenLieu
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
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Cối Lẻ", Func = "Xóa Xếp Khuôn / Cối Lẻ")]
        public async Task<IActionResult> DoDelete(int id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CoiLeXepKhuons/Delete/{id}";
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
