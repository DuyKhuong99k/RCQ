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
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Syncfusion.EJ2.Layouts;
using ToolsEx;

namespace PMS.Controllers
{
    [Authorize]
    public class XuLyPhieuCanNguyenLieuHQController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public XuLyPhieuCanNguyenLieuHQController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region view & data
        #region PHIEUCANNHAP
        public IActionResult XuLyPhieuCanNhapHQ()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanNguyenLieuView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Xử Lý Phiếu Cân Nhập Nguyên Liệu";
            var apiSanPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_SanPhamNguyenLieu/GetAlls";
            using var helperSanPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sanPhams = helperSanPham.GetAsync<IEnumerable<HQ_SanPhamNguyenLieu>>(HttpContext, apiSanPhamUrl);
            ViewBag.listSanPhams = sanPhams.Result.ToList();

            var apiQuyCachUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_QuyCachNguyenLieu/GetAlls";
            using var helperQuyCach = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var quyCachs = helperQuyCach.GetAsync<IEnumerable<HQ_QuyCachNguyenLieu>>(HttpContext, apiQuyCachUrl);
            ViewBag.listQuyCachs = quyCachs.Result.ToList();

            var apiKhoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_KhoNguyenLieu/GetAlls";
            using var helperKho = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khos = helperKho.GetAsync<IEnumerable<HQ_KhoNguyenLieu>>(HttpContext, apiKhoUrl);
            ViewBag.listKhos = khos.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongTiens = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = phuongTiens.Result.ToList();

            var apiDonViTinhUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DonViTinh/GetAlls";
            using var helperDonViTinh = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var donViTinhs = helperDonViTinh.GetAsync<IEnumerable<HQ_DonViTinh>>(HttpContext, apiDonViTinhUrl);
            ViewBag.listDonViTinhss = donViTinhs.Result.ToList();

