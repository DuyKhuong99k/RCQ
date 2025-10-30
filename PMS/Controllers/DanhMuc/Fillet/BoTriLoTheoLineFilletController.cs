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
    public class BoTriLoTheoLineFilletController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BoTriLoTheoLineFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Lô Theo Chuyền", Func = "Xem Fillet / Bố Trí Lô Theo Chuyền")]
        public async Task<IActionResult> Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BoTriLoTheoLineView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Bố Trí Lô Theo Chuyền";

            var apiLoFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LoTheoLines/GetDefaultLos";
            using var helperLoFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var los = await helperLoFillet.GetAsync<List<string>>(HttpContext, apiLoFilletUrl);
            ViewBag.ListLos = los?.ToList() ?? new List<string>();

            var apiLineFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LineFilletv2/GetAlls";
            using var helperLineFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var lines = await helperLineFillet.GetAsync<IEnumerable<LineFilletv2>>(HttpContext, apiLineFilletUrl); 
            ViewBag.listLines = lines.ToList();

            return View("~/Views/DanhMuc/Fillet/BoTriLoTheoLineView.cshtml");
        }

        public async Task<IEnumerable<object>> GetAllsFullField(DateTime ngay, string maLo, bool isCheckNow)
        {
            IEnumerable<object> dataSource = null;

            string endpoint = isCheckNow
                ? "GetsFullFieldLastNew"
                : "GetAllsFullField";

            string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LoTheoLines/{endpoint}/{ngay:yyyy-MM-dd}/{maLo}";

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

            return dataSource ?? Enumerable.Empty<object>();
        }

        public async Task<IEnumerable<LoTheoLine>> GetAlls()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LoTheoLines/GetAlls";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<LoTheoLine>>(HttpContext, apiUrl);
            return dataSource ?? Enumerable.Empty<LoTheoLine>();
        }


        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Lô Theo Chuyền", Func = "Thêm Fillet / Bố Trí Lô Theo Chuyền")]
        public async Task<IActionResult> DoInsert(string selectedIdsString, DateTime ngay, TimeSpan gio, string maLo,string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LoTheoLines/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(xuongId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maLines = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var loLines = new List<LoTheoLine>();
                int idCount = 1;
                foreach (var item in maLines)
                {
                    idCount++;
                    var items = await GetAlls();
                    var maxId = items.Any() ? items.Max(x => x.Id) + idCount : 1;
                    var loLine = new LoTheoLine
                    {
                        Id = maxId,
                        CodeId = xuongId,
                        Ngay = ngay,
                        Gio = gio,
                        MaLo = maLo,
                        MaLine = item,
                    };
                    loLines.Add(loLine);
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var loLine in loLines)
                {
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

        [CustomAuthorize(Fu = "Danh Mục / Fillet / Bố Trí Lô Theo Chuyền", Func = "Xoá Fillet / Bố Trí Lô Theo Chuyền")]
        public async Task<IActionResult> DoDelete(string selectedIdsStringLoLine)
        {
            try
            {
                string[] ids = selectedIdsStringLoLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var successCount = 0;
                var failureCount = 0;
                foreach (var id in ids)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/LoTheoLines/Delete/{id}";
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
