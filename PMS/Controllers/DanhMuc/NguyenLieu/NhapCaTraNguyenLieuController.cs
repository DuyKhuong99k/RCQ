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
using System;
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;
using AppViewModels;

namespace PMS.Controllers.DanhMuc.NguyenLieu
{
    [Authorize]
    public class NhapCaTraNguyenLieuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public NhapCaTraNguyenLieuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Nhập Cá Trả", Func = "Xem Nguyên Liệu / Nhập Cá Trả")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhapCaTraNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var xuongId = HttpContext.Session.GetString("XuongId");

            var apiNhaCCUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieux/GetAlls";
            using var helperNhaCC = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhaCCs = helperNhaCC.GetAsync<IEnumerable<NhaCungCapNguyenLieu>>(HttpContext, apiNhaCCUrl);
            ViewBag.listNhaCCs = nhaCCs.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaNguyenLieux/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaNguyenLieu>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongTiens = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = phuongTiens.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamNguyenLieux/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamNguyenLieu>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeNguyenLieux/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeNguyenLieu>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiMaAoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetAos/{xuongId}";
            using var helperMaAo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maAos = helperMaAo.GetAsync<IEnumerable<object>>(HttpContext, apiMaAoUrl);
            ViewBag.listMaAos = maAos.Result.ToList();

            ViewBag.TitlePage = "Nhập Cá Trả Nguyên Liệu";
            return View("~/Views/DanhMuc/NguyenLieu/NhapCaTraNguyenLieuView.cshtml");
        }

        public async Task<IEnumerable<object>> GetsCaTra(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetsCaTra/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nguyên Liệu / Nhập Cá Trả", Func = "Thêm Nguyên Liệu / Nhập Cá Trả")]
        public async Task<IActionResult> DoInsert(string nccnhapcatra, string ghenhapcatra, string mslnhapcatra, string aonhapcatra, string canhapcatra, decimal trongluongnhapcatra, string tpnhapcatra, string sizenhapcatra, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/InsertNhapCaTra";
            try
            {
                if (string.IsNullOrEmpty(nccnhapcatra) || string.IsNullOrEmpty(ghenhapcatra) || string.IsNullOrEmpty(mslnhapcatra) || string.IsNullOrEmpty(aonhapcatra) || string.IsNullOrEmpty(canhapcatra) || string.IsNullOrEmpty(trongluongnhapcatra.ToString()) || string.IsNullOrEmpty(tpnhapcatra) || string.IsNullOrEmpty(sizenhapcatra))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanNguyenLieu
                {
                    TrongLuong = trongluongnhapcatra * -1,
                    MaPhuongTien = ghenhapcatra,
                    TyLeNuoc = 0,
                    TrongLuongOrg = trongluongnhapcatra * -1,
                    MaBanCatTiet = "CT99",
                    NhaCC = nccnhapcatra,
                    MSL = mslnhapcatra,
                    MaAo = aonhapcatra,
                    MaLoaiThanhPham = tpnhapcatra,
                    MaSize = sizenhapcatra,
                    SuDung = true,
                    MaXuongSanXuat = xuongId,
                    MaMayTinhCan = AppViewModels.AppViewModel.Instance.PCName,
                    Ngay = DateTime.Now,
                    ThoiGianCan = new DateTime(
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day,
                        DateTime.Now.Hour,
                        DateTime.Now.Minute,
                        DateTime.Now.Second,
                        DateTime.Now.Millisecond),
                    MaUserCan = AppViewModel.Instance.UserName,
                    MaMau = "",
                    TrongLuongTare = 0,
                    Pheu = "0",
                    Chuyen = 1,
                    MaLoaiCa = canhapcatra,
                    GhiChu = "",
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
    }
}