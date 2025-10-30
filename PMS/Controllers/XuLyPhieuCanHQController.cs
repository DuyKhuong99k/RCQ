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
using AppViewModels;
using System.Transactions;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Syncfusion.EJ2.Layouts;


namespace PMS.Controllers
{
    [Authorize]
    public class XuLyPhieuCanHQController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public XuLyPhieuCanHQController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanHQView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiL = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_Los/GetAlls";
            using var helperL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var l = helperL.GetAsync<IEnumerable<HQ_Lo>>(HttpContext, apiL);
            ViewBag.listLs = l.Result.ToList();

            var apiS = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_Sizes/GetAlls";
            using var helperS = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var s = helperS.GetAsync<IEnumerable<HQ_Size>>(HttpContext, apiS);
            ViewBag.listSs = s.Result.ToList();

            var apiTP = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThanhPhams/GetAlls";
            using var helperTP = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tp = helperTP.GetAsync<IEnumerable<HQ_ThanhPham>>(HttpContext, apiTP);
            ViewBag.listTPs = tp.Result.ToList();

            var apiLNL = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_LoaiNguyenLieus/GetAlls";
            using var helperLNL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var lnl = helperLNL.GetAsync<IEnumerable<HQ_ThanhPham>>(HttpContext, apiLNL);
            ViewBag.listLNLs = lnl.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            ViewBag.titile = "Xử Lý Phiếu Cân HQ";
            return View();
        }
        public async Task<IEnumerable<HQ_PhieuCan>> GetAllsWithDateAndXuongHQ(DateTime dateTime, string xuongId)
        {
            IEnumerable<HQ_PhieuCan> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<HQ_PhieuCan>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetChiTiets_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetChiTiets_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanSua(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetPhieuCanUpdateXLPCs/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanXoa(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetPhieuCanDeleteXLPCs/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetNhanVien_XLPC()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNewHQ(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongHQ(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MayCan == AppViewModels.AppViewModel.Instance.PCName)
                        .Where(x => x.STT != null)
                        .Select(x => Math.Abs(x.STT))
                        .DefaultIfEmpty(0)
                        .Max();
                    var mayCan = AppViewModels.AppViewModel.Instance.PCName;

                    var stt = maxstt + 1;
                    var id = dateTime.ToString("yyyyMMdd") + "." + xuongId + "." + mayCan + "." + stt.ToString("00000");
                    return Json(new
                    {
                        isSuccess = true,
                        Id = id,
                        STT = stt,
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

        public async Task<IActionResult> GetsByMa_HQ(string listInfoPhieuCan, DateTime ngay)
        {

            if (string.IsNullOrEmpty(listInfoPhieuCan))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            string id = listInfoPhieuCan.Split(',')[0];
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<HQ_PhieuCan>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        STT = item.STT,
                        Ngay = item.Ngay,
                        NgayGio = item.NgayGio,
                        MayCan = item.MayCan,
                        MaLo = item.MaLo,
                        MaSize = item.MaSize,
                        MaThanhPham = item.MaThanhPham,
                        MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                        MaNhanVien = item.MaNhanVien,
                        MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                        MaNhanVienBanKiem = item.MaNhanVienBanKiem,
                        TrongLuong = item.TrongLuong,
                        TrongLuongTare = item.TrongLuongTare,
                        ChiSanLuong = item.ChiSanLuong,
                        Status = item.Status,
                        TheId = item.TheId,
                        TheIdNhanVien = item.TheIdNhanVien,
                        GhiChu = item.GhiChu,
                        MaXuong = item.MaXuong,
                        Gio = item.NgayGio.TimeOfDay
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> DoInsert_HQ(DateTime dateTime, TimeSpan gio, string xuongId, string id, int stt, string maLo, string maSize, string maThanhPham, string maLoaiNguyenLieu, string maNhanVien, string maNhanVienPhucVu, string maNhanVienBanKiem, decimal trongLuong)
        {
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);
            DateTime dateWithTime = dateTime.Add(gio);
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/Insert";
            try
            {
                // if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maLoaiNguyenLieu) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maNhanVienPhucVu) || string.IsNullOrEmpty(maNhanVienBanKiem) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(trongLuong.ToString()))
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maLoaiNguyenLieu) || string.IsNullOrEmpty(maNhanVien) ||  string.IsNullOrEmpty(id) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_PhieuCan
                {
                    Id = id,
                    STT = stt,
                    MayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaLo = maLo,
                    MaSize = maSize,
                    MaThanhPham = maThanhPham,
                    MaLoaiNguyenLieu = maLoaiNguyenLieu,
                    MaNhanVien = maNhanVien,
                    MaNhanVienPhucVu = maNhanVienPhucVu,
                    MaNhanVienBanKiem = maNhanVienBanKiem,
                    MaXuong = xuongId,
                    TrongLuong = trongLuong,
                    TheId = "0",
                    TheIdNhanVien = "0",
                    ChiSanLuong = false,
                    Ngay = dateOnly,
                    NgayGio = dateWithTime,
                    Status = 0,
                    TrongLuongTare = 0,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THÊM,",
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> DoUpDate_HQ(string id, string maLo, string maSize, string maThanhPham, string maLoaiNguyenLieu, string maNhanVien, string maNhanVienPhucVu, string maNhanVienBanKiem)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/Update/{id}";
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maLoaiNguyenLieu) || string.IsNullOrEmpty(maNhanVien) )
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new
                {
                    Id = id,
                    STT = 0,
                    MayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaLo = maLo,
                    MaSize = maSize,
                    MaThanhPham = maThanhPham,
                    MaLoaiNguyenLieu = maLoaiNguyenLieu,
                    MaNhanVien = maNhanVien,
                    MaNhanVienPhucVu = maNhanVienPhucVu,
                    MaNhanVienBanKiem = maNhanVienBanKiem,
                    MaXuong = "xuongId",
                    TrongLuong = 0,
                    TheId = "0",
                    TheIdNhanVien = "0",
                    ChiSanLuong = false,
                    Ngay = DateOnly.MaxValue,
                    NgayGio = DateTime.Now,
                    Status = 0,
                    TrongLuongTare = 0,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: SỬA",
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> DoDelete_HQ(string listInfoPhieuCan, DateTime ngay)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");
            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {

                    string id = parts[0];
                    int stt = int.Parse(parts[1]);
                    if (stt < 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = id + "XOA",
                        STT = stt * -1,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaSize = "maSize",
                        MaThanhPham = "maThanhPham",
                        MaLoaiNguyenLieu = "maLoaiNguyenLieu",
                        MaNhanVien = "maNhanVien",
                        MaNhanVienPhucVu = "maNhanVienPhucVu",
                        MaNhanVienBanKiem = "maNhanVienBanKiem",
                        MaXuong = "xuongId",
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/Delete/{id}";

                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {

                        return Json(new
                        {
                            isSuccess = responseTP.Success,
                            Messages = responseTP.Message
                        });

                    }
                }
                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã xoá toàn bộ phiếu cân được chọn!"
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
        public async Task<IActionResult>  CheckQuyenHQ(string typeOption)
        {
            try
            {
                // Lấy danh sách roleid từ cookie
                var roleIds = PMS.Middlewares.AuthenticationHelpers.GetRoleIdsFromCookie(HttpContext);

                // Lấy danh sách role permissions từ list roleId
                var rolePermistions = await PMS.Middlewares.AuthenticationHelpers.GetRolePermistionsAsync(HttpContext, roleIds);

                // Kiểm tra quyền dựa trên typeOption
                var permissionMapping = new Dictionary<string, string>
                {
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân HQ" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân HQ" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân HQ" },
                    { "CHUYENLOAINGUYENLIEU", "Chuyển Loại Nguyên Liệu Xử Lý Phiếu Cân / Phiếu Cân HQ" },
                    { "CHUYENNHANVIEN", "Chuyển Nhân Viên Xử Lý Phiếu Cân / Phiếu Cân HQ" },
                    { "CHUYENHANVIENPHUCVU", "Chuyển Nhân Viên Phục Vụ Xử Lý Phiếu Cân / Phiếu Cân HQ" },
                    { "CHUYENNHANVIENBANKIEM", "Chuyển Nhân Viên Bàn Kiểm Xử Lý Phiếu Cân / Phiếu Cân HQ" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân HQ" && x.Func == func && x.Status == 1);

                    if (permission != null)
                    {
                        return Json(new
                        {
                            isSuccess = true,
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Bạn không có quyền sử dụng chức năng này!"
                        });
                    }
                }

                return Json(new
                {
                    isSuccess = false,
                    Messages = "Tùy chọn không hợp lệ!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> ChuyenXuong_HQ(string listInfoPhieuCan, string maXuongChange)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {
                    string id = parts[0];

                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = "id",
                        STT = 01,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaSize = "maSize",
                        MaThanhPham = "maThanhPham",
                        MaLoaiNguyenLieu = "maLoaiNguyenLieu",
                        MaNhanVien = "maNhanVien",
                        MaNhanVienPhucVu = "maNhanVienPhucVu",
                        MaNhanVienBanKiem = "maNhanVienBanKiem",
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/ChuyenXuong/{id}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Xưởng Phiếu Cân thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Xưởng các phiếu  đã chọn! Vui lòng đổi xưởng để kiểm tra!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> ChuyenSize_HQ(string listInfoPhieuCan, string maSizeChange)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {
                    string id = parts[0];

                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = "id",
                        STT = 01,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaThanhPham = "maThanhPham",
                        MaLoaiNguyenLieu = "maLoaiNguyenLieu",
                        MaNhanVien = "maNhanVien",
                        MaNhanVienPhucVu = "maNhanVienPhucVu",
                        MaNhanVienBanKiem = "maNhanVienBanKiem",
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        MaXuong = "maXuongChange",
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/ChuyenSize/{id}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Size Phiếu Cân thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Size các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> ChuyenThanhPham_HQ(string listInfoPhieuCan, string maThanhPhamChange)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {
                    string id = parts[0];

                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = "id",
                        STT = 01,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaLoaiNguyenLieu = "maLoaiNguyenLieu",
                        MaNhanVien = "maNhanVien",
                        MaNhanVienPhucVu = "maNhanVienPhucVu",
                        MaNhanVienBanKiem = "maNhanVienBanKiem",
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        MaXuong = "maXuongChange",
                        MaSize = "maSizeChange",
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/ChuyenThanhPham/{id}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Thành Phẩm Phiếu Cân thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Thành Phẩm các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Chuyển Loại Nguyên Liệu Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> ChuyenLoaiNguyenLieu_HQ(string listInfoPhieuCan, string maLoaiNguyenLieuChange)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {
                    string id = parts[0];

                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = "id",
                        STT = 01,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaLoaiNguyenLieu = maLoaiNguyenLieuChange,
                        MaNhanVien = "maNhanVien",
                        MaNhanVienPhucVu = "maNhanVienPhucVu",
                        MaNhanVienBanKiem = "maNhanVienBanKiem",
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        MaXuong = "maXuongChange",
                        MaSize = "maSizeChange",
                        MaThanhPham = "maThanhPhamChange",
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/ChuyenLoaiNguyenLieu/{id}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Thành Phẩm Phiếu Cân thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Thành Phẩm các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Chuyển Nhân Viên Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> ChuyenNhanVien_HQ(string listInfoPhieuCan, string maNhanVienChange)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {
                    string id = parts[0];

                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = "id",
                        STT = 01,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaLoaiNguyenLieu = "maLoaiNguyenLieuChange",
                        MaNhanVien = maNhanVienChange,
                        MaNhanVienPhucVu = "maNhanVienPhucVu",
                        MaNhanVienBanKiem = "maNhanVienBanKiem",
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        MaXuong = "maXuongChange",
                        MaSize = "maSizeChange",
                        MaThanhPham = "maThanhPhamChange",
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/ChuyenNhanVien/{id}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Thành Phẩm Phiếu Cân thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Thành Phẩm các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Chuyển Nhân Viên Phục Vụ Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> ChuyenNhanVienPhucVu_HQ(string listInfoPhieuCan, string maNhanVienPhucVuChange)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {
                    string id = parts[0];

                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = "id",
                        STT = 01,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaLoaiNguyenLieu = "maLoaiNguyenLieuChange",
                        MaNhanVien = "maNhanVienChange",
                        MaNhanVienPhucVu = maNhanVienPhucVuChange,
                        MaNhanVienBanKiem = "maNhanVienBanKiem",
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        MaXuong = "maXuongChange",
                        MaSize = "maSizeChange",
                        MaThanhPham = "maThanhPhamChange",
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/ChuyenLoaiNguyenLieu/{id}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Thành Phẩm Phiếu Cân thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Thành Phẩm các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân HQ", Func = "Chuyển Nhân Viên Bàn Kiểm Xử Lý Phiếu Cân / Phiếu Cân HQ")]
        public async Task<IActionResult> ChuyenNhanVienBanKiem_HQ(string listInfoPhieuCan, string maNhanVienBanKiemChange)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Khởi tạo danh sách kết quả
            List<string[]> result = new List<string[]>();

            // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
            foreach (string subArray in subArrays)
            {
                string[] elements = subArray.Split(',');
                result.Add(elements);
            }

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var parts in result)
                {
                    string id = parts[0];

                    if (string.IsNullOrEmpty(id))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new
                    {
                        Id = "id",
                        STT = 01,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaLo = "maLo",
                        MaLoaiNguyenLieu = "maLoaiNguyenLieuChange",
                        MaNhanVien = "maNhanVienChange",
                        MaNhanVienPhucVu = "maNhanVienPhucVuChange",
                        MaNhanVienBanKiem = maNhanVienBanKiemChange,
                        TrongLuong = 0,
                        TheId = "0",
                        TheIdNhanVien = "0",
                        ChiSanLuong = false,
                        Ngay = DateOnly.MaxValue,
                        NgayGio = DateTime.Now,
                        Status = 0,
                        TrongLuongTare = 0,
                        MaXuong = "maXuongChange",
                        MaSize = "maSizeChange",
                        MaThanhPham = "maThanhPhamChange",
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/ChuyenLoaiNguyenLieu/{id}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Thành Phẩm Phiếu Cân thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Thành Phẩm các phiếu  đã chọn!"
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
        public async Task<ActionResult> Reload(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;

            try
            {
                dataSource = await GetChiTiets_XLPC(dateTime, xuongId);

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
    }
}
