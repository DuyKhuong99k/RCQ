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
using System.Net.Http.Json;
using Syncfusion.XlsIO;
using ToolEx;
using AppViewModels;
using Syncfusion.EJ2.Maps;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using PMS.Attrs;
using ViewModels.Repos.HQ;
using Syncfusion.EJ2.Spreadsheet;

namespace PMS.Controllers.BaoCao.T
{
    [Authorize]
    public class T_PhieuCanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private AppViewModels.AppViewModel _vmApp => AppViewModel.Instance;
        public T_PhieuCanController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [CustomAuthorize(Fu = "Báo Cáo / Tôm / Chi Tiết", Func = "Xem Báo Cáo / Tôm / Chi Tiết")]
        public IActionResult BaoCaoChiTiet()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BaoCaoChiTietTom");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Chi Tiết Tôm";
            return View("~/Views/BaoCao/Tom/BaoCaoChiTietTomView.cshtml");
        }
        [CustomAuthorize(Fu = "Báo Cáo / Tôm / Tổng Hợp Nhân Viên", Func = "Xem Báo Cáo / Tôm / Tổng Hợp Nhân Viên")]
        public IActionResult BaoCaoTongHopNhanVien()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BaoCaoTongHopNhanVienTom");
            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Báo Cáo Tổng Hợp Nhân Viên Tôm";
            return View("~/Views/BaoCao/Tom/BaoCaoTongHopNhanVienTomView.cshtml");
        }
        public async Task<IEnumerable<object>> GetChiTietTom(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetPhieuCanChiTietsToNgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanVienTom(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetTongHopNhanViensToNgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> Reload(DateTime? fromDate, DateTime? dateTime, string? xuongId = "1")
        {
            IEnumerable<object> dataSource = null;
            string reportType = HttpContext.Session.GetString("reportType");
            bool success = true;
            if (reportType == "BaoCaoChiTietTom")
            {
                dataSource = await GetChiTietTom(fromDate, dateTime, xuongId);
            }
            if (reportType == "BaoCaoTongHopNhanVienTom")
            {
                dataSource = await GetTongHopNhanVienTom(fromDate, dateTime, xuongId);
            }
            if (dataSource == null || !dataSource.Any())
            {
                success = false;
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
        [HttpGet]
        //[DeleteFileAttribute] //Action Filter, it will auto delete the file after download, 
        //I will explain it later
        public ActionResult Download(string file)
        {
            //get the temp folder and file path in server
            string fullPath = Path.Combine($"{_vmApp.AppPath}", file);
            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file);
        }
        public async Task<IActionResult> ExportBangKeNangXuatCongNhat(DateTime fromDate, DateTime dateTime, bool isNhom, bool isNgayNguyenLieu)
        {
            try
            {
                var xuongId = HttpContext.Session.GetString("XuongId");
                var vmApp = AppViewModel.Instance;
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var ngayThangNam = $@"{vmApp.FromDate.ToString("dd-MM-yyyy")}-{vmApp.DateReport.ToString("dd-MM-yyyy")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var apiUrlXiNghiep = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetXuongById/{xuongId}";
                var xiNghiep = await helper.GetAsync<XiNghiep>(HttpContext, apiUrlXiNghiep);

                var items = new List<object>();
                if (isNgayNguyenLieu == true)
                {
                    //call api GetTongHopNhanViens3ToNgayNguyenLieuNSRC
                }
                else
                {
                    var valueIsNhom = Convert.ToInt32(isNhom);

                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetTongHopNhanViens3DateTimeToDateTimeNSRC/{fromDate.ToString("yyyy-MM-dd HH:mm:ss")}/{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}/{xuongId}/{isNhom.ToString()}";
                    using var helper2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    items = await helper2.GetAsync2<object>(HttpContext, apiUrl);
                }
                if (isNhom == true)//năng suất chung
                {
                    var appPath = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "T", "TemplateBangKeNSCongNhat_CMX.xlsx");
                    var worksheetIndex = 0;
                    if (items != null && items.Any())
                    {
                        var _items = items.ToDataTable();
                        using var excelEngine = new ExcelEngine();
                        var application = excelEngine.Excel;
                        using var fileStream = new FileStream(appPath, FileMode.Open);
                        var workbook = application.Workbooks.Open(fileStream);
                        var worksheet = workbook.Worksheets[worksheetIndex];
                        worksheet.EnableSheetCalculations();
                        var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                        if (_items.Rows.Count > 0)
                        {
                            var listOfNhom = items.Cast<dynamic>()
                                        .ToList()
                                        .Select(x => new { Nhom = (string)x.Nhom })
                                        .Distinct()
                                        .OrderBy(x => x.Nhom)
                                        .ThenBy(x => x.Nhom)
                                        .ToList();
                            var itemDycs = items.Cast<dynamic>().ToList();
                            for (var i = 0; i < listOfNhom.Count; i++)
                            {
                                var nhoms = listOfNhom[i];
                                var nhanviensNhom = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(x => (string)x.Nhom == nhoms.Nhom)
                                    .GroupBy(x => new { x.Ngay, x.MaNhanVien, x.MaHoSo, x.TenNhanVien, x.Nhom })
                                    .Select(
                                        x => new
                                        {
                                            Ngay = (DateTime?)x.Key.Ngay,
                                            MaNhanVien = (string)x.Key.MaNhanVien,
                                            MaHoSo = (string)x.Key.MaHoSo,
                                            TenNhanVien = (string)x.Key.TenNhanVien,
                                            Nhom = (string)x.Key.Nhom
                                        })
                                    .OrderBy(x => x.Ngay)
                                    .ThenBy(x => x.MaNhanVien)
                                    .ToList();
                                var headersNhom = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(x => (string)x.Nhom == nhoms.Nhom)
                                    .GroupBy(x => new { x.MaSize, x.SizeName, x.MaCongViec, x.CongViecName })
                                    .Select(
                                        x => new
                                        {
                                            MaSize = (string)x.Key.MaSize,
                                            SizeName = (string)x.Key.SizeName,
                                            CongViecName = (string)x.Key.CongViecName,
                                            MaCongViec = (string)x.Key.MaCongViec
                                            //Nhom = (string)x.Key.Nhom
                                        })
                                    .OrderBy(x => x.MaSize)
                                    .Where(
                                        x => x.MaSize != null &&
                                             x.SizeName != null &&
                                             x.MaCongViec != null &&
                                             x.CongViecName != null).Distinct()
                                    .ToList();

                                if (headersNhom.Count < 1) continue;

                                worksheet = workbook.Worksheets[worksheetIndex];
                                workbook.Worksheets.AddCopyAfter(workbook.Worksheets[worksheetIndex]);
                                worksheetIndex += 1;
                                if (nhoms.Nhom == null)
                                    worksheet.Name = $@"{"NULL"} ";
                                else
                                    worksheet.Name = $@"{nhoms.Nhom} ";

                                worksheet.Range[4, 4, 4, 12].Merge();
                                worksheet.Range[4, 4].Value = $@"{nhoms.Nhom} {xiNghiep.Ten}, Từ ngày {vmApp.FromDate:dd/MM/yyyy} đến {vmApp.DateReport.ToString("dd/MM/yyyy")}";
                                worksheet.Range[4, 4].CellStyle.Font.Bold = true;
                                //them tao header data
                                var numCol = 0;
                                var col = 5;
                                var lastCol = 0;
                                for (var y = 0; y < headersNhom.Count; y++)
                                {
                                    var header = headersNhom[y];
                                    if (y == 0)
                                    {
                                        col = 6;
                                        //numCol = 1;
                                        lastCol = col;
                                    }
                                    else
                                    {
                                        col = lastCol + 1;
                                        //numCol = 1;
                                        lastCol = col;
                                    }

                                    var headerGroupName = $@"{header.CongViecName} ({header.SizeName})";
                                    worksheet.Range[9, col].Value2 = $@"{headerGroupName}";
                                    worksheet.Range[9, 1, 9, lastCol].BorderAround();
                                    worksheet.Range[9, 1, 9, lastCol].BorderInside();
                                    worksheet.Range[9, col].CellStyle.Font.Bold = true;
                                    worksheet.Range[9, col].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                                    worksheet.Range[9, col].WrapText = true;
                                }

                                var colData = 6;
                                var lastColData = 0;
                                var rowNum = 0;
                                decimal tongTLT = 0;
                                var STT = 0;

                                for (var y = 0; y < headersNhom.Count; y++)
                                {
                                    var header = headersNhom[y];
                                    tongTLT = 0;
                                    if (y == 0)
                                    {
                                        colData = 6;
                                        lastColData = colData;
                                    }
                                    else
                                    {
                                        colData = lastColData + 1;
                                        lastColData = colData;
                                    }

                                    for (var j = 0; j < nhanviensNhom.Count; j++)
                                    {
                                        rowNum = j;
                                        var nhanVien = nhanviensNhom[j];
                                        STT = j + 1;
                                        worksheet.Range[10 + j, 1, 10 + j, 1].Value2 =
                                            nhanVien.Ngay?.ToString("dd/MM/yyyy");
                                        worksheet.Range[10 + j, 2, 10 + j, 2].Value2 = STT.ToString();
                                        worksheet.Range[10 + j, 3, 10 + j, 3].Value2 = nhanVien.MaNhanVien;
                                        worksheet.Range[10 + j, 4, 10 + j, 4].Value2 = nhanVien.MaHoSo;
                                        worksheet.Range[10 + j, 5, 10 + j, 5].Value2 = nhanVien.TenNhanVien;
                                        var trongLuong = itemDycs.Where(
                                                x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                     x.MaSize == header.MaSize &&
                                                     x.MaCongViec == header.MaCongViec && x.Ngay == nhanVien.Ngay)
                                            .Select(x => (decimal)x.TrongLuong)
                                            .DefaultIfEmpty(0).Sum();

                                        worksheet.Range[10 + j, colData, 10 + j, colData].Value2 = trongLuong;

                                        tongTLT += trongLuong;
                                        if (j == nhanviensNhom.Count - 1)
                                        {
                                            worksheet.Range[10 + j + 1, colData, 10 + j + 1, colData].Value2 =
                                                tongTLT;
                                            worksheet.Range[10 + j + 1, colData, 10 + j + 1, colData].CellStyle.Font
                                                .Bold = true;
                                        }
                                    }

                                    var numNhanVien = nhanviensNhom.Select(x => x.MaNhanVien).Distinct().Count();
                                    worksheet.Range[7, 3].Value2 = numNhanVien;

                                }

                                worksheet.UsedRange.AutofitColumns();
                                //boder datta
                                worksheet.Range[10, 1, 10 + rowNum + 1, lastCol].BorderAround();
                                worksheet.Range[10, 1, 10 + rowNum + 1, lastCol].BorderInside();
                                worksheet.Range[10, 4, 10 + rowNum + 1, lastCol].NumberFormat =
                                    "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";

                                ////format tong cong
                                worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 5].Merge();
                                worksheet.Range[10 + rowNum + 1, 1].Value2 = @"TỔNG CỘNG";
                                worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 4].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[10 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, 3].Merge();
                                worksheet.Range[12 + rowNum + 1, 1].Value2 = @"BGĐ ZONE 1";
                                worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, 3].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 4, 12 + rowNum + 1, 7].Merge();
                                worksheet.Range[12 + rowNum + 1, 4].Value2 = @"TRƯỞNG PHÒNG NVTH";
                                worksheet.Range[12 + rowNum + 1, 4, 12 + rowNum + 1, 7].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 4].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 8, 12 + rowNum + 1, 11].Merge();
                                worksheet.Range[12 + rowNum + 1, 8].Value2 = @"TỔ TRƯỞNG";
                                worksheet.Range[12 + rowNum + 1, 8, 12 + rowNum + 1, 11].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 8].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 12, 12 + rowNum + 1, 15].Merge();
                                worksheet.Range[12 + rowNum + 1, 12].Value2 = @"THỐNG KÊ";
                                worksheet.Range[12 + rowNum + 1, 12, 12 + rowNum + 1, 15].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 12].CellStyle.Font.Bold = true;


                            }
                        }
                        markersProcessor.ApplyMarkers();
                        workbook.Version = ExcelVersion.Excel2007;
                        workbook.Worksheets.Remove(worksheetIndex);
                        worksheet.EnableSheetCalculations();
                        worksheet.Calculate();
                        worksheet.DisableSheetCalculations();
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            return new JsonResult(new
                            {
                                isSuccess = true,
                                Mesages = "Đã xuất file Excel thành công!",
                                ExcelContent = content,
                                ExcelFileName = $@"NangSuatChung-{ngayThangNam}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Không tìm thấy dữ liệu!"
                        });
                    }
                }
                else// năng suất riêng
                {
                    var appPath2 = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "T", "TemplateBangKeNSCongNhat_CMX2.xlsx");
                    var worksheetIndex = 0;
                    if (items != null && items.Any())
                    {
                        var _items = items.ToDataTable();
                        using var excelEngine = new ExcelEngine();
                        var application = excelEngine.Excel;
                        using var fileStream = new FileStream(appPath2, FileMode.Open);
                        var workbook = application.Workbooks.Open(fileStream);
                        // var worksheet = workbook.Worksheets[0]; gốc
                        var worksheet = workbook.Worksheets[worksheetIndex]; //chắt sửa ngày 8/2
                        worksheet.EnableSheetCalculations();
                        var markersProcessor = workbook.CreateTemplateMarkersProcessor();


                        //xử lý dữ liệu
                        if (_items.Rows.Count > 0)
                        {
                            var listOfNhom = items.Cast<dynamic>()
                                .ToList()
                                .Select(x => new { Nhom = (string)x.Nhom })
                                .Distinct()
                                .OrderBy(x => x.Nhom)
                                .ThenBy(x => x.Nhom)
                                .ToList();

                            var itemDycs = items.Cast<dynamic>().ToList();
                            // xử lý dữ liệu
                            for (var i = 0; i < listOfNhom.Count; i++)
                            {
                                //workbook.Worksheets.AddCopyAfter(workbook.Worksheets[0]);
                                var nhoms = listOfNhom[i];


                                //worksheet.Range[4, 4].Value2 = nhoms.Nhom; //nhớ thêm ngày ở đây

                                //markersProcessor.ApplyMarkers();
                                var nhanviensNhom = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(x => (string)x.Nhom == nhoms.Nhom)
                                    .GroupBy(x => new { x.MaNhanVien, x.MaHoSo, x.TenNhanVien, x.Nhom })
                                    .Select(
                                        x => new
                                        {
                                            MaNhanVien = (string)x.Key.MaNhanVien,
                                            MaHoSo = (string)x.Key.MaHoSo,
                                            TenNhanVien = (string)x.Key.TenNhanVien,
                                            Nhom = (string)x.Key.Nhom
                                        })
                                    .OrderBy(x => x.MaNhanVien)
                                    .ToList();
                                var headersNhom = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(x => (string)x.Nhom == nhoms.Nhom)
                                    .GroupBy(x => new { x.MaSize, x.SizeName, x.MaCongViec, x.CongViecName })
                                    .Select(
                                        x => new
                                        {
                                            MaSize = (string)x.Key.MaSize,
                                            SizeName = (string)x.Key.SizeName,
                                            CongViecName = (string)x.Key.CongViecName,
                                            MaCongViec = (string)x.Key.MaCongViec
                                            //Nhom = (string)x.Key.Nhom
                                        })
                                    .OrderBy(x => x.MaSize)
                                    .Where(
                                        x => x.MaSize != null &&
                                             x.SizeName != null &&
                                             x.MaCongViec != null &&
                                             x.CongViecName != null).Distinct()
                                    .ToList();
                                //var ngay = items.Cast<dynamic>()
                                //    .Select(x => x.Ngay).Distinct().ToList();
                                if (headersNhom.Count < 1) continue;


                                worksheet = workbook.Worksheets[worksheetIndex];

                                workbook.Worksheets.AddCopyAfter(workbook.Worksheets[worksheetIndex]);
                                worksheetIndex += 1;
                                if (nhoms.Nhom == null)
                                    worksheet.Name = $@"{"NULL"} ";
                                else
                                    worksheet.Name = $@"{nhoms.Nhom} ";

                                worksheet.Range[4, 4, 4, 12].Merge();
                                worksheet.Range[4, 4].Value =
                                    $@"{nhoms.Nhom} {xiNghiep.Ten}, Từ ngày {vmApp.FromDate:dd/MM/yyyy} đến {vmApp.DateReport.ToString("dd/MM/yyyy")}";
                                worksheet.Range[4, 4].CellStyle.Font.Bold = true;
                                //them tao header data
                                var numCol = 0;
                                var col = 5;
                                var lastCol = 0;
                                //var saveI = 0;

                                for (var y = 0; y < headersNhom.Count; y++)
                                {
                                    var header = headersNhom[y];
                                    if (y == 0)
                                    {
                                        col = 6;
                                        //numCol = 1;
                                        lastCol = col;
                                    }
                                    else
                                    {
                                        col = lastCol + 1;
                                        //numCol = 1;
                                        lastCol = col;
                                    }

                                    var headerGroupName = $@"{header.CongViecName} ({header.SizeName})";
                                    worksheet.Range[9, col].Value2 = $@"{headerGroupName}";
                                    worksheet.Range[9, 1, 9, lastCol].BorderAround();
                                    worksheet.Range[9, 1, 9, lastCol].BorderInside();
                                    //worksheet.Range[7, 4, 7, lastCol].Merge();
                                    //worksheet.Range[7, 4].Value2 = $@"BTP FILLET (Kg)";
                                    worksheet.Range[9, col].CellStyle.Font.Bold = true;
                                    worksheet.Range[9, col].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                                    worksheet.Range[9, col].WrapText = true;
                                }

                                var numColData = 0;
                                var colData = 6;
                                var lastColData = 0;
                                var rowNum = 0;
                                decimal tongTLT = 0;
                                decimal trongLuongVao = 0;
                                decimal trongLuongRa = 0;
                                var STT = 0;


                                for (var y = 0; y < headersNhom.Count; y++)
                                {
                                    var header = headersNhom[y];
                                    tongTLT = 0;


                                    if (y == 0)
                                    {
                                        colData = 6;

                                        lastColData = colData;
                                    }
                                    else
                                    {
                                        colData = lastColData + 1;
                                        lastColData = colData;
                                    }

                                    for (var j = 0; j < nhanviensNhom.Count; j++)
                                    {
                                        rowNum = j;


                                        //foreach (var d in ngay)
                                        //{
                                        var nhanVien = nhanviensNhom[j];
                                        STT = j + 1;
                                        worksheet.Range[10 + j, 1].ColumnWidth = 0;

                                        worksheet.Range[10 + j, 2, 10 + j, 2].Value2 = STT.ToString();
                                        worksheet.Range[10 + j, 3, 10 + j, 3].Value2 = nhanVien.MaNhanVien;
                                        worksheet.Range[10 + j, 4, 10 + j, 4].Value2 = nhanVien.MaHoSo;
                                        worksheet.Range[10 + j, 5, 10 + j, 5].Value2 = nhanVien.TenNhanVien;
                                        //dau vao, dau ra, dinh muc
                                        var trongLuong = itemDycs.Where(
                                                x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                     x.MaSize == header.MaSize &&
                                                     x.MaCongViec == header.MaCongViec)
                                            .Select(x => (decimal)x.TrongLuong)
                                            .DefaultIfEmpty(0).Sum();

                                        worksheet.Range[10 + j, colData, 10 + j, colData].Value2 = trongLuong;

                                        tongTLT += trongLuong;
                                        if (j == nhanviensNhom.Count - 1)
                                        {
                                            worksheet.Range[10 + j + 1, colData, 10 + j + 1, colData].Value2 =
                                                tongTLT;
                                            worksheet.Range[10 + j + 1, colData, 10 + j + 1, colData].CellStyle.Font
                                                .Bold = true;
                                        }

                                        //}
                                    }

                                    var numNhanVien = nhanviensNhom.Select(x => x.MaNhanVien).Distinct().Count();
                                    worksheet.Range[7, 3].Value2 = numNhanVien;

                                }

                                //boder datta
                                worksheet.Range[10, 1, 10 + rowNum + 1, lastCol].BorderAround();
                                worksheet.Range[10, 1, 10 + rowNum + 1, lastCol].BorderInside();
                                worksheet.Range[10, 4, 10 + rowNum + 1, lastCol].NumberFormat =
                                    "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";

                                ////format tong cong
                                worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 5].Merge();
                                worksheet.Range[10 + rowNum + 1, 1].Value2 = @"TỔNG CỘNG";
                                worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 4].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[10 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, 3].Merge();
                                worksheet.Range[12 + rowNum + 1, 1].Value2 = @"BGĐ ZONE 1";
                                worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, 3].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 4, 12 + rowNum + 1, 7].Merge();
                                worksheet.Range[12 + rowNum + 1, 4].Value2 = @"TRƯỞNG PHÒNG NVTH";
                                worksheet.Range[12 + rowNum + 1, 4, 12 + rowNum + 1, 7].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 4].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 8, 12 + rowNum + 1, 11].Merge();
                                worksheet.Range[12 + rowNum + 1, 8].Value2 = @"TỔ TRƯỞNG";
                                worksheet.Range[12 + rowNum + 1, 8, 12 + rowNum + 1, 11].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 8].CellStyle.Font.Bold = true;

                                worksheet.Range[12 + rowNum + 1, 12, 12 + rowNum + 1, 15].Merge();
                                worksheet.Range[12 + rowNum + 1, 12].Value2 = @"THỐNG KÊ";
                                worksheet.Range[12 + rowNum + 1, 12, 12 + rowNum + 1, 15].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[12 + rowNum + 1, 12].CellStyle.Font.Bold = true;
                            }
                        }
                        markersProcessor.ApplyMarkers();
                        workbook.Version = ExcelVersion.Excel2007;
                        //------------------------------------
                        //CHAT
                        workbook.Worksheets.Remove(worksheetIndex);
                        //------------------------------------
                        worksheet.EnableSheetCalculations();
                        worksheet.Calculate();
                        worksheet.DisableSheetCalculations();
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            return new JsonResult(new
                            {
                                isSuccess = true,
                                Mesages = "Đã xuất file Excel thành công!",
                                ExcelContent = content,
                                ExcelFileName = $@"NangSuatRieng-{ngayThangNam}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Không tìm thấy dữ liệu!"
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    isSuccess = false,
                    Mesages = $@"Lỗi! {ex.ToString()}"
                });
            }
        }
        #region Báo Cáo Hóa CHất
        public async Task<IEnumerable<object>> GetTongHopBaoCaoHoaChatNgayNguyenLieu(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetTongHopBaoCaoHoaChatNgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopBaoCaoHoaChat(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetTongHopBaoCaoHoaChat/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetNgayAndNguyenLieuHoaChat(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetNgayAndNguyenLieuHoaChat/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<T_TyLeHoaChatTheoNhom>> GetAllTyLeHoaChatTheoNhoms()
        {
            IEnumerable<T_TyLeHoaChatTheoNhom> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_TyLeHoaChatTheoNhom/Gets";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<T_TyLeHoaChatTheoNhom>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<T_HoaChat>> GetAllHoaChats()
        {
            IEnumerable<T_HoaChat> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_HoaChat/Gets";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<T_HoaChat>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        public async Task<IActionResult> ExportBaoCaoHoaChat(DateTime fromDate, DateTime dateTime, string xuongId, bool isCheckedNgayNguyenLieu)
        {

            try
            {
                var TemplateBangKeNSCongNhat_CMX = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "T", "TemplateBCNgamHoaChat.xls");
                //xử lý
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var xiNghiep = xuongId;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //var nhomsName = $@"";
                var items = new List<dynamic>();
                if (isCheckedNgayNguyenLieu == true)
                {
                    var rs = await GetTongHopBaoCaoHoaChatNgayNguyenLieu(fromDate, dateTime, xuongId);
                    items = rs.Cast<dynamic>().ToList();
                }
                else
                {
                    var rs = await GetTongHopBaoCaoHoaChat(fromDate, dateTime, xuongId);
                    items = rs != null ? rs.Cast<dynamic>().ToList() : new List<dynamic>();
                }


                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(TemplateBangKeNSCongNhat_CMX, FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    if (_items.Rows.Count > 0)
                    {
                        if (isCheckedNgayNguyenLieu == false)
                        {
                            var ngayAndNgayNguyenLieuRs = await GetNgayAndNguyenLieuHoaChat(fromDate, dateTime, xuongId);
                            var ngayAndNgayNguyenLieu = ngayAndNgayNguyenLieuRs!= null ? ngayAndNgayNguyenLieuRs.Cast<dynamic>().ToList() : new List<dynamic>();
                            var maxNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayGio).FirstOrDefault() ?? "";
                            var minNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MinNgayGio).FirstOrDefault() ?? "";
                            var maxNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayNguyenLieu)
                                .FirstOrDefault() ?? "";
                            var minNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MinNgayNguyenLieu)
                                .FirstOrDefault() ?? "";

                            var ngayGioToNgayGio =
                                $@"Ngày: {minNgayGio.ToString("dd/MM/yyyy HH:mm:ss")} - {maxNgayGio.ToString("dd/MM/yyyy HH:mm:ss")}";
                            worksheet.Range[3, 1].Value2 = $@"{ngayGioToNgayGio}";
                            worksheet.Range[3, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;

                            var ngayNLToNgayNL =
                                $@"Ngày Nguyên Liệu: {minNgayNguyenLieu.ToString("dd/MM/yyyy")} - {maxNgayNguyenLieu.ToString("dd/MM/yyyy")}";
                            worksheet.Range[4, 1].Value2 = $@"{ngayNLToNgayNL}";
                            //worksheet.Range[4, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        }
                        else
                        {
                            var ngayAndNgayNguyenLieuRs = await GetNgayAndNguyenLieuHoaChat_NgayNguyenLieu(fromDate, dateTime, xuongId);
                            var ngayAndNgayNguyenLieu = ngayAndNgayNguyenLieuRs!= null ? ngayAndNgayNguyenLieuRs.Cast<dynamic>().ToList() : new List<dynamic>();
                            var maxNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayGio).FirstOrDefault() ?? "";
                            var minNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MinNgayGio).FirstOrDefault() ?? "";
                            var maxNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayNguyenLieu)
                                .FirstOrDefault() ?? "";
                            var minNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MinNgayNguyenLieu)
                                .FirstOrDefault() ?? "";

                            var ngayGioToNgayGio =
                                $@"Ngày: {minNgayGio.ToString("dd/MM/yyyy HH:mm:ss")} - {maxNgayGio.ToString("dd/MM/yyyy HH:mm:ss")}";
                            worksheet.Range[3, 4].Value2 = $@"{ngayGioToNgayGio}";
                            worksheet.Range[3, 4].HorizontalAlignment = ExcelHAlign.HAlignCenter;

                            var ngayNLToNgayNL =
                                $@"Ngày Nguyên Liệu: {minNgayNguyenLieu.ToString("dd/MM/yyyy")} - {maxNgayNguyenLieu.ToString("dd/MM/yyyy")}";
                            worksheet.Range[4, 1].Value2 = $@"{ngayNLToNgayNL}";
                            worksheet.Range[4, 1].HorizontalAlignment = ExcelHAlign.HAlignLeft;
                        }

                        markersProcessor.AddVariable("items", _items, VariableTypeAction.DetectDataType);
                        var itesmDyc = items.Cast<dynamic>();
                        //var hoaChatSuDungs = new List<HoaChatSuDung>();
                        var listMaNhomHoaChatTheoSTT = itesmDyc.Select(x => new
                        {
                            x.STT,
                            x.MaNhomHoaChat,
                            TrongLuong = (decimal)x.TrongLuong
                        }).ToList();
                        for (var stt = 0; stt < listMaNhomHoaChatTheoSTT.Count; stt++)
                        {
                            var maNhomHoaChat = listMaNhomHoaChatTheoSTT[stt];
                            var listHoaChatTheoMaNhomHoaChatCuaSTT = GetAllTyLeHoaChatTheoNhoms().Result
                                .Where(x => x.MaNhomHoaChat == maNhomHoaChat.MaNhomHoaChat)
                                .Select(x => new
                                {
                                    x.Ten,
                                    x.MaHoaChat,
                                    x.TyLe
                                }).ToList();

                            for (var hc = 0; hc < listHoaChatTheoMaNhomHoaChatCuaSTT.Count; hc++)
                            {
                                var hoachat = listHoaChatTheoMaNhomHoaChatCuaSTT[hc];
                                var tenHoaChat = GetAllHoaChats().Result
                                    .Where(x => x.Ma == hoachat.MaHoaChat)
                                    .Select(x => x.Ten).FirstOrDefault() ?? "";
                                var tlHoaChatSuDung = maNhomHoaChat.TrongLuong * 1.4m * hoachat.TyLe / 100;
                                if (tenHoaChat.ToUpper() == "New".ToUpper())
                                    worksheet.Range[7 + stt, 9].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "TCM 505".ToUpper())
                                    worksheet.Range[7 + stt, 10].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "STPP".ToUpper())
                                    worksheet.Range[7 + stt, 11].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "P89E".ToUpper())
                                    worksheet.Range[7 + stt, 12].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "Europhost".ToUpper())
                                    worksheet.Range[7 + stt, 13].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "N05".ToUpper())
                                    worksheet.Range[7 + stt, 14].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "N97".ToUpper())
                                    worksheet.Range[7 + stt, 15].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "Metabisulfite".ToUpper())
                                    worksheet.Range[7 + stt, 16].Value2 = tlHoaChatSuDung.ToString("0.00");

                                if (tenHoaChat.ToUpper() == "Không Hoá Chất".ToUpper())
                                    worksheet.Range[7 + stt, 17].Value2 = tlHoaChatSuDung.ToString("0.00");
                                if (tenHoaChat.ToUpper() == "Salt".ToUpper())
                                    worksheet.Range[7 + stt, 18].Value2 = tlHoaChatSuDung.ToString("0.00");
                            }
                        }

                        worksheet.UsedRange.AutofitColumns();
                        worksheet.Range[7, 1, _items.Rows.Count + 6, 19].BorderAround();
                        worksheet.Range[7, 1, _items.Rows.Count + 6, 19].BorderInside();

                        worksheet.Range[_items.Rows.Count + 9, 1, _items.Rows.Count + 9, 3].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 1].Value2 = "BAN QUẢN ĐÓC";
                        worksheet.Range[_items.Rows.Count + 9, 1].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 1].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 9, 4, _items.Rows.Count + 9, 6].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 4].Value2 = "ĐIỀU HÀNH SẢN XUẤT";
                        worksheet.Range[_items.Rows.Count + 9, 4].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 4].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 9, 7, _items.Rows.Count + 9, 8].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 7].Value2 = "TT.HÓA CHẤT";
                        worksheet.Range[_items.Rows.Count + 9, 7].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 7].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 9, 9, _items.Rows.Count + 9, 10].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 9].Value2 = "THỐNG KÊ";
                        worksheet.Range[_items.Rows.Count + 9, 9].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 9].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Không tìm thấy dữ liệu!"
                        });
                    }

                    markersProcessor.ApplyMarkers();
                    workbook.Version = ExcelVersion.Excel2007;
                    worksheet.UsedRange.AutofitColumns();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return new JsonResult(new
                        {
                            isSuccess = true,
                            Mesages = "Đã xuất file Excel thành công!",
                            ExcelContent = content,
                            ExcelFileName = $@"NangSuatChung-{ngayThangNam}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Không tìm thấy dữ liệu!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Có lỗi xảy ra!" + ex.ToString()
                });
            }

        }
        #endregion
        public async Task<IActionResult> ExportBaoCaoHoaChat_DuLieuHoaChatDong(DateTime fromDate, DateTime dateTime, string xuongId, bool isCheckedNgayNguyenLieu)
        {
            try
            {
                var TemplateBangKeNSCongNhat_CMX = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "T", "TemplateBCNgamHoaChat.xls");
                //xử lý
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var xiNghiep = xuongId;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //var nhomsName = $@"";
                var items = new List<dynamic>();
                if (isCheckedNgayNguyenLieu)
                {
                    var rs = await GetTongHopBaoCaoHoaChatNgayNguyenLieu(fromDate, dateTime, xuongId);
                    items = rs.Cast<dynamic>().ToList();
                }

                else
                {
                    var rs = await GetTongHopBaoCaoHoaChat(fromDate, dateTime, xuongId);
                    items = rs.Cast<dynamic>().ToList();
                }


                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(TemplateBangKeNSCongNhat_CMX, FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    if (_items.Rows.Count > 0)
                    {
                        markersProcessor.AddVariable("items", _items, VariableTypeAction.DetectDataType);
                        var itesmDyc = items.Cast<dynamic>();
                        //var hoaChatSuDungs = new List<HoaChatSuDung>();
                        var listMaNhomHoaChatTheoSTT = itesmDyc.Select(x => new
                        {
                            x.STT,
                            x.MaNhomHoaChat,
                            TrongLuong = (decimal)x.TrongLuong
                        }).ToList();
                        for (var stt = 0; stt < listMaNhomHoaChatTheoSTT.Count; stt++)
                        {
                            var maNhomHoaChat = listMaNhomHoaChatTheoSTT[stt];
                            var listHoaChatTheoMaNhomHoaChatCuaSTT = GetAllTyLeHoaChatTheoNhoms().Result
                                .Where(x => x.MaNhomHoaChat == maNhomHoaChat.MaNhomHoaChat)
                                .Select(x => new
                                {
                                    x.Ten,
                                    x.MaHoaChat,
                                    x.TyLe
                                }).ToList();

                            for (var hc = 0; hc < listHoaChatTheoMaNhomHoaChatCuaSTT.Count; hc++)
                            {
                                var hoachat = listHoaChatTheoMaNhomHoaChatCuaSTT[hc];
                                var tenHoaChat = GetAllHoaChats().Result
                                    .Where(x => x.Ma == hoachat.MaHoaChat)
                                    .Select(x => x.Ten).FirstOrDefault() ?? "";
                                var tlHoaChatSuDung = maNhomHoaChat.TrongLuong * 1.4m * hoachat.TyLe;
                                //if (tenHoaChat == "New")
                                //{
                                //    worksheet.Range[7 + stt, 9].Value2 = tlHoaChatSuDung.ToString();
                                //}

                                //if (tenHoaChat == "TCM505")
                                //{
                                //    worksheet.Range[7 + stt, 10].Value2 = tlHoaChatSuDung.ToString();
                                //}

                                //if (tenHoaChat == "STTP")
                                //{
                                //    worksheet.Range[7 + stt, 11].Value2 = tlHoaChatSuDung.ToString();
                                //}

                                //if (tenHoaChat == "P89E")
                                //{
                                //    worksheet.Range[7 + stt, 12].Value2 = tlHoaChatSuDung.ToString();
                                //}

                                //if (tenHoaChat == "Europhost")
                                //{
                                //    worksheet.Range[7 + stt, 14].Value2 = tlHoaChatSuDung.ToString();
                                //}

                                //if (tenHoaChat == "N05")
                                //{
                                //    worksheet.Range[7 + stt, 14].Value2 = tlHoaChatSuDung.ToString();
                                //}

                                //if (tenHoaChat == "N97")
                                //{
                                //    worksheet.Range[7 + stt, 15].Value2 = tlHoaChatSuDung.ToString();
                                //}

                                //if (tenHoaChat == "Muối")
                                //{
                                //    worksheet.Range[7 + stt, 16].Value2 = tlHoaChatSuDung.ToString();
                                //}
                            }
                        }

                        worksheet.UsedRange.AutofitColumns();
                        worksheet.Range[7, 1, _items.Rows.Count + 6, 17].BorderAround();
                        worksheet.Range[7, 1, _items.Rows.Count + 6, 17].BorderInside();

                        worksheet.Range[_items.Rows.Count + 9, 1, _items.Rows.Count + 9, 3].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 1].Value2 = "BAN QUẢN ĐÓC";
                        worksheet.Range[_items.Rows.Count + 9, 1].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 1].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 9, 4, _items.Rows.Count + 9, 6].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 4].Value2 = "ĐIỀU HÀNH SẢN XUẤT";
                        worksheet.Range[_items.Rows.Count + 9, 4].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 4].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 9, 7, _items.Rows.Count + 9, 8].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 7].Value2 = "TT.HÓA CHẤT";
                        worksheet.Range[_items.Rows.Count + 9, 7].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 7].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 9, 9, _items.Rows.Count + 9, 10].Merge();
                        worksheet.Range[_items.Rows.Count + 9, 9].Value2 = "THỐNG KÊ";
                        worksheet.Range[_items.Rows.Count + 9, 9].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 9, 9].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Không tìm thấy dữ liệu!"
                        });
                    }

                    markersProcessor.ApplyMarkers();
                    workbook.Version = ExcelVersion.Excel2007;
                    worksheet.UsedRange.AutofitColumns();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return new JsonResult(new
                        {
                            isSuccess = true,
                            Mesages = "Đã xuất file Excel thành công!",
                            ExcelContent = content,
                            ExcelFileName = $@"NangSuatChung-{ngayThangNam}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Không tìm thấy dữ liệu!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = ex.ToString()
                });
            }
        }

        public async Task<IEnumerable<object>> GetTongHopBaoCaoHangNgay(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetTongHopBaoCaoHangNgay/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> ExportBaoCaoTongHopHangNgay(DateTime fromDate, DateTime dateTime, string xuongId, bool isCheckedNgayNguyenLieu)
        {
            try
            {
                var TemplateBangKeNSCongNhat_CMX = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "T", "TemplateBCNgamHoaChat.xls");
                //xử lý
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var xiNghiep = xuongId;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //var nhomsName = $@"";
                var items = new List<dynamic>();
                isCheckedNgayNguyenLieu = false;

                var rs = await GetTongHopBaoCaoHangNgay(fromDate, dateTime, xuongId);
                items = rs!= null ? rs.Cast<dynamic>().ToList() : new List<dynamic>();
                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(TemplateBangKeNSCongNhat_CMX, FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    if (_items.Rows.Count > 0)
                    {
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Không tìm thấy dữ liệu!"
                        });
                    }

                    markersProcessor.ApplyMarkers();
                    workbook.Version = ExcelVersion.Excel2007;
                    worksheet.UsedRange.AutofitColumns();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return new JsonResult(new
                        {
                            isSuccess = true,
                            Mesages = "Đã xuất file Excel thành công!",
                            ExcelContent = content,
                            ExcelFileName = $@"NangSuatChung-{ngayThangNam}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Không tìm thấy dữ liệu!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = ex.ToString()
                });
            }
        }

        private class HoaChatSuDung
        {
            public string Ten { get; set; }
            public decimal TLPhuGia { get; set; }
        }
        public async Task<IEnumerable<object>> GetChiTietsDateTimeToDateTimeNgayNguyenLieu(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCanThuMua/GetChiTietsDateTimeToDateTimeNgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetPhieuCanChiTietsDateTimeToDateTime(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetPhieuCanChiTietsDateTimeToDateTime/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetChiTietsDateTimeToDateTime(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCanThuMua/GetChiTietsDateTimeToDateTime/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public class PhanCoThuMua
        {
            public dynamic IsPhanCo { get; set; }
            public dynamic MaKhachHang { get; set; }
            public dynamic Ngay { get; set; }
            public dynamic NgayNguyenLieu { get; set; }
            public dynamic NhomLo { get; set; }
            public dynamic QuyTrinhName { get; set; }
            public dynamic SanPhamName { get; set; }
            public dynamic SizeTPName { get; set; }
            public dynamic ThongTinPhuName { get; set; }
            public dynamic TrongLuong { get; set; }
            public dynamic VoXo { get; set; }
        }

        public async Task<IActionResult> ExportListTongHopSauRaiMay2(DateTime fromDate, DateTime dateTime, string xuongId, bool isCheckedNgayNguyenLieu)
        {

            try
            {
                var TemplateBangKeNSCongNhat_CMX = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "T", "TemplateListTHSauRai_CMX2.xlsx");
                //xử lý
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var xiNghiep = xuongId;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //var nhomsName = $@"";
                var items = new List<dynamic>();
                var _itemPC = new List<dynamic>();
                var _itemTM = new List<dynamic>();
                if (isCheckedNgayNguyenLieu)
                {
                    var rsPC = await GetChiTietTom(fromDate, dateTime, xuongId);
                    _itemPC = rsPC!= null ? rsPC.Cast<dynamic>().ToList() : new List<dynamic>();
                    var rsTM = await GetChiTietsDateTimeToDateTimeNgayNguyenLieu(fromDate, dateTime, xuongId);
                    _itemTM = rsTM!= null ? rsTM.Cast<dynamic>().ToList() : new List<dynamic>();

                }

                else
                {
                    var rsPC = await GetPhieuCanChiTietsDateTimeToDateTime(fromDate, dateTime, xuongId);
                    _itemPC =rsPC!= null ? rsPC.Cast<dynamic>().ToList() : new List<dynamic>();

                    var rsTM = await GetChiTietsDateTimeToDateTime(fromDate, dateTime, xuongId);
                    _itemTM = rsTM!= null ? rsTM.Cast<dynamic>().ToList() : new List<dynamic>();

                }

                if (_itemPC != null && _itemPC.Any())
                    items.AddRange(_itemPC.Select(x => new PhanCoThuMua
                    {
                        IsPhanCo = x.IsPhanCo,
                        SanPhamName = x.SanPhamName,
                        NgayNguyenLieu = x.NgayNguyenLieu,
                        SizeTPName = x.SizeTPName,
                        VoXo = (dynamic)((x.VoXo).ToString()),
                        QuyTrinhName = x.QuyTrinhName,
                        MaKhachHang = x.MaKhachHang,
                        ThongTinPhuName = x.ThongTinPhuName,
                        NhomLo = x.NhomLo,
                        TrongLuong = x.TrongLuong,
                        Ngay = x.Ngay
                    }).ToList());
                if (_itemTM != null && _itemTM.Any())
                    items.AddRange(_itemTM.Select(x => new PhanCoThuMua
                    {
                        IsPhanCo = (dynamic)(true),
                        SanPhamName = x.SanPhamName,
                        NgayNguyenLieu = x.NgayNguyenLieu,
                        SizeTPName = x.BaoBiName,
                        VoXo = x.VoXoTB,
                        QuyTrinhName = x.QuyTrinhName,
                        MaKhachHang = x.MaKhachHang,
                        ThongTinPhuName = x.ThongTinPhuName,
                        NhomLo = x.MaLo,
                        TrongLuong = x.TrongLuong,
                        Ngay = x.Ngay
                    }).ToList());
                var worksheetIndex = 0;
                if (items != null && items.Any())
                {
                    var _items = items
                        //.Cast<object>()
                        //.Where(x => (bool)x.IsPhanCo == true)
                        //.ToList()
                        .ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(TemplateBangKeNSCongNhat_CMX, FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    worksheet.EnableSheetCalculations();
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();


                    //xử lý dữ liệu
                    if (_items.Rows.Count > 0)
                    {
                        var listSanPham = items
                            .ToList()
                            .Select(
                                x => new
                                {
                                    SanPhamName = (string)x.SanPhamName,
                                    x.NgayNguyenLieu,
                                    IsPhanCo = (bool)x.IsPhanCo
                                })
                            .Distinct()
                            .Where(x => x.IsPhanCo)
                            .OrderBy(x => x.SanPhamName)
                            .ThenBy(x => x.SanPhamName)
                            .ToList();

                        var itemDycs = items.ToList();
                        // xử lý dữ liệu
                        for (var i = 0; i < listSanPham.Count; i++)
                        {
                            var sanPham = listSanPham[i];
                            //thêm mới wordsheet
                            worksheet = workbook.Worksheets[i];
                            workbook.Worksheets.AddCopyAfter(workbook.Worksheets[i]);
                            worksheetIndex += 1;
                            var ngayNguyenLieu = $@"{sanPham.NgayNguyenLieu}";
                            worksheet.Name =
                                $@"{sanPham.SanPhamName}_{sanPham.NgayNguyenLieu.ToString("dd-MM-yyyy")}";
                            //Ngày nguyên liệu
                            worksheet.Range[5, 2].Merge();
                            worksheet.Range[5, 3].Value =
                                $@"{sanPham.SanPhamName} {sanPham.NgayNguyenLieu.ToString("dd/MM/yyyy")} ";
                            worksheet.Range[5, 3].CellStyle.Font.Bold = true;

                            var listTHSauRai = items
                                .ToList()
                                .Where(
                                    x => (string)x.SanPhamName == sanPham.SanPhamName &&
                                         x.NgayNguyenLieu == sanPham.NgayNguyenLieu &&
                                         (bool)x.IsPhanCo == sanPham.IsPhanCo)
                                .GroupBy(
                                    x => new
                                    {
                                        x.SizeTPName,
                                        x.VoXo,
                                        x.QuyTrinhName,
                                        x.MaKhachHang,
                                        x.ThongTinPhuName
                                        //x.PhuGiaName bỏ theo yc của a ngôn cmx 9h57 ngày 20/07/2023 vì báo cáo bị tách do HC và KHC
                                    })
                                .Select(
                                    x => new
                                    {
                                        x.Key.SizeTPName,
                                        x.Key.VoXo,
                                        x.Key.QuyTrinhName,
                                        x.Key.MaKhachHang,
                                        x.Key.ThongTinPhuName
                                        //x.Key.PhuGiaName bỏ theo yc của a ngôn cmx 9h57 ngày 20/07/2023 vì báo cáo bị tách do HC và KHC
                                    })
                                .OrderBy(x => x.VoXo)
                                .ToList();

                            var headersNhomLo = items
                                .ToList()
                                .Where(
                                    x => (string)x.SanPhamName == sanPham.SanPhamName &&
                                         x.NgayNguyenLieu == sanPham.NgayNguyenLieu &&
                                         (bool)x.IsPhanCo == sanPham.IsPhanCo)
                                .GroupBy(x => new { x.NhomLo })
                                .Select(
                                    x => new
                                    {
                                        NhomLo = (string)x.Key.NhomLo

                                        //Nhom = (string)x.Key.Nhom
                                    })
                                .OrderBy(x => x.NhomLo)
                                .Where(x => x.NhomLo != null) //Where phân cỡ
                                .ToList();
                            //LẤY NGÀY RẢI "Chắt làm đại chứ không biết đúng hay sai
                            var ngayRai = items
                                .ToList()
                                .Where(
                                    x => (string)x.SanPhamName == sanPham.SanPhamName &&
                                         x.NgayNguyenLieu == sanPham.NgayNguyenLieu &&
                                         (bool)x.IsPhanCo == sanPham.IsPhanCo)
                                .GroupBy(x => new { x.Ngay })
                                .Select(x => new { Ngay = (DateTime)x.Key.Ngay })
                                .OrderBy(x => x.Ngay)
                                .Where(x => x.Ngay != null) //Where phân cỡ
                                .FirstOrDefault();


                            //them tao header data
                            var numCol = 0;
                            var col = 6;
                            var lastCol = 0;
                            //var saveI = 0;

                            for (var y = 0; y < headersNhomLo.Count; y++)
                            {
                                var header = headersNhomLo[y];
                                if (y == 0)
                                {
                                    col = 6;
                                    lastCol = col;
                                }
                                else
                                {
                                    col = lastCol + 1;
                                    lastCol = col;
                                }

                                var headerGroupName = $@"{header.NhomLo}";
                                worksheet.Range[6, col, 7, lastCol].Merge();
                                worksheet.Range[6, col].Value2 = $@"{headerGroupName}";
                                worksheet.Range[6, 1, 7, lastCol].BorderAround();
                                worksheet.Range[6, 1, 7, lastCol].BorderInside();
                                worksheet.Range[6, col].CellStyle.Font.Bold = true;
                                worksheet.Range[6, col].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                                worksheet.Range[6, col].VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range[6, col].WrapText = true;
                            }

                            var numColData = 0;
                            var colData = 6;
                            var lastColData = 0;
                            var rowNum = 0;
                            decimal tongTLT = 0;
                            decimal trongLuongVao = 0;
                            decimal trongLuongRa = 0;
                            var STT = 0;

                            for (var y = 0; y < headersNhomLo.Count; y++)
                            {
                                var header = headersNhomLo[y];
                                tongTLT = 0;


                                if (y == 0)
                                {
                                    colData = 6;

                                    lastColData = colData;
                                }
                                else
                                {
                                    colData = lastColData + 1;
                                    lastColData = colData;
                                }

                                for (var j = 0; j < listTHSauRai.Count; j++)
                                {
                                    rowNum = j;

                                    var listSauRai = listTHSauRai[j];

                                    worksheet.Range[8 + j, 1, 8 + j, 1].Value2 = $"'{listSauRai.SizeTPName}";
                                    worksheet.Range[8 + j, 2, 8 + j, 2].Value2 = listSauRai.VoXo;
                                    ////cột SIZE K/HÀNG cũ
                                    //var sizeK_Hang =
                                    //    $@"{listSauRai.QuyTrinhName} ({listSauRai.MaKhachHang}) {listSauRai.PhuGiaName} {listSauRai.ThongTinPhuName}";
                                    //worksheet.Range[8 + j, 3, 8 + j, 3].Value2 = sizeK_Hang;

                                    //cột SIZE K/HÀNG mới
                                    var quiTrinh = $@"{listSauRai.QuyTrinhName}";
                                    var khachHang = $@"'{listSauRai.MaKhachHang}";
                                    var thongTinPhu = $@"{listSauRai.ThongTinPhuName}";
                                    worksheet.Range[8 + j, 3].Value2 = quiTrinh;
                                    worksheet.Range[8 + j, 4].Value2 = khachHang;
                                    worksheet.Range[8 + j, 5].Value2 = thongTinPhu;

                                    ////dau vao, dau ra, dinh muc
                                    var trongLuong = itemDycs.Where(
                                            x => x.SizeTPName == listSauRai.SizeTPName &&
                                                 x.VoXo == listSauRai.VoXo &&
                                                 x.QuyTrinhName == listSauRai.QuyTrinhName &&
                                                 x.MaKhachHang == listSauRai.MaKhachHang &&
                                                 x.ThongTinPhuName == listSauRai.ThongTinPhuName &&
                                                 //x.PhuGiaName == listSauRai.PhuGiaName && bỏ theo yc của a ngôn cmx 9h57 ngày 20/07/2023 vì báo cáo bị tách do HC và KHC
                                                 x.NhomLo == header.NhomLo &&
                                                 x.NgayNguyenLieu == sanPham.NgayNguyenLieu &&
                                                 (bool)x.IsPhanCo == sanPham.IsPhanCo &&
                                                 x.SanPhamName == sanPham.SanPhamName)
                                        .Select(x => (decimal)x.TrongLuong)
                                        .DefaultIfEmpty(0)
                                        .Sum();

                                    worksheet.Range[8 + j, colData, 8 + j, colData].Value2 = trongLuong;
                                    worksheet.Range[8 + j, colData, 8 + j, colData].HorizontalAlignment =
                                        ExcelHAlign.HAlignRight;
                                    worksheet.Range[8 + j, colData, 8 + j, colData].CellStyle.Font.Bold = false;
                                    //tính tổng từng cột
                                    tongTLT += trongLuong;
                                    if (j == listTHSauRai.Count - 1)
                                    {
                                        worksheet.Range[8 + j + 1, colData, 8 + j + 1, colData].Value2 =
                                            tongTLT;
                                        worksheet.Range[8 + j + 1, colData, 8 + j + 1, colData].CellStyle.Font
                                            .Bold = true;
                                        worksheet.Range[8 + j + 1, colData, 8 + j + 1, colData].CellStyle.Font
                                            .FontName = "Times New Roman";
                                        worksheet.Range[8 + j + 1, colData, 8 + j + 1, colData].CellStyle.Font
                                            .Size = 12;
                                        worksheet.Range[8 + rowNum, lastColData + 1].HorizontalAlignment =
                                            ExcelHAlign.HAlignRight;
                                    }


                                    //tính tổng từng hàng
                                    worksheet.Range[8 + rowNum, lastColData + 1].Formula = "=Sum(" +
                                        worksheet.Range[8 + rowNum, 6].Address +
                                        ":" +
                                        worksheet.Range[8 + rowNum, lastColData].Address +
                                        ")";
                                    worksheet.Range[8 + rowNum, lastColData + 1].CellStyle.Font.Bold = true;
                                    worksheet.Range[8 + rowNum, lastColData + 1].CellStyle.Font.FontName =
                                        "Times New Roman";
                                    worksheet.Range[8 + rowNum, lastColData + 1].CellStyle.Font.Size = 12;
                                    worksheet.Range[8 + rowNum, lastColData + 1].HorizontalAlignment =
                                        ExcelHAlign.HAlignRight;
                                }
                            }

                            //border full
                            var lastBorder = 12;
                            if (lastCol + 1 > lastBorder) lastBorder++;
                            for (var indexMerCol = 1; indexMerCol <= lastBorder; indexMerCol++)
                                worksheet.Range[6, lastCol + indexMerCol, 7, lastCol + indexMerCol].Merge();
                            worksheet.Range[6, 1, 8 + rowNum + 1, lastBorder]
                                .BorderAround(); //8 + rowNum + 2, 8, 8 + rowNum + 2, 12
                            worksheet.Range[6, 1, 8 + rowNum + 1, lastBorder].BorderInside();
                            worksheet.Range[8, 1, 8 + rowNum + 1, lastBorder].NumberFormat =
                                "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";
                            //boder datta
                            worksheet.Range[8, 1, 8 + rowNum + 1, lastCol + 1]
                                .BorderAround(); //8 + rowNum + 2, 8, 8 + rowNum + 2, 12
                            worksheet.Range[8, 1, 8 + rowNum + 1, lastCol + 1].BorderInside();
                            worksheet.Range[8, 1, 8 + rowNum + 1, lastCol + 1].NumberFormat =
                                "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";
                            //ngay rải
                            worksheet.Range[5, lastBorder - 1].Value2 = @"Ngày rải: ";
                            worksheet.Range[5, lastBorder - 1].CellStyle.Font.FontName = "Times New Roman";
                            worksheet.Range[5, lastBorder - 1].CellStyle.Font.Size = 12;
                            worksheet.Range[5, lastBorder - 1].VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range[5, lastBorder].Value2 = ngayRai.Ngay.ToString("dd/MM/yyy");
                            worksheet.Range[5, lastBorder].CellStyle.Font.FontName = "Times New Roman";
                            worksheet.Range[5, lastBorder].CellStyle.Font.Size = 12;
                            worksheet.Range[5, lastBorder].CellStyle.Font.Bold = true;
                            // tổng hàng ngang
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].Merge();
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].BorderAround();
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].BorderInside();
                            worksheet.Range[6, lastCol + 1].Value2 = @"TỔNG";
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].VerticalAlignment =
                                ExcelVAlign.VAlignCenter;
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].CellStyle.Font.Bold = true;
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].CellStyle.Font.FontName =
                                "Times New Roman";
                            worksheet.Range[6, lastCol + 1, 7, lastCol + 1].CellStyle.Font.Size = 12;
                            //tính tổng all
                            worksheet.Range[8 + rowNum + 1, lastColData + 1].Formula = "=Sum(" +
                                worksheet.Range[8 + rowNum + 1, 6].Address +
                                ":" +
                                worksheet.Range[8 + rowNum + 1, lastColData].Address +
                                ")";
                            worksheet.Range[8 + rowNum + 1, colData + 1, 8 + rowNum + 1, colData + 1].CellStyle
                                .Font.Bold = true;
                            worksheet.Range[8 + rowNum + 1, colData + 1, 8 + rowNum + 1, colData + 1].CellStyle
                                .Font.FontName = "Times New Roman";
                            worksheet.Range[8 + rowNum + 1, colData + 1, 8 + rowNum + 1, colData + 1].CellStyle
                                .Font.Size = 12;
                            worksheet.Range[8 + rowNum + 1, colData + 1, 8 + rowNum + 1, colData + 1].CellStyle
                                .Font.Color = ExcelKnownColors.Red;
                            worksheet.Range[8 + rowNum + 1, colData + 1, 8 + rowNum + 1, colData + 1]
                                    .HorizontalAlignment =
                                ExcelHAlign.HAlignRight;
                            ////format tong cong
                            worksheet.Range[8 + rowNum + 1, 1, 8 + rowNum + 1, 5].Merge();
                            worksheet.Range[8 + rowNum + 1, 1].Value2 = @"TỔNG CỘNG";
                            worksheet.Range[8 + rowNum + 1, 1, 8 + rowNum + 1, 5].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[8 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                            worksheet.Range[8 + rowNum + 2, 1, 8 + rowNum + 2, 2].Merge();
                            worksheet.Range[8 + rowNum + 2, 1].Value2 = @"QUẢN ĐỐC";
                            worksheet.Range[8 + rowNum + 2, 1, 8 + rowNum + 2, 2].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[8 + rowNum + 2, 1].CellStyle.Font.Bold = true;

                            worksheet.Range[8 + rowNum + 2, 3, 8 + rowNum + 2, 5].Merge();
                            worksheet.Range[8 + rowNum + 2, 3].Value2 = @"ĐHSX";
                            worksheet.Range[8 + rowNum + 2, 3, 8 + rowNum + 2, 5].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[8 + rowNum + 2, 3].CellStyle.Font.Bold = true;

                            worksheet.Range[8 + rowNum + 2, 6, 8 + rowNum + 2, 7].Merge();
                            worksheet.Range[8 + rowNum + 2, 6].Value2 = @"TT.PHÂN CỠ";
                            worksheet.Range[8 + rowNum + 2, 6, 8 + rowNum + 2, 7].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[8 + rowNum + 2, 6].CellStyle.Font.Bold = true;

                            worksheet.Range[8 + rowNum + 2, 8, 8 + rowNum + 2, 12].Merge();
                            worksheet.Range[8 + rowNum + 2, 8].Value2 = @"THỐNG KÊ";
                            worksheet.Range[8 + rowNum + 2, 8, 8 + rowNum + 2, 12].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[8 + rowNum + 2, 8].CellStyle.Font.Bold = true;

                            worksheet.UsedRange.AutofitColumns();
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Không tìm thấy dữ liệu!"
                        });
                    }

                    markersProcessor.ApplyMarkers();
                    workbook.Version = ExcelVersion.Excel2007;
                    //XÓA SHEEET CUỐI CÙNG
                    workbook.Worksheets.Remove(worksheetIndex);
                    //------------------------------------
                    worksheet.EnableSheetCalculations();
                    worksheet.Calculate();
                    worksheet.DisableSheetCalculations();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return new JsonResult(new
                        {
                            isSuccess = true,
                            Mesages = "Đã xuất file Excel thành công!",
                            ExcelContent = content,
                            ExcelFileName = $@"NangSuatChung-{ngayThangNam}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Không tìm thấy dữ liệu!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = ex.ToString()
                });
            }
        }
        public async Task<IEnumerable<object>> GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieuThuMua(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCanThuMuas/GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopBaoCaoSauRaiMayPhanCo(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetTongHopBaoCaoSauRaiMayPhanCo/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopBaoCaoSauRaiMayPhanCoThuMua(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCanThuMuas/GetTongHopBaoCaoSauRaiMayPhanCo/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetNgayAndNguyenLieuPhanCo(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetNgayAndNguyenLieuPhanCo/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetNgayAndNguyenLieuPhanCoThuMua(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCanThuMuas/GetNgayAndNguyenLieuPhanCo/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCan/GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetNgayAndNguyenLieuPhanCo_NgayNguyenLieuThuMua(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/T_PhieuCanThuMuas/GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> ExportBCTHSauRaiMayPC(DateTime fromDate, DateTime dateTime, string xuongId, bool isCheckedNgayNguyenLieu)
        {

            try
            {
                var TemplateBangKeNSCongNhat_CMX = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "T", "TemplateListTHSauRai_CMX2.xlsx");
                //xử lý
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var xiNghiep = XiNghiepViewModel.Instance.XiNghiepSelectedItem?.Ten;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //var nhomsName = $@"";
                var items = new List<dynamic>();
                var _itemPC = new List<dynamic>();
                var _itemTM = new List<dynamic>();
                if (isCheckedNgayNguyenLieu)
                {
                    var rsPC = await GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieu(fromDate, dateTime, xuongId);
                    _itemPC = rsPC!= null ? rsPC.Cast<dynamic>().ToList() : new List<dynamic>();
                    var rsTM = await GetTongHopBaoCaoSauRaiMayPhanCoNgayNguyenLieuThuMua(fromDate, dateTime, xuongId);
                    _itemTM = rsTM!= null ? rsTM.Cast<dynamic>().ToList() : new List<dynamic>();
                }
                else
                {
                    var rsPC = await GetTongHopBaoCaoSauRaiMayPhanCo(fromDate, dateTime, xuongId);
                    _itemPC = rsPC!= null ? rsPC.Cast<dynamic>().ToList() : new List<dynamic>();
                    var rsTM = await GetTongHopBaoCaoSauRaiMayPhanCoThuMua(fromDate, dateTime, xuongId);
                    _itemTM = rsTM!= null ? rsTM.Cast<dynamic>().ToList() : new List<dynamic>();
                }

                if (_itemPC != null && _itemPC.Any()) items.AddRange(_itemPC);
                if (_itemTM != null && _itemTM.Any()) items.AddRange(_itemTM);
                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(TemplateBangKeNSCongNhat_CMX, FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    if (_items.Rows.Count > 0)
                    {
                        if (isCheckedNgayNguyenLieu == false)
                        {
                            var ngayAndNgayNguyenLieu = new List<dynamic>();
                            var rsNgayPC = await GetNgayAndNguyenLieuPhanCo(fromDate, dateTime, xuongId);
                            var ngayPC = rsNgayPC!= null ? rsNgayPC.Cast<dynamic>().ToList() : new List<dynamic>();
                            var rsNgayTM = await GetNgayAndNguyenLieuPhanCoThuMua(fromDate, dateTime, xuongId);
                            var ngayTM = rsNgayTM!= null ? rsNgayTM.Cast<dynamic>().ToList() : new List<dynamic>();
                            if (ngayPC.Any())
                            {
                                ngayAndNgayNguyenLieu.AddRange(ngayPC);
                            }

                            if (ngayTM.Any())
                            {
                                ngayAndNgayNguyenLieu.AddRange(ngayTM);
                            }
                            var maxNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayGio).Max() ?? "";
                            var minNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MinNgayGio).Min() ?? "";
                            var maxNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayNguyenLieu).Max()
                                ?? "";
                            var minNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MinNgayNguyenLieu).Min()
                                 ?? "";

                            var ngayGioToNgayGio =
                                $@"Ngày: {minNgayGio.ToString("dd/MM/yyyy HH:mm:ss")} - {maxNgayGio.ToString("dd/MM/yyyy HH:mm:ss")}";
                            worksheet.Range[3, 4].Value2 = $@"{ngayGioToNgayGio}";
                            worksheet.Range[3, 4].HorizontalAlignment = ExcelHAlign.HAlignCenter;

                            var ngayNLToNgayNL =
                                $@"Ngày Nguyên Liệu: {minNgayNguyenLieu.ToString("dd/MM/yyyy")} - {maxNgayNguyenLieu.ToString("dd/MM/yyyy")}";
                            worksheet.Range[4, 1].Value2 = $@"{ngayNLToNgayNL}";
                            //worksheet.Range[4, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        }
                        else
                        {
                            var ngayAndNgayNguyenLieu = new List<dynamic>();
                            var rsNgayPC = await GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu(fromDate, dateTime, xuongId);
                            var ngayPC = rsNgayPC!= null ? rsNgayPC.Cast<dynamic>().ToList() : new List<dynamic>();

                            var rsNgayTM = await GetNgayAndNguyenLieuPhanCo_NgayNguyenLieuThuMua(fromDate, dateTime, xuongId);
                            var ngayTM = rsNgayTM!= null ? rsNgayTM.Cast<dynamic>().ToList() : new List<dynamic>();
                            if (ngayPC.Any())
                            {
                                ngayAndNgayNguyenLieu.AddRange(ngayPC);
                            }

                            if (ngayTM.Any())
                            {
                                ngayAndNgayNguyenLieu.AddRange(ngayTM);
                            }
                            //var ngayAndNgayNguyenLieu = T_PhieuCanViewModel.Ins
                            //    .GetNgayAndNguyenLieuPhanCo_NgayNguyenLieu<dynamic>(
                            //        AppViewModel.Ins.FromDate,
                            //        AppViewModel.Ins.DateReport,
                            //        XiNghiepViewModel.Ins.XiNghiepSelectedItem?.Ma);
                            var maxNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayGio).Max() ?? "";
                            var minNgayGio =
                                ngayAndNgayNguyenLieu.Select(x => x?.MinNgayGio).Min() ?? "";
                            var maxNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MaxNgayNguyenLieu).Max()
                                 ?? "";
                            var minNgayNguyenLieu = ngayAndNgayNguyenLieu.Select(x => x?.MinNgayNguyenLieu).Min()
                                ?? "";

                            var ngayGioToNgayGio =
                                $@"Ngày: {minNgayGio.ToString("dd/MM/yyyy HH:mm:ss")} - {maxNgayGio.ToString("dd/MM/yyyy HH:mm:ss")}";
                            worksheet.Range[3, 4].Value2 = $@"{ngayGioToNgayGio}";
                            worksheet.Range[3, 4].HorizontalAlignment = ExcelHAlign.HAlignCenter;

                            var ngayNLToNgayNL =
                                $@"Ngày Nguyên Liệu: {minNgayNguyenLieu.ToString("dd/MM/yyyy")} - {maxNgayNguyenLieu.ToString("dd/MM/yyyy")}";
                            worksheet.Range[4, 1].Value2 = $@"{ngayNLToNgayNL}";
                            //worksheet.Range[4, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        }


                        markersProcessor.AddVariable("items", _items, VariableTypeAction.DetectDataType);
                        markersProcessor.ApplyMarkers();
                        workbook.Version = ExcelVersion.Excel2007;
                        worksheet.UsedRange.AutofitColumns();
                        worksheet.Range[6, 1, _items.Rows.Count + 5, 10].BorderAround();
                        worksheet.Range[6, 1, _items.Rows.Count + 5, 10].BorderInside();

                        worksheet.Range[_items.Rows.Count + 7, 1, _items.Rows.Count + 7, 3].Merge();
                        worksheet.Range[_items.Rows.Count + 7, 1].Value2 = "BAN QUẢN ĐÓC";
                        worksheet.Range[_items.Rows.Count + 7, 1].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 7, 1].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 7, 4, _items.Rows.Count + 7, 6].Merge();
                        worksheet.Range[_items.Rows.Count + 7, 4].Value2 = "ĐIỀU HÀNH SẢN XUẤT";
                        worksheet.Range[_items.Rows.Count + 7, 4].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 7, 4].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 7, 7, _items.Rows.Count + 7, 8].Merge();
                        worksheet.Range[_items.Rows.Count + 7, 7].Value2 = "TT.PHÂN CỠ";
                        worksheet.Range[_items.Rows.Count + 7, 7].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 7, 7].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        worksheet.Range[_items.Rows.Count + 7, 9, _items.Rows.Count + 7, 10].Merge();
                        worksheet.Range[_items.Rows.Count + 7, 9].Value2 = "THỐNG KÊ";
                        worksheet.Range[_items.Rows.Count + 7, 9].CellStyle.Font.Bold = true;
                        worksheet.Range[_items.Rows.Count + 7, 9].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            return new JsonResult(new
                            {
                                isSuccess = true,
                                Mesages = "Đã xuất file Excel thành công!",
                                ExcelContent = content,
                                ExcelFileName = $@"NangSuatChung-{ngayThangNam}.{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                            });
                        }
                    }

                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Mesages = "Không tìm thấy dữ liệu!"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Không tìm thấy dữ liệu!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    Mesages = ex.ToString()
                });
            }

        }

        //public void ExportBangKe_TienThoiVu(object p)
        //{
        //    try
        //    {
        //        SpreadSheetShow = true;
        //        var spreadSheet = p as SfSpreadsheet;
        //        Application.Current.Dispatcher?.Invoke(
        //            () => { AppViewModel.Ins.IsBusy = true; });

        //        void action()
        //        {
        //            try
        //            {
        //                T_Phieus.Clear();
        //                TemplateBangKe_TienThoiVu_CMX =
        //                    $@"{AppViewModel.Ins.AppPath}{@"XlsIOTemplate\T\TemplateBangKe_TienThoiVu1.xlsx"}";
        //                var vmApp = AppViewModel.Ins;
        //                var listNhanVienSelected = NhanVienViewModel.Ins.SelectedItems;
        //                var items = T_PhieuCanViewModel.Ins
        //                    .GetTongHopNhanViensDateTimeToDateTime<object>(
        //                        AppViewModel.Ins.FromDate,
        //                        AppViewModel.Ins.DateReport,
        //                        XiNghiepViewModel.Ins.XiNghiepSelectedItem?.Ma,
        //                        T_KhuVucViewModel.Ins.SelectedItem?.Ma);

        //                var worksheetIndex = 0;
        //                if (items != null && items.Any())
        //                {
        //                    var _items = items.ToDataTable();
        //                    using var excelEngine = new ExcelEngine();
        //                    var application = excelEngine.Excel;
        //                    using var fileStream = new FileStream(TemplateBangKe_TienThoiVu_CMX, FileMode.Open);
        //                    var workbook = application.Workbooks.Open(fileStream);
        //                    var worksheet = workbook.Worksheets[worksheetIndex];
        //                    worksheet.EnableSheetCalculations();
        //                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
        //                    //xu ly du lieu
        //                    if (_items.Rows.Count > 0)
        //                    {
        //                        // lay danh sach nhan vien
        //                        var listNhanVien = listNhanVienSelected.Cast<dynamic>()
        //                            .ToList()
        //                            .Select(x => new { MaNhanVien = (string)x.MaNhanVien })
        //                            .ToList();
        //                        var nhanVienLamViec = items.Cast<dynamic>()
        //                            .Where(item =>
        //                                item.TrongLuong > 0 && item.TrongLuong != null &&
        //                                listNhanVien.Any(nv => nv.MaNhanVien == item.MaNhanVien))
        //                            .Select(item => new { item.MaNhanVien }).Distinct()
        //                            .ToList();
        //                        // lấy So lớn nhất trong T_Phieu
        //                        var getMax = T_PhieuViewModel.Ins.GetMaxSoInDay<object>(DateTime.Now);
        //                        //Toán tử ?. được sử dụng để kiểm tra getMax có phải là null trước khi truy cập phần tử tiếp theo trong chuỗi lệnh (Cast<dynamic>().Select(x => (int)x.MaxSo).FirstOrDefault()). Nếu getMax là null, giá trị của maxSo sẽ được đặt thành 0 bằng toán tử ??.
        //                        var maxSo = getMax?.Cast<dynamic>().Select(x => (int?)x.MaxSo).FirstOrDefault() ?? 0;
        //                        for (var i = 0; i < nhanVienLamViec.Count; i++)
        //                        {
        //                            var nhanviens = nhanVienLamViec[i];
        //                            worksheet = workbook.Worksheets[worksheetIndex];
        //                            //workbook.Worksheets.AddCopyAfter(workbook.Worksheets[worksheetIndex]);
        //                            //worksheetIndex += 1;
        //                            //dat ten sheet
        //                            if (nhanviens.MaNhanVien == null)
        //                                worksheet.Name = $@"{"NULL"} ";
        //                            else
        //                                worksheet.Name = $@"{nhanviens.MaNhanVien} ";


        //                            var Ngay = DateTime.Now;
        //                            var Nam = Ngay.Year % 100; // Lấy hai chữ số cuối của nămr
        //                            var Thang = Ngay.Month; // Lấy giá trị tháng
        //                            var NgayTrongThang = Ngay.Day; //lấy giá trị ngày
        //                                                           // chuyển ngày thành số vd:2023-06-09 = 2369
        //                            var SoNgay = int.Parse(string.Concat(Nam, Thang.ToString("D2"),
        //                                NgayTrongThang.ToString("D2")));

        //                            //var soPhieu = $@"{SoNgay.ToString()}{(maxSo + 1 + i).ToString("0000")}"; ban đầu + i mà ko hiểu vì sao lại cong thêm i :))
        //                            var soPhieu = $@"{SoNgay.ToString()}{(maxSo + i + 1).ToString("0000")}"; // bỏ + i
        //                            var ngay =
        //                                $@"{vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
        //                            var thoiGian =
        //                                $@"{vmApp.FromDate.ToString("HH:mm:ss")} - {vmApp.DateReport.ToString("HH:mm:ss")} ";
        //                            var ngayGioIn = $@"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
        //                            var xiNghiep = $@"{XiNghiepViewModel.Ins.XiNghiepSelectedItem?.Ten}";
        //                            var ngaySL = $@"{vmApp.FromDate.ToString("dd/MM/yyyy")}";
        //                            worksheet.Range[1, 7].Value2 = soPhieu;
        //                            worksheet.Range[2, 7].Value2 = ngay;
        //                            worksheet.Range[3, 7].Value2 = thoiGian;
        //                            worksheet.Range[10, 7].Value2 = ngayGioIn;
        //                            worksheet.Range[8, 7].Value2 = xiNghiep;
        //                            worksheet.Range[10, 2].Value2 = ngaySL;
        //                            worksheet.Range[9, 2].HorizontalAlignment = ExcelHAlign.HAlignLeft;
        //                            worksheet.Range[10, 2].HorizontalAlignment = ExcelHAlign.HAlignLeft;
        //                            worksheet.Range[10, 7].HorizontalAlignment = ExcelHAlign.HAlignLeft;
        //                            //footer
        //                            worksheet.Range[31, 9, 31, 10].Merge();

        //                            worksheet.Range[31, 9, 31, 10].HorizontalAlignment = ExcelHAlign.HAlignCenter;
        //                            //worksheet.Range[row + 2, 2, row + 2, 3].Merge();

        //                            //worksheet.Range[23, 2].Value2 = $@"Cộng Lượng: ";
        //                            //worksheet.Range[23, 4, 23, 6].Merge();

        //                            //worksheet.Range[23, 4, 23, 6].HorizontalAlignment = ExcelHAlign.HAlignLeft;

        //                            //worksheet.Range[25, 2, 25, 5].Merge();
        //                            //worksheet.Range[25, 2].Value2 = $@"Thống Kê";
        //                            //worksheet.Range[25, 2, 25, 5].HorizontalAlignment = ExcelHAlign.HAlignCenter;
        //                            //worksheet.Range[25, 2, 25, 5].CellStyle.Font.Bold = true;
        //                            //worksheet.Range[25, 6, 25, 10].Merge();
        //                            //worksheet.Range[25, 6].Value2 = $@"Ban Giám đốc Zone/ Quản đốc";
        //                            //worksheet.Range[25, 6, 25, 10].HorizontalAlignment = ExcelHAlign.HAlignCenter;
        //                            //worksheet.Range[25, 6, 25, 10].CellStyle.Font.Bold = true;


        //                            //worksheet.Range[12, 2, 21, 10].BorderAround();
        //                            //worksheet.Range[12, 2, 21, 10].BorderInside();
        //                            //thông tin nhân viên
        //                            var dataNhanVien = items.Cast<dynamic>()
        //                                .ToList()
        //                                .Where(x => (string)x.MaNhanVien == nhanviens.MaNhanVien)
        //                                .GroupBy(x => new { x.MaNhanVien, x.MaHoSo, x.TenNhanVien, x.Nhom })
        //                                .Select(
        //                                    x => new
        //                                    {
        //                                        MaNhanVien = (string)x.Key.MaNhanVien,
        //                                        MaHoSo = (string)x.Key.MaHoSo,
        //                                        TenNhanVien = (string)x.Key.TenNhanVien,
        //                                        Nhom = (string)x.Key.Nhom
        //                                    })
        //                                .OrderBy(x => x.MaNhanVien)
        //                                .ToList();
        //                            var itemDycs = items.Cast<dynamic>().ToList();
        //                            for (var j = 0; j < dataNhanVien.Count; j++)
        //                            {
        //                                var nhanVien = dataNhanVien[j];
        //                                worksheet.Range[7, 2].Value2 = $@"{nhanVien.MaNhanVien}";
        //                                worksheet.Range[7, 2].HorizontalAlignment = ExcelHAlign.HAlignLeft;
        //                                worksheet.Range[8, 2].Value2 = $@"{nhanVien.TenNhanVien}";
        //                                worksheet.Range[8, 2].HorizontalAlignment = ExcelHAlign.HAlignLeft;
        //                                worksheet.Range[9, 2].Value2 = $@"{nhanVien.Nhom}";
        //                                worksheet.Range[9, 2].HorizontalAlignment = ExcelHAlign.HAlignLeft;

        //                                var listCongViecOfNhanVien = itemDycs
        //                                    .Where(x => (string)x.MaNhanVien == nhanVien.MaNhanVien)
        //                                    .Select(x => new
        //                                    {
        //                                        MaCongDoan = (string)x.MaCongDoan,
        //                                        CongViecName = (string)x.CongViecName
        //                                    }).Distinct().ToList();

        //                                //index
        //                                var itemsPerPage = 20; // giới hạn mỗi trang chỉ hiển thị 20 công việc
        //                                                       //Phép chia (double)totalItems / 20 sẽ cho kết quả dưới dạng số thập phân, vì vậy chúng ta cần ép kiểu thành double để đảm bảo kết quả chính xác.
        //                                                       //Hàm Math.Ceiling được sử dụng để làm tròn lên kết quả của phép chia, vì ta muốn số trang được làm tròn lên đến trang kế tiếp nếu tổng số dữ liệu không chia hết cho 20.
        //                                var totalItems = listCongViecOfNhanVien.Count;
        //                                var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        //                                //worksheetIndex = worksheetIndex + 
        //                                for (var page = 0; page < totalPages; page++)
        //                                {
        //                                    var maxRow = 10; // giới hạn row công việc đc hiển thị

        //                                    var colTrongLuong = 3; // +4 nếu row = 22(y =9)
        //                                    var row =
        //                                        12; // tăng dần theo y++, nếu row = 22(y=9) thì về lại row đầu tiên
        //                                    var row2 = 12;
        //                                    var rowIndex = 12;

        //                                    decimal tongTLT = 0;
        //                                    decimal trongLuong = 0;


        //                                    var currentPage = 0;
        //                                    var colCongViec = 1; // +4 nếu row = 22( y=9)
        //                                    var rowRevert =
        //                                        -1; // row này sử áp và +1 sau mỗi lần duyệt qua vòng for khi y >9 
        //                                    row2 = 12;
        //                                    row = 12;

        //                                    workbook.Worksheets.AddCopyAfter(workbook.Worksheets[worksheetIndex]);
        //                                    workbook.Worksheets[worksheetIndex].Name =
        //                                        $@"{nhanviens.MaNhanVien}-{page + 1} ";
        //                                    //worksheetIndex += page;
        //                                    currentPage = page + 1;
        //                                    worksheet = workbook.Worksheets[worksheetIndex];
        //                                    // worksheetIndex = page;
        //                                    //startIndex và endIndex xác định phạm vi của công việc cần được xử lý trên mỗi page.
        //                                    var startIndex = page * itemsPerPage;
        //                                    var endIndex = Math.Min(startIndex + itemsPerPage, totalItems);
        //                                    tongTLT = 0;
        //                                    var rowMergeIndex = 0;
        //                                    for (var y = startIndex; y < endIndex; y++)
        //                                    {
        //                                        var congViec = listCongViecOfNhanVien[y];
        //                                        trongLuong = itemDycs.Where(
        //                                                x => x.MaNhanVien == nhanVien.MaNhanVien &&
        //                                                     x.CongViecName == congViec.CongViecName)
        //                                            .Select(x => (decimal)x.TrongLuong)
        //                                            .DefaultIfEmpty(0).Sum();
        //                                        tongTLT += trongLuong;

        //                                        if (y % itemsPerPage <= 9)
        //                                        {
        //                                            row = rowIndex + y % itemsPerPage;

        //                                            worksheet.Range[row, colCongViec, row, colCongViec + 1].Merge();
        //                                            worksheet.Range[row, colCongViec].Value2 =
        //                                                $@"{congViec.CongViecName}";
        //                                            worksheet.Range[row, colTrongLuong, row, colTrongLuong + 1].Merge();
        //                                            worksheet.Range[row, colTrongLuong].Value2 = $@"{trongLuong}";
        //                                            worksheet.Range[12, 2, row, colTrongLuong + 1].BorderAround();
        //                                            worksheet.Range[12, 2, row, colTrongLuong + 1].BorderInside();
        //                                        }

        //                                        if (y % itemsPerPage > 9)
        //                                        {
        //                                            rowRevert = rowRevert + 1;
        //                                            row2 = 12 + rowRevert;

        //                                            worksheet.Range[row2, colCongViec + 4, row2, colCongViec + 4 + 1]
        //                                                .Merge();
        //                                            worksheet.Range[row2, colCongViec + 4].Value2 =
        //                                                $@"{congViec.CongViecName}";
        //                                            worksheet.Range[row2, colTrongLuong + 4, row2,
        //                                                colTrongLuong + 4 + 2].Merge();
        //                                            //worksheet.Range[row2, colTrongLuong + 4].Value2 = $@"{trongLuong}";
        //                                            //string trongLuongString = trongLuong.ToString("0.00");
        //                                            var trongLuongNumber = Convert.ToDouble(trongLuong);
        //                                            var trongLuongRound = Math.Round(trongLuongNumber, 2);
        //                                            worksheet.Range[row2, colTrongLuong + 4].NumberFormat = "0.00";
        //                                            worksheet.Range[row2, colTrongLuong + 4].Value2 =
        //                                                $@"{trongLuongRound}";
        //                                            worksheet.Range[12, 2, row2, colTrongLuong + 4 + 2].BorderAround();
        //                                            worksheet.Range[12, 2, row2, colTrongLuong + 4 + 2].BorderInside();
        //                                        }

        //                                        //thêm ngày 8/9/2023 xử lý lỗi lập lại số phiếu, nghi ngờ do trích suất data từ template có vấn đề
        //                                        // nên ko trích dữ liệu để Insert từ tempalte dưới commandPrint nữa mà lấy trực tiếp từ đây
        //                                        var phieu = new T_Phieu();
        //                                        // Gán các giá trị cho đối tượng phieu từ danh sách congViec và trongLuong
        //                                        phieu.So = soPhieu;
        //                                        phieu.MaNhanVien = nhanVien.MaNhanVien;
        //                                        phieu.XiNghiep = xiNghiep;
        //                                        phieu.NgayGio = DateTime.Now;
        //                                        phieu.TuNgayGio = vmApp.FromDate;
        //                                        phieu.DenNgayGio = vmApp.DateReport;
        //                                        phieu.MaCongDoan = congViec.MaCongDoan;
        //                                        phieu.TrongLuong = trongLuong;
        //                                        phieu.ThanhTien = 0;
        //                                        phieu.UserName = VmApp.UserName;
        //                                        phieu.PCName = VmApp.PCName;
        //                                        // Thêm đối tượng phieu vào danh sách phieuList
        //                                        T_Phieus.Add(phieu);

        //                                        rowMergeIndex = y % itemsPerPage;
        //                                    }

        //                                    //var rowM1 = 12;
        //                                    //var rowM2 = 12;
        //                                    //var colMcongviec = 1;
        //                                    //var colMluong = 3;
        //                                    //var rowMRevert = -1;
        //                                    //for (var m = rowMergeIndex + 1; m < 20; m++)
        //                                    //{
        //                                    //    if (m <= 9)
        //                                    //    {
        //                                    //        rowM1 = 12 + m % 20;
        //                                    //        worksheet.Range[rowM1, colMcongviec, rowM1, colMcongviec + 1].Merge();
        //                                    //        worksheet.Range[rowM1, colMluong, rowM1, colMluong + 1].Merge();
        //                                    //    }

        //                                    //    if (m > 9)
        //                                    //    {
        //                                    //        rowMRevert = rowMRevert + 1;
        //                                    //        rowM2 = 12 + rowMRevert;
        //                                    //        worksheet.Range[rowM2, colMcongviec + 4, rowM2, colMcongviec + 4 + 1]
        //                                    //            .Merge();
        //                                    //        worksheet.Range[rowM2, colMluong + 4, rowM2, colMluong + 4 + 2].Merge();
        //                                    //    }
        //                                    //}

        //                                    worksheet.Range[29, 9].Value2 = $@"'{currentPage}/{totalPages}";
        //                                    worksheet.Range[29, 1].Value2 = $@"Người in: {VmApp.UserName}";
        //                                    worksheet.Range[23, 2].Value2 = $@"{tongTLT.ToString("0.00")} Kg";
        //                                    //worksheet.UsedRange.AutofitColumns();
        //                                    worksheetIndex += 1;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    workbook.Version = ExcelVersion.Excel2013;
        //                    if (workbook.Worksheets.Count <= 1)
        //                    {
        //                        MessageBox.Show("Không có dữ liệu!");
        //                        return;
        //                    }

        //                    workbook.Worksheets.Remove(workbook.Worksheets.Count - 1); // xoa sheet cuoi cung
        //                                                                               // workbook.Worksheets.Remove(workbook.Worksheets.Count -1);

        //                    foreach (var item in workbook.Worksheets) item.Protect("domaybietduocpass");

        //                    workbook.SaveAs(
        //                        $@"{AppViewModel.Ins.AppPath}{@"\Template\BaoCaoModified.xlsx"}");
        //                    using var fileStream2 = new FileStream(
        //                        $@"{AppViewModel.Ins.AppPath}{@"\Template\BaoCaoModified.xlsx"}",
        //                        FileMode.Open);
        //                    //var workbook2 = application.Workbooks.OpenReadOnly( $@"{AppViewModel.Ins.AppPath}{@"\Template\BaoCaoModified.xlsx"}");
        //                    Application.Current.Dispatcher?.Invoke(
        //                        () =>
        //                        {
        //                            //spreadSheet.can = true;
        //                            //spreadSheet.Protect(true, true,"pmspmspms");
        //                            //spreadSheet.ProtectSheet(spreadSheet.ActiveSheet, "");
        //                            spreadSheet.Open(fileStream2);
        //                            // spreadSheet.ActiveGrid.CurrentCellBeginEdit -= ActiveGrid_CurrentCellBeginEdit;
        //                            //spreadSheet.ActiveGrid.CurrentCellBeginEdit += ActiveGrid_CurrentCellBeginEdit;
        //                            AppViewModel.Ins.IsBusy = false;
        //                        });
        //                }
        //                else
        //                {
        //                    Application.Current.Dispatcher?.Invoke(
        //                        () => { MessageBox.Show("Không có dữ liệu"); });
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Application.Current.Dispatcher?.Invoke(
        //                    () => { MessageBox.Show(ex.ToString()); });
        //            }
        //            finally
        //            {
        //                Application.Current.Dispatcher?.Invoke(
        //                    () => { AppViewModel.Ins.IsBusy = false; });
        //            }
        //        }

        //        var task = new Task(action);
        //        task.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}

        //public void ExportBangKe_TienThoiVu_ThanhTien(object p)
        //{
        //    try
        //    {
        //        SpreadSheetShow = true;
        //        var spreadSheet = p as SfSpreadsheet;
        //        Application.Current.Dispatcher?.Invoke(
        //            () => { AppViewModel.Ins.IsBusy = true; });

        //        void action()
        //        {
        //            try
        //            {
        //                T_Phieus.Clear();
        //                TemplateBangKe_TienThoiVu_CMX =
        //                    $@"{AppViewModel.Ins.AppPath}{@"XlsIOTemplate\T\TemplateBangKe_TienThoiVu2.xlsx"}";
        //                var vmApp = AppViewModel.Ins;
        //                var listNhanVienSelected = NhanVienViewModel.Ins.SelectedItems;
        //                var items = T_PhieuCanViewModel.Ins
        //                    .GetTongHopNhanViensDateTimeToDateTime<object>(
        //                        AppViewModel.Ins.FromDate,
        //                        AppViewModel.Ins.DateReport,
        //                        XiNghiepViewModel.Ins.XiNghiepSelectedItem?.Ma,
        //                        T_KhuVucViewModel.Ins.SelectedItem?.Ma);

        //                var worksheetIndex = 0;
        //                if (items != null && items.Any())
        //                {
        //                    var _items = items.ToDataTable();
        //                    using var excelEngine = new ExcelEngine();
        //                    var application = excelEngine.Excel;
        //                    using var fileStream = new FileStream(TemplateBangKe_TienThoiVu_CMX, FileMode.Open);
        //                    var workbook = application.Workbooks.Open(fileStream);
        //                    var worksheet = workbook.Worksheets[worksheetIndex];
        //                    worksheet.EnableSheetCalculations();
        //                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
        //                    //xu ly du lieu
        //                    if (_items.Rows.Count > 0)
        //                    {
        //                        // lay danh sach nhan vien
        //                        var listNhanVien = listNhanVienSelected.Cast<dynamic>()
        //                            .ToList()
        //                            .Select(x => new { MaNhanVien = (string)x.MaNhanVien })
        //                            .ToList();
        //                        var nhanVienLamViec = items.Cast<dynamic>()
        //                            .Where(item =>
        //                                item.TrongLuong > 0 && item.TrongLuong != null &&
        //                                listNhanVien.Any(nv => nv.MaNhanVien == item.MaNhanVien))
        //                            .Select(item => new { item.MaNhanVien }).Distinct()
        //                            .ToList();
        //                        // lấy So lớn nhất trong T_Phieu
        //                        var getMax = T_PhieuViewModel.Ins.GetMaxSoInDay<object>(DateTime.Now);
        //                        //?. được sử dụng để kiểm tra getMax có phải là null trước khi truy cập phần tử tiếp theo trong chuỗi lệnh Nếu getMax là null, giá trị của maxSo sẽ được đặt thành 0 bằng ??.
        //                        var maxSo = getMax?.Cast<dynamic>().Select(x => (int?)x.MaxSo).FirstOrDefault() ?? 0;
        //                        for (var i = 0; i < nhanVienLamViec.Count; i++)
        //                        {
        //                            var nhanviens = nhanVienLamViec[i];
        //                            worksheet = workbook.Worksheets[worksheetIndex];
        //                            //workbook.Worksheets.AddCopyAfter(workbook.Worksheets[worksheetIndex]);
        //                            //worksheetIndex += 1;
        //                            //dat ten sheet
        //                            if (nhanviens.MaNhanVien == null)
        //                                worksheet.Name = $@"{"NULL"} ";
        //                            else
        //                                worksheet.Name = $@"{nhanviens.MaNhanVien} ";


        //                            var Ngay = DateTime.Now;
        //                            var Nam = Ngay.Year % 100; // Lấy hai chữ số cuối của nămr
        //                            var Thang = Ngay.Month; // Lấy giá trị tháng
        //                            var NgayTrongThang = Ngay.Day; //lấy giá trị ngày
        //                                                           // chuyển ngày thành số vd:2023-06-09 = 2369
        //                            var SoNgay = int.Parse(string.Concat(Nam, Thang.ToString("D2"),
        //                                NgayTrongThang.ToString("D2")));

        //                            var soPhieu = $@"{SoNgay.ToString()}{(maxSo + i + 1).ToString("0000")}";
        //                            // maxSo++;// tăng maxSo
        //                            var ngay =
        //                                $@"{vmApp.FromDate.ToString("dd/MM/yyyy HH:mm:ss")} - {vmApp.DateReport.ToString("dd/MM/yyyy HH:mm:ss")} ";
        //                            var ngayGioIn =
        //                                $@"{DateTime.Now.ToString("dd/MM/yyyy")} {DateTime.Now.ToString("HH:mm:ss")}";
        //                            var xiNghiep = $@"{XiNghiepViewModel.Ins.XiNghiepSelectedItem?.Ten}";
        //                            var ngaySL = $@"{vmApp.FromDate.ToString("dd/MM/yyyy")} ";
        //                            worksheet.Range[2, 6].Value2 = soPhieu;
        //                            worksheet.Range[7, 7].Value2 = xiNghiep;
        //                            worksheet.Range[8, 3].Value2 = ngay;
        //                            worksheet.Range[9, 3].Value2 = ngaySL;
        //                            worksheet.Range[9, 6].Value2 = ngayGioIn;


        //                            //thông tin nhân viên
        //                            var dataNhanVien = items.Cast<dynamic>()
        //                                .ToList()
        //                                .Where(x => (string)x.MaNhanVien == nhanviens.MaNhanVien)
        //                                .GroupBy(x => new { x.MaNhanVien, x.MaHoSo, x.TenNhanVien, x.Nhom })
        //                                .Select(
        //                                    x => new
        //                                    {
        //                                        MaNhanVien = (string)x.Key.MaNhanVien,
        //                                        MaHoSo = (string)x.Key.MaHoSo,
        //                                        TenNhanVien = (string)x.Key.TenNhanVien,
        //                                        Nhom = (string)x.Key.Nhom
        //                                    })
        //                                .OrderBy(x => x.MaNhanVien)
        //                                .ToList();
        //                            var itemDycs = items.Cast<dynamic>().ToList();
        //                            for (var j = 0; j < dataNhanVien.Count; j++)
        //                            {
        //                                var nhanVien = dataNhanVien[j];

        //                                worksheet.Range[6, 3].Value2 = $@"{nhanVien.TenNhanVien}";
        //                                worksheet.Range[6, 7].Value2 = $@"{nhanVien.MaNhanVien}";
        //                                worksheet.Range[7, 3].Value2 = $@"{nhanVien.Nhom}";

        //                                var listCongViecOfNhanVien = itemDycs
        //                                    .Where(x => (string)x.MaNhanVien == nhanVien.MaNhanVien)
        //                                    .Select(x => new
        //                                    {
        //                                        MaCongDoan = (string)x.MaCongDoan,
        //                                        CongViecName = (string)x.CongViecName
        //                                    }).Distinct().ToList();

        //                                //index
        //                                var colCongViec = 2; // +4 nếu row = 22( y=9)
        //                                var colTrongLuong = 4; // +4 nếu row = 22(y =9)
        //                                var colThanhTien = 6;
        //                                var row = 12; // tăng dần theo y++, nếu row = 22(y=9) thì về lại row đầu tiên

        //                                var rowIndex = 12;

        //                                decimal tongTLT = 0;
        //                                decimal tongThanhTien = 0;
        //                                decimal trongLuong = 0;

        //                                var itemsPerPage = 20; // giới hạn mỗi trang chỉ hiển thị 20 công việc
        //                                var currentPage = 0;
        //                                //Phép chia (double)totalItems / 20 sẽ cho kết quả dưới dạng số thập phân, vì vậy chúng ta cần ép kiểu thành double để đảm bảo kết quả chính xác.
        //                                //Hàm Math.Ceiling được sử dụng để làm tròn lên kết quả của phép chia, vì ta muốn số trang được làm tròn lên đến trang kế tiếp nếu tổng số dữ liệu không chia hết cho 20.
        //                                var totalItems = listCongViecOfNhanVien.Count;
        //                                var totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
        //                                //worksheetIndex = worksheetIndex + 
        //                                for (var page = 0; page < totalPages; page++)
        //                                {
        //                                    row = 12;
        //                                    workbook.Worksheets.AddCopyAfter(workbook.Worksheets[worksheetIndex]);
        //                                    workbook.Worksheets[worksheetIndex].Name =
        //                                        $@"{nhanviens.MaNhanVien}-{page + 1} ";
        //                                    //worksheetIndex += page;
        //                                    currentPage = page + 1;
        //                                    worksheet = workbook.Worksheets[worksheetIndex];
        //                                    // worksheetIndex = page;
        //                                    //startIndex và endIndex xác định phạm vi của công việc cần được xử lý trên mỗi page.
        //                                    var startIndex = page * itemsPerPage;
        //                                    var endIndex = Math.Min(startIndex + itemsPerPage, totalItems);
        //                                    tongTLT = 0;
        //                                    for (var y = startIndex; y < endIndex; y++)
        //                                    {
        //                                        var congViec = listCongViecOfNhanVien[y];
        //                                        var donGia = DG_DonGiaTViewModel.Ins.Items?
        //                                            .Where(x => x.NgayGio <= vmApp.FromDate &&
        //                                                        x.MaCongDoan == congViec.MaCongDoan)
        //                                            .OrderByDescending(x => x.NgayGio)
        //                                            .Select(x => (int?)x.Gia).FirstOrDefault() ?? 0;
        //                                        trongLuong = itemDycs.Where(
        //                                                x => x.MaNhanVien == nhanVien.MaNhanVien &&
        //                                                     x.CongViecName == congViec.CongViecName)
        //                                            .Select(x => (decimal)x.TrongLuong)
        //                                            .DefaultIfEmpty(0).Sum();
        //                                        var thanhTien = trongLuong * donGia;
        //                                        tongTLT += trongLuong;
        //                                        tongThanhTien += thanhTien;
        //                                        row = rowIndex + y % itemsPerPage;
        //                                        worksheet.Range[row, colCongViec].Value2 = $@"{congViec.CongViecName}";
        //                                        //worksheet.Range[row, colTrongLuong].Value2 = $@"{trongLuong}";
        //                                        var trongLuongNumber = Convert.ToDouble(trongLuong);
        //                                        var trongLuongRound = Math.Round(trongLuongNumber, 2);
        //                                        worksheet.Range[row, colTrongLuong].NumberFormat = "0.00";
        //                                        worksheet.Range[row, colTrongLuong].Value2 = $@"{trongLuongRound}";
        //                                        var formatThanhTien = thanhTien
        //                                            .ToString("N0", new CultureInfo("vi-VN")).Replace("₫", "");
        //                                        worksheet.Range[row, colThanhTien].Value2 = $@"{formatThanhTien}";


        //                                        //thêm ngày 8/9/2023 xử lý lỗi lập lại số phiếu, nghi ngờ do trích suất data từ template có vấn đề
        //                                        // nên ko trích dữ liệu để Insert từ tempalte dưới commandPrint nữa mà lấy trực tiếp từ đây
        //                                        var phieu = new T_Phieu();
        //                                        // Gán các giá trị cho đối tượng phieu từ danh sách congViec và trongLuong
        //                                        phieu.So = soPhieu;
        //                                        phieu.MaNhanVien = nhanVien.MaNhanVien;
        //                                        phieu.XiNghiep = xiNghiep;
        //                                        phieu.NgayGio = DateTime.Now;
        //                                        phieu.TuNgayGio = vmApp.FromDate;
        //                                        phieu.DenNgayGio = vmApp.DateReport;
        //                                        phieu.MaCongDoan = congViec.MaCongDoan;
        //                                        phieu.TrongLuong = trongLuong;
        //                                        phieu.ThanhTien = thanhTien;
        //                                        phieu.UserName = VmApp.UserName;
        //                                        phieu.PCName = VmApp.PCName;
        //                                        // Thêm đối tượng phieu vào danh sách phieuList
        //                                        T_Phieus.Add(phieu);
        //                                    }

        //                                    worksheet.Range[41, 6].Value2 = $@"'{currentPage}/{totalPages}";
        //                                    worksheet.Range[41, 2].Value2 = $@"Người in: {VmApp.UserName}";
        //                                    //worksheet.Range[23, 4].Value2 = $@"{tongTLT} Kg";
        //                                    worksheet.Range[32, 4].Value2 = $@"{tongTLT.ToString("0.00")} Kg";
        //                                    var formattedTongThanhTien = tongThanhTien
        //                                        .ToString("N0", new CultureInfo("vi-VN")).Replace("₫", "");
        //                                    worksheet.Range[32, 6].Value2 = $@"{formattedTongThanhTien}";
        //                                    //worksheet.UsedRange.AutofitColumns();
        //                                    worksheetIndex += 1;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    workbook.Version = ExcelVersion.Excel2013;
        //                    if (workbook.Worksheets.Count <= 1)
        //                    {
        //                        MessageBox.Show("Không có dữ liệu!");
        //                        return;
        //                    }

        //                    workbook.Worksheets.Remove(workbook.Worksheets.Count - 1); // xoa sheet cuoi cung
        //                                                                               // workbook.Worksheets.Remove(workbook.Worksheets.Count -1);

        //                    foreach (var item in workbook.Worksheets) item.Protect("domaybietduocpass");
        //                    workbook.SaveAs(
        //                        $@"{AppViewModel.Ins.AppPath}{@"\Template\BaoCaoModified.xlsx"}");
        //                    using var fileStream2 = new FileStream(
        //                        $@"{AppViewModel.Ins.AppPath}{@"\Template\BaoCaoModified.xlsx"}",
        //                        FileMode.Open);
        //                    Application.Current.Dispatcher?.Invoke(
        //                        () =>
        //                        {
        //                            spreadSheet.Open(fileStream2);
        //                            AppViewModel.Ins.IsBusy = false;
        //                        });
        //                }
        //                else
        //                {
        //                    Application.Current.Dispatcher?.Invoke(
        //                        () => { MessageBox.Show("Không có dữ liệu"); });
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Application.Current.Dispatcher?.Invoke(
        //                    () => { MessageBox.Show(ex.ToString()); });
        //            }
        //            finally
        //            {
        //                Application.Current.Dispatcher?.Invoke(
        //                    () => { AppViewModel.Ins.IsBusy = false; });
        //            }
        //        }

        //        var task = new Task(action);
        //        task.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}
    }
}
