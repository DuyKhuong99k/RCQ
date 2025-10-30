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

namespace PMS.Controllers.DanhMuc.NguyenLieu
{
    [Authorize]
    public class NhapDauAoNguyenLieuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public NhapDauAoNguyenLieuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Nhập Đầu Ao", Func = "Xem Nguyên Liệu / Nhập Đầu Ao")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhapDauAoNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var xuongId = HttpContext.Session.GetString("XuongId");

            var apiNhaCCUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieux/GetAlls";
            using var helperNhaCC = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhaCCs = helperNhaCC.GetAsync<IEnumerable<NhaCungCapNguyenLieu>>(HttpContext, apiNhaCCUrl);
            ViewBag.listNhaCCs = nhaCCs.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongTiens = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = phuongTiens.Result.ToList();

            var apiMaAoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetAos/{xuongId}";
            using var helperMaAo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maAos = helperMaAo.GetAsync<IEnumerable<object>>(HttpContext, apiMaAoUrl);
            ViewBag.listMaAos = maAos.Result.ToList();

            var apiThuKyUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/GetThuKys";
            using var helperThuKy = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thukys = helperThuKy.GetAsync<IEnumerable<object>>(HttpContext, apiThuKyUrl);
            ViewBag.listThuKy = thukys.Result.ToList();

            var apiApTaiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/GetApTais";
            using var helperApTai = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var aptais = helperApTai.GetAsync<IEnumerable<object>>(HttpContext, apiApTaiUrl);
            ViewBag.listApTai = aptais.Result.ToList();

            var apiTaisUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/GetTais";
            using var helperTai = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tais = helperTai.GetAsync<IEnumerable<object>>(HttpContext, apiTaisUrl);
            ViewBag.listTai = tais.Result.ToList();

