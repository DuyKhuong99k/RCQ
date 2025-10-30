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
using Models.Repos.AppModel;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using Models.Repos;
using System;
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;
using AppViewModels;
using System.Transactions;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Syncfusion.EJ2.Layouts;
using Syncfusion.EJ2.Navigations;
using ViewModels.Repos.HQ;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using Models.Repos.A_Model;


namespace PMS.Controllers.TinhLuong.XepKhuon
{
    [Authorize]
    public class GianTiepController : Controller
    {
        private readonly IHubContext<PMS.Hubs.ProgressHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public GianTiepController(IHubContext<PMS.Hubs.ProgressHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult ChinhGianTiepView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChinhGianTiepView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
         
            ViewBag.titile = "Tính Lương Xếp Khuôn Gián Tiếp";
            return View("~/Views/TinhLuong/XepKhuon/ChinhGianTiepView.cshtml");
        }
        public async Task<IEnumerable<object>> LoadTongHopTinhLuongSanLuongXepKhuon(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuSanLuongRaCoiTinhLuongXepKhuons/LoadTongHopTinhLuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<ActionResult> Reload(DateTime dateTime, string xuongId, string typeReload)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;

            try
            {
                switch (typeReload)
                {
                    case "LoadTongHopTinhLuongSanLuongXepKhuon":
                        dataSource = await LoadTongHopTinhLuongSanLuongXepKhuon(dateTime, xuongId);
                        break;

                }

                if (dataSource == null || !dataSource.Any())
                {
                    success = false;
                }

                var result = new
                {
                    Success = success,
                    Messages = success ? "Lấy dữ liệu Thành Công." : "Vui lòng kiểm tra lại!.",
                    Data = dataSource
                };

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> KetChuyen(DateTime dateTime, string maXuong, string listSanLuongPhuFillet)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuSanLuongRaCoiTinhLuongXepKhuons/KetChuyenTinhLuongSanLuongXepKhuon/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listSanLuongPhuFillet));
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


