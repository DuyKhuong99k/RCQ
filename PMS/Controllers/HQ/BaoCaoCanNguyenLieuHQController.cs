using Azure;
using Azure.Core;
using Dapper;
using Dao.Repos.HQ;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using PMS.Attrs;
using PMS.Models;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Charts;
using Syncfusion.EJ2.Notifications;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMS.Controllers.HQ
{
    [Authorize]
    public class BaoCaoCanNguyenLieuHQController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaoCaoCanNguyenLieuHQController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private sealed class KhoiLuongXuatRowContext
        {
            public string? SoPhieuCanNhap { get; set; }
            public string? MaQuyCach { get; set; }
            public string? MaLo { get; set; }
            public long MaSanPham { get; set; }
            public int Index { get; set; }
            public decimal KhoiLuongNhap { get; set; }
            public decimal KhoiLuongXuat { get; set; }
            public decimal TongXuatThuong { get; set; }
            public decimal TongXuatHu { get; set; }
            public decimal TongNhapLuyKeThuong { get; set; }
            public decimal TongNhapThuong { get; set; }
        }

        private sealed class SourceRowInfo
        {
            public string Id { get; set; } = string.Empty;
            public decimal TrongLuongHang { get; set; }
        }

        private static void AdjustSourceRows(
            SqlConnection connection,
            SqlTransaction transaction,
            string maLo,
            long maSanPham,
            bool isCanHu,
            decimal delta)
        {
            delta = Math.Round(delta, 1);
            if (Math.Abs(delta) < 0.0001m)            {
                return;
            }

            if (delta > 0)
            {
                const string pickSql = @"
SELECT TOP (1)
    px.Id
FROM HQ_PhieuCanXuatNguyenLieu px
JOIN HQ_PhieuCanNguyenLieu pc
    ON pc.Id = px.IdPhieuCanNguyenLieu
WHERE pc.MaLo = @MaLo
  AND px.MaSanPham = @MaSanPham
  AND ISNULL(px.IsCanHu,0) = @IsCanHu
  AND ISNULL(px.IsHuy,0) = 0
ORDER BY
    pc.NgayGio DESC,
    px.STT DESC,
    px.Id DESC;";

                var targetRowId = connection.QueryFirstOrDefault<string>(pickSql, new
                {
                    MaLo = maLo,
                    MaSanPham = maSanPham,
                    IsCanHu = isCanHu
                }, transaction);

                if (string.IsNullOrWhiteSpace(targetRowId))
                {
                    throw new InvalidOperationException("Không tìm thấy dòng nguồn để tăng.");
                }

                var updated = connection.Execute(@"
UPDATE HQ_PhieuCanXuatNguyenLieu
SET TrongLuongHang = TrongLuongHang + @Delta,
    TrongLuongTong = TrongLuongHang + @Delta
WHERE Id = @Id;", new
                {
                    Id = targetRowId,
                    Delta = delta
                }, transaction);

                if (updated == 0)
                {
                    throw new InvalidOperationException("Cập nhật dòng nguồn thất bại.");
                }

                return;
            }

            var remaining = Math.Abs(delta);

while (remaining > 0.0001m)            {
                const string pickSql = @"
SELECT TOP (1)
    px.Id,
    px.TrongLuongHang
FROM HQ_PhieuCanXuatNguyenLieu px
JOIN HQ_PhieuCanNguyenLieu pc
    ON pc.Id = px.IdPhieuCanNguyenLieu
WHERE pc.MaLo = @MaLo
  AND px.MaSanPham = @MaSanPham
  AND ISNULL(px.IsCanHu,0) = @IsCanHu
  AND ISNULL(px.IsHuy,0) = 0
  AND px.TrongLuongHang > 0
ORDER BY
    pc.NgayGio DESC,
    px.STT DESC,
    px.Id DESC;";

                var sourceRow = connection.QueryFirstOrDefault<SourceRowInfo>(pickSql, new
                {
                    MaLo = maLo,
                    MaSanPham = maSanPham,
                    IsCanHu = isCanHu
                }, transaction);

                if (sourceRow == null)
                {
                    throw new InvalidOperationException("Không còn dòng nguồn để giảm.");
                }

                var applied = remaining < sourceRow.TrongLuongHang ? remaining : sourceRow.TrongLuongHang;

                var updated = connection.Execute(@"
UPDATE HQ_PhieuCanXuatNguyenLieu
SET TrongLuongHang = TrongLuongHang - @Applied,
    TrongLuongTong = TrongLuongHang - @Applied
WHERE Id = @Id AND TrongLuongHang >= @Applied;", new
                {
                    Id = sourceRow.Id,
                    Applied = applied
                }, transaction);

                if (updated == 0)
                {
                    throw new InvalidOperationException("Cập nhật dòng nguồn thất bại.");
                }

                remaining = Math.Round(remaining - applied, 1);
            }
        }

        private KhoiLuongXuatRowContext? GetKhoiLuongXuatRowContext(string maLo, string soPhieuCanNhap, string maQuyCach)
        {
            var dao = new HQ_PhieuCanXuatNguyenLieu(AppViewModels.Base.Ins.ConnectionString);
            var rows = dao.GetKhoiLuongXuatLoTheoXuonget<KhoiLuongXuatRowContext>(maLo);

            return rows.FirstOrDefault(row =>
                string.Equals(row.SoPhieuCanNhap, soPhieuCanNhap, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(row.MaQuyCach, maQuyCach, StringComparison.OrdinalIgnoreCase));
        }

        #region Nhập Nguyên Liệu
        #region Chi Tiết Phiếu Cân Nhập Nguyên Liệu
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ", Func = "Xem Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ")]
        public IActionResult ChiTietNLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietNLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết NL Nhập";
            return View("~/Views/BaoCaoNguyenLieuHQ/ChiTietNLNHQView.cshtml");
        }
//        [HttpPost]
//        public IActionResult UpdateKhoiLuong(
//    string soPhieuCanNhap,
//    string maQuyCach,
//    decimal khoiLuongNhap)
//        {
//            try
//            {
//                var query = @"
//IF EXISTS (
//    SELECT 1
//    FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
//    WHERE SoPhieuCanNhap = @SoPhieuCanNhap
//)
//BEGIN

//    -- Có dữ liệu thì update
//			DECLARE @TongTrongLuong FLOAT
//			DECLARE @KhoiLuongCu FLOAT
//			DECLARE @ChenhLech FLOAT
//			DECLARE @MaQuyCachBu VARCHAR(50)

//			-- Tổng phiếu
//			SELECT @TongTrongLuong = TrongLuongHang
//			FROM HQ_PhieuCanNhapNguyenLieu
//			WHERE SoPhieuCanNhap = @SoPhieuCanNhap

//			-- Khối lượng cũ của dòng đang sửa
//			SELECT @KhoiLuongCu =
//				(TyLe * @TongTrongLuong) / 100.0
//			FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
//			WHERE SoPhieuCanNhap = @SoPhieuCanNhap
//			AND MaQuyCach = @MaQuyCach

//			-- Chênh lệch
//			SET @ChenhLech = @KhoiLuongNhap - @KhoiLuongCu

//			------------------------------------------------
//			-- Update dòng đang sửa
//			------------------------------------------------
//			UPDATE HQ_ChiTietPhanBoTyLeNguyenLieuNhap
//			SET TyLe =
//				(@KhoiLuongNhap * 100.0)
//				/ NULLIF(@TongTrongLuong,0)
//			WHERE SoPhieuCanNhap = @SoPhieuCanNhap
//			AND MaQuyCach = @MaQuyCach

//			------------------------------------------------
//			-- Lấy dòng đầu tiên khác để bù
//			------------------------------------------------
//			SELECT TOP 1
//				@MaQuyCachBu = MaQuyCach
//			FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
//			WHERE SoPhieuCanNhap = @SoPhieuCanNhap
//			AND MaQuyCach <> @MaQuyCach

//			------------------------------------------------
//			-- Update dòng bù
//			------------------------------------------------
//			UPDATE pn
//			SET pn.TyLe =
//			(
//				(
//					((pn.TyLe * @TongTrongLuong) / 100.0)
//					- @ChenhLech
//				) * 100.0
//			) / NULLIF(@TongTrongLuong,0)

//			FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap pn
//			WHERE pn.SoPhieuCanNhap = @SoPhieuCanNhap
//			AND pn.MaQuyCach = @MaQuyCachBu

//END
//ELSE
//BEGIN

//    -- Không có dữ liệu thì chạy câu khác
//			 DECLARE @TongTrongLuong FLOAT
//			DECLARE @KhoiLuongCu FLOAT
//			DECLARE @ChenhLech FLOAT
//			DECLARE @MaQuyCachBu VARCHAR(50)

//			------------------------------------------------
//			-- Tổng trọng lượng phiếu
//			------------------------------------------------
//			SELECT @TongTrongLuong = TrongLuongHang
//			FROM HQ_PhieuCanNhapNguyenLieu
//			WHERE SoPhieuCanNhap = @SoPhieuCanNhap

//			------------------------------------------------
//			-- Lấy trọng lượng cũ của dòng đang sửa
//			------------------------------------------------
//			SELECT @KhoiLuongCu = TrongLuongBaoLua
//			FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap
//			WHERE SoPhieuCanNhap = @SoPhieuCanNhap
//			AND MaQuyCach = @MaQuyCach

//			------------------------------------------------
//			-- Chênh lệch
//			------------------------------------------------
//			SET @ChenhLech = @KhoiLuongNhap - @KhoiLuongCu

//			------------------------------------------------
//			-- Update dòng đang sửa
//			------------------------------------------------
//			UPDATE bl
//			SET bl.TrongLuongBaoLua = @KhoiLuongNhap
//			FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap bl
//			WHERE bl.SoPhieuCanNhap = @SoPhieuCanNhap
//			AND bl.MaQuyCach = @MaQuyCach

//			------------------------------------------------
//			-- Lấy dòng đầu tiên khác để bù
//			------------------------------------------------
//			SELECT TOP 1
//				@MaQuyCachBu = MaQuyCach
//			FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap
//			WHERE SoPhieuCanNhap = @SoPhieuCanNhap
//			AND MaQuyCach <> @MaQuyCach

//			------------------------------------------------
//			-- Update dòng bù để cân bằng tổng
//			------------------------------------------------
//			UPDATE bl
//			SET bl.TrongLuongBaoLua =
//				bl.TrongLuongBaoLua - @ChenhLech
//			FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap bl
//			WHERE bl.SoPhieuCanNhap = @SoPhieuCanNhap
//			AND bl.MaQuyCach = @MaQuyCachBu

//END";

//                using var connection = new SqlConnection(AppViewModels.Base.Ins.ConnectionString);
//                connection.Open();

//                var rows = connection.Execute(query, new
//                {
//                    soPhieuCanNhap,
//                    maQuyCach,
//                    khoiLuongNhap
//                });

//                return Json(new { success = rows > 0 });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
        public async Task<IEnumerable<object>> GetChiTietPhieuCanNhapNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/GetChiTietPhieuCanNhapNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Sant Phẩm Phiếu Cân Nhập Nguyên Liệu
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ", Func = "Xem Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ")]
        public IActionResult TongHopSanPhamNLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopSanPhamNLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Sản Phẩm NL Nhập";
            return View("~/Views/BaoCaoNguyenLieuHQ/TongHopSanPhamNLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopSanPhamPhieuCanNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanNhapNguyenLieu/GetTongHopSanPhamPhieuCanNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        #endregion
        #endregion

        #region Xuất Nguyên Liệu
        #region Chi Tiết
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Chi Tiết HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Chi Tiết HQ")]
        public IActionResult ChiTietXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChiTietXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/ChiTietXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTietPhieuCanXuatNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetChiTietPhieuCaXuatNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tổng Hợp Sản Phẩm
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Tổng Hợp Sản Phẩm HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Tổng Hợp Sản Phẩm HQ")]
        public IActionResult TongHopSanPhamXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TongHopSanPhamXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Sản Phẩm NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/TongHopSanPhamXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTongHopSanPhamPhieuCanXuatNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetTongHopSanPhamPhieuCanXuatNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tồn kho
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Tồn Kho HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Tồn Kho HQ")]
        public IActionResult TonKhoXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TonKhoXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tồn Kho NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/TonKhoXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTonKhoSanPhamPhieuCanXuatNguyenLieus(DateTime? fromDate = null, DateTime? toDate = null, string xuongId = null)
        {
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetTonKhoSanPhamPhieuCanXuatNguyenLieus/{fromDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{toDate?.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion
        #region Tỷ Lệ Hao Hụt
        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Tỷ Lệ Hao Hụt HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Tỷ Lệ Hao Hụt HQ")]
        public IActionResult TyLeHaoHutXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TyLeHaoHutXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tỷ Lệ Hao Hụt NL Xuất";
            return View("~/Views/BaoCaoNguyenLieuHQ/TyLeHaoHutXLNHQView.cshtml");
        }
        public async Task<IEnumerable<object>> GetTyLeNguyenLieuHaoHut(DateTime? fromDate = null)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetTyLeNguyenLieuHaoHut/{fromDate?.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        #endregion


        #region Khối Lượng Xuất Xưởng Theo Lô

        public class ListLo
        {

            public string MaLo { get; set; }

        }


        [CustomAuthorize(Fu = "Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ", Func = "Xem Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ")]
        public IActionResult KhoiLuongXuatLoTheoXuongXLNHQView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "KhoiLuongXuatLoTheoXuongXLNHQView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            //var apiListLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetListLo";
            //using var helperListLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            //var listLo = helperListLo.GetAsync<IEnumerable<ListLo>>(HttpContext, apiListLoUrl);
            //ViewBag.listLo = listLo.Result.ToList();


            var apiListLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetListLo";

            using var helperListLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            var listLo = helperListLo
                            .GetAsync<IEnumerable<ListLo>>(HttpContext, apiListLoUrl)
                            .Result;

            ViewBag.listLo = listLo?.ToList() ?? new List<ListLo>();


            ViewBag.TitlePage = "Báo Cáo Khối Lượng Xuất Lô Theo Xưởng";
            return View("~/Views/BaoCaoNguyenLieuHQ/KhoiLuongXuatLoTheoXuongXLNHQView.cshtml");
        }
       
        public async Task<IEnumerable<object>> GetKhoiLuongXuatLoTheoXuong(string maLo = null)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCanXuatNguiyenLieu/GetKhoiLuongXuatLoTheoXuong/{maLo}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        public async Task<ActionResult> ReloadKhoiLuongXuatLoTheoXuong(string maLo)
        {

            if (maLo == "Chọn Lô" || maLo == null)
            {
                maLo = "'";
            }
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "KhoiLuongXuatLoTheoXuongXLNHQView")
            {
                dataSource = await GetKhoiLuongXuatLoTheoXuong(maLo);
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
        [HttpPost]
        public IActionResult UpdateKhoiLuong(
    string soPhieuCanNhap,
    string maQuyCach,
    decimal khoiLuongNhap)
        {
            try
            {
                var query = @"

DECLARE @TongTrongLuong FLOAT
DECLARE @KhoiLuongCu FLOAT
DECLARE @ChenhLech FLOAT
DECLARE @MaQuyCachBu VARCHAR(50)

IF EXISTS (
    SELECT 1
    FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap
)
BEGIN

    ------------------------------------------------
    -- Tổng phiếu
    ------------------------------------------------

    SELECT @TongTrongLuong = TrongLuongHang
    FROM HQ_PhieuCanNhapNguyenLieu
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap

    ------------------------------------------------
    -- Khối lượng cũ
    ------------------------------------------------

    SELECT @KhoiLuongCu =
        (TyLe * @TongTrongLuong) / 100.0
    FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap
    AND MaQuyCach = @MaQuyCach

    ------------------------------------------------
    -- Chênh lệch
    ------------------------------------------------

    SET @ChenhLech = @KhoiLuongNhap - @KhoiLuongCu

    ------------------------------------------------
    -- Update dòng sửa
    ------------------------------------------------

    UPDATE HQ_ChiTietPhanBoTyLeNguyenLieuNhap
    SET TyLe =
        (@KhoiLuongNhap * 100.0)
        / NULLIF(@TongTrongLuong,0)
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap
    AND MaQuyCach = @MaQuyCach

    ------------------------------------------------
    -- Lấy dòng khác để bù
    ------------------------------------------------

    SELECT TOP 1
        @MaQuyCachBu = MaQuyCach
    FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap
    AND MaQuyCach <> @MaQuyCach

    ------------------------------------------------
    -- Update bù
    ------------------------------------------------

    UPDATE pn
    SET pn.TyLe =
    (
        (
            ((pn.TyLe * @TongTrongLuong) / 100.0)
            - @ChenhLech
        ) * 100.0
    ) / NULLIF(@TongTrongLuong,0)

    FROM HQ_ChiTietPhanBoTyLeNguyenLieuNhap pn
    WHERE pn.SoPhieuCanNhap = @SoPhieuCanNhap
    AND pn.MaQuyCach = @MaQuyCachBu

END
ELSE
BEGIN

    ------------------------------------------------
    -- Tổng trọng lượng
    ------------------------------------------------

    SELECT @TongTrongLuong = TrongLuongHang
    FROM HQ_PhieuCanNhapNguyenLieu
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap

    ------------------------------------------------
    -- Khối lượng cũ
    ------------------------------------------------

    SELECT @KhoiLuongCu = TrongLuongBaoLua
    FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap
    AND MaQuyCach = @MaQuyCach

    ------------------------------------------------
    -- Chênh lệch
    ------------------------------------------------

    SET @ChenhLech = @KhoiLuongNhap - @KhoiLuongCu

    ------------------------------------------------
    -- Update dòng sửa
    ------------------------------------------------

    UPDATE bl
    SET bl.TrongLuongBaoLua = @KhoiLuongNhap
    FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap bl
    WHERE bl.SoPhieuCanNhap = @SoPhieuCanNhap
    AND bl.MaQuyCach = @MaQuyCach

    ------------------------------------------------
    -- Lấy dòng khác để bù
    ------------------------------------------------

    SELECT TOP 1
        @MaQuyCachBu = MaQuyCach
    FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap
    WHERE SoPhieuCanNhap = @SoPhieuCanNhap
    AND MaQuyCach <> @MaQuyCach

    ------------------------------------------------
    -- Update bù
    ------------------------------------------------

    UPDATE bl
    SET bl.TrongLuongBaoLua =
        bl.TrongLuongBaoLua - @ChenhLech
    FROM HQ_ChiTietPhanBoBaoLuaNguyenLieuNhap bl
    WHERE bl.SoPhieuCanNhap = @SoPhieuCanNhap
    AND bl.MaQuyCach = @MaQuyCachBu

END
";

                using var connection =
                    new SqlConnection(AppViewModels.Base.Ins.ConnectionString);

                connection.Open();

                var rows = connection.Execute(query, new
                {
                    soPhieuCanNhap,
                    maQuyCach,
                    khoiLuongNhap
                });

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateKhoiLuongXuat(
            string soPhieuCanNhap,
            string maQuyCach,
            string maLo,
            decimal khoiLuongXuat)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(soPhieuCanNhap) || string.IsNullOrWhiteSpace(maQuyCach))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Thiếu dữ liệu định danh để cập nhật."
                    });
                }

                if (string.IsNullOrWhiteSpace(maLo) )
                {
                    return Json(new
                    {
                        success = false,
                        message = "Thiếu dữ liệu lot để cập nhật."
                    });
                }

                var context = GetKhoiLuongXuatRowContext(maLo.Trim(), soPhieuCanNhap.Trim(), maQuyCach.Trim());

                if (context == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không tìm thấy dữ liệu nguồn để cập nhật."
                    });
                }

                if (khoiLuongXuat < 0 || khoiLuongXuat > context.KhoiLuongNhap)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Khối lượng xuất phải nằm trong khoảng từ 0 đến khối lượng nhập."
                    });
                }

                using var connection = new SqlConnection(AppViewModels.Base.Ins.ConnectionString);

                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    var delta = Math.Round(
                        khoiLuongXuat - context.KhoiLuongXuat,
                        1
                    );
                    if (Math.Abs(delta) < 0.0001m)
{
    return Json(new
    {
        success = true
    });
}

if (context.Index == 0)
{
    AdjustSourceRows(
        connection,
        transaction,
        context.MaLo!,
        context.MaSanPham,
        true,
        delta
    );
}
else
{
    AdjustSourceRows(
        connection,
        transaction,
        context.MaLo!,
        context.MaSanPham,
        false,
        delta
    );
}
                    transaction.Commit();

                return Json(new
                {
                    success = true
                });
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        #endregion
        #endregion

        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "ChiTietNLNHQView")
            {
                dataSource = await GetChiTietPhieuCanNhapNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopSanPhamNLNHQView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "ChiTietXLNHQView")
            {
                dataSource = await GetChiTietPhieuCanXuatNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TongHopSanPhamXLNHQView")
            {
                dataSource = await GetTongHopSanPhamPhieuCanXuatNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TonKhoXLNHQView")
            {
                dataSource = await GetTonKhoSanPhamPhieuCanXuatNguyenLieus(fromDate, toDate, xuongId);
            }
            else if (reportType == "TyLeHaoHutXLNHQView")
            {
                dataSource = await GetTyLeNguyenLieuHaoHut(fromDate);
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
