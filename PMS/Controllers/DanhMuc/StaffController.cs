using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Models;
using System.Runtime.CompilerServices;
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
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;

namespace PMS.Controllers.DanhMuc
{
    [Authorize]
    public class StaffController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public StaffController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên", Func = "Xem Nhân Viên")]
        public async Task<IActionResult> NhanVien()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhanVienView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Nhân Viên";
            var apiUrlXiNghiep = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            var apiUrlGetNhom = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNhoms";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var listXuongs = helper.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiUrlXiNghiep);
            using var helper2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var listNhoms = await helper2.GetAsync2<string>(HttpContext, apiUrlGetNhom);
            //var listXuongs = GetApiAsync<XiNghiep>(apiUrlXiNghiep);
            //var listNhoms = GetApiAsync<string>(apiUrlGetNhom);
            ViewBag.listXuongs = listXuongs.Result.ToList();
            ViewBag.listNhoms = listNhoms.ToList();
            return View("~/Views/DanhMuc/Staff/NhanVienView.cshtml");
        }
        private async Task<List<T>> GetApiAsync<T>(string apiurl)
        {
            IEnumerable<object> dataSource = null;

            var apiUrl = apiurl;

            if (dataSource == null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWTToken"));
                    // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var sourceObjects = JsonConvert.DeserializeObject<IEnumerable<object>>(data);
                        if (sourceObjects != null) dataSource = sourceObjects;
                    }
                }
            }
            var jsonString = JsonConvert.SerializeObject(dataSource);
            var apiData = JsonConvert.DeserializeObject<List<T>>(jsonString)?.ToList();
            return apiData;
        }
        public async Task<IEnumerable<NhanVienDaiThanh>> GetAllNhanViens()
        {
            IEnumerable<NhanVienDaiThanh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanViens";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        //public async Task<IEnumerable<NhanVienDaiThanh>> GetMoreNhanViens(int skip)
        //{
        //    IEnumerable<NhanVienDaiThanh> dataSource = ViewBag.dataSource;
        //    if (dataSource == null)
        //    {
        //        var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetMoreNhanViens?skip={skip}";
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //        ViewBag.dataSource = await helper.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiUrl);
        //        dataSource = ViewBag.dataSource;
        //    }
        //    return dataSource;
        //}
        public async Task<IEnumerable<NhanVienDaiThanh>> GetMoreNhanViens(int page, int pageSize)
        {
            IEnumerable<NhanVienDaiThanh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetMoreNhanViens/{page}/{pageSize}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefaulNew()
        {
            var dataSource = await GetAllNhanViens();
            if (dataSource != null)
            {
                int rl;
                var maxMaNhanVien = dataSource.Where(x => int.TryParse(x.MaNhanVien, out rl))
                    .Select(x => int.Parse(x.MaNhanVien))
                    .DefaultIfEmpty(0)
                    .Max();
                var maNhanVien = $"{(maxMaNhanVien + 1).ToString("000000")}";
                var defaultNhanVien = new NhanVienDaiThanh
                {
                    MaNhanVien = maNhanVien,
                    FirstWorkingDate = @"Đang Làm",
                    IsShowDinhMuc = true,
                    IsContracting = true,
                    IsPhucVu = false,
                    IsHuman = true,
                    IsGiaCong = false,
                    IsChucNang = false,
                    LoaiSanLuong = 1,
                    IsBanKiem = false,
                    MaChamCong = null,
                    MaHoSo = null,
                    Xuong = null,
                    Name = null,
                    BirthDate = null,
                    DeptCode0 = null,
                    DeptName0 = null,
                    ChucVu = null,
                    GenderName = null,
                    Tel = null,
                    Address = null,
                    JobPositionName0 = null,
                    AC = 0,
                    IsNhanVienCat = false,
                    IsNhom = false
                    // Tiếp tục xử lý với giá trị maxMaNhanVien
                };
                return Json(new
                {
                    isSuccess = true,
                    MaNhanVien = maNhanVien,
                    MaChamCong = defaultNhanVien.MaChamCong,
                    MaHoSo = defaultNhanVien.MaHoSo,
                    Xuong = defaultNhanVien.Xuong,
                    Name = defaultNhanVien.Name,
                    BirthDate = defaultNhanVien.BirthDate,
                    DeptCode0 = defaultNhanVien.DeptCode0,
                    DeptName0 = defaultNhanVien.DeptName0,
                    ChucVu = defaultNhanVien.ChucVu,
                    GenderName = defaultNhanVien.GenderName,
                    Tel = defaultNhanVien.Tel,
                    Address = defaultNhanVien.Address,
                    JobPositionName0 = defaultNhanVien.JobPositionName0,
                    FirstWorkingDate = defaultNhanVien.FirstWorkingDate,
                    IsShowDinhMuc = defaultNhanVien.IsShowDinhMuc,
                    IsContracting = defaultNhanVien.IsContracting,
                    IsPhucVu = defaultNhanVien.IsPhucVu,
                    IsHuman = defaultNhanVien.IsHuman,
                    IsGiaCong = defaultNhanVien.IsGiaCong,
                    AC = defaultNhanVien.AC,
                    IsChucNang = defaultNhanVien.IsChucNang,
                    LoaiSanLuong = defaultNhanVien.LoaiSanLuong,
                    IsBanKiem = defaultNhanVien.IsBanKiem,
                    IsNhanVienCat = defaultNhanVien.IsNhanVienCat,
                    IsNhom = defaultNhanVien.IsNhom
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
        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên", Func = "Thêm Nhân Viên")]
        public async Task<IActionResult> DoInsert(string maNhanVien, string maHoSo, string tenNhanVien, int loaiSL, string nhomName, string xuongId, bool isContracting, bool isHuman, bool isPhucVu, bool isGiaCong, bool isBanKiem, bool isChucNang, bool isNhom, bool isShowDinhMuc)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/InsertNhanVien";
            var accessToken = HttpContext.Session.GetString("JWTToken");
            try
            {
                if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maHoSo) || string.IsNullOrEmpty(tenNhanVien))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var nhanVienModel = new NhanVienDaiThanh
                {
                    MaNhanVien = maNhanVien,
                    MaHoSo = maHoSo,
                    Name = tenNhanVien,
                    LoaiSanLuong = loaiSL,
                    DeptName0 = nhomName,
                    Xuong = xuongId,
                    IsContracting = isContracting,
                    IsHuman = isHuman,
                    IsPhucVu = isPhucVu,
                    IsGiaCong = isGiaCong,
                    IsBanKiem = isBanKiem,
                    IsChucNang = isChucNang,
                    IsNhom = isNhom,
                    IsShowDinhMuc = isShowDinhMuc,
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(nhanVienModel), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (response.Success)
                {
                    if (response.Success)
                    {
                        // Đăng ký thành công
                        return Json(new
                        {
                            isSuccess = response.Success,
                            Messages = response.Message,
                        });
                    }
                }
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Đã xảy ra lỗi khi gọi API thêm nhân viên. Vui lòng thử lại sau."
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

        public async Task<IActionResult> GetInfoNhanVienByMaNhanVien(string maNhanVien)
        {
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn nhân viên!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNhanVienByMaNhanVien/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<NhanVienDaiThanh>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaNhanVien = item?.MaNhanVien,
                        MaHoSo = item?.MaHoSo,
                        Name = item?.Name,
                        LoaiSanLuong = item?.LoaiSanLuong,
                        DeptName0 = item?.DeptName0,
                        Xuong = item?.Xuong,
                        IsContracting = item?.IsContracting,
                        IsHuman = item?.IsHuman,
                        IsPhucVu = item?.IsPhucVu,
                        IsGiaCong = item?.IsGiaCong,
                        IsBanKiem = item?.IsBanKiem,
                        IsChucNang = item?.IsChucNang,
                        IsNhom = item?.IsNhom,
                        IsShowDinhMuc = item?.IsShowDinhMuc,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> GetInfoNhanVienByMaNhanVien2(string maNhanVien)
        {
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn nhân viên!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNhanVienByMaNhanVien/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<NhanVienDaiThanh>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Data = new
                        {
                            MaNhanVien = item?.MaNhanVien,
                            MaHoSo = item?.MaHoSo,
                            Name = item?.Name,
                            LoaiSanLuong = item?.LoaiSanLuong,
                            DeptName0 = item?.DeptName0,
                            Xuong = item?.Xuong,
                            IsContracting = item?.IsContracting,
                            IsHuman = item?.IsHuman,
                            IsPhucVu = item?.IsPhucVu,
                            IsGiaCong = item?.IsGiaCong,
                            IsBanKiem = item?.IsBanKiem,
                            IsChucNang = item?.IsChucNang,
                            IsNhom = item?.IsNhom,
                            IsShowDinhMuc = item?.IsShowDinhMuc,
                        }
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> GetInfoNhanVienByMaHoSo(string maHoSo)
        {
            if (string.IsNullOrEmpty(maHoSo))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn nhân viên!."
                });
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNhanVienByMaHoSo/{maHoSo}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<NhanVienDaiThanh>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaNhanVien = item?.MaNhanVien,
                        MaHoSo = item?.MaHoSo,
                        Name = item?.Name,
                        LoaiSanLuong = item?.LoaiSanLuong,
                        DeptName0 = item?.DeptName0,
                        Xuong = item?.Xuong,
                        IsContracting = item?.IsContracting,
                        IsHuman = item?.IsHuman,
                        IsPhucVu = item?.IsPhucVu,
                        IsGiaCong = item?.IsGiaCong,
                        IsBanKiem = item?.IsBanKiem,
                        IsChucNang = item?.IsChucNang,
                        IsNhom = item?.IsNhom,
                        IsShowDinhMuc = item?.IsShowDinhMuc,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên", Func = "Sửa Nhân Viên")]
        public async Task<IActionResult> DoUpDate(string maNhanVien, string maHoSo, string tenNhanVien, int loaiSL, string nhomName, string xuongId, bool isContracting, bool isHuman, bool isPhucVu, bool isGiaCong, bool isBanKiem, bool isChucNang, bool isNhom, bool isShowDinhMuc)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/UpdateNhanVien/{maNhanVien}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maHoSo) || string.IsNullOrEmpty(tenNhanVien) || string.IsNullOrEmpty(xuongId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var updateModel = new NhanVienDaiThanh
                {
                    MaNhanVien = maNhanVien,
                    MaHoSo = maHoSo,
                    Name = tenNhanVien,
                    LoaiSanLuong = loaiSL,
                    DeptName0 = nhomName,
                    Xuong = xuongId,
                    IsContracting = isContracting,
                    IsHuman = isHuman,
                    IsPhucVu = isPhucVu,
                    IsGiaCong = isGiaCong,
                    IsBanKiem = isBanKiem,
                    IsChucNang = isChucNang,
                    IsNhom = isNhom,
                    IsShowDinhMuc = isShowDinhMuc,
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(updateModel), Encoding.UTF8, "application/json");
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
                    Messages = "Đã xảy ra lỗi khi gọi API cập nhật. Vui lòng thử lại sau."
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
        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên", Func = "Xoá Nhân Viên")]
        public async Task<IActionResult> DeleteNhanVien(string maNhanVien)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/DeleteNhanVien/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);

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
                    Messages = "Đã xảy ra lỗi khi gọi API xóa người dùng. Vui lòng thử lại sau."
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

        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên / Trạng Thái Nhân Viên Tạm Thời", Func = "Xem / Nhân Viên / Trạng Thái Nhân Viên Tạm Thời")]
        public async Task<IActionResult> NhanVienTrangThaiTamThoi()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhanVienTrangThaiTamThoiView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Nhân Viên - Trạng Thái Tạm Thời";
            var apiUrlXiNghiep = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            var apiUrlGetNhom = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNhoms";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var listXuongs = helper.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiUrlXiNghiep);
            using var helper2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var listNhoms = await helper2.GetAsync2<string>(HttpContext, apiUrlGetNhom);
            //var listXuongs = GetApiAsync<XiNghiep>(apiUrlXiNghiep);
            //var listNhoms = GetApiAsync<string>(apiUrlGetNhom);
            ViewBag.listXuongs = listXuongs.Result.ToList();
            ViewBag.listNhoms = listNhoms.ToList();
            return View("~/Views/DanhMuc/Staff/NhanVienTrangThaiTamThoiView.cshtml");
        }
        public async Task<IEnumerable<object>> GetDanhSachNhanVienTrangThaiTamThois(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            var accsessToken = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TrangThaiNhanVienTamThois/GetDanhSachNhanVienTrangThaiTamThois/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

    }
}
