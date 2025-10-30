using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Attrs;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Charts;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;
using PMS.Models;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Security.Policy;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Models.Repos.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using Models.Repos;
using System;
using DocumentFormat.OpenXml.Spreadsheet;


namespace PMS.Controllers.HQ
{
    [Authorize]
    public class BaoCaoHQController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoHQController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region CHi Tiết
        [CustomAuthorize(Fu = "Báo Cáo / Chi Tiết HQ", Func = "Xem Báo Cáo / Chi Tiết HQ")]
        public IActionResult ChiTietHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết";
            return View("~/Views/BaoCaoHQ/ChiTietHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTiets(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetChiTiets/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp NV
        [CustomAuthorize(Fu = "Báo Cáo / Tổng Hợp Nhân Viên HQ", Func = "Xem Báo Cáo / Tổng Hợp Nhân Viên HQ")]
        public IActionResult TongHopNhanVienHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên";
            return View("~/Views/BaoCaoHQ/TongHopNhanVienHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopNhanViens(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopNhanViens/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp tp
        [CustomAuthorize(Fu = "Báo Cáo / Tổng Hợp Thành Phẩm HQ", Func = "Xem Báo Cáo / Tổng Hợp Thành Phẩm HQ")]
        public IActionResult TongHopThanhPhamHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopThanhPhamHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Thành Phẩm";
            return View("~/Views/BaoCaoHQ/TongHopThanhPhamHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopThanhPhams(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhams/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Nhân Viên Theo Ca
        [CustomAuthorize(Fu = "Báo Cáo / Tổng Hợp Nhân Viên Theo Ca HQ", Func = "Xem Báo Cáo / Tổng Hợp Nhân Viên Theo Ca HQ")]
        public IActionResult TongHopNhanVienTheoCaHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopNhanVienTheoCaHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên Theo Ca";
            return View("~/Views/BaoCaoHQ/TongHopNhanVienTheoCaHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVienTheoCas(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopNhanVienTheoCas/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Tính Lương
        [CustomAuthorize(Fu = "Báo Cáo / Tổng Hợp Tính Lương HQ", Func = "Xem Báo Cáo / Tổng Hợp Tính Lương HQ")]
        public IActionResult TongHopTinhLuongHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopTinhLuongHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Tính Lương";
            return View("~/Views/BaoCaoHQ/TongHopTinhLuongHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopTinhLuongs(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopTinhLuongs/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion

        #region Báo cáo thống kê sản xuất
        [CustomAuthorize(Fu = "Báo Cáo / Thống Kê Sản Xuất HQ", Func = "Xem Báo Cáo / Thống Kê Sản Xuất HQ")]
        public IActionResult ThongKeSanXuatHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ThongKeSanXuatHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Thống Kê Sản Xuất";
            return View("~/Views/BaoCaoHQ/ThongKeSanXuatHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetThongKeSanXuats(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThongKeSanXuat/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;

        }

        public async Task<IEnumerable<HQ_PhieuThongKeSanXuat>> GetExistingData(DateTime ngay)
        {
            IEnumerable<HQ_PhieuThongKeSanXuat> dataSource = ViewBag.hqphieutksx;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuThongKeSanXuats/GetAlls/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.hqphieutksx = await helper.GetAsync<IEnumerable<HQ_PhieuThongKeSanXuat>>(HttpContext, apiUrl);
                dataSource = ViewBag.hqphieutksx;
            }
            return dataSource;
        }
        
        public class Item
        {
            public DateTime Ngay { get; set; }
            public string CaLamViec { get; set; }
            public string MA_DAI_DIEN { get; set; }
            public string TenNhom { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string MaSanPham { get; set; }
            public string SanPhamName { get; set; }
            public decimal TongTrongLuong { get; set; }
            public string GioBatDau { get; set; }
            public string GioKetThuc { get; set; }
            public double TongGio { get; set; }
        }
        public async Task<IActionResult> DoInsertPhieuThongKeSanXuat(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuThongKeSanXuats/Insert";
            try
            {
                var rs = await GetThongKeSanXuats(fromDate, toDate, xuongId);


                var serializedData = JsonConvert.SerializeObject(rs);


                var items = JsonConvert.DeserializeObject<List<Item>>(serializedData);
                var models = new List<HQ_PhieuThongKeSanXuat>();

                var existingData = await GetExistingData(toDate);
                int currentMaxSTT = 0;


                if (existingData == null || !existingData.Any())
                {
                    // Nếu không có dữ liệu, gán giá trị mặc định cho currentMaxSTT
                    currentMaxSTT = 1;
                }
                else
                {
                    // Nếu có dữ liệu, lấy giá trị STT cao nhất và cộng thêm 1
                    currentMaxSTT = existingData
                                    .Where(x => x.SoChungTu.Length >= 2) // Kiểm tra độ dài SoChungTu
                                    .Max(x => int.Parse(x.SoChungTu.Substring(x.SoChungTu.Length - 2))) + 1;
                }

                foreach (var item in items)
                {
                    var ngay = item.Ngay; // Date of the item
                    var soChungTu = $"PMSSX{ngay:MM/yy}-{ngay:dd}{currentMaxSTT:00}";

                    var model = new HQ_PhieuThongKeSanXuat
                    {
                        //Id = 1,
                        Ngay = ngay,
                        SoChungTu = soChungTu,
                        Ca = item.CaLamViec,
                        MaTo = item.MA_DAI_DIEN,
                        TenTo = item.TenNhom,
                        MaNhanVien = item.MaNhanVien,
                        TenNhanVien = item.TenNhanVien,
                        MaCongViec = item.MaSanPham,
                        TenCongViec = item.SanPhamName,
                        SanLuong = item.TongTrongLuong,
                        GioBatDau = TimeSpan.TryParse(item.GioBatDau, out var gioBatDau)
                            ? ngay.Add(gioBatDau)
                            : ngay,
                        GioKetThuc = TimeSpan.TryParse(item.GioKetThuc, out var gioKetThuc)
                            ? ngay.Add(gioKetThuc)
                            : ngay,
                        NgayGioTao = DateTime.Now
                    };

                    models.Add(model);
                }
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(models));
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
        public IActionResult GetSignalRConnectionInfo()
        {
            var url = AppViewModels.AppViewModel.Instance.ApiHostUrl+ "/progressHub";
            var token = HttpContext.Session.GetString("JWTToken");

            // Mã hóa URL và Token
            var encryptedUrl = Security.Crypt.ED.EncryptString(url);
            var encryptedToken = Security.Crypt.ED.EncryptString(token);

            return Json(new { url = encryptedUrl, token = encryptedToken });
        }
        public async Task<IActionResult> KetChuyenThongKeSanXuatToBravo(DateTime dateTime)
        {
            try
            {
                if (dateTime == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không có ngày."
                    });
                }
                var model = new 
                {
                    Ngay = dateTime
                };
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuThongKeSanXuats/InsertBravo";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                //var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
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
                    Messages = "Đã xảy ra lỗi: " + ex.Message
                });
            }
        }
        #endregion

        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietHQView")
            {
                dataSource = await GetChiTiets(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopNhanVienHQView")
            {
                dataSource = await GetTongHopNhanViens(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopThanhPhamHQView")
            {
                dataSource = await GetTongHopThanhPhams(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopNhanVienTheoCaHQView")
            {
                dataSource = await GetTongHopNhanVienTheoCas(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopTinhLuongHQView")
            {
                dataSource = await GetTongHopTinhLuongs(fromDate, toDate, xuongId);
            }
            else if (reportType == "ThongKeSanXuatHQView")
            {
                dataSource = await GetThongKeSanXuats(fromDate, toDate, xuongId);
            }
            if (dataSource == null || !dataSource.Any())
            {
                success = false; // Đặt trạng thái thành không thành công nếu dataSource là null hoặc không có dữ liệu.
            }
            var result = new
            {
                Success = success, // Trạng thái thành công
                Messages = success ? "Lấy dữ liệu Thành Công." : "Vui lòng kiểm tra lại!.", // Thông báo tùy thuộc vào trạng thái
                Data = dataSource // Dữ liệu từ GetChiTiets, GetTongHopNhanViens hoặc GetTongHopThanhPhams
            };
            ViewBag.datasource = dataSource;
            return Json(result);
        }
    }
}
