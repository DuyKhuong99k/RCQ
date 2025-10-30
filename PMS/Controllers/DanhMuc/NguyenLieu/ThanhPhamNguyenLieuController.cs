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
    public class ThanhPhamNguyenLieuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ThanhPhamNguyenLieuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Thành Phẩm", Func = "Xem Nguyên Liệu / Thành Phẩm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "LoaiCaNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            return View("~/Views/DanhMuc/NguyenLieu/ThanhPhamNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<MaThanhPhamNguyenLieu>> GetAlls()
        {
            IEnumerable<MaThanhPhamNguyenLieu> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamNguyenLieux/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamNguyenLieu>>(HttpContext, apiUrl);
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
                        Ma = id,
                        SuDung = true,
                        Min = 0,
                        Max = 999,
                        MaCa = "A",
                        IsCaCanTin = false,
                        IsCaNgopXeMuoi = false,
                        IsDatNho = false,
                        IsManh = false,
                        IsMuoiGhePhuPham = false,
                        IsNgopAoMuoi = false,
                        IsNgopAoPhuPham = false,
                        IsNgopGhe = false,
                        IsNgopGheMuoi = false,
                        IsNgopGhePhuPham = false,
                        IsNgopXePhuPham = false,
                        IsPhuPhamCaTap = false,
                        IsSNL = false,
                        TyLeNuoc = 0,
                        IsCaNgopGheTuoiBanNgoai = false,
                        IsCaNgopGheAoBanNgoai = false,
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Thành Phẩm", Func = "Thêm Nguyên Liệu / Thành Phẩm")]
        public async Task<IActionResult> DoInsert(string ma, string ten, decimal tyLeNuoc, bool suDung, bool isSNL, bool isNgopGhe, bool isNgopGheMuoi, bool isMuoiGhePhuPham, bool isNgopAoMuoi, bool isNgopAoPhuPham, bool isDatNho, bool isPhuPhamCaTap, bool isCaCanTin, bool isCaNgopXeMuoi, bool isNgopGhePhuPham, bool isNgopXePhuPham, bool isNgopGheTuoiBanNgoai, bool isCaNgopGheAoBanNgoai, bool isManh)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamNguyenLieux/Insert";
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
                var model = new MaThanhPhamNguyenLieu
                {
                    MaCa = "A",
                    Ma = ma,
                    Ten = ten,
                    SuDung = suDung,
                    Min = 0,
                    Max = 999,
                    IsSNL = isSNL,
                    IsNgopGhe = isNgopGhe,
                    IsNgopGheMuoi = isNgopGheMuoi,
                    IsMuoiGhePhuPham = isMuoiGhePhuPham,
                    IsNgopAoMuoi = isNgopAoMuoi,
                    IsNgopAoPhuPham = isNgopAoPhuPham,
                    IsDatNho = isDatNho,
                    IsPhuPhamCaTap = isPhuPhamCaTap,
                    IsCaCanTin = isCaCanTin,
                    IsCaNgopXeMuoi = isCaNgopXeMuoi,
                    IsNgopGhePhuPham = isNgopGhePhuPham,
                    IsNgopXePhuPham = isNgopXePhuPham,
                    IsManh = isManh,
                    TyLeNuoc = tyLeNuoc,
                    IsCaNgopGheTuoiBanNgoai = isNgopGheTuoiBanNgoai,
                    IsCaNgopGheAoBanNgoai = isCaNgopGheAoBanNgoai

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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamNguyenLieux/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaThanhPhamNguyenLieu>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaCa = item?.MaCa,
                        Ma = item?.Ma,
                        Ten = item?.Ten,
                        SuDung = item?.SuDung,
                        Min = item?.Min,
                        Max = item?.Max,
                        IsSNL = item?.IsSNL,
                        IsNgopGhe = item?.IsNgopGhe,
                        IsNgopGheMuoi = item?.IsNgopGheMuoi,
                        IsMuoiGhePhuPham = item?.IsMuoiGhePhuPham,
                        IsNgopAoMuoi = item?.IsNgopAoMuoi,
                        IsNgopAoPhuPham = item?.IsNgopAoPhuPham,
                        IsDatNho = item?.IsDatNho,
                        IsPhuPhamCaTap = item?.IsPhuPhamCaTap,
                        IsCaCanTin = item?.IsCaCanTin,
                        IsCaNgopXeMuoi = item?.IsCaNgopXeMuoi,
                        IsNgopGhePhuPham = item?.IsNgopGhePhuPham,
                        IsNgopXePhuPham = item?.IsNgopXePhuPham,
                        IsManh = item?.IsManh,
                        TyLeNuoc = item?.TyLeNuoc,
                        IsCaNgopGheTuoiBanNgoai = item?.IsCaNgopGheTuoiBanNgoai,
                        IsCaNgopGheAoBanNgoai = item?.IsCaNgopGheAoBanNgoai
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Thành Phẩm", Func = "Sửa Nguyên Liệu / Thành Phẩm")]
        public async Task<IActionResult> DoUpDate(string ma, string ten, decimal tyLeNuoc, bool suDung, bool isSNL, bool isNgopGhe, bool isNgopGheMuoi, bool isMuoiGhePhuPham, bool isNgopAoMuoi, bool isNgopAoPhuPham, bool isDatNho, bool isPhuPhamCaTap, bool isCaCanTin, bool isCaNgopXeMuoi, bool isNgopGhePhuPham, bool isNgopXePhuPham, bool isNgopGheTuoiBanNgoai, bool isCaNgopGheAoBanNgoai, bool isManh)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamNguyenLieux/Update/{ma}";
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
                var model = new MaThanhPhamNguyenLieu
                {
                    MaCa = "A",
                    Ma = ma,
                    Ten = ten,
                    SuDung = suDung,
                    Min = 0,
                    Max = 999,
                    IsSNL = isSNL,
                    IsNgopGhe = isNgopGhe,
                    IsNgopGheMuoi = isNgopGheMuoi,
                    IsMuoiGhePhuPham = isMuoiGhePhuPham,
                    IsNgopAoMuoi = isNgopAoMuoi,
                    IsNgopAoPhuPham = isNgopAoPhuPham,
                    IsDatNho = isDatNho,
                    IsPhuPhamCaTap = isPhuPhamCaTap,
                    IsCaCanTin = isCaCanTin,
                    IsCaNgopXeMuoi = isCaNgopXeMuoi,
                    IsNgopGhePhuPham = isNgopGhePhuPham,
                    IsNgopXePhuPham = isNgopXePhuPham,
                    IsManh = isManh,
                    TyLeNuoc = tyLeNuoc,
                    IsCaNgopGheTuoiBanNgoai = isNgopGheTuoiBanNgoai,
                    IsCaNgopGheAoBanNgoai = isCaNgopGheAoBanNgoai
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
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Thành Phẩm", Func = "Xoá Nguyên Liệu / Thành Phẩm")]
        public async Task<IActionResult> DoDelete(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamNguyenLieux/Delete/{ma}";
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
