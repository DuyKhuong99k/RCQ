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
using Models.Repos.A_Model;

namespace PMS.Controllers.TinhLuong.BaoTu
{
    [Authorize]
    public class ChinhBaoTuController : Controller
    {
        private readonly IHubContext<PMS.Hubs.ProgressHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public ChinhBaoTuController(IHubContext<PMS.Hubs.ProgressHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult ChinhBaoTuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ChinhBaoTuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            ViewBag.TitlePage = "Tính Lương Bao Tử";
            return View("~/Views/TinhLuong/BaoTu/ChinhBaoTuView.cshtml");
        }
        public async Task<IEnumerable<object>> LoadTongHopTinhLuongBaoTu(DateTime dateTime, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/LoadTongHopTinhLuongBaoTu/{dateTime.ToString("yyyy-MM-dd")}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<ActionResult> Reload(DateTime dateTime, string xuongId, string typeReload)
        {
            IEnumerable<object> dataSource = null;
            bool success = true;

            try
            {
                switch (typeReload)
                {
                    case "LoadTongHopTinhLuongBaoTu":
                        dataSource = await LoadTongHopTinhLuongBaoTu(dateTime, xuongId);
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
        public async Task<IActionResult> KetChuyen(DateTime dateTime, string maXuong, string listSanLuongPhuFillet)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_PhieuCan/KetChuyen/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(listSanLuongPhuFillet));
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
        #region Nhóm Tính Lương
        [CustomAuthorize(Fu = "Tính Lương / Bao Tử / Chính", Func = "Cài Đặt Nhóm / Tính Lương / Bao Tử / Chính")]
        public IActionResult NhomTinhLuongView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhomTinhLuongView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Cài Đặt Nhóm Tính Lương";
            return View("~/Views/TinhLuong/BaoTu/NhomTinhLuongView.cshtml");
        }
        public async Task<IEnumerable<BT_NhomTinhLuong>> GetAllNhomTinhLuongs()
        {
            IEnumerable<BT_NhomTinhLuong> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhomTinhLuong/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<BT_NhomTinhLuong>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNewNhomTinhLuong()
        {
            try
            {
                var dataSource = await GetAllNhomTinhLuongs();
                if (dataSource != null)
                {

                    var maxId = dataSource.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
                    .Max();
                    var id = (int.Parse(maxId) + 1).ToString("000");
                    return Json(new
                    {
                        isSuccess = true,
                        Ma = id,
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
        public async Task<IActionResult> DoInsertNhomTinhLuong(string ma, string ten, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhomTinhLuong/Insert";
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
                var model = new BT_NhomTinhLuong
                {
                    Ma = ma,
                    Ten = ten,
                    SuDung = suDung,
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
        public async Task<IActionResult> GetsByMaNhomTinhLuong(string ma)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhomTinhLuong/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<BT_NhomTinhLuong>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        SuDung = item?.SuDung,
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
        public async Task<IActionResult> DoUpDateNhomTinhLuong(string ma, string ten, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhomTinhLuong/Update/{ma}";
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
                var model = new BT_NhomTinhLuong
                {
                    Ma = ma,
                    Ten = ten,
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
        public async Task<IActionResult> DoDeleteNhomTinhLuong(string ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhomTinhLuong/Delete/{ma}";
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
        #endregion
        #region Nhân Viên Theo Nhóm
        public IActionResult NhanVienTheoNhomBaoTuView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "NhanVienTheoNhomBaoTuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhomTinhLuong/GetAlls";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<List<BT_NhomTinhLuong>>(HttpContext, apiUrl);
            ViewBag.listNhoms = items.Result.ToList();

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanViens = helperNhanVien.GetAsync<IEnumerable<NhanVienDaiThanh>>(HttpContext, apiNhanVienUrl);
            ViewBag.listNhanViens = nhanViens.Result.ToList();
            ViewBag.TitlePage = "Cài Đặt Nhân Viên Theo Nhóm";
            return View("~/Views/TinhLuong/BaoTu/NhanVienTheoNhomBaoTuView.cshtml");
        }
        public async Task<IEnumerable<BT_NhanVienTheoNhom>> GetAllByMaNhoms(string maNhom, DateTime dateTime, string maXuong)
        {
            IEnumerable<BT_NhanVienTheoNhom> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhanVienTheoNhom/GetAllByMaNhoms/{maNhom}/{dateTime.ToString("yyyy-MM-dd")}/{maXuong}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<BT_NhanVienTheoNhom>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Thêm Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoInsertNhanVienNhom(string selectedIdsString, string maNhom, string maXuong)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhanVienTheoNhom/Insert";
            try
            {
                if (string.IsNullOrEmpty(selectedIdsString) || string.IsNullOrEmpty(maNhom) || string.IsNullOrEmpty(maXuong))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                string[] maNhanViens = selectedIdsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                var nhanVienNhoms = new List<BT_NhanVienTheoNhom>();
                foreach (var item in maNhanViens)
                {

                    var nhanVienNhom = new BT_NhanVienTheoNhom
                    {
                        MaNhanVien = item,
                        MaNhom = maNhom,
                        Ngay = DateTime.Now,
                        TyLeHuong = 1,
                        TyLeTru = 0,
                        SoGio = 8
                    };
                    nhanVienNhoms.Add(nhanVienNhom);
                }

                var successCount = 0;
                var failureCount = 0;

                foreach (var model in nhanVienNhoms)
                {
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
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

        //public async Task<IActionResult> DoUpDateNhanVienNhom(string maNhanVien, string maNhom, DateTime dateTime, decimal tyLeHuong, decimal tyLeTru, decimal soGio)
        //{
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhanVienTheoNhom/Update/{maNhanVien}/{maNhom}/{dateTime.ToString("yyyy-MM-dd")}";
        //    try
        //    {
        //        // Kiểm tra dữ liệu đầu vào
        //        if (string.IsNullOrEmpty(maNhanVien) || string.IsNullOrEmpty(maNhom))
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Messages = "Vui lòng nhập đầy đủ thông tin."
        //            });
        //        }
        //        var model = new BT_NhanVienTheoNhom
        //        {
        //            TyLeHuong = tyLeHuong,
        //            TyLeTru = tyLeTru,
        //            SoGio = soGio
        //        };
        //        var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
        //        var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
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
        //[CustomAuthorize(Fu = "Danh Mục / Quản Lý Phụ Gia / Đơn Vị Tính", Func = "Xóa Quản Lý Phụ Gia / Đơn Vị Tính")]
        public async Task<IActionResult> DoDeleteNhanVienNhom(string selectedIdNhanVienNhoms)
        {
            try
            {
                string[] ids = selectedIdNhanVienNhoms.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (!ids.Any())
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không có phần tử nào được chọn."
                    });
                }

                var maList = ids.Select(id =>
                {
                    var parts = id.Split('-');
                    return new BT_NhanVienTheoNhom
                    {
                        MaNhanVien = parts[0],
                        MaNhom = parts[1],
                        Ngay = DateTime.Parse(parts[2]),
                        MaXuong = parts[3]
                    };
                }).ToList();

                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BT_NhanVienTheoNhom/Delete";
                var jsonContent = new StringContent(JsonConvert.SerializeObject(maList), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (!rl.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = rl.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã xoá các mục thành công!"
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
    }
}
