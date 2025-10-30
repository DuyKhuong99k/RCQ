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

namespace PMS.Controllers.TinhLuong.PhuFillet
{
    [Authorize]
    public class ChinhPhuFilletController : Controller
    {
        private readonly IHubContext<PMS.Hubs.ProgressHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChinhPhuFilletController(IHubContext<PMS.Hubs.ProgressHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xem / Tính Lương / Phụ Fillet / Chính")]
        public IActionResult ChinhView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();
            var apiThanhPhamFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAllsByCodeId/{codeId}";
            using var helperThanhPhamFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamFLs = helperThanhPhamFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiThanhPhamFLUrl);
            ViewBag.listThanhPhamFLs = thanhPhamFLs.Result.ToList();
            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiKhuVucUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/KhuVucs/Gets";
            using var helperKhuVuc = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khuvuc = helperKhuVuc.GetAsync<IEnumerable<object>>(HttpContext, apiKhuVucUrl);
            ViewBag.listKhuVucs = khuvuc.Result.ToList();

            ViewBag.token = HttpContext.Session.GetString("JWTTokenEncry");
            ViewBag.titile = "Tính Lương Định Hình Chính";
            return View("~/Views/TinhLuong/PhuFillet/ChinhView.cshtml");
        }
        public async Task<IEnumerable<object>> LoadSanLuongPhuFillet(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/LoadSanLuongPhuFillet/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
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
                    case "LoadSanLuongPhuFillet":
                        dataSource = await LoadSanLuongPhuFillet(dateTime, xuongId);
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/KetChuyen/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
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
        #region xác nhận giờ ra vào
        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xác Nhận Giờ Vào Ra / Tính Lương / Phụ Fillet / Chính")]
        public IActionResult XacNhanGioVaoRaView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiThanhPhamPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecPhuFillets/GetAlls";
            using var helperThanhPhamPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamPFLs = helperThanhPhamPFL.GetAsync<IEnumerable<CongViecPhuFillet>>(HttpContext, apiThanhPhamPFLUrl);
            ViewBag.listThanhPhamPFLs = thanhPhamPFLs.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanViens = helperNhanVien.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiNhanVienUrl);
            ViewBag.listNhanViens = nhanViens.Result.ToList();

