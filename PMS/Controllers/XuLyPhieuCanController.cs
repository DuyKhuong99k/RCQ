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
    public class XuLyPhieuCanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public XuLyPhieuCanController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        #region Khai Báo View
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tôm", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Tôm")]
        public IActionResult XuLyPhieuCanTomView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanTomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu")]
        public IActionResult XuLyPhieuCanNguyenLieuView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanNguyenLieuView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Nguyên Liệu";
            var xuongId = HttpContext.Session.GetString("XuongId");

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaNguyenLieux/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaNguyenLieu>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiNhaCCUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaCungCapNguyenLieux/GetAlls";
            using var helperNhaCC = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhaCCs = helperNhaCC.GetAsync<IEnumerable<NhaCungCapNguyenLieu>>(HttpContext, apiNhaCCUrl);
            ViewBag.listNhaCCs = nhaCCs.Result.ToList();

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

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongTiens = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = phuongTiens.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauNguyenLieux/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauNguyenLieu>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiMaAoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetAos/{xuongId}";
            using var helperMaAo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maAos = helperMaAo.GetAsync<IEnumerable<object>>(HttpContext, apiMaAoUrl);
            ViewBag.listMaAos = maAos.Result.ToList();


            var apiBanCatTietUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanCatTiet/GetAllBanCatTiets";
            using var helperBanCatTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var banCatTiets = helperBanCatTiet.GetAsync<IEnumerable<BanCatTiet>>(HttpContext, apiBanCatTietUrl);
            ViewBag.BanCatTiets = banCatTiets.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();
            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn")]
        public IActionResult XuLyPhieuCanPhuXepKhuonView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanPhuXepKhuonView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Phụ Xếp Khuôn";

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaXepKhuons/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaXepKhuon>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamXepKhuons/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamXepKhuon>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeXepKhuons/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeXepKhuon>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauXepKhuons/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauXepKhuon>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiKhuVucUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaKhuVucXepKhuons/GetAlls";
            using var helperKhuVuc = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khuVucs = helperKhuVuc.GetAsync<IEnumerable<MaKhuVucXepKhuon>>(HttpContext, apiKhuVucUrl);
            ViewBag.listKhuVucs = khuVucs.Result.ToList();


            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn")]
        public IActionResult XuLyPhieuCanChinhXepKhuonView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanChinhXepKhuonView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Chính Xếp Khuôn";
            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaXepKhuons/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaXepKhuon>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamChinhXepKhuons/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamChinhXepKhuon>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiChieuXaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaChieuXaXepKhuons/GetAlls";
            using var helperChieuXa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var chieuxas = helperChieuXa.GetAsync<IEnumerable<MaChieuXaXepKhuon>>(HttpContext, apiChieuXaUrl);
            ViewBag.listChieuXas = chieuxas.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeChinhXepKhuons/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeChinhXepKhuon>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauXepKhuons/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauXepKhuon>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiChatLuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaChatLuongXepKhuons/GetAlls";
            using var helperChatLuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var chatluongs = helperChatLuong.GetAsync<IEnumerable<MaChatLuongXepKhuon>>(HttpContext, apiChatLuongUrl);
            ViewBag.listChatLuongs = chatluongs.Result.ToList();

            var apiKhuVucUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaKhuVucXepKhuons/GetAlls";
            using var helperKhuVuc = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khuVucs = helperKhuVuc.GetAsync<IEnumerable<MaKhuVucXepKhuon>>(HttpContext, apiKhuVucUrl);
            ViewBag.listKhuVucs = khuVucs.Result.ToList();

            var apiCoiTamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAllCoiTams";
            using var helperCoiTam = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var coitams = helperCoiTam.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiTamUrl);
            ViewBag.listCoiTams = coitams.Result.ToList();

            var apiCoiChinhUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAllCoiChinhs";
            using var helperCoiChinh = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var coiChinhs = helperCoiChinh.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiChinhUrl);
            ViewBag.listCoiChinhs = coiChinhs.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiNhanVienPVUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienPhucVuWithDataNeededs";
            using var helperNhanVienPV = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanvienPVs = helperNhanVienPV.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienPVUrl);
            ViewBag.DataNhanVienPV = nhanvienPVs.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn")]
        public IActionResult XuLyPhieuCanSauXepKhuonView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanChinhXepKhuonView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Chính Xếp Khuôn";
            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamChinhXepKhuons/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamChinhXepKhuon>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();


            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeChinhXepKhuons/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeChinhXepKhuon>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();


            var apiCoiChinhUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAllCoiChinhs";
            using var helperCoiChinh = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var coiChinhs = helperCoiChinh.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiChinhUrl);
            ViewBag.listCoiChinhs = coiChinhs.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaXepKhuons/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaXepKhuon>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiChieuXaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaChieuXaXepKhuons/GetAlls";
            using var helperChieuXa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var chieuxas = helperChieuXa.GetAsync<IEnumerable<MaChieuXaXepKhuon>>(HttpContext, apiChieuXaUrl);
            ViewBag.listChieuXas = chieuxas.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn")]
        public IActionResult XuLyPhieuCanKXLXepKhuonView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanKXLXepKhuonView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân KXL Xếp Khuôn";
            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaXepKhuons/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaXepKhuon>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamXepKhuonKHCs/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamXepKhuonKHC>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeXepKhuonKHCs/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeXepKhuonKHC>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiMaKhachHangUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaKhachHangXepKhuons/GetAlls";
            using var helperMaKhachHang = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khachhangs = helperMaKhachHang.GetAsync<IEnumerable<MaKhachHangXepKhuon>>(HttpContext, apiMaKhachHangUrl);
            ViewBag.listMaKhachHangs = khachhangs.Result.ToList();


            var apiCoiTamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAllCoiTams";
            using var helperCoiTam = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var coitams = helperCoiTam.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiTamUrl);
            ViewBag.listCoiTams = coitams.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn")]
        public IActionResult XuLyPhieuCanBlockXepKhuonView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanBlockXepKhuonView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Block Xếp Khuôn";

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamXepKhuonBlocks/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamXepKhuonBlock>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeXepKhuonBlocks/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeXepKhuonKHC>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiChatLuongLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaChatLuongXepKhuonBlocks/GetAlls";
            using var helperChatLuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var chatluongs = helperChatLuong.GetAsync<IEnumerable<MaChatLuongXepKhuonBlock>>(HttpContext, apiChatLuongLUrl);
            ViewBag.listChatLuongs = chatluongs.Result.ToList();

            var apiNetLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaNetXepKhuons/GetAlls";
            using var helperNet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nets = helperNet.GetAsync<IEnumerable<MaNetXepKhuon>>(HttpContext, apiNetLUrl);
            ViewBag.listNets = nets.Result.ToList();

            var apiChieuXaLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaChieuXaXepKhuons/GetAlls";
            using var helperChieuXa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var ChieuXas = helperChieuXa.GetAsync<IEnumerable<MaChieuXaXepKhuon>>(HttpContext, apiChieuXaLUrl);
            ViewBag.listChieuXas = ChieuXas.Result.ToList();

            var apiMaKhachHangUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaKhachHangXepKhuons/GetAlls";
            using var helperMaKhachHang = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khachhangs = helperMaKhachHang.GetAsync<IEnumerable<MaKhachHangXepKhuon>>(HttpContext, apiMaKhachHangUrl);
            ViewBag.listMaKhachHangs = khachhangs.Result.ToList();

            var apiMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauXepKhuonBlocks/GetAlls";
            using var helperMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var Maus = helperMau.GetAsync<IEnumerable<MaMauXepKhuonBlock>>(HttpContext, apiMauUrl);
            ViewBag.listMaus = Maus.Result.ToList();

            var apiCongDoanUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCongDoanXepKhuons/GetAlls";
            using var helperCongDoan = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var CongDoans = helperCongDoan.GetAsync<IEnumerable<MaCongDoanXepKhuon>>(HttpContext, apiCongDoanUrl);
            ViewBag.listCongDoans = CongDoans.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();


            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Bao Tử")]
        public IActionResult XuLyPhieuCanBaoTuView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanBaoTuView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Bao Tử";

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_MaLoaiCa/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<BT_MaLoaiCa>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_MaThanhPham/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<BT_MaThanhPham>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiKhachHangUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_KhachHang/GetAlls";
            using var helperKhachHang = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var KhachHangs = helperKhachHang.GetAsync<IEnumerable<BT_KhachHang>>(HttpContext, apiKhachHangUrl);
            ViewBag.listKhachHangs = KhachHangs.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Tái Chế")]
        public IActionResult XuLyPhieuCanTaiCheView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanTaiCheView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Tái Chế";

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeTaiChes/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeTaiChe>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiChatLuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaChatLuongTaiChes/GetAlls";
            using var helperChatLuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var ChatLuongs = helperChatLuong.GetAsync<IEnumerable<MaChatLuongTaiChe>>(HttpContext, apiChatLuongUrl);
            ViewBag.listChatLuongs = ChatLuongs.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaTaiChes/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaTaiChe>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauTaiChes/GetAlls";
            using var helperMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var Maus = helperMau.GetAsync<IEnumerable<MaMauTaiChe>>(HttpContext, apiMauUrl);
            ViewBag.listMaus = Maus.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamTaiChes/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamTaiChe>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiCongViecUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCongViecTaiChes/GetAlls";
            using var helperCongViec = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var CongViecs = helperCongViec.GetAsync<IEnumerable<MaCongViecTaiChe>>(HttpContext, apiCongViecUrl);
            ViewBag.listCongViecs = CongViecs.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia")]
        public IActionResult XuLyPhieuCanPhuGiaView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanPhuGiaView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Phụ Gia";


            var apiSanPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_SanPham/GetAlls";
            using var helperSanPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var SanPhams = helperSanPham.GetAsync<IEnumerable<PD_SanPham>>(HttpContext, apiSanPhamUrl);
            ViewBag.listSanPhams = SanPhams.Result.ToList();

            var apiCoiChinhUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaCoiXepKhuons/GetAllCoiChinhs";
            using var helperCoiChinh = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var coiChinhs = helperCoiChinh.GetAsync<IEnumerable<MaCoiXepKhuon>>(HttpContext, apiCoiChinhUrl);
            ViewBag.listCoiChinhs = coiChinhs.Result.ToList();

            var apiCongthucUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_Congthuc/GetAlls";
            using var helperCongthuc = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var Congthucs = helperCongthuc.GetAsync<IEnumerable<PD_CongThuc>>(HttpContext, apiCongthucUrl);
            ViewBag.listCongthucs = Congthucs.Result.ToList();

            var apiLyDoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_LyDo/GetAlls";
            using var helperLyDo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var LyDos = helperLyDo.GetAsync<IEnumerable<PD_LyDo>>(HttpContext, apiLyDoUrl);
            ViewBag.listLyDos = LyDos.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm")]
        public IActionResult XuLyPhieuCanPhuPhamView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanPhuPhamView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Phụ Phẩm";

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaPhuPhams/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaPhuPham>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiNhaMuaHangUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaMuaPhuPhams/GetAlls";
            using var helperNhaMuaHang = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var NhaMuaHangs = helperNhaMuaHang.GetAsync<IEnumerable<NhaMuaPhuPham>>(HttpContext, apiNhaMuaHangUrl);
            ViewBag.listNhaMuaHangs = NhaMuaHangs.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamPhuPhams/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamPhuPham>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizePhuPhams/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizePhuPham>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoPhuPhams/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var PhuongTiens = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoPhuPham>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = PhuongTiens.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauPhuPhams/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauPhuPham>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2")]
        public IActionResult XuLyPhieuCanPhuPhamv2View()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanPhuPhamv2View");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Phụ Phẩm v2";

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaPhuPhams/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaPhuPham>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var apiNhaMuaHangUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhaMuaPhuPhams/GetAlls";
            using var helperNhaMuaHang = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var NhaMuaHangs = helperNhaMuaHang.GetAsync<IEnumerable<NhaMuaPhuPham>>(HttpContext, apiNhaMuaHangUrl);
            ViewBag.listNhaMuaHangs = NhaMuaHangs.Result.ToList();

            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamPhuPhams/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamPhuPham>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizePhuPhams/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizePhuPham>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoPhuPhams/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var PhuongTiens = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoPhuPham>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = PhuongTiens.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauPhuPhams/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauPhuPham>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Lạng Da", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Lạng Da")]
        public IActionResult XuLyPhieuCanLangDaView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanLangDaView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Lạng Da";
            var apiPhieuCanDHUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperPhieuCanDH = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phieucandinhhinhs = helperPhieuCanDH.GetAsync<IEnumerable<object>>(HttpContext, apiPhieuCanDHUrl);
            ViewBag.datasourcePhieuCanDH = phieucandinhhinhs.Result.ToList();


            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaDinhHinhs/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaDinhHinh>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeDinhHinhs/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeDinhHinh>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauDinhHinhs/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauDinhHinh>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiMayLangDaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayLangDas/GetAlls";
            using var helperMayLangDa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maylangdas = helperMayLangDa.GetAsync<IEnumerable<MayLangDa>>(HttpContext, apiMayLangDaUrl);
            ViewBag.listMayLangDas = maylangdas.Result.ToList();


            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Vùng Nuôi Đại Thành Side", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Vùng Nuôi Đại Thành Side")]
        public IActionResult XuLyPhieuCanVungNuoiDaiThanhSideView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanVungNuoiDaiThanhSideView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Vùng Nuôi Đại Thành Side";

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaVungNuois/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaicas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaVungNuoi>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaicas.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongtien = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = phuongtien.Result.ToList();
            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Cá Giống Đại Thành Side", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Cá Giống Đại Thành Side")]
        public IActionResult XuLyPhieuCanCaGiongDaiThanhSideView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanCaGiongDaiThanhSideView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Cá Giống Đại Thành Side";

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaVungNuois/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaicas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaVungNuoi>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaicas.Result.ToList();

            var apiPhuongTienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhuongTienChoNguyenLieux/GetAlls";
            using var helperPhuongTien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phuongtien = helperPhuongTien.GetAsync<IEnumerable<PhuongTienChoNguyenLieu>>(HttpContext, apiPhuongTienUrl);
            ViewBag.listPhuongTiens = phuongtien.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Cá Chết Đại Thành Side", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Cá Chết Đại Thành Side")]
        public IActionResult XuLyPhieuCanCaChetDaiThanhSideView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanCaChetDaiThanhSideView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Cá Chết Đại Thành Side";
            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Định Hình", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Định Hình")]
        public IActionResult XuLyPhieuCanDinhHinhView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanDinhHinhView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Định Hình";
            var apiPhieuCanDHUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperPhieuCanDH = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phieucandinhhinhs = helperPhieuCanDH.GetAsync<IEnumerable<object>>(HttpContext, apiPhieuCanDHUrl);
            ViewBag.datasourcePhieuCanDH = phieucandinhhinhs.Result.ToList();


            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaDinhHinhs/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaDinhHinh>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var codeId = HttpContext.Session.GetString("XuongId");
            //var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}"; //Nam Việt TP theo xưởng
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();


            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeDinhHinhs/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeDinhHinh>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauDinhHinhs/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauDinhHinh>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiMayLangDaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayLangDas/GetAlls";
            using var helperMayLangDa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maylangdas = helperMayLangDa.GetAsync<IEnumerable<MayLangDa>>(HttpContext, apiMayLangDaUrl);
            ViewBag.listMayLangDas = maylangdas.Result.ToList();


            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public IActionResult XuLyPhieuCanBTPFilletView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanBTPFilletView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân BTP Fillet";

            var apiPhieuCanDHUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperPhieuCanDH = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phieucandinhhinhs = helperPhieuCanDH.GetAsync<IEnumerable<object>>(HttpContext, apiPhieuCanDHUrl);
            ViewBag.datasourcePhieuCanDH = phieucandinhhinhs.Result.ToList();


            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAllsByCodeId/{codeId}";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeFillets/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeFillet>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();


            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiNhanVienPVUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienPhucVuWithDataNeededs";
            using var helperNhanVienPV = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanvienPVs = helperNhanVienPV.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienPVUrl);
            ViewBag.DataNhanVienPV = nhanvienPVs.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Fillet", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Fillet")]
        public IActionResult XuLyPhieuCanTPFilletView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanTPFilletView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân TP Fillet";

            var apiPhieuCanDHUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperPhieuCanDH = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var phieucandinhhinhs = helperPhieuCanDH.GetAsync<IEnumerable<object>>(HttpContext, apiPhieuCanDHUrl);
            ViewBag.datasourcePhieuCanDH = phieucandinhhinhs.Result.ToList();


            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaFillets/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaFillet>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAllsByCodeId/{codeId}";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeFillets/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeFillet>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();

            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauFillets/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<MaMauFillet>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiNhanVienPVUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienPhucVuWithDataNeededs";
            using var helperNhanVienPV = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanvienPVs = helperNhanVienPV.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienPVUrl);
            ViewBag.DataNhanVienPV = nhanvienPVs.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            var apiBanFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanFillets/GetAlls";
            using var helperBanFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var banfls = helperBanFL.GetAsync<IEnumerable<BanFillet>>(HttpContext, apiBanFLUrl);
            ViewBag.listBanFLs = banfls.Result.ToList();
            return View();
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình", Func = "Xem Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình")]
        public IActionResult XuLyPhieuCanSoCheDinhHinhView()
        {

            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "XuLyPhieuCanSoCheDinhHinhView");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.titile = "Xử Lý Phiếu Cân Sơ Chế Định Hình";

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaSoCheDinhHinhs/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<MaLoaiCaSoCheDinhHinh>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();

            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamSoCheDinhHinhs/GetAlls";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamSoCheDinhHinh>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();

            var apiMayLangDaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MayLangDas/GetAlls";
            using var helperMayLangDa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maylangdas = helperMayLangDa.GetAsync<IEnumerable<MayLangDa>>(HttpContext, apiMayLangDaUrl);
            ViewBag.listMayLangDas = maylangdas.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();

            return View();
        }
        #endregion
        #region get data with API
        public async Task<IEnumerable<object>> GetAlls(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetAllWithFullField/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanDinhHinhs_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetPhieuCanDinhHinh_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanBTPFillets_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetPhieuCanBTPFillet_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanTPFillets_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetPhieuCanTPFillet_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanTPDinhHinh>> GetAllsWithDateAndXuongDinhHinh(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanTPDinhHinh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanTPDinhHinh>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanBTPFilletv2>> GetAllsWithDateAndXuongFillet(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanBTPFilletv2> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanBTPFilletv2>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<NhanVienDaiThanh>> GetNhanVienPhucVu_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<NhanVienDaiThanh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienPhucVuWithDataNeededs";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiUrl);
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
        public async Task<IEnumerable<object>> GetPhieuCanVungNuoiDaiThanhSide_XLPC(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanCaGiongVungNuoiDaiThanhSide_XLPC(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaGiongVungNuoiDaiThanhSides/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanCaChetDaiThanhSide_XLPC(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaChetDaiThanhSides/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanNguyenLieu_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanPhuXepKhuon_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanPhuXepKhuon>> GetAllsWithDateAndXuongPhuXepKhuon(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanPhuXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanPhuXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanChinhXepKhuon_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanPhuXepKhuon>> GetAllsWithDateAndXuongChinhXepKhuon(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanPhuXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanPhuXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanXepKhuonKHC_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanXepKhuonKHC>> GetAllsWithDateAndXuongXepKhuonKHC(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanXepKhuonKHC> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanXepKhuonKHC>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanXepKhuonBlock_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanXepKhuonBlock>> GetAllsWithDateAndXuongXepKhuonBlock(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanXepKhuonBlock> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanXepKhuonBlock>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanPhuGia_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PD_PhieuCan>> GetAllsWithDateAndXuongPhuGia(DateTime dateTime, string xuongId)
        {
            IEnumerable<PD_PhieuCan> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PD_PhieuCan>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanTaiChe_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanTaiChe>> GetAllsWithDateAndXuongTaiChe(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanTaiChe> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanTaiChe>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanBaoTu_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<BT_PhieuCan>> GetAllsWithDateAndXuongBaoTu(DateTime dateTime, string xuongId)
        {
            IEnumerable<BT_PhieuCan> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<BT_PhieuCan>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanSoCheDinhHinh_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanSoCheDinhHinh>> GetAllsWithDateAndXuongSoCheDinhHinh(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanSoCheDinhHinh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanSoCheDinhHinh>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanSauXepKhuon_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanSauXepKhuon>> GetAllsWithDateAndXuongSauXepKhuon(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanSauXepKhuon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanSauXepKhuon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanPhuPham_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanPhuPhamv2_XLPC(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/GetPhieuCan_XLPC/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<PhieuCanPhuPhamv2>> GetAllsWithDateAndXuongPhuPhamv2(DateTime dateTime, string xuongId)
        {
            IEnumerable<PhieuCanPhuPhamv2> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/GetAllsWithDateAndXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanPhuPhamv2>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        #endregion
        #region xử lý các hàm
        public async Task<ActionResult> Reload(DateTime dateTime, string xuongId, string typeValue)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;

            try
            {
                switch (typeValue)
                {
                    case "VUNGNUOIDAITHANHSIDE":
                        dataSource = await GetPhieuCanVungNuoiDaiThanhSide_XLPC(dateTime);
                        break;
                    case "CAGIONGVUNGNUOIDAITHANHSIDE":
                        dataSource = await GetPhieuCanCaGiongVungNuoiDaiThanhSide_XLPC(dateTime);
                        break;
                    case "CACHETDAITHANHSIDE":
                        dataSource = await GetPhieuCanCaChetDaiThanhSide_XLPC(dateTime);
                        break;
                    case "NGUYENLIEU":
                        dataSource = await GetPhieuCanNguyenLieu_XLPC(dateTime, xuongId);
                        break;
                    case "BTPFILLET":
                        dataSource = await GetPhieuCanBTPFillets_XLPC(dateTime, xuongId);
                        break;
                    case "TPFILLET":
                        dataSource = await GetPhieuCanTPFillets_XLPC(dateTime, xuongId);
                        break;
                    case "DINHHINH":
                        dataSource = await GetPhieuCanDinhHinhs_XLPC(dateTime, xuongId);
                        break;
                    case "PHUXEPKHUON":
                        dataSource = await GetPhieuCanPhuXepKhuon_XLPC(dateTime, xuongId);
                        break;
                    case "CHINHXEPKHUON":
                        dataSource = await GetPhieuCanChinhXepKhuon_XLPC(dateTime, xuongId);
                        break;
                    case "SAUXEPKHUON":
                        dataSource = await GetPhieuCanSauXepKhuon_XLPC(dateTime, xuongId);
                        break;
                    case "XEPKHUONKHC":
                        dataSource = await GetPhieuCanXepKhuonKHC_XLPC(dateTime, xuongId);
                        break;
                    case "XEPKHUONBLOCK":
                        dataSource = await GetPhieuCanXepKhuonBlock_XLPC(dateTime, xuongId);
                        break;
                    case "PHUGIA":
                        dataSource = await GetPhieuCanPhuGia_XLPC(dateTime, xuongId);
                        break;
                    case "TAICHE":
                        dataSource = await GetPhieuCanTaiChe_XLPC(dateTime, xuongId);
                        break;
                    case "BAOTU":
                        dataSource = await GetPhieuCanBaoTu_XLPC(dateTime, xuongId);
                        break;
                    case "SOCHEDINHHINH":
                        dataSource = await GetPhieuCanSoCheDinhHinh_XLPC(dateTime, xuongId);
                        break;
                    case "PHUPHAM":
                        dataSource = await GetPhieuCanPhuPham_XLPC(dateTime, xuongId);
                        break;
                    case "PHUPHAMV2":
                        dataSource = await GetPhieuCanPhuPhamv2_XLPC(dateTime, xuongId);
                        break;
                    case "TOM":
                        dataSource = await GetAlls(dateTime);
                        break;
                    default:
                        // Xử lý trường hợp mặc định ở đây nếu cần
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
        public string EncryBiosId(string biosId)
        {
            //var biosIdEncry = Security.Crypt.ED.EncryptString(biosId);
            //string sanitizedBiosId;

            //if (biosIdEncry.Contains("/"))
            //{
            //    // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
            //    sanitizedBiosId = biosIdEncry.Replace("/", "_");
            //}
            //else
            //{
            //    // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
            //    sanitizedBiosId = biosIdEncry;
            //}
            ////sanitizedBiosId = HttpUtility.UrlEncode( biosIdEncry);
            //return sanitizedBiosId;
            return biosId.StringToHexString();
        }
        #region Vùng Nuôi Đại Thành Side
        public async Task<IActionResult> CreatDefautNewVungNuoiDaiThanh(DateTime dateTime)
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    CanLai = false,
                    IsTap = false,
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PhieuCanVungNuoiDaiThanhSide>> GetAllsWithDateVungNuoiDaiThanhSide(DateTime dateTime)
        {
            IEnumerable<PhieuCanVungNuoiDaiThanhSide> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetAllsWithDate/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanVungNuoiDaiThanhSide>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoInsert_VungNuoiDaiThanhSide(DateTime dateTime, string xuongId, DateTime ngayTai, TimeSpan gioTai, double trongLuong, string maLo, string tenAo, string tenCongDoan, string tenThongKeDauAo, string maLoaiCaVungNuoi, string tenLoaiCa, string maGhe, string tenGhe, string maMayCan, bool canLai, bool isTap)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/Insert";
            try
            {
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(ngayTai.ToString()) || string.IsNullOrEmpty(gioTai.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(tenAo.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenCongDoan.ToString()) || string.IsNullOrEmpty(tenThongKeDauAo.ToString()) || string.IsNullOrEmpty(maGhe.ToString()) || string.IsNullOrEmpty(maMayCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var stt = 0;
                var dataPhieuCanVungNuoiDaiThanh = await GetAllsWithDateVungNuoiDaiThanhSide(dateTime);
                if (dataPhieuCanVungNuoiDaiThanh.Any())
                {
                    stt = dataPhieuCanVungNuoiDaiThanh.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
                }
                else
                {
                    stt = 1;
                }
                var model = new PhieuCanVungNuoiDaiThanhSide
                {
                    STT = stt,
                    Ngay = dateTime,
                    MaMayCan = maMayCan,
                    BiosId = AppViewModels.HardId.GetBiosId(),
                    NgayTai = ngayTai,
                    GioTai = gioTai,
                    Gio = DateTime.Now.TimeOfDay,
                    MaLo = maLo,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THEM PHIEU CAN",
                    MaLoaiCaDaiThanhId = maLoaiCaVungNuoi??"",
                    TenLoaiCa = tenLoaiCa,
                    CanLai = canLai,
                    MaGhe = maGhe,
                    TenGhe = tenGhe,
                    TenAo = tenAo,
                    TenCongDoan = tenCongDoan,
                    TenThongKeDauAo = tenThongKeDauAo,
                    TrongLuongTare = 0,
                    TrongLuong = trongLuong,
                    IsTap = isTap
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoUpDate_VungNuoiDaiThanhSide(DateTime dateTime, string xuongId, int stt, string biosId, DateTime ngayTai, TimeSpan gioTai, double trongLuong, string maLo, string tenAo, string tenCongDoan, string tenThongKeDauAo, string maLoaiCaVungNuoi, string tenLoaiCa, string maGhe, string tenGhe, string maMayCan, bool canLai, bool isTap)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/Update/{stt}/{dateTime.ToString("yyyy-MM-dd")}/{maMayCan}/{EncryBiosId(biosId)}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(ngayTai.ToString()) || string.IsNullOrEmpty(gioTai.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(tenAo.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenCongDoan.ToString()) || string.IsNullOrEmpty(tenThongKeDauAo.ToString()) || string.IsNullOrEmpty(maLoaiCaVungNuoi.ToString()) || string.IsNullOrEmpty(maGhe.ToString()) || string.IsNullOrEmpty(maMayCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanVungNuoiDaiThanhSide
                {
                    NgayTai = ngayTai,
                    GioTai = gioTai,
                    TrongLuong = trongLuong,
                    MaLo = maLo,
                    TenAo = tenAo,
                    TenCongDoan = tenCongDoan,
                    TenThongKeDauAo = tenThongKeDauAo,
                    MaLoaiCaDaiThanhId = maLoaiCaVungNuoi,
                    TenLoaiCa = tenLoaiCa,
                    MaGhe = maGhe,
                    TenGhe = tenGhe,
                    CanLai = canLai,
                    IsTap = isTap,
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
        public async Task<IActionResult> GetsByMa_VungNuoiDaiThanhSide(string listInfoPhieuCan, DateTime ngay)
        {
            try
            {
                string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string biosId = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(biosId))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn thông tin!."
                });
            }
                //var biosIdEncry = Security.Crypt.ED.EncryptString(biosId);
                //string sanitizedBiosId;

                //if (biosIdEncry.Contains("/"))
                //{
                //    // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
                //    sanitizedBiosId = biosIdEncry.Replace("/", "_");
                //}
                //else
                //{
                //    // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
                //    sanitizedBiosId = biosIdEncry;
                //}
                string sanitizedBiosId = biosId.StringToHexString();
                IEnumerable<object> dataSource = ViewBag.dataSource;
                if (dataSource == null)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{sanitizedBiosId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanVungNuoiDaiThanhSide>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        MaMayCan = item.MaMayCan,
                        BiosId = item.BiosId,
                        NgayTai = item.NgayTai,
                        GioTai = item.GioTai,
                        Gio = item.Gio,
                        MaLo = item.MaLo,
                        MaUserCan = item.MaUserCan,
                        GhiChu = item.GhiChu,
                        MaLoaiCaDaiThanhId = item.MaLoaiCaDaiThanhId,
                        TenLoaiCa = item.TenLoaiCa,
                        CanLai = item.CanLai,
                        MaGhe = item.MaGhe,
                        TenGhe = item.TenGhe,
                        TenAo = item.TenAo,
                        TenCongDoan = item.TenCongDoan,
                        TenThongKeDauAo = item.TenThongKeDauAo,
                        TrongLuongTare = item.TrongLuongTare,
                        TrongLuong = item.TrongLuong,
                        IsTap = item.IsTap
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = $"Item Null {apiUrl}"
                    });
                }
            }
                else
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "DataSource Null"
                });
            }

        }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new
                {
                    isSuccess = false,
                    Mesages = e.ToString()
                });
            }
            
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoDelete_VungNuoiDaiThanhSide(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string biosId = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanVungNuoiDaiThanhSide
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        TrongLuongTare = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{EncryBiosId(biosId)}";

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
        #region Cá giống vùng nuôi đại thành side
        public async Task<IActionResult> CreatDefautNewCaGiongVungNuoiDaiThanh(DateTime dateTime)
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    CanLai = false,
                    IsTap = false,
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<PhieuCanCaGiongVungNuoiDaiThanhSide>> GetAllsWithDateCaGiongVungNuoiDaiThanhSide(DateTime dateTime)
        {
            IEnumerable<PhieuCanCaGiongVungNuoiDaiThanhSide> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaGiongVungNuoiDaiThanhSides/GetAllsWithDate/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanCaGiongVungNuoiDaiThanhSide>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoInsert_CaGiongVungNuoiDaiThanhSide(DateTime dateTime, string xuongId, DateTime ngayTai, TimeSpan gioTai, decimal trongLuong, string tenAo, string tenChuAo, string tenCongDoan, string tenThongKeDauAo, string maLoaiCaVungNuoi, string tenLoaiCa, string maGhe, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaGiongVungNuoiDaiThanhSides/Insert";
            try
            {
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(ngayTai.ToString()) || string.IsNullOrEmpty(gioTai.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenAo.ToString()) || string.IsNullOrEmpty(tenChuAo.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenCongDoan.ToString()) || string.IsNullOrEmpty(tenThongKeDauAo.ToString()) || string.IsNullOrEmpty(maLoaiCaVungNuoi.ToString()) || string.IsNullOrEmpty(maGhe.ToString()) || string.IsNullOrEmpty(maMayCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var stt = 0;
                var dataPhieuCanCaGiongVungNuoiDaiThanh = await GetAllsWithDateCaGiongVungNuoiDaiThanhSide(dateTime);
                if (dataPhieuCanCaGiongVungNuoiDaiThanh.Any())
                {
                    stt = dataPhieuCanCaGiongVungNuoiDaiThanh.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
                }
                else
                {
                    stt = 1;
                }
                var model = new PhieuCanCaGiongVungNuoiDaiThanhSide
                {
                    STT = stt,
                    Ngay = dateTime,
                    MaMayCan = maMayCan,
                    BiosId = AppViewModels.HardId.GetBiosId(),
                    NgayTai = ngayTai,
                    GioTai = gioTai,
                    Gio = DateTime.Now.TimeOfDay,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THEM PHIEU CAN",
                    MaLoaiCaDaiThanhId = maLoaiCaVungNuoi,
                    TenLoaiCa = tenLoaiCa,
                    MaGhe = maGhe,
                    TenAo = tenAo,
                    TenChuAo = tenChuAo,
                    TenCongDoan = tenCongDoan,
                    TenThongKeDauAo = tenThongKeDauAo,
                    TrongLuongTare = 0,
                    TrongLuong = trongLuong,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoUpDate_CaGiongVungNuoiDaiThanhSide(DateTime dateTime, string xuongId, int stt, string biosId, DateTime ngayTai, TimeSpan gioTai, decimal trongLuong, string tenAo, string tenChuAo, string tenCongDoan, string tenThongKeDauAo, string maLoaiCaVungNuoi, string tenLoaiCa, string maGhe, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaGiongVungNuoiDaiThanhSides/Update/{stt}/{dateTime.ToString("yyyy-MM-dd")}/{maMayCan}/{EncryBiosId(biosId)}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(ngayTai.ToString()) || string.IsNullOrEmpty(gioTai.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenAo.ToString()) || string.IsNullOrEmpty(tenChuAo.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenCongDoan.ToString()) || string.IsNullOrEmpty(tenThongKeDauAo.ToString()) || string.IsNullOrEmpty(maLoaiCaVungNuoi.ToString()) || string.IsNullOrEmpty(maGhe.ToString()) || string.IsNullOrEmpty(maMayCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanCaGiongVungNuoiDaiThanhSide
                {
                    NgayTai = ngayTai,
                    GioTai = gioTai,
                    TrongLuong = trongLuong,
                    TenAo = tenAo,
                    TenChuAo = tenChuAo,
                    TenCongDoan = tenCongDoan,
                    TenThongKeDauAo = tenThongKeDauAo,
                    MaLoaiCaDaiThanhId = maLoaiCaVungNuoi,
                    TenLoaiCa = tenLoaiCa,
                    MaGhe = maGhe,
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
        public async Task<IActionResult> GetsByMa_CaGiongVungNuoiDaiThanhSide(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string biosId = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(biosId))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn thông tin!."
                });
            }
            var biosIdEncry = Security.Crypt.ED.EncryptString(biosId);
            string sanitizedBiosId;

            if (biosIdEncry.Contains("/"))
            {
                // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
                sanitizedBiosId = biosIdEncry.Replace("/", "_");
            }
            else
            {
                // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
                sanitizedBiosId = biosIdEncry;
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaGiongVungNuoiDaiThanhSides/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{sanitizedBiosId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanCaGiongVungNuoiDaiThanhSide>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        MaMayCan = item.MaMayCan,
                        BiosId = item.BiosId,
                        NgayTai = item.NgayTai,
                        GioTai = item.GioTai,
                        Gio = item.Gio,
                        MaUserCan = item.MaUserCan,
                        GhiChu = item.GhiChu,
                        MaLoaiCaDaiThanhId = item.MaLoaiCaDaiThanhId,
                        TenLoaiCa = item.TenLoaiCa,
                        MaGhe = item.MaGhe,
                        TenAo = item.TenAo,
                        TenChuAo = item.TenChuAo,
                        TenCongDoan = item.TenCongDoan,
                        TenThongKeDauAo = item.TenThongKeDauAo,
                        TrongLuongTare = item.TrongLuongTare,
                        TrongLuong = item.TrongLuong,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoDelete_CaGiongVungNuoiDaiThanhSide(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string biosId = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanCaGiongVungNuoiDaiThanhSide
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        TrongLuongTare = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaGiongVungNuoiDaiThanhSides/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{EncryBiosId(biosId)}";

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
        #region cá chết vùng nuôi đại thành side
        public async Task<IActionResult> CreatDefautNewCaChetDaiThanh()
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<PhieuCanCaChetDaiThanhSide>> GetAllsWithDateCaChetDaiThanhSide(DateTime dateTime)
        {
            IEnumerable<PhieuCanCaChetDaiThanhSide> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaChetDaiThanhSides/GetAllsWithDate/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanCaChetDaiThanhSide>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoInsert_CaChetDaiThanhSide(DateTime dateTime, string xuongId, DateTime ngayTai, TimeSpan gioTai, decimal trongLuong, decimal trongLuongBinhQuan, string tenLoaiCa, string tenKhachHang, string tenAo, string tenThongKe, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaChetDaiThanhSides/Insert";
            try
            {
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(ngayTai.ToString()) || string.IsNullOrEmpty(gioTai.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenAo.ToString()) || string.IsNullOrEmpty(tenKhachHang.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(trongLuongBinhQuan.ToString()) || string.IsNullOrEmpty(tenThongKe.ToString()) || string.IsNullOrEmpty(maMayCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var stt = 0;
                var dataPhieuCanCaChetDaiThanh = await GetAllsWithDateCaChetDaiThanhSide(dateTime);
                if (dataPhieuCanCaChetDaiThanh.Any())
                {
                    stt = dataPhieuCanCaChetDaiThanh.Select(x => Math.Abs(x.STT)).DefaultIfEmpty(0).Max() + 1;
                }
                else
                {
                    stt = 1;
                }
                var model = new PhieuCanCaChetDaiThanhSide
                {
                    STT = stt,
                    Ngay = dateTime,
                    MaMayCan = maMayCan,
                    Gio = DateTime.Now.TimeOfDay,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THEM PHIEU CAN",
                    TenLoaiCa = tenLoaiCa,
                    TenKhachHang = tenKhachHang,
                    TenThongKe = tenThongKe,
                    TenAo = tenAo,
                    TrongLuong = trongLuong,
                    TrongLuongBinhQuan = trongLuongBinhQuan,
                    TrongLuongTare = 0,
                    BiosId = AppViewModels.HardId.GetBiosId(),
                    GioTai = gioTai,
                    NgayTai = ngayTai,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoUpDate_CaChetDaiThanhSide(DateTime dateTime, string xuongId, int stt, string biosId, DateTime ngayTai, TimeSpan gioTai, decimal trongLuong, decimal trongLuongBinhQuan, string tenLoaiCa, string tenKhachHang, string tenAo, string tenThongKe, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaChetDaiThanhSides/Update/{stt}/{dateTime.ToString("yyyy-MM-dd")}/{maMayCan}/{EncryBiosId(biosId)}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(ngayTai.ToString()) || string.IsNullOrEmpty(gioTai.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(tenAo.ToString()) || string.IsNullOrEmpty(tenKhachHang.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(trongLuongBinhQuan.ToString()) || string.IsNullOrEmpty(tenThongKe.ToString()) || string.IsNullOrEmpty(maMayCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanCaChetDaiThanhSide
                {
                    NgayTai = ngayTai,
                    GioTai = gioTai,
                    TrongLuong = trongLuong,
                    TrongLuongBinhQuan = trongLuongBinhQuan,
                    TenAo = tenAo,
                    TenLoaiCa = tenLoaiCa,
                    TenKhachHang = tenKhachHang,
                    TenThongKe = tenThongKe,
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
        public async Task<IActionResult> GetsByMa_CaChetDaiThanhSide(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string biosId = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(biosId))
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn thông tin!."
                });
            }
            var biosIdEncry = Security.Crypt.ED.EncryptString(biosId);
            string sanitizedBiosId;

            if (biosIdEncry.Contains("/"))
            {
                // Nếu có dấu '/' trong chuỗi mã hóa, thay thế bằng dấu '_'
                sanitizedBiosId = biosIdEncry.Replace("/", "_");
            }
            else
            {
                // Nếu không có dấu '/', giữ nguyên chuỗi mã hóa
                sanitizedBiosId = biosIdEncry;
            }
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaChetDaiThanhSides/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{sanitizedBiosId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanCaChetDaiThanhSide>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        MaMayCan = item.MaMayCan,
                        Gio = item.Gio,
                        MaUserCan = item.MaUserCan,
                        GhiChu = item.GhiChu,
                        TenLoaiCa = item.TenLoaiCa,
                        TenKhachHang = item.TenKhachHang,
                        TenThongKe = item.TenThongKe,
                        TenAo = item.TenAo,
                        TrongLuong = item.TrongLuong,
                        TrongLuongBinhQuan = item.TrongLuongBinhQuan,
                        TrongLuongTare = item.TrongLuongTare,
                        BiosId = item.BiosId,
                        GioTai = item.GioTai,
                        NgayTai = item.NgayTai,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoDelete_CaChetDaiThanhSide(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string biosId = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanCaChetDaiThanhSide
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        TrongLuongBinhQuan = 0,
                        TrongLuongTare = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanCaChetDaiThanhSides/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{EncryBiosId(biosId)}";

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
        #region Nguyên Liêu
        public async Task<IActionResult> CreatDefautNewNguyenLieu(DateTime dateTime)
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    SuDung = true,
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PhieuCanNguyenLieu>> GetAllsWithDateNguyenLieu(DateTime dateTime)
        {
            IEnumerable<PhieuCanNguyenLieu> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetAllsWithDate/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<PhieuCanNguyenLieu>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoInsert_NguyenLieu(DateTime dateTime, string xuongId, DateTime thoiGianCan, string maLoaiCa, string nhaCC, string maThanhPham, string maLo, string maSize, string maPhuongTien, string maMau, string maAo, string maBanCatTiet, bool suDung, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/Insert";
            try
            {
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(maLoaiCa.ToString()) || string.IsNullOrEmpty(nhaCC.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maPhuongTien.ToString()) || string.IsNullOrEmpty(maMau.ToString()) || string.IsNullOrEmpty(maAo.ToString()) || string.IsNullOrEmpty(maBanCatTiet.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanNguyenLieu
                {
                    MaMayTinhCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    ThoiGianCan = thoiGianCan,
                    Ngay = dateTime,
                    NhaCC = nhaCC,
                    MSL = maLo,
                    MaAo = maAo,
                    MaPhuongTien = maPhuongTien,
                    MaLoaiCa = maLoaiCa,
                    MaLoaiThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaMau = maMau,
                    MaBanCatTiet = maBanCatTiet,
                    TrongLuong = trongLuong,
                    SuDung = suDung,
                    MaXuongSanXuat = xuongId,
                    TyLeNuoc = 0,
                    TrongLuongOrg = 0,
                    TrongLuongTare = 0,
                    Pheu = "0",
                    Chuyen = 0,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THEM PHIEU CAN",
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoUpDate_NguyenLieu(DateTime dateTime, string xuongId, DateTime thoiGianCan, string maLoaiCa, string nhaCC, string maThanhPham, string maLo, string maSize, string maPhuongTien, string maMau, string maAo, string maBanCatTiet, bool suDung, decimal trongLuong, string maMayTinhCan, string maUserCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/Update/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("HH-mm-ss")}";
            try
            {
                if (string.IsNullOrEmpty(dateTime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(maLoaiCa.ToString()) || string.IsNullOrEmpty(nhaCC.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maPhuongTien.ToString()) || string.IsNullOrEmpty(maMau.ToString()) || string.IsNullOrEmpty(maAo.ToString()) || string.IsNullOrEmpty(maBanCatTiet.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(maMayTinhCan.ToString()) || string.IsNullOrEmpty(maUserCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanNguyenLieu
                {
                    ThoiGianCan = thoiGianCan,
                    MaLoaiCa = maLoaiCa,
                    NhaCC = nhaCC,
                    MaLoaiThanhPham = maThanhPham,
                    MSL = maLo,
                    MaSize = maSize,
                    MaPhuongTien = maPhuongTien,
                    MaMau = maMau,
                    MaAo = maAo,
                    MaBanCatTiet = maBanCatTiet,
                    SuDung = suDung,
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
        public async Task<IActionResult> GetsByMa_NguyenLieu(string listInfoPhieuCan, DateTime ngay, string xuongId)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            string maMayTinhCan = parts[0];
            string maUserCan = parts[1];
            DateTime thoiGianCan = DateTime.Parse(parts[2]);
            decimal trongLuong = decimal.Parse(parts[3]);
            if (trongLuong <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa hoặc trọng lượng = 0! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(maMayTinhCan.ToString()) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetsByMa/{ngay.ToString("yyyy-MM-dd")}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("HH-mm-ss")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanNguyenLieu>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaMayTinhCan = item.MaMayTinhCan,
                        MaUserCan = item.MaUserCan,
                        ThoiGianCan = item.ThoiGianCan,
                        Ngay = item.Ngay,
                        NhaCC = item.NhaCC,
                        MSL = item.MSL,
                        MaAo = item.MaAo,
                        MaPhuongTien = item.MaPhuongTien,
                        MaLoaiCa = item.MaLoaiCa,
                        MaLoaiThanhPham = item.MaLoaiThanhPham,
                        MaSize = item.MaSize,
                        MaMau = item.MaMau,
                        MaBanCatTiet = item.MaBanCatTiet,
                        TrongLuong = item.TrongLuong,
                        SuDung = item.SuDung,
                        MaXuongSanXuat = item.MaXuongSanXuat,
                        TyLeNuoc = item.TyLeNuoc,
                        TrongLuongOrg = item.TrongLuongOrg,
                        TrongLuongTare = item.TrongLuongTare,
                        Pheu = item.Pheu,
                        Chuyen = item.Chuyen,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoDelete_NguyenLieu(string listInfoPhieuCan, DateTime dateTime, string xuongId)
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

                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);

                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanNguyenLieu
                    {
                        TrongLuong = 0,
                        TrongLuongTare = 0,
                        TrongLuongOrg = 0,
                        TyLeNuoc = 0,
                        SuDung = false,
                        Pheu = "0",
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/Delete/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("HH-mm-ss")}";

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
        public async Task<IActionResult> CheckQuyenNguyenLieu(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu")]
        public async Task<IActionResult> ChuyenXuong_NguyenLieu(string listInfoPhieuCan, DateTime dateTime, string xuongId, string maXuongChange)
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
                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);

                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanNguyenLieu
                    {
                        MaXuongSanXuat = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/ChuyenXuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("HH-mm-ss")}";
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
                    Messages = "Đã Chuyển Xưởng các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu")]
        public async Task<IActionResult> ChuyenSize_NguyenLieu(string listInfoPhieuCan, DateTime dateTime, string xuongId, string maSizeChange)
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
                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);

                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanNguyenLieu
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/ChuyenSize/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("HH-mm-ss")}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Nguyên Liệu")]
        public async Task<IActionResult> ChuyenThanhPham_NguyenLieu(string listInfoPhieuCan, DateTime dateTime, string xuongId, string maThanhPhamChange)
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
                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);

                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanNguyenLieu
                    {
                        MaLoaiThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/ChuyenThanhPham/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("HH-mm-ss")}";
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
        #endregion
        #region Fillet

        #region BTP Fillet
        public async Task<IActionResult> CreatDefautNewFillet(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongFillet(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = stt,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoInsert_Fillet(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maThanhPham, string maSize, string maNhanVien, string maNhanVienPhucVu, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(maNhanVienPhucVu.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanBTPFilletv2
                {
                    STT = stt,
                    Ngay = dateTime,
                    Gio = gio,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaLoaiCa = "A",
                    MaMau = "0",
                    MaSize = maSize,
                    MaThanhPham = maThanhPham,
                    MaLo = maLo,
                    MaThe = "0",
                    MaNhanVien = maNhanVien,
                    MaMayLangDa = "M1",
                    TrongLuong = trongLuong,
                    IsEnabled = true,
                    MaXuong = xuongId,
                    CaTra = false,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THÊM,",
                    TrongLuongTare = 0,
                    MaNhanVienPhucVu = maNhanVienPhucVu,
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
        public async Task<IActionResult> GetsByMaFillet(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanBTPFilletv2>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaUserCan = item.MaUserCan,
                        MaMayCan = item.MaMayCan,
                        MaLoaiCa = item.MaLoaiCa,
                        MaMau = item.MaMau,
                        MaSize = item.MaSize,
                        MaThanhPham = item.MaThanhPham,
                        MaLo = item.MaLo,
                        MaThe = item.MaThe,
                        MaNhanVien = item.MaNhanVien,
                        MaMayLangDa = item.MaMayLangDa,
                        TrongLuong = item.TrongLuong,
                        IsEnabled = item.IsEnabled,
                        MaXuong = item.MaXuong,
                        CaTra = item.CaTra,
                        GhiChu = item.GhiChu,
                        TrongLuongTare = item.TrongLuongTare,
                        MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoUpDateFillet(DateTime ngay, string maXuong, string maMayCanBTP, int sttBTP, TimeSpan gio, string maLo, string maThanhPham, string maSize, string maNhanVien, string maNhanVienPhucVu, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/Update/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(maNhanVienPhucVu.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanBTPFilletv2
                {
                    MaLo = maLo,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaNhanVien = maNhanVien,
                    MaNhanVienPhucVu = maNhanVienPhucVu,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> DoDeleteFillet(string listInfoPhieuCan, DateTime ngay)
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

                    int sttBTP = int.Parse(parts[0]);
                    string maMayCanBTP = parts[1];
                    string maXuong = parts[2];

                    if (sttBTP <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanBTPFilletv2
                    {
                        STT = sttBTP * -1,
                        TrongLuong = 0,
                        TrongLuongTare = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/Delete/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenFillet(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> ChuyenXuong_Fillet(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanBTPFilletv2
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Xưởng Phiếu Cân BTP Fillet thất bại: " + responseTP.Message
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> ChuyenSize_Fillet(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanBTPFilletv2
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Size Phiếu Cân BTP Fillet thất bại: " + responseTP.Message
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân BTP Fillet")]
        public async Task<IActionResult> ChuyenThanhPham_Fillet(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanBTPFilletv2
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Size Phiếu Cân BTP Fillet thất bại: " + responseTP.Message
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
        #endregion
        #region TP Fillet


        public class NhanVienBanAndTrongLuongTra
        {
            public string MaNhanVien { get; set; }
            public decimal? TrongLuongTra { get; set; }
        }
        public class MaxSTTResponse
        {
            public int MaxSTT { get; set; }
        }
        public async Task<int> GetMaxSTT_TPFillet(DateTime dateTime, string maMayCan, string xuongId)
        {
            int maxSTT = 0;
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetMaxSTT/{dateTime.ToString("yyyy-MM-dd")}/{maMayCan}/{xuongId}";

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var response = await helper.GetAsync<MaxSTTResponse>(HttpContext, apiUrl);

            if (response != null)
            {
                maxSTT = response.MaxSTT;
            }

            return maxSTT;
        }
        public async Task<int> GetSTT_TPFillet(DateTime ngay, string maNhanVien, string maXuong, string maMayCan, int sttBTP, string thePhieuSanLuongId)
        {
            int stt = -1; // Default value to indicate failure
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetSTT/{ngay:yyyy-MM-dd}/{maNhanVien}/{maXuong}/{maMayCan}/{sttBTP}/{thePhieuSanLuongId}";

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            try
            {
                var response = await helper.GetAsync<ApiResponse>(HttpContext, apiUrl);

                if (response != null && response.Success)
                {
                    stt = response.STT;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging mechanism)
                Console.WriteLine($"Error calling API: {ex.Message}");
            }

            return stt;
        }
        public async Task<List<PhieuCanTPFilletv2>> GetAllByThePhieuSLIds(string thePhieuSanLuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetAllByThePhieuSLIds/{thePhieuSanLuongId}";

            try
            {
                var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.GetAsync<ApiResponse>(HttpContext, apiUrl);
                if (rl != null && rl.Success == true)
                {
                    // Chuyển đổi dữ liệu từ object sang danh sách đối tượng
                    var jsonData = JsonConvert.SerializeObject(rl.Data); // Chuyển đổi dữ liệu object sang JSON
                    var listData = JsonConvert.DeserializeObject<List<PhieuCanTPFilletv2>>(jsonData); // Chuyển đổi JSON sang danh sách đối tượng

                    return listData;
                }
                else
                {
                    return new List<PhieuCanTPFilletv2>(); // Trả về danh sách rỗng nếu không có item
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi khi gọi API: {ex.Message}");
                // Ném ngoại lệ để báo hiệu rằng có lỗi xảy ra
                throw;
            }
        }

        // ApiResponse class to match the structure of the API response
        public class ApiResponse
        {
            public bool Success { get; set; }
            public int STT { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }
        private string CreateIdThePhieuSanLuongFillet(string dateTime, string maMayCan, string xuongId)
        {
            var header = $@"{dateTime}.{maMayCan}";
            var id = $"{header}.{xuongId}.{DateTime.Now.TimeOfDay.ToString(@"hhmmssffffff")}";
            return id;
        }
        public ThePhieuSanLuongFillet CreateNewThePhieuSanLuongFillet(PhieuCanTPFilletv2 phieuCan)
        {
            var id = CreateIdThePhieuSanLuongFillet(phieuCan.Ngay.ToString("yyyyMMdd"), phieuCan.MaMayCan, phieuCan.MaXuong);
            return new ThePhieuSanLuongFillet
            {
                Id = id,
                IsDone = false,
                CaTra = phieuCan.CaTra,
                MaLoaiCa = phieuCan.MaLoaiCa,
                MaMau = phieuCan.MaMau,
                MaSize = phieuCan.MaSize,
                MaThanhPham = phieuCan.MaThanhPham,
                MaXuong = phieuCan.MaXuong,
                Ngay = phieuCan.Ngay,
                TrongLuongTare = phieuCan.TrongLuongTare,
                MayCan_PC = phieuCan.MaMayCan,
                MayCan_PC_BTP = phieuCan.MaMayCanBTP ?? "",
                Gio = DateTime.Now.TimeOfDay,
                MaLo = phieuCan.MaLo,
                STT_PC_BTP = phieuCan.STTBTP ?? 0,
                STT_PC = phieuCan.STT,
                MaThe = phieuCan.MaThe,
                MaNhanVien = phieuCan.MaNhanVien,
                MaBan = phieuCan.MaBan ?? "",
                TrongLuongNhan = phieuCan.TrongLuongNhan,
                GhiChu = phieuCan.GhiChu,
                IdIn = phieuCan.IdIn
            };
        }
        public ThePhieuSanLuongFillet CreateUpdateThePhieuSanLuongFillet(PhieuCanTPFilletv2 phieuCan)
        {
            return new ThePhieuSanLuongFillet
            {
                Id = phieuCan.ThePhieuSanLuongId ?? "",
                MaLoaiCa = phieuCan.MaLoaiCa,
                MaMau = phieuCan.MaMau,
                MaSize = phieuCan.MaSize,
                MaThanhPham = phieuCan.MaThanhPham,
                MaLo = phieuCan.MaLo,
                MaNhanVien = phieuCan.MaNhanVien,
                GhiChu = phieuCan.GhiChu,
            };
        }
        public PhieuCanBTPFilletv2 CreateUpdatePhieuCanBTPFillet(PhieuCanTPFilletv2 phieuCan)
        {
            return new PhieuCanBTPFilletv2
            {
                MaLo = phieuCan.MaLo,
                MaThanhPham = phieuCan.MaThanhPham,
                MaSize = phieuCan.MaSize,
                MaNhanVien = phieuCan.MaNhanVien,
                MaNhanVienPhucVu = phieuCan.MaNhanVienPhucVu,
                GhiChu = phieuCan.GhiChu,
            };
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Fillet", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Fillet")]
        public async Task<IActionResult> DoInsert_TPFillet(DateTime datetime, string xuongId, TimeSpan gio, string maLo, int sttBTP, string maSize, string maMau, string maThanhPham, string maLoaiCa, decimal trongLuongNhan, decimal dinhMuc, string maBan, string maNhanVienPhucVu, string maMayCanBTP, string listMaNhanVienBanAndTrongLuongRa)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/Insert";
            var apiThePhieuSanLuongFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ThePhieuSanLuongFillets/Insert";
            try
            {
                if (string.IsNullOrEmpty(datetime.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maMau.ToString()) || string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(maMayCanBTP.ToString()) || string.IsNullOrEmpty(listMaNhanVienBanAndTrongLuongRa.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    List<NhanVienBanAndTrongLuongTra> listNhanVienBan = new List<NhanVienBanAndTrongLuongTra>();

                    //var records = listMaNhanVienBanAndTrongLuongRa.Split('|');
                    //foreach (var record in records)
                    //{
                    //    if (!string.IsNullOrEmpty(record))
                    //    {
                    //        var parts = record.Split(',');
                    //        if (parts.Length == 2)
                    //        {
                    //            listNhanVienBan.Add(new NhanVienBanAndTrongLuongTra
                    //            {
                    //                MaNhanVien = parts[0],
                    //                TrongLuongTra = decimal.Parse(parts[1])
                    //            });
                    //        }
                    //    }
                    //}
                    var records = listMaNhanVienBanAndTrongLuongRa.Split('|');
                    foreach (var record in records)
                    {
                        if (!string.IsNullOrEmpty(record))
                        {
                            var parts = record.Split(',');
                            if (parts.Length == 2)
                            {
                                decimal? trongLuongTra = null;
                                if (decimal.TryParse(parts[1], out decimal parsedValue))
                                {
                                    trongLuongTra = parsedValue;
                                }

                                listNhanVienBan.Add(new NhanVienBanAndTrongLuongTra
                                {
                                    MaNhanVien = parts[0],
                                    TrongLuongTra = trongLuongTra
                                });
                            }
                        }
                    }
                    // Loại bỏ các phần tử có TrongLuong < 0
                    listNhanVienBan.RemoveAll(nhanVien => nhanVien.TrongLuongTra == 0 || string.IsNullOrEmpty(nhanVien.TrongLuongTra.ToString()));
                    if (listNhanVienBan.Count == 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Danh sách nhân viên trống."
                        });
                    }
                    var count = 1;
                    int maxSTT = await GetMaxSTT_TPFillet(datetime, AppViewModels.AppViewModel.Instance.PCName, xuongId);
                    if (maxSTT < 0)
                    {
                        maxSTT = 0;
                    }
                    var phieuCanTPFilletv212 = listNhanVienBan.Select(i => new PhieuCanTPFilletv2
                    {
                        STT = maxSTT + count++,
                        CaTra = false,
                        Gio = gio,
                        MaLo = maLo,
                        MaLoaiCa = maLoaiCa,
                        MaMau = maMau,
                        MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                        MaSize = maSize,
                        MaThanhPham = maThanhPham,
                        MaThe = "0",
                        MaUserCan = "Xử Lý Phiếu Cân",
                        MaXuong = xuongId,
                        Ngay = datetime,
                        TrongLuongTare = 0,
                        TrongLuongTra = i.TrongLuongTra ?? 0,
                        STTBTP = sttBTP,
                        DinhMucThucTe = dinhMuc,
                        DinhMucYeuCau = 0,
                        MaMayCanBTP = maMayCanBTP,
                        TrongLuongNhan = trongLuongNhan,
                        MaNhanVien = i.MaNhanVien,
                        MaBan = maBan,
                        ThePhieuSanLuongId = "0",
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THÊM,",
                        MaNhanVienPhucVu = maNhanVienPhucVu == null ? "" : maNhanVienPhucVu,
                        IdIn = "",
                        Id = ""
                    }).ToList();

                    var thePhieuSLFillet = CreateNewThePhieuSanLuongFillet(phieuCanTPFilletv212[0]);
                    var jsonContentThePhieuSLFillet = new StringContent(JsonConvert.SerializeObject(thePhieuSLFillet), Encoding.UTF8, "application/json");
                    using var helperThePhieuSLFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var responseThePhieuSLFillet = await helperThePhieuSLFillet.PostAsync(HttpContext, apiThePhieuSanLuongFilletUrl, jsonContentThePhieuSLFillet);
                    if (responseThePhieuSLFillet.Success)
                    {
                        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        foreach (var phieuCan in phieuCanTPFilletv212)
                        {
                            phieuCan.ThePhieuSanLuongId = thePhieuSLFillet.Id;
                            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(phieuCan));
                            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                            var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                            if (!response.Success)
                            {
                                // Nếu có lỗi, rollback transaction và trả về thông báo lỗi
                                transactionScope.Dispose();
                                return Json(new
                                {
                                    isSuccess = response.Success,
                                    Messages = response.Message
                                });
                            }
                        }
                        // Nếu mọi thứ thành công, commit transaction
                        transactionScope.Complete();
                        return Json(new
                        {
                            isSuccess = true,
                            Messages = "Thêm phiếu cân cho danh sách phân bổ trọng lượng thành công!."
                        });
                    }
                    else
                    {
                        // Nếu có lỗi khi ghi dữ liệu vào ThePhieuSanLuongFillet, rollback transaction
                        transactionScope.Dispose();
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = responseThePhieuSLFillet.Message
                        });
                    }
                }
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
        public async Task<IEnumerable<object>> GetDanhSachNhanVienBanTrongLuongByThePhieuSLFillet(string maBan, string idThePhieuSLFillet)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetDanhSachNhanVienBanTrongLuongByThePhieuSLFillet/{maBan}/{idThePhieuSLFillet}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> GetsByMaTPFillet(string listInfoPhieuCan, DateTime ngay)
        {
            if (listInfoPhieuCan == null)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Chưa chọn thông tin!."
                });
            }
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanTPFilletv2>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item?.STT,
                        CaTra = item?.CaTra,
                        Gio = item?.Gio,
                        MaLo = item?.MaLo,
                        MaLoaiCa = item?.MaLoaiCa,
                        MaMau = item?.MaMau,
                        MaMayCan = item?.MaMayCan,
                        MaSize = item?.MaSize,
                        MaThanhPham = item?.MaThanhPham,
                        MaThe = item?.MaThe,
                        MaUserCan = item?.MaUserCan,
                        MaXuong = item?.MaXuong,
                        Ngay = item?.Ngay,
                        TrongLuongTare = item?.TrongLuongTare,
                        TrongLuongTra = item?.TrongLuongTra,
                        STTBTP = item?.STTBTP,
                        DinhMucThucTe = item?.DinhMucThucTe,
                        DinhMucYeuCau = item?.DinhMucYeuCau,
                        MaMayCanBTP = item?.MaMayCanBTP,
                        TrongLuongNhan = item?.TrongLuongNhan,
                        MaNhanVien = item?.MaNhanVien,
                        MaBan = item?.MaBan,
                        ThePhieuSanLuongId = item?.ThePhieuSanLuongId,
                        GhiChu = item?.GhiChu,
                        MaNhanVienPhucVu = item?.MaNhanVienPhucVu,
                        IdIn = item?.IdIn,
                        Id = item?.Id
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Fillet", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Fillet")]
        public async Task<IActionResult> DoUpdate_TPFillet(DateTime ngay, string maXuong, string stt, string maMayCan, string maLo, int sttBTP, string maMayCanBTP, string maSize, string maMau, string maThanhPham, string maLoaiCa, decimal dinhMuc, string maNhanVienPhucVu, string thePhieuSanLuongId, string listMaNhanVienBanAndTrongLuongRa)
        {
            var userName = HttpContext.Session.GetString("Username");

            var apiBTPFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/Update/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";
            var apiThePhieuSanLuongFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ThePhieuSanLuongFillets/Update/{thePhieuSanLuongId}";
            try
            {
                if (string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maMau.ToString()) || string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(thePhieuSanLuongId) || string.IsNullOrEmpty(listMaNhanVienBanAndTrongLuongRa))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    List<NhanVienBanAndTrongLuongTra> listNhanVienBan = new List<NhanVienBanAndTrongLuongTra>();

                    //var records = listMaNhanVienBanAndTrongLuongRa.Split('|');
                    //foreach (var record in records)
                    //{
                    //    if (!string.IsNullOrEmpty(record))
                    //    {
                    //        var parts = record.Split(',');
                    //        if (parts.Length == 2)
                    //        {
                    //            listNhanVienBan.Add(new NhanVienBanAndTrongLuongTra
                    //            {
                    //                MaNhanVien = parts[0],
                    //                TrongLuongTra = decimal.Parse(parts[1])
                    //            });
                    //        }
                    //    }
                    //}
                    var records = listMaNhanVienBanAndTrongLuongRa.Split('|');
                    foreach (var record in records)
                    {
                        if (!string.IsNullOrEmpty(record))
                        {
                            var parts = record.Split(',');
                            if (parts.Length == 2)
                            {
                                decimal? trongLuongTra = null;
                                if (decimal.TryParse(parts[1], out decimal parsedValue))
                                {
                                    trongLuongTra = parsedValue;
                                }

                                listNhanVienBan.Add(new NhanVienBanAndTrongLuongTra
                                {
                                    MaNhanVien = parts[0],
                                    TrongLuongTra = trongLuongTra
                                });
                            }
                        }
                    }
                    // Loại bỏ các phần tử có TrongLuong < 0
                    listNhanVienBan.RemoveAll(nhanVien => nhanVien.TrongLuongTra == 0 || string.IsNullOrEmpty(nhanVien.TrongLuongTra.ToString()));
                    if (listNhanVienBan.Count == 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Danh sách nhân viên trống."
                        });
                    }
                    var phieuCanTPFilletv212 = await Task.WhenAll(listNhanVienBan.Select(async i => new PhieuCanTPFilletv2
                    {
                        STT = await GetSTT_TPFillet(ngay, i.MaNhanVien, maXuong, maMayCan, sttBTP, thePhieuSanLuongId),
                        MaNhanVien = i.MaNhanVien,
                        MaLo = maLo,
                        MaLoaiCa = maLoaiCa,
                        MaMau = maMau,
                        MaSize = maSize,
                        MaThanhPham = maThanhPham,
                        DinhMucThucTe = dinhMuc,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, loại: SUA,",
                        MaNhanVienPhucVu = maNhanVienPhucVu ?? "",
                        MaMayCan = maMayCan,
                        MaXuong = maXuong
                    }));

                    // Convert the result to a list if needed
                    var phieuCanTPFilletv212List = phieuCanTPFilletv212.ToList();

                    var thePhieuSLFillet = CreateUpdateThePhieuSanLuongFillet(phieuCanTPFilletv212List[0]);
                    var dataTuplePhieuSLFillet = new Tuple<string>(JsonConvert.SerializeObject(thePhieuSLFillet));
                    var jsonContentThePhieuSLFillet = new StringContent(JsonConvert.SerializeObject(dataTuplePhieuSLFillet), Encoding.UTF8, "application/json");
                    using var helperThePhieuSLFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var responseThePhieuSLFillet = await helperThePhieuSLFillet.PostAsync(HttpContext, apiThePhieuSanLuongFilletUrl, jsonContentThePhieuSLFillet);
                    if (responseThePhieuSLFillet.Success)
                    {
                        var phieuCanBTPFilletv2 = CreateUpdateThePhieuSanLuongFillet(phieuCanTPFilletv212List[0]);
                        var dataTupleBTPFilletv2 = new Tuple<string>(JsonConvert.SerializeObject(phieuCanBTPFilletv2));
                        var jsonContentBTPFilletv2 = new StringContent(JsonConvert.SerializeObject(dataTupleBTPFilletv2), Encoding.UTF8, "application/json");
                        using var helperBTPFilletv2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var responseBTPFilletv2 = await helperBTPFilletv2.PostAsync(HttpContext, apiBTPFilletUrl, jsonContentBTPFilletv2);
                        if (responseBTPFilletv2.Success)
                        {
                            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                            foreach (var phieuCan in phieuCanTPFilletv212List)
                            {
                                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/Update/{phieuCan.STT}/{ngay.ToString("yyyy-MM-dd")}/{phieuCan.MaMayCan}/{phieuCan.MaXuong}";
                                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(phieuCan));
                                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                                if (!response.Success)
                                {
                                    // Nếu có lỗi, rollback transaction và trả về thông báo lỗi
                                    transactionScope.Dispose();
                                    return Json(new
                                    {
                                        isSuccess = response.Success,
                                        Messages = response.Message
                                    });
                                }
                            }
                            // Nếu mọi thứ thành công, commit transaction
                            transactionScope.Complete();
                            return Json(new
                            {
                                isSuccess = true,
                                Messages = "Sửa phiếu cân cho danh sách phân bổ trọng lượng thành công!."
                            });
                        }
                        else
                        {
                            transactionScope.Dispose();
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = responseBTPFilletv2.Message
                            });
                        }

                    }
                    else
                    {
                        // Nếu có lỗi khi ghi dữ liệu vào ThePhieuSanLuongFillet, rollback transaction
                        transactionScope.Dispose();
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = responseThePhieuSLFillet.Message
                        });
                    }
                }
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


        ///
        /// Đây là chức năng mở rộng của Sửa Phiếu Cân Fillet V2,
        /// Nó sẽ sửa bằng cách phân bố lại trọng lượng ngay trong phiếu sửa mà không cần phải xóa phiếu để thêm lại
        /// update trọng Lương nhân viên đã có 
        /// nếu có nhân viên mới được chia trọng lượng thì thêm nhân viên đó vào danh sách phiếu cân
        /// nếu nhân viên cũ đã có trọng lượng mà không được chia trong lương nữa thì xoa người đó ra khỏi danh sách
        /// còn lại các tính năng sẽ giống như hàm DoUpdate_TPFillet
        /// PENDING CHỜ THỰC HIỆN SAU
        ///
        //public async Task<IActionResult> DoUpdate_TPFillet2(DateTime ngay, string maXuong, string stt, string maMayCan, string maLo, int sttBTP, string maMayCanBTP, string maSize, string maMau, string maThanhPham, string maLoaiCa, decimal dinhMuc, string maNhanVienPhucVu, string thePhieuSanLuongId, string listMaNhanVienBanAndTrongLuongRa)
        //{
        //    var userName = HttpContext.Session.GetString("Username");
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/Update/{stt}/{ngay}/{maMayCan}/{maXuong}";
        //    var apiBTPFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/Update/{sttBTP}/{ngay}/{maMayCanBTP}/{maXuong}";
        //    var apiThePhieuSanLuongFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ThePhieuSanLuongFillets/Update/{thePhieuSanLuongId}";

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maMau.ToString()) || string.IsNullOrEmpty(sttBTP.ToString()) || string.IsNullOrEmpty(thePhieuSanLuongId) || string.IsNullOrEmpty(listMaNhanVienBanAndTrongLuongRa))
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Messages = "Vui lòng nhập đầy đủ thông tin."
        //            });
        //        }

        //        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            List<NhanVienBanAndTrongLuongTra> listNhanVienBan = new List<NhanVienBanAndTrongLuongTra>();

        //            var records = listMaNhanVienBanAndTrongLuongRa.Split('|');
        //            foreach (var record in records)
        //            {
        //                if (!string.IsNullOrEmpty(record))
        //                {
        //                    var parts = record.Split(',');
        //                    if (parts.Length == 2)
        //                    {
        //                        listNhanVienBan.Add(new NhanVienBanAndTrongLuongTra
        //                        {
        //                            MaNhanVien = parts[0],
        //                            TrongLuongTra = decimal.Parse(parts[1])
        //                        });
        //                    }
        //                }
        //            }

        //            // Loại bỏ các phần tử có TrongLuong < 0
        //            listNhanVienBan.RemoveAll(nhanVien => nhanVien.TrongLuongTra == 0 || string.IsNullOrEmpty(nhanVien.TrongLuongTra.ToString()));
        //            if (listNhanVienBan.Count == 0)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = "Danh sách nhân viên trống."
        //                });
        //            }

        //            // Lấy danh sách hiện tại từ PhieuCanTPFilletv2
        //            var existingPhieuCanTPFilletv2 = await GetPhieuCanTPFilletv2List(maLo); // Hàm này cần được định nghĩa để lấy danh sách hiện tại

        //            // Tạo danh sách các nhân viên cần thêm mới
        //            var newPhieuCanTPFilletv2 = listNhanVienBan
        //                .Where(nv => !existingPhieuCanTPFilletv2.Any(ep => ep.MaNhanVien == nv.MaNhanVien))
        //                .Select(i => new PhieuCanTPFilletv2
        //                {
        //                    MaNhanVien = i.MaNhanVien,
        //                    MaLo = maLo,
        //                    MaLoaiCa = maLoaiCa,
        //                    MaMau = maMau,
        //                    MaSize = maSize,
        //                    MaThanhPham = maThanhPham,
        //                    TrongLuongTra = i.TrongLuongTra,
        //                    DinhMucThucTe = dinhMuc,
        //                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: SUA,",
        //                    MaNhanVienPhucVu = maNhanVienPhucVu ?? string.Empty,
        //                }).ToList();

        //            // Tạo danh sách các nhân viên cần xóa bỏ
        //            var deletePhieuCanTPFilletv2 = existingPhieuCanTPFilletv2
        //                .Where(ep => !listNhanVienBan.Any(nv => nv.MaNhanVien == ep.MaNhanVien))
        //                .ToList();

        //            var thePhieuSLFillet = CreateUpdateThePhieuSanLuongFillet(newPhieuCanTPFilletv2[0]);
        //            var jsonContentThePhieuSLFillet = new StringContent(JsonConvert.SerializeObject(thePhieuSLFillet), Encoding.UTF8, "application/json");
        //            using var helperThePhieuSLFillet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //            var responseThePhieuSLFillet = await helperThePhieuSLFillet.PostAsync(HttpContext, apiThePhieuSanLuongFilletUrl, jsonContentThePhieuSLFillet);
        //            if (responseThePhieuSLFillet.Success)
        //            {
        //                var phieuCanBTPFilletv2 = CreateUpdateThePhieuSanLuongFillet(newPhieuCanTPFilletv2[0]);
        //                var jsonContentBTPFilletv2 = new StringContent(JsonConvert.SerializeObject(phieuCanBTPFilletv2), Encoding.UTF8, "application/json");
        //                using var helperBTPFilletv2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //                var responseBTPFilletv2 = await helperBTPFilletv2.PostAsync(HttpContext, apiBTPFilletUrl, jsonContentBTPFilletv2);
        //                if (responseBTPFilletv2.Success)
        //                {
        //                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

        //                    // Thêm các bản ghi mới
        //                    foreach (var phieuCan in newPhieuCanTPFilletv2)
        //                    {
        //                        var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(phieuCan));
        //                        var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //                        var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
        //                        if (!response.Success)
        //                        {
        //                            // Nếu có lỗi, rollback transaction và trả về thông báo lỗi
        //                            transactionScope.Dispose();
        //                            return Json(new
        //                            {
        //                                isSuccess = response.Success,
        //                                Messages = response.Message
        //                            });
        //                        }
        //                    }

        //                    // Xóa các bản ghi không còn tồn tại
        //                    foreach (var phieuCan in deletePhieuCanTPFilletv2)
        //                    {
        //                        var deleteApiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/Delete/{phieuCan.MaNhanVien}";
        //                        var response = await helper.DeleteAsync(HttpContext, deleteApiUrl);
        //                        if (!response.Success)
        //                        {
        //                            // Nếu có lỗi, rollback transaction và trả về thông báo lỗi
        //                            transactionScope.Dispose();
        //                            return Json(new
        //                            {
        //                                isSuccess = response.Success,
        //                                Messages = response.Message
        //                            });
        //                        }
        //                    }

        //                    // Nếu mọi thứ thành công, commit transaction
        //                    transactionScope.Complete();
        //                    return Json(new
        //                    {
        //                        isSuccess = true,
        //                        Messages = "Sửa phiếu cân cho danh sách phân bổ trọng lượng thành công!."
        //                    });
        //                }
        //                else
        //                {
        //                    transactionScope.Dispose();
        //                    return Json(new
        //                    {
        //                        isSuccess = false,
        //                        Messages = responseBTPFilletv2.Message
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                // Nếu có lỗi khi ghi dữ liệu vào ThePhieuSanLuongFillet, rollback transaction
        //                transactionScope.Dispose();
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = responseThePhieuSLFillet.Message
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            isSuccess = false,
        //            Messages = "Đã xảy ra lỗi: " + ex.Message.ToString()
        //        });
        //    }
        //}
        public class ThongTinPhieuRemove
        {
            public int STT { get; set; }
            public string MaMayCan { get; set; }
            public string MaXuong { get; set; }
            public string STTBTP { get; set; }
            public string MaMayCanBTP { get; set; }
            public string ThePhieuSLId { get; set; }
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Fillet", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Fillet")]
        //public async Task<IActionResult> DoDelete_TPFillet(string listInfoPhieuCan, DateTime ngay)
        //{
        //    try
        //    {
        //        if (listInfoPhieuCan == null)
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Mesages = "Chưa chọn thông tin!."
        //            });
        //        }
        //        List<ThongTinPhieuRemove> thongTinPhieus = new List<ThongTinPhieuRemove>();

        //        var records = listInfoPhieuCan.Split('|');
        //        foreach (var record in records)
        //        {
        //            if (!string.IsNullOrEmpty(record))
        //            {
        //                var parts2 = record.Split(',');
        //                thongTinPhieus.Add(new ThongTinPhieuRemove
        //                {
        //                    STT = int.Parse(parts2[0]),
        //                    MaMayCan = parts2[1],
        //                    MaXuong = parts2[2],
        //                    STTBTP = parts2[3],
        //                    MaMayCanBTP = parts2[4],
        //                    ThePhieuSLId = parts2[5]
        //                });
        //            }
        //        }

        //        // Sử dụng HashSet để giữ các ThePhieuSLId đã gặp
        //        var seenIds = new HashSet<string>();
        //        var uniqueThongTinPhieus = new List<ThongTinPhieuRemove>();

        //        foreach (var item in thongTinPhieus)
        //        {
        //            if (seenIds.Add(item.ThePhieuSLId))
        //            {
        //                uniqueThongTinPhieus.Add(item);
        //            }
        //        }

        //        // thongTinPhieus bây giờ sẽ chỉ chứa các phiếu duy nhất
        //        thongTinPhieus = uniqueThongTinPhieus;
        //        foreach (var item in thongTinPhieus)
        //        {
        //            //string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

        //            // Gán tên cho từng phần tử
        //            int stt = item.STT;
        //            string maMayCan = item.MaMayCan;
        //            string maXuong = item.MaXuong;
        //            string sttBTP = item.STTBTP;
        //            string maMayCanBTP = item.MaMayCanBTP;
        //            string thePhieuSanLuongId = item.ThePhieuSLId;
        //            if (stt <= 0)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
        //                });
        //            }

        //            var userName = HttpContext.Session.GetString("Username");


        //            var apiThePhieuSanLuongFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ThePhieuSanLuongFillets/Delete/{thePhieuSanLuongId}";


        //            if (string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maXuong))
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = "Vui lòng nhập đầy đủ thông tin."
        //                });
        //            }

        //            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //            {
        //                var modelThePhieuSLFillet = new ThePhieuSanLuongFillet
        //                {
        //                    GhiChu = $@"Người thực hiện: {AppViewModel.Instance.UserName}, trên máy: {AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss")}, Trọng Lượng Nhận:  , Trọng Lượng Trả: ,loại: XOA"
        //                };
        //                var modelBTPFillet = new PhieuCanBTPFilletv2
        //                {
        //                    STT = int.Parse(sttBTP) * -1,
        //                    TrongLuong = 0,
        //                    TrongLuongTare = 0,

        //                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
        //                };
        //                List<PhieuCanTPFilletv2> listPhieuCanRemoveByThePhieuSLId = new List<PhieuCanTPFilletv2>();
        //                listPhieuCanRemoveByThePhieuSLId = await GetAllByThePhieuSLIds(thePhieuSanLuongId);
        //                var phieuCanTPFilletv212 = await Task.WhenAll(listPhieuCanRemoveByThePhieuSLId.Select(async i => new PhieuCanTPFilletv2
        //                {
        //                    STT = i.STT * -1,
        //                    MaNhanVien = i.MaNhanVien,
        //                    TrongLuongTra = 0,
        //                    MaXuong = maXuong,
        //                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, loại: XOA,",
        //                }));

        //                // Convert the result to a list if needed
        //                var phieuCanTPFilletv212List = phieuCanTPFilletv212.ToList();

        //                using var helperThePhieuSLFilletUrl = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //                var apiThePhieuSLFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ThePhieuSanLuongFillets/Delete_XLPC/{thePhieuSanLuongId}";
        //                var dataTupleThePhieuSLFillet = new Tuple<string>(JsonConvert.SerializeObject(modelThePhieuSLFillet));
        //                var jsonContentThePhieuSLFillet = new StringContent(JsonConvert.SerializeObject(dataTupleThePhieuSLFillet), Encoding.UTF8, "application/json");
        //                var responseThePhieuSLFillet = await helperThePhieuSLFilletUrl.PostAsync(HttpContext, apiThePhieuSLFilletUrl, jsonContentThePhieuSLFillet);
        //                if (responseThePhieuSLFillet.Success)
        //                {
        //                    using var helperBTPFilletUrl = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //                    var apiBTPFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/Delete/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";
        //                    var dataTupleBTPFillet = new Tuple<string>(JsonConvert.SerializeObject(modelBTPFillet));
        //                    var jsonContentBTPFillet = new StringContent(JsonConvert.SerializeObject(dataTupleBTPFillet), Encoding.UTF8, "application/json");
        //                    var responseBTPFillet = await helperBTPFilletUrl.PostAsync(HttpContext, apiBTPFilletUrl, jsonContentBTPFillet);
        //                    if (responseBTPFillet.Success)
        //                    {
        //                        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //                        foreach (var phieuCan in phieuCanTPFilletv212List)
        //                        {
        //                            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/Delete/{phieuCan.STT}/{ngay.ToString("yyyy-MM-dd")}/{phieuCan.MaMayCan}/{phieuCan.MaXuong}";
        //                            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(phieuCan));
        //                            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //                            var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
        //                            if (!response.Success)
        //                            {
        //                                // Nếu có lỗi, rollback transaction và trả về thông báo lỗi
        //                                transactionScope.Dispose();
        //                                return Json(new
        //                                {
        //                                    isSuccess = response.Success,
        //                                    Messages = response.Message
        //                                });
        //                            }
        //                        }
        //                        // Nếu mọi thứ thành công, commit transaction
        //                        transactionScope.Complete();
        //                        return Json(new
        //                        {
        //                            isSuccess = true,
        //                            Messages = "Sửa phiếu cân cho danh sách phân bổ trọng lượng thành công!."
        //                        });
        //                    }
        //                    else
        //                    {
        //                        transactionScope.Dispose();
        //                        return Json(new
        //                        {
        //                            isSuccess = false,
        //                            Messages = responseBTPFillet.Message
        //                        });
        //                    }

        //                }
        //                else
        //                {
        //                    transactionScope.Dispose();
        //                    return Json(new
        //                    {
        //                        isSuccess = false,
        //                        Messages = responseThePhieuSLFillet.Message
        //                    });
        //                }

        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            isSuccess = false,
        //            Messages = "Đã xảy ra lỗi: " + ex.Message.ToString()
        //        });
        //    }
        //}
        public async Task<IActionResult> DoDelete_TPFillet2(string listInfoPhieuCan, DateTime ngay)
        {
            try
            {
                // Kiểm tra xem listInfoPhieuCan có rỗng hoặc null không
                if (string.IsNullOrEmpty(listInfoPhieuCan))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Chưa chọn thông tin!."
                    });
                }

                // Tạo danh sách thông tin phiếu cần xóa từ chuỗi đầu vào
                List<ThongTinPhieuRemove> thongTinPhieus = new List<ThongTinPhieuRemove>();
                var records = listInfoPhieuCan.Split('|');
                foreach (var record in records)
                {
                    if (!string.IsNullOrEmpty(record))
                    {
                        var parts2 = record.Split(',');
                        thongTinPhieus.Add(new ThongTinPhieuRemove
                        {
                            STT = int.Parse(parts2[0]),
                            MaMayCan = parts2[1],
                            MaXuong = parts2[2],
                            STTBTP = parts2[3],
                            MaMayCanBTP = parts2[4],
                            ThePhieuSLId = parts2[5]
                        });
                    }
                }

                // Loại bỏ các phiếu có ID trùng lặp
                var seenIds = new HashSet<string>();
                var uniqueThongTinPhieus = new List<ThongTinPhieuRemove>();
                foreach (var item in thongTinPhieus)
                {
                    if (seenIds.Add(item.ThePhieuSLId))
                    {
                        uniqueThongTinPhieus.Add(item);
                    }
                }

                thongTinPhieus = uniqueThongTinPhieus;

                // Kiểm tra tính hợp lệ của các thông tin phiếu
                foreach (var item in thongTinPhieus)
                {
                    if (item.STT <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }

                    if (ngay == default || string.IsNullOrEmpty(item.MaXuong))
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Vui lòng nhập đầy đủ thông tin."
                        });
                    }
                }

                var userName = HttpContext.Session.GetString("Username");

                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var item in thongTinPhieus)
                    {
                        // Tạo model để ghi chú phiếu sản lượng fillet
                        var modelThePhieuSLFillet = new ThePhieuSanLuongFillet
                        {
                            GhiChu = $"Người thực hiện: {AppViewModels.AppViewModel.Instance.UserName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Trọng Lượng Nhận:  , Trọng Lượng Trả: ,loại: XOA"
                        };

                        // Tạo model để ghi chú phiếu cân BTP fillet
                        var modelBTPFillet = new PhieuCanBTPFilletv2
                        {
                            STT = int.Parse(item.STTBTP) * -1,
                            TrongLuong = 0,
                            TrongLuongTare = 0,
                            GhiChu = $"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, loại: XOA"
                        };

                        // Lấy danh sách các phiếu cân cần xóa theo ID phiếu sản lượng
                        var listPhieuCanRemoveByThePhieuSLId = await GetAllByThePhieuSLIds(item.ThePhieuSLId);
                        var phieuCanTPFilletv212List = (await Task.WhenAll(listPhieuCanRemoveByThePhieuSLId.Select(async i => new PhieuCanTPFilletv2
                        {
                            STT = i.STT * -1,
                            MaNhanVien = i.MaNhanVien,
                            MaMayCan = i.MaMayCan,
                            TrongLuongTra = 0,
                            MaXuong = item.MaXuong,
                            GhiChu = $"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, loại: XOA"
                        }))).ToList();

                        // Gửi yêu cầu xóa phiếu sản lượng fillet
                        var helperThePhieuSLFilletUrl = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var apiThePhieuSLFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/ThePhieuSanLuongFillets/Delete_XLPC/{item.ThePhieuSLId}";
                        var jsonContentThePhieuSLFillet = new StringContent(JsonConvert.SerializeObject(new Tuple<string>(JsonConvert.SerializeObject(modelThePhieuSLFillet))), Encoding.UTF8, "application/json");
                        var responseThePhieuSLFillet = await helperThePhieuSLFilletUrl.PostAsync(HttpContext, apiThePhieuSLFilletUrl, jsonContentThePhieuSLFillet);

                        // Kiểm tra phản hồi của yêu cầu xóa phiếu sản lượng fillet
                        if (!responseThePhieuSLFillet.Success)
                        {
                            transactionScope.Dispose();
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = responseThePhieuSLFillet.Message
                            });
                        }

                        // Gửi yêu cầu xóa phiếu cân BTP fillet
                        var helperBTPFilletUrl = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var apiBTPFilletUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/Delete/{item.STTBTP}/{ngay.ToString("yyyy-MM-dd")}/{item.MaMayCanBTP}/{item.MaXuong}";
                        var jsonContentBTPFillet = new StringContent(JsonConvert.SerializeObject(new Tuple<string>(JsonConvert.SerializeObject(modelBTPFillet))), Encoding.UTF8, "application/json");
                        var responseBTPFillet = await helperBTPFilletUrl.PostAsync(HttpContext, apiBTPFilletUrl, jsonContentBTPFillet);

                        // Kiểm tra phản hồi của yêu cầu xóa phiếu cân BTP fillet
                        if (!responseBTPFillet.Success)
                        {
                            transactionScope.Dispose();
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = responseBTPFillet.Message
                            });
                        }

                        // Gửi yêu cầu xóa các phiếu cân theo danh sách
                        var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        foreach (var phieuCan in phieuCanTPFilletv212List)
                        {
                            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/Delete/{phieuCan.STT * -1}/{ngay.ToString("yyyy-MM-dd")}/{phieuCan.MaMayCan}/{phieuCan.MaXuong}";
                            var jsonContent = new StringContent(JsonConvert.SerializeObject(new Tuple<string>(JsonConvert.SerializeObject(phieuCan))), Encoding.UTF8, "application/json");
                            var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                            // Kiểm tra phản hồi của yêu cầu xóa phiếu cân
                            if (!response.Success)
                            {
                                transactionScope.Dispose();
                                return Json(new
                                {
                                    isSuccess = false,
                                    Messages = response.Message
                                });
                            }
                        }
                    }

                    // Hoàn tất giao dịch nếu không có lỗi xảy ra
                    transactionScope.Complete();
                }

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Xóa toàn bộ phiếu cân theo Phiếu SL Fillet hoàn tất!."
                });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Đã xảy ra lỗi: " + ex.Message
                });
            }
        }



        #endregion
        #endregion
        #region Định Hình
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Định Hình", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Định Hình")]
        public async Task<IActionResult> DoInsert_DinhHinh(DateTime dateTime, string xuongId, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maMau, string maNhanVien, string maMayLangDa, decimal trongLuongNhan, decimal dinhMuc, bool caTra,string maNhanVienPV)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/Insert_XLPC";
            var apiPhieuCanBTPDinhHinhUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/Insert_XLPC";

            try
            {
                if (maLo == null)
                {
                    maLo = "LoTest";
                }
                // Kiểm tra và xác thực dữ liệu đầu vào
                if (string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuongNhan.ToString()) || string.IsNullOrEmpty(dinhMuc.ToString()) || string.IsNullOrEmpty(caTra.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                var data = await GetAllsWithDateAndXuongDinhHinh(dateTime, xuongId);
                var stt = data.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
                           .Where(x => x.STT != null)
                           .Select(x => Math.Abs(x.STT))
                           .DefaultIfEmpty(0)
                           .Max();
                var sttBTP = data.Where(x => x.MaMayCanBTP == AppViewModels.AppViewModel.Instance.PCName)
                    .Where(x => x.STTBTP != null)
                    .Select(x => x.STTBTP)
                    .DefaultIfEmpty(0)
                    .Max();
                var model = new PhieuCanTPDinhHinh
                {
                    STT = stt + 1,
                    STTBTP = sttBTP + 1,
                    MaLoaiCa = maLoaiCa,
                    CaTra = false,
                    DinhMucThucTe = dinhMuc,
                    Gio = gio,
                    Ngay = dateTime,
                    MaLo = maLo,
                    MaSize = maSize,
                    MaMau = maMau,
                    MaThe = "0",
                    TrongLuongNhan = trongLuongNhan,
                    MaXuong = xuongId,
                    MaThanhPham = maThanhPham,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaMayCanBTP = AppViewModels.AppViewModel.Instance.PCName,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaNhanVien = maNhanVien,
                    DinhMucYeuCau = 0,
                    SuDung = true,
                    ChiSanLuong = false,
                    TrongLuongTare = 0,
                    TrongLuongBu = 0,
                    IsOffline = false,
                    MaNhanVienPhucVu = maNhanVienPV,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THÊM,",
                };
                model.TrongLuongTra = model.TrongLuongNhan / model.DinhMucThucTe;

                // Chuẩn bị dữ liệu cho PhieuCanBTPDinhHinh
                var modelBTP = new PhieuCanBTPDinhHinh
                {
                    STT = (int)(sttBTP + 1),
                    Ngay = dateTime,
                    Gio = gio,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaLoaiCa = maLoaiCa,
                    MaMau = maMau,
                    MaSize = maSize,
                    MaThanhPham = maThanhPham,
                    MaLo = maLo,
                    MaThe = "0",
                    MaNhanVien = maNhanVien,
                    MaMayLangDa = maMayLangDa,
                    TrongLuong = trongLuongNhan,
                    IsEnabled = true,
                    MaXuong = xuongId,
                    CaTra = caTra,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: THÊM,",
                    ChiSanLuong = false,
                    TrongLuongTare = 0,
                    TrongLuongBu = 0,
                    IsOffline = false,
                };


                using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Thực hiện insert PhieuCanTPDinhHinh
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                // Thực hiện insert PhieuCanBTPDinhHinh nếu insert PhieuCanTPDinhHinh thành công
                if (responseTP.Success)
                {
                    var dataTupleBTP = new Tuple<string>(JsonConvert.SerializeObject(modelBTP));
                    var jsonContentBTP = new StringContent(JsonConvert.SerializeObject(dataTupleBTP), Encoding.UTF8, "application/json");
                    var responseBTP = await helper.PostAsync(HttpContext, apiPhieuCanBTPDinhHinhUrl, jsonContentBTP);

                    if (responseBTP.Success)
                    {
                        // Hoàn thành transaction nếu cả hai insert đều thành công
                        transactionScope.Complete();

                        return Json(new
                        {
                            isSuccess = true,
                            Messages = "Đã thêm 2 phiếu ở BTP Định Hình và TP Định Hình"
                        });
                    }
                    else
                    {
                        // Quay lại trạng thái trước transaction nếu insert PhieuCanBTPDinhHinh thất bại
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Thêm Phiếu Cân BTP Định Hình thất bại: " + responseBTP.Message
                        });
                    }
                }
                else
                {
                    // Quay lại trạng thái trước transaction nếu insert PhieuCanTPDinhHinh thất bại
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Thêm Phiếu Cân TP Định Hình thất bại: " + responseTP.Message
                    });
                }
            }
            catch (Exception ex)
            {
                // Quay lại trạng thái trước transactionh nếu có lỗi xảy ra
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Đã xảy ra lỗi: " + ex.Message.ToString()
                });
            }
        }

        public async Task<IActionResult> GetsByMa(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            string maNhanVien = parts[0];
            int stt = int.Parse(parts[1]);
            string maMayCan = parts[2];
            string maXuong = parts[3];
            int sttBTP = int.Parse(parts[4]);
            string maMayCanBTP = parts[5];
            if (stt <= 0 || sttBTP <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maNhanVien}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanTPDinhHinh>(HttpContext, apiUrl);
                var apiBTPUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetsByMa/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maNhanVien}/{maMayCanBTP}/{maXuong}";
                using var helperBTP = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var itemBTP = await helperBTP.GetAsync<PhieuCanBTPDinhHinh>(HttpContext, apiBTPUrl);
                if (item != null && itemBTP != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item?.STT,
                        STTBTP = item?.STTBTP,
                        MaLoaiCa = item?.MaLoaiCa,
                        CaTra = item?.CaTra,
                        DinhMucThucTe = item?.DinhMucThucTe,
                        Gio = item?.Gio,
                        Ngay = item?.Ngay,
                        MaLo = item?.MaLo,
                        MaSize = item?.MaSize,
                        MaMau = item?.MaMau,
                        MaThe = item?.MaThe,
                        TrongLuongNhan = item?.TrongLuongNhan,
                        MaXuong = item?.MaXuong,
                        MaThanhPham = item?.MaThanhPham,
                        MaUserCan = item?.MaUserCan,
                        MaMayCanBTP = item?.MaMayCanBTP,
                        MaMayCan = item?.MaMayCan,
                        MaNhanVien = item?.MaNhanVien,
                        DinhMucYeuCau = item?.DinhMucYeuCau,
                        SuDung = item?.SuDung,
                        ChiSanLuong = item?.ChiSanLuong,
                        TrongLuongTare = item?.TrongLuongTare,
                        TrongLuongBu = item?.TrongLuongBu,
                        IsOffline = item?.IsOffline,
                        GhiChu = item?.GhiChu,
                        MaMayLangDa = itemBTP?.MaMayLangDa,
                        MaNhanVienPhucVu = item?.MaNhanVienPhucVu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Định Hình", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Định Hình")]
        public async Task<IActionResult> DoUpDate_XLPC(int stt, int sttBTP, string maMayCan, string maMayCanBTP, DateTime ngay, string maXuong, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maMau, string maNhanVien, string maMayLangDa, decimal trongLuongNhan, decimal dinhMuc, bool caTra,string maNhanVienPV)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/Update_XLPC/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            var apiBTPUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/Update_XLPC/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";
            try
            {
                if (stt <= 0 || sttBTP <= 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                    });
                }
                if (maLo == null)
                {
                    maLo = "LoTest";
                }
                // Kiểm tra dữ liệu đầu vào
                // Kiểm tra và xác thực dữ liệu đầu vào
                if (string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuongNhan.ToString()) || string.IsNullOrEmpty(dinhMuc.ToString()) || string.IsNullOrEmpty(caTra.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanTPDinhHinh
                {
                    MaLoaiCa = maLoaiCa,
                    CaTra = false,
                    DinhMucThucTe = dinhMuc,
                    Gio = gio,
                    MaLo = maLo,
                    MaSize = maSize,
                    MaMau = maMau,
                    TrongLuongNhan = trongLuongNhan,
                    MaThanhPham = maThanhPham,
                    MaNhanVien = maNhanVien,
                    MaNhanVienPhucVu = maNhanVienPV,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: SỬA",
                };
                model.TrongLuongTra = model.TrongLuongNhan / model.DinhMucThucTe;
                // Chuẩn bị dữ liệu cho PhieuCanBTPDinhHinh
                var modelBTP = new PhieuCanBTPDinhHinh
                {
                    Gio = gio,
                    MaLoaiCa = maLoaiCa,
                    MaMau = maMau,
                    MaSize = maSize,
                    MaThanhPham = maThanhPham,
                    MaLo = maLo,
                    MaNhanVien = maNhanVien,
                    MaMayLangDa = maMayLangDa,
                    TrongLuong = trongLuongNhan,
                    MaXuong = maXuong,
                    CaTra = caTra,
                    GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: SỬA",
                };


                using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Thực hiện insert PhieuCanTPDinhHinh
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                // Thực hiện insert PhieuCanBTPDinhHinh nếu insert PhieuCanTPDinhHinh thành công
                if (responseTP.Success)
                {
                    var dataTupleBTP = new Tuple<string>(JsonConvert.SerializeObject(modelBTP));
                    var jsonContentBTP = new StringContent(JsonConvert.SerializeObject(dataTupleBTP), Encoding.UTF8, "application/json");
                    var responseBTP = await helper.PostAsync(HttpContext, apiBTPUrl, jsonContentBTP);

                    if (responseBTP.Success)
                    {
                        // Hoàn thành transaction nếu cả hai update đều thành công
                        transactionScope.Complete();

                        return Json(new
                        {
                            isSuccess = true,
                            Messages = "Đã sửa 2 phiếu ở BTP Định Hình và TP Định Hình"
                        });
                    }
                    else
                    {
                        // Quay lại trạng thái trước transaction nếu update PhieuCanBTPDinhHinh thất bại
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Sửa Phiếu Cân BTP Định Hình thất bại: " + responseBTP.Message
                        });
                    }
                }
                else
                {
                    // Quay lại trạng thái trước transaction nếu update PhieuCanTPDinhHinh thất bại
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Sửa Phiếu Cân TP Định Hình thất bại: " + responseTP.Message
                    });
                }

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

        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Định Hình", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Định Hình")]
        public async Task<IActionResult> DoDelete_DinhHinh(string listInfoPhieuCan, DateTime ngay)
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
                    // Khởi tạo các biến từ mảng con
                    string maNhanVien = parts[0];
                    int stt = int.Parse(parts[1]);
                    string maMayCan = parts[2];
                    string maXuong = parts[3];
                    int sttBTP = int.Parse(parts[4]);
                    string maMayCanBTP = parts[5];
                    if (stt <= 0 || sttBTP <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTPDinhHinh
                    {
                        STT = stt * -1,
                        STTBTP = sttBTP * -1,
                        TrongLuongNhan = 0,
                        TrongLuongTra = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };
                    // Chuẩn bị dữ liệu cho PhieuCanBTPDinhHinh
                    var modelBTP = new PhieuCanBTPDinhHinh
                    {
                        STT = sttBTP * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/Delete_DinhHinh/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                    var apiBTPUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/Delete_DinhHinh/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";

                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (responseTP.Success)
                    {
                        // Thực hiện insert PhieuCanBTPDinhHinh nếu insert PhieuCanTPDinhHinh thành công
                        var dataTupleBTP = new Tuple<string>(JsonConvert.SerializeObject(modelBTP));
                        var jsonContentBTP = new StringContent(JsonConvert.SerializeObject(dataTupleBTP), Encoding.UTF8, "application/json");
                        var responseBTP = await helper.PostAsync(HttpContext, apiBTPUrl, jsonContentBTP);

                        if (!responseBTP.Success)
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = "Sửa Phiếu Cân BTP Định Hình thất bại: " + responseBTP.Message
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Sửa Phiếu Cân TP Định Hình thất bại: " + responseTP.Message
                        });
                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã sửa 2 phiếu ở BTP Định Hình và TP Định Hình"
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


        public async Task<IActionResult> CheckQuyenXLPC(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Định Hình" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Định Hình" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Định Hình" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Định Hình" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Định Hình", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Định Hình")]
        public async Task<IActionResult> ChuyenXuong_DinhHinh(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    // Khởi tạo các biến từ mảng con
                    string maNhanVien = parts[0];
                    int stt = int.Parse(parts[1]);
                    string maMayCan = parts[2];
                    string maXuong = parts[3];
                    int sttBTP = int.Parse(parts[4]);
                    string maMayCanBTP = parts[5];
                    if (stt <= 0 || sttBTP <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTPDinhHinh
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };
                    // Chuẩn bị dữ liệu cho PhieuCanBTPDinhHinh
                    var modelBTP = new PhieuCanBTPDinhHinh
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/ChuyenXuong_DinhHinh/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                    var apiBTPUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/ChuyenXuong_DinhHinh/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";

                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (responseTP.Success)
                    {
                        // Thực hiện insert PhieuCanBTPDinhHinh nếu insert PhieuCanTPDinhHinh thành công
                        var dataTupleBTP = new Tuple<string>(JsonConvert.SerializeObject(modelBTP));
                        var jsonContentBTP = new StringContent(JsonConvert.SerializeObject(dataTupleBTP), Encoding.UTF8, "application/json");
                        var responseBTP = await helper.PostAsync(HttpContext, apiBTPUrl, jsonContentBTP);

                        if (!responseBTP.Success)
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = "Chuyển Xưởng Phiếu Cân BTP Định Hình thất bại: " + responseBTP.Message
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Xưởng Phiếu Cân TP Định Hình thất bại: " + responseTP.Message
                        });
                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Xưởng các phiếu  đã chọn ở BTP Định Hình và TP Định Hình! Vui lòng đổi xưởng để kiểm tra!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Định Hình", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Định Hình")]
        public async Task<IActionResult> ChuyenSize_DinhHinh(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    // Khởi tạo các biến từ mảng con
                    string maNhanVien = parts[0];
                    int stt = int.Parse(parts[1]);
                    string maMayCan = parts[2];
                    string maXuong = parts[3];
                    int sttBTP = int.Parse(parts[4]);
                    string maMayCanBTP = parts[5];
                    if (stt <= 0 || sttBTP <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTPDinhHinh
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };
                    // Chuẩn bị dữ liệu cho PhieuCanBTPDinhHinh
                    var modelBTP = new PhieuCanBTPDinhHinh
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/ChuyenSize_XLPC/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                    var apiBTPUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/ChuyenSize_XLPC/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";

                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (responseTP.Success)
                    {
                        // Thực hiện insert PhieuCanBTPDinhHinh nếu insert PhieuCanTPDinhHinh thành công
                        var dataTupleBTP = new Tuple<string>(JsonConvert.SerializeObject(modelBTP));
                        var jsonContentBTP = new StringContent(JsonConvert.SerializeObject(dataTupleBTP), Encoding.UTF8, "application/json");
                        var responseBTP = await helper.PostAsync(HttpContext, apiBTPUrl, jsonContentBTP);

                        if (!responseBTP.Success)
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = "Chuyển Size Phiếu Cân BTP Định Hình thất bại: " + responseBTP.Message
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Size Phiếu Cân TP Định Hình thất bại: " + responseTP.Message
                        });
                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Size các phiếu  đã chọn ở BTP Định Hình và TP Định Hình!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Định Hình", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Định Hình")]
        public async Task<IActionResult> ChuyenThanhPham_DinhHinh(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    // Khởi tạo các biến từ mảng con
                    string maNhanVien = parts[0];
                    int stt = int.Parse(parts[1]);
                    string maMayCan = parts[2];
                    string maXuong = parts[3];
                    int sttBTP = int.Parse(parts[4]);
                    string maMayCanBTP = parts[5];
                    if (stt <= 0 || sttBTP <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTPDinhHinh
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };
                    // Chuẩn bị dữ liệu cho PhieuCanBTPDinhHinh
                    var modelBTP = new PhieuCanBTPDinhHinh
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/ChuyenThanhPham_XLPC/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                    var apiBTPUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/ChuyenThanhPham_XLPC/{sttBTP}/{ngay.ToString("yyyy-MM-dd")}/{maMayCanBTP}/{maXuong}";

                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (responseTP.Success)
                    {
                        // Thực hiện insert PhieuCanBTPDinhHinh nếu insert PhieuCanTPDinhHinh thành công
                        var dataTupleBTP = new Tuple<string>(JsonConvert.SerializeObject(modelBTP));
                        var jsonContentBTP = new StringContent(JsonConvert.SerializeObject(dataTupleBTP), Encoding.UTF8, "application/json");
                        var responseBTP = await helper.PostAsync(HttpContext, apiBTPUrl, jsonContentBTP);

                        if (!responseBTP.Success)
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = "Chuyển Thành Phẩm Phiếu Cân BTP Định Hình thất bại: " + responseBTP.Message
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Thành Phẩm Phiếu Cân TP Định Hình thất bại: " + responseTP.Message
                        });
                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Thành Phẩm các phiếu  đã chọn ở BTP Định Hình và TP Định Hình!"
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
        #region Phụ Xếp Khuôn
        public async Task<IActionResult> CreatDefautNewPhuXepKhuon(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongPhuXepKhuon(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn")]
        public async Task<IActionResult> DoInsert_PhuXepKhuon(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maMau, string maKhuVuc, string maNhanVien, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanPhuXepKhuon
                {
                    STT = stt,
                    Ngay = dateTime,
                    Gio = gio,
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaMau = maMau,
                    MaKhuVuc = maKhuVuc,
                    MaNhanVien = maNhanVien,
                    MaNhom = "0",
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaXuong = xuongId,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    TrongLuong = trongLuong,
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
        public async Task<IActionResult> GetsByMa_PhuXepKhuon(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanPhuXepKhuon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaLo = item.MaLo,
                        MaLoaiCa = item.MaLoaiCa,
                        MaThanhPham = item.MaThanhPham,
                        MaSize = item.MaSize,
                        MaMau = item.MaMau,
                        MaKhuVuc = item.MaKhuVuc,
                        MaNhanVien = item.MaNhanVien,
                        MaNhom = item.MaNhom,
                        MaUserCan = item.MaUserCan,
                        MaXuong = item.MaXuong,
                        MaMayCan = item.MaMayCan,
                        TrongLuong = item.TrongLuong,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn")]
        public async Task<IActionResult> DoUpDate_PhuXepKhuon(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maMau, string maKhuVuc, string maNhanVien, decimal trongLuong, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanPhuXepKhuon
                {
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaMau = maMau,
                    MaKhuVuc = maKhuVuc,
                    MaNhanVien = maNhanVien,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn")]
        public async Task<IActionResult> DoDelete_PhuXepKhuon(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuXepKhuon
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenPhuXepKhuon(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn")]
        public async Task<IActionResult> ChuyenXuong_PhuXepKhuon(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuXepKhuon
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn")]
        public async Task<IActionResult> ChuyenSize_PhuXepKhuon(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuXepKhuon
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Xếp Khuôn")]
        public async Task<IActionResult> ChuyenThanhPham_PhuXepKhuon(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuXepKhuon
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Chính Xếp Khuôn
        public async Task<IActionResult> CreatDefautNewChinhXepKhuon(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongChinhXepKhuon(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn")]
        public async Task<IActionResult> DoInsert_ChinhXepKhuon(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maThanhPham, string maChieuXa, string maSize, string maMau, string maChatLuong, string maKhuVuc, string maCoiTam, string maCoiChinh, bool taiChe, bool chuyenXuong, int thoiGianQuay, decimal trongLuong, TimeSpan thoiGianBatDauQuay, TimeSpan thoiGianRaCoi, string maNhanVien, string maNhanVienPV)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanChinhXepKhuon
                {
                    STT = stt,
                    Ngay = DateTime.Now,
                    Gio = gio,
                    MaCoiTam = maCoiTam,
                    MaCoiChinh = maCoiChinh,
                    DaQuay = false,
                    ThoiGianBatDauQuay = thoiGianBatDauQuay,
                    ThoiGianRaCoi = thoiGianRaCoi,
                    Forced = false,
                    ThoiGianQuay = thoiGianQuay,
                    TrongLuong = trongLuong,
                    MaLo = maLo,
                    MaLoaiCa = "A",
                    MaSizeChinh = maSize,
                    MaMau = maMau,
                    MaChatLuong = maChatLuong,
                    MaThanhPhamChinh = maThanhPham,
                    MaKhuVuc = maKhuVuc,
                    MaNhanVien = maNhanVien,
                    MaNhom = "0",
                    MaChieuXa = maChieuXa,
                    TaiChe = taiChe,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaXuong = xuongId,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    LuotQuay = 0,
                    ChuyenXuong = chuyenXuong,
                    MaNhanVienPvPhanCo = maNhanVienPV,
                    TrongLuongTare = 0,
                    NgayNguyenLieu = dateTime,
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
        public async Task<IActionResult> GetsByMa_ChinhXepKhuon(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanChinhXepKhuon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaCoiTam = item.MaCoiTam,
                        MaCoiChinh = item.MaCoiChinh,
                        DaQuay = item.DaQuay,
                        ThoiGianBatDauQuay = item.ThoiGianBatDauQuay,
                        ThoiGianRaCoi = item.ThoiGianRaCoi,
                        Forced = item.Forced,
                        ThoiGianQuay = item.ThoiGianQuay,
                        TrongLuong = item.TrongLuong,
                        MaLo = item.MaLo,
                        MaLoaiCa = item.MaLoaiCa,
                        MaSizeChinh = item.MaSizeChinh,
                        MaMau = item.MaMau,
                        MaChatLuong = item.MaChatLuong,
                        MaThanhPhamChinh = item.MaThanhPhamChinh,
                        MaKhuVuc = item.MaKhuVuc,
                        MaNhanVien = item.MaNhanVien,
                        MaNhom = item.MaNhom,
                        MaChieuXa = item.MaChieuXa,
                        TaiChe = item.TaiChe,
                        MaUserCan = item.MaUserCan,
                        MaXuong = item.MaXuong,
                        MaMayCan = item.MaMayCan,
                        LuotQuay = item.LuotQuay,
                        ChuyenXuong = item.ChuyenXuong,
                        MaNhanVienPvPhanCo = item.MaNhanVienPvPhanCo,
                        TrongLuongTare = item.TrongLuongTare,
                        NgayNguyenLieu = item.NgayNguyenLieu,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn")]
        public async Task<IActionResult> DoUpDate_ChinhXepKhuon(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maThanhPham, string maChieuXa, string maSize, string maMau, string maChatLuong, string maKhuVuc, string maCoiTam, string maCoiChinh, bool taiChe, bool chuyenXuong, int thoiGianQuay, decimal trongLuong, TimeSpan thoiGianBatDauQuay, TimeSpan thoiGianRaCoi, string maNhanVien, string maNhanVienPV, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanChinhXepKhuon
                {
                    MaLo = maLo,
                    MaThanhPhamChinh = maThanhPham,
                    MaSizeChinh = maSize,
                    MaMau = maMau,
                    MaChatLuong = maChatLuong,
                    MaKhuVuc = maKhuVuc,
                    MaChieuXa = maChieuXa,
                    MaCoiTam = maCoiTam,
                    MaCoiChinh = maCoiChinh,
                    TaiChe = taiChe,
                    ChuyenXuong = chuyenXuong,
                    ThoiGianQuay = thoiGianQuay,
                    MaNhanVien = maNhanVien,
                    ThoiGianBatDauQuay = thoiGianBatDauQuay,
                    MaNhanVienPvPhanCo = maNhanVienPV,
                    ThoiGianRaCoi = thoiGianRaCoi,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn")]
        public async Task<IActionResult> DoDelete_ChinhXepKhuon(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanChinhXepKhuon
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenChinhXepKhuon(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn")]
        public async Task<IActionResult> ChuyenXuong_ChinhXepKhuon(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanChinhXepKhuon
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn")]
        public async Task<IActionResult> ChuyenSize_ChinhXepKhuon(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanChinhXepKhuon
                    {
                        MaSizeChinh = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Chính Xếp Khuôn")]
        public async Task<IActionResult> ChuyenThanhPham_ChinhXepKhuon(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanChinhXepKhuon
                    {
                        MaThanhPhamChinh = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Sau Xếp Khuôn
        public async Task<IActionResult> CreatDefautNewSauXepKhuon(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongSauXepKhuon(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = stt,
                        IsTam = false
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn")]
        public async Task<IActionResult> DoInsert_SauXepKhuon(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maCoi, string maChieuXa, int tyLeMaBang, string maNhanVien, decimal trongLuong, bool isTam)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maChieuXa) || string.IsNullOrEmpty(tyLeMaBang.ToString()) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(isTam.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanSauXepKhuon
                {
                    STT = stt,
                    Ngay = DateTime.Now,
                    NgayNguyenLieu = dateTime,
                    Gio = gio,
                    MaXuong = xuongId,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaChieuXa = maChieuXa,
                    MaCoi = maCoi,
                    TrongLuong = trongLuong,
                    TrongLuongTare = 0,
                    ChiTietLuotRaCoiId = 0,
                    MaNhanVien = maNhanVien,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaThe = "0",
                    TyLeMaBang = tyLeMaBang,
                    IsTam = isTam,
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
        public async Task<IActionResult> GetsByMa_SauXepKhuon(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanSauXepKhuon>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        NgayNguyenLieu = item.NgayNguyenLieu,
                        Gio = item.Gio,
                        MaXuong = item.MaXuong,
                        MaMayCan = item.MaMayCan,
                        MaLo = item.MaLo,
                        MaLoaiCa = item.MaLoaiCa,
                        MaThanhPham = item.MaThanhPham,
                        MaSize = item.MaSize,
                        MaChieuXa = item.MaChieuXa,
                        MaCoi = item.MaCoi,
                        TrongLuong = item.TrongLuong,
                        TrongLuongTare = item.TrongLuongTare,
                        ChiTietLuotRaCoiId = item.ChiTietLuotRaCoiId,
                        MaNhanVien = item.MaNhanVien,
                        MaUserCan = item.MaUserCan,
                        MaThe = item.MaThe,
                        TyLeMaBang = item.TyLeMaBang,
                        IsTam = item.IsTam,
                        GhiChu = item.GhiChu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn")]
        public async Task<IActionResult> DoUpDate_SauXepKhuon(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maCoi, string maChieuXa, int tyLeMaBang, string maNhanVien, decimal trongLuong, bool isTam, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maChieuXa) || string.IsNullOrEmpty(tyLeMaBang.ToString()) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(isTam.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanSauXepKhuon
                {
                    MaLo = maLo,
                    MaNhanVien = maNhanVien,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaCoi = maCoi,
                    MaLoaiCa = maLoaiCa,
                    MaChieuXa = maChieuXa,
                    TyLeMaBang = tyLeMaBang,
                    IsTam = isTam,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn")]
        public async Task<IActionResult> DoDelete_SauXepKhuon(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanSauXepKhuon
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenSauXepKhuon(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn")]
        public async Task<IActionResult> ChuyenXuong_SauXepKhuon(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanSauXepKhuon
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn")]
        public async Task<IActionResult> ChuyenSize_SauXepKhuon(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanSauXepKhuon
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Sau Xếp Khuôn")]
        public async Task<IActionResult> ChuyenThanhPham_SauXepKhuon(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanSauXepKhuon
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSauXepKhuons/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Xếp Khuôn KHC
        public async Task<IActionResult> CreatDefautNewXepKhuonKHC(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongXepKhuonKHC(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn")]
        public async Task<IActionResult> DoInsert_XepKhuonKHC(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maKhachHang, string maCoiTam, string maNhanVien, decimal trongLuong, int block)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maKhachHang.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(block.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanXepKhuonKHC
                {
                    STT = stt,
                    Ngay = dateTime,
                    Gio = gio,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaXuong = xuongId,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaKhachHang = maKhachHang,
                    MaCoiTam = maCoiTam,
                    TrongLuong = trongLuong,
                    DaXacNhan = false,
                    MaNhanVien = maNhanVien,
                    Block = block,
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
        public async Task<IActionResult> GetsByMa_XepKhuonKHC(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanXepKhuonKHC>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaUserCan = item.MaUserCan,
                        MaXuong = item.MaXuong,
                        MaMayCan = item.MaMayCan,
                        MaLo = item.MaLo,
                        MaLoaiCa = item.MaLoaiCa,
                        MaThanhPham = item.MaThanhPham,
                        MaSize = item.MaSize,
                        MaKhachHang = item.MaKhachHang,
                        MaCoiTam = item.MaCoiTam,
                        TrongLuong = item.TrongLuong,
                        DaXacNhan = item.DaXacNhan,
                        MaNhanVien = item.MaNhanVien,
                        Block = item.Block,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn")]
        public async Task<IActionResult> DoUpDate_XepKhuonKHC(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maSize, string maKhachHang, string maCoiTam, string maNhanVien, decimal trongLuong, int block, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maKhachHang.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(block.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanXepKhuonKHC
                {
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaKhachHang = maKhachHang,
                    MaCoiTam = maCoiTam,
                    MaNhanVien = maNhanVien,
                    Block = block,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn")]
        public async Task<IActionResult> DoDelete_XepKhuonKHC(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonKHC
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenXepKhuonKHC(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn")]
        public async Task<IActionResult> ChuyenXuong_XepKhuonKHC(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonKHC
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn")]
        public async Task<IActionResult> ChuyenSize_XepKhuonKHC(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonKHC
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân KXL Xếp Khuôn")]
        public async Task<IActionResult> ChuyenThanhPham_XepKhuonKHC(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonKHC
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Xếp Khuôn Block
        public async Task<IActionResult> CreatDefautNewXepKhuonBlock(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongXepKhuonBlock(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = stt,
                        MaMayCan = AppViewModels.AppViewModel.Instance.PCName
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn")]
        public async Task<IActionResult> DoInsert_XepKhuonBlock(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maThanhPham, string maSize, string maChatLuong, string maNet, string maChieuXa, string maKhachHang, string maMau, string maCongDoan, string maNhanVien, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maChatLuong.ToString()) || string.IsNullOrEmpty(maNet.ToString()) || string.IsNullOrEmpty(maChieuXa.ToString()) || string.IsNullOrEmpty(maKhachHang.ToString()) || string.IsNullOrEmpty(maMau.ToString()) || string.IsNullOrEmpty(maCongDoan.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanXepKhuonBlock
                {
                    STT = stt,
                    Ngay = dateTime,
                    MaXuong = xuongId,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    Gio = gio,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaLo = maLo,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaChatLuong = maChatLuong,
                    MaNet = maNet,
                    MaChieuXa = maChieuXa,
                    MaKhachHang = maKhachHang,
                    MaMau = maMau,
                    MaCongDoan = maCongDoan,
                    MaNhanVien = maNhanVien,
                    TrongLuong = trongLuong,
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
        public async Task<IActionResult> GetsByMa_XepKhuonBlock(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanXepKhuonBlock>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        MaXuong = item.MaXuong,
                        MaMayCan = item.MaMayCan,
                        Gio = item.Gio,
                        MaUserCan = item.MaUserCan,
                        MaLo = item.MaLo,
                        MaThanhPham = item.MaThanhPham,
                        MaSize = item.MaSize,
                        MaChatLuong = item.MaChatLuong,
                        MaNet = item.MaNet,
                        MaChieuXa = item.MaChieuXa,
                        MaKhachHang = item.MaKhachHang,
                        MaMau = item.MaMau,
                        MaCongDoan = item.MaCongDoan,
                        MaNhanVien = item.MaNhanVien,
                        TrongLuong = item.TrongLuong,
                        GhiChu = item.GhiChu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn")]
        public async Task<IActionResult> DoUpDate_XepKhuonBlock(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maThanhPham, string maSize, string maChatLuong, string maNet, string maChieuXa, string maKhachHang, string maMau, string maCongDoan, string maNhanVien, decimal trongLuong, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo.ToString()) || string.IsNullOrEmpty(maThanhPham.ToString()) || string.IsNullOrEmpty(maSize.ToString()) || string.IsNullOrEmpty(maChatLuong.ToString()) || string.IsNullOrEmpty(maNet.ToString()) || string.IsNullOrEmpty(maChieuXa.ToString()) || string.IsNullOrEmpty(maKhachHang.ToString()) || string.IsNullOrEmpty(maMau.ToString()) || string.IsNullOrEmpty(maCongDoan.ToString()) || string.IsNullOrEmpty(maNhanVien.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(maMayCan.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanXepKhuonBlock
                {
                    MaLo = maLo,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaChatLuong = maChatLuong,
                    MaNet = maNet,
                    MaChieuXa = maChieuXa,
                    MaKhachHang = maKhachHang,
                    MaMau = maMau,
                    MaCongDoan = maCongDoan,
                    MaNhanVien = maNhanVien,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn")]
        public async Task<IActionResult> DoDelete_XepKhuonBlock(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonBlock
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenXepKhuonBlock(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn")]
        public async Task<IActionResult> ChuyenXuong_XepKhuonBlock(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonBlock
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn")]
        public async Task<IActionResult> ChuyenSize_XepKhuonBlock(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonBlock
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Block Xếp Khuôn")]
        public async Task<IActionResult> ChuyenThanhPham_XepKhuonBlock(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanXepKhuonBlock
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Phụ Gia
        public async Task<IActionResult> CreatDefautNewPhuGia(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongPhuGia(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = stt,
                        MaMayCan = AppViewModels.AppViewModel.Instance.PCName
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia")]
        public async Task<IActionResult> DoInsert_PhuGia(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maSanPham, string maCoi, string maCongThuc, string maLyDo, decimal trongLuongNguyenLieu, decimal trongLuong, int block, string maNhanVien, bool khoa)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maCongThuc) || string.IsNullOrEmpty(maLyDo) || string.IsNullOrEmpty(trongLuongNguyenLieu.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(block.ToString()) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(khoa.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PD_PhieuCan
                {
                    STT = stt,
                    Ngay = dateTime,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaXuong = xuongId,
                    Gio = gio,
                    MaSanPham = maSanPham,
                    TrongLuongNguyenLieu = trongLuongNguyenLieu,
                    TrongLuong = trongLuong,
                    MaCoi = maCoi,
                    MaCongThuc = maCongThuc,
                    SuDung = true,
                    MaLyDo = maLyDo,
                    CreateDateTime = DateTime.Now,
                    CreateBy = userName ?? "",
                    ModifiedDateTime = DateTime.Now,
                    ModifiedBy = userName ?? "",
                    MaNhanVien = maNhanVien,
                    Khoa = khoa,
                    Block = block,
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
        public async Task<IActionResult> GetsByMa_PhuGia(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PD_PhieuCan>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        MaMayCan = item.MaMayCan,
                        MaXuong = item.MaXuong,
                        Gio = item.Gio,
                        MaSanPham = item.MaSanPham,
                        TrongLuongNguyenLieu = item.TrongLuongNguyenLieu,
                        TrongLuong = item.TrongLuong,
                        MaCoi = item.MaCoi,
                        MaCongThuc = item.MaCongThuc,
                        SuDung = item.SuDung,
                        MaLyDo = item.MaLyDo,
                        CreateDateTime = item.CreateDateTime,
                        CreateBy = item.CreateBy,
                        ModifiedDateTime = item.ModifiedDateTime,
                        ModifiedBy = item.ModifiedBy,
                        MaNhanVien = item.MaNhanVien,
                        Khoa = item.Khoa,
                        Block = item.Block,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia")]
        public async Task<IActionResult> DoUpDate_PhuGia(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maSanPham, string maCoi, string maCongThuc, string maLyDo, decimal trongLuongNguyenLieu, decimal trongLuong, int block, string maNhanVien, bool khoa, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maCoi) || string.IsNullOrEmpty(maCongThuc) || string.IsNullOrEmpty(maLyDo) || string.IsNullOrEmpty(trongLuongNguyenLieu.ToString()) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(block.ToString()) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(khoa.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PD_PhieuCan
                {
                    MaSanPham = maSanPham,
                    MaCoi = maCoi,
                    MaCongThuc = maCongThuc,
                    MaLyDo = maLyDo,
                    TrongLuongNguyenLieu = trongLuongNguyenLieu,
                    TrongLuong = trongLuong,
                    Block = block,
                    MaNhanVien = maNhanVien,
                    Khoa = khoa,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia")]
        public async Task<IActionResult> DoDelete_PhuGia(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PD_PhieuCan
                    {
                        STT = stt * -1,
                        TrongLuongNguyenLieu = 0,
                        TrongLuong = 0,
                        SuDung = false,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenPhuGia(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia")]
        public async Task<IActionResult> ChuyenXuong_PhuGia(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PD_PhieuCan
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCan/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        //[CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia")]
        //public async Task<IActionResult> ChuyenSize_PhuGia(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
        //{
        //    // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
        //    string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

        //    // Khởi tạo danh sách kết quả
        //    List<string[]> result = new List<string[]>();

        //    // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
        //    foreach (string subArray in subArrays)
        //    {
        //        string[] elements = subArray.Split(',');
        //        result.Add(elements);
        //    }

        //    var userName = HttpContext.Session.GetString("Username");

        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    try
        //    {
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

        //        foreach (var parts in result)
        //        {
        //            int stt = int.Parse(parts[0]);
        //            string maMayCan = parts[1];
        //            string maXuong = parts[2];

        //            if (stt <= 0)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
        //                });
        //            }
        //            // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
        //            var model = new PD_PhieuCan
        //            {
        //                MaSize = maSizeChange,
        //                GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
        //            };


        //            // Chuẩn bị URL cho các API
        //            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCans/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
        //            // Thực hiện insert PhieuCanTPDinhHinh
        //            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
        //            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //            var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

        //            if (!responseTP.Success)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = "Chuyển Size Phiếu Cân thất bại: " + responseTP.Message
        //                });

        //            }
        //        }

        //        // Hoàn thành transaction nếu tất cả các update đều thành công
        //        transactionScope.Complete();

        //        return Json(new
        //        {
        //            isSuccess = true,
        //            Messages = "Đã Chuyển Size các phiếu  đã chọn!"
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
        //[CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Gia")]
        //public async Task<IActionResult> ChuyenThanhPham_PhuGia(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
        //{
        //    // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
        //    string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

        //    // Khởi tạo danh sách kết quả
        //    List<string[]> result = new List<string[]>();

        //    // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
        //    foreach (string subArray in subArrays)
        //    {
        //        string[] elements = subArray.Split(',');
        //        result.Add(elements);
        //    }

        //    var userName = HttpContext.Session.GetString("Username");

        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    try
        //    {
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

        //        foreach (var parts in result)
        //        {
        //            int stt = int.Parse(parts[0]);
        //            string maMayCan = parts[1];
        //            string maXuong = parts[2];

        //            if (stt <= 0)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
        //                });
        //            }
        //            // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
        //            var model = new PD_PhieuCan
        //            {
        //                MaThanhPham = maThanhPhamChange,
        //                GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
        //            };


        //            // Chuẩn bị URL cho các API
        //            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PD_PhieuCans/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
        //            // Thực hiện insert PhieuCanTPDinhHinh
        //            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
        //            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //            var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

        //            if (!responseTP.Success)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = "Chuyển Thành Phẩm Phiếu Cân thất bại: " + responseTP.Message
        //                });

        //            }
        //        }

        //        // Hoàn thành transaction nếu tất cả các update đều thành công
        //        transactionScope.Complete();

        //        return Json(new
        //        {
        //            isSuccess = true,
        //            Messages = "Đã Chuyển Thành Phẩm các phiếu  đã chọn!"
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
        #endregion
        #region Tai Che
        public async Task<IActionResult> CreatDefautNewTaiChe(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongTaiChe(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Tái Chế")]
        public async Task<IActionResult> DoInsert_TaiChe(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maSize, string maChatLuong, string maLoaiCa, string maMau, string maThanhPham, string maCongViec, string maNhanVien, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maChatLuong) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanTaiChe
                {
                    STT = stt,
                    Ngay = dateTime,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaXuong = xuongId,
                    Gio = gio,
                    Id = "0",
                    MaUserCan = "Xử Lý Phiếu Cân",
                    MaLo = maLo,
                    MaSize = maSize,
                    MaChatLuong = maChatLuong,
                    MaLoaiCa = maLoaiCa,
                    MaMau = maMau,
                    MaThanhPham = maThanhPham,
                    TrongLuong = trongLuong,
                    MaNhanVien = maNhanVien,
                    MaCongViec = maCongViec,
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
        public async Task<IActionResult> GetsByMa_TaiChe(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanTaiChe>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        MaMayCan = item.MaMayCan,
                        MaXuong = item.MaXuong,
                        Gio = item.Gio,
                        Id = item.Id,
                        MaUserCan = item.MaUserCan,
                        MaLo = item.MaLo,
                        MaSize = item.MaSize,
                        MaChatLuong = item.MaChatLuong,
                        MaLoaiCa = item.MaLoaiCa,
                        MaMau = item.MaMau,
                        MaThanhPham = item.MaThanhPham,
                        TrongLuong = item.TrongLuong,
                        MaNhanVien = item.MaNhanVien,
                        MaCongViec = item.MaCongViec,
                        GhiChu = item.GhiChu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Tái Chế")]
        public async Task<IActionResult> DoUpDate_TaiChe(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maSize, string maChatLuong, string maLoaiCa, string maMau, string maThanhPham, string maCongViec, string maNhanVien, decimal trongLuong, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maChatLuong) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maCongViec) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanTaiChe
                {
                    MaLo = maLo,
                    MaSize = maSize,
                    MaChatLuong = maChatLuong,
                    MaLoaiCa = maLoaiCa,
                    MaMau = maMau,
                    MaThanhPham = maThanhPham,
                    MaCongViec = maCongViec,
                    MaNhanVien = maNhanVien,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Tái Chế")]
        public async Task<IActionResult> DoDelete_TaiChe(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTaiChe
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenTaiChe(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Tái Chế" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Tái Chế" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Tái Chế" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Tái Chế")]
        public async Task<IActionResult> ChuyenXuong_TaiChe(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTaiChe
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Tái Chế")]
        public async Task<IActionResult> ChuyenSize_TaiChe(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTaiChe
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Tái Chế", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Tái Chế")]
        public async Task<IActionResult> ChuyenThanhPham_TaiChe(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanTaiChe
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Bao Tử
        public async Task<IActionResult> CreatDefautNewBaoTu(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongBaoTu(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = stt,
                        MaMayCan = AppViewModels.AppViewModel.Instance.PCName
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Bao Tử")]
        public async Task<IActionResult> DoInsert_BaoTu(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLoaiCa, string maThanhPham, string maKhachHang, string maNhanVien, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maKhachHang) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new BT_PhieuCan
                {
                    STT = stt,
                    Ngay = dateTime,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaXuong = xuongId,
                    Gio = gio,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaKhachHang = maKhachHang,
                    MaNhanVien = maNhanVien,
                    TrongLuong = trongLuong,
                    SuDung = true,
                    CreateDateTime = DateTime.Now,
                    CreateBy = userName ?? "",
                    ModifiedDateTime = DateTime.Now,
                    ModifiedBy = userName ?? "",
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
        public async Task<IActionResult> GetsByMa_BaoTu(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<BT_PhieuCan>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        MaMayCan = item.MaMayCan,
                        MaXuong = item.MaXuong,
                        Gio = item.Gio,
                        MaLoaiCa = item.MaLoaiCa,
                        MaThanhPham = item.MaThanhPham,
                        MaKhachHang = item.MaKhachHang,
                        MaNhanVien = item.MaNhanVien,
                        TrongLuong = item.TrongLuong,
                        SuDung = item.SuDung,
                        CreateDateTime = item.CreateDateTime,
                        CreateBy = item.CreateBy,
                        ModifiedDateTime = item.ModifiedDateTime,
                        ModifiedBy = item.ModifiedBy,
                        GhiChu = item.GhiChu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Bao Tử")]
        public async Task<IActionResult> DoUpDate_BaoTu(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLoaiCa, string maThanhPham, string maKhachHang, string maNhanVien, decimal trongLuong, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maKhachHang) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new BT_PhieuCan
                {
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaKhachHang = maKhachHang,
                    MaNhanVien = maNhanVien,
                    ModifiedDateTime = DateTime.Now,
                    ModifiedBy = userName ?? "",
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Bao Tử")]
        public async Task<IActionResult> DoDelete_BaoTu(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new BT_PhieuCan
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        SuDung = false,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenBaoTu(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Bao Tử" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Bao Tử" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Bao Tử" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Bao Tử")]
        public async Task<IActionResult> ChuyenXuong_BaoTu(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new BT_PhieuCan
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        //[CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Bao Tử")]
        //public async Task<IActionResult> ChuyenSize_BaoTu(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
        //{
        //    // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
        //    string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

        //    // Khởi tạo danh sách kết quả
        //    List<string[]> result = new List<string[]>();

        //    // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
        //    foreach (string subArray in subArrays)
        //    {
        //        string[] elements = subArray.Split(',');
        //        result.Add(elements);
        //    }

        //    var userName = HttpContext.Session.GetString("Username");

        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    try
        //    {
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

        //        foreach (var parts in result)
        //        {
        //            int stt = int.Parse(parts[0]);
        //            string maMayCan = parts[1];
        //            string maXuong = parts[2];

        //            if (stt <= 0)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
        //                });
        //            }
        //            // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
        //            var model = new BT_PhieuCan
        //            {
        //                MaSize = maSizeChange,
        //                GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
        //            };


        //            // Chuẩn bị URL cho các API
        //            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCans/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
        //            // Thực hiện insert PhieuCanTPDinhHinh
        //            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
        //            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //            var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

        //            if (!responseTP.Success)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = "Chuyển Size Phiếu Cân thất bại: " + responseTP.Message
        //                });

        //            }
        //        }

        //        // Hoàn thành transaction nếu tất cả các update đều thành công
        //        transactionScope.Complete();

        //        return Json(new
        //        {
        //            isSuccess = true,
        //            Messages = "Đã Chuyển Size các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Bao Tử", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Bao Tử")]
        public async Task<IActionResult> ChuyenThanhPham_BaoTu(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new BT_PhieuCan
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Sơ Chế Định Hình
        public async Task<IActionResult> CreatDefautNewSoCheDinhHinh(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongSoCheDinhHinh(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình")]
        public async Task<IActionResult> DoInsert_SoCheDinhHinh(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maMayLangDa, string maNhanVien, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()))
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanSoCheDinhHinh
                {
                    STT = stt,
                    Ngay = dateTime,
                    Gio = gio,
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaNhanVien = maNhanVien,
                    MaXuong = xuongId,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    TrongLuong = trongLuong,
                    MaMayLangDa = maMayLangDa,
                    MaUserCan = "Xử Lý Phiếu Cân",
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
        public async Task<IActionResult> GetsByMa_SoCheDinhHinh(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanSoCheDinhHinh>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaLo = item.MaLo,
                        MaLoaiCa = item.MaLoaiCa,
                        MaThanhPham = item.MaThanhPham,
                        MaNhanVien = item.MaNhanVien,
                        MaXuong = item.MaXuong,
                        MaMayCan = item.MaMayCan,
                        TrongLuong = item.TrongLuong,
                        MaMayLangDa = item.MaMayLangDa,
                        MaUserCan = item.MaUserCan,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình")]
        public async Task<IActionResult> DoUpDate_SoCheDinhHinh(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maLoaiCa, string maThanhPham, string maMayLangDa, string maNhanVien, decimal trongLuong, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()))
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanSoCheDinhHinh
                {
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaThanhPham = maThanhPham,
                    MaMayLangDa = maMayLangDa,
                    MaNhanVien = maNhanVien,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình")]
        public async Task<IActionResult> DoDelete_SoCheDinhHinh(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanSoCheDinhHinh
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenSoCheDinhHinh(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình")]
        public async Task<IActionResult> ChuyenXuong_SoCheDinhHinh(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanSoCheDinhHinh
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        //[CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình")]
        //public async Task<IActionResult> ChuyenSize_SoCheDinhHinh(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
        //{
        //    // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
        //    string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

        //    // Khởi tạo danh sách kết quả
        //    List<string[]> result = new List<string[]>();

        //    // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
        //    foreach (string subArray in subArrays)
        //    {
        //        string[] elements = subArray.Split(',');
        //        result.Add(elements);
        //    }

        //    var userName = HttpContext.Session.GetString("Username");

        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    try
        //    {
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

        //        foreach (var parts in result)
        //        {
        //            int stt = int.Parse(parts[0]);
        //            string maMayCan = parts[1];
        //            string maXuong = parts[2];

        //            if (stt <= 0)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
        //                });
        //            }
        //            // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
        //            var model = new PhieuCanSoCheDinhHinh
        //            {
        //                MaSize = maSizeChange,
        //                GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
        //            };


        //            // Chuẩn bị URL cho các API
        //            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
        //            // Thực hiện insert PhieuCanTPDinhHinh
        //            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
        //            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //            var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

        //            if (!responseTP.Success)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = "Chuyển Size Phiếu Cân thất bại: " + responseTP.Message
        //                });

        //            }
        //        }

        //        // Hoàn thành transaction nếu tất cả các update đều thành công
        //        transactionScope.Complete();

        //        return Json(new
        //        {
        //            isSuccess = true,
        //            Messages = "Đã Chuyển Size các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Sơ Chế Định Hình")]
        public async Task<IActionResult> ChuyenThanhPham_SoCheDinhHinh(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanSoCheDinhHinh
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #region Phụ Phẩm
        public async Task<IActionResult> CreatDefautNewPhuPham(DateTime dateTime, string xuongId)
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    SuDung = true
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm")]
        public async Task<IActionResult> DoInsert_PhuPham(string maXuong, DateTime thoiGianCan, string maLoaiCa, string nhaMuaHang, string maThanhPham, string maLo, string maSize, string maPhuongTien, string maMau, bool suDung, decimal trongLuong)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/Insert";
            try
            {
                if (string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(nhaMuaHang) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maPhuongTien) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(trongLuong.ToString()))
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanPhuPham
                {
                    MaMayTinhCan = AppViewModels.AppViewModel.Instance.PCName,
                    MaUserCan = "Xử Lý Phiếu Cân",
                    ThoiGianCan = thoiGianCan,
                    Ngay = thoiGianCan.Date,
                    NhaMuaHang = nhaMuaHang,
                    MSL = maLo,
                    MaPhuongTien = maPhuongTien,
                    MaLoaiCa = maLoaiCa,
                    MaLoaiThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaMau = maMau,
                    TrongLuong = trongLuong,
                    SuDung = suDung,
                    MaXuongSanXuat = maXuong,
                    NgayCan = thoiGianCan.Date,
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
        public async Task<IActionResult> GetsByMa_PhuPham(string listInfoPhieuCan, DateTime ngayCan)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            string maMayTinhCan = parts[0];
            string maUserCan = parts[1];
            DateTime thoiGianCan = DateTime.Parse(parts[2]);
            decimal trongLuong = decimal.Parse(parts[3]);
            if (trongLuong <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(maMayTinhCan) || string.IsNullOrEmpty(maUserCan) || string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(ngayCan.ToString()))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/GetsByMa/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss")}/{ngayCan.ToString("yyyy-MM-dd HH:mm:ss")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanPhuPham>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaMayTinhCan = item.MaMayTinhCan,
                        MaUserCan = item.MaUserCan,
                        ThoiGianCan = item.ThoiGianCan,
                        Ngay = item.Ngay,
                        NhaMuaHang = item.NhaMuaHang,
                        MSL = item.MSL,
                        MaPhuongTien = item.MaPhuongTien,
                        MaLoaiCa = item.MaLoaiCa,
                        MaLoaiThanhPham = item.MaLoaiThanhPham,
                        MaSize = item.MaSize,
                        MaMau = item.MaMau,
                        TrongLuong = item.TrongLuong,
                        SuDung = item.SuDung,
                        MaXuongSanXuat = item.MaXuongSanXuat,
                        NgayCan = item.NgayCan,
                        TrongLuongTare = item.TrongLuongTare,
                        GhiChu = item.GhiChu,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm")]
        public async Task<IActionResult> DoUpDate_PhuPham(string maXuong, DateTime thoiGianCan, string maLoaiCa, string nhaMuaHang, string maThanhPham, string maLo, string maSize, string maPhuongTien, string maMau, bool suDung, decimal trongLuong, string maMayTinhCan, string maUserCan)
        {
            var ngayCan = thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss");
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/Update/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss")}/{ngayCan}";
            try
            {
                if (string.IsNullOrEmpty(thoiGianCan.ToString()) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(nhaMuaHang) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maPhuongTien) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(trongLuong.ToString()))
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanPhuPham
                {
                    MaLoaiCa = maLoaiCa,
                    NhaMuaHang = nhaMuaHang,
                    MaSize = maSize,
                    MaPhuongTien = maPhuongTien,
                    MaMau = maMau,
                    SuDung = suDung,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm")]
        public async Task<IActionResult> DoDelete_PhuPham(string listInfoPhieuCan, DateTime ngay)
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

                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);
                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuPham
                    {
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };
                    var ngayCan = thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss");
                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/Delete/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss")}/{ngayCan}";

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
        public async Task<IActionResult> CheckQuyenPhuPham(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm")]
        public async Task<IActionResult> ChuyenXuong_PhuPham(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);
                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuPham
                    {
                        MaXuongSanXuat = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };

                    var ngayCan = thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss");
                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/ChuyenXuong/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss")}/{ngayCan}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm")]
        public async Task<IActionResult> ChuyenSize_PhuPham(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
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
                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);
                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuPham
                    {
                        MaSize = maSizeChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
                    };

                    var ngayCan = thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss");
                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/ChuyenSize/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss")}/{ngayCan}";
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm")]
        public async Task<IActionResult> ChuyenThanhPham_PhuPham(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    string maMayTinhCan = parts[0];
                    string maUserCan = parts[1];
                    DateTime thoiGianCan = DateTime.Parse(parts[2]);
                    decimal trongLuong = decimal.Parse(parts[3]);
                    if (trongLuong <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuPham
                    {
                        MaLoaiThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };

                     var ngayCan = thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss");
                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/ChuyenThanhPham/{maMayTinhCan}/{maUserCan}/{thoiGianCan.ToString("yyyy-MM-dd HH:mm:ss")}/{ngayCan}";
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
        #endregion
        #region Phụ Phẩmv2
        public async Task<IActionResult> CreatDefautNewPhuPhamv2(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAllsWithDateAndXuongPhuPhamv2(dateTime, xuongId);
                if (dataSource != null)
                {

                    var maxstt = dataSource.Where(x => x.MaMayCan == AppViewModels.AppViewModel.Instance.PCName)
        .Where(x => x.STT != null)
        .Select(x => Math.Abs(x.STT))
        .DefaultIfEmpty(0)
        .Max();
                    var stt = maxstt + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = stt,
                        SuDung = true
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2", Func = "Thêm Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2")]
        public async Task<IActionResult> DoInsert_PhuPhamv2(DateTime dateTime, string xuongId, int stt, TimeSpan gio, string maLo, string maThanhPham, string maNhanVien, decimal trongLuong, bool suDung)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/Insert";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(xuongId) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(suDung.ToString()))
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanPhuPhamv2
                {
                    STT = stt,
                    Ngay = dateTime,
                    Gio = gio,
                    MaLo = maLo,
                    MaThanhPham = maThanhPham,
                    MaNhanVien = maNhanVien,
                    MaXuong = xuongId,
                    MaMayCan = AppViewModels.AppViewModel.Instance.PCName,
                    TrongLuong = trongLuong,
                    MaThe = "0",
                    SuDung = suDung,
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
        public async Task<IActionResult> GetsByMa_PhuPhamv2(string listInfoPhieuCan, DateTime ngay)
        {
            string[] parts = listInfoPhieuCan.TrimEnd('|').Split(',');

            // Gán tên cho từng phần tử
            int stt = int.Parse(parts[0]);
            string maMayCan = parts[1];
            string maXuong = parts[2];
            if (stt <= 0)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                });
            }
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay.ToString()) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/GetsByMa/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<PhieuCanPhuPhamv2>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        STT = item.STT,
                        Ngay = item.Ngay,
                        Gio = item.Gio,
                        MaLo = item.MaLo,
                        MaThanhPham = item.MaThanhPham,
                        MaNhanVien = item.MaNhanVien,
                        MaXuong = item.MaXuong,
                        MaMayCan = item.MaMayCan,
                        TrongLuong = item.TrongLuong,
                        MaThe = item.MaThe,
                        SuDung = item.SuDung,
                        GhiChu = item.GhiChu
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2", Func = "Sửa Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2")]
        public async Task<IActionResult> DoUpDate_PhuPhamv2(DateTime ngay, string maXuong, int stt, TimeSpan gio, string maLo, string maThanhPham, string maNhanVien, decimal trongLuong, bool suDung, string maMayCan)
        {
            var userName = HttpContext.Session.GetString("Username");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/Update/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
            try
            {
                if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(gio.ToString()) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(trongLuong.ToString()) || string.IsNullOrEmpty(suDung.ToString()))
                {

                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new PhieuCanPhuPhamv2
                {
                    MaLo = maLo,
                    MaThanhPham = maThanhPham,
                    MaNhanVien = maNhanVien,
                    SuDung = suDung,
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2", Func = "Xoá Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2")]
        public async Task<IActionResult> DoDelete_PhuPhamv2(string listInfoPhieuCan, DateTime ngay)
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

                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuPhamv2
                    {
                        STT = stt * -1,
                        TrongLuong = 0,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: XOA",
                    };

                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/Delete/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";

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
        public async Task<IActionResult> CheckQuyenPhuPhamv2(string typeOption)
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
                    { "CHUYENXUONG", "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2" },
                    { "CHUYENSIZE", "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2" },
                    { "CHUYENTHANHPHAM", "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2" }
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2" && x.Func == func && x.Status == 1);

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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2", Func = "Chuyển Xưởng Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2")]
        public async Task<IActionResult> ChuyenXuong_PhuPhamv2(string listInfoPhieuCan, DateTime ngay, string maXuongChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuPhamv2
                    {
                        MaXuong = maXuongChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN XUONG"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/ChuyenXuong/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        //[CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2", Func = "Chuyển Size Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2")]
        //public async Task<IActionResult> ChuyenSize_PhuPhamv2(string listInfoPhieuCan, DateTime ngay, string maSizeChange)
        //{
        //    // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
        //    string[] subArrays = listInfoPhieuCan.Split('|', StringSplitOptions.RemoveEmptyEntries);

        //    // Khởi tạo danh sách kết quả
        //    List<string[]> result = new List<string[]>();

        //    // Tách các phần tử trong từng mảng con ngăn cách bởi dấu ,
        //    foreach (string subArray in subArrays)
        //    {
        //        string[] elements = subArray.Split(',');
        //        result.Add(elements);
        //    }

        //    var userName = HttpContext.Session.GetString("Username");

        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    try
        //    {
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

        //        foreach (var parts in result)
        //        {
        //            int stt = int.Parse(parts[0]);
        //            string maMayCan = parts[1];
        //            string maXuong = parts[2];

        //            if (stt <= 0)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
        //                });
        //            }
        //            // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
        //            var model = new PhieuCanPhuPhamv2
        //            {
        //                MaSize = maSizeChange,
        //                GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN SIZE"
        //            };


        //            // Chuẩn bị URL cho các API
        //            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2s/ChuyenSize/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
        //            // Thực hiện insert PhieuCanTPDinhHinh
        //            var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
        //            var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
        //            var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

        //            if (!responseTP.Success)
        //            {
        //                return Json(new
        //                {
        //                    isSuccess = false,
        //                    Messages = "Chuyển Size Phiếu Cân thất bại: " + responseTP.Message
        //                });

        //            }
        //        }

        //        // Hoàn thành transaction nếu tất cả các update đều thành công
        //        transactionScope.Complete();

        //        return Json(new
        //        {
        //            isSuccess = true,
        //            Messages = "Đã Chuyển Size các phiếu  đã chọn!"
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
        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2", Func = "Chuyển Thành Phẩm Xử Lý Phiếu Cân / Phiếu Cân Phụ Phẩm v2")]
        public async Task<IActionResult> ChuyenThanhPham_PhuPhamv2(string listInfoPhieuCan, DateTime ngay, string maThanhPhamChange)
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
                    int stt = int.Parse(parts[0]);
                    string maMayCan = parts[1];
                    string maXuong = parts[2];

                    if (stt <= 0)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Trong danh sách chọn có phiếu cân đã xóa! Vui lòng kiểm tra lại!."
                        });
                    }
                    // Chuẩn bị dữ liệu cho PhieuCanTPDinhHinh
                    var model = new PhieuCanPhuPhamv2
                    {
                        MaThanhPham = maThanhPhamChange,
                        GhiChu = $@"Người thực hiện: {userName}, trên máy: {AppViewModels.AppViewModel.Instance.PCName}, thời gian: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}, loại: CHUYEN THANH PHAM"
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhamv2/ChuyenThanhPham/{stt}/{ngay.ToString("yyyy-MM-dd")}/{maMayCan}/{maXuong}";
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
        #endregion
        #endregion


    }
}
