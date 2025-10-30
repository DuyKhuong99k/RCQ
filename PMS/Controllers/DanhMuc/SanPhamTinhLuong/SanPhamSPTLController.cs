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
using SkiaSharp;
using Syncfusion.XlsIO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PMS.Controllers.DanhMuc.SanPhamTinhLuong
{
    [Authorize]
    public class SanPhamSPTLController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SanPhamSPTLController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Sản Phẩm", Func = "Xem Sản Phẩm Tính Lương / Sản Phẩm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "SanPhamView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Sản Phẩm";
            return View("~/Views/DanhMuc/SanPhamTinhLuong/SanPhamView.cshtml");
        }
        public async Task<IEnumerable<DG_SanPhamTinhLuong>> GetAlls()
        {
            IEnumerable<DG_SanPhamTinhLuong> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<DG_SanPhamTinhLuong>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew()
        {
            try
            {
                var dataSource = await GetAlls();
                if (dataSource != null)
                {

                    var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
                    .Max();
                    var id = (int.Parse(maxId) + 1).ToString("000");
                    return Json(new
                    {
                        isSuccess = true,
                        Ma = id
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
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Sản Phẩm", Func = "Thêm Sản Phẩm Tính Lương / Sản Phẩm")]
        public async Task<IActionResult> DoInsert(string ma, string ten, string ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/Insert";
            try
            {
                if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new DG_SanPhamTinhLuong
                {
                    Ma = ma,
                    Ten = ten,
                    GhiChu = ghiChu,
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
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
        public async Task<IActionResult> GetsByMa(string ma)
        {
            if (string.IsNullOrEmpty(ma))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<DG_SanPhamTinhLuong>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        GhiChu = item?.GhiChu,
                        Ma = item?.Ma,
                        Ten = item?.Ten
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Sản Phẩm", Func = "Sửa Sản Phẩm Tính Lương / Sản Phẩm")]
        public async Task<IActionResult> DoUpDate(string ma, string? ten, string? ghiChu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/Update/{ma}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new DG_SanPhamTinhLuong
                {
                    Ma = ma,
                    Ten = ten,
                    GhiChu = ghiChu
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
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
        [CustomAuthorize(Fu = "Danh Mục / Sản Phẩm Tính Lương / Sản Phẩm", Func = "Xoá Sản Phẩm Tính Lương / Sản Phẩm")]
        public async Task<IActionResult> DoDelete(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DG_SanPhamTinhLuong/Delete/{ma}";
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
                            var ma = worksheet.Cell(row, 1).GetValue<string>()?.Trim();
                            var ten = worksheet.Cell(row, 2).GetValue<string>()?.Trim();
                            var ghiChu = worksheet.Cell(row, 3).GetValue<string>()?.Trim();

                            if (!string.IsNullOrEmpty(ma) && !string.IsNullOrEmpty(ten))
                            {
                                var jsonResult = await DoInsert(ma, ten, ghiChu) as JsonResult;
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