            ViewBag.titile = "Xác Nhận Giờ Vào Ra";
            return View("~/Views/TinhLuong/PhuFillet/XacNhanGioVaoRaView.cshtml");
        }
        public async Task<IActionResult> LoadPhanBoNhanVienPhuFillet(DateTime dateTime, string xuongId, string thanhPham)
        {
            bool success = true;

            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecPhuFillets/LoadPhanBoNhanVienPhuFillet/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{thanhPham}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
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
        public async Task<IActionResult> LoadGioRaVaoFilletFillet(DateTime dateTime, string xuongId)
        {
            bool success = true;

            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/GioVaoRaFillets/LoadGioRaVaoFillet/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
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
        public async Task<IActionResult> FindNhanVienByMaHoSo(DateTime dateTime, string xuongId, string thanhPham, string maHoSoInput)
        {
            bool success = true;

            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecPhuFillets/FindNhanVienByMaHoSo/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{thanhPham}/{maHoSoInput}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
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
        public async Task<IActionResult> NhanViensFindGhiNhanSelectedItemChanged(DateTime dateTime, string xuongId, string thanhPham, string maNhanVien)
        {
            bool success = true;

            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/GioVaoRaFillets/NhanViensFindGhiNhanSelectedItemChanged/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{thanhPham}/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
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
        public async Task<IActionResult> SetGioVaoRaMacDinh(DateTime dateTime, string xuongId, TimeSpan gioVao, TimeSpan gioRa, string thanhPham, string listPhanBoNhanVienPhuFilletSelectedItems)
        {


            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/GioVaoRaFillets/SetGioVaoRaMacDinh/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{thanhPham}/{gioVao}/{gioRa}";
            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listPhanBoNhanVienPhuFilletSelectedItems));
            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
            if (rl.Success)
            {
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
            return Json(new
            {
                isSuccess = false,
                Messages = "Đã xảy ra lỗi. Vui lòng thử lại sau."
            });



        }
        #endregion
        #region Cài đặt nhân viên phụ theo công việc
        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xác Nhận Giờ Vào Ra / Tính Lương / Phụ Fillet / Chính")]
        public IActionResult CaiDatNhanVienPhuTheoCongViecView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CaiDatNhanVienPhuTheoCongViecView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiThanhPhamPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecPhuFillets/GetAlls";
            using var helperThanhPhamPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamPFLs = helperThanhPhamPFL.GetAsync<IEnumerable<CongViecPhuFillet>>(HttpContext, apiThanhPhamPFLUrl);
            ViewBag.listThanhPhamPFLs = thanhPhamPFLs.Result.ToList();


            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanViens = helperNhanVien.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiNhanVienUrl);
            ViewBag.listNhanViens = nhanViens.Result.ToList();
            ViewBag.titile = "Xác Nhận Giờ Vào Ra";
            return View("~/Views/TinhLuong/PhuFillet/CaiDatNhanVienPhuTheoCongViecView.cshtml");
        }
        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xác Nhận Giờ Vào Ra / Tính Lương / Phụ Fillet / Chính")]
        public async Task<IActionResult> DoInsertNhanVienPhuTheoCongViec(string selectedIdsString, string maCongViec, DateTime dateTime, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhanBoNhanVienTheoCongViecPhuFillets/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maNhanViens = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var nhanVienPhuTheoCongViecs = new List<PhanBoNhanVienTheoCongViecPhuFillet>();
                foreach (var item in maNhanViens)
                {

                    var nhanVienPhuCongViec = new PhanBoNhanVienTheoCongViecPhuFillet
                    {
                        MaNhanVien = item,
                        MaCongViecPhuFillet = maCongViec,
                        Ngay = dateTime,
                        MaXuong = maXuong,
                        TyLeHuong = 1,
                        TyLeTru = 0
                    };
                    nhanVienPhuTheoCongViecs.Add(nhanVienPhuCongViec);
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var nhanVienPhuTheoCongViec in nhanVienPhuTheoCongViecs)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(nhanVienPhuTheoCongViec), Encoding.UTF8, "application/json");
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
        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xác Nhận Giờ Vào Ra / Tính Lương / Phụ Fillet / Chính")]
        public async Task<IActionResult> DoDeleteNhanVienPhuTheoCongViec(string selectedIdsStringNhanVienTheoCongViec, string maCongViec, DateTime dateTime, string maXuong)
        {
            try
            {
                string[] maNhanViens = selectedIdsStringNhanVienTheoCongViec.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var successCount = 0;
                var failureCount = 0;
                foreach (var maNhanVien in maNhanViens)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhanBoNhanVienTheoCongViecPhuFillets/Delete/{maNhanVien}/{maCongViec}/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var rl = await helper.PostAsync(HttpContext, apiUrl, null);

                    if (!rl.Success)
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
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Có lỗi xảy ra"
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
        #region Cài đặt nhân viên phụ theo bàn và line

        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xác Nhận Giờ Vào Ra / Tính Lương / Phụ Fillet / Chính")]
        public IActionResult CaiDatNhanVienPhuTheoBanVaLineView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CaiDatNhanVienPhuTheoBanVaLineView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiThanhPhamPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecPhuFillets/GetAlls";
            using var helperThanhPhamPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamPFLs = helperThanhPhamPFL.GetAsync<IEnumerable<CongViecPhuFillet>>(HttpContext, apiThanhPhamPFLUrl);
            ViewBag.listThanhPhamPFLs = thanhPhamPFLs.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanViens = helperNhanVien.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiNhanVienUrl);
            ViewBag.listNhanViens = nhanViens.Result.ToList();
            ViewBag.titile = "Cài Đặt NV Bàn Line";
            return View("~/Views/TinhLuong/PhuFillet/CaiDatNhanVienPhuTheoBanVaLineView.cshtml");
        }

        public async Task<IActionResult> LoadNhanVienCaiDatNVBanLine(DateTime dateTime, string xuongId)
        {
            bool success = true;

            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/LoadCaiDatNhanVienBanLinePhuFilletTL/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
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
        #endregion
        #region Cài đặt sản lượng
        [CustomAuthorize(Fu = "Tính Lương / Phụ Fillet / Chính", Func = "Xác Nhận Giờ Vào Ra / Tính Lương / Phụ Fillet / Chính")]
        public IActionResult CaiDatSanLuongView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CaiDatSanLuongView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiThanhPhamPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/CongViecPhuFillets/GetAlls";
            using var helperThanhPhamPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamPFLs = helperThanhPhamPFL.GetAsync<IEnumerable<CongViecPhuFillet>>(HttpContext, apiThanhPhamPFLUrl);
            ViewBag.listThanhPhamPFLs = thanhPhamPFLs.Result.ToList();


            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanViens = helperNhanVien.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiNhanVienUrl);
            ViewBag.listNhanViens = nhanViens.Result.ToList();
            ViewBag.titile = "Xác Nhận Giờ Vào Ra";
            return View("~/Views/TinhLuong/PhuFillet/CaiDatSanLuongView.cshtml");
        }
        #endregion
    }
}
