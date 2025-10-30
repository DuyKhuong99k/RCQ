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
    public class TrongLuongCoiTheoThanhPhamController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TrongLuongCoiTheoThanhPhamController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm", Func = "Xem Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TrongLuongCoiTheoThanhPhamView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiCoiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAlls";
            using var helperCoi = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var cois = helperCoi.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiUrl);
            ViewBag.listCois = cois.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamChinhXepKhuons/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamChinhXepKhuon>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            ViewBag.TitlePage = "Trọng Lượng Cối Theo Thành Phẩm";
            return View("~/Views/DanhMuc/XepKhuon/Chinh/TrongLuongCoiTheoThanhPhamView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoThanhPhams/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm", Func = "Thêm Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm")]
        public async Task<IActionResult> DoInsert(string maCoi, string maThanhPham, string maXuong, decimal trongLuongMax)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoThanhPhams/Insert";
            try
            {
                if (string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong) || trongLuongMax < 0 || trongLuongMax == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new TrongLuongCoiTheoThanhPham
                {
                    MaCoi = maCoi,
                    MaThanhPham = maThanhPham,
                    MaXuong = maXuong,
                    TrongLuongMax = trongLuongMax
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

        public async Task<IActionResult> GetsByMa(string maCoi, string maThanhPham, string maXuong)
        {
            if (string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoThanhPhams/GetsByMa/{maCoi}/{maThanhPham}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<TrongLuongCoiTheoThanhPham>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaCoi = item.MaCoi,
                        MaThanhPham = item.MaThanhPham,
                        MaXuong = item.MaXuong,
                        TrongLuongMax = item.TrongLuongMax
                        
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm", Func = "Sửa Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm")]
        public async Task<IActionResult> DoUpDate(string maCoi, string maThanhPham, string maXuong, decimal trongLuongMax)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoThanhPhams/Update/{maCoi}/{maThanhPham}/{maXuong}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong) || trongLuongMax < 0 || trongLuongMax == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new TrongLuongCoiTheoThanhPham
                {
                    TrongLuongMax = trongLuongMax

                };

                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");;
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
        [CustomAuthorize(Fu = "Danh Mục / Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm", Func = "Xóa Xếp Khuôn / Trọng Lượng Cối Theo Thành Phẩm")]
        public async Task<IActionResult> DoDelete(string maCoi, string maThanhPham, string maXuong)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrongLuongCoiTheoThanhPhams/Delete/{maCoi}/{maThanhPham}/{maXuong}";
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
