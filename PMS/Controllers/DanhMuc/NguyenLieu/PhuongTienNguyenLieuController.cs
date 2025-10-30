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
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;

namespace PMS.Controllers.DanhMuc.NguyenLieu
{
    [Authorize]
    public class PhuongTienNguyenLieuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public PhuongTienNguyenLieuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Nguyên Liệu", Func = "Xem Nguyên Liệu / Phương Tiện Nguyên Liệu")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhuongTienNguyenLieuView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Phương Tiện Nguyên Liệu";
            return View("~/Views/DanhMuc/NguyenLieu/PhuongTienNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<PhuongTienChoNguyenLieu>> GetAlls()
        {
            IEnumerable<PhuongTienChoNguyenLieu> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> GetsByMa(string ma)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn phương tiện nguyên liệu!."
                });
            }
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetByMa/{ma}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongTienNguyenLieu = await helper.GetAsync<PhuongTienChoNguyenLieu>(HttpContext, apiUrl);
            if (phuongTienNguyenLieu != null)
            {
                return Json(new
                {
                    isSuccess = true,
                    Mesages = "Thành Công",
                    Ma = phuongTienNguyenLieu?.Ma,
                    Ten = phuongTienNguyenLieu?.Ten,
                    SuDung = phuongTienNguyenLieu?.SuDung,
                    IsHD = phuongTienNguyenLieu?.IsHD,
                    IsGhe = phuongTienNguyenLieu?.IsGhe,
                    VungNuoiId = phuongTienNguyenLieu?.VungNuoiId,
                    SoGhe = phuongTienNguyenLieu?.SoGhe,
                });
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> CreatDefautNew()
        {
            try
            {
                var dataSource = await GetAlls();
                if (dataSource != null)
                {
                    var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
                    .Max();
                    var id = (int.Parse(maxId) + 1).ToString("00");
                    return Json(new
                    {
                        isSuccess = true,
                        Ma = id,
                        SuDung = true,
                        IsHD = true,
                        IsGhe = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Không lấy được dữ liệu Phương Tiện Nguyên Liệu!"
                    });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Nguyên Liệu", Func = "Thêm Nguyên Liệu / Phương Tiện Nguyên Liệu")]
        public async Task<IActionResult> DoInsert(string ma, string ten, bool suDung, bool isHD, bool isGhe, string vungNuoiId, string soGhe)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/Insert";

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
                var model = new PhuongTienChoNguyenLieu
                {
                    Ma = ma,
                    Ten = ten,
                    SuDung = suDung,
                    IsHD = isHD,
                    IsGhe = isGhe,
                    VungNuoiId = vungNuoiId,
                    SoGhe = soGhe
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = rl.Success,
                        Messages = rl.Message,
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = rl.Success,
                        Messages = rl.Message,
                    });
                }
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Nguyên Liệu", Func = "Sửa Nguyên Liệu / Phương Tiện Nguyên Liệu")]
        public async Task<IActionResult> DoUpDate(string ma, string? ten, bool? suDung, bool? isHD, bool? isGhe, string vungNuoiId, string? soGhe)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/Update/{ma}";
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
                var model = new PhuongTienChoNguyenLieu
                {
                    Ma = ma,
                    Ten = ten,
                    SuDung = suDung,
                    IsHD = isHD??false,
                    IsGhe = isGhe??false,
                    VungNuoiId = vungNuoiId,
                    SoGhe = soGhe
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                if (rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = rl.Message
                    });
                }
                return Json(new
                {
                    isSuccess = false,
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Nguyên Liệu", Func = "Xoá Nguyên Liệu / Phương Tiện Nguyên Liệu")]
        public async Task<IActionResult> DoDelete(string ma)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/Delete/{ma}";

            try
            {
                 using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);

                if (rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = rl.Message
                    });
                }
                return Json(new
                {
                    isSuccess = false,
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
