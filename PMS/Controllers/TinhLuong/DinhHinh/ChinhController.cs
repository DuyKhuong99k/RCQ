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
using Models.Repos.AppModel;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Syncfusion.EJ2.Layouts;
using Syncfusion.EJ2.Navigations;
using ViewModels.Repos.HQ;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;


namespace PMS.Controllers.TinhLuong.DinhHinh
{
    [Authorize]
    public class ChinhController : Controller
    {
        private readonly IHubContext<PMS.Hubs.ProgressHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChinhController(IHubContext<PMS.Hubs.ProgressHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {

            return View();
        }
        [CustomAuthorize(Fu = "Tính Lương / Định Hình / Chính", Func = "Xem / Tính Lương / Định Hình / Chính")]
        public IActionResult ChinhView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChinhView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var codeId = HttpContext.Session.GetString("XuongId");
            var apiThanhPhamUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamDinhHinhs/GetAllsByCodeId/{codeId}";
            using var helperThanhPham = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhams = helperThanhPham.GetAsync<IEnumerable<MaThanhPhamDinhHinh>>(HttpContext, apiThanhPhamUrl);
            ViewBag.listThanhPhams = thanhPhams.Result.ToList();
            var apiThanhPhamFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAllsByCodeId/{codeId}";
            using var helperThanhPhamFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thanhPhamFLs = helperThanhPhamFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiThanhPhamFLUrl);
            ViewBag.listThanhPhamFLs = thanhPhamFLs.Result.ToList();
            var apiMaLoUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaLos/GetsMSLWithSize";
            using var helperMaLo = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var malos = helperMaLo.GetAsync<IEnumerable<object>>(HttpContext, apiMaLoUrl);
            ViewBag.listMaLos = malos.Result.ToList();

            var apiKhuVucUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/KhuVucs/Gets";
            using var helperKhuVuc = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var khuvuc = helperKhuVuc.GetAsync<IEnumerable<object>>(HttpContext, apiKhuVucUrl);
            ViewBag.listKhuVucs = khuvuc.Result.ToList();

