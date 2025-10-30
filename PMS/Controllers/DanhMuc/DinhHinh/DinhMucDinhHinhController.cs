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

namespace PMS.Controllers.DanhMuc.DinhHinh
{
    [Authorize]
    public class DinhMucDinhHinhController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DinhMucDinhHinhController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Định Mức", Func = "Xem Định Hình / Định Mức")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "DinhMucDinhHinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();


            var apiMaLoaiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLoaiCaDinhHinhs/GetAlls";
            using var helperMaLoaiCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var maloaiCas = helperMaLoaiCa.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoaiCaUrl);
            ViewBag.listMaLoaiCas = maloaiCas.Result.ToList();


            var apiMaMauUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaMauDinhHinhs/GetAlls";
            using var helperMaMau = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var mamaus = helperMaMau.GetAsync<IEnumerable<object>>(HttpContext, apiMaMauUrl);
            ViewBag.listMaMaus = mamaus.Result.ToList();


            var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAlls";
            using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiTPFLUrl);
            ViewBag.listTPFLs = tpfls.Result.ToList();

            var apiSFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaSizeDinhHinhs/GetAlls";
            using var helperSFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var sfls = helperSFL.GetAsync<IEnumerable<MaSizeDinhHinh>>(HttpContext, apiSFLUrl);
            ViewBag.listSFLs = sfls.Result.ToList();


            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();


            ViewBag.TitlePage = "Định Mức Định Hình";
            return View("~/Views/DanhMuc/DinhHinh/DinhMucDinhHinhView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucDinhHinhs/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<DinhMucDinhHinh>> GetAlls()
        {
            IEnumerable<DinhMucDinhHinh> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucDinhHinhs/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<DinhMucDinhHinh>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew(DateTime dateTime, string xuongId)
        {
            try
            {
                var dataSource = await GetAlls();
                if (dataSource != null)
                {

                    var maxId = dataSource.Where(x => x.Ngay.Date == dateTime.Date && x.MaXuong == xuongId)
                    .Select(x => x.STT)
                    .DefaultIfEmpty(0)
                    .Max();
                    var id = maxId + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = id,
                        SuDung = true,
                        CaTra = false,
                        Ngay = dateTime.Date,
                        MaXuong = xuongId,
                        DinhMuc = 0,
                        Gio = DateTime.Now
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
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Định Mức", Func = "Thêm Định Hình / Định Mức")]
        public async Task<IActionResult> DoInsert(int stt, TimeSpan gio, string maLo, string maLoaiCa, string maMau, string maSize, string maThanhPham, string maXuong, bool caTra, double dinhMuc, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucDinhHinhs/Insert";
            try
            {
                if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new DinhMucDinhHinh
                {
                    STT = stt,
                    Ngay = DateTime.Now,
                    Gio = gio,
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaMau = maMau,
                    MaSize = maSize,
                    MaThanhPham = maThanhPham,
                    MaXuong = maXuong,
                    CaTra = caTra,
                    DinhMuc = dinhMuc,
                    SuDung = suDung
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
        public async Task<IActionResult> GetsByMa(string stt, string ngay, string gio, string maLo, string maLoaiCa, string maMau, string maSize, string maThanhPham, string maXuong, string caTra)
        {
            if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucDinhHinhs/GetsByMa/{stt.ToString()}/{ngay.ToString()}/{gio.ToString()}/{maLo}/{maLoaiCa}/{maMau}/{maSize}/{maThanhPham}/{maXuong}/{caTra.ToString()}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<DinhMucDinhHinh>(HttpContext, apiUrl);
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
                        MaMau = item.MaMau,
                        MaSize = item.MaSize,
                        MaThanhPham = item.MaThanhPham,
                        MaXuong = item.MaXuong,
                        CaTra = item.CaTra,
                        DinhMuc = item.DinhMuc,
                        SuDung = item.SuDung
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Định Mức", Func = "Sửa Định Hình / Định Mức")]
        public async Task<IActionResult> DoUpDate(string stt, string ngay, string gio, string maLo, string maLoaiCa, string maMau, string maSize, string maThanhPham, string maXuong, string caTra, double dinhMuc, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucDinhHinhs/Update/{stt}/{ngay}/{gio}/{maLo}/{maLoaiCa}/{maMau}/{maSize}/{maThanhPham}/{maXuong}/{caTra}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maLoaiCa) || string.IsNullOrEmpty(maMau) || string.IsNullOrEmpty(maSize) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                int number = int.Parse(stt);
                DateTime dateTime = DateTime.Parse(ngay);
                TimeSpan timeSpan = TimeSpan.Parse(gio);
                bool booleanValue = bool.Parse(caTra);
                var model = new DinhMucDinhHinh
                {
                    STT = number,
                    Ngay = dateTime,
                    Gio = timeSpan,
                    MaLo = maLo,
                    MaLoaiCa = maLoaiCa,
                    MaMau = maMau,
                    MaSize = maSize,
                    MaThanhPham = maThanhPham,
                    MaXuong = maXuong,
                    CaTra = booleanValue,
                    DinhMuc = dinhMuc,
                    SuDung = suDung
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
        [CustomAuthorize(Fu = "Danh Mục / Định Hình / Định Mức", Func = "Xoá Định Hình / Định Mức")]
        public async Task<IActionResult> DoDelete(string stt, string ngay, string gio, string maLo, string maLoaiCa, string maMau, string maSize, string maThanhPham, string maXuong, string caTra)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/DinhMucDinhHinhs/Delete/{stt}/{ngay}/{gio}/{maLo}/{maLoaiCa}/{maMau}/{maSize}/{maThanhPham}/{maXuong}/{caTra}";
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
