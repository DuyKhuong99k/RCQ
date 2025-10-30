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
using GroupDocs.Viewer.Results;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;

namespace PMS.Controllers.DanhMuc.DinhHinh
{
    [Authorize]
    public class ThanhPhamDinhHinhController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ThanhPhamDinhHinhController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm", Func = "Xem Định Hình / Thành Phẩm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ThanhPhamDinhHinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Thành Phẩm Định Hình";

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            var apiSPTLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetAlls";
            using var helperSPTL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sptls = helperSPTL.GetAsync<IEnumerable<DG_SanPhamTinhLuong>>(HttpContext, apiSPTLUrl);
            ViewBag.listSPTLs = sptls.Result.ToList();
            return View("~/Views/DanhMuc/DinhHinh/ThanhPhamDinhHinhView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<MaThanhPhamDinhHinh>> GetAlls()
        {
            IEnumerable<MaThanhPhamDinhHinh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
         public async Task<IEnumerable<MaThanhPhamDinhHinh>> GetAllsByCodeId(string codeId)
        {
            IEnumerable<MaThanhPhamDinhHinh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiUrl);
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
                        DinhMuc = 1,
                        MaCa = "A",
                        TyLeDinhMucDau = 0.5M,
                        TyLeDinhMucRot = 0.5M,
                        TrongLuongTare = 0M,
                        IsDauVaoBatBuoc = false,
                        DinhMucKhongDauVao = 1,
                        IsDisplay = true,
                        //CodeId = XiNghiepViewModel.Ins.XiNghiepSelectedItem?.CodeId,
                        DinhMucCaTra = 1,
                        BaoCaoDauRot = false,
                        IsSuDungThoiGianGiuaLoaiThanhPham = false,
                        MinOut = 0,
                        MaxOut = 999
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
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm", Func = "Thêm Định Hình / Thành Phẩm")]
        public async Task<IActionResult> DoInsert(string ma, string ten, double dinhMuc, double min, double max, double minOut, double maxOut, decimal tyLeDinhMucDau, decimal tyLeDinhMucRot, decimal trongLuongTare, string bravoId, decimal dinhMucKhongCanDauVao, decimal dinhMucCaTra, string codeId, bool isSuDungThoiGianGiuaLoaiThanhPham, bool isDauVaoBatBuoc, bool isBaoCaoDauRot, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/Insert";
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
                var model = new MaThanhPhamDinhHinh
                {
                    Ma = ma,
                    MaCa = "A",
                    Ten = ten,
                    SuDung = suDung,
                    DinhMuc = dinhMuc,
                    Min = min,
                    Max = max,
                    BravoId = bravoId,
                    TyLeDinhMucDau = tyLeDinhMucDau,
                    TyLeDinhMucRot = tyLeDinhMucRot,
                    TrongLuongTare = trongLuongTare,
                    IsDauVaoBatBuoc = isDauVaoBatBuoc,
                    DinhMucKhongDauVao = dinhMucKhongCanDauVao,
                    IsDisplay = true,
                    CodeId = codeId,
                    DinhMucCaTra = dinhMucCaTra,
                    BaoCaoDauRot = isBaoCaoDauRot,
                    IsSuDungThoiGianGiuaLoaiThanhPham = isSuDungThoiGianGiuaLoaiThanhPham,
                    MinOut = minOut,
                    MaxOut = maxOut
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
        public async Task<IActionResult> GetsByMa(string ma, string maCa)
        {
            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(maCa))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetsByMa/{ma}/{maCa}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaThanhPhamDinhHinh>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Ma = item?.Ma,
                        MaCa = item?.MaCa,
                        Ten = item?.Ten,
                        SuDung = item?.SuDung,
                        DinhMuc = item?.DinhMuc,
                        Min = item?.Min,
                        Max = item?.Max,
                        BravoId = item?.BravoId,
                        TyLeDinhMucDau = item?.TyLeDinhMucDau,
                        TyLeDinhMucRot = item?.TyLeDinhMucRot,
                        TrongLuongTare = item?.TrongLuongTare,
                        IsDauVaoBatBuoc = item?.IsDauVaoBatBuoc,
                        DinhMucKhongDauVao = item?.DinhMucKhongDauVao,
                        IsDisplay = item?.IsDisplay,
                        CodeId = item?.CodeId,
                        DinhMucCaTra = item?.DinhMucCaTra,
                        BaoCaoDauRot = item?.BaoCaoDauRot,
                        IsSuDungThoiGianGiuaLoaiThanhPham = item?.IsSuDungThoiGianGiuaLoaiThanhPham,
                        MinOut = item?.MinOut,
                        MaxOut = item?.MaxOut
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm", Func = "Sửa Định Hình / Thành Phẩm")]
        public async Task<IActionResult> DoUpDate(string ma,string ten, double dinhMuc, double min, double max, double minOut, double maxOut, decimal tyLeDinhMucDau, decimal tyLeDinhMucRot, decimal trongLuongTare, string bravoId, decimal dinhMucKhongCanDauVao, decimal dinhMucCaTra, string codeId, bool isSuDungThoiGianGiuaLoaiThanhPham, bool isDauVaoBatBuoc, bool isBaoCaoDauRot, bool suDung,string maCa)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/Update/{ma}/{maCa}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(ma)||string.IsNullOrEmpty(maCa))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaThanhPhamDinhHinh
                {
                    Ma = ma,
                    MaCa = maCa,
                    Ten = ten,
                    SuDung = suDung,
                    DinhMuc = dinhMuc,
                    Min = min,
                    Max = max,
                    BravoId = bravoId,
                    TyLeDinhMucDau = tyLeDinhMucDau,
                    TyLeDinhMucRot = tyLeDinhMucRot,
                    TrongLuongTare = trongLuongTare,
                    IsDauVaoBatBuoc = isDauVaoBatBuoc,
                    DinhMucKhongDauVao = dinhMucKhongCanDauVao,
                    IsDisplay = true,
                    CodeId = codeId,
                    DinhMucCaTra = dinhMucCaTra,
                    BaoCaoDauRot = isBaoCaoDauRot,
                    IsSuDungThoiGianGiuaLoaiThanhPham = isSuDungThoiGianGiuaLoaiThanhPham,
                    MinOut = minOut,
                    MaxOut = maxOut
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
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Thành Phẩm", Func = "Xoá Định Hình / Thành Phẩm")]
        public async Task<IActionResult> DoDelete(string ma, string maCa)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/Delete/{ma}/{maCa}";
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