            return View();
        }
        public async Task<IEnumerable<HQ_PhieuCanNhapNguyenLieu>> GetAllsWithDateAndXuongPhieuCanNhapHQ(DateTime dateTime, string xuongId)
        {
            IEnumerable<HQ_PhieuCanNhapNguyenLieu> data = new List<HQ_PhieuCanNhapNguyenLieu>();
            if (data.Count() == 0)
            {
                var apiUrl = $"{AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNguyenLieu/GetPhieuCanNguyenLieuByDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                data = await helper.GetAsync<IEnumerable<HQ_PhieuCanNhapNguyenLieu>>(HttpContext, apiUrl);
            }
            return data;
        }
        public async Task<IEnumerable<object>> GetPhieuCanNhapNguyenLieuHQ(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> data = new List<object>();
            if (data.Count() == 0)
            {
                var apiUrl = $"{AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/GetChiTietPhieuCanNhapNguyenLieus/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                data = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            }
            return data;
        }
        #endregion
        #region PHIEUCANXUAT
        public IActionResult XuLyPhieuCanXuatHQ()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanNguyenLieuView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Xử Lý Phiếu Cân Xuất Nguyên Liệu";
            var apiSanPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_SanPhamNguyenLieu/GetAlls";
            using var helperSanPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sanPhams = helperSanPham.GetAsync<IEnumerable<HQ_SanPhamNguyenLieu>>(HttpContext, apiSanPhamUrl);
            ViewBag.listSanPhams = sanPhams.Result.ToList();

            var apiQuyCachUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_QuyCachNguyenLieu/GetAlls";
            using var helperQuyCach = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var quyCachs = helperQuyCach.GetAsync<IEnumerable<HQ_QuyCachNguyenLieu>>(HttpContext, apiQuyCachUrl);
            ViewBag.listQuyCachs = quyCachs.Result.ToList();

            var apiKhoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_KhoNguyenLieu/GetAlls";
            using var helperKho = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khos = helperKho.GetAsync<IEnumerable<HQ_KhoNguyenLieu>>(HttpContext, apiKhoUrl);
            ViewBag.listKhos = khos.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongTiens = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = phuongTiens.Result.ToList();

            var apiDonViTinhUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_DonViTinh/GetAlls";
            using var helperDonViTinh = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var donViTinhs = helperDonViTinh.GetAsync<IEnumerable<HQ_DonViTinh>>(HttpContext, apiDonViTinhUrl);
            ViewBag.listDonViTinhss = donViTinhs.Result.ToList();
            return View();
        }
        public async Task<IEnumerable<object>> GetPhieuCanXuatNguyenLieuHQ(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> data = new List<object>();
            if (data == null)
            {
                var apiUrl = $"{AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetPhieuCanXuatNguyenLieuHQ/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                data = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            }
            return data;
        }
        #endregion
        #endregion
        #region crud
        #region PHIEUCANNHAP
        public async Task<IActionResult> CreatDefautNewNguyenLieuHQ(DateTime dateTime, string xuongId)
        {
            var data = await GetAllsWithDateAndXuongPhieuCanNhapHQ(dateTime, xuongId);
            if (data.Count() != 0)
            {
                var maxStt = data.Select(c => c.STT).DefaultIfEmpty(0).Max();
                var tenMayCan = AppViewModel.Instance.PCName;
                var id = tenMayCan + "." + dateTime.ToString("yyyyMMddhhmmss");
                return Json(new
                {
                    isSuccess = true,
                    Id = id,
                    STT = maxStt + 1
                });
            }
            else
            {
                return Json(new
                {
                    isSuccess = false,
                    Id = string.Empty,
                    STT = 0,
                    Mesages = "Lấy dữ liệu thất bại, vui lòng thử lại!"
                });
            }
        }

        public async Task<IEnumerable<HQ_PhieuCanNguyenLieu>> GetPhieuCanNguyenLieus(DateTime dateTime, string xuongId)
        {
            IEnumerable<HQ_PhieuCanNguyenLieu> data = new List<HQ_PhieuCanNguyenLieu>();
            if (data.Count() == 0)
            {
                var apiUrl = $"{AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNguyenLieu/GetPhieuCanNguyenLieuByDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                data = await helper.GetAsync<IEnumerable<HQ_PhieuCanNguyenLieu>>(HttpContext, apiUrl);
            }
            return data;
        }

        public async Task<IActionResult> GetByMaPhieuCanNhapHQ(string listInfoPhieuCan, DateTime ngay)
        {
            if (string.IsNullOrEmpty(listInfoPhieuCan))
            {
                return Json(new
                {
                    isSuccess = false,
                    Data = new List<HQ_PhieuCanNhapNguyenLieu>(),
                    Messages = "Dữ liệu truyền lên không hợp lệ, vui lòng thử lại!"
                });
            }
            string id = listInfoPhieuCan.Split(',')[0];
            IEnumerable<object> data = new List<object>();
            if (data.Count() == 0)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<HQ_PhieuCanNhapNguyenLieu>(HttpContext, apiUrl);
                if (item != null)
                {
                    data = new List<object>() { item };
                    return Json(new
                    {
                        isSuccess = true,
                        Data = data,
                        Messages = "Lấy dữ liệu thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Data = new List<HQ_PhieuCanNhapNguyenLieu>(),
                        Messages = "Không tìm thấy dữ liệu phiếu cân nhập nguyên liệu!"
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Data = new List<HQ_PhieuCanNhapNguyenLieu>(),
                Messages = "Lấy dữ liệu thất bại, vui lòng thử lại!"
            });
        }


        public async Task<IActionResult> DoInsert_NLHQ(DateTime dateTime, string xuongId, string id, int stt, int idPhieuCanNguyenLieu, string soPhieuCanNhap, int idSanPham, int idQuyCach, int idKho, string idPhuongTien,string taiXe,string cccd, string sdt, int canHang, decimal trongLuong, int idDonViTinh, string noiDungGiaoNhan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/Insert";
            try
            {
                if (stt <= 0 || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(id) || idPhieuCanNguyenLieu <= 0 || string.IsNullOrEmpty(soPhieuCanNhap))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_PhieuCanNhapNguyenLieu
                {
                    Id = $@"{AppViewModels.AppViewModel.Instance.PCName}" + "." + $@"{DateTime.Now.ToString("yyyyMMddHHmmss")}",
                    STT = stt,
                    IdPhieuCanNguyenLieu = idPhieuCanNguyenLieu,
                    SoPhieuCanNhap = soPhieuCanNhap,
                    SoLanSua = 1,
                    MaKho = idKho,
                    TaiXe = taiXe,
                    CCCD = cccd,
                    SDT = sdt,
                    NoiDungGiaoNhan = noiDungGiaoNhan,
                    MaPhuongTien = idPhuongTien,
                    MaSanPham = idSanPham,
                    TrongLuongTong = 0,
                    TrongLuongHang = trongLuong,
                    TrongLuongXe = 0,
                    MaDonVi = idDonViTinh,
                    CanHang = canHang,
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

        public async Task<IActionResult> DoUpDate_NLHQ(string idPhieuCan,DateTime dateTime, string xuongId, string id, int stt, int idPhieuCanNguyenLieu, string soPhieuCanNhap, int idSanPham, int idQuyCach, int idKho, string idPhuongTien,string taiXe,string cccd, string sdt, int canHang, decimal trongLuong, int idDonViTinh, string noiDungGiaoNhan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/Update/{idPhieuCan}";
            try
            {
                if (stt <= 0 || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(id) || idPhieuCanNguyenLieu <= 0 || string.IsNullOrEmpty(soPhieuCanNhap))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new
                {
                    Id = idPhieuCan,
                    STT = stt,
                    IdPhieuCanNguyenLieu = idPhieuCanNguyenLieu,
                    SoPhieuCanNhap = soPhieuCanNhap,
                    SoLanSua = 1,
                    MaKho = idKho,
                    TaiXe = taiXe,
                    CCCD = cccd,
                    SDT = sdt,
                    NoiDungGiaoNhan = noiDungGiaoNhan,
                    MaPhuongTien = idPhuongTien,
                    MaSanPham = idSanPham,
                    TrongLuongTong = 0,
                    TrongLuongHang = trongLuong,
                    TrongLuongXe = 0,
                    MaDonVi = idDonViTinh,
                    CanHang = canHang,
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
        public async Task<IActionResult> DoDelete_NLHQ(string listInfoPhieuCan, DateTime ngay)
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
                    var model = new
                    {
                        Id = id + "XOA",
                        STT = stt * -1,
                        MayCan = AppViewModels.AppViewModel.Instance.PCName,
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
        #endregion
        #region PHIEUCANXUAT
        #endregion
        #endregion

        public async Task<IActionResult> Reload(DateTime dateTime, string xuongId, string typeValue)
        {
            IEnumerable<object> data = new List<object>();
            bool success = true;
            try
            {
                switch (typeValue)
                {
                    case "PHIEUCANNHAP":
                        data = await GetPhieuCanNhapNguyenLieuHQ(dateTime, xuongId);
                        break;
                    case "PHIEUCANXUAT":
                        data = await GetPhieuCanXuatNguyenLieuHQ(dateTime, xuongId);
                        break;
                    default:
                        break;
                }
                if (data.Count() == 0)
                {
                    success = false;
                }
                var result = new
                {
                    Success = success,
                    Messages = success ? "Lấy dữ liệu Thành Công." : "Vui lòng kiểm tra lại!.",
                    Data = data
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
