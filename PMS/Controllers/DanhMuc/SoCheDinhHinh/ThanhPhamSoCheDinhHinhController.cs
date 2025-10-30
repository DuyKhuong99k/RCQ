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
using Microsoft.AspNetCore.Http.HttpResults;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;

namespace PMS.Controllers.DanhMuc.SoCheDinhHinh
{
    [Authorize]
    public class ThanhPhamSoCheDinhHinhController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ThanhPhamSoCheDinhHinhController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Sơ Chế Định Hình / Thành Phẩm", Func = "Xem Sơ Chế Định Hình / Thành Phẩm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ThanhPhamSoCheDinhHinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Thành Phẩm Sơ Chế Định Hình";
            return View("~/Views/DanhMuc/SoCheDinhHinh/ThanhPhamSoCheDinhHinhView.cshtml");
        }
        public async Task<IEnumerable<MaThanhPhamSoCheDinhHinh>> GetAlls()
        {
            IEnumerable<MaThanhPhamSoCheDinhHinh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamSoCheDinhHinhs/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamSoCheDinhHinh>>(HttpContext, apiUrl);
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
                        Nhan = true,
                        IsNhapTay = false,
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
        [CustomAuthorize(Fu = "Danh Mục / Sơ Chế Định Hình / Thành Phẩm", Func = "Thêm Sơ Chế Định Hình / Thành Phẩm")]
        public async Task<IActionResult> DoInsert(string ma, string ten, bool suDung, int x, int y, string bravoId, bool tinhKiem, bool tinhPhucVu, bool caMuoi, bool nguyenLieu, bool ban09, bool loaiGui, bool truocLangDa, bool sauLangDa, bool nhan, bool tinhGio, bool batCO, bool isNhapTay)
        {
            var idUser = HttpContext.Session.GetString("Id");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamSoCheDinhHinhs/Insert";
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
                var model = new MaThanhPhamSoCheDinhHinh
                {
                    Ma = ma,
                    Ten = ten,
                    SuDung = suDung,
                    X = x,
                    Y = y,
                    Min = 0,
                    Max = 0,
                    TinhKiem = tinhKiem,
                    TinhPhucVu = tinhPhucVu,
                    CaMuoi = caMuoi,
                    NguyenLieu = nguyenLieu,
                    Ban09 = ban09,
                    BravoId = bravoId,
                    LoaiGui = loaiGui,
                    TruocLangDa = truocLangDa,
                    SauLangDa = sauLangDa,
                    Nhan = nhan,
                    TinhGio = tinhGio,
                    BatCO = batCO,
                    IsNhapTay = isNhapTay,
                    Createdby = idUser,
                    CreatedDateTime = DateTime.Now,
                    Modifiedby = "",
                    ModifiedDateTime = null
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamSoCheDinhHinhs/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaThanhPhamSoCheDinhHinh>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Ten = item.Ten,
                        Ma = item.Ma,
                        SuDung = item.SuDung,
                        X = item.X,
                        Y = item.Y,
                        Min = item.Min,
                        Max = item.Max,
                        TinhKiem = item.TinhKiem,
                        TinhPhucVu = item.TinhPhucVu,
                        CaMuoi = item.CaMuoi,
                        NguyenLieu = item.NguyenLieu,
                        Ban09 = item.Ban09,
                        BravoId = item.BravoId,
                        LoaiGui = item.LoaiGui,
                        TruocLangDa = item.TruocLangDa,
                        SauLangDa = item.SauLangDa,
                        Nhan = item.Nhan,
                        TinhGio = item.TinhGio,
                        BatCO = item.BatCO,
                        IsNhapTay = item.IsNhapTay,
                        Createdby = item.Createdby,
                        CreatedDateTime = item.CreatedDateTime,
                        Modifiedby = item.Modifiedby,
                        ModifiedDateTime = item.ModifiedDateTime
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Sơ Chế Định Hình / Thành Phẩm", Func = "Sửa Sơ Chế Định Hình / Thành Phẩm")]
        public async Task<IActionResult> DoUpDate(string ma, string ten, bool suDung, int x, int y, bool tinhKiem, bool tinhPhucVu, bool caMuoi, bool nguyenLieu, bool ban09, string bravoId, bool loaiGui, bool truocLangDa, bool sauLangDa, bool nhan, bool tinhGio, bool batCO, bool isNhapTay, string createdBy, DateTime createdDateTime)
        {
            var idUser = HttpContext.Session.GetString("Id");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamSoCheDinhHinhs/Update/{ma}";
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
                var model = new MaThanhPhamSoCheDinhHinh
                {
                    Ma = ma,
                    Ten = ten,
                    SuDung = suDung,
                    X = x,
                    Y = y,
                    Min = 0,
                    Max = 0,
                    TinhKiem = tinhKiem,
                    TinhPhucVu = tinhPhucVu,
                    CaMuoi = caMuoi,
                    NguyenLieu = nguyenLieu,
                    Ban09 = ban09,
                    BravoId = bravoId,
                    LoaiGui = loaiGui,
                    TruocLangDa = truocLangDa,
                    SauLangDa = sauLangDa,
                    Nhan = nhan,
                    TinhGio = tinhGio,
                    BatCO = batCO,
                    IsNhapTay = isNhapTay,
                    Createdby = createdBy,
                    CreatedDateTime = createdDateTime,
                    Modifiedby = idUser,
                    ModifiedDateTime = DateTime.Now
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
        [CustomAuthorize(Fu = "Danh Mục / Sơ Chế Định Hình / Thành Phẩm", Func = "Xóa Sơ Chế Định Hình / Thành Phẩm")]
        public async Task<IActionResult> DoDelete(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamSoCheDinhHinhs/Delete/{ma}";
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
