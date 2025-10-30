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
using Syncfusion.EJ2.Gantt;

namespace PMS.Controllers.DanhMuc.XepKhuon.QuanLyPhuGia
{
    [Authorize]
    public class CongThucController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CongThucController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Công Thức", Func = "Xem Quản Lý Phụ Gia / Công Thức")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CongThucView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Công Thức";
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_SanPham/GetAlls";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiUrl);
            ViewBag.listSanPhams = items.Result.ToList();

            var apiUrlDonViTinh = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_DonViTinh/GetAlls";
            using var helperDonViTinh = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var itemDonViTinhs = helperDonViTinh.GetAsync<IEnumerable<PD_DonViTinh>>(HttpContext, apiUrlDonViTinh);
            ViewBag.listDonViTinhs = itemDonViTinhs.Result.ToList();
            return View("~/Views/DanhMuc/XepKhuon/QuanLyPhuGia/CongThucView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThuc/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllsFullFieldCongThucChiTiet(string maCongThuc, DateTime ngay)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThucChiTiet/GetAllsFullField/{maCongThuc}/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<PD_CongThuc>> GetAlls()
        {
            IEnumerable<PD_CongThuc> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThuc/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PD_CongThuc>>(HttpContext, apiUrl);
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
                        TrongLuongNguyenLieu = 0,
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
        [CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Công Thức", Func = "Thêm Quản Lý Phụ Gia / Công Thức")]
        public async Task<IActionResult> DoInsert(string ma, string ten, decimal trongLuongNguyenLieu, string maDonViTinh, string ghiChu, bool suDung, DateTime creatDateTime, string creatBy)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThuc/Insert";
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
                var model = new PD_CongThuc
                {
                    Ma = ma,
                    Ten = ten,
                    TrongLuongNguyenLieu = trongLuongNguyenLieu,
                    MaDonViTinh = maDonViTinh,
                    GhiChu = ghiChu,
                    SuDung = suDung,
                    CreateDateTime = creatDateTime,
                    CreateBy = creatBy,
                    ModifiedDateTime = creatDateTime,
                    ModifiedBy = creatBy
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThuc/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PD_CongThuc>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        SuDung = item?.SuDung,
                        Ma = item?.Ma,
                        Ten = item?.Ten,
                        TrongLuongNguyenLieu = item?.TrongLuongNguyenLieu,
                        MaDonViTinh = item?.MaDonViTinh,
                        GhiChu = item?.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Công Thức", Func = "Sửa Quản Lý Phụ Gia / Công Thức")]
        public async Task<IActionResult> DoUpDate(string ma, string ten, decimal trongLuongNguyenLieu, string maDonViTinh, string ghiChu, bool suDung, DateTime modifiedDateTime, string modifiedBy)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThuc/Update/{ma}";
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
                var model = new PD_CongThuc
                {
                    Ma = ma,
                    Ten = ten,
                    TrongLuongNguyenLieu = trongLuongNguyenLieu,
                    MaDonViTinh = maDonViTinh,
                    GhiChu = ghiChu,
                    SuDung = suDung,
                    ModifiedDateTime = modifiedDateTime,
                    ModifiedBy = modifiedBy
                };
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
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
        [CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Công Thức", Func = "Xóa Quản Lý Phụ Gia / Công Thức")]
        public async Task<IActionResult> DoDelete(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThuc/Delete/{ma}";
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
        //Công thức chi tiết begin
        public async Task<IEnumerable<PD_CongThucChiTiet>> GetAllCongThucChiTiets()
        {
            IEnumerable<PD_CongThucChiTiet> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThucChiTiet/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PD_CongThucChiTiet>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<PD_CongThucChiTiet>> GetAllCongThucChiTietWithMaCongThucAndNgays(string maCongThuc)
        {
            IEnumerable<PD_CongThucChiTiet> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThucChiTiet/GetAllWithMaCongThucAndNgays/{maCongThuc}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PD_CongThucChiTiet>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNewCongThucChiTiet(string maCongThuc)
        {
            try
            {
                var dataSource = await GetAllCongThucChiTietWithMaCongThucAndNgays(maCongThuc);
                if (dataSource != null)
                {

                    var maxId = dataSource.Select(x => x.STT).Max();
                    var id = maxId + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = id,
                        SanLuong = 0,
                        TyLe = 0,
                        BienDo = 0
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
        [CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Công Thức", Func = "Thêm Quản Lý Phụ Gia / Công Thức")]
        public async Task<IActionResult> DoInsertCongThucChiTiet(int stt, DateTime ngay, string maCongThuc, string maSanPham, decimal sanLuong, decimal tyLe, decimal bienDo)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThucChiTiet/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maCongThuc) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(sanLuong.ToString()) || string.IsNullOrEmpty(tyLe.ToString()) || string.IsNullOrEmpty(bienDo.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PD_CongThucChiTiet
                {
                    STT = stt,
                    Ngay = ngay,
                    MaCongThuc = maCongThuc,
                    MaSanPham = maSanPham,
                    SanLuong = sanLuong,
                    TyLe = tyLe,
                    BienDo = bienDo
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
        public async Task<IActionResult> GetsByMaCongThucChiTiet(int stt, DateTime ngay, string maCongThuc)
        {
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maCongThuc))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThucChiTiet/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maCongThuc}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PD_CongThucChiTiet>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item?.STT,
                        Ngay = item?.Ngay.ToString("yyyy-MM-dd"),
                        MaCongThuc = item?.MaCongThuc,
                        MaSanPham = item?.MaSanPham,
                        SanLuong = item?.SanLuong,
                        TyLe = item?.TyLe,
                        BienDo = item?.BienDo
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Công Thức", Func = "Sửa Quản Lý Phụ Gia / Công Thức")]
        public async Task<IActionResult> DoUpDateCongThucChiTiet(int stt, DateTime ngay, string maCongThuc, string maSanPham, decimal sanLuong, decimal tyLe, decimal bienDo)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThucChiTiet/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maCongThuc}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maCongThuc))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PD_CongThucChiTiet
                {
                    STT = stt,
                    Ngay = ngay,
                    MaCongThuc = maCongThuc,
                    MaSanPham = maSanPham,
                    SanLuong = sanLuong,
                    TyLe = tyLe,
                    BienDo = bienDo
                };
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
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
        [CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Công Thức", Func = "Xóa Quản Lý Phụ Gia / Công Thức")]
        public async Task<IActionResult> DoDeleteCongThucChiTiet(int stt,DateTime ngay, string maCongThuc)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_CongThucChiTiet/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maCongThuc}";
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
