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
using ViewModels.Repos.HQ;

namespace PMS.Controllers.DanhMuc.Fillet
{
    [Authorize]
    public class NhanVienTheoBanFilletController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public NhanVienTheoBanFilletController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Nhân Viên Theo Bàn", Func = "Xem Fillet / Nhân Viên Theo Bàn")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhanVienTheoBanView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiBanFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BanFillets/GetAlls";
            using var helperBanFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var banfls = helperBanFL.GetAsync<IEnumerable<BanFillet>>(HttpContext, apiBanFLUrl);
            ViewBag.listBanFLs = banfls.Result.ToList();

            var apiXuongUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuong";
            using var helperXuong = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var xuongs = helperXuong.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiXuongUrl);
            ViewBag.listXuongs = xuongs.Result.ToList();


            ViewBag.TitlePage = "Nhân viên theo bàn Fillet";
            return View("~/Views/DanhMuc/Fillet/NhanVienTheoBanView.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllsFullField(string maBan, string maXuong, bool isCheckNow)
        {
            //IEnumerable<object> dataSource = ViewBag.dataSource;
            //if (dataSource == null)
            //{
            //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoBans/GetAllsFullField/{maBan}/{maXuong}";
            //    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            //    ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            //    dataSource = ViewBag.dataSource;
            //}
            //return dataSource;

            IEnumerable<object> dataSource = null;

            string endpoint = isCheckNow
                ? "GetNhanVienTheoBansMoiNhat"
                : "GetAllsFullField";

            string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoBans/{endpoint}/{maBan}/{maXuong}";

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

            return dataSource ?? Enumerable.Empty<object>();
        }
        public async Task<IEnumerable<MaThanhPhamFillet_HanMucTrongLuong>> GetAlls()
        {
            IEnumerable<MaThanhPhamFillet_HanMucTrongLuong> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_HanMucTrongLuong/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPhamFillet_HanMucTrongLuong>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNew(DateTime ngay, string maXuong)
        {
            try
            {
                var dataSource = await GetAlls();
                if (dataSource != null)
                {

                    var maxId = dataSource.Where(x => x.Ngay.Date == ngay.Date && x.MaXuong == maXuong)
                    .Select(x => x.STT)
                    .DefaultIfEmpty(0)
                    .Max();
                    var id = maxId + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        STT = id,
                        Ngay = ngay.Date,
                        MaXuong = maXuong,
                        Gio = DateTime.Now.TimeOfDay
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Nhân Viên Theo Bàn", Func = "Thêm Fillet / Nhân Viên Theo Bàn")]
        public async Task<IActionResult> DoInsert(string selectedIdsString, string maBan, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoBans/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(maBan) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maNhanViens = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var apiNhanVienBanUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoBans/GetAlls";
                using var helperNhanVienBan = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var nhanvienbansTask = helperNhanVienBan.GetAsync<IEnumerable<NhanVienTheoBan>>(HttpContext, apiNhanVienBanUrl);
                var nhanvienbans = await nhanvienbansTask;

                var maxId = nhanvienbans.Select(x => int.Parse(x.Id.Substring(9))).DefaultIfEmpty(0).Max();
                var nhanVienBans = new List<NhanVienTheoBan>();
                foreach (var item in maNhanViens)
                {

                    var nhanVienBan = new NhanVienTheoBan
                    {
                        MaNhanVien = item,
                        MaBan = maBan,
                        MaKhuVuc = "FL",
                        MaXuong = maXuong,
                        NgayGio = DateTime.Now,
                        UserName = AppViewModel.Instance.UserName,
                        PCName = AppViewModel.Instance.PCName,
                        IsDone = false
                    };

                    if (nhanVienBan != null)
                    {
                        maxId += 1;
                        var dateNow = DateTime.Now;
                        var date = dateNow.ToString("yyyyMMdd");
                        var id = $"{maxId.ToString("0000")}";
                        var idd = string.Concat(date + "." + id);
                        nhanVienBan.Id = idd;
                        nhanVienBans.Add(nhanVienBan);
                    }
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var nhanVienBan in nhanVienBans)
                {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(nhanVienBan), Encoding.UTF8, "application/json");
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!response.Success)
                    {
                        failureCount++;
                    }
                    else
                    {
                        successCount++;
                    }
                }

                if (failureCount > 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"{failureCount} phần tử không thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $"{successCount} phần tử thành công."
                    });
                }

                //return Json(new
                //{
                //    isSuccess = false,
                //    Messages = "Insert không thành công."
                //});
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có.
                return Json(new
                {
                    isSuccess = false,
                    Messages = ex.Message
                });
            }
        }
        //public async Task<IActionResult> GetsByMa(string stt, string ngay, string maXuong)
        //{
        //    if (string.IsNullOrEmpty(stt) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maXuong))
        //    {
        //        return Json(new
        //        {
        //            isSuccess = false,
        //            Mesages = "Chưa chọn thông tin!."
        //        });
        //    }
        //    IEnumerable<object> dataSource = ViewBag.dataSource;
        //    if (dataSource == null)
        //    {
        //        var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_HanMucTrongLuong/GetsByMa/{stt}/{ngay}/{maXuong}";
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //        var item = await helper.GetAsync<MaThanhPhamFillet_HanMucTrongLuong>(HttpContext, apiUrl);
        //        if (item != null)
        //        {
        //            return Json(new
        //            {
        //                isSuccess = true,
        //                Mesages = "Thành Công",
        //                STT = item.STT,
        //                Ngay = item.Ngay,
        //                Gio = item.Gio,
        //                MaLo = item.MaLo,
        //                MaThanhPham = item.MaThanhPham,
        //                TrongLuong = item.TrongLuong,
        //                MaXuong = item.MaXuong,

        //            });
        //        }
        //    }
        //    return Json(new
        //    {
        //        isSuccess = false,
        //        Mesages = "Lỗi!"
        //    });
        //}
        //[CustomAuthorize(Fu = "Danh Mục / Fillet / Nhân Viên Theo Bàn", Func = "Sửa Fillet / Nhân Viên Theo Bàn")]
        //public async Task<IActionResult> DoUpDate(string stt, string ngay, string maLo, string maThanhPham, decimal trongLuong, string maXuong)
        //{
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillet_HanMucTrongLuong/Update/{stt}/{ngay}/{maXuong}";
        //    try
        //    {
        //        // Kiểm tra dữ liệu đầu vào
        //        if (string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(maThanhPham) || string.IsNullOrEmpty(maXuong))
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Messages = "Vui lòng nhập đầy đủ thông tin."
        //            });
        //        }
        //        int number = int.Parse(stt);
        //        DateTime dateTime = DateTime.Parse(ngay);
        //        var model = new MaThanhPhamFillet_HanMucTrongLuong
        //        {
        //            STT = number,
        //            Ngay = dateTime,
        //            Gio = DateTime.Now.TimeOfDay,
        //            MaLo = maLo,
        //            MaThanhPham = maThanhPham,
        //            TrongLuong = trongLuong,
        //            MaXuong = maXuong,
        //        };

        //        var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //        var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
        //        if (rl.Success)
        //        {
        //            return Json(new
        //            {
        //                isSuccess = rl.Success,
        //                Messages = rl.Message
        //            });
        //        }
        //        return Json(new
        //        {
        //            isSuccess = rl.Success,
        //            Messages = rl.Message
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
        [CustomAuthorize(Fu = "Danh Mục / Fillet / Nhân Viên Theo Bàn", Func = "Xoá Fillet / Nhân Viên Theo Bàn")]
        public async Task<IActionResult> DoDelete(string selectedIdNhanVienBans)
        {
            try
            {
                string[] ids = selectedIdNhanVienBans.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var successCount = 0;
                var failureCount = 0;
                foreach (var id in ids)
                {
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienTheoBans/Delete/{id}";
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var rl = await helper.PostAsync(HttpContext, apiUrl, null);

                    if (!rl.Success)
                    {
                        failureCount++;
                    }
                    else
                    {
                        successCount++;
                    }

                }
                if (failureCount > 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $"{failureCount} phần tử không thành công."
                    });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $"{successCount} phần tử thành công."
                    });
                }
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Có lỗi xảy ra"
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
