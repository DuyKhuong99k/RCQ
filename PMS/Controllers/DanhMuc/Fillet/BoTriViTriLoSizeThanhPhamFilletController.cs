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
    public class BoTriViTriLoSizeThanhPhamFilletController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BoTriViTriLoSizeThanhPhamFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Vị Trí - Lô - Size - TP", Func = "Xem Fillet / Bố Trí Vị Trí - Lô - Size - TP")]
        public async Task<IActionResult> Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BoTriViTriLoSizeThanhPhamView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Bố Trí Vị Trí Lô Size Thành Phẩm";

            var apiLoFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LoTheoLines/GetDefaultLos";
            using var helperLoFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var los = await helperLoFillet.GetAsync<List<string>>(HttpContext, apiLoFilletUrl);
            ViewBag.ListLos = los?.ToList() ?? new List<string>();

            var apiViTriFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ViTriFillets/GetAlls";
            using var helperViTri = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var viTris = helperViTri.GetAsync<IEnumerable<ViTriFillet>>(HttpContext, apiViTriFilletUrl);
            ViewBag.listViTris = viTris.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeFillets/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeFillet>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
            using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl);
            ViewBag.listTPFLs = tpfls.Result.ToList();

            return View("~/Views/DanhMuc/Fillet/BoTriViTriLoSizeThanhPhamView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField(DateTime ngay, string maLo, bool isCheckNow)
        {
            IEnumerable<object> dataSource = null;

            string endpoint = isCheckNow
                ? "GetsFullFieldLastNew"
                : "GetAllsFullField";

            string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriLoSizeThanhPhams/{endpoint}/{ngay:yyyy-MM-dd}/{maLo}";

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

            return dataSource ?? Enumerable.Empty<object>();
        }
        public async Task<IEnumerable<BoTriLoSizeThanhPham>> GetAlls()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriLoSizeThanhPhams/GetAlls";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<BoTriLoSizeThanhPham>>(HttpContext, apiUrl);
            return dataSource ?? Enumerable.Empty<BoTriLoSizeThanhPham>();
        }

        public async Task<List<string>> GetLosWithDate(DateTime ngay)
        {
            var apiLoFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LoTheoLines/GetLosWithDate/{ngay.ToString("yyyy-MM-dd")}";

            using var helperLoFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helperLoFillet.GetAsync<List<string>>(HttpContext, apiLoFilletUrl);

            return dataSource ?? new List<string>();
        }

        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Vị Trí - Lô - Size - TP", Func = "Thêm Fillet / Bố Trí Vị Trí - Lô - Size - TP")]
        public async Task<IActionResult> DoInsert(string maLo, string maViTri, string maSize, string maSizePhu, string maThanhPham, DateOnly ngay, TimeOnly gio, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriLoSizeThanhPhams/Insert";
            try
            {
                if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maViTri) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maSizePhu) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(xuongId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }


                int idCount = 1;

                idCount++;
                var items = await GetAlls();
                var maxId = items.Any() ? items.Max(x => x.Id) + idCount : 1;
                var loLine = new BoTriLoSizeThanhPham
                {
                    Id = maxId,
                    CodeId = xuongId,
                    MaLo = maLo,
                    MaViTri = maViTri,
                    MaSize = maSize,
                    MaSizePhu = maSizePhu,
                    MaThanhPham = maThanhPham,
                    Ngay = ngay,
                    Gio = gio
                };


                var successCount = 0;
                var failureCount = 0;


                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(loLine));
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Vị Trí - Lô - Size - TP", Func = "Xoá Fillet / Bố Trí Vị Trí - Lô - Size - TP")]
        public async Task<IActionResult> DoDelete(string selectedIdsStringBoTriLoSizeThanhPham)
        {
            try
            {
                string[] ids = selectedIdsStringBoTriLoSizeThanhPham.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var successCount = 0;
                var failureCount = 0;
                foreach (var id in ids)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriLoSizeThanhPhams/Delete/{id}";
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
