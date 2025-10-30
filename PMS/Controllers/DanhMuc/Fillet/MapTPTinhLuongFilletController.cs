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

namespace PMS.Controllers.DanhMuc.Fillet
{
    [Authorize]
    public class MapTPTinhLuongFilletController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MapTPTinhLuongFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Map TPFillet Tính Lương", Func = "Xem Fillet / Map TPFillet Tính Lương")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "MapTPTinhLuongFilletView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaFillets/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
            using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl);
            ViewBag.listTPFLs = tpfls.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeFillets/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeFillet>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiSPTLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetAlls";
            using var helperSPTL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sptls = helperSPTL.GetAsync<IEnumerable<DG_SanPhamTinhLuong>>(HttpContext, apiSPTLUrl);
            ViewBag.listSPTLs = sptls.Result.ToList();



            ViewBag.TitlePage = "Map TPFillet Tính Lương Fillet";
            return View("~/Views/DanhMuc/Fillet/MapTPTLFilletView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MapThanhPhamFillets/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<MapThanhPhamFillet>> GetAlls()
        {
            IEnumerable<MapThanhPhamFillet> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MapThanhPhamFillets/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MapThanhPhamFillet>>(HttpContext, apiUrl);
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
                    return Json(new
                    {
                        isSuccess = true,
                        IsTangCa = false
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Map TPFillet Tính Lương", Func = "Thêm Fillet / Map TPFillet Tính Lương")]
        public async Task<IActionResult> DoInsert(string maLoaiCa, string maThanhPham, string maSize, string maBravo, bool isTangCa)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MapThanhPhamFillets/Insert";
            try
            {
                if (string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MapThanhPhamFillet
                {
                    MaLoaiCaFillet = maLoaiCa,
                    MaTPFillet = maThanhPham,
                    MaSize = maSize,
                    MaBravoFillet = maBravo,
                    IsTangCa = isTangCa
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
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
        public async Task<IActionResult> GetsByMa(string maLoaiCa, string maThanhPham, string maSize, bool isTangCa)
        {
            if (string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MapThanhPhamFillets/GetsByMa/{maLoaiCa}/{maThanhPham}/{maSize}/{isTangCa.ToString()}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MapThanhPhamFillet>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaLoaiCaFillet = item.MaLoaiCaFillet,
                        MaTPFillet = item.MaTPFillet,
                        MaSize = item.MaSize,
                        MaBravoFillet = item.MaBravoFillet,
                        IsTangCa = item.IsTangCa

                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Map TPFillet Tính Lương", Func = "Sửa Fillet / Map TPFillet Tính Lương")]
        public async Task<IActionResult> DoUpDate(string maLoaiCa, string maThanhPham, string maSize, string maBravo, string isTangCa)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MapThanhPhamFillets/Update/{maLoaiCa}/{maThanhPham}/{maSize}/{isTangCa.ToString()}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                bool booleanValue = bool.Parse(isTangCa);
                var model = new MapThanhPhamFillet
                {
                    MaLoaiCaFillet = maLoaiCa,
                    MaTPFillet = maThanhPham,
                    MaSize = maSize,
                    MaBravoFillet = maBravo,
                    IsTangCa = booleanValue

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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Map TPFillet Tính Lương", Func = "Xoá Fillet / Map TPFillet Tính Lương")]
        public async Task<IActionResult> DoDelete(string maLoaiCa, string maThanhPham, string maSize, string isTangCa)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MapThanhPhamFillets/Delete/{maLoaiCa}/{maThanhPham}/{maSize}/{isTangCa.ToString()}";
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
