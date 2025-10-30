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
using PMS.Attrs;

namespace PMS.Controllers.DanhMuc.NguyenLieu
{
    [Authorize]
    public class BanCatTietController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BanCatTietController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Bàn Cắt Tiết", Func = "Xem Nguyên Liệu / Bàn Cắt Tiết")]
        public async Task<IActionResult> Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BanCatTietView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Bàn cắt tiết";
            return View("~/Views/DanhMuc/NguyenLieu/BanCatTietView.cshtml");
        }
        public async Task<IEnumerable<BanCatTiet>> GetAllBanCatTiets()
        {
            IEnumerable<BanCatTiet> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanCatTiet/GetAllBanCatTiets";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<BanCatTiet>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew()
        {
            try
            {
                var dataSource = await GetAllBanCatTiets();
                if (dataSource != null)
                {

                    var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
                    .Max();
                    var id = (int.Parse(maxId) + 1).ToString("000");
                    return Json(new
                    {
                        isSuccess = true,
                        SuDung = true,
                        IsShowX2 = false,
                        Y2 = 0,
                        Y1 = 0,
                        X2 = 0,
                        X1 = 0,
                        ColSpanX2 = 1,
                        ColSpanX1 = 1,
                        IsShowX1 = false,
                        DoUuTienX1 = 0,
                        DoUuTienX2 = 0,
                        ThoiGianQuangDuongPheu2X2 = 1000,
                        ThoiGianQuangDuongPheu2X1 = 1000,
                        ThoiGianQuangDuongPheu1X2 = 1000,
                        ThoiGianQuangDuongPheu1X1 = 1000,
                        SoLanChiaCaX2 = 1,
                        SoLanChiaCaX1 = 1,
                        IsKhoaX2 = false,
                        IsDetect = false,
                        IsFalse = false,
                        IsKhoaX1 = false,
                        Ma = id,
                        TimeOpen = new TimeSpan(0, 0, 0, 0),
                        PlcAdr = "00",
                        PlcValue = false,
                        PlcYValue = false,
                        PlcYadr = "00",
                        VongChiaCa = 1,
                        ViTri_HX1 = 0,
                        ViTri_HX2 = 0,
                        ThoiGianNhanCaX1 = 1000,
                        ThoiGianNhanCaX2 = 1000,
                        PlcOffAdr = "00",
                        PlcOffValue = false,
                        //TimeManual = new TimeSpan(0, 0, 0, 0),
                        IsCaMuoiX1 = false,
                        IsCaMuoiX2 = false,
                        //IsEnabled = true
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Bàn Cắt Tiết", Func = "Thêm Nguyên Liệu / Bàn Cắt Tiết")]
        public async Task<IActionResult> DoInsert(string maBanCatTiet, string tenBanCatTiet, bool isShowX1, bool isShowX2, bool isCaMuoiX1, bool isCaMuoiX2, int x1, int y1, int x2, int y2, int colSpanX1, int colSpanX2, int doUuTienX1, int doUuTienX2, int soLanChiaCaX1, int soLanChiaCaX2, int thoiGianQuangDuongPheu1X1, int thoiGianQuangDuongPheu1X2, int thoiGianQuangDuongPheu2X1, int thoiGianQuangDuongPheu2X2, int thoiGianNhanCaX1, int thoiGianNhanCaX2, int viTri_HX1, int viTri_HX2, string plcAdr, string plcYadr, bool isKhoaX1, bool isKhoaX2, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanCatTiet/InsertBanCatTiet";
            try
            {
                if (string.IsNullOrEmpty(maBanCatTiet) || string.IsNullOrEmpty(tenBanCatTiet))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var banCatTietModel = new BanCatTiet
                {
                    Ma = maBanCatTiet,
                    Ten = tenBanCatTiet,
                    IsShowX1 = isShowX1,
                    IsShowX2 = isShowX2,
                    IsCaMuoiX1 = isCaMuoiX1,
                    IsCaMuoiX2 = isCaMuoiX2,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    ColSpanX1 = colSpanX1,
                    ColSpanX2 = colSpanX2,
                    DoUuTienX1 = doUuTienX1,
                    DoUuTienX2 = doUuTienX2,
                    SoLanChiaCaX1 = soLanChiaCaX1,
                    SoLanChiaCaX2 = soLanChiaCaX2,
                    ThoiGianQuangDuongPheu1X1 = thoiGianQuangDuongPheu1X1,
                    ThoiGianQuangDuongPheu1X2 = thoiGianQuangDuongPheu1X2,
                    ThoiGianQuangDuongPheu2X1 = thoiGianQuangDuongPheu2X1,
                    ThoiGianQuangDuongPheu2X2 = thoiGianQuangDuongPheu2X2,
                    ThoiGianNhanCaX1 = thoiGianNhanCaX1,
                    ThoiGianNhanCaX2 = thoiGianNhanCaX2,
                    ViTri_HX1 = viTri_HX1,
                    ViTri_HX2 = viTri_HX2,
                    PlcAdr = plcAdr,
                    PlcYadr = plcYadr,
                    IsKhoaX1 = isKhoaX1,
                    IsKhoaX2 = isKhoaX2,
                    SuDung = suDung,
                    //khoong lay thoong tin
                    IsDetect = false,
                    IsFalse = false,
                    TimeOpen = TimeSpan.Zero,
                    VongChiaCa = 0,
                    PlcYValue = false,
                    PlcValue = false,
                    PlcOffAdr = "",
                    PlcOffValue = false
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(banCatTietModel), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                if (response.Success)
                {
                    // Đăng ký thành công
                    return Json(new
                    {
                        isSuccess = response.Success,
                        Messages = response.Message,
                    });
                }
                return Json(new
                {
                    isSuccess = response.Success,
                    Messages = response.Message,
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
        public async Task<IActionResult> GetBanCatTietByMaBanCatTiet(string maBanCatTiet)
        {
            if (string.IsNullOrEmpty(maBanCatTiet))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn bàn cắt tiết!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanCatTiet/GetBanCatTietByMaBanCatTiet/{maBanCatTiet}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<BanCatTiet>(HttpContext, apiUrl);

                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        SuDung = item?.SuDung,
                        IsShowX2 = item?.IsShowX2,
                        Y2 = item?.Y2,
                        Y1 = item?.Y1,
                        X2 = item?.X2,
                        X1 = item?.X1,
                        ColSpanX2 = item?.ColSpanX2,
                        ColSpanX1 = item?.ColSpanX1,
                        IsShowX1 = item?.IsShowX1,
                        DoUuTienX1 = item?.DoUuTienX1,
                        DoUuTienX2 = item?.DoUuTienX2,
                        ThoiGianQuangDuongPheu2X2 = item?.ThoiGianQuangDuongPheu2X2,
                        ThoiGianQuangDuongPheu2X1 = item?.ThoiGianQuangDuongPheu2X1,
                        ThoiGianQuangDuongPheu1X2 = item?.ThoiGianQuangDuongPheu1X2,
                        ThoiGianQuangDuongPheu1X1 = item?.ThoiGianQuangDuongPheu1X1,
                        SoLanChiaCaX2 = item?.SoLanChiaCaX2,
                        SoLanChiaCaX1 = item?.SoLanChiaCaX1,
                        IsKhoaX2 = item?.IsKhoaX2,
                        IsDetect = item?.IsDetect,
                        IsFalse = item?.IsFalse,
                        IsKhoaX1 = item?.IsKhoaX1,
                        Ma = item?.Ma,
                        TimeOpen = item?.TimeOpen,
                        PlcAdr = item?.PlcAdr,
                        PlcValue = item?.PlcValue,
                        PlcYValue = item?.PlcYValue,
                        PlcYadr = item?.PlcYadr,
                        VongChiaCa = item?.VongChiaCa,
                        ViTri_HX1 = item?.ViTri_HX1,
                        ViTri_HX2 = item?.ViTri_HX2,
                        ThoiGianNhanCaX1 = item?.ThoiGianNhanCaX1,
                        ThoiGianNhanCaX2 = item?.ThoiGianNhanCaX2,
                        PlcOffAdr = item?.PlcOffAdr,
                        PlcOffValue = item?.PlcOffValue,
                        IsCaMuoiX1 = item?.IsCaMuoiX1,
                        IsCaMuoiX2 = item?.IsCaMuoiX2,
                        Ten = item?.Ten
                    });
                }


            }

            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Bàn Cắt Tiết", Func = "Sửa Nguyên Liệu / Bàn Cắt Tiết")]
        public async Task<IActionResult> DoUpDate(string maBanCatTiet, string tenBanCatTiet, bool isShowX1, bool isShowX2, bool isCaMuoiX1, bool isCaMuoiX2, int x1, int y1, int x2, int y2, int colSpanX1, int colSpanX2, int doUuTienX1, int doUuTienX2, int soLanChiaCaX1, int soLanChiaCaX2, int thoiGianQuangDuongPheu1X1, int thoiGianQuangDuongPheu1X2, int thoiGianQuangDuongPheu2X1, int thoiGianQuangDuongPheu2X2, int thoiGianNhanCaX1, int thoiGianNhanCaX2, int viTri_HX1, int viTri_HX2, string plcAdr, string plcYadr, bool isKhoaX1, bool isKhoaX2, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanCatTiet/UpdateBanCatTiet/{maBanCatTiet}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maBanCatTiet) || string.IsNullOrEmpty(tenBanCatTiet))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var banCatTietModel = new BanCatTiet
                {
                    Ma = maBanCatTiet,
                    Ten = tenBanCatTiet,
                    IsShowX1 = isShowX1,
                    IsShowX2 = isShowX2,
                    IsCaMuoiX1 = isCaMuoiX1,
                    IsCaMuoiX2 = isCaMuoiX2,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    ColSpanX1 = colSpanX1,
                    ColSpanX2 = colSpanX2,
                    DoUuTienX1 = doUuTienX1,
                    DoUuTienX2 = doUuTienX2,
                    SoLanChiaCaX1 = soLanChiaCaX1,
                    SoLanChiaCaX2 = soLanChiaCaX2,
                    ThoiGianQuangDuongPheu1X1 = thoiGianQuangDuongPheu1X1,
                    ThoiGianQuangDuongPheu1X2 = thoiGianQuangDuongPheu1X2,
                    ThoiGianQuangDuongPheu2X1 = thoiGianQuangDuongPheu2X1,
                    ThoiGianQuangDuongPheu2X2 = thoiGianQuangDuongPheu2X2,
                    ThoiGianNhanCaX1 = thoiGianNhanCaX1,
                    ThoiGianNhanCaX2 = thoiGianNhanCaX2,
                    ViTri_HX1 = viTri_HX1,
                    ViTri_HX2 = viTri_HX2,
                    PlcAdr = plcAdr,
                    PlcYadr = plcYadr,
                    IsKhoaX1 = isKhoaX1,
                    IsKhoaX2 = isKhoaX2,
                    SuDung = suDung,
                    //khoong lay thoong tin
                    IsDetect = false,
                    IsFalse = false,
                    TimeOpen = TimeSpan.Zero,
                    VongChiaCa = 0,
                    PlcYValue = false,
                    PlcValue = false,
                    PlcOffAdr = "",
                    PlcOffValue = false
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(banCatTietModel), Encoding.UTF8, "application/json");
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Bàn Cắt Tiết", Func = "Xoá Nguyên Liệu / Bàn Cắt Tiết")]
        public async Task<IActionResult> DeleteNhanVien(string maBanCatTiet)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanCatTiet/DeleteBanCatTiet/{maBanCatTiet}";
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
