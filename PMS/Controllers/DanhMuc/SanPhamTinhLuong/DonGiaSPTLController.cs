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
using Syncfusion.EJ2.Navigations;


namespace PMS.Controllers.DanhMuc.SanPhamTinhLuong
{
    [Authorize]
    public class DonGiaSPTLController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DonGiaSPTLController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Đơn Giá", Func = "Xem Sản Phẩm Tính Lương / Đơn Giá")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "DonGiaView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiSPTLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetAlls";
            using var helperSPTL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sptls = helperSPTL.GetAsync<IEnumerable<DG_SanPhamTinhLuong>>(HttpContext, apiSPTLUrl);
            ViewBag.listSPTLs = sptls.Result.ToList();

            var apiSDHUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeDinhHinhs/GetAlls";
            using var helperSDH = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sdhs = helperSDH.GetAsync<IEnumerable<MaSizeDinhHinh>>(HttpContext, apiSDHUrl);
            ViewBag.listSDHs = sdhs.Result.ToList();

            var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
            using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl);
            ViewBag.listTPFLs = tpfls.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeFillets/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeFillet>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiLDGUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_LoaiDonGia/GetAlls";
            using var helperLDG = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var ldgs = helperLDG.GetAsync<IEnumerable<DG_LoaiDonGia>>(HttpContext, apiLDGUrl);
            ViewBag.listLDGs = ldgs.Result.ToList();

            var apiXHUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaXepHangs/GetAlls";
            using var helperXH = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xhs = helperXH.GetAsync<IEnumerable<MaXepHang>>(HttpContext, apiXHUrl);
            ViewBag.listXHs = xhs.Result.ToList();

            ViewBag.TitlePage = "Đơn Giá";
            return View("~/Views/DanhMuc/SanPhamTinhLuong/DonGiaView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullFieldByYear()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_DonGia/GetAllsFullFieldByYear";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<DG_DonGia>> GetAlls()
        {
            IEnumerable<DG_DonGia> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_DonGia/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<DG_DonGia>>(HttpContext, apiUrl);
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
                    var maxId = dataSource.Where(x => int.TryParse(x.Id, out int rl)).Select(x => x.Id).DefaultIfEmpty("0")
                     .Max();
                    var id = (int.Parse(maxId) + 1).ToString("0000000");
                    return Json(new
                    {
                        isSuccess = true,
                        Id = id,
                        //CreateBy = PMSMSharedv1.ViewModel.AppViewModel.Ins.UserName,
                        CreateDateTime = DateTime.Now,
                        DanhGia = true,
                        DinhMucDown = 0,
                        DinhMucUp = 0,
                        DonGia = 0,
                        Gio = DateTime.Now,
                        HeSo = 1,
                        //ModifiedBy = PMSMSharedv1.ViewModel.AppViewModel.Ins.UserName,
                        ModifiedDateTime = DateTime.Now,
                        Ngay = DateTime.Now,
                        HeSoRot = 1,
                        IsUsedHeSoRot = false,
                        Range = 0,
                        DonGiaGiaCong = 0,
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
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Đơn Giá", Func = "Thêm Sản Phẩm Tính Lương / Đơn Giá")]
        public async Task<IActionResult> DoInsert(string id, DateTime ngay, TimeSpan gio, string maSanPham, string maSizeDinhHinh, string maThanhPhamFillet, string maSizeFillet, string maloaiDonGia, bool danhGia, string maXepHang, decimal dinhMucDown, decimal dinhMucUp, decimal heSo, decimal heSoRot, bool isUsedHeSoRot, decimal donGia, decimal donGiaGiaCong, string createBy)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_DonGia/Insert";
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new DG_DonGia
                {
                    Id = id,
                    Ngay = ngay,
                    Gio = gio,
                    MaSanPham = maSanPham,
                    MaLoaiDonGia = maloaiDonGia,
                    DanhGia = danhGia,
                    DinhMucDown = dinhMucDown,
                    DinhMucUp = dinhMucUp,
                    HeSo = heSo,
                    DonGia = donGia,
                    CreateBy = createBy,
                    CreateDateTime = DateTime.Now,
                    ModifiedBy = "",
                    ModifiedDateTime = DateTime.MinValue,
                    GhiChu = "",
                    HeSoRot = heSoRot,
                    IsUsedHeSoRot = isUsedHeSoRot,
                    Range = 0,
                    MaSizeDinhHinh = maSizeDinhHinh,
                    DonGiaGiaCong = donGiaGiaCong,
                    MaXepHang = maXepHang,
                    MaThanhPham = maThanhPhamFillet,
                    LoaiCan = "",
                    MaSizeFillet = maSizeFillet
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
        public async Task<IActionResult> GetsByMa(string id)
        {
            if (string.IsNullOrEmpty(id))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_DonGia/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<DG_DonGia>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaSanPham = item.MaSanPham,
                        MaLoaiDonGia = item.MaLoaiDonGia,
                        DanhGia = item.DanhGia,
                        DinhMucDown = item.DinhMucDown,
                        DinhMucUp = item.DinhMucUp,
                        HeSo = item.HeSo,
                        DonGia = item.DonGia,
                        CreateBy = item.CreateBy,
                        CreateDateTime = item.CreateDateTime,
                        ModifiedBy = item.ModifiedBy,
                        ModifiedDateTime = item.ModifiedDateTime,
                        GhiChu = item.GhiChu,
                        HeSoRot = item.HeSoRot,
                        IsUsedHeSoRot = item.IsUsedHeSoRot,
                        Range = item.Range,
                        MaSizeDinhHinh = item.MaSizeDinhHinh,
                        DonGiaGiaCong = item.DonGiaGiaCong,
                        MaXepHang = item.MaXepHang,
                        MaThanhPham = item.MaThanhPham,
                        LoaiCan = item.LoaiCan,
                        MaSizeFillet = item.MaSizeFillet
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Đơn Giá", Func = "Sửa Sản Phẩm Tính Lương / Đơn Giá")]
        public async Task<IActionResult> DoUpDate(string id, DateTime ngay, TimeSpan gio, string maSanPham, string maSizeDinhHinh, string maThanhPhamFillet, string maSizeFillet, string maloaiDonGia, bool danhGia, string maXepHang, decimal dinhMucDown, decimal dinhMucUp, decimal heSo, decimal heSoRot, bool isUsedHeSoRot, decimal donGia, decimal donGiaGiaCong, string createBy,DateTime createDateTime, string modifiedBy)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_DonGia/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new DG_DonGia
                {
                    Id = id,
                    Ngay = ngay,
                    Gio = gio,
                    MaSanPham = maSanPham,
                    MaLoaiDonGia = maloaiDonGia,
                    DanhGia = danhGia,
                    DinhMucDown = dinhMucDown,
                    DinhMucUp = dinhMucUp,
                    HeSo = heSo,
                    DonGia = donGia,
                    CreateBy = createBy,
                    CreateDateTime = createDateTime,
                    ModifiedBy = modifiedBy,
                    ModifiedDateTime = DateTime.Now,
                    GhiChu = "",
                    HeSoRot = heSoRot,
                    IsUsedHeSoRot = isUsedHeSoRot,
                    Range = 0,
                    MaSizeDinhHinh = maSizeDinhHinh,
                    DonGiaGiaCong = donGiaGiaCong,
                    MaXepHang = maXepHang,
                    MaThanhPham = maThanhPhamFillet,
                    LoaiCan = "",
                    MaSizeFillet = maSizeFillet
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
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Đơn Giá", Func = "Xoá Sản Phẩm Tính Lương / Đơn Giá")]
        public async Task<IActionResult> DoDelete(string id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_DonGia/Delete/{id}";
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
