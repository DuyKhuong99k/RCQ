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
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace PMS.Controllers.TinhLuong
{
    [Authorize]
    public class KetNoiBravoController : Controller
    {
        private readonly IHubContext<PMS.Hubs.ProgressHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public KetNoiBravoController(IHubContext<PMS.Hubs.ProgressHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult KetNoiBravoView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "KetNoiBravoView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }

            ViewBag.titile = "Kết Nối Bravo";

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanSanXuat/GetCongDoans";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<Dictionary<string, string>>(HttpContext, apiUrl);
            ViewBag.ListCongDoan = items.Result.ToList();
            return View("~/Views/TinhLuong/KetNoiBravoView.cshtml");
        }

        #region Phiếu Cân Sản Xuất
        public async Task<IActionResult> CheckItemServerBVPhieuCanSanXuat(DateTime ngay, string congDoanId)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanSanXuat/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}/{congDoanId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
        public async Task<IActionResult> DeleteItemServerBVPhieuCanSanXuat(DateTime ngay, string congDoanId)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanSanXuat/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}/{congDoanId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> ConfirmItemCountUploadBVPhieuCanSanXuat(DateTime ngay,string congDoanId)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanSanXuat/ConfirmItemCountUpload/{ngay.ToString("yyyy-MM-dd")}/{congDoanId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> UploadSanXuat(DateTime dateTime, string congDoanId)
        {
            try
            {
                if (dateTime == default || string.IsNullOrEmpty(congDoanId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin Ngày và Công Đoạn."
                    });
                }

                string baseApiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanSanXuat";
                string ngay = dateTime.ToString("yyyy-MM-dd");

                Dictionary<string, string> apiUrls = new Dictionary<string, string>
                {
                    { "TN", $"{baseApiUrl}/Upload_TN/{ngay}/{congDoanId}" },
                    { "FL", $"{baseApiUrl}/Upload_FL/{ngay}/{congDoanId}" },
                    { "DH", $"{baseApiUrl}/Upload_DH/{ngay}/{congDoanId}" },
                    { "XK", $"{baseApiUrl}/Upload_XK/{ngay}/{congDoanId}" },
                    { "CXBN", $"{baseApiUrl}/Upload_PP/{ngay}/{congDoanId}" }
                };

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                if (!apiUrls.TryGetValue(congDoanId, out string apiUrl))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Công đoạn không hợp lệ."
                    });
                }

                var response = await helper.PostAsync(HttpContext, apiUrl, null);

                if (!response.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = response.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
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
        #endregion
        #region Cá Giống
        public async Task<IActionResult> CheckItemServerBVPhieuCanCaGiong(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaGiong/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
        public async Task<IActionResult> DeleteItemServerBVPhieuCanCaGiong(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaGiong/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> ConfirmItemCountUploadBVPhieuCanCaGiong(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaGiong/ConfirmItemCountUpload/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> UploadCaGiong(DateTime dateTime)
        {
            try
            {
                if (dateTime == default)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin Ngày."
                    });
                }


                string ngay = dateTime.ToString("yyyy-MM-dd");
                string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaGiong/Upload/{ngay}";
                //Dictionary<string, string> apiUrls = new Dictionary<string, string>
                //{
                //    { "TN", $"{baseApiUrl}/Upload_TN/{ngay}/{congDoanId}" },
                //    { "FL", $"{baseApiUrl}/Upload_FL/{ngay}/{congDoanId}" },
                //    { "DH", $"{baseApiUrl}/Upload_DH/{ngay}/{congDoanId}" },
                //    { "XK", $"{baseApiUrl}/Upload_XK/{ngay}/{congDoanId}" },
                //    { "CXBN", $"{baseApiUrl}/Upload_PP/{ngay}/{congDoanId}" }
                //};

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                //if (!apiUrls.TryGetValue(congDoanId, out string apiUrl))
                //{
                //    return Json(new
                //    {
                //        isSuccess = false,
                //        Messages = "Công đoạn không hợp lệ."
                //    });
                //}

                var response = await helper.PostAsync(HttpContext, apiUrl, null);

                if (!response.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = response.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
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
        #endregion
        #region Cá Thịt
        public async Task<IActionResult> CheckItemServerBVPhieuCanCaThit(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaThit/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
        public async Task<IActionResult> DeleteItemServerBVPhieuCanCaThit(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaThit/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> ConfirmItemCountUploadBVPhieuCanCaThit(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaThit/ConfirmItemCountUpload/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> UploadCaThit(DateTime dateTime)
        {
            try
            {
                if (dateTime == default)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin Ngày."
                    });
                }


                string ngay = dateTime.ToString("yyyy-MM-dd");
                string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaThit/Upload/{ngay}";
                //Dictionary<string, string> apiUrls = new Dictionary<string, string>
                //{
                //    { "TN", $"{baseApiUrl}/Upload_TN/{ngay}/{congDoanId}" },
                //    { "FL", $"{baseApiUrl}/Upload_FL/{ngay}/{congDoanId}" },
                //    { "DH", $"{baseApiUrl}/Upload_DH/{ngay}/{congDoanId}" },
                //    { "XK", $"{baseApiUrl}/Upload_XK/{ngay}/{congDoanId}" },
                //    { "CXBN", $"{baseApiUrl}/Upload_PP/{ngay}/{congDoanId}" }
                //};

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                //if (!apiUrls.TryGetValue(congDoanId, out string apiUrl))
                //{
                //    return Json(new
                //    {
                //        isSuccess = false,
                //        Messages = "Công đoạn không hợp lệ."
                //    });
                //}

                var response = await helper.PostAsync(HttpContext, apiUrl, null);

                if (!response.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = response.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
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
        #endregion
        #region Cá Ngộp
        public async Task<IActionResult> CheckItemServerBVPhieuCanCaNgop(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaNgop/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
        public async Task<IActionResult> DeleteItemServerBVPhieuCanCaNgop(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaNgop/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> ConfirmItemCountUploadBVPhieuCanCaNgop(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaNgop/ConfirmItemCountUpload/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> UploadCaNgop(DateTime dateTime)
        {
            try
            {
                if (dateTime == default)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin Ngày."
                    });
                }


                string ngay = dateTime.ToString("yyyy-MM-dd");
                string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanCaNgop/Upload/{ngay}";
                //Dictionary<string, string> apiUrls = new Dictionary<string, string>
                //{
                //    { "TN", $"{baseApiUrl}/Upload_TN/{ngay}/{congDoanId}" },
                //    { "FL", $"{baseApiUrl}/Upload_FL/{ngay}/{congDoanId}" },
                //    { "DH", $"{baseApiUrl}/Upload_DH/{ngay}/{congDoanId}" },
                //    { "XK", $"{baseApiUrl}/Upload_XK/{ngay}/{congDoanId}" },
                //    { "CXBN", $"{baseApiUrl}/Upload_PP/{ngay}/{congDoanId}" }
                //};

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                //if (!apiUrls.TryGetValue(congDoanId, out string apiUrl))
                //{
                //    return Json(new
                //    {
                //        isSuccess = false,
                //        Messages = "Công đoạn không hợp lệ."
                //    });
                //}

                var response = await helper.PostAsync(HttpContext, apiUrl, null);

                if (!response.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = response.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
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
        #endregion
        #region Bột Cá
        public async Task<IActionResult> CheckItemServerBVPhieuCanBotCa(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanBotCa/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
        public async Task<IActionResult> DeleteItemServerBVPhieuCanBotCa(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanBotCa/CheckItemServer/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> ConfirmItemCountUploadBVPhieuCanBotCa(DateTime ngay)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanBotCa/ConfirmItemCountUpload/{ngay.ToString("yyyy-MM-dd")}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var rl = await helper.PostAsync(HttpContext, apiUrl, null);
                if (!rl.Success)
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
                    Messages = rl.Message,
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
        public async Task<IActionResult> UploadBotCa(DateTime dateTime)
        {
            try
            {
                if (dateTime == default)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin Ngày."
                    });
                }


                string ngay = dateTime.ToString("yyyy-MM-dd");
                string apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BV_PhieuCanBotCa/Upload/{ngay}";
                //Dictionary<string, string> apiUrls = new Dictionary<string, string>
                //{
                //    { "TN", $"{baseApiUrl}/Upload_TN/{ngay}/{congDoanId}" },
                //    { "FL", $"{baseApiUrl}/Upload_FL/{ngay}/{congDoanId}" },
                //    { "DH", $"{baseApiUrl}/Upload_DH/{ngay}/{congDoanId}" },
                //    { "XK", $"{baseApiUrl}/Upload_XK/{ngay}/{congDoanId}" },
                //    { "CXBN", $"{baseApiUrl}/Upload_PP/{ngay}/{congDoanId}" }
                //};

                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                //if (!apiUrls.TryGetValue(congDoanId, out string apiUrl))
                //{
                //    return Json(new
                //    {
                //        isSuccess = false,
                //        Messages = "Công đoạn không hợp lệ."
                //    });
                //}

                var response = await helper.PostAsync(HttpContext, apiUrl, null);

                if (!response.Success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = response.Message
                    });
                }

                return Json(new
                {
                    isSuccess = true,
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
        #endregion
    }
}
