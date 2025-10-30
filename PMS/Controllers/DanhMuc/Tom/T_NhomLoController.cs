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

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_NhomLoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public T_NhomLoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhomLoView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "NhomLoChatView";
            return View("~/Views/DanhMuc/Tom/NhomLoView.cshtml");
        }

        public async Task<IEnumerable<T_NhomLo>> GetAlls()
        {
            IEnumerable<T_NhomLo> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_NhomLo/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<T_NhomLo>>(HttpContext, apiUrl);
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

                    var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0").Max();
                    var id = (int.Parse(maxId) + 1).ToString("000000");
                    return Json(new
                    {
                        isSuccess = true,
                        Ma = id,
                        SuDung = true,
                        NgayTao = DateTime.Now,
                        DaKetThuc = false,
                        NgayBatDau = DateTime.Now,
                        NgayNguyenLieu = DateTime.Now,
                        NgayKetThuc = DateTime.Now
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
        public async Task<IActionResult> DoInsert(string ma, string ten, bool suDung, DateTime ngayNguyenLieu, bool daKetThuc, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_NhomLo/Insert";
            try
            {
                if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new T_NhomLo
                {
                    Ten = ten,
                    Ma = ma,
                    NgayTao = DateTime.Now,
                    SuDung = suDung,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc =  DateTime.Now,
                    NgayNguyenLieu = ngayNguyenLieu,
                    DaKetThuc = daKetThuc,
                    GhiChu = ghiChu
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (response.Success)
                {
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
        public async Task<IActionResult> GetsByMa(string ma)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn lô!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_NhomLo/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<T_NhomLo>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        SuDung = item?.SuDung,
                        Ma = item?.Ma,
                        Ten = item?.Ten,
                        NgayTao = item?.NgayTao,
                        NgayBatDau = item?.NgayBatDau,
                        NgayKetThuc = item?.NgayKetThuc,
                        NgayNguyenLieu = item?.NgayNguyenLieu,
                        DaKetThuc = item?.DaKetThuc,
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
        public async Task<IActionResult> DoUpDate(string ma, string ten, bool suDung, DateTime ngayNguyenLieu, bool daKetThuc, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_NhomLo/Update/{ma}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new T_NhomLo
                {
                    Ten = ten,
                    Ma = ma,
                    NgayTao = DateTime.Now,
                    SuDung = suDung,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now,
                    NgayNguyenLieu = ngayNguyenLieu,
                    DaKetThuc = daKetThuc,
                    GhiChu = ghiChu
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
        public async Task<IActionResult> DoDelete(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_NhomLo/Delete/{ma}";
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