            ViewBag.TitlePage = "Nhập Đầu Ao Nguyên Liệu";
            return View("~/Views/DanhMuc/NguyenLieu/NhapDauAoNguyenLieuView.cshtml");
        }
        public async Task<IEnumerable<object>> GetsNgayBatCas(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/GetsNgayBatCas/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetsNgayNhapXuong(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/GetsNgayNhapXuong/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Nhập Đầu Ao", Func = "Thêm Nguyên Liệu / Nhập Đầu Ao")]
        public async Task<IActionResult> DoInsert(
            DateTime ngayNX,
            DateTime ngayXP,
            DateTime ngayBC,
            string ao,
            TimeSpan gioXP,
            string thuky,
            string phuongtien,
            string aptai,
            int tai,
            int chuyen,
            string ncc,
            decimal cutruoc,
            decimal camanh,
            decimal ngopmuoi,
            decimal ngopngoai,
            decimal ngopxe,
            decimal cusau,
            decimal tylemoi)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/InsertNhapDauAo";
            try
            {

                var userName = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(thuky) || string.IsNullOrEmpty(ao) || string.IsNullOrEmpty(phuongtien) || string.IsNullOrEmpty(aptai) || string.IsNullOrEmpty(tai.ToString()) || string.IsNullOrEmpty(chuyen.ToString()) || string.IsNullOrEmpty(ncc) || string.IsNullOrEmpty(cutruoc.ToString()) || string.IsNullOrEmpty(camanh.ToString()) || string.IsNullOrEmpty(ngopmuoi.ToString()) || string.IsNullOrEmpty(ngopngoai.ToString()) || string.IsNullOrEmpty(ngopxe.ToString()) || string.IsNullOrEmpty(cusau.ToString()) || string.IsNullOrEmpty(tylemoi.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhuongTien_TrongLuongDauAo
                {
                    MaPhuongTien = phuongtien,
                    Ngay = ngayNX,
                    MaAo = ao,
                    CaManh = camanh,
                    CaNgopAoGhe = ngopmuoi,
                    CaNgopAoXe = ngopxe,
                    TongHam = camanh + ngopmuoi + ngopxe,
                    CaNgayTruoc = cutruoc,
                    CaConLai = cusau,
                    ThuKy = thuky,
                    ApTai = aptai,
                    STTChuyen = chuyen,
                    TyLeMoi = tylemoi,
                    GhiChu = "",
                    GioXuatPhat = gioXP,
                    NgayXuatPhat = ngayXP,
                    NgayBatCa = ngayBC,
                    CreateDateTime = DateTime.Now,
                    CreateBy = userName,
                    ModifiedDateTime = DateTime.Now,
                    ModifiedBy = userName,
                    IsVungNuoiBlocked = true,
                    MaNhaCungCap = ncc,
                    CaNgopAoBanNgoai = ngopngoai,
                    Chuyen = chuyen
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
        public async Task<IActionResult> GetsByMa(string phuongTien, DateTime ngay, string maAo, string maNhaCC, int chuyen)
        {
            if (string.IsNullOrEmpty(phuongTien) || string.IsNullOrEmpty(maAo) || string.IsNullOrEmpty(maNhaCC))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn thông tin!."
                });
            }

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/GetsByMa2/{phuongTien}/{ngay.ToString("yyyy-MM-dd")}/{maAo}/{maNhaCC}/{chuyen}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var item = await helper.GetAsync<PhuongTien_TrongLuongDauAo>(HttpContext, apiUrl);
            if (item != null)
            {
                return Json(new
                {
                    isSuccess = true,
                    Mesages = "Thành Công",
                    MaPhuongTien = item?.MaPhuongTien,
                    Ngay = item?.Ngay,
                    MaAo = item?.MaAo,
                    CaManh = item?.CaManh,
                    CaNgopAoGhe = item?.CaNgopAoGhe,
                    CaNgopAoXe = item?.CaNgopAoXe,
                    TongHam = item?.TongHam,
                    CaNgayTruoc = item?.CaNgayTruoc,
                    CaConLai = item?.CaConLai,
                    ThuKy = item?.ThuKy,
                    ApTai = item?.ApTai,
                    STTChuyen = item?.STTChuyen,
                    TyLeMoi = item?.TyLeMoi,
                    GhiChu = item?.GhiChu,
                    NgayXuatPhat = item?.NgayXuatPhat,
                    GioXuatPhat = item?.GioXuatPhat,
                    NgayBatCa = item?.NgayBatCa,
                    CreateDateTime = item?.CreateDateTime,
                    CreateBy = item?.CreateBy,
                    ModifiedDateTime = item?.ModifiedDateTime,
                    ModifiedBy = item?.ModifiedBy,
                    IsVungNuoiBlocked = item?.IsVungNuoiBlocked,
                    MaNhaCungCap = item?.MaNhaCungCap,
                    CaNgopAoBanNgoai = item?.CaNgopAoBanNgoai,
                    Chuyen = item?.Chuyen
                });
            }

            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Nhập Đầu Ao", Func = "Sửa Nguyên Liệu / Nhập Đầu Ao")]
        public async Task<IActionResult> DoUpDate(
            string phuongtien,
            DateTime ngay,
            string maAo,
            string maNhaCC,
            int chuyen,
            DateTime ngayXP,
            DateTime ngayBC,
            TimeSpan gioXP,
            string thuky,           
            string aptai,
            int tai,
            decimal cutruoc,
            decimal camanh,
            decimal ngopmuoi,
            decimal ngopngoai,
            decimal ngopxe,
            decimal cusau,
            decimal tylemoi)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTien_TrongLuongDauAo/Update/{phuongtien}/{ngay.ToString("yyyy-MM-dd")}/{maAo}/{maNhaCC}/{chuyen}";
            try
            {
                var userName = HttpContext.Session.GetString("Username");
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(chuyen.ToString()) || string.IsNullOrEmpty(phuongtien))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhuongTien_TrongLuongDauAo
                {
                    //MaPhuongTien = phuongtien,
                    //Ngay = ngayNX,
                    //MaAo = ao,
                    CaManh = camanh,
                    CaNgopAoGhe = ngopmuoi,
                    CaNgopAoXe = ngopxe,
                    TongHam = camanh + ngopmuoi + ngopxe,
                    CaNgayTruoc = cutruoc,
                    CaConLai = cusau,
                    ThuKy = thuky,
                    ApTai = aptai,
                    STTChuyen = chuyen,
                    TyLeMoi = tylemoi,
                    GhiChu = "",
                    GioXuatPhat = gioXP,
                    NgayXuatPhat = ngayXP,
                    NgayBatCa = ngayBC,
                    ModifiedDateTime = DateTime.Now,
                    ModifiedBy = userName,
                    IsVungNuoiBlocked = true,
                    //MaNhaCungCap = ncc,
                    CaNgopAoBanNgoai = ngopngoai,
                    //Chuyen = chuyen
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
        public async Task<ActionResult> LoadBaoCao(DateTime dateTime, string loaiNgay)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;
            if (loaiNgay == "ngaybatca")
            {
                dataSource = await GetsNgayBatCas(dateTime);
            }
            else if (loaiNgay == "ngaynhapxuong")
            {
                dataSource = await GetsNgayNhapXuong(dateTime);
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
