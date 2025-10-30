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

namespace PMS.Controllers.DanhMucHQ
{
    [Authorize]
    public class HQ_NhanVienTheoNhomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HQ_NhanVienTheoNhomController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [CustomAuthorize(Fu = "Danh Mục HQ / Nhân Viên Theo Nhóm", Func = "Xem HQ / Nhân Viên Theo Nhóm")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "HQ_NhanVienTheoNhom");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiNhomUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_Nhoms/GetAlls";
            using var helperNhom = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhoms = helperNhom.GetAsync<IEnumerable<object>>(HttpContext, apiNhomUrl);
            ViewBag.Nhoms = nhoms.Result.ToList();

            ViewBag.TitlePage = "Nhân Viên Theo Nhóm";
            return View("~/Views/DanhMucHQ/HQ_NhanVienTheoNhom/HQ_NhanVienTheoNhomView.cshtml");
        }
        public async Task<IEnumerable<HQ_NhanVienTheoNhom>> GetAlls()
        {
            IEnumerable<HQ_NhanVienTheoNhom> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoNhoms/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<HQ_NhanVienTheoNhom>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllsNewFullField(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoNhoms/GetAllListNews/{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
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

                    long maxId = (dataSource != null && dataSource.Any())
    ? dataSource.Select(x => x.Id).Max()
    : 0;
                    var id = (maxId + 1);
                    return Json(new
                    {
                        isSuccess = true,
                        Id = id,
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
        [CustomAuthorize(Fu = "Danh Mục HQ / Nhân Viên Theo Nhóm", Func = "Thêm HQ / Nhân Viên Theo Nhóm")]
        public async Task<IActionResult> DoInsert(long id, string maNhanVien, string maNhom, DateTime ngayGioBatDau, decimal heSo)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoNhoms/Insert";
            try
            {
                if (string.IsNullOrEmpty(maNhanVien) || string .IsNullOrEmpty(maNhom))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_NhanVienTheoNhom
                {
                    Id = id,
                    MaNhanVien = maNhanVien,
                    MaNhom = maNhom,
                    NgayGioBatDau = ngayGioBatDau,
                    HeSo = heSo
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
        public async Task<IActionResult> GetsByMa(string id)
        {
            if (string.IsNullOrEmpty(id))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoNhoms/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<HQ_NhanVienTheoNhom>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item?.Id,
                        MaNhanVien = item?.MaNhanVien,
                        MaNhom = item?.MaNhom,
                        NgayGioBatDau = item?.NgayGioBatDau,
                        HeSo = item?.HeSo,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục HQ / Nhân Viên Theo Nhóm", Func = "Sửa HQ / Nhân Viên Theo Nhóm")]
        public async Task<IActionResult> DoUpDate(long id, string maNhanVien, string maNhom, DateTime ngayGioBatDau, decimal heSo)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoNhoms/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (id != null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_NhanVienTheoNhom
                {
                    Id = id,
                    MaNhanVien = maNhanVien,
                    MaNhom = maNhom,
                    NgayGioBatDau = ngayGioBatDau,
                    HeSo = heSo,
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
        [CustomAuthorize(Fu = "Danh Mục HQ / Nhân Viên Theo Nhóm", Func = "Xoá HQ / Nhân Viên Theo Nhóm")]
        public async Task<IActionResult> DoDelete(long id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoNhoms/Delete/{id}";
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
    }
}