            ViewBag.token = HttpContext.Session.GetString("JWTTokenEncry");
            ViewBag.titile = "Tính Lương Định Hình Chính";
            return View("~/Views/TinhLuong/DinhHinh/ChinhView.cshtml");
        }
        public async Task<List<BravoModelV1.Model.PhieuCanTongHopTinhLuong>> GetPhieuCanTongHopTinhLuongs2(DateTime dateTime, string xuongId, string khuVucId)
        {
            List<BravoModelV1.Model.PhieuCanTongHopTinhLuong> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetPhieuCanTongHopTinhLuongs2/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{khuVucId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<List<BravoModelV1.Model.PhieuCanTongHopTinhLuong>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }

        public async Task<List<BravoModelV1.Model.PhieuCanTongHopTinhLuong>> GetPhieuCanTongHopTinhLuong(DateTime dateTime, string xuongId, string khuVucId)
        {
            List<BravoModelV1.Model.PhieuCanTongHopTinhLuong> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetPhieuCanTongHopTinhLuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{khuVucId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<List<BravoModelV1.Model.PhieuCanTongHopTinhLuong>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<List<string>> GetEmployeeCodes(DateTime dateTime)
        {
            List<string> dataSource = null;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_BravoCheckInOuts/GetEmployeeCodes/{dateTime.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<List<string>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<BravoModelV1.Model.NhanVienKiem>> GetNhanViensKiems(string xuongId, DateTime dateTime, int type)
        {
            IEnumerable<BravoModelV1.Model.NhanVienKiem> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNhanViensKiems/{xuongId}/{dateTime.ToString("yyyy-MM-dd")}/{type}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<BravoModelV1.Model.NhanVienKiem>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<ToKiem>> GetToKiemsByXuongId(string xuongId)
        {
            IEnumerable<ToKiem> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetToKiemsByXuongId/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<ToKiem>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> GetKhuVucs()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/KhuVucs/Gets";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<IEnumerable<object>> LoadPhieuCanTongHopTinhLuong(DateTime dateTime, string xuongId, string khuVucId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/LoadPhieuCanTongHopTinhLuong/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}/{khuVucId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }

            return dataSource;
        }
        public async Task<ActionResult> Reload(DateTime dateTime, string xuongId, string khuVucId)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;

            try
            {
                dataSource = await LoadPhieuCanTongHopTinhLuong(dateTime, xuongId, khuVucId);

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
        public async Task<IEnumerable<MaThanhPham_PhoiTron>> GetAllThanhPhamPhoiTrons()
        {
            IEnumerable<MaThanhPham_PhoiTron> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPham_PhoiTron/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<MaThanhPham_PhoiTron>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllWithKhuVucThanhPhamPhoiTrons(DateTime dateTime, string maKhuVuc)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPham_PhoiTron/GetAllWithMaKhuVucs/{dateTime.ToString("yyyy-MM-dd")}/{maKhuVuc}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> DoInsertThanhPhamPhoiTron(DateTime dateTime, string maXuong, string maKhuVuc, string maThanhPhamOrg, string maThanhPhamDes, string maLo, decimal tyLe)
        {
            var userId = HttpContext.Session.GetString("Id");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPham_PhoiTron/Insert";
            try
            {
                if (string.IsNullOrEmpty(maThanhPhamOrg) || string.IsNullOrEmpty(maThanhPhamDes) || string.IsNullOrEmpty(maLo) || string.IsNullOrEmpty(tyLe.ToString()))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new MaThanhPham_PhoiTron
                {
                    Ngay = dateTime,
                    MaThanhPhamOrg = maThanhPhamOrg,
                    MaThanhPhamDes = maThanhPhamDes,
                    MaKhuVuc = maKhuVuc,
                    MaXuong = maXuong,
                    MaLo = maLo,
                    TyLe = tyLe,
                    CreateBy = userId,
                    CreateDateTime = DateTime.Now,
                    ModifyBy = userId,
                    ModifyDateTime = DateTime.Now,
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
        public async Task<IActionResult> GetsByMaThanhPhamPhoiTron(string listKeyPhanBoSL)
        {
            string[] parts = listKeyPhanBoSL.TrimEnd('|').Split(',');
            DateTime dateTime = DateTime.Parse(parts[0]);
            string maThanhPhamOrg = parts[1];
            string maThanhPhamDes = parts[2];
            string maKhuVuc = parts[3];
            string maXuong = parts[4];
            string maLo = parts[5];
            if (string.IsNullOrEmpty(maThanhPhamOrg) || string.IsNullOrEmpty(maThanhPhamDes) || string.IsNullOrEmpty(maKhuVuc) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maLo))
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPham_PhoiTron/GetsByMa/{dateTime.ToString("yyyy-MM-dd")}/{maThanhPhamOrg}/{maThanhPhamDes}/{maKhuVuc}/{maXuong}/{maLo}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<MaThanhPham_PhoiTron>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Ngay = item.Ngay,
                        MaThanhPhamOrg = item.MaThanhPhamOrg,
                        MaThanhPhamDes = item.MaThanhPhamDes,
                        MaKhuVuc = item.MaKhuVuc,
                        MaXuong = item.MaXuong,
                        MaLo = item.MaLo,
                        TyLe = item.TyLe,
                        CreateBy = item.CreateBy,
                        CreateDateTime = item.CreateDateTime,
                        ModifyBy = item.ModifyBy,
                        ModifyDateTime = item.ModifyDateTime,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> DoUpDateThanhPhamPhoiTron(DateTime dateTime, string maXuong, string maKhuVuc, string maThanhPhamOrg, string maThanhPhamDes, string maLo, decimal tyLe)
        {
            var userId = HttpContext.Session.GetString("Id");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPham_PhoiTron/Update/{dateTime.ToString("yyyy-MM-dd")}/{maThanhPhamOrg}/{maThanhPhamDes}/{maKhuVuc}/{maXuong}/{maLo}";
            try
            {
                if (string.IsNullOrEmpty(maThanhPhamOrg) || string.IsNullOrEmpty(maThanhPhamDes) || string.IsNullOrEmpty(maKhuVuc) || string.IsNullOrEmpty(maXuong) || string.IsNullOrEmpty(maLo))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Mesages = "Chưa chọn thông tin!."
                    });
                }
                var model = new MaThanhPham_PhoiTron
                {
                    TyLe = tyLe,
                    ModifyBy = userId,
                    ModifyDateTime = DateTime.Now,
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
        public async Task<IActionResult> DoDeleteThanhPhamPhoiTron(DateTime dateTime, string maThanhPhamOrg, string maThanhPhamDes, string maKhuVuc, string maXuong, string maLo)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPham_PhoiTron/Delete/{dateTime.ToString("yyyy-MM-dd")}/{maThanhPhamOrg}/{maThanhPhamDes}/{maKhuVuc}/{maXuong}/{maLo}";
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
        public async Task<IActionResult> ExecuteTPSoftAction(DateTime dateTime, string maXuong)
        {
            //var progress = new Progress<string>(async message =>
            //{
            //    await _hubContext.Clients.All.SendAsync("ReceiveProgress", message);
            //});

            await CallExecuteTPSoftActionApi(dateTime, maXuong);
            return Ok();
        }

        private async Task CallExecuteTPSoftActionApi(DateTime dateTime, string maXuong)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/ExecuteTPSoftAction/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";

                //var model = new ProgressTPSoftcs
                //{
                //    DateTime = dateTime,
                //    MaXuong = maXuong
                //};
                //var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                //var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                var response = await helper.PostAsync(HttpContext, apiUrl, null);
                //if (response.Success)
                //{
                //    progress.Report("Thực hiện xong!");
                //}
                //else
                //{
                //    progress.Report($"Error: {response.Message}");
                //}
            }
            catch (Exception ex)
            {
                //progress.Report("Có lỗi xảy ra: " + ex.ToString());
            }
        }
        public IActionResult GetSignalRConnectionInfo()
        {
            var url = AppViewModels.AppViewModel.Instance.ApiHostUrl+ "/progressHub";
            var token = HttpContext.Session.GetString("JWTToken");

            // Mã hóa URL và Token
            var encryptedUrl = Security.Crypt.ED.EncryptString(url);
            var encryptedToken = Security.Crypt.ED.EncryptString(token);

            return Json(new { url = encryptedUrl, token = encryptedToken });
        }
        public async Task<IActionResult> KetChuyen(DateTime dateTime, string maXuong, string listPhieuCanTongHopDinhHinh)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/KetChuyen/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listPhieuCanTongHopDinhHinh));
                var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                //var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
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
                    Messages = "Đã xảy ra lỗi: " + ex.Message
                });
            }

        }
    }
}
