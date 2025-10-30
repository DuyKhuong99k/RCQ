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
    public class TrongLuongCoiTheoSanPhamController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TrongLuongCoiTheoSanPhamController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm", Func = "Xem Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TrongLuongCoiTheoSanPhamView");

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

            ViewBag.TitlePage = "Trọng Lượng Cối Theo Sản Phẩm Xếp Khuôn";
            return View("~/Views/DanhMuc/XepKhuon/Chinh/TrongLuongCoiTheoSanPhamView.cshtml");
        }
        public async Task<IEnumerable<TrongLuongCoiTheoSanPham>> GetAlls()
        {
            IEnumerable<TrongLuongCoiTheoSanPham> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoSanPhams/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<TrongLuongCoiTheoSanPham>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoSanPhams/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm", Func = "Thêm Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm")]
        public async Task<IActionResult> DoInsert(string maCoi,string maSanPham, decimal trongLuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoSanPhams/Insert";
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
                var model = new TrongLuongCoiTheoSanPham
                {
                    Id = id,
                    MaCoi = maCoi,
                    MaSanPham = maSanPham,
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoSanPhams/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<TrongLuongCoiTheoSanPham>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",

                        MaCoi = item.MaCoi,
                        MaSanPham = item.MaSanPham,
                        TrongLuong = item.TrongLuong,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm", Func = "Sửa Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm")]
        public async Task<IActionResult> DoUpDate(int id,string maCoi,string maSanPham, decimal trongLuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoSanPhams/Update/{id}";
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
                var model = new TrongLuongCoiTheoSanPham
                {
                    Id = id,
                    MaCoi = maCoi,
                    MaSanPham = maSanPham,
                    TrongLuong = trongLuong,
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
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm", Func = "Xóa Xếp Khuôn / Trọng Lượng Cối Theo Sản Phẩm")]
        public async Task<IActionResult> DoDelete(int id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoSanPhams/Delete/{id}";
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
