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
    public class BoTriNVTheoChuyenViTriFilletController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BoTriNVTheoChuyenViTriFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Nhân Viên Theo Chuyền Và Vị Trí", Func = "Xem Fillet / Bố Trí Nhân Viên Theo Chuyền Và Vị Trí")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BoTriNhanVienTheoChuyenViTriView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Bố Trí Nhân Viên Theo Chuyền Và Vị Trí";

            var apiLineFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LineFilletv2/GetAlls";
            using var helperLineFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var lines = helperLineFillet.GetAsync<IEnumerable<LineFilletv2>>(HttpContext, apiLineFilletUrl);
            ViewBag.listLines = lines.Result.ToList();

            var apiViTriFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ViTriFillets/GetAlls";
            using var helperViTri = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var viTris = helperViTri.GetAsync<IEnumerable<ViTriFillet>>(HttpContext, apiViTriFilletUrl);
            ViewBag.listViTris = viTris.Result.ToList();
            return View("~/Views/DanhMuc/Fillet/BoTriNhanVienTheoChuyenViTriView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField(DateTime ngay, string maLine, string maViTri, bool isCheckNow)
        {
            IEnumerable<object> dataSource = null;

            string endpoint = isCheckNow
                ? "GetsFullFieldLastNew"
                : "GetAllsFullField";

            string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoLines/{endpoint}/{ngay:yyyy-MM-dd}/{maLine}/{maViTri}";

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

            return dataSource ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<NhanVienTheoLine>> GetAlls()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoLines/GetAlls";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<NhanVienTheoLine>>(HttpContext, apiUrl);
            return dataSource ?? Enumerable.Empty<NhanVienTheoLine>();
        }


        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Nhân Viên Theo Chuyền Và Vị Trí", Func = "Thêm Fillet / Bố Trí Nhân Viên Theo Chuyền Và Vị Trí")]
        public async Task<IActionResult> DoInsert(string selectedIdsString, DateTime ngay, TimeSpan gio, string maLine, string maViTri, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoLines/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(maLine) || string.IsNullOrEmpty(maViTri) || string.IsNullOrEmpty(xuongId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maNhanViens = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var nhanVienSizes = new List<NhanVienTheoLine>();
                int idCount = 1;
                foreach (var item in maNhanViens)
                {
                    idCount++;
                    var items = await GetAlls();
                    var maxId = items.Any() ? items.Max(x => x.Id) + idCount : 1;
                    var nhanVienSize = new NhanVienTheoLine
                    {
                        Id = maxId,
                        CodeId = xuongId,
                        Ngay = ngay,
                        Gio = gio,
                        MaNhanVien = item,
                        MaLine = maLine,
                        MaViTri = maViTri,
                    };
                    nhanVienSizes.Add(nhanVienSize);
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var nhanVienSize in nhanVienSizes)
                {
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(nhanVienSize));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Nhân Viên Theo Chuyền Và Vị Trí", Func = "Xoá Fillet / Bố Trí Nhân Viên Theo Chuyền Và Vị Trí")]
        public async Task<IActionResult> DoDelete(string selectedIdsStringLineStaff)
        {
            try
            {
                string[] ids = selectedIdsStringLineStaff.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var successCount = 0;
                var failureCount = 0;
                foreach (var id in ids)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoLines/Delete/{id}";
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
