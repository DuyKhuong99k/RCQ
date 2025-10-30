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
using static PMS.Controllers.AuthenticationController;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMS.Controllers
{
    public class UtilitiesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UtilitiesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Search(string search, int userId)
        {
            try
            {
                var data = await GetAllQuickSearchMenuByUser(userId);
                var filteredData = data.Where(item => item.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                return Ok(filteredData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult QuickSearchMenu()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "QuickSearchMenu");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "QuickSearchMenu";
            return View("QuickSearchMenu");
        }
        public async Task<IEnumerable<QuickSearchMenu>> GetAllQuickSearchMenuByUser(int userId)
        {
            IEnumerable<QuickSearchMenu> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/QuickSearchMenus/GetAllByUsers/{userId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<QuickSearchMenu>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<QuickSearchMenu>> GetAllQuickSearchMenu()
        {
            IEnumerable<QuickSearchMenu> dataSource = ViewBag.dataSource;
            //if (dataSource == null)
            //{
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/QuickSearchMenus/GetAlls";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<QuickSearchMenu>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            //}
            return dataSource;
        }
        public async Task<IActionResult> CreatDefautNewQuickSearchMenu()
        {
            try
            {
                return Json(new
                {
                    isSuccess = true,
                    SuDung = true,
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ActionResult<int>> GetMaxQuickSearchMenu()
        {
            var data = await GetAllQuickSearchMenu();
            if (data != null && data.Any())
            {
                var maxId = data.Max(x => x.Id);
                var id = maxId + 1;
                return id;
            }
            else
            {
                // Trả về một giá trị mặc định nếu không có dữ liệu hoặc dữ liệu là rỗng
                return 1;
            }
        }
        public async Task<IActionResult> DoInsertQuickSearchMenu(string name, string path, int userId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/QuickSearchMenus/Insert";
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path) || userId <= 0)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin và UserId phải là một số nguyên dương."
                    });
                }
                var data = await GetAllQuickSearchMenuByUser(userId);

                if (data.Any(x => x.Path == path))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Trang này đã được lưu trong tìm kiếm của bạn! vui lòng kiểm tra lại."
                    });
                }
                var idResult = await GetMaxQuickSearchMenu();
                if (idResult != null && idResult.Value != null)
                {
                    int id = idResult.Value;
                    var model = new QuickSearchMenu
                    {
                        Id = id,
                        Name = name,
                        Path = path,
                        UserId = userId,
                        SuDung = true
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
                            isSuccess = true,
                            Messages = response.Message
                        });
                    }
                    else
                    {
                        // Xử lý trường hợp không thành công khi gửi dữ liệu đến API
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = "Gặp sự cố khi thêm dữ liệu: " + response.Message
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không lấy được giá trị Id từ máy chủ!"
                    });
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
        public async Task<IActionResult> GetsByMaQuickSearchMenu(int id)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/QuickSearchMenus/GetsByMa/{id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<QuickSearchMenu>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Name = item.Name,
                        Path = item.Path,
                        UserId = item.UserId,
                        SuDung = item.SuDung,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> DoUpDateQuickSearchMenu(int id, string name, bool suDung)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/QuickSearchMenus/Update/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(id.ToString()) || string.IsNullOrEmpty(name))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new QuickSearchMenu
                {
                    Name = name,
                    SuDung = suDung
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
        public async Task<IActionResult> DoDeleteQuickSearchMenu(int id)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/QuickSearchMenus/Delete/{id}";
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

