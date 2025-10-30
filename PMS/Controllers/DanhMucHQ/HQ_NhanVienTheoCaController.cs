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
using System.Transactions;

namespace PMS.Controllers.DanhMucHQ
{
    [Authorize]
    public class HQ_NhanVienTheoCaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HQ_NhanVienTheoCaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên Theo Ca HQ", Func = "Xem Nhân Viên Theo Ca HQ")]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "HQ_NhanVienTheoCa");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            var apiNhanVienUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetAllNhanVienWithDataNeededs";
            using var helperNhanVien = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var nhanviens = helperNhanVien.GetAsync<IEnumerable<object>>(HttpContext, apiNhanVienUrl);
            ViewBag.DataNhanVien = nhanviens.Result.ToList();

            var apiCaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_Cas/GetAlls";
            using var helperCa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var cas = helperCa.GetAsync<IEnumerable<object>>(HttpContext, apiCaUrl);
            ViewBag.Cas = cas.Result.ToList();

            ViewBag.TitlePage = "Nhân Viên Theo Ca";
            return View("~/Views/DanhMucHQ/HQ_NhanVienTheoCa/HQ_NhanVienTheoCaView.cshtml");
        }
        public async Task<IEnumerable<HQ_NhanVienTheoCa>> GetAlls()
        {
            IEnumerable<HQ_NhanVienTheoCa> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoCas/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<HQ_NhanVienTheoCa>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetAllsNewFullField(DateTime dateTime)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoCas/GetAllListNews/{dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}";
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
        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên Theo Ca HQ", Func = "Thêm Nhân Viên Theo Ca HQ")]
        public async Task<IActionResult> DoInsert(string caId, string nhanVienId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoCas/Insert";
            try
            {
                if (string.IsNullOrEmpty(caId) || string.IsNullOrEmpty(nhanVienId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new HQ_NhanVienTheoCa
                {
                    CaId = caId,
                    NhanVienId = nhanVienId,
                    NgayGio = DateTime.Now,
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoCas/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<HQ_NhanVienTheoCa>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        CaId = item.CaId,
                        NhanVienId = item.NhanVienId,
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
        //[CustomAuthorize(Fu = "Danh Mục / Nhân Viên Theo Ca HQ", Func = "Sửa Nhân Viên Theo Ca HQ")]
        //public async Task<IActionResult> DoUpDate(int id, string caId,string nhanVienId, DateTime gioBatDau)
        //{
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoCas/Update/{id}";
        //    try
        //    {
        //        // Kiểm tra dữ liệu đầu vào
        //        if (string.IsNullOrEmpty(ten))
        //        {
        //            return Json(new
        //            {
        //                isSuccess = false,
        //                Messages = "Vui lòng nhập đầy đủ thông tin."
        //            });
        //        }
        //        var model = new HQ_NhanVienTheoCa
        //        {
        //            Ten = ten,
        //            GioBatDau = gioBatDau,
        //            GioKetThuc = gioKetThuc,
        //            IsQuaDem = isQuaDem,
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
        [CustomAuthorize(Fu = "Danh Mục / Nhân Viên Theo Ca HQ", Func = "Xóa Nhân Viên Theo Ca HQ")]
        public async Task<IActionResult> DoDelete(int id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoCas/Delete/{id}";
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

        public async Task<IActionResult> CheckQuyenHQ(string typeOption)
        {
            try
            {
                // Lấy danh sách roleid từ cookie
                var roleIds = PMS.Middlewares.AuthenticationHelpers.GetRoleIdsFromCookie(HttpContext);

                // Lấy danh sách role permissions từ list roleId
                var rolePermistions = await PMS.Middlewares.AuthenticationHelpers.GetRolePermistionsAsync(HttpContext, roleIds);

                // Kiểm tra quyền dựa trên typeOption
                var permissionMapping = new Dictionary<string, string>
                {
                    { "CHUYENNHANVIEN", "Chuyển Ca Nhân Viên Theo Ca HQ" },
                };

                if (permissionMapping.TryGetValue(typeOption, out var func))
                {
                    var permission = rolePermistions
                        .FirstOrDefault(x => x.Fu == "Danh Mục / Nhân Viên Theo Ca HQ" && x.Func == func && x.Status == 1);

                    if (permission != null)
                    {
                        return Json(new
                        {
                            isSuccess = true,
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Bạn không có quyền sử dụng chức năng này!"
                        });
                    }
                }

                return Json(new
                {
                    isSuccess = false,
                    Messages = "Tùy chọn không hợp lệ!"
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

        [CustomAuthorize(Fu = "Xử Lý Phiếu Cân HQ / Phiếu Cân HQ", Func = "Thêm Xử Lý Phiếu Cân HQ / Phiếu Cân HQ")]
        public async Task<IActionResult> DoChuyenCa(string caId,string listNhanVien)
        {
            // Tách chuỗi thành các mảng con ngăn cách bởi dấu |
            string[] nhanVienIds = listNhanVien.Split('|', StringSplitOptions.RemoveEmptyEntries);

            var userName = HttpContext.Session.GetString("Username");

            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                foreach (var nhanVienId in nhanVienIds)
                {
                    var model = new HQ_NhanVienTheoCa
                    {
                        CaId = caId,
                        NhanVienId = nhanVienId
                    };


                    // Chuẩn bị URL cho các API
                    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_NhanVienTheoCas/Insert";
                    // Thực hiện insert PhieuCanTPDinhHinh
                    var dataTuple = new Tuple<string>(JsonConvert.SerializeObject(model));
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(dataTuple), Encoding.UTF8, "application/json");
                    var responseTP = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (!responseTP.Success)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Chuyển Ca thất bại: " + responseTP.Message
                        });

                    }
                }

                // Hoàn thành transaction nếu tất cả các update đều thành công
                transactionScope.Complete();

                return Json(new
                {
                    isSuccess = true,
                    Messages = "Đã Chuyển Ca Thành Công"
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
