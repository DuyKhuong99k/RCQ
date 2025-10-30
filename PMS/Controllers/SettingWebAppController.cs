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
using Azure.Core;
using Models.Repos.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using Models.Repos;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PMS.Attrs;

namespace PMS.Controllers
{
    [Authorize]
    public class SettingWebAppController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SettingWebAppController(IHttpClientFactory httpClientFactory) { _httpClientFactory = httpClientFactory; }
        public IActionResult Index() { return View(); }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Tài Khoản")]
        public IActionResult Account() { ViewBag.TitlePage = "Tài Khoản"; return View(); }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Thông Báo")]
        public IActionResult Notifications() { return View(); }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Kết Nối")]
        public IActionResult Connections() { return View(); }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Dashboard")]
        public IActionResult Dashboard()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "DashboardView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Dashboard";
            return View();
        }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Quản Lý Máy Cân")]
        public IActionResult MayCanManager()
        {
            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();
            ViewBag.AppKVValues = Enum.GetValues(typeof(Vars.AppKV)).Cast<Vars.AppKV>();
            ViewBag.TitlePage = "Quản Lý Máy Cân";
            return View();
        }
        public async Task<IEnumerable<MayCan>> GetAllMayCanManagers()
        {
            IEnumerable<MayCan> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayCans/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MayCan>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            //// Ánh xạ lại dữ liệu cho trường WKv và AType
            //foreach (var mayCan in dataSource)
            //{
            //    // Ánh xạ giá trị WKv
            //    mayCan.WKVText = (Vars.AppKV)(int)Enum.Parse(typeof(Vars.AppKV), Enum.GetName(typeof(Vars.AppKV), mayCan.WKv));
            //    // Ánh xạ giá trị AType
            //    mayCan.ATypeText = (Vars.AppType)(int)Enum.Parse(typeof(Vars.AppType), Enum.GetName(typeof(Vars.AppType), mayCan.AType));
            //}

            return dataSource;
        }

        public async Task<IActionResult> CreatDefautNewMayCanManagers()
        {
            try
            {
                var dataSource = await GetAllMayCanManagers();
                if (dataSource != null)
                {

                    var maxId = dataSource.Where(x => int.TryParse(x.Id, out int rl)).Select(x => x.Id).DefaultIfEmpty("0")
                    .Max();
                    var id = (int.Parse(maxId) + 1).ToString("000");
                    return Json(new
                    {
                        isSuccess = true,
                        Id = id,
                        SuDung = true,
                        SCode = "XXX",
                        Par1 = "XXX",
                        Par2 = "XXX",
                        Par3 = "XXX",
                        Par4 = "XXX",
                        Idx = 0,
                        IsSuDungMauThanhPham = true

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
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Thêm Quản Lý Máy Cân")]
        public async Task<IActionResult> DoInsertMayCanManager(string id, string displayName,string sCode,int wKv,int aType, string mType,string xuongId ,bool isSuDungMauThanhPham, string par1, string par2, string par3, string par4,int idx)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayCans/Insert";
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(displayName))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MayCan
                {
                    Id = id,
                    DisplayName = displayName,
                    WKv = (Vars.AppKV)wKv,
                    AType = (Vars.AppType)aType,
                    MType = mType,
                    MaXuong = xuongId,
                    IsSuDungMauThanhPham = isSuDungMauThanhPham,
                    SCode = sCode,
                    Par1 = par1,
                    Par2 = par2,
                    Par3 = par3,
                    Par4 = par4,
                    Idx = idx

                };
                var dataTuple=new Tuple<string>(JsonConvert.SerializeObject(model));
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
        public async Task<IActionResult> GetsByMaMayCanManager(string id)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayCans/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MayCan>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        DisplayName = item.DisplayName,
                        SCode = item.SCode,
                        Par1 = item.Par1,
                        Par2 = item.Par2,
                        Par3 = item.Par3,
                        Par4 = item.Par4,
                        Idx = item.Idx,
                        MType = item.MType,
                        WKv = item.WKv,
                        MaXuong = item.MaXuong,
                        IsSuDungMauThanhPham = item.IsSuDungMauThanhPham,
                        AType = item.AType
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Sửa Quản Lý Máy Cân")]
        public async Task<IActionResult> DoUpDateManager(string id, string displayName,string sCode,int wKv,int aType, string mType,string xuongId ,bool isSuDungMauThanhPham, string par1, string par2, string par3, string par4,int idx)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayCans/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(displayName))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MayCan
                {
                    Id = id,
                    DisplayName = displayName,
                    WKv = (Vars.AppKV)wKv,
                    AType = (Vars.AppType)aType,
                    MType = mType,
                    MaXuong = xuongId,
                    IsSuDungMauThanhPham = isSuDungMauThanhPham,
                    SCode = sCode,
                    Par1 = par1,
                    Par2 = par2,
                    Par3 = par3,
                    Par4 = par4,
                    Idx = idx
                };
                 var dataTuple=new Tuple<string>(JsonConvert.SerializeObject(model));
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
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Xoá Quản Lý Máy Cân")]
        public async Task<IActionResult> DoDeleteManager(string id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayCans/Delete/{id}";
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
        public async Task<SettingDashboard> Get()
        {
            var apiUrl3 = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/SettingWebApp/Get";
            using var helper3 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var numberDate = await helper3.GetAsync<SettingDashboard>(HttpContext, apiUrl3);
            return numberDate;
        }

        public async Task<SettingDashboard> GetFromCokie()
        {
            var numberDate = User.FindFirst("NumberDate")?.Value;
            if (numberDate == null || numberDate.Trim() == "")
            {
                numberDate = "0";
            }
            if (!int.TryParse(numberDate, out int num))
            {
                num = 0;
            }
            return new SettingDashboard { NumberDate = num };
        }

        public async Task<IActionResult> GetsSDB()
        {
            try
            {
                var dataSource = await GetFromCokie();
                if (dataSource != null)
                {
                    return Json(new { isSuccess = true, NumberDate = dataSource.NumberDate });
                }
                else
                {
                    return Json(new { isSuccess = false, Mesages = "Lỗi!" });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> DoUpDateSDB(int numberDate)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/SettingWebApp/UpdateSDB/{numberDate}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (numberDate == null)
                {
                    return Json(new { isSuccess = false, Messages = "Vui lòng nhập đầy đủ thông tin." });
                }
                var model = new SettingDashboard { NumberDate = numberDate };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    "application/json");
                //using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                //var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                //if(rl.Success)
                //{
                HttpContext.Session.Remove("NumberDate");
                HttpContext.Session.SetString("NumberDate", numberDate.ToString());
                if (User.Identity.IsAuthenticated)
                {
                    var claims = User.Claims.ToList(); // Chuyển danh sách claims sang danh sách có thể chỉnh sửa

                    // Tìm claim có tên "NumberDate" trong danh sách claims
                    var numberDateClaim = claims.FirstOrDefault(c => c.Type == "NumberDate");

                    // Nếu claim tồn tại, cập nhật giá trị của nó
                    if (numberDateClaim != null)
                    {
                        claims.Remove(numberDateClaim); // Xóa claim cũ
                        claims.Add(new Claim("NumberDate", numberDate.ToString())); // Thêm claim mới
                    }

                    // Tạo lại ClaimsPrincipal với danh sách claims đã được cập nhật
                    var claimsIdentity = new ClaimsIdentity(claims, User.Identity.AuthenticationType);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Cập nhật thông tin xác thực của người dùng trong HttpContext
                    HttpContext.SignInAsync(
                        User.Identity.AuthenticationType,
                        claimsPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true // Đặt lại các thuộc tính khác nếu cần
                        });
                    //}

                    return Json(new { isSuccess = true, Messages = "OK" });
                }
                return Json(new { isSuccess = false, Messages = "Error Cokie" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, Messages = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
    }
}
