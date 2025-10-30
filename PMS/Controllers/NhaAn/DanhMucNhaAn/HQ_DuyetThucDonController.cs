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
using Azure.Identity;

namespace PMS.Controllers.NhaAn.DanhMucNhaAn
{
    [Authorize]
    public class HQ_DuyetThucDonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HQ_DuyetThucDonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "MonAnView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiLoaiMonAnUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_LoaiMonAns/GetAlls";
            using var helperLoaiMonAn = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var loaiMonAns = helperLoaiMonAn.GetAsync<IEnumerable<HQ_DuyetThucDon>>(HttpContext, apiLoaiMonAnUrl);
            ViewBag.loaiMonAns = loaiMonAns.Result.ToList();

            ViewBag.TitlePage = "Món Ăn";
            return View("~/Views/NhaAn/DanhMucNhaAn/HQ_DuyetThucDon/MonAnView.cshtml");
        }
        public async Task<IEnumerable<HQ_DuyetThucDon>> GetAlls()
        {
            IEnumerable<HQ_DuyetThucDon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DuyetThucDons/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<HQ_DuyetThucDon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DuyetThucDons/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        [CustomAuthorize(Fu = "Nhà Ăn / Loại Món Ăn", Func = "Thêm Loại Món Ăn")]
        public async Task<IActionResult> DoInsert(int thucDonId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DuyetThucDons/Insert";
            try
            {
                var userName = HttpContext.Session.GetString("Username");
                if (thucDonId == null || thucDonId <= 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                //var id = 1;
                //var dataSource = await GetAlls();
                //if (dataSource != null)
                //{
                //    var maxId = dataSource.Select(x => x.Id)
                //        .DefaultIfEmpty(0)
                //        .Max();
                //    id = maxId + 1;
                //}
                //else
                //{
                //    return Json(new
                //    {
                //        isSuccess = false,
                //        Mesages = "Lỗi!"
                //    });
                //}
                var model = new HQ_DuyetThucDon
                {
                    //Id = id,
                    NgayTao = DateTime.Now,
                    NguoiTao = userName,
                    ThucDonId = thucDonId,
                    GhiChu = "Duyệt thực đơn",
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
        public async Task<IActionResult> GetsByMa(int id)
        {
            if (id <= 0 || id == null)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DuyetThucDons/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<HQ_DuyetThucDon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        NgayTao = item.NgayTao,
                        NguoiTao = item.NguoiTao,
                        ThucDonId = item.ThucDonId,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        //[CustomAuthorize(Fu = "Nhà Ăn / Loại Món Ăn", Func = "Sửa Loại Món Ăn")]
        //public async Task<IActionResult> DoUpDate(int id, string ten,string loaiMonAnId, string ghiChu)
        //{
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DuyetThucDons/Update/{id}";
        //    try
        //    {
        //        // Kiểm tra dữ liệu đầu vào
        //        if (id <= 0 || string.IsNullOrEmpty(ten))
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Messages = "Vui lòng nhập đầy đủ thông tin."
        //            });
        //        }
        //        var model = new HQ_DuyetThucDon
        //        {
        //            Id = id,
        //            Ten = ten,
        //            LoaiMonAnId = loaiMonAnId,
        //            GhiChu = ghiChu
        //        };

        //        var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
        //        var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //        var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
        //        if (rl.Success)
        //        {
        //            return Json(new
        //            {
        //                isSuccess = rl.Success,
        //                Messages = rl.Message
        //            });
        //        }
        //        return Json(new
        //        {
        //            isSuccess = rl.Success,
        //            Messages = rl.Message
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            isSuccess = false,
        //            Messages = "Đã xảy ra lỗi: " + ex.Message
        //        });
        //    }
        //}
        [CustomAuthorize(Fu = "Nhà Ăn / Loại Món Ăn", Func = "Xóa Loại Món Ăn")]
        public async Task<IActionResult> DoDelete(int id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DuyetThucDons/Delete/{id}";
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
