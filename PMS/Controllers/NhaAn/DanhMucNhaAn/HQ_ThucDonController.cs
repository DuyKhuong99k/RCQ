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
using static PMS.Controllers.NhaAn.DanhMucNhaAn.HQ_ThucDonController;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;

namespace PMS.Controllers.NhaAn.DanhMucNhaAn
{
    [Authorize]
    public class HQ_ThucDonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HQ_ThucDonController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "ThucDonView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            var apiLoaiMonAnUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_LoaiMonAns/GetAlls";
            using var helperLoaiMonAn = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var loaiMonAns = helperLoaiMonAn.GetAsync<IEnumerable<HQ_MonAn>>(HttpContext, apiLoaiMonAnUrl);
            ViewBag.loaiMonAns = loaiMonAns.Result.ToList();
            ViewBag.TitlePage = "Thực Đơn";
            return View("~/Views/NhaAn/DanhMucNhaAn/HQ_ThucDon/ThucDonView.cshtml");
        }
        public async Task<IEnumerable<HQ_ThucDon>> GetAlls()
        {
            IEnumerable<HQ_ThucDon> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<HQ_ThucDon>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetDanhSachTrangThaiThucDonTungNgay()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetDanhSachTrangThaiThucDonTungNgay";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetDanhSachTrangThaiThucDon()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetDanhSachTrangThaiThucDon";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<int> GetMaxId()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetMaxId";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            int id = await helper.GetAsync<int>(HttpContext, apiUrl);
            return id;
        }
        public async Task<IActionResult> GetNgayThucDonGanNhat()
        {
            int maxId = await GetMaxId();
            var chiTietThucDons = await GetChiTietThucDonListByThucDonId(maxId);
            var thucDon = chiTietThucDons.FirstOrDefault();

            return Json(new
            {
                isSuccess = true,
                NgayThucDon = thucDon?.NgayThucDon
            });
        }

        [CustomAuthorize(Fu = "Nhà Ăn / Thực Đơn", Func = "Thêm Thực Đơn")]
        public async Task<IActionResult> DoInsert(DateTime ngay, string ten, string listMonAnId, string ghiChu)
        {
            try
            {
                var userName = HttpContext.Session.GetString("Username");
                if (ngay == null || string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(listMonAnId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                else
                {
                    try
                    {
                        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                        var thucDonModel = new HQ_ThucDon
                        {
                            Ngay = ngay,
                            NgayTao = DateTime.Now,
                            NguoiTao = userName,
                            ThietBi = AppViewModels.AppViewModel.Instance.PCName,
                            GhiChu = ghiChu,
                            Ten = ten,
                        };
                        var apiThucDonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/Insert";
                        var apiThucDonChiTietUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDonChiTiets/Insert";
                        var dataTupleThucDon = new Tuple<string>(JsonConvert.SerializeObject(thucDonModel));
                        var jsonContentThucDon = new StringContent(JsonConvert.SerializeObject(dataTupleThucDon), Encoding.UTF8, "application/json");
                        using var helperThucDon = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var responseThucDon = await helperThucDon.PostAsync(HttpContext, apiThucDonUrl, jsonContentThucDon);
                        if (responseThucDon.Success)
                        {
                            int id = await GetMaxId();
                            using var helperThucDonChiTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                            string[] parts = listMonAnId.TrimEnd('|').Split('|');
                            foreach (string part in parts)
                            {
                                int idMonAn = int.Parse(part);

                                var model = new HQ_ThucDonChiTiet
                                {
                                    ThucDonId = id,
                                    MonAnId = idMonAn,
                                    GhiChu = ghiChu,
                                };

                                var dataTupleThucDonChiTiet = new Tuple<string>(JsonConvert.SerializeObject(model));
                                var jsonContentThucDonChiTiet = new StringContent(JsonConvert.SerializeObject(dataTupleThucDonChiTiet), Encoding.UTF8, "application/json");
                                var response = await helperThucDonChiTiet.PostAsync(HttpContext, apiThucDonChiTietUrl, jsonContentThucDonChiTiet);
                                if (!response.Success)
                                {
                                    return Json(new
                                    {
                                        isSuccess = false,
                                        Messages = "Thêm món thất bại: " + response.Message
                                    });

                                }

                            }
                        }
                        transactionScope.Complete();
                        return Json(new
                        {
                            isSuccess = true,
                            Messages = "Đã Thêm các các món đã chọn!"
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




        #region Hiển thị danh sách thực đơn theo ngày
        public class ThucDon
        {
            public int ThucDonId { get; set; }
            public DateTime NgayThucDon { get; set; }
            public string TenThucDon { get; set; }
            public string? GhiChu { get; set; }
            public int TrangThaiThucDon { get; set; }
            public List<string> MonAn { get; set; } = new List<string>();
        }

        public class ThucDonRecord
        {
            public int ThucDonId { get; set; }
            public DateTime NgayThucDon { get; set; }
            public DateTime NgayTao { get; set; }
            public string NguoiTao { get; set; }
            public string TenThucDon { get; set; }
            public string? GhiChu { get; set; }
            public string ThietBi { get; set; }
            public int TrangThaiThucDon { get; set; }
        }

        public class ChiTietThucDonRecord
        {
            public int Id { get; set; }
            public int ThucDonId { get; set; }
            public int MonAnId { get; set; }
            public string TenMonAn { get; set; }
            public string TenLoaiMonAn { get; set; }
            public string? GhiChu { get; set; }
            public DateTime NgayThucDon { get; set; }
            public string TenThucDon { get; set; }
        }

        public static List<ThucDon> GetThucDonWithDishes(
        List<ThucDonRecord> thucDons,
        List<ChiTietThucDonRecord> chiTietThucDons)
        {
            var result = thucDons.Select(td => new ThucDon
            {
                ThucDonId = td.ThucDonId,
                NgayThucDon = td.NgayThucDon,
                TenThucDon = td.TenThucDon,
                GhiChu = td.GhiChu,
                TrangThaiThucDon = td.TrangThaiThucDon,
                MonAn = chiTietThucDons
                            .Where(ct => ct.ThucDonId == td.ThucDonId)
                            .Select(ct => ct.TenMonAn)
                            .ToList()
            }).ToList();

            return result;
        }


        public async Task<IActionResult> GetThucDonByDate(DateTime ngay)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetThucDonByDate/{ngay:yyyy-MM-dd}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thucDons = await helper.GetAsync<IEnumerable<ThucDonRecord>>(HttpContext, apiUrl);

            if (thucDons == null || !thucDons.Any())
            {
                return Json(new { isSuccess = true, data = new List<ThucDon>() });
            }

            var thucDonIds = thucDons.Select(td => td.ThucDonId).ToList();
            var apiUrlChiTiet = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetChiTietTheoThucDon";

            var chiTietThucDons = new List<ChiTietThucDonRecord>();

            foreach (var thucDonId in thucDonIds)
            {
                using var helperChiTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var apiUrlChiTietForId = $"{apiUrlChiTiet}/{thucDonId}";
                var chiTiet = await helperChiTiet.GetAsync<IEnumerable<ChiTietThucDonRecord>>(HttpContext, apiUrlChiTietForId);
                if (chiTiet != null)
                {
                    chiTietThucDons.AddRange(chiTiet);
                }
            }

            var result = GetThucDonWithDishes(thucDons.ToList(), chiTietThucDons);

            return Json(new
            {
                isSuccess = true,
                danhSachThucDon = result
            });
        }
        public async Task<IActionResult> GetChiTietTheoThucDon(DateTime ngay)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetThucDonByDate/{ngay:yyyy-MM-dd}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var thucDons = await helper.GetAsync<IEnumerable<ThucDonRecord>>(HttpContext, apiUrl);

            if (thucDons == null || !thucDons.Any())
            {
                return Json(new { isSuccess = true, data = new List<ThucDon>() });
            }

            var thucDonIds = thucDons.Select(td => td.ThucDonId).ToList();
            var apiUrlChiTiet = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetChiTietTheoThucDon";

            var chiTietThucDons = new List<ChiTietThucDonRecord>();

            foreach (var thucDonId in thucDonIds)
            {
                using var helperChiTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var apiUrlChiTietForId = $"{apiUrlChiTiet}/{thucDonId}";
                var chiTiet = await helperChiTiet.GetAsync<IEnumerable<ChiTietThucDonRecord>>(HttpContext, apiUrlChiTietForId);
                if (chiTiet != null)
                {
                    chiTietThucDons.AddRange(chiTiet);
                }
            }

            var result = GetThucDonWithDishes(thucDons.ToList(), chiTietThucDons);

            return Json(new
            {
                isSuccess = true,
                danhSachThucDon = result
            });
        }
        public async Task<List<ChiTietThucDonRecord>> GetChiTietThucDonListByThucDonId(int thucDonId)
        {
            var apiUrlChiTiet = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetChiTietTheoThucDon/{thucDonId}";
            using var helperChiTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var chiTiet = await helperChiTiet.GetAsync<IEnumerable<ChiTietThucDonRecord>>(HttpContext, apiUrlChiTiet);

            return chiTiet?.ToList() ?? new List<ChiTietThucDonRecord>();
        }
        public async Task<IActionResult> GetChiTietThucDonByThucDonId(int thucDonId)
        {
            var chiTietThucDons = await GetChiTietThucDonListByThucDonId(thucDonId);
            var thucDon = chiTietThucDons.FirstOrDefault();

            return Json(new
            {
                isSuccess = true,
                ThucDonId = thucDon?.ThucDonId ?? 0,
                NgayThucDon = thucDon?.NgayThucDon,
                TenThucDon = thucDon?.TenThucDon,
                GhiChu = thucDon?.GhiChu,
                thucDonChiTiet = chiTietThucDons
            });
        }
        #endregion

        [CustomAuthorize(Fu = "Nhà Ăn / Thực Đơn", Func = "Thêm Thực Đơn")]
        public async Task<IActionResult> DoUpDate(int thucDonId, string ten, string listMonAnId, string listIdThucDonChiTietRs, string ghiChu)
        {
            try
            {
                if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(listMonAnId) || string.IsNullOrEmpty(listIdThucDonChiTietRs))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                else
                {
                    try
                    {
                        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                        var apiDeleteThucDonChiTietUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDonChiTiets/DeleteThucDonChiTietByListId/{listIdThucDonChiTietRs}";
                        using var helperDeleteThucDonChiTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var responseDeleteThucDonChiTiet = await helperDeleteThucDonChiTiet.PostAsync(HttpContext, apiDeleteThucDonChiTietUrl, null);

                        var apiThucDonChiTietUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDonChiTiets/Insert";
                        if (responseDeleteThucDonChiTiet.Success) // xóa thành công toàn bộ danh sách cũ
                        {
                            using var helperThucDonChiTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                            string[] parts = listMonAnId.TrimEnd('|').Split('|');
                            foreach (string part in parts)
                            {
                                int idMonAn = int.Parse(part);
                                var model = new HQ_ThucDonChiTiet
                                {
                                    ThucDonId = thucDonId,
                                    MonAnId = idMonAn,
                                    GhiChu = ghiChu,
                                };

                                var dataTupleThucDonChiTiet = new Tuple<string>(JsonConvert.SerializeObject(model));
                                var jsonContentThucDonChiTiet = new StringContent(JsonConvert.SerializeObject(dataTupleThucDonChiTiet), Encoding.UTF8, "application/json");
                                var response = await helperThucDonChiTiet.PostAsync(HttpContext, apiThucDonChiTietUrl, jsonContentThucDonChiTiet);
                                if (!response.Success)
                                {
                                    return Json(new
                                    {
                                        isSuccess = false,
                                        Messages = "Sửa món thất bại: " + response.Message
                                    });

                                }

                            }
                        }
                        transactionScope.Complete();
                        return Json(new
                        {
                            isSuccess = true,
                            Messages = $@"Đã sửa thực đơn {thucDonId}!"
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
        [CustomAuthorize(Fu = "Nhà Ăn / Thực Đơn", Func = "Thêm Thực Đơn")]
        public async Task<IActionResult> DoDelete(int thucDonId, string listIdThucDonChiTietRs)
        {
            try
            {
                if (thucDonId <= 0 || thucDonId == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                if (string.IsNullOrEmpty(listIdThucDonChiTietRs) || listIdThucDonChiTietRs == "|")
                {
                    var apiDeleteThucDonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/Delete/{thucDonId}";
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var rl = await helper.PostAsync(HttpContext, apiDeleteThucDonUrl, null);
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = $@"Đã xóa thực đơn {thucDonId}!"
                    });
                }
                else
                {
                    try
                    {
                        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                        var apiDeleteThucDonChiTietUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDonChiTiets/DeleteThucDonChiTietByListId/{listIdThucDonChiTietRs}";
                        using var helperDeleteThucDonChiTiet = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var responseDeleteThucDonChiTiet = await helperDeleteThucDonChiTiet.PostAsync(HttpContext, apiDeleteThucDonChiTietUrl, null);

                        var apiDeleteThucDonUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/Delete/{thucDonId}";
                        if (responseDeleteThucDonChiTiet.Success) // xóa thành công toàn bộ danh sách cũ
                        {
                            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                            var rl = await helper.PostAsync(HttpContext, apiDeleteThucDonUrl, null);
                            if (!rl.Success)
                            {
                                return Json(new
                                {
                                    isSuccess = false,
                                    Messages = "Sửa món thất bại: " + rl.Message
                                });

                            }

                        }
                        else
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = "Xóa món thất bại"
                            });
                        }

                        transactionScope.Complete();
                        return Json(new
                        {
                            isSuccess = true,
                            Messages = $@"Đã xóa thực đơn {thucDonId}!"
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


        public async Task<IActionResult> AutoApproveMenu()
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    Messages = $@"Đã tự động phê duyệt thực đơn gần nhất!"
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
        public async Task<IEnumerable<ThucDonRecord>> GetDanhSachThucDonTrongNgay(DateTime ngay)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_ThucDons/GetThucDonByDate/{ngay:yyyy-MM-dd}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = await helper.GetAsync<IEnumerable<ThucDonRecord>>(HttpContext, apiUrl);
            return items.ToList();
        }


        public async Task<IActionResult> AutoApproveThucDon()
        {
            try
            {
                var date = DateTime.Now.Date;
                var datePrev = date.AddDays(-1);
                var items = await GetDanhSachThucDonTrongNgay(datePrev);

                if (items == null || !items.Any())
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = $@"{datePrev:dd/MM/yyyy} không có thực đơn nào!"
                    });
                }

                if (items.All(x => x.TrangThaiThucDon != 1))
                {
                    var thucDonId = items.OrderByDescending(x => x.NgayThucDon).FirstOrDefault();
                    RedirectToAction("DoInsert", "HQ_DuyetThucDon");


                    return Json(new
                    {
                        isSuccess = true,
                        ThucDonId = thucDonId?.ThucDonId,
                        Messages = "Tìm duyệt thực đơn gần nhất"
                    });
                }

                else
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Messages = "Đã có thực đơn đc duyệt, hủy các thực đơn còn lại"
                    });
                }

               
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
