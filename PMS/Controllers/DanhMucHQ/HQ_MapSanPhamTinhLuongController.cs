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
using ClosedXML.Excel;
using ToolsEx;
namespace PMS.Controllers.DanhMucHQ
{
    [Authorize]
    public class HQ_MapSanPhamTinhLuongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HQ_MapSanPhamTinhLuongController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Map Sản Phẩm Tính Lương HQ", Func = "Xem Map Sản Phẩm Tính Lương HQ")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "HQ_MapSanPhamTinhLuong");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiTPHQUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThanhPhams/GetAlls";
            using var helperTPHQ = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tphqs = helperTPHQ.GetAsync<IEnumerable<HQ_ThanhPham>>(HttpContext, apiTPHQUrl);
            ViewBag.listTPHQ = tphqs.Result.ToList();


            var apiSPTLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetAlls";
            using var helperSPTL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sptls = helperSPTL.GetAsync<IEnumerable<DG_SanPhamTinhLuong>>(HttpContext, apiSPTLUrl);
            ViewBag.listSPTLs = sptls.Result.ToList();


            var apiSizeUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_Sizes/GetAlls";
            using var helperSize = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sizes = helperSize.GetAsync<IEnumerable<HQ_Size>>(HttpContext, apiSizeUrl);
            ViewBag.listSizes = sizes.Result.ToList();

            var apiLoaiNguyenLieuUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_LoaiNguyenLieus/GetAlls";
            using var helperLoaiNguyenLieu = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var loaiNguyenLieu = helperLoaiNguyenLieu.GetAsync<IEnumerable<HQ_LoaiNguyenLieu>>(HttpContext, apiLoaiNguyenLieuUrl);
            ViewBag.listLoaiNguyenLieus = loaiNguyenLieu.Result.ToList();