        #region Công Việc Tính Lương Loại Cong Việc Thành Phẩm
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Xem Quản Lý Phụ Gia / Đơn Vị Tính")]
        public IActionResult CongViecLoaiThanhPhamView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CongViecLoaiThanhPhamView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiCongViecUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/GetAlls";
            using var helperCongViec = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var congviecs = helperCongViec.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiCongViecUrl);
            ViewBag.CongViecs = congviecs.Result.ToList();

            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamDHUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}";
            using var helperThanhPhamDH = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamDHs = helperThanhPhamDH.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiThanhPhamDHUrl);
            ViewBag.listThanhPhamDHs = thanhPhamDHs.Result.ToList();

            var apiThanhPhamPhuXepKhuonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamXepKhuons/GetAlls";
            using var helperThanhPhamPhuXepKhuon = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamPhuXepKhuoncs = helperThanhPhamPhuXepKhuon.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamPhuXepKhuonUrl);
            ViewBag.ThanhPhamPhuXepKhuons = thanhPhamPhuXepKhuoncs.Result.ToList();

            var apiThanhPhamChinhXepKhuonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamChinhXepKhuons/GetAlls";
            using var helperThanhPhamChinhXepKhuon = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamChinhXepKhuoncs = helperThanhPhamChinhXepKhuon.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamChinhXepKhuonUrl);
            ViewBag.ThanhPhamChinhXepKhuons = thanhPhamChinhXepKhuoncs.Result.ToList();

            var apiThanhPhamChieuXaXepKhuonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaChieuXaXepKhuons/GetAlls";
            using var helperThanhPhamChieuXaXepKhuon = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamChieuXaXepKhuoncs = helperThanhPhamChieuXaXepKhuon.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamChieuXaXepKhuonUrl);
            ViewBag.ThanhPhamChieuXaXepKhuons = thanhPhamChieuXaXepKhuoncs.Result.ToList();

            var apiThanhPhamBlockXepKhuonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamXepKhuonBlocks/GetAlls";
            using var helperThanhPhamBlockXepKhuon = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamBlockXepKhuoncs = helperThanhPhamBlockXepKhuon.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamBlockXepKhuonUrl);
            ViewBag.ThanhPhamBlockXepKhuons = thanhPhamBlockXepKhuoncs.Result.ToList();

            var apiThanhPhamKHCXepKhuonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamXepKhuonKHCs/GetAlls";
            using var helperThanhPhamKHCXepKhuon = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamKHCXepKhuoncs = helperThanhPhamKHCXepKhuon.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamKHCXepKhuonUrl);
            ViewBag.ThanhPhamKHCXepKhuons = thanhPhamKHCXepKhuoncs.Result.ToList();

            var apiThanhPhamTaiCheUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamTaiChes/GetAlls";
            using var helperThanhPhamTaiChe = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamTaiChecs = helperThanhPhamTaiChe.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamTaiCheUrl);
            ViewBag.ThanhPhamTaiChes = thanhPhamTaiChecs.Result.ToList();

            var apiThanhPhamSoCheUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamSoCheDinhHinhs/GetAlls";
            using var helperThanhPhamSoChe = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamSoChecs = helperThanhPhamSoChe.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamSoCheUrl);
            ViewBag.ThanhPhamSoChes = thanhPhamSoChecs.Result.ToList();

            var apiThanhPhamCongViecTaiCheUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCongViecTaiChes/GetAlls";
            using var helperThanhPhamCongViecTaiChe = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamCongViecTaiChecs = helperThanhPhamCongViecTaiChe.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamCongViecTaiCheUrl);
            ViewBag.ThanhPhamCongViecTaiChes = thanhPhamCongViecTaiChecs.Result.ToList();

            var apiThanhPhamCongDoanUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCongDoanXepKhuons/GetAlls";
            using var helperThanhPhamCongDoan = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamCongDoancs = helperThanhPhamCongDoan.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiThanhPhamCongDoanUrl);
            ViewBag.ThanhPhamCongDoans = thanhPhamCongDoancs.Result.ToList();


            ViewBag.TitlePage = "Công Việc Theo Loại Thành Phẩm";
            return View("~/Views/TinhLuong/XepKhuon/CongViecLoaiThanhPhamView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllCongViecTheoThanhPhams()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        //public async Task<IActionResult> CreatDefautNew()
        //{
        //    try
        //    {
        //        var dataSource = await GetAlls();
        //        if (dataSource != null)
        //        {

        //            var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
        //            .Max();
        //            var id = (int.Parse(maxId) + 1).ToString("000");
        //            return Json(new
        //            {
        //                isSuccess = true,
        //                Ma = id,
        //                SuDung = true,
        //            });
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Mesages = "Lỗi!"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Thêm Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoInsertCongViecTheoThanhPham(string maCongViec, string maThanhPhamDinhHinh, string maThanhPhamPhuXepKhuon, string maThanhPhamChinhXepKhuon, string maThanhPhamBlockXepKhuon, string maThanhPhamKHCXepKhuon, string maThanhPhamTaiChe, string maThanhPhamSoChe, string maCongViecTaiChe, string maCongDoan, string maChieuXaChinhXepKhuon)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams/Insert";
            try
            {
                if (string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(maThanhPhamDinhHinh))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new CongViecTinhLuongXepKhuonTheoLoaiThanhPham
                {
                    MaCongViec = maCongViec,
                    MaThanhPhamDinhHinh = maThanhPhamDinhHinh,
                    MaThanhPhamPhuXepKhuon = maThanhPhamPhuXepKhuon,
                    MaThanhPhamChinhXepKhuon = maThanhPhamChinhXepKhuon,
                    MaThanhPhamBlockXepKhuon = maThanhPhamBlockXepKhuon,
                    MaThanhPhamKHCXepKhuon = maThanhPhamKHCXepKhuon,
                    MaThanhPhamTaiChe = maThanhPhamTaiChe,
                    MaThanhPhamSoChe = maThanhPhamSoChe,
                    MaCongViecTaiChe = maCongViecTaiChe,
                    MaCongDoan = maCongDoan,
                    MaChieuXaChinhXepKhuon = maChieuXaChinhXepKhuon
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
        public async Task<IActionResult> GetsByMaCongViecTheoThanhPham(string maCongViec, string maThanhPhamDinhHinh, string maThanhPhamPhuXepKhuon, string maThanhPhamChinhXepKhuon, string maThanhPhamBlockXepKhuon, string maThanhPhamKHCXepKhuon, string maThanhPhamTaiChe, string maThanhPhamSoChe, string maCongViecTaiChe, string maCongDoan, string maChieuXaChinhXepKhuon)
        {
            if (string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(maThanhPhamDinhHinh))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams/GetsByMa/{maCongViec}/{maThanhPhamDinhHinh}/{maThanhPhamPhuXepKhuon}/{maThanhPhamChinhXepKhuon}/{maThanhPhamBlockXepKhuon}/{maThanhPhamKHCXepKhuon}/{maThanhPhamTaiChe}/{maThanhPhamSoChe}/{maCongViecTaiChe}/{maCongDoan}/{maChieuXaChinhXepKhuon}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<CongViecTinhLuongXepKhuonTheoLoaiThanhPham>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaCongViec = item?.MaCongViec,
                        MaThanhPhamDinhHinh = item?.MaThanhPhamDinhHinh,
                        MaThanhPhamPhuXepKhuon = item?.MaThanhPhamPhuXepKhuon,
                        MaThanhPhamChinhXepKhuon = item?.MaThanhPhamChinhXepKhuon,
                        MaThanhPhamBlockXepKhuon = item?.MaThanhPhamBlockXepKhuon,
                        MaThanhPhamKHCXepKhuon = item?.MaThanhPhamKHCXepKhuon,
                        MaThanhPhamTaiChe = item?.MaThanhPhamTaiChe,
                        MaThanhPhamSoChe = item?.MaThanhPhamSoChe,
                        MaCongViecTaiChe = item?.MaCongViecTaiChe,
                        MaCongDoan = item?.MaCongDoan,
                        MaChieuXaChinhXepKhuon = item?.MaChieuXaChinhXepKhuon
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Sửa Quản Lý Phụ Gia / Đơn Vị Tính")]
        //public async Task<IActionResult> DoUpDate(string ma, string ten, bool suDung)
        //{
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_DonViTinh/Update/{ma}";
        //    try
        //    {
        //        // Kiểm tra dữ liệu đầu vào
        //        if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Messages = "Vui lòng nhập đầy đủ thông tin."
        //            });
        //        }
        //        var model = new PD_DonViTinh
        //        {
        //            Ma = ma,
        //            Ten = ten,
        //            SuDung = suDung
        //        };

        //        var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
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
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Xóa Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoDeleteCongViecTheoThanhPham(string maCongViec, string maThanhPhamDinhHinh, string maThanhPhamPhuXepKhuon, string maThanhPhamChinhXepKhuon, string maThanhPhamBlockXepKhuon, string maThanhPhamKHCXepKhuon, string maThanhPhamTaiChe, string maThanhPhamSoChe, string maCongViecTaiChe, string maCongDoan, string maChieuXaChinhXepKhuon)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonTheoLoaiThanhPhams/Delete/{maCongViec}/{maThanhPhamDinhHinh}/{maThanhPhamPhuXepKhuon}/{maThanhPhamChinhXepKhuon}/{maThanhPhamBlockXepKhuon}/{maThanhPhamKHCXepKhuon}/{maThanhPhamTaiChe}/{maThanhPhamSoChe}/{maCongViecTaiChe}/{maCongDoan}/{maChieuXaChinhXepKhuon}";
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
        #endregion
        #region Tính Lương Công Việc Xếp Khuôn Sản Lượng
        public IActionResult CongViecSanLuongView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CongViecSanLuongView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiCongViecUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/GetAlls";
            using var helperCongViec = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var congviecs = helperCongViec.GetAsync<IEnumerable<CongViecTinhLuongXepKhuon>>(HttpContext, apiCongViecUrl);
            ViewBag.CongViecs = congviecs.Result.ToList();

            var apiCasUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCas/GetCas";
            using var helperCas = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var Cass = helperCas.GetAsync<IEnumerable<MaCa>>(HttpContext, apiCasUrl);
            ViewBag.Cass = Cass.Result.ToList();


            ViewBag.TitlePage = "Công Việc Theo Loại Thành Phẩm";
            return View("~/Views/TinhLuong/XepKhuon/CongViecSanLuongView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllCongViecSanLuongs()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonSanLuongs/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Thêm Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoInsertCongViecSanLuong(string maCongViec, string maCa, string maXuong, decimal sanLuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonSanLuongs/Insert";
            try
            {

                if (string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new CongViecTinhLuongXepKhuonSanLuong
                {
                    MaCongViec = maCongViec,
                    Ngay = DateTime.Now,
                    MaCa = maCa,
                    MaXuong = maXuong,
                    SanLuong = sanLuong
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
        public async Task<IActionResult> GetsByMaCongViecSanLuong(string maCongViec, string maCa, string maXuong)
        {
            if (string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(maXuong))
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
                var dateTime = DateTime.Now;
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonSanLuongs/GetsByMa/{maCongViec}/{dateTime.ToString("yyyy-MM-dd")}/{maCa}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<CongViecTinhLuongXepKhuonSanLuong>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaCongViec = item?.MaCongViec,
                        Ngay = item?.Ngay,
                        MaCa = item?.MaCa,
                        MaXuong = item?.MaXuong,
                        SanLuong = item?.SanLuong
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }

        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Xóa Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoDeleteCongViecSanLuong(string maCongViec, DateTime dateTime, string maCa, string maXuong)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuonSanLuongs/Delete/{maCongViec}/{dateTime.ToString("yyyy-MM-dd")}/{maCa}/{maXuong}";
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
        #endregion
        #region Công Việc
        public IActionResult CongViecView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CongViecView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiLoaiDuLieuUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/GetLoaiDuLieus";
            using var helperLoaiDuLieu = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var LoaiDuLieus = helperLoaiDuLieu.GetAsync<IEnumerable<BravoModelV1.Model.LoaiDuLieu>>(HttpContext, apiLoaiDuLieuUrl);
            ViewBag.LoaiDuLieus = LoaiDuLieus.Result.ToList();
            ViewBag.TitlePage = "Công Việc Tính Lương";
            return View("~/Views/TinhLuong/XepKhuon/CongViecView.cshtml");
        }
        public async Task<IEnumerable<CongViecTinhLuongXepKhuon>> GetAllCongViecs()
        {
            IEnumerable<CongViecTinhLuongXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<CongViecTinhLuongXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNewCongViec()
        {
            try
            {
                var dataSource = await GetAllCongViecs();
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
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Thêm Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoInsertCongViec(string ma, string ten, string bravoId, string bravoIdDem, bool suDung, int loaiDuLieu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/Insert";
            try
            {

                if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(bravoId) || string.IsNullOrEmpty(suDung.ToString()) || string.IsNullOrEmpty(loaiDuLieu.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new CongViecTinhLuongXepKhuon
                {
                    Ma = ma,
                    Ten = ten,
                    BravoId = bravoId,
                    BravoIdDem = bravoIdDem,
                    SuDung = suDung,
                    LoaiDuLieu = loaiDuLieu
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
        public async Task<IActionResult> GetsByMaCongViec(string ma)
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
                var dateTime = DateTime.Now;
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<CongViecTinhLuongXepKhuon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Ma = item?.Ma,
                        Ten = item?.Ten,
                        BravoId = item?.BravoId,
                        BravoIdDem = item?.BravoIdDem,
                        SuDung = item?.SuDung,
                        LoaiDuLieu = item?.LoaiDuLieu,

                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> DoUpDateCongViec(string ma, string ten, string bravoId, string bravoIdDem, bool suDung, int loaiDuLieu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/Update/{ma}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(bravoId) || string.IsNullOrEmpty(suDung.ToString()) || string.IsNullOrEmpty(loaiDuLieu.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new CongViecTinhLuongXepKhuon
                {
                    Ma = ma,
                    Ten = ten,
                    BravoId = bravoId,
                    BravoIdDem = bravoIdDem,
                    SuDung = suDung,
                    LoaiDuLieu = loaiDuLieu
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
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Xóa Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoDeleteCongViec(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/Delete/{ma}";
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
        #endregion
        #region Nhóm
        public IActionResult NhomView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<List<XiNghiep>>(HttpContext, apiUrl);
            ViewBag.listXuongs = items.Result.ToList();
            ViewBag.TitlePage = "Nhóm Tính Lương";
            return View("~/Views/TinhLuong/XepKhuon/NhomView.cshtml");
        }
        public async Task<IEnumerable<MaNhomXepKhuon>> GetAllNhoms(string maXuong)
        {
            IEnumerable<MaNhomXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhomXepKhuons/GetAllsByMaXuong/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaNhomXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNewNhom(string maXuong)
        {
            try
            {
                var dataSource = await GetAllNhoms(maXuong);
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
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Thêm Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoInsertNhom(string ma, string ten, string maXuong, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhomXepKhuons/Insert";
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
                var model = new MaNhomXepKhuon
                {
                    Ma = ma,
                    Ten = ten,
                    MaXuong = maXuong,
                    GhiChu = ghiChu
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
        public async Task<IActionResult> GetsByMaNhom(string ma)
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
                var dateTime = DateTime.Now;
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhomXepKhuons/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaNhomXepKhuon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Ma = item?.Ma,
                        Ten = item?.Ten,
                        MaXuong = item?.MaXuong,
                        GhiChu = item?.GhiChu


                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> DoUpDateNhom(string ma, string ten, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhomXepKhuons/Update/{ma}";
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
                var model = new MaNhomXepKhuon
                {
                    Ma = ma,
                    Ten = ten,
                    GhiChu = ghiChu,
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
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Xóa Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoDeleteNhom(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhomXepKhuons/Delete/{ma}";
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
        #endregion
        #region Nhập mốc thời gian
        public IActionResult NhapMocThoiGianView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhapMocThoiGianView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/GetNguoiDungNeededDatas";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<List<NguoiDung>>(HttpContext, apiUrl);
            ViewBag.listNguoiDung = items.Result.ToList();
            ViewBag.TitlePage = "Mốc Giờ Phân Ca";
            return View("~/Views/TinhLuong/XepKhuon/NhapMocThoiGianView.cshtml");
        }
        public async Task<IEnumerable<MocThoiGianPhanCa>> GetAllMocThoiGianPhanCas(DateTime dateTime, string maXuong)
        {
            IEnumerable<MocThoiGianPhanCa> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MocThoiGianPhanCas/GetAllsByNgayMaXuong/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MocThoiGianPhanCa>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> DoInsertMocThoiGianPhanCa(TimeSpan gio, string userName, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MocThoiGianPhanCas/Insert";
            try
            {

                if (string.IsNullOrEmpty(gio.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập mốc thời gian."
                    });
                }
                var model = new MocThoiGianPhanCa
                {
                    Ngay = DateTime.Now,
                    Gio = gio,
                    CreateDateTime = DateTime.Now,
                    CreateBy = userName,
                    ModifyDateTime = DateTime.Now,
                    ModifyBy = "#",
                    MaXuong = maXuong
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
        public async Task<IActionResult> DoUpDateMocThoiGianPhanCa(TimeSpan gio, string gioGoc, string userName, string maXuong)
        {
            TimeSpan timeSpan = TimeSpan.Parse(gioGoc);
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MocThoiGianPhanCas/Update/{gio}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(gio.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập mốc thời gian."
                    });
                }
                var model = new MocThoiGianPhanCa
                {
                    Ngay = DateTime.Now,
                    Gio = timeSpan,
                    ModifyDateTime = DateTime.Now,
                    ModifyBy = userName,
                    MaXuong = maXuong
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
        #endregion
        #region Cài Đặt Sản Lượng Nhân Viên Theo Lượt Ra Cối
        public IActionResult SanLuongNhanVienTheoLuotRaCoiView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "SanLuongNhanVienTheoLuotRaCoiView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var maXuong = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhomXepKhuons/GetAllsByMaXuong/{maXuong}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<List<MaNhomXepKhuon>>(HttpContext, apiUrl);
            ViewBag.NhomXepKhuon = items.Result.ToList();
            ViewBag.TitlePage = "Sản Lượng Nhân Viên Theo Lượt Ra Cối";
            return View("~/Views/TinhLuong/XepKhuon/SanLuongNhanVienTheoLuotRaCoiView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllSanLuongLuotRaCois(DateTime dateTime, string maXuong)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuSanLuongRaCoiTinhLuongXepKhuons/GetAllsFullField/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> DoUpDateSanLuongLuotRaCoi(int stt, DateTime ngay, string maXuong, string maMayCan, string maNhom)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuSanLuongRaCoiTinhLuongXepKhuons/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maXuong}/{maMayCan}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(stt.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui chọn thông tin cần cập nhật."
                    });
                }
                var model = new PhieuSanLuongRaCoiTinhLuongXepKhuon
                {
                    MaNhom = maNhom
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
        public async Task<IActionResult> GetsBySTTSanLuongLuotRaCoi(int stt, DateTime ngay, string maXuong, string maMayCan)
        {
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maMayCan))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuSanLuongRaCoiTinhLuongXepKhuons/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maXuong}/{maMayCan}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuSanLuongRaCoiTinhLuongXepKhuon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item?.STT,
                        Ngay = item?.Ngay,
                        MaXuong = item?.MaXuong,
                        MaMayCan = item?.MaMayCan,
                        MaUserCan = item?.MaUserCan,
                        Gio = item?.Gio,
                        NgayThem = item?.NgayThem,
                        GioRaCoi = item?.GioRaCoi,
                        MaCoi = item?.MaCoi,
                        LuotRaCoi = item?.LuotRaCoi,
                        MaNhom = item?.MaNhom,
                        TrongLuong = item?.TrongLuong,
                        GhiChu = item?.GhiChu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> DoDeleteAllsSanLuongLuotRaCoi(DateTime dateTime, string maXuong)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuSanLuongRaCoiTinhLuongXepKhuons/DeleteAlls/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
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
        #endregion
        #region Cài Đặt Nhân Viên Viên Theo Nhóm Xếp Khuôn
        public IActionResult NhanVienTheoNhomXepKhuonView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhanVienTheoNhomXepKhuonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var maXuong = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhomXepKhuons/GetAllsByMaXuong/{maXuong}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<List<MaNhomXepKhuon>>(HttpContext, apiUrl);
            ViewBag.listNhoms = items.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanViens = helperNhanVien.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiNhanVienUrl);
            ViewBag.listNhanViens = nhanViens.Result.ToList();
            ViewBag.TitlePage = "Cài Đặt Nhân Viên Công Việc";
            return View("~/Views/TinhLuong/XepKhuon/NhanVienTheoNhomXepKhuonView.cshtml");
        }
        public async Task<IEnumerable<MaNhanVienTheoNhomXepKhuon>> GetAllByMaNhoms(string maNhom, DateTime dateTime)
        {
            IEnumerable<MaNhanVienTheoNhomXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhanVienTheoNhomXepKhuons/GetAllByMaNhoms/{maNhom}/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaNhanVienTheoNhomXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Thêm Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoInsertNhanVienNhom(string selectedIdsString, string maNhom, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhanVienTheoNhomXepKhuons/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(maNhom) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maNhanViens = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                var nhanVienNhoms = new List<MaNhanVienTheoNhomXepKhuon>();
                foreach (var item in maNhanViens)
                {

                    var nhanVienNhom = new MaNhanVienTheoNhomXepKhuon
                    {
                        MaNhanVien = item,
                        MaNhom = maNhom,
                        Ngay = DateTime.Now,
                        TyLeHuong = 1,
                        TyLeTru = 0,
                        SoGio = 8
                    };
                    nhanVienNhoms.Add(nhanVienNhom);
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var model in nhanVienNhoms)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!response.Success)
                    {
                        failureCount++;
                    }
                    else
                    {
                        successCount++;
                    }
                }

                if (failureCount > 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"{failureCount} phần tử không thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $"{successCount} phần tử thành công."
                    });
                }

                //return Json(new
                //{
                //    isSuccess = false,
                //    Messages = "Insert không thành công."
                //});
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có.
                return Json(new
                {
                    isSuccess = false,
                    Messages = ex.Message
                });
            }
        }

        public async Task<IActionResult> DoUpDateNhanVienNhom(string maNhanVien, string maNhom, DateTime dateTime, decimal tyLeHuong, decimal tyLeTru, decimal soGio)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhanVienTheoNhomXepKhuons/Update/{maNhanVien}/{maNhom}/{dateTime.ToString("yyyy-MM-dd")}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maNhom))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaNhanVienTheoNhomXepKhuon
                {
                    TyLeHuong = tyLeHuong,
                    TyLeTru = tyLeTru,
                    SoGio = soGio
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
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Xóa Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoDeleteNhanVienNhom(string selectedIdNhanVienNhoms)
        {
            try
            {
                string[] ids = selectedIdNhanVienNhoms.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (!ids.Any())
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không có phần tử nào được chọn."
                    });
                }

                var maList = ids.Select(id =>
                {
                    var parts = id.Split('-');
                    return new MaNhanVienTheoNhomXepKhuon
                    {
                        MaNhanVien = parts[0],
                        MaNhom = parts[1],
                        Ngay = DateTime.Parse(parts[2])
                    };
                }).ToList();

                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNhanVienTheoNhomXepKhuons/Delete";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(maList), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (!rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = rl.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã xoá các mục thành công!"
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
        #region cài dặt nhân viên theo công việc tính lương
        public IActionResult NhanVienTheoCongViecView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhanVienTheoCongViecView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecTinhLuongXepKhuons/GetAlls";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<List<CongViecTinhLuongXepKhuon>>(HttpContext, apiUrl);
            ViewBag.listCongViecs = items.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanViens = helperNhanVien.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiNhanVienUrl);
            ViewBag.listNhanViens = nhanViens.Result.ToList();
            ViewBag.TitlePage = "Cài Đặt Nhân Viên Công Việc";
            return View("~/Views/TinhLuong/XepKhuon/NhanVienTheoCongViecView.cshtml");
        }
        public async Task<IEnumerable<BoTriTinhLuonXepKhuon>> GetAllByMaCongViecs(string maCongViec, DateTime dateTime, string maXuong)
        {
            IEnumerable<BoTriTinhLuonXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriTinhLuonXepKhuons/GetAllByMaCongViecs/{maCongViec}/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<BoTriTinhLuonXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> DoInsertNhanVienCongViec(string selectedIdsString, string maCongViec, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriTinhLuonXepKhuons/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maNhanViens = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                var nhanVienCongViecs = new List<BoTriTinhLuonXepKhuon>();
                foreach (var item in maNhanViens)
                {

                    var nhanVienCongViec = new BoTriTinhLuonXepKhuon
                    {
                        MaNhanVien = item,
                        MaCongViec = maCongViec,
                        Ngay = DateTime.Now,
                        MaXuong = maXuong,
                        TyLeHuong = 1,
                        TyLeTru = 0,
                        SoGio = 8
                    };
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var model in nhanVienCongViecs)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!response.Success)
                    {
                        failureCount++;
                    }
                    else
                    {
                        successCount++;
                    }
                }

                if (failureCount > 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"{failureCount} phần tử không thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $"{successCount} phần tử thành công."
                    });
                }

                //return Json(new
                //{
                //    isSuccess = false,
                //    Messages = "Insert không thành công."
                //});
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có.
                return Json(new
                {
                    isSuccess = false,
                    Messages = ex.Message
                });
            }
        }
        public async Task<IActionResult> DoUpDateNhanVienCongViec(string maNhanVien, string maCongViec, DateTime dateTime, string maXuong, decimal tyLeHuong, decimal tyLeTru, decimal soGio)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriTinhLuonXepKhuons/Update/{maNhanVien}/{maCongViec}/{dateTime.ToString("yyyy-MM-dd")}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maCongViec))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new BoTriTinhLuonXepKhuon
                {
                    MaXuong = maXuong,
                    TyLeHuong = tyLeHuong,
                    TyLeTru = tyLeTru,
                    SoGio = soGio
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
        public async Task<IActionResult> DoDeleteNhanVienCongViec(string selectedIdNhanVienNhoms)
        {
            try
            {
                string[] ids = selectedIdNhanVienNhoms.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (!ids.Any())
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không có phần tử nào được chọn."
                    });
                }

                var maList = ids.Select(id =>
                {
                    var parts = id.Split('-');
                    return new BoTriTinhLuonXepKhuon
                    {
                        MaNhanVien = parts[0],
                        MaCongViec = parts[1],
                        Ngay = DateTime.Parse(parts[2]),
                        MaXuong = parts[3],
                    };
                }).ToList();

                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BoTriTinhLuonXepKhuons/Delete";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(maList), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (!rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = rl.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã xoá các mục thành công!"
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

    }
}
