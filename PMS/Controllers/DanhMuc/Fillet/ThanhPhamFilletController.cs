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
using ViewModels.Repos.HQ;

namespace PMS.Controllers.DanhMuc.Fillet
{
    [Authorize]
    public class ThanhPhamFilletController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ThanhPhamFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm", Func = "Xem Fillet / Thành Phẩm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ThanhPhamFilletView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Thành Phẩm Fillet";
            return View("~/Views/DanhMuc/Fillet/ThanhPhamFilletView.cshtml");
        }
        public async Task<IEnumerable<MaThanhPhamFillet>> GetAlls()
        {
            IEnumerable<MaThanhPhamFillet> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiUrl);
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

                    var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
                    .Max();
                    var id = (int.Parse(maxId) + 1).ToString("000");
                    return Json(new
                    {
                        isSuccess = true,
                        SuDung = true,
                        Min = 0,
                        Max = 999,
                        MaCa = LoaiCaFilletViewModel.Instance.Items.Where(x => x.SuDung == true).FirstOrDefault()?.Ma,
                        IsCaMuoi = false,
                        IsSoChe = false,
                        Ma = id,
                        ThoiGianTren1kgSeconds = 0,
                        TrangThaiThanhPham = "NORMAL",
                        DinhMucHaoHut = 1,
                        CodeId = XiNghiepViewModel.Instance.XiNghiepSelectedItem?.CodeId,
                        IsNotSetByTime = false,
                        KhongPhanBietSize = false
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm", Func = "Thêm Fillet / Thành Phẩm")]
        public async Task<IActionResult> DoInsert(string ma, string maCa, string ten, decimal thoiGianTren1KgSeconds, decimal dinhMucHaoHut, string codeId, bool isCaMuoi, bool isSoChe, bool isNotSetByTime, bool khongPhanBietSize)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/Insert";
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
                var model = new MaThanhPhamFillet
                {
                    Ma = ma,
                    MaCa = maCa,
                    Ten = ten,
                    SuDung = true,
                    Min = 0,
                    Max = 99999,
                    IsSoChe = isSoChe,
                    IsCaMuoi = isCaMuoi,
                    DinhMuc = 0,
                    BravoId = "",
                    TrongLuong = 0,
                    TrangThaiThanhPham = "NORMAL",
                    TrongLuongHienTai = 0,
                    ThoiGianTren1kgSeconds = thoiGianTren1KgSeconds,
                    DinhMucHaoHut = dinhMucHaoHut,
                    CodeId = codeId, //mã xưởng
                    IsNotSetByTime = isNotSetByTime,
                    ColorRGB = "",
                    KhongPhanBietSize = khongPhanBietSize,
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
        public async Task<IActionResult> GetsByMa(string ma)
        {
            if (string.IsNullOrEmpty(ma))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaThanhPhamFillet>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Ma = item?.Ma,
                        MaCa = item?.MaCa,
                        Ten = item?.Ten,
                        BravoId = item?.BravoId,
                        ThoiGianTren1kgSeconds = item?.ThoiGianTren1kgSeconds,
                        DinhMucHaoHut = item?.DinhMucHaoHut,
                        CodeId = item?.CodeId,
                        IsCaMuoi = item?.IsCaMuoi,
                        IsSoChe = item?.IsSoChe,
                        IsNotSetByTime = item?.IsNotSetByTime,
                        KhongPhanBietSize = item?.KhongPhanBietSize,
                        SuDung = true,
                        Min = 0,
                        Max = 99999,
                        DinhMuc = 0,
                        TrongLuong = 0,
                        TrangThaiThanhPham = "NORMAL",
                        TrongLuongHienTai = 0,
                        ColorRGB = "",
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm", Func = "Sửa Fillet / Thành Phẩm")]
        public async Task<IActionResult> DoUpDate(string ma, string maCa, string ten, decimal thoiGianTren1KgSeconds, decimal dinhMucHaoHut, string codeId, bool isCaMuoi, bool isSoChe, bool isNotSetByTime, bool khongPhanBietSize)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/Update/{ma}";
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
                var model = new MaThanhPhamFillet
                {
                    Ma = ma,
                    MaCa = maCa,
                    Ten = ten,
                    SuDung = true,
                    Min = 0,
                    Max = 99999,
                    IsSoChe = isSoChe,
                    IsCaMuoi = isCaMuoi,
                    DinhMuc = 0,
                    BravoId = "",
                    TrongLuong = 0,
                    TrangThaiThanhPham = "NORMAL",
                    TrongLuongHienTai = 0,
                    ThoiGianTren1kgSeconds = thoiGianTren1KgSeconds,
                    DinhMucHaoHut = dinhMucHaoHut,
                    CodeId = codeId, //mã xưởng
                    IsNotSetByTime = isNotSetByTime,
                    ColorRGB = "",
                    KhongPhanBietSize = khongPhanBietSize,
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Thành Phẩm", Func = "Xoá Fillet / Thành Phẩm")]
        public async Task<IActionResult> DoDelete(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/Delete/{ma}";
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