            ViewBag.TitlePage = "Map Sản Phẩm Tính Lương";
            return View("~/Views/DanhMucHQ/HQ_MapSanPhamTinhLuong/HQ_MapSanPhamTinhLuongView.cshtml");
        }
        public async Task<IEnumerable<HQ_MapSanPhamTinhLuong>> GetAlls()
        {
            IEnumerable<HQ_MapSanPhamTinhLuong> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_MapSanPhamTinhLuongs/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<HQ_MapSanPhamTinhLuong>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllsFullField(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_MapSanPhamTinhLuongs/GetAllsFullField/{dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        //public async Task<IActionResult> CreatDefautNew()
        //{
        //    try
        //    {
        //        var dataSource = await GetAlls();
        //        if (dataSource != null)
        //        {

        //            var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
        //            .Max();
        //            var id = (int.Parse(maxId) + 1).ToString("000");
        //            return Json(new
        //            {
        //                isSuccess = true,
        //                Ma = id,
        //            });
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Mesages = "Lỗi!"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        [CustomAuthorize(Fu = "Danh Mục / Map Sản Phẩm Tính Lương HQ", Func = "Thêm Map Sản Phẩm Tính Lương HQ")]
        public async Task<IActionResult> DoInsert(string maSanPham, string maThanhPham, string maSize, string maLoaiNguyenLieu,DateTime ngayGio)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_MapSanPhamTinhLuongs/Insert";
            try
            {
                if (string.IsNullOrEmpty(maSanPham) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maLoaiNguyenLieu))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_MapSanPhamTinhLuong
                {
                    MaSanPham = maSanPham,
                    MaThanhPham = maThanhPham,
                    MaSize = maSize,
                    MaLoaiNguyenLieu = maLoaiNguyenLieu,
                    NgayGio = ngayGio,
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
        public async Task<IActionResult> GetsByMa(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_MapSanPhamTinhLuongs/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<HQ_MapSanPhamTinhLuong>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        MaSanPham = item.MaSanPham,
                        MaThanhPham = item.MaThanhPham,
                        MaSize = item.MaSize,
                        MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                        NgayGio = item.NgayGio,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }

        [CustomAuthorize(Fu = "Danh Mục / Map Sản Phẩm Tính Lương HQ", Func = "Xóa Map Sản Phẩm Tính Lương HQ")]
        public async Task<IActionResult> DoDelete(int id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_MapSanPhamTinhLuongs/Delete/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
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

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { isSuccess = false, Messages = "Vui lòng chọn file Excel hợp lệ!" });
            }
            try
            {

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var lastRow = worksheet.LastRowUsed().RowNumber();
                        int successCount = 0;
                        int failCount = 0;
                        List<string> errorMessages = new List<string>();

                        for (int row = 2; row <= lastRow; row++)
                        {
                            //var tenSanPham = StringExtensions.NonUnicode(worksheet.Cell(row, 1).GetValue<string>()?.Trim().Replace(" ", "")).ToUpper();
                            //var tenThanhPham = StringExtensions.NonUnicode(worksheet.Cell(row, 2).GetValue<string>()?.Trim().Replace(" ", "")).ToUpper();
                            //var tenSize = StringExtensions.NonUnicode(worksheet.Cell(row, 3).GetValue<string>()?.Trim().Replace(" ", "")).ToUpper();
                            //var tenLoaiNguyenLieu = StringExtensions.NonUnicode(worksheet.Cell(row, 4).GetValue<string>()?.Trim().Replace(" ", "")).ToUpper();

                            var tenSanPham = worksheet.Cell(row, 1).GetValue<string>()?.Trim();
                            var tenThanhPham = worksheet.Cell(row, 2).GetValue<string>()?.Trim();
                            var tenSize = worksheet.Cell(row, 3).GetValue<string>()?.Trim();
                            var tenLoaiNguyenLieu = worksheet.Cell(row, 4).GetValue<string>()?.Trim();
                            var ngayGio = worksheet.Cell(row, 5).GetValue<string>()?.Trim();
                            if (ngayGio == null || ngayGio == "")
                            {
                                ngayGio = DateTime.Now.ToString();
                            }
                            if (!string.IsNullOrEmpty(tenSanPham) && !string.IsNullOrEmpty(tenThanhPham) && !string.IsNullOrEmpty(tenSize) && !string.IsNullOrEmpty(tenLoaiNguyenLieu))
                            {
                                var apiSPTLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetsByTen/{tenSanPham}";
                                using var helperSPTL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                                var maSanPhamJ = await helperSPTL.GetAsync<dynamic>(HttpContext, apiSPTLUrl);
                                string maSanPham = maSanPhamJ?.Ma;

                                var apiTPHQUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThanhPhams/GetsByTen/{tenThanhPham}";
                                using var helperTPHQ = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                                var maThanhPhamJ = await helperTPHQ.GetAsync<dynamic>(HttpContext, apiTPHQUrl);
                                string maThanhPham = maThanhPhamJ?.Ma;

                                var apiSizeUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_Sizes/GetsByTen/{tenSize}";
                                using var helperSize = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                                var maSizeJ = await helperSize.GetAsync<dynamic>(HttpContext, apiSizeUrl);
                                string maSize = maSizeJ?.Ma;

                                var apiLoaiNguyenLieuUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_LoaiNguyenLieus/GetsByTen/{tenLoaiNguyenLieu}";
                                using var helperLoaiNguyenLieu = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                                var maLoaiNguyenLieuJ = await helperLoaiNguyenLieu.GetAsync<dynamic>(HttpContext, apiLoaiNguyenLieuUrl);
                                string maLoaiNguyenLieu = maLoaiNguyenLieuJ?.Ma;
                                DateTime date = DateTime.ParseExact(ngayGio, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                var jsonResult = await DoInsert(maSanPham, maThanhPham, maSize, maLoaiNguyenLieu, date) as JsonResult;
                                dynamic resultObject = jsonResult?.Value;

                                if (resultObject != null && resultObject.isSuccess == true)
                                {
                                    successCount++;
                                }
                                else
                                {
                                    failCount++;
                                    errorMessages.Add($"Dòng {row}: {resultObject?.Messages}");
                                }
                            }
                        }

                        return Json(new
                        {
                            isSuccess = true,
                            Messages = $"Import hoàn tất",
                            successCount = successCount,
                            failCount = failCount,
                            Errors = errorMessages
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, Messages = $"Lỗi khi import: {ex.Message}" });
            }
        }
    }
}
