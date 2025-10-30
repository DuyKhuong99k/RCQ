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
using AppViewModels;
using ViewModels.Repos.HQ;
using System.Data.Common;
namespace PMS.Controllers.DanhMuc.Fillet
{
    [Authorize]
    public class BoTriNVTheoSizeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BoTriNVTheoSizeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Nhân Viên Theo Size", Func = "Xem Fillet / Bố Trí Nhân Viên Theo Size")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BoTriNVTheoSizeView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Bố Trí Nhân Viên Theo Size";

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            ViewBag.listMaLos = items.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeFillets/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeFillet>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();
            return View("~/Views/DanhMuc/Fillet/BoTriNVTheoSizeView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField(string dateTime, string maLo, string maXuong)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dateTime != null)
            {
                string ngay = dateTime.Replace('/', '-');
                if (dataSource == null)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhanVienTheoSizes/GetAllsFullField/{ngay}/{maLo}/{maXuong}";
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                    dataSource = ViewBag.dataSource;
                }
            }
            return dataSource;
        }

        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Nhân Viên Theo Size", Func = "Thêm Fillet / Bố Trí Nhân Viên Theo Size")]
        public async Task<IActionResult> DoInsert(string selectedIdsString, DateTime ngay, string maLo, string maXuong, string maSize, TimeSpan thoiGianBatDau)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhanVienTheoSizes/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maNhanViens = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var nhanVienSizes = new List<BoTriNhanVienTheoSize>();
                foreach (var item in maNhanViens)
                {
                    var nhanVienSize = new BoTriNhanVienTheoSize
                    {
                        Ngay = ngay,
                        MaNhanVien = item,
                        MaSize = maSize,
                        MaXuong = maXuong,
                        SuDung = true,
                        ThoiGianBatDau = thoiGianBatDau,
                        MaLo = maLo,
                    };
                    nhanVienSizes.Add(nhanVienSize);
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var nhanVienSize in nhanVienSizes)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(nhanVienSize), Encoding.UTF8, "application/json");
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!response.Success)
                    {
                        failureCount++;
                    }
                    else
                    {
                        successCount++;
                    }
                }

                if (failureCount > 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"{failureCount} phần tử không thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $"{successCount} phần tử thành công."
                    });
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có.
                return Json(new
                {
                    isSuccess = false,
                    Messages = ex.Message
                });
            }
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Nhân Viên Theo Size", Func = "Xoá Fillet / Bố Trí Nhân Viên Theo Size")]
        public async Task<IActionResult> DoDelete(string selectedIdNhanVienSizes, string dateTime, string maLo, string maXuong)
        {
            try
            {
                string ngay = dateTime.Replace('/', '-');
                string[] ids = selectedIdNhanVienSizes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var successCount = 0;
                var failureCount = 0;
                foreach (var maNhanVien in ids)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhanVienTheoSizes/Delete/{maNhanVien}/{ngay}/{maLo}/{maXuong}";
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var rl = await helper.PostAsync(HttpContext, apiUrl, null);

                    if (!rl.Success)
                    {
                        failureCount++;
                    }
                    else
                    {
                        successCount++;
                    }

                }
                if (failureCount > 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"{failureCount} phần tử không thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $"{successCount} phần tử thành công."
                    });
                }
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Có lỗi xảy ra"
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Nhân Viên Theo Size", Func = "Sửa Fillet / Bố Trí Nhân Viên Theo Size")]
        public async Task<IActionResult> DoUpDate(string selectedIdNhanVienSizes, string dateTime, string maLo, string maXuong, string maSize, TimeSpan thoiGianBatDau)
        {

            try
            {
                string ngay = dateTime.Replace('/', '-');
                string[] ids = selectedIdNhanVienSizes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var successCount = 0;
                var failureCount = 0;
                foreach (var maNhanVien in ids)
                {
                    // Kiểm tra dữ liệu đầu vào
                    if (string.IsNullOrEmpty(dateTime) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maSize))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Vui lòng nhập đầy đủ thông tin."
                        });
                    }
                    DateTime ngayCV = DateTime.Parse(ngay);
                    var model = new BoTriNhanVienTheoSize
                    {
                        Ngay = ngayCV,
                        MaNhanVien = maNhanVien,
                        MaSize = maSize,
                        MaXuong = maXuong,
                        SuDung = true,
                        ThoiGianBatDau = thoiGianBatDau,
                        MaLo = maLo,
                    };
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriNhanVienTheoSizes/Update/{maNhanVien}/{ngay}/{maLo}/{maXuong}";
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!rl.Success)
                    {
                        failureCount++;
                    }
                    else
                    {
                        successCount++;
                    }

                }
                if (failureCount > 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"{failureCount} phần tử không thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $"{successCount} phần tử thành công."
                    });
                }
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
