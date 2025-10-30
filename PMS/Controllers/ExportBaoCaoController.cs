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
using Syncfusion.EJ2.Spreadsheet;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using ViewModels.Repos.HQ;
using Syncfusion.EJ2.Layouts;

namespace PMS.Controllers
{
    [Authorize]
    public class ExportBaoCaoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private AppViewModels.AppViewModel _vmApp => AppViewModel.Instance;
        public ExportBaoCaoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<object>> GetsTongHopThangDinhMuc(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetsTongHopThangDinhMuc/{fromDate?.ToString("yyyy-MM-dd")}/{dateTime?.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> ExportFilletThangDinhMuc(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                //var TemplateThangDinhMucHN = $@"{AppViewModel.Instance.AppPath}{@"\XlsIOTemplate\TemplateThangDinhMucFillet.xlsx"}";
                var TemplateThangDinhMucHN = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "TemplateThangDinhMucFillet.xlsx");
                var vmApp = AppViewModel.Instance;
                //từ ngày
                var day = vmApp.DateReport.Day;
                var month = vmApp.DateReport.Month;
                var year = vmApp.DateReport.Year;
                //đến ngày
                var fromDay = vmApp.FromDate.Day;
                var fromMonth = vmApp.FromDate.Month;
                var fromYear = vmApp.FromDate.Year;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;

                //Color color = Color.FromArgb(50, 205, 50);
                //string thangNam = month + "/" + year;
                //var thangNam = $@"   THÁNG {month.ToString("00")} / {year}";
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //string[] thangNam = new string[] { month + "/" + year };
                var items1 = await GetsTongHopThangDinhMuc(fromDate, dateTime, xuongId);
                var rs = items1 != null ? items1.Cast<dynamic>().ToList() : new List<dynamic>();
                var items = rs
                    .ToList().Where(x =>
                        x.MaNhanVien != null && !string.IsNullOrWhiteSpace(x.MaNhanVien.ToString()))
                    .Cast<object>()
                    .ToList();
                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(TemplateThangDinhMucHN, FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    worksheet.EnableSheetCalculations();
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    var color = worksheet.Range[9, 1, 9, 1].CellStyle.Color;
                    if (_items.Rows.Count > 0)
                    {
                        //danh sách nhân viên
                        var nhanviens = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaNhanVien, x.NhanVienName, x.MaHoSo })
                            .Select(
                                x => new
                                {
                                    MaNhanVien = (string)x.Key.MaNhanVien,
                                    NhanVienName = (string)x.Key.NhanVienName,
                                    MaHoSo = (string)x.Key.MaHoSo
                                })
                            .OrderBy(x => x.MaHoSo)
                            .ToList();

                        var ngays = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new
                            {
                                x.Ngay
                            })
                            .Select(x => new
                            {
                                Ngay = (DateTime)x.Key.Ngay
                            }).OrderBy(x => x.Ngay)
                            .Distinct().ToList();

                        var sizes = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new
                            {
                                x.SizeName,
                                x.Ngay
                            })
                            .Select(x => new
                            {
                                SizeName = (string)x.Key.SizeName,
                                Ngay = (DateTime)x.Key.Ngay
                            }).OrderBy(x => x.SizeName)
                            .Distinct().ToList();
                        var sizesOfTong = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new
                            {
                                x.SizeName
                            })
                            .Select(x => new
                            {
                                SizeName = (string)x.Key.SizeName
                            }).OrderBy(x => x.SizeName)
                            .Distinct().ToList();
                        var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new
                            {
                                x.ThanhPhamName
                            })
                            .Select(x => new
                            {
                                ThanhPhamName = (string)x.Key.ThanhPhamName
                            }).OrderBy(x => x.ThanhPhamName)
                            .Distinct().ToList();


                        //header
                        var itemDycs = items.Cast<dynamic>().ToList();
                        //index header1
                        var numCol = 0;
                        var col = 4;
                        var lastCol = 0;
                        //index header2
                        var numCol2 = 0;
                        var col2 = 4;
                        var lastCol2 = 0;
                        //index header3
                        var numCol3 = 0;
                        var col3 = 4;
                        var lastCol3 = 0;
                        for (var ngi = 0; ngi < ngays.Count; ngi++)
                        {
                            var ngay = ngays[ngi];
                            for (var si = 0; si < sizes.Count; si++)
                            {
                                var size = sizes[si];
                                if (size.Ngay != ngay.Ngay) continue;
                                //header ngay(size)
                                var ngayAdd = DateTime.Parse(ngay.Ngay.ToString()).ToString("dd/MM");

                                //var ngayAdd = ngay.ToString("dd/MM");
                                var sizeAdd = size.SizeName;

                                for (var tpi = 0; tpi < thanhPhams.Count; tpi++)
                                {
                                    //header tên thành phẩm
                                    var thanhPham = thanhPhams[tpi];
                                    //if (thanhPham.Ngay != ngay.Ngay)
                                    //{
                                    //    continue;
                                    //}

                                    var sanPhams = items.Cast<dynamic>()
                                        .ToList().Where(x =>
                                            x.ThanhPhamName == thanhPham.ThanhPhamName &&
                                            x.SizeName == size.SizeName &&
                                            ((DateTime)x.Ngay).Date == ngay.Ngay.Date)
                                        .GroupBy(x => new
                                        {
                                            x.SanPhamName
                                        })
                                        .Select(x => new
                                        {
                                            SanPhamName = (string)x.Key.SanPhamName
                                        }).OrderBy(x => x.SanPhamName).Distinct().ToList();
                                    if (sanPhams.Count <= 0) continue;
                                    if (ngi == 0)
                                    {
                                        //index header1
                                        col = 4;
                                        numCol = sanPhams.Count * thanhPhams.Count - 1;
                                        lastCol = col + numCol + thanhPhams.Count;
                                    }
                                    else
                                    {
                                        //index header1
                                        col = lastCol + 1;
                                        numCol = sanPhams.Count * thanhPhams.Count - 1;
                                        lastCol = col + numCol + thanhPhams.Count;
                                    }

                                    // add value header1
                                    var header1Add = $@"Ngày {ngayAdd}({sizeAdd})";
                                    worksheet.Range[9, col, 9, lastCol].Merge();
                                    worksheet.Range[9, col, 9, lastCol].Value2 = $@"{header1Add}";
                                    if (ngi == 0)
                                    {
                                        col2 = 4;
                                        numCol2 = sanPhams.Count - 1;
                                        lastCol2 = col2 + numCol2 + 1;
                                    }
                                    else
                                    {
                                        col2 = lastCol2 + 1;
                                        numCol2 = sanPhams.Count - 1;
                                        lastCol2 = col2 + numCol2 + 1;
                                    }

                                    // add value header2
                                    var header2Add = $@"{thanhPham.ThanhPhamName}";
                                    worksheet.Range[10, col2, 10, lastCol2].Merge();
                                    worksheet.Range[10, col2, 10, lastCol2].Value2 = $@"{header2Add}";
                                    for (var spi = 0; spi < sanPhams.Count; spi++)
                                    {
                                        var sanPham = sanPhams[spi];
                                        var sanPhamName = sanPham.SanPhamName.Split('|')[1];
                                        //index  header3
                                        if (spi == 0 && ngi == 0)
                                        {
                                            col3 = 4;
                                            numCol3 = 1 - 1;
                                            lastCol3 = col3 + numCol3;
                                        }
                                        else
                                        {
                                            col3 = lastCol3 + 1;
                                            numCol3 = 1 - 1;
                                            lastCol3 = col3 + numCol3;
                                        }

                                        //add value header3
                                        var header3Add = $@"{sanPhamName}";
                                        //worksheet.Range[11, col3, 11, lastCol3].Merge();
                                        worksheet.Range[11, col3, 11, lastCol3].Value2 = $@"{header3Add}";
                                        worksheet.Range[11, lastCol3 + 1, 11, lastCol3 + 1].Value2 = @"TT";
                                        //worksheet.Range[11, lastCol3 + 1].Value2 = $@"TT";
                                    }

                                    lastCol3++;
                                    worksheet.Range[11, lastCol3, 11, lastCol3].Value2 = $@"{"TT"}";
                                }
                            }


                            worksheet.Range[9, 1, 11, lastCol].CellStyle.Color = color; //color;
                            worksheet.Range[9, 4, 11, lastCol].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[9, 4, 11, lastCol].VerticalAlignment = ExcelVAlign.VAlignCenter;
                        }

                        for (var si = 0; si < sizesOfTong.Count; si++)
                        {
                            var size = sizesOfTong[si];

                            //header ngay(size)


                            //var ngayAdd = ngay.ToString("dd/MM");
                            var sizeAdd = size.SizeName;

                            for (var tpi = 0; tpi < thanhPhams.Count; tpi++)
                            {
                                //header tên thành phẩm
                                var thanhPham = thanhPhams[tpi];
                                //if (thanhPham.Ngay != ngay.Ngay)
                                //{
                                //    continue;
                                //}

                                var sanPhams = items.Cast<dynamic>()
                                    .ToList().Where(x =>
                                        x.ThanhPhamName == thanhPham.ThanhPhamName &&
                                        x.SizeName == size.SizeName)
                                    .GroupBy(x => new
                                    {
                                        x.SanPhamName
                                    })
                                    .Select(x => new
                                    {
                                        SanPhamName = (string)x.Key.SanPhamName
                                    }).OrderBy(x => x.SanPhamName).Distinct().ToList();
                                if (sanPhams.Count <= 0) continue;
                                //if (ngi == 0)
                                //{
                                //    //index header1
                                //    col = 4;
                                //    numCol = sanPhams.Count * thanhPhams.Count - 1;
                                //    lastCol = col + numCol + thanhPhams.Count;
                                //}
                                //else
                                //{
                                //index header1
                                col = lastCol + 1;
                                numCol = sanPhams.Count * thanhPhams.Count - 1;
                                lastCol = col + numCol + thanhPhams.Count;
                                //}

                                // add value header1
                                var header1Add = $@"Tổng ({sizeAdd})";
                                worksheet.Range[9, col, 9, lastCol].Merge();
                                worksheet.Range[9, col, 9, lastCol].Value2 = $@"{header1Add}";
                                //if (ngi == 0)
                                //{
                                //    col2 = 4;
                                //    numCol2 = sanPhams.Count - 1;
                                //    lastCol2 = col2 + numCol2 + 1;
                                //}
                                //else
                                //{
                                col2 = lastCol2 + 1;
                                numCol2 = sanPhams.Count - 1;
                                lastCol2 = col2 + numCol2 + 1;
                                //}

                                // add value header2
                                var header2Add = $@"{thanhPham.ThanhPhamName}";
                                worksheet.Range[10, col2, 10, lastCol2].Merge();
                                worksheet.Range[10, col2, 10, lastCol2].Value2 = $@"{header2Add}";
                                for (var spi = 0; spi < sanPhams.Count; spi++)
                                {
                                    var sanPham = sanPhams[spi];
                                    var sanPhamName = sanPham.SanPhamName.Split('|')[1];
                                    //index  header3
                                    //if (spi == 0 && ngi == 0)
                                    //{
                                    //    col3 = 4;
                                    //    numCol3 = 1 - 1;
                                    //    lastCol3 = col3 + numCol3;
                                    //}
                                    //else
                                    //{
                                    col3 = lastCol3 + 1;
                                    numCol3 = 1 - 1;
                                    lastCol3 = col3 + numCol3;
                                    //}

                                    //add value header3
                                    var header3Add = $@"{sanPhamName}";
                                    //worksheet.Range[11, col3, 11, lastCol3].Merge();
                                    worksheet.Range[11, col3, 11, lastCol3].Value2 = $@"{header3Add}";
                                    worksheet.Range[11, lastCol3 + 1].Value2 = @"TT";
                                }

                                lastCol3++;
                            }

                            worksheet.Range[9, 1, 11, lastCol].CellStyle.Color = color; //color;
                            worksheet.Range[9, 4, 11, lastCol].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[9, 4, 11, lastCol].VerticalAlignment = ExcelVAlign.VAlignCenter;
                        }

                        //data
                        var rowNum = 0;
                        var numColData = 0;
                        var colData = 4;
                        var lastColData = 0;
                        decimal tongTL = 0;

                        if (nhanviens.Count > 0 && nhanviens.Any())
                        {
                            for (var ngi = 0; ngi < ngays.Count; ngi++)
                            {
                                var ngay = ngays[ngi];
                                for (var si = 0; si < sizes.Count; si++)
                                {
                                    var size = sizes[si];
                                    if (size.Ngay != ngay.Ngay) continue;
                                    for (var tpi = 0; tpi < thanhPhams.Count; tpi++)
                                    {
                                        var thanhPham = thanhPhams[tpi];
                                        //if (thanhPham.Ngay != ngay.Ngay)
                                        //{
                                        //    continue;
                                        //}
                                        var sanPhams = items.Cast<dynamic>()
                                            .ToList().Where(x =>
                                                x.ThanhPhamName == thanhPham.ThanhPhamName &&
                                                x.SizeName == size.SizeName &&
                                                ((DateTime)x.Ngay).Date == ngay.Ngay.Date)
                                            .GroupBy(x => new
                                            {
                                                x.SanPhamName
                                            })
                                            .Select(x => new
                                            {
                                                SanPhamName = (string)x.Key.SanPhamName
                                            }).OrderBy(x => x.SanPhamName).Distinct().ToList();
                                        if (sanPhams.Count <= 0) continue;
                                        for (var spi = 0; spi < sanPhams.Count; spi++)
                                        {
                                            var sanPham = sanPhams[spi];

                                            // index data
                                            if (spi == 0 && ngi == 0)
                                            {
                                                colData = 4;
                                                numColData = 1 - 1;
                                                lastColData = colData + numColData;
                                            }
                                            else
                                            {
                                                colData = lastColData + 1;
                                                numColData = 1 - 1;
                                                lastColData = colData + numColData;
                                            }

                                            tongTL = 0;
                                            for (var i = 0; i < nhanviens.Count; i++)
                                            {
                                                rowNum = i;
                                                var nhanVien = nhanviens[i];
                                                var STT = i + 1;
                                                worksheet.Range[12 + i, 1, 12 + i, 1].Value2 =
                                                    STT.ToString();
                                                worksheet.Range[12 + i, 2, 12 + i, 2].Value2 =
                                                    nhanVien.NhanVienName;
                                                worksheet.Range[12 + i, 3, 12 + i, 3].Value2 =
                                                    nhanVien.MaHoSo;

                                                var trongLuong = itemDycs.Where(
                                                        x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                             x.Ngay == ngay.Ngay &&
                                                             x.SizeName == size.SizeName &&
                                                             x.ThanhPhamName == thanhPham.ThanhPhamName &&
                                                             x.SanPhamName == sanPham.SanPhamName)
                                                    .Select(x => (decimal)x.TrongLuong).DefaultIfEmpty(0)
                                                    .Sum();

                                                worksheet.Range[12 + i, colData, 12 + i, colData].Value2 =
                                                    trongLuong;

                                                tongTL += trongLuong;
                                                if (i == nhanviens.Count - 1)
                                                    worksheet.Range[12 + i + 1, colData].Value2 = tongTL;
                                            }
                                        }

                                        lastColData++;
                                    }
                                }
                            }

                            for (var si = 0; si < sizesOfTong.Count; si++)
                            {
                                var size = sizesOfTong[si];

                                for (var tpi = 0; tpi < thanhPhams.Count; tpi++)
                                {
                                    var thanhPham = thanhPhams[tpi];
                                    //if (thanhPham.Ngay != ngay.Ngay)
                                    //{
                                    //    continue;
                                    //}
                                    var sanPhams = items.Cast<dynamic>()
                                        .ToList().Where(x =>
                                            x.ThanhPhamName == thanhPham.ThanhPhamName &&
                                            x.SizeName == size.SizeName)
                                        .GroupBy(x => new
                                        {
                                            x.SanPhamName
                                        })
                                        .Select(x => new
                                        {
                                            SanPhamName = (string)x.Key.SanPhamName
                                        }).OrderBy(x => x.SanPhamName).Distinct().ToList();
                                    if (sanPhams.Count <= 0) continue;
                                    for (var spi = 0; spi < sanPhams.Count; spi++)
                                    {
                                        var sanPham = sanPhams[spi];

                                        //// index data
                                        //if (spi == 0 && ngi == 0)
                                        //{
                                        //    colData = 4;
                                        //    numColData = 1 - 1;
                                        //    lastColData = colData + numColData;
                                        //}
                                        //else
                                        //{
                                        colData = lastColData + 1;
                                        numColData = 1 - 1;
                                        lastColData = colData + numColData;
                                        //}

                                        tongTL = 0;
                                        for (var i = 0; i < nhanviens.Count; i++)
                                        {
                                            rowNum = i;
                                            var nhanVien = nhanviens[i];


                                            var trongLuong = itemDycs.Where(
                                                    x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                         x.SizeName == size.SizeName &&
                                                         x.ThanhPhamName == thanhPham.ThanhPhamName &&
                                                         x.SanPhamName == sanPham.SanPhamName)
                                                .Select(x => (decimal)x.TrongLuong).DefaultIfEmpty(0)
                                                .Sum();

                                            worksheet.Range[12 + i, colData, 12 + i, colData].Value2 =
                                                trongLuong;

                                            tongTL += trongLuong;
                                            if (i == nhanviens.Count - 1)
                                                worksheet.Range[12 + i + 1, colData].Value2 = tongTL;
                                        }
                                    }

                                    lastColData++;
                                }
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Mesages = "Danh Sách Nhân Viên Trống!"
                            });
                        }

                        markersProcessor.AddVariable(
                            "thangNam",
                            ngayThangNam,
                            VariableTypeAction.DetectDataType);
                        markersProcessor.ApplyMarkers();
                        worksheet.Range[12, 4, 12 + rowNum + 1, lastCol].NumberFormat =
                            "#,##0.00;(#,##0.00);_( \"-\"_);_(@_)";
                        worksheet.Range[9, 1, 12 + rowNum + 1, lastCol].BorderAround();
                        worksheet.Range[9, 1, 12 + rowNum + 1, lastCol].BorderInside();
                        worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, 3].Merge();
                        worksheet.Range[12 + rowNum + 1, 1].Value2 = "TỎNG CỘNG";
                        worksheet.Range[12 + rowNum + 1, 1].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;
                        worksheet.Range[12 + rowNum + 1, 1].VerticalAlignment = ExcelVAlign.VAlignCenter;
                        worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, lastCol].CellStyle.Color =
                            color; //color;
                        worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, lastCol].CellStyle.Font.Bold =
                            true;

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
                }
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Không có dữ liệu!"
                });
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
        public async Task<IEnumerable<object>> GetTongHopNhanViensSCHN(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopNhanVien/{fromDate?.ToString("yyyy-MM-dd")},{dateTime?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanViensHN(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopNhanViensHN/{fromDate?.ToString("yyyy-MM-dd")},{dateTime?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> ExportTongHopNangSuatHNSuaCaV3(DateTime fromDate, DateTime dateTime, string? xuongId)

        {

            try
            {
                var TemplateTongHopNangSuatHN = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "TemplateNS_HN.xlsx");
                var vmApp = AppViewModel.Instance;
                //từ ngày
                var day = vmApp.DateReport.Day;
                var month = vmApp.DateReport.Month;
                var year = vmApp.DateReport.Year;
                //đến ngày
                var fromDay = vmApp.FromDate.Day;
                var fromMonth = vmApp.FromDate.Month;
                var fromYear = vmApp.FromDate.Year;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;

                //Color color = Color.FromArgb(50, 205, 50);
                //string thangNam = month + "/" + year;
                //var thangNam = $@"   THÁNG {month.ToString("00")} / {year}";
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //string[] thangNam = new string[] { month + "/" + year };
                var items2 = await GetTongHopNhanViensSCHN(fromDate, dateTime, xuongId);
                var rs = items2 != null ? items2.Cast<dynamic>().ToList() : new List<dynamic>();
                var items = rs.Where(x => x.TrongLuongNhan > 0).Cast<object>().ToList();
                //lấy danh sách ko trùng
                //lấy danh sách loại thanh fphaamr size
                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(TemplateTongHopNangSuatHN, FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    worksheet.EnableSheetCalculations();
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    var color = worksheet.Range[9, 1, 9, 1].CellStyle.Color;
                    // Xử lý
                    if (_items.Rows.Count > 0)
                    {
                        var nhanviens = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaNhanVien, x.NhanVienName, x.MaHoSo })
                            .Select(
                                x => new
                                {
                                    MaNhanVien = (string)x.Key.MaNhanVien,
                                    NhanVienName = (string)x.Key.NhanVienName,
                                    MaHoSo = (string)x.Key.MaHoSo
                                })
                            .OrderBy(x => x.MaHoSo)
                            .ToList();

                        var headers = items.Cast<dynamic>()
                            .ToList()
                            .Where(x => x.BaoCaoDauRot == false)
                            .GroupBy(
                                x => new
                                {
                                    x.MaThanhPham,
                                    x.ThanhPhamName,
                                    x.MaSize,
                                    x.SizeName,
                                    x.BaoCaoDauRot
                                })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = (string)x.Key.MaThanhPham,
                                    ThanhPhamName = (string)x.Key.ThanhPhamName,
                                    MaSize = (string)x.Key.MaSize,
                                    SizeName = (string)x.Key.SizeName,
                                    BaoCaoDauRot = (bool)x.Key.BaoCaoDauRot
                                })
                            .OrderBy(x => x.BaoCaoDauRot)
                            .ToList();
                        var headers2 = items.Cast<dynamic>()
                            .ToList()
                            .Where(x => x.BaoCaoDauRot == true)
                            .GroupBy(
                                x => new
                                {
                                    x.MaThanhPham,
                                    x.ThanhPhamName,
                                    x.MaSize,
                                    x.SizeName,
                                    x.BaoCaoDauRot
                                })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = (string)x.Key.MaThanhPham,
                                    ThanhPhamName = (string)x.Key.ThanhPhamName,
                                    MaSize = (string)x.Key.MaSize,
                                    SizeName = (string)x.Key.SizeName,
                                    BaoCaoDauRot = (bool)x.Key.BaoCaoDauRot
                                })
                            .OrderBy(x => x.BaoCaoDauRot)
                            .ToList();
                        headers.AddRange(headers2);
                        var itemDycs = items.Cast<dynamic>().ToList();
                        //them tao header data
                        var numCol = 0;
                        var col = 4;
                        var lastCol = 0;
                        //var saveI = 0;
                        for (var i = 0; i < headers.Count; i++)
                        {
                            //saveI = i;
                            // sau môi lần lặp lại phải kiểm tra đc vị trí cột hiện tại để merger loại tp tiếp theo


                            //worksheet.InsertColumn(9, 3+i);

                            var header = headers[i];

                            if (header.BaoCaoDauRot == false)
                            {
                                if (i == 0)
                                {
                                    col = 4;
                                    numCol = 2;
                                    lastCol = col + numCol;
                                }
                                else
                                {
                                    col = lastCol + 1;
                                    numCol = 2;
                                    lastCol = col + numCol;
                                }

                                var headerGroupName = $@"{header.ThanhPhamName} {header.SizeName}";
                                worksheet.Range[9, col, 10, lastCol].Merge();
                                worksheet.Range[9, col].Value2 = $@"{headerGroupName}";

                                worksheet.Range[11, col, 11, col].Value2 = @"Đầu Vào";
                                worksheet.Range[11, col + 1, 11, col + 1].Value2 = @"Đầu Ra";
                                worksheet.Range[11, col + 2, 11, col + 2].Value2 = @"Định Mức";
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    col = 4;
                                    numCol = 1;
                                    lastCol = col + numCol;
                                }
                                else
                                {
                                    col = lastCol + 1;
                                    numCol = 1;
                                    lastCol = col + numCol;
                                }

                                var headerGroupName = $@"{header.ThanhPhamName} {header.SizeName}";
                                worksheet.Range[9, col, 10, lastCol].Merge();
                                worksheet.Range[9, col].Value2 = $@"{headerGroupName}";
                                worksheet.Range[11, col, 11, col].Value2 = @"Đậu";
                                worksheet.Range[11, col + 1, 11, col + 1].Value2 = @"Rớt";
                            }

                            //dong tren
                            //worksheet.Range[8, lastCol, 8, lastCol].Value2 = $@"ĐVT: KG";
                            //var color = System.Drawing.Color.FromArgb(50, 205, 50);
                            worksheet.Range[9, 1, 11, lastCol].CellStyle.Color = color; //color;
                            worksheet.Range[9, 4, 11, lastCol].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[9, 4, 11, lastCol].VerticalAlignment = ExcelVAlign.VAlignCenter;
                            //worksheet.Range[9, 1, 11, lastCol].CellStyle.FillBackground = worksheet.Range[9, 1, 11, lastCol].CellStyle.FillBackground;
                            worksheet.Range[9, 1, 11, lastCol].BorderAround();
                            worksheet.Range[9, 1, 11, lastCol].BorderInside();
                            //worksheet.Range[9, 1, 11, lastCol].CellStyle.Font.Bold = true;
                            //worksheet.Range[9, 1, 11, lastCol].CellStyle.Font.Size = 13;
                        }

                        //data
                        var numColData = 0;
                        var colData = 4;
                        var lastColData = 0;
                        var rowNum = 0;
                        var colSum = 0;
                        decimal tongTLT = 0;
                        decimal tongDau = 0;
                        decimal tongRot = 0;
                        for (var i = 0; i < headers.Count; i++)
                        {
                            var header = headers[i];
                            tongTLT = 0;
                            tongDau = 0;
                            tongRot = 0;
                            if (header.BaoCaoDauRot == false)
                            {
                                if (i == 0)
                                {
                                    colData = 4;
                                    numColData = 2;
                                    lastColData = colData + numColData;
                                }
                                else
                                {
                                    //nếu i++ thì cột cuối cùng của tp +1

                                    //sau khi xác định được cột bất đầu rồi thì không cho nó tăng nữa
                                    //giữ vị trí cột đầu tiên trong khi i chưa tăng
                                    //j++ nhân viên sẽ tăng sau mỗi dòng

                                    colData = lastColData + 1;
                                    numColData = 2;
                                    lastColData = colData + numColData;
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    colData = 4;
                                    numColData = 1;
                                    lastColData = colData + numColData;
                                }
                                else
                                {
                                    colData = lastColData + 1;
                                    numColData = 1;
                                    lastColData = colData + numColData;
                                }
                            }


                            for (var j = 0; j < nhanviens.Count; j++)
                            {
                                rowNum = j;
                                //worksheet.InsertRow(12 +j, 1, ExcelInsertOptions.FormatAsBefore);
                                var nhanVien = nhanviens[j];
                                //var STT = 0;

                                var STT = j + 1;
                                //if (STT == 0)
                                //{
                                //    STT = j+1;

                                //}
                                worksheet.Range[12 + j, 1, 12 + j, 1].Value2 = STT.ToString();
                                worksheet.Range[12 + j, 2, 12 + j, 2].Value2 = nhanVien.NhanVienName;
                                worksheet.Range[12 + j, 3, 12 + j, 3].Value2 = nhanVien.MaHoSo;

                                if (header.BaoCaoDauRot == false)
                                {
                                    //dau vao, dau ra, dinh muc
                                    var trongLuongDauVao = itemDycs.Where(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                 x.MaSize == header.MaSize &&
                                                 x.MaThanhPham == header.MaThanhPham)
                                        .Select(x => (decimal)x.TrongLuongNhan)
                                        .DefaultIfEmpty(0)
                                        .Sum();
                                    var trongLuongDauRa = itemDycs.Where(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                 x.MaSize == header.MaSize &&
                                                 x.MaThanhPham == header.MaThanhPham)
                                        .Select(x => (decimal)x.TrongLuongTra)
                                        .DefaultIfEmpty(0)
                                        .Sum();
                                    var dinhMuc = trongLuongDauRa == 0
                                        ? 0
                                        : Math.Round(trongLuongDauVao / trongLuongDauRa, 3);
                                    //

                                    worksheet.Range[12 + j, colData, 12 + j, colData].Value2 =
                                        trongLuongDauVao;
                                    worksheet.Range[12 + j, colData + 1, 12 + j, colData + 1].Value2 =
                                        trongLuongDauRa;
                                    worksheet.Range[12 + j, colData + 2, 12 + j, colData + 2].Value2 =
                                        dinhMuc;
                                    tongTLT += trongLuongDauRa;
                                    if (j == nhanviens.Count - 1)
                                        worksheet.Range[12 + j + 1, colData + 1, 12 + j + 1, colData + 1]
                                            .Value2 = tongTLT;
                                }
                                else
                                {
                                    //trong luong dau rot
                                    var trongLuongDau = itemDycs.Where(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                 x.MaSize == header.MaSize &&
                                                 x.MaThanhPham == header.MaThanhPham &&
                                                 x.DanhGia == true)
                                        .Select(x => (decimal)x.TrongLuongTra)
                                        .DefaultIfEmpty(0)
                                        .Sum();


                                    var trongLuongRot = itemDycs.Where(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                 x.MaSize == header.MaSize &&
                                                 x.MaThanhPham == header.MaThanhPham &&
                                                 x.DanhGia == false)
                                        .Select(x => (decimal)x.TrongLuongTra)
                                        .DefaultIfEmpty(0)
                                        .Sum();
                                    //
                                    worksheet.Range[12 + j, colData, 12 + j, colData].Value2 =
                                        trongLuongDau;
                                    worksheet.Range[12 + j, colData + 1, 12 + j, colData + 1].Value2 =
                                        trongLuongRot;
                                    tongDau += trongLuongDau;
                                    tongRot += trongLuongRot;
                                    if (j == nhanviens.Count - 1)
                                    {
                                        worksheet.Range[12 + j + 1, colData, 12 + j + 1, colData].Value2 =
                                            tongDau;
                                        worksheet.Range[12 + j + 1, colData + 1, 12 + j + 1, colData + 1]
                                            .Value2 = tongRot;
                                    }
                                }
                            }
                            // them o tong
                        }

                        worksheet.Range[12, 1, 12 + rowNum + 1, lastCol].BorderAround();
                        worksheet.Range[12, 1, 12 + rowNum + 1, lastCol].BorderInside();

                        worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, 3].Merge();
                        worksheet.Range[12 + rowNum + 1, 1].Value2 = @"TỔNG CỘNG";
                        worksheet.Range[12 + rowNum + 1, 1].CellStyle.Font.Bold = true;
                        worksheet.Range[12 + rowNum + 1, 1, 12 + rowNum + 1, lastCol].CellStyle.Color =
                            color;

                        worksheet.Range[12 + rowNum + 2, 1, 12 + rowNum + 2, lastCol].Merge();
                        worksheet.Range[12 + rowNum + 2, 1].Value2 =
                            $@"Thanh Bình, ngày {dayNow.ToString("dd")} tháng {monthNow.ToString("00")} năm {yearNow.ToString("")}";
                        worksheet.Range[12 + rowNum + 2, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        worksheet.Range[12 + rowNum + 2, 1].CellStyle.Font.Size = 14;
                        worksheet.Range[12 + rowNum + 2, 1].CellStyle.Font.Italic = true;

                        worksheet.Range[12 + rowNum + 3, 1, 12 + rowNum + 3, 3].Merge();
                        worksheet.Range[12 + rowNum + 3, 1].Value2 = @"Phụ Trách Sản Suất";
                        worksheet.Range[12 + rowNum + 3, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[12 + rowNum + 3, lastCol - 3, 12 + rowNum + 3, lastCol].Merge();
                        worksheet.Range[12 + rowNum + 3, lastCol - 3].Value2 = @"Người Lập bảng";
                        worksheet.Range[12 + rowNum + 3, lastCol - 3].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;

                        markersProcessor.AddVariable(
                            "thangNam",
                            ngayThangNam,
                            VariableTypeAction.DetectDataType);
                        markersProcessor.ApplyMarkers();
                        //workbook.Version = ExcelVersion.Excel2007;
                        //worksheet.EnableSheetCalculations();
                        //worksheet.Calculate();
                        //worksheet.DisableSheetCalculations();
                        worksheet.Range[12, 4, 12 + rowNum + 1, lastCol].NumberFormat =
                            "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";

                        //worksheet.Range[13, 4, 13 + rowNum + 1, lastCol].CellStyle = numberFormat;
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
                }
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Không có dữ liệu!"
                });
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
        public async Task<IEnumerable<object>> GetTongHopNhanViensHNFillet(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopNhanViens/{fromDate?.ToString("yyyy-MM-dd")},{dateTime?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> ExportTongHopNangSuatHNFillet(DateTime fromDate, DateTime dateTime, string? xuongId)
        {

            try
            {
                //var TemplateTongHopNangSuatHNFillet = $@"{AppViewModel.Instance.AppPath}{@"\XlsIOTemplate\TemplateNSFillet_HN.xlsx"}";
                var TemplateTongHopNangSuatHNFillet = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "TemplateNSFillet_HN.xlsx");
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                //Color color = Color.FromArgb(50, 205, 50);
                //string thangNam = month + "/" + year;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //string[] thangNam = new string[] { month + "/" + year };
                var items1 = await GetTongHopNhanViensHNFillet(fromDate, dateTime, xuongId);
                var rs = items1 != null ? items1.Cast<dynamic>().ToList() : new List<dynamic>();
                var items = rs.Cast<dynamic>().ToList();
                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(
                        TemplateTongHopNangSuatHNFillet,
                        FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    worksheet.EnableSheetCalculations();
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    var color = worksheet.Range[9, 1, 9, 1].CellStyle.Color;
                    //xử lý code
                    if (_items.Rows.Count > 0)
                    {
                        var nhanviens = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaNhanVien, x.NhanVienName, x.MaHoSo })
                            .Select(
                                x => new
                                {
                                    MaNhanVien = (string)x.Key.MaNhanVien,
                                    NhanVienName = (string)x.Key.NhanVienName,
                                    MaHoSo = (string)x.Key.MaHoSo
                                })
                            .OrderBy(x => x.MaNhanVien)
                            .ToList();

                        var headers = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaLoaiThanhPham, x.ThanhPhamName, x.MaSize, x.SizeName })
                            .Select(
                                x => new
                                {
                                    MaLoaiThanhPham = (string)x.Key.MaLoaiThanhPham,
                                    ThanhPhamName = (string)x.Key.ThanhPhamName,
                                    MaSize = (string)x.Key.MaSize,
                                    SizeName = (string)x.Key.SizeName
                                })
                            .OrderBy(x => x.MaLoaiThanhPham)
                            .ToList();
                        var itemDycs = items.Cast<dynamic>().ToList();
                        //them tao header data
                        var numCol = 0;
                        var col = 4;
                        var lastCol = 0;
                        //var saveI = 0;
                        for (var i = 0; i < headers.Count; i++)
                        {
                            var header = headers[i];
                            if (i == 0)
                            {
                                col = 4;
                                lastCol = col;
                            }
                            else
                            {
                                col = lastCol + 1;
                                lastCol = col;
                            }

                            var headerGroupName = $@"{header.ThanhPhamName} {header.SizeName}";
                            worksheet.Range[9, col].Value2 = $@"{headerGroupName}";


                            //dong tren
                            //worksheet.Range[8, lastCol, 8, lastCol].Value2 = $@"ĐVT: KG";
                            //var color = System.Drawing.Color.FromArgb(50, 205, 50);
                            worksheet.Range[8, 1, 9, lastCol].CellStyle.Color = color;
                            // worksheet.Range[9, 1, 11, lastCol].CellStyle.Color = ExcelKnownColors.Bright_green;

                            //worksheet.Range[9, 1, 11, lastCol].CellStyle.Font.Bold = true;
                            //worksheet.Range[9, 1, 11, lastCol].CellStyle.Font.Size = 13;
                        }

                        //format header
                        worksheet.Range[8, 4, 9, lastCol].BorderAround();
                        worksheet.Range[8, 4, 9, lastCol].BorderInside();
                        worksheet.Range[8, 4, 8, lastCol].Merge();
                        worksheet.Range[8, 4].Value2 = @"TỔNG HỢP NĂNG SUẤT FILLET";
                        worksheet.Range[8, 4].CellStyle.Font.Bold = true;
                        worksheet.Range[8, 4, 8, lastCol].HorizontalAlignment = ExcelHAlign.HAlignCenter;


                        var colData = 4;
                        var lastColData = 0;
                        var rowNum = 0;

                        decimal tongTLT = 0;
                        //xử lý data
                        for (var i = 0; i < headers.Count; i++)
                        {
                            var header = headers[i];
                            tongTLT = 0;


                            if (i == 0)
                            {
                                colData = 4;

                                lastColData = colData;
                            }
                            else
                            {
                                colData = lastColData + 1;
                                lastColData = colData;
                            }

                            for (var j = 0; j < nhanviens.Count; j++)
                            {
                                rowNum = j;

                                var nhanVien = nhanviens[j];
                                var STT = j + 1;
                                worksheet.Range[10 + j, 1, 10 + j, 1].Value2 = STT.ToString();
                                worksheet.Range[10 + j, 2, 10 + j, 2].Value2 = nhanVien.NhanVienName;
                                worksheet.Range[10 + j, 3, 10 + j, 3].Value2 = nhanVien.MaHoSo;
                                //dau vao, dau ra, dinh muc
                                var trongLuong = itemDycs.Where(
                                        x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                             x.MaSize == header.MaSize &&
                                             x.MaLoaiThanhPham == header.MaLoaiThanhPham)
                                    .Select(x => (decimal)x.TrongLuong)
                                    .DefaultIfEmpty(0)
                                    .Sum();

                                worksheet.Range[10 + j, colData, 10 + j, colData].Value2 = trongLuong;

                                tongTLT += trongLuong;
                                if (j == nhanviens.Count - 1)
                                    worksheet.Range[10 + j + 1, colData, 10 + j + 1, colData].Value2 =
                                        tongTLT;
                            }
                            // them o tong
                        }

                        //boder datta
                        worksheet.Range[10, 1, 10 + rowNum + 1, lastCol].BorderAround();
                        worksheet.Range[10, 1, 10 + rowNum + 1, lastCol].BorderInside();
                        worksheet.Range[10, 4, 10 + rowNum + 1, lastCol].NumberFormat =
                            "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";

                        //format tong cong
                        worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 3].Merge();
                        worksheet.Range[10 + rowNum + 1, 1].Value2 = @"TỔNG CỘNG";
                        worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 3].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;
                        worksheet.Range[10 + rowNum + 1, 1].CellStyle.Font.Bold = true;
                        worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, lastCol].CellStyle.Color =
                            color;
                        //ngay lap bang
                        worksheet.Range[10 + rowNum + 2, 1, 10 + rowNum + 2, lastCol].Merge();
                        worksheet.Range[10 + rowNum + 2, 1].Value2 =
                            $@"Thanh Bình, ngày {dayNow.ToString("dd")} tháng {monthNow.ToString("00")} năm {yearNow.ToString("")}";
                        worksheet.Range[10 + rowNum + 2, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        worksheet.Range[10 + rowNum + 2, 1].CellStyle.Font.Size = 14;
                        worksheet.Range[10 + rowNum + 2, 1].CellStyle.Font.Italic = true;

                        //người lap
                        worksheet.Range[10 + rowNum + 3, 1, 12 + rowNum + 3, 3].Merge();
                        worksheet.Range[10 + rowNum + 3, 1].Value2 = @"Phụ Trách Sản Suất";
                        worksheet.Range[10 + rowNum + 3, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[10 + rowNum + 3, lastCol - 3, 12 + rowNum + 3, lastCol].Merge();
                        worksheet.Range[10 + rowNum + 3, lastCol - 3].Value2 = @"Người Lập bảng";
                        worksheet.Range[10 + rowNum + 3, lastCol - 3].HorizontalAlignment =
                            ExcelHAlign.HAlignCenter;
                        worksheet.Range[10 + rowNum + 3, lastCol - 3].VerticalAlignment =
                            ExcelVAlign.VAlignCenter;
                    }


                    markersProcessor.AddVariable(
                        "ngayThangNam",
                        ngayThangNam,
                        VariableTypeAction.DetectDataType);
                    markersProcessor.ApplyMarkers();
                    workbook.Version = ExcelVersion.Excel2007;
                    worksheet.EnableSheetCalculations();
                    worksheet.Calculate();
                    worksheet.DisableSheetCalculations();
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
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Không có dữ liệu!"
                });
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
        public async Task<IEnumerable<object>> GetTongHopNhanVienNV(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopNhanVien/{fromDate?.ToString("yyyy-MM-dd")},{dateTime?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetTongHopNhanViens2(DateTime? fromDate, DateTime? dateTime, string? xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopNhanViens2/{fromDate?.ToString("yyyy-MM-dd")},{dateTime?.ToString("yyyy-MM-dd")},{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<DG_SanPhamTinhLuong>> GetAllsSPTLNV()
        {
            IEnumerable<DG_SanPhamTinhLuong> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<DG_SanPhamTinhLuong>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> ExportTongHopNSTheoNhomSuaCa_NV(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            var TemplateTongHopNSTheoNhomSuaCa_NV = "";
            if (xuongId == "1")
                TemplateTongHopNSTheoNhomSuaCa_NV = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "TemplateNSSuaCa_NV.xlsx");
            else
                TemplateTongHopNSTheoNhomSuaCa_NV = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "TemplateNSSuaCa_AD.xlsx");
            try
            {
                //xử lý
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                var items1 = await GetTongHopNhanVienNV(fromDate, dateTime, xuongId);
                var rs = items1 != null ? items1.Cast<dynamic>().ToList() : new List<dynamic>();
                var items2 = rs.Cast<dynamic>().ToList();

                var items = items2.Where(x => x.TrongLuongNhan > 0).Cast<object>().ToList();
                var sanPhams = await GetAllsSPTLNV();
                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(
                        TemplateTongHopNSTheoNhomSuaCa_NV,
                        FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    worksheet.EnableSheetCalculations();
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();
                    var color = worksheet.Range[9, 1, 9, 1].CellStyle.Color;
                    //xử lý code
                    if (_items.Rows.Count > 0)
                    {
                        var nhanviens = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaNhanVien, x.NhanVienName, x.Nhom })
                            .Select(
                                x => new
                                {
                                    MaNhanVien = (string)x.Key.MaNhanVien,
                                    NhanVienName = (string)x.Key.NhanVienName,

                                    Nhom = (string)x.Key.Nhom
                                })
                            .OrderBy(x => x.MaNhanVien)
                            .ToList();


                        //var headers = items.Cast<dynamic>()
                        // .ToList()
                        // .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName, x.MaSize, x.SizeName,x.MaSanPham /*, x.Nhom*/})
                        // .Select(
                        //     x => new
                        //     {
                        //         MaThanhPham = (string)x.Key.MaThanhPham,
                        //         ThanhPhamName = (string)x.Key.ThanhPhamName,
                        //         MaSize = (string)x.Key.MaSize,
                        //         SizeName = (string)x.Key.SizeName,
                        //         MaSanPham = (string)x.Key.MaSanPham
                        //         //Nhom = (string)x.Key.Nhom
                        //     }).OrderBy(x => x.MaThanhPham)
                        // .ToList();
                        var headers = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new
                            {
                                /*x.MaSize, x.SizeName,*/
                                x.MaSanPham,
                                x.Nhom
                            })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = (string)x.Key.MaSanPham,
                                    ThanhPhamName = @"Chưa Thiết Lập Sản Phẩm",
                                    //MaSize = (string)x.Key.MaSize,
                                    //SizeName = (string)x.Key.SizeName,
                                    Nhom = (string)x.Key.Nhom
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                        var listOfNhom = items.Cast<dynamic>()
                            .ToList()
                            .Select(x => new { Nhom = (string)x.Nhom })
                            .Distinct()
                            .OrderBy(x => x.Nhom)
                            .ThenBy(x => x.Nhom)
                            .ToList();

                        for (var i = 1; i < listOfNhom.Count; i++)
                            workbook.Worksheets.AddCopyAfter(workbook.Worksheets[0]);
                        var itemDycs = items.Cast<dynamic>().ToList();

                        for (var i = 0; i < listOfNhom.Count; i++)
                        {
                            var nhoms = listOfNhom[i];


                            worksheet = workbook.Worksheets[i];
                            worksheet.Name = $@" {nhoms.Nhom}";
                            worksheet.Range[7, 3].Value2 = nhoms.Nhom;
                            //markersProcessor.ApplyMarkers();
                            var nhanviensNhom = items.Cast<dynamic>()
                                .ToList()
                                .Where(x => (string)x.Nhom == nhoms.Nhom)
                                .GroupBy(x => new { x.MaNhanVien, x.NhanVienName, x.Nhom })
                                .Select(
                                    x => new
                                    {
                                        MaNhanVien = (string)x.Key.MaNhanVien,
                                        NhanVienName = (string)x.Key.NhanVienName,

                                        Nhom = (string)x.Key.Nhom
                                    })
                                .OrderBy(x => x.MaNhanVien)
                                .ToList();

                            //    var headersNhom = items.Cast<dynamic>()
                            //.ToList().Where(x => (string)x.Nhom == nhoms.Nhom)
                            //.GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName, x.MaSize, x.SizeName /*, x.Nhom*/})
                            //.Select(
                            //    x => new
                            //    {
                            //        MaThanhPham = (string)x.Key.MaThanhPham,
                            //        ThanhPhamName = (string)x.Key.ThanhPhamName,
                            //        MaSize = (string)x.Key.MaSize,
                            //        SizeName = (string)x.Key.SizeName
                            //         //Nhom = (string)x.Key.Nhom
                            //    }).OrderBy(x => x.MaThanhPham)
                            //.ToList();
                            var headersNhom = headers.Where(x => x.Nhom == nhoms.Nhom)
                                .GroupBy(
                                    x => new
                                    {
                                        x.MaThanhPham,
                                        x.ThanhPhamName
                                        //x.MaSize,
                                        //x.SizeName /*, x.Nhom*/
                                    })
                                .Select(
                                    x => new
                                    {
                                        x.Key.MaThanhPham,
                                        ThanhPhamName = sanPhams.FirstOrDefault(
                                                            s => s.Ma == x.Key.MaThanhPham)?.Ten ??
                                                        $@" {x.Key.ThanhPhamName}: {x.Key.MaThanhPham}" //(string)x.Key.ThanhPhamName,
                                                                                                        //MaSize = (string)x.Key.MaSize,
                                                                                                        //SizeName = (string)x.Key.SizeName
                                                                                                        //Nhom = (string)x.Key.Nhom
                                    })
                                .OrderBy(x => x.MaThanhPham)
                                .ToList();
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
                                    col = 5;
                                    numCol = 1;
                                    lastCol = col + numCol;
                                }
                                else
                                {
                                    col = lastCol + 1;
                                    numCol = 1;
                                    lastCol = col + numCol;
                                }
                                //var headerGroupName = $@"{header.ThanhPhamName} {header.SizeName}";
                                //worksheet.Range[9, col].Value2 = $@"{headerGroupName}";

                                var headerGroupName = $@"{header.ThanhPhamName} - {header.MaThanhPham}";
                                worksheet.Range[7, col, 7, lastCol].Merge();

                                worksheet.Range[7, col].Value2 = $@"{headerGroupName}";
                                worksheet.Range[8, col, 8, col].Value2 = @"N.Liệu";
                                worksheet.Range[8, col + 1, 8, col + 1].Value2 = @"T.Phẩm";
                                worksheet.Range[7, 5, 8, lastCol].BorderAround();
                                worksheet.Range[7, 5, 8, lastCol].BorderInside();
                                worksheet.Range[7, 5, 8, lastCol].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[7, 5, 8, lastCol].VerticalAlignment =
                                    ExcelVAlign.VAlignCenter;
                                worksheet.Range[7, col].WrapText = true;
                            }

                            var numColData = 0;
                            var colData = 5;
                            var lastColData = 0;
                            var rowNum = 0;
                            decimal tongTLT = 0;
                            decimal trongLuongVao = 0;
                            decimal trongLuongRa = 0;

                            for (var y = 0; y < headersNhom.Count; y++)
                            {
                                var header = headersNhom[y];

                                tongTLT = 0;
                                trongLuongVao = 0;
                                trongLuongRa = 0;

                                if (y == 0)
                                {
                                    colData = 5;
                                    numColData = 1;
                                    lastColData = colData + numColData;
                                }
                                else
                                {
                                    colData = lastColData + 1;
                                    numColData = 1;
                                    lastColData = colData + numColData;
                                }

                                //for (int j = 0; j < nhanviens.Count; j++)
                                for (var j = 0; j < nhanviensNhom.Count; j++)
                                {
                                    rowNum = j;

                                    var nhanVien = nhanviensNhom[j];
                                    var STT = j + 1;
                                    worksheet.Range[9 + j, 1, 9 + j, 1].Value2 = STT.ToString();
                                    worksheet.Range[9 + j, 2, 9 + j, 2].Value2 =
                                        nhanVien.MaNhanVien; //nhanVien.NhanVienName;
                                    worksheet.Range[9 + j, 3, 9 + j, 3].Value2 =
                                        nhanVien.NhanVienName; //nhanVien.MaNhanVien;

                                    ////dau vao, dau ra, dinh muc
                                    //var trongLuongDauVao = itemDycs.Where(
                                    //    x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                    //        x.MaSize == header.MaSize &&
                                    //        x.MaThanhPham == header.MaThanhPham)
                                    //    .Select(x => (decimal)x.TrongLuongNhan)
                                    //    .DefaultIfEmpty(0)
                                    //    .Sum();
                                    //var trongLuongDauRa = itemDycs.Where(
                                    //    x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                    //        x.MaSize == header.MaSize &&
                                    //        x.MaThanhPham == header.MaThanhPham)
                                    //    .Select(x => (decimal)x.TrongLuongTra)
                                    //    .DefaultIfEmpty(0)
                                    //    .Sum();
                                    //dau vao, dau ra, dinh muc
                                    var trongLuongDauVao = itemDycs.Where(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                 //x.MaSize == header.MaSize &&
                                                 x.MaSanPham == header.MaThanhPham)
                                        .Select(x => (decimal)x.TrongLuongNhan)
                                        .DefaultIfEmpty(0)
                                        .Sum();
                                    var trongLuongDauRa = itemDycs.Where(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                 //x.MaSize == header.MaSize &&
                                                 x.MaSanPham == header.MaThanhPham)
                                        .Select(x => (decimal)x.TrongLuongTra)
                                        .DefaultIfEmpty(0)
                                        .Sum();
                                    ////var dinhMuc = trongLuongDauRa == 0 ? 0 : Math.Round(trongLuongDauVao / trongLuongDauRa, 3);
                                    ////

                                    worksheet.Range[9 + j, colData, 9 + j, colData].Value2 =
                                        trongLuongDauVao;
                                    worksheet.Range[9 + j, colData + 1, 9 + j, colData + 1].Value2 =
                                        trongLuongDauRa;
                                    //worksheet.Range[12 + j, colData + 2, 12 + j, colData + 2].Value2 = dinhMuc;

                                    trongLuongVao += trongLuongDauVao;
                                    trongLuongRa += trongLuongDauRa;
                                    if (j == nhanviensNhom.Count - 1)
                                    {
                                        worksheet.Range[9 + j + 1, colData, 9 + j + 1, colData].Value2 =
                                            trongLuongVao;
                                        worksheet.Range[9 + j + 1, colData + 1, 9 + j + 1, colData + 1]
                                            .Value2 = trongLuongRa;
                                    }
                                }
                                //them o tong
                            }

                            ////boder datta
                            worksheet.Range[9, 1, 9 + rowNum + 1, lastCol].BorderAround();
                            worksheet.Range[9, 1, 9 + rowNum + 1, lastCol].BorderInside();
                            worksheet.Range[9, 5, 9 + rowNum + 1, lastCol].NumberFormat =
                                "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";

                            ////format tong cong
                            worksheet.Range[9 + rowNum + 1, 1, 9 + rowNum + 1, 3].Merge();
                            worksheet.Range[9 + rowNum + 1, 1].Value2 = @"TỔNG CỘNG";
                            worksheet.Range[9 + rowNum + 1, 1, 9 + rowNum + 1, 3].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[9 + rowNum + 1, 1].CellStyle.Font.Bold = true;


                            worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 3].Merge();
                            worksheet.Range[10 + rowNum + 1, 1].Value2 = @"BGĐ NHÀ MÁY";
                            worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 3].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[10 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                            worksheet.Range[10 + rowNum + 1, 4, 10 + rowNum + 1, 7].Merge();
                            worksheet.Range[10 + rowNum + 1, 4].Value2 = @"TỔ TRƯỞNG";
                            worksheet.Range[10 + rowNum + 1, 4, 10 + rowNum + 1, 7].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[10 + rowNum + 1, 4].CellStyle.Font.Bold = true;

                            worksheet.Range[10 + rowNum + 1, 8, 10 + rowNum + 1, 11].Merge();
                            worksheet.Range[10 + rowNum + 1, 8].Value2 = @"THỐNG KÊ";
                            worksheet.Range[10 + rowNum + 1, 8, 10 + rowNum + 1, 11].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[10 + rowNum + 1, 8].CellStyle.Font.Bold = true;

                            worksheet.Range[6, 1, 6, 3].Merge();
                            worksheet.Range[6, 1].Value =
                                $@"Từ ngày {vmApp.FromDate.ToString("dd/MM/yyyy")} đến {vmApp.DateReport.ToString("dd/MM/yyyy")}";
                            worksheet.UsedRange.AutofitColumns();
                        }


                        //format header


                        ////ngay lap bang
                        //worksheet.Range[10 + rowNum + 2, 1, 10 + rowNum + 2, lastCol].Merge();
                        //worksheet.Range[10 + rowNum + 2, 1].Value2 = $@"Thanh Bình, ngày {dayNow.ToString("dd")} tháng {monthNow.ToString("00")} năm {yearNow.ToString("")}";
                        //worksheet.Range[10 + rowNum + 2, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        //worksheet.Range[10 + rowNum + 2, 1].CellStyle.Font.Size = 14;
                        //worksheet.Range[10 + rowNum + 2, 1].CellStyle.Font.Italic = true;

                        ////người lap
                        //worksheet.Range[10 + rowNum + 3, 1, 12 + rowNum + 3, 3].Merge();
                        //worksheet.Range[10 + rowNum + 3, 1].Value2 = $@"Phụ Trách Sản Suất";
                        //worksheet.Range[10 + rowNum + 3, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        //worksheet.Range[10 + rowNum + 3, lastCol - 3, 12 + rowNum + 3, lastCol].Merge();
                        //worksheet.Range[10 + rowNum + 3, lastCol - 3].Value2 = $@"Người Lập bảng";
                        //worksheet.Range[10 + rowNum + 3, lastCol - 3].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        //worksheet.Range[10 + rowNum + 3, lastCol - 3].VerticalAlignment = ExcelVAlign.VAlignCenter;
                    }

                    markersProcessor.AddVariable(
                        "ngayThangNam",
                        ngayThangNam,
                        VariableTypeAction.DetectDataType);

                    markersProcessor.ApplyMarkers();
                    workbook.Version = ExcelVersion.Excel2007;
                    //worksheet.EnableSheetCalculations();
                    //worksheet.Calculate();
                    //worksheet.DisableSheetCalculations();

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
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Không có dữ liệu!"
                });
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



        public async Task<IActionResult> ExportTongHopNSTheoNhomFillet_NV(DateTime fromDate, DateTime dateTime, string xuongId)
        {
            try
            {
                var TemplateTongHopNSTheoNhomFillet_NV = "";
                if (xuongId == "1")
                    TemplateTongHopNSTheoNhomFillet_NV = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "TemplateNSFillet_NV.xlsx");
                else
                    TemplateTongHopNSTheoNhomFillet_NV = Path.Combine(_vmApp.AppPath, "XlsIOTemplate", "TemplateNSFillet_AD.xlsx");
                //xử lý
                var vmApp = AppViewModel.Instance;
                //Hiện tại
                var dayNow = DateTime.Now.Date;
                var monthNow = DateTime.Now.Month;
                var yearNow = DateTime.Now.Year;
                var ngayThangNam =
                    $@"          {vmApp.FromDate.ToString("dd/MM/yyyy")} - {vmApp.DateReport.ToString("dd/MM/yyyy")} ";
                //var nhomsName = $@"";
                var items1 = await GetTongHopNhanViens2(fromDate, dateTime, xuongId);
                var rs = items1 != null ? items1.Cast<dynamic>().ToList() : new List<dynamic>();
                var items = rs.Cast<dynamic>().ToList();
                if (items != null && items.Any())
                {
                    var _items = items.ToDataTable();
                    using var excelEngine = new ExcelEngine();
                    var application = excelEngine.Excel;
                    using var fileStream = new FileStream(
                        TemplateTongHopNSTheoNhomFillet_NV,
                        FileMode.Open);
                    var workbook = application.Workbooks.Open(fileStream);
                    var worksheet = workbook.Worksheets[0];
                    worksheet.EnableSheetCalculations();
                    var markersProcessor = workbook.CreateTemplateMarkersProcessor();


                    //xử lý dữ liệu
                    if (_items.Rows.Count > 0)
                    {
                        //var nhanviens = items.Cast<dynamic>()
                        // .ToList()
                        // .GroupBy(x => new { x.MaNhanVien, x.NhanVienName, x.Nhom })
                        // .Select(
                        //     x => new
                        //     {
                        //         MaNhanVien = (string)x.Key.MaNhanVien,
                        //         NhanVienName = (string)x.Key.NhanVienName,

                        //         Nhom = (string)x.Key.Nhom

                        //     }).OrderBy(x => x.MaNhanVien)
                        // .ToList();


                        var headers = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(
                                x => new
                                {
                                    x.MaLoaiThanhPham,
                                    x.MaLuong,
                                    x.ThanhPhamName,
                                    x.MaSize,
                                    x.SizeName /*, x.Nhom*/
                                })
                            .Select(
                                x => new
                                {
                                    MaLoaiThanhPham = (string)x.Key.MaLoaiThanhPham,
                                    MaLuong = (string)x.Key.MaLuong,
                                    ThanhPhamName = (string)x.Key.ThanhPhamName,
                                    MaSize = (string)x.Key.MaSize,
                                    SizeName = (string)x.Key.SizeName
                                    //Nhom = (string)x.Key.Nhom
                                })
                            .OrderBy(x => x.MaLoaiThanhPham)
                            .ToList();

                        var listOfNhom = items.Cast<dynamic>()
                            .ToList()
                            .Select(x => new { Nhom = (string)x.Nhom })
                            .Distinct()
                            .OrderBy(x => x.Nhom)
                            .ThenBy(x => x.Nhom)
                            .ToList();

                        //copy template sang từng sheet
                        for (var ii = 1; ii < listOfNhom.Count; ii++)
                            workbook.Worksheets.AddCopyAfter(workbook.Worksheets[0]);
                        var itemDycs = items.Cast<dynamic>().ToList();
                        // xử lý dữ liệu
                        for (var i = 0; i < listOfNhom.Count; i++)
                        {
                            var nhoms = listOfNhom[i];


                            worksheet = workbook.Worksheets[i];
                            worksheet.Name = $@"{nhoms.Nhom}";
                            worksheet.Range[7, 3].Value2 = nhoms.Nhom;

                            //markersProcessor.ApplyMarkers();
                            var nhanviensNhom = items.Cast<dynamic>()
                                .ToList()
                                .Where(x => (string)x.Nhom == nhoms.Nhom)
                                .GroupBy(x => new { x.MaNhanVien, x.NhanVienName, x.Nhom })
                                .Select(
                                    x => new
                                    {
                                        MaNhanVien = (string)x.Key.MaNhanVien,
                                        NhanVienName = (string)x.Key.NhanVienName,

                                        Nhom = (string)x.Key.Nhom
                                    })
                                .OrderBy(x => x.MaNhanVien)
                                .ToList();
                            var headersNhom = items.Cast<dynamic>()
                                .ToList()
                                .Where(x => (string)x.Nhom == nhoms.Nhom)
                                .GroupBy(
                                    x => new
                                    {
                                        x.MaSize,
                                        x.SizeName,
                                        x.MaLoaiThanhPham,
                                        x.ThanhPhamName,
                                        x.MaLuong
                                    })
                                .Select(
                                    x => new
                                    {
                                        MaSize = (string)x.Key.MaSize,
                                        SizeName = (string)x.Key.SizeName,
                                        ThanhPhamName = (string)x.Key.ThanhPhamName,
                                        MaLoaiThanhPham = (string)x.Key.MaLoaiThanhPham,
                                        MaLuong = (string)x.Key.MaLuong
                                        //Nhom = (string)x.Key.Nhom
                                    })
                                .OrderBy(x => x.MaSize)
                                .ToList();

                            //them tao header data
                            var numCol = 0;
                            var col = 4;
                            var lastCol = 0;
                            //var saveI = 0;

                            for (var y = 0; y < headersNhom.Count; y++)
                            {
                                var header = headersNhom[y];
                                if (y == 0)
                                {
                                    col = 4;
                                    //numCol = 1;
                                    lastCol = col;
                                }
                                else
                                {
                                    col = lastCol + 1;
                                    //numCol = 1;
                                    lastCol = col;
                                }

                                var headerGroupName =
                                    $@"{header.ThanhPhamName} {header.SizeName} - Mã Lương: {header.MaLuong}";
                                worksheet.Range[8, col].Value2 = $@"{headerGroupName}";
                                worksheet.Range[7, 4, 8, lastCol].BorderAround();
                                worksheet.Range[7, 4, 8, lastCol].BorderInside();
                                worksheet.Range[7, 4, 7, lastCol].Merge();
                                worksheet.Range[7, 4].Value2 = @"BTP FILLET (Kg)";
                                worksheet.Range[7, 4].CellStyle.Font.Bold = true;
                                worksheet.Range[7, 4, 7, lastCol].HorizontalAlignment =
                                    ExcelHAlign.HAlignCenter;
                                worksheet.Range[8, col].WrapText = true;
                            }

                            var numColData = 0;
                            var colData = 4;
                            var lastColData = 0;
                            var rowNum = 0;
                            decimal tongTLT = 0;
                            decimal trongLuongVao = 0;
                            decimal trongLuongRa = 0;

                            for (var y = 0; y < headersNhom.Count; y++)
                            {
                                var header = headersNhom[y];
                                tongTLT = 0;


                                if (y == 0)
                                {
                                    colData = 4;

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
                                    var STT = j + 1;
                                    worksheet.Range[9 + j, 1, 9 + j, 1].Value2 = STT.ToString();
                                    worksheet.Range[9 + j, 2, 9 + j, 2].Value2 = nhanVien.MaNhanVien;
                                    worksheet.Range[9 + j, 3, 9 + j, 3].Value2 = nhanVien.NhanVienName;
                                    //dau vao, dau ra, dinh muc
                                    var trongLuong = itemDycs.Where(
                                            x => x.MaNhanVien == nhanVien.MaNhanVien &&
                                                 x.MaSize == header.MaSize &&
                                                 x.MaLoaiThanhPham == header.MaLoaiThanhPham &&
                                                 x.MaLuong == header.MaLuong)
                                        .Select(x => (decimal)x.TrongLuong)
                                        .DefaultIfEmpty(0)
                                        .Sum();

                                    worksheet.Range[9 + j, colData, 9 + j, colData].Value2 = trongLuong;

                                    tongTLT += trongLuong;
                                    if (j == nhanviensNhom.Count - 1)
                                        worksheet.Range[9 + j + 1, colData, 9 + j + 1, colData].Value2 =
                                            tongTLT;

                                    //////

                                    //worksheet.Range[9 + j, colData, 9 + j, colData].Value2 = trongLuongDauVao;
                                    //worksheet.Range[9 + j, colData + 1, 9 + j, colData + 1].Value2 = trongLuongDauRa;
                                    ////worksheet.Range[12 + j, colData + 2, 12 + j, colData + 2].Value2 = dinhMuc;

                                    //trongLuongVao += trongLuongDauVao;
                                    //trongLuongRa += trongLuongDauRa;
                                    //if (j == nhanviensNhom.Count - 1)
                                    //{

                                    //    worksheet.Range[9 + j + 1, colData, 9 + j + 1, colData].Value2 = trongLuongVao;
                                    //    worksheet.Range[9 + j + 1, colData + 1, 9 + j + 1, colData + 1].Value2 = trongLuongRa;
                                    //}
                                }
                                ///////////////////////////


                                //them o tong
                            }

                            worksheet.UsedRange.AutofitColumns();
                            ////boder datta
                            worksheet.Range[9, 1, 9 + rowNum + 1, lastCol].BorderAround();
                            worksheet.Range[9, 1, 9 + rowNum + 1, lastCol].BorderInside();
                            worksheet.Range[9, 4, 9 + rowNum + 1, lastCol].NumberFormat =
                                "#,##0.000;(#,##0.000);_( \"-\"_);_(@_)";

                            ////format tong cong
                            worksheet.Range[9 + rowNum + 1, 1, 9 + rowNum + 1, 3].Merge();
                            worksheet.Range[9 + rowNum + 1, 1].Value2 = @"TỔNG CỘNG";
                            worksheet.Range[9 + rowNum + 1, 1, 9 + rowNum + 1, 3].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[9 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                            worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 3].Merge();
                            worksheet.Range[10 + rowNum + 1, 1].Value2 = @"BGĐ NHÀ MÁY";
                            worksheet.Range[10 + rowNum + 1, 1, 10 + rowNum + 1, 3].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[10 + rowNum + 1, 1].CellStyle.Font.Bold = true;

                            worksheet.Range[10 + rowNum + 1, 4, 10 + rowNum + 1, 7].Merge();
                            worksheet.Range[10 + rowNum + 1, 4].Value2 = @"TỔ TRƯỞNG";
                            worksheet.Range[10 + rowNum + 1, 4, 10 + rowNum + 1, 7].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[10 + rowNum + 1, 4].CellStyle.Font.Bold = true;

                            worksheet.Range[10 + rowNum + 1, 8, 10 + rowNum + 1, 11].Merge();
                            worksheet.Range[10 + rowNum + 1, 8].Value2 = @"THỐNG KÊ";
                            worksheet.Range[10 + rowNum + 1, 8, 10 + rowNum + 1, 11].HorizontalAlignment =
                                ExcelHAlign.HAlignCenter;
                            worksheet.Range[10 + rowNum + 1, 8].CellStyle.Font.Bold = true;

                            worksheet.Range[6, 1, 6, 3].Merge();
                            worksheet.Range[6, 1].Value =
                                $@"Từ ngày {vmApp.FromDate.ToString("dd/MM/yyyy")} đến {vmApp.DateReport.ToString("dd/MM/yyyy")}";
                        }
                    }

                    markersProcessor.AddVariable(
                        "ngayThangNam",
                        ngayThangNam,
                        VariableTypeAction.DetectDataType);
                    //markersProcessor.AddVariable("nhom", nhomsName, VariableTypeAction.DetectDataType);
                    markersProcessor.ApplyMarkers();
                    workbook.Version = ExcelVersion.Excel2007;
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
                return Json(new
                {
                    isSuccess = false,
                    Mesages = "Không có dữ liệu!"
                });
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
    }
}
