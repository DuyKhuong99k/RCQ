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

namespace PMS.Controllers.DanhMuc.NguyenLieu
{
    [Authorize]
    public class PhuongTienTaiTrongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public PhuongTienTaiTrongController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Tải Trọng", Func = "Xem Nguyên Liệu / Phương Tiện Tải Trọng")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhuongTienTaiTrongView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var listPhuongTiens = helper.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiUrl);
            ViewBag.listPhuongTiens = listPhuongTiens.Result.ToList();
            return View("~/Views/DanhMuc/NguyenLieu/PhuongTienTaiTrongView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAlls()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TaiTrong/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> GetByMaPhuongTienAndNgayApDung(string maPhuongTien, DateTime ngayApDung)
        {
            if (string.IsNullOrEmpty(maPhuongTien) || ngayApDung == null)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn phương tiện tải trọng!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TaiTrong/GetByMaPhuongTienAndNgayApDung/{maPhuongTien}/{ngayApDung.ToString("yyyy-MM-dd")}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var item = await helper.GetAsync<PhuongTien_TaiTrong>(HttpContext, apiUrl);
            if (item != null)
            {
                return Json(new
                {
                    isSuccess = true,
                    Mesages = "Thành Công",
                    MaPhuongTien = item.MaPhuongTien,
                    NgayApDung = item.NgayApDung.ToString("yyyy-MM-dd"),
                    TaiTrong = item.TaiTrong,
                    GhiChu = item.GhiChu
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
                return Json(new
                {
                    isSuccess = true,
                    TaiTrong = 0,
                    NgayApDung = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Lỗi!" + ex.Message.ToString()
                });
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Tải Trọng", Func = "Thêm Nguyên Liệu / Phương Tiện Tải Trọng")]
        public async Task<IActionResult> DoInsert(string maPhuongTien, DateTime ngayApDung, decimal taiTrong, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TaiTrong/Insert";

            try
            {
                if (string.IsNullOrEmpty(maPhuongTien) || ngayApDung == null || taiTrong == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhuongTien_TaiTrong
                {
                    MaPhuongTien = maPhuongTien,
                    NgayApDung = ngayApDung,
                    TaiTrong = taiTrong,
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Tải Trọng", Func = "Sửa Nguyên Liệu / Phương Tiện Tải Trọng")]
        public async Task<IActionResult> DoUpDate(string maPhuongTien, DateTime ngayApDung, decimal taiTrong, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TaiTrong/Update/{maPhuongTien}/{ngayApDung.ToString("yyyy-MM-dd")}";
            try
            {
                if (string.IsNullOrEmpty(maPhuongTien) || ngayApDung == null || taiTrong == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhuongTien_TaiTrong
                {
                    MaPhuongTien = maPhuongTien,
                    NgayApDung = ngayApDung,
                    TaiTrong = taiTrong,
                    GhiChu = ghiChu
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Phương Tiện Tải Trọng", Func = "Xoá Nguyên Liệu / Phương Tiện Tải Trọng")]
        public async Task<IActionResult> DoDelete(string maPhuongTien, DateTime ngayApDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TaiTrong/Delete/{maPhuongTien}/{ngayApDung.ToString("yyyy-MM-dd")}";

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
