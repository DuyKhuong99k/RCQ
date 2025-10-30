using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Models;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Security.Policy;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using Models;
using Azure.Core;
using Models.Repos.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using System.Net.Http.Json;

using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;
using System;

namespace PMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

        }
        private readonly Random _random = new Random();
        private List<Product> _oldData = new List<Product>(); // Lưu trữ dữ liệu cũ.
        private static int _stt = 1; // Thêm một biến static để lưu giữ giá trị của STT.

        public async Task<IActionResult> Index()
        {
            return RedirectToAction("LoginBridge", "Authentication",new {url = "Home/Index/"}); // Token tồn tại trong cookie
           // return View();
        }
     
        public async Task<IActionResult> GetAllsXuong()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = helper.GetAsync<List<XiNghiep>>(HttpContext, apiUrl);
            var datas = dataSource.Result.ToList();
            return Json(new
            {
                isSuccess = true,
                Messages = "",
                datas = datas
            });
        }
        public async Task<IActionResult> ChangeXuong(string xuongId)
        {
            try
            {
                if (string.IsNullOrEmpty(xuongId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Chưa chọn xưởng!."
                    });
                }
                //xoá thông tin sesstion của xưởng hiện tại
                HttpContext.Session.Remove("XuongId");
                HttpContext.Session.Remove("XuongName");
                //cập nhật lại thông tin sesstion của xưởng mới
                HttpContext.Session.SetString("XuongId", xuongId);
                var apiUrl2 = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetXuongById/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<XiNghiep>(HttpContext, apiUrl2);
                HttpContext.Session.SetString("XuongName", item.Ten.ToString());

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Chuyển xưởng thành công!."
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    isSuccess = false,
                    Messages = "Có lỗi khi chuyển xưởng" + ex.ToString()
                });
            }

        }

        [HttpPost]
        public IActionResult GetPagedData(int page, int pageSize)
        {
            // Tính toán chỉ lấy các dòng dữ liệu cần hiển thị trên trang hiện tại.
            var pagedData = _oldData.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Json(new { items = pagedData, totalCount = _oldData.Count });
        }
        // Action trả về dữ liệu ngẫu nhiên.
        [HttpGet]
        public IActionResult GetRandomData()
        {
            var product = new Product
            {
                STT = _stt++,
                TenSanPham = "Product " + _random.Next(1, 100),
                TrongLuong = _random.NextDouble() * 10,
                Size = (_random.Next(0, 2) == 0) ? "Small" : "Medium"
            };

            _oldData.Insert(0, product); // Thêm dữ liệu mới vào đầu danh sách.

            return Json(product);
        }

        public class Product
        {
            public int STT { get; set; }
            public string TenSanPham { get; set; }
            public double TrongLuong { get; set; }
            public string Size { get; set; }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //public IActionResult DBTongHopTPDinhDinhHinh()
        //{
        //    return PartialView();
        //}
        public class HubsMayCanModel
        {
            public string NameMayCanHubs { get; set; }
            public string HubsMayCanUrl { get; set; }
            public int TypeMayCanHubs { get; set; }
        }

        //public string GetJsonData(string nameMayCanHubs)
        //{
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "HubsUrl.json");

        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return null;
        //    }

        //    var jsonData = System.IO.File.ReadAllText(filePath);

        //     Deserialize dữ liệu thành đối tượng TableNameJsonData
        //    var hubsMayCanData = JsonConvert.DeserializeObject<HubsMayCanModel>(jsonData);

        //     Trả về danh sách TableName từ đối tượng TableNameJsonData
        //    return hubsMayCanData?.HubsMayCanUrl;
        //}
        public List<HubsMayCanModel> GetAllHubsUrlJsonData()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "HubsUrl.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var jsonData = System.IO.File.ReadAllText(filePath);

            // Deserialize dữ liệu thành danh sách các đối tượng HubsMayCanModel
            var hubsMayCanDataList = JsonConvert.DeserializeObject<List<HubsMayCanModel>>(jsonData);

            return hubsMayCanDataList;
        }
        public HubsMayCanModel GetJsonData(string nameMayCanHubs)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "HubsUrl.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var jsonData = System.IO.File.ReadAllText(filePath);

            // Deserialize dữ liệu thành danh sách các đối tượng HubsMayCanModel
            var hubsMayCanDataList = JsonConvert.DeserializeObject<List<HubsMayCanModel>>(jsonData);

            // Tìm đối tượng có NameMayCanHubs trùng với nameMayCanHubs được cung cấp
            var selectedHubsMayCan = hubsMayCanDataList.FirstOrDefault(h => h.NameMayCanHubs == nameMayCanHubs);

            return selectedHubsMayCan;
        }
        [CustomAuthorize(Fu = "Hubs", Func = "Máy Cân")]
        public IActionResult NavigateHubsMayCan(string nameMayCanHubs)
        {
            try
            {
                var hubMayCanList = GetJsonData(nameMayCanHubs);
                if (hubMayCanList != null)
                {
                    var userName = HttpContext.Session.GetString("Username") ?? string.Empty;
                    var codeEncry = AppViewModels.AppViewModel.Instance.HubsMayCanCODE;

                    var encryptedCode = Security.Crypt.ED.EncryptString(codeEncry);

                    string sanitizedCode;

                    if (encryptedCode.Contains("/"))
                    {
                        // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
                        sanitizedCode = encryptedCode.Replace("/", "_");
                    }
                    else
                    {
                        // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
                        sanitizedCode = encryptedCode;
                    }

                    var url = $@"{hubMayCanList.HubsMayCanUrl}";

                    return Json(new
                    {
                        isSuccess = true,
                        url = url
                    });
                }
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Có lỗi khi chuyển hướng trang"
                });

            }
            catch (Exception ex)
            {

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Có lỗi khi chuyển hướng trang" + ex.ToString()
                });
            }

        }
        //test push trên máy tính ở nhà của chắt

        //public async Task<IActionResult> AutoAlertExpityNotice()
        //{
        //    try
        //    {
        //        return Json(new
        //        {
        //            isSuccess = true,
        //            Messages = $@"HẾT HẠN SỬ DỤNG!"
        //        });
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(new
        //        {
        //            isSuccess = false,
        //            Messages = "Đã xảy ra lỗi: " + ex.Message.ToString()
        //        });
        //    }
        //}


    }
}