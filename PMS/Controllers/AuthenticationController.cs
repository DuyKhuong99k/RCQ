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
using Models;
using Azure.Core;
using Models.Repos.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Notifications;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using PMS.Attrs;
using Syncfusion.EJ2.Layouts;

namespace PMS.Controllers
{
    [Authorize]
    public class AuthenticationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private bool RememberMe = false;
        private readonly double Hour = 168;
        public AuthenticationController(IHttpClientFactory httpClientFactory, IWebHostEnvironment hostingEnvironment)
        {
            _httpClientFactory = httpClientFactory;
            _hostingEnvironment = hostingEnvironment;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuongNoCheckAuth";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiUrl);
            try
            {
                ViewBag.listXuongs = items.Result.ToList();
                ViewBag.TitlePage = "Login";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }

            return View("~/Views/Authentication/LoginView.cshtml");
        }

        [AllowAnonymous]
        public async Task<IActionResult> DoLogin(string username, string password, string xuongId, bool rememberme, string returnUrl)
        {
            RememberMe = rememberme;
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/Validate";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            //var items = helper.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiUrl);
            UserInfo user = new UserInfo();
            user.UserName = username;
            user.Password = password;
            user.RememberMe = rememberme;
            user.Hour = Hour;
            var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);
            HttpContext.Session.SetString("XuongId", xuongId);
            AppViewModels.AppViewModel.Instance.XuongId = xuongId;
            if (response.Success)
            {

                var apiResponse = response;
                if (apiResponse.Success)
                {
                    // Lưu token vào session hoặc cookie để sử dụng cho các yêu cầu sau này
                    var tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(apiResponse.Data.ToString());
                    HttpContext.Session.SetString("JWTToken", tokenData.AccessToken);
                    HttpContext.Session.SetString("RefreshJWTToken", tokenData.RefreshToken);
                    HttpContext.Session.SetString("ExpiresJWTToken", tokenData.Expires.ToString());
                    HttpContext.Session.SetString("UserName", username);

                    var apiUrl3 = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/SettingWebApp/Get";
                    using var helper3 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var numberDate = await helper3.GetAsync<SettingDashboard>(HttpContext, apiUrl3);
                    HttpContext.Session.SetString("NumberDate", numberDate.NumberDate.ToString());

                    // Đăng nhập người dùng bằng cookie
                    var claims = new List<Claim>
                    {
                         new Claim(ClaimTypes.Name, username),
                         new Claim("jwt",Security.Crypt.ED.EncryptString(tokenData.AccessToken, AppViewModels.AppViewModel.Instance.DefaultKey)),
                         new Claim("RefreshJWT", tokenData.RefreshToken),
                         new Claim("ExpiresJWT", tokenData.Expires.ToString()),
                         new Claim("XuongId", xuongId),
                         new Claim("NumberDate", numberDate.NumberDate.ToString())
                            // Thêm các claims khác nếu cần
                    };
                    var apiListRoles = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/GetRolesFull/{username}";
                    using var helperListRoles = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var listRoles = await helperListRoles.GetAsync<List<Role>>(HttpContext, apiListRoles);
                    var roles = listRoles.ToList();
                    foreach (var role in roles)
                    {
                        if (role?.Id > 0 && !string.IsNullOrWhiteSpace(role.Name))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Name));
                            claims.Add(new Claim("RoleId", role.Id.ToString()));
                        }
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Lưu timeout vào session để đồng bộ
                    HttpContext.Session.SetInt32("SessionTimeout", rememberme ? (int)Math.Round(Hour) : 1);
                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(rememberme ? Hour : 1),
                        IsPersistent = rememberme,
                        AllowRefresh = true
                    };

                    //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    //var authProperties = new AuthenticationProperties
                    //{
                    //    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(rememberme ? Hour : 1)),//DateTime.UtcNow.AddMinutes(rememberme ? Hour * 60 : 60), // Thời hạn là 7 ngày nếu rememberMe=true, ngược lại là 1 tiếng
                    //    IsPersistent = true
                    //};

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    var url = "/Home/Index";
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        url = returnUrl;
                    }


                    // Tiến hành đăng nhập thành công
                    //return Json(new
                    //{
                    //    isSuccess = true,
                    //    url = url
                    //    // Điều hướng đến trang sau khi đăng nhập thành công
                    //});
                    return RedirectToAction("LoginBridge", new { url = url, isUrl = true });
                }
                else
                {
                    // Xử lý trường hợp xác thực thất bại
                    //ViewBag.ErrorMessage = apiResponse.Message;
                    return Json(new
                    {
                        isSuccess = apiResponse.Success,
                        Messages = apiResponse.Message
                    }); // 
                }
            }
            else
            {
                // Xử lý lỗi khi gọi API
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi gọi API xác thực.";
                return Json(new
                {
                    isSuccess = response.Success,
                    Messages = response.Message
                });
            }
        }
        [Authorize]
        [Route("Home/Index")]
        public async Task<IActionResult> LoginBridge(string url, bool isUrl = false)
        {
            await CreateSession();
            if (isUrl)
            {
                return Json(new
                {
                    isSuccess = true,
                    url = url // Điều hướng đến trang sau khi đăng nhập thành công
                });
            }
            else
             if (User.Identity.IsAuthenticated)
            {
                try
                {
                    //lấy danh sách roleid từ cookie
                    var roleIds = PMS.Middlewares.AuthenticationHelpers.GetRoleIdsFromCookie(HttpContext);
                    // lấy danh sách fu func từ list roleId
                    var rolePermistions = await PMS.Middlewares.AuthenticationHelpers.GetRolePermistionsAsync(HttpContext, roleIds);
                    //return View(url);
                    var dashboard = rolePermistions.FirstOrDefault(x => x.Fu == "Dashboard" && x.Status == 1);
                    var dashboardHQ = rolePermistions.FirstOrDefault(x => x.Fu == "DashboardHQ" && x.Status == 1);
                    if (dashboard != null)
                    {
                        ViewBag.TitlePage = "Trang Chủ";
                        return View("~/Views/Home/Index.cshtml");
                    }
                    else if (dashboardHQ != null)
                    {
                        return RedirectToAction("Index", "DashboardHQ", new { url = "DashboardHQ/Index" });

                    }
                    else
                    {
                        ViewBag.TitlePage = "Trang Chủ";
                        return View("~/Views/Home/IndexPMS.cshtml");
                    }
                }
                catch (Exception)
                {

                    return View("~/Views/Authentication/LoginView.cshtml");
                }


            }
            else
            {
                return View("~/Views/Authentication/LoginView.cshtml");
            }

        }
        public async Task<IActionResult> CreateSession()
        {
            // Đảm bảo user đã đăng nhập
            if (User.Identity.IsAuthenticated)
            {
                var accessToken = Security.Crypt.ED.DecryptString(User.FindFirst("jwt")?.Value, AppViewModels.AppViewModel.Instance.DefaultKey);
                var refreshJWT = User.FindFirst("RefreshJWT")?.Value;
                var expiresJWT = User.FindFirst("ExpiresJWT")?.Value;
                var xuongId = User.FindFirst("XuongId")?.Value; ;
                var numberDate = User.FindFirst("NumberDate")?.Value;

                ////lấy danh sách Claims từ cookie
                //var userClaims = HttpContext.User.Claims;
                //// Lọc ra các Claims có loại là "RoleId"
                //var roleClaims = userClaims.Where(c => c.Type == "RoleId").ToList();
                //// Tạo danh sách RoleId từ các Claims đã lọc
                //List<int> roleIds = roleClaims.Select(c => int.Parse(c.Value)).ToList();

                var xuongIdSession = HttpContext.Session.GetString("XuongId");
                if (numberDate == null || numberDate.Trim() == "")
                {
                    numberDate = "0";
                }
                if (!int.TryParse(numberDate, out int num))
                {
                    num = 0;
                }
                HttpContext.Session.SetString("NumberDate", num.ToString());
                // var numberDateession = HttpContext.Session.GetString("NumberDate");
                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshJWT) && !string.IsNullOrEmpty(expiresJWT) && !string.IsNullOrEmpty(xuongId))
                {
                    HttpContext.Session.SetString("JWTToken", accessToken);
                    HttpContext.Session.SetString("JWTTokenEncry", Security.Crypt.ED.EncryptString(accessToken));
                    HttpContext.Session.SetString("RefreshJWTToken", refreshJWT);
                    HttpContext.Session.SetString("ExpiresJWTToken", expiresJWT);
                    //var apiUrl3 = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/SettingWebApp/Get";
                    // using var helper3 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    //var numberDate2 = await helper3.GetAsync<SettingDashboard>(HttpContext, apiUrl3);
                    //HttpContext.Session.SetString("NumberDate", numberDate?.NumberDate.ToString());
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(accessToken);
                    //var sessionTimeout = HttpContext.Session.GetInt32("SessionTimeout") ?? 0;


                    //var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                    var roles = token.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                    .Select(c => c.Value)
                    .ToList();
                    if (roles.Any())
                    {
                        HttpContext.Session.SetString("Roles", string.Join(",", roles));
                    }

                    //if (sessionTimeout == 0)
                    //{
                    //    // Nếu session timeout, tái tạo claims từ JWT
                    //    var claims = new List<Claim>
                    //    {
                    //        new Claim(ClaimTypes.Name, User.Identity.Name),
                    //        new Claim("jwt", User.FindFirst("jwt")?.Value ?? string.Empty),
                    //        new Claim("RefreshJWT", refreshJWT ?? string.Empty),
                    //        new Claim("ExpiresJWT", expiresJWT ?? string.Empty),
                    //        new Claim("XuongId", xuongId ?? string.Empty)
                    //    };

                    //    foreach (var role in roles)
                    //    {
                    //        claims.Add(new Claim(ClaimTypes.Role, role));
                    //    }

                    //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //    var authProperties = new AuthenticationProperties
                    //    {
                    //        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(sessionTimeout),
                    //        IsPersistent = true
                    //    };

                    //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    //}

                    HttpContext.Session.SetString("ExpiresSec", token.Claims.FirstOrDefault(claim => claim.Type.ToUpper() == "ExpiresSec".ToUpper())?.Value);
                    HttpContext.Session.SetString("Id", token.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value);
                    HttpContext.Session.SetString("Username", token.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value);
                    HttpContext.Session.SetString("Roles", token.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value);


                    if (xuongIdSession == null)
                    {
                        HttpContext.Session.SetString("XuongId", xuongId);
                    }

                    var apiUrl2 = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetXuongById/{xuongId}";
                    using var helper2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var item = await helper2.GetAsync<XiNghiep>(HttpContext, apiUrl2);
                    if (item == null)
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


                        // Xóa cookie "JWTToken"
                        HttpContext.Response.Cookies.Delete("JWTToken", new CookieOptions
                        {
                            Path = "/",         // Đảm bảo đặt cùng path đã sử dụng khi thiết lập cookie
                            Secure = true,      // Đặt cùng secure flag như khi thiết lập cookie
                            SameSite = SameSiteMode.None // Đặt cùng SameSite mode như khi thiết lập cookie
                        });
                        //gọi hàm remove tất cả session

                        RemoveSession();
                        var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuongNoCheckAuth";
                        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var items = helper.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiUrl);
                        try
                        {
                            ViewBag.listXuongs = items.Result.ToList();
                            ViewBag.TitlePage = "Login";
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            //throw;
                        }



                    }
                    else
                    {
                        HttpContext.Session.SetString("XuongName", item?.Ten?.ToString());
                        //lấy danh sách roleid từ cookie


                    }


                }
            }
            return View("~/Views/Authentication/LoginView.cshtml");
        }
        /////// bản cập nhật sủa lỗi phần không hoạt động 1 khoản thời gian thì mất cookie
        //[AllowAnonymous]
        //public async Task<IActionResult> DoLogin(string username, string password, string xuongId, bool rememberme, string returnUrl)
        //{
        //    RememberMe = rememberme;
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/Validate";
        //    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //    UserInfo user = new UserInfo
        //    {
        //        UserName = username,
        //        Password = password,
        //        RememberMe = rememberme,
        //        Hour = Hour
        //    };
        //    var jsonContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
        //    var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

        //    HttpContext.Session.SetString("XuongId", xuongId);
        //    AppViewModels.AppViewModel.Instance.XuongId = xuongId;

        //    if (response.Success)
        //    {
        //        var tokenData = JsonConvert.DeserializeObject<TokenModel>(response.Data.ToString());
        //        if (tokenData != null)
        //        {
        //            HttpContext.Session.SetString("JWTToken", tokenData.AccessToken);
        //            HttpContext.Session.SetString("RefreshJWTToken", tokenData.RefreshToken);
        //            HttpContext.Session.SetString("ExpiresJWTToken", tokenData.Expires.ToString());
        //            HttpContext.Session.SetString("UserName", username);

        //            var claims = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.Name, username),
        //                new Claim("jwt", Security.Crypt.ED.EncryptString(tokenData.AccessToken, AppViewModels.AppViewModel.Instance.DefaultKey)),
        //                new Claim("RefreshJWT", tokenData.RefreshToken),
        //                new Claim("ExpiresJWT", tokenData.Expires.ToString()),
        //                new Claim("XuongId", xuongId)
        //            };

        //            var apiListRoles = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/GetRolesFull/{username}";
        //            using var helperListRoles = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //            var listRoles = await helperListRoles.GetAsync<List<Role>>(HttpContext, apiListRoles);
        //            var roles = listRoles.ToList();
        //            foreach (var role in roles)
        //            {
        //                if (role?.Id > 0 && !string.IsNullOrWhiteSpace(role.Name))
        //                {
        //                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
        //                    claims.Add(new Claim("RoleId", role.Id.ToString()));
        //                }
        //            }

        //            //var apiListRoles = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/GetRolesFull/{username}";
        //            //var roles = await helper.GetAsync<List<Role>>(HttpContext, apiListRoles);

        //            //if (roles != null)
        //            //{
        //            //    foreach (var role in roles)
        //            //    {
        //            //        claims.Add(new Claim(ClaimTypes.Role, role.Name));
        //            //        claims.Add(new Claim("RoleId", role.Id.ToString()));
        //            //    }
        //            //}

        //            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //            // Lưu timeout vào session để đồng bộ
        //            HttpContext.Session.SetInt32("SessionTimeout", rememberme ? (int)Math.Round(Hour) : 1);
        //            var authProperties = new AuthenticationProperties
        //            {
        //                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(rememberme ? Hour : 1),
        //                IsPersistent = rememberme,
        //                AllowRefresh = true
        //            };

        //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        //            return RedirectToAction("LoginBridge", new { url = returnUrl ?? "/Home/Index", isUrl = true });
        //        }
        //    }

        //    return Json(new { isSuccess = false, Messages = response.Message });
        //}

        //[Authorize]
        //[Route("Home/Index")]
        //public async Task<IActionResult> LoginBridge(string url, bool isUrl = false)
        //{
        //    await CreateSession();
        //    if (isUrl)
        //    {
        //        return Json(new { isSuccess = true, url = url });
        //    }

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var rolePermistions = await PMS.Middlewares.AuthenticationHelpers.GetRolePermistionsAsync(HttpContext, PMS.Middlewares.AuthenticationHelpers.GetRoleIdsFromCookie(HttpContext));
        //        var dashboard = rolePermistions.FirstOrDefault(x => x.Fu == "Dashboard" && x.Status == 1);
        //        var dashboardHQ = rolePermistions.FirstOrDefault(x => x.Fu == "DashboardHQ" && x.Status == 1);

        //        if (dashboard != null)
        //        {
        //            return View("~/Views/Home/Index.cshtml");
        //        }
        //        else if (dashboardHQ != null)
        //        {
        //            return RedirectToAction("Index", "DashboardHQ");
        //        }

        //        return View("~/Views/Home/IndexPMS.cshtml");
        //    }

        //    return View("~/Views/Authentication/LoginView.cshtml");
        //}

        //public async Task<IActionResult> CreateSession()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var accessToken = Security.Crypt.ED.DecryptString(User.FindFirst("jwt")?.Value, AppViewModels.AppViewModel.Instance.DefaultKey);
        //        var refreshJWT = User.FindFirst("RefreshJWT")?.Value;
        //        var expiresJWT = User.FindFirst("ExpiresJWT")?.Value;
        //        var xuongId = User.FindFirst("XuongId")?.Value;

        //        if (!string.IsNullOrEmpty(accessToken))
        //        {
        //            HttpContext.Session.SetString("JWTToken", accessToken);
        //            HttpContext.Session.SetString("RefreshJWTToken", refreshJWT);
        //            HttpContext.Session.SetString("ExpiresJWTToken", expiresJWT);
        //            HttpContext.Session.SetString("XuongId", xuongId);

        //            // Kiểm tra session timeout và tái tạo claims từ JWT nếu cần
        //            var sessionTimeout = HttpContext.Session.GetInt32("SessionTimeout") ?? 0;
        //            var tokenHandler = new JwtSecurityTokenHandler();
        //            var token = tokenHandler.ReadJwtToken(accessToken);

        //            var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        //            if (roles.Any())
        //            {
        //                HttpContext.Session.SetString("Roles", string.Join(",", roles));
        //            }

        //            if (sessionTimeout == 0)
        //            {
        //                // Nếu session timeout, tái tạo claims từ JWT
        //                var claims = new List<Claim>
        //                {
        //                    new Claim(ClaimTypes.Name, User.Identity.Name),
        //                    new Claim("jwt", User.FindFirst("jwt")?.Value ?? string.Empty),
        //                    new Claim("RefreshJWT", refreshJWT ?? string.Empty),
        //                    new Claim("ExpiresJWT", expiresJWT ?? string.Empty),
        //                    new Claim("XuongId", xuongId ?? string.Empty)
        //                };

        //                foreach (var role in roles)
        //                {
        //                    claims.Add(new Claim(ClaimTypes.Role, role));
        //                }

        //                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //                var authProperties = new AuthenticationProperties
        //                {
        //                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(sessionTimeout),
        //                    IsPersistent = true
        //                };

        //                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        //            }
        //        }
        //    }
        //    return View("~/Views/Authentication/LoginView.cshtml");

        //    //if (User.Identity.IsAuthenticated)
        //    //{
        //    //    var accessToken = Security.Crypt.ED.DecryptString(User.FindFirst("jwt")?.Value, AppViewModels.AppViewModel.Instance.DefaultKey);
        //    //    if (!string.IsNullOrEmpty(accessToken))
        //    //    {
        //    //        var handler = new JwtSecurityTokenHandler();
        //    //        var token = handler.ReadJwtToken(accessToken);

        //    //        HttpContext.Session.SetString("JWTToken", accessToken);
        //    //        HttpContext.Session.SetString("RefreshJWTToken", User.FindFirst("RefreshJWT")?.Value);
        //    //        HttpContext.Session.SetString("ExpiresJWTToken", User.FindFirst("ExpiresJWT")?.Value);
        //    //        HttpContext.Session.SetString("XuongId", User.FindFirst("XuongId")?.Value);

        //    //        var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        //    //        HttpContext.Session.SetString("Roles", string.Join(",", roles));
        //    //    }
        //    //}
        //    //return View("~/Views/Authentication/LoginView.cshtml");
        //}

        //private void RemoveSession()
        //{
        //    HttpContext.Session.Clear();
        //}


        public async Task<IActionResult> Logout()
        {

            var token = HttpContext.Session.GetString("JWTToken"); // Lấy token từ session
            if (!string.IsNullOrEmpty(token))
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/Logout";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

                var response = await helper.PostAsync(HttpContext, apiUrl, null);

                if (response.Success)
                {
                    // Đăng xuất người dùng
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


                    // Xóa cookie "JWTToken"
                    HttpContext.Response.Cookies.Delete("JWTToken", new CookieOptions
                    {
                        Path = "/",         // Đảm bảo đặt cùng path đã sử dụng khi thiết lập cookie
                        Secure = true,      // Đặt cùng secure flag như khi thiết lập cookie
                        SameSite = SameSiteMode.None // Đặt cùng SameSite mode như khi thiết lập cookie
                    });
                    //gọi hàm remove tất cả session

                    RemoveSession();
                    return Json(new
                    {
                        Success = response.Success,
                        Messages = response.Message,
                        url = "/Authentication/Login"
                    });
                }
                else
                {
                    // Xử lý lỗi khi gọi API logout
                    return Json(new
                    {
                        Success = response.Success,
                        Messages = response.Message,
                        url = "/Authentication/Login"
                    });
                }
            }
            return Json(new
            {
                Success = false,
                Messages = "Không tìm thấy token trong phiên đăng nhập. Tự chuyển về trang đang nhập!",
                url = "/Authentication/Login"
            });
        }

        // Hàm để cập nhật lại các session sau khi renewToken
        private void UpdateSessionAfterRenewToken(TokenModel tokenData)
        {
            HttpContext.Session.SetString("JWTToken", tokenData.AccessToken);
            HttpContext.Session.SetString("RefreshJWTToken", tokenData.RefreshToken);
            HttpContext.Session.SetString("ExpiresJWTToken", tokenData.Expires.ToString());


            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenData.AccessToken);
            HttpContext.Session.SetString("Username", token.Claims.FirstOrDefault(claim => claim.Type == "unique_name").Value);
            HttpContext.Session.SetString("Roles", token.Claims.FirstOrDefault(claim => claim.Type == "role").Value);
            HttpContext.Session.SetString("Id", token.Claims.FirstOrDefault(claim => claim.Type == "Id").Value);
            // HttpContext.Session.SetString("XuongName", xuongName.ToString());
        }
        // Hàm để remove các sesstion
        private void RemoveSession()
        {
            HttpContext.Session.Remove("JWTToken");// Xóa cac thong tin token khỏi session
            HttpContext.Session.Remove("RefreshJWTToken");
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("Roles");
            HttpContext.Session.Remove("ExpiresJWTToken");
            HttpContext.Session.Remove("Id");
            //HttpContext.Session.Remove("XuongId");
            HttpContext.Session.Remove("XuongName");
            HttpContext.Session.Remove("NumberDate");

        }
        private void RenewCookie(TokenModel tokenData, bool rememberMe)
        {
            HttpContext.Response.Cookies.Append("JWTToken", tokenData.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(rememberMe ? Hour * 60 : 60),
                Path = "/",
            });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, HttpContext.User.Identity.Name), // Giữ nguyên tên người dùng
                new Claim("jwt", Security.Crypt.ED.EncryptString(tokenData.AccessToken, AppViewModels.AppViewModel.Instance.DefaultKey)),
                new Claim("RefreshJWT", tokenData.RefreshToken),
                new Claim("ExpiresJWT", tokenData.Expires.ToString()),
                new Claim("XuongId", HttpContext.Session.GetString("XuongId"))
                // Thêm các claims khác nếu cần
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(rememberMe ? Hour * 60 : 60),
                IsPersistent = true
            };

            // Đăng nhập người dùng bằng cookie
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
        public async Task<IActionResult> RenewToken()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/RenewToken";
            var accessToken = HttpContext.Session.GetString("JWTToken");
            var refreshToken = HttpContext.Session.GetString("RefreshJWTToken");
            var refeshTokeModel = new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(refeshTokeModel), Encoding.UTF8, "application/json");

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

            if (response.Success)
            {
                var tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(response.Data.ToString());
                UpdateSessionAfterRenewToken(tokenData);
                RenewCookie(tokenData, RememberMe);
                return Json(new
                {
                    Success = response.Success,
                    Messages = response.Message
                });
            }
            else
            {
                return Json(new
                {
                    Success = response.Success,
                    Messages = response.Message
                });
            }
        }
        [CustomAuthorize(Fu = "Hubs", Func = "Máy Cân")]
        public async Task<IActionResult> UserArea()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "UserArea");

            if (rl == false)
            {

                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Người Dùng Máy Cân";
            return View("~/Views/Authentication/UserArea.cshtml");
        }
        [CustomAuthorize(Fu = "Xác Thực", Func = "Đăng Ký Tài Khoản")]
        public async Task<IActionResult> Register()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "Register");

            if (rl == false)
            {

                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Đăng ký tài khoản";
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Roles/GetAllRoles";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = helper.GetAsync<IEnumerable<RolesModel>>(HttpContext, apiUrl);//JsonConvert.DeserializeObject<List<Models.RolesModel>>(jsonString);
            ViewBag.listRoles = items.Result.ToList();
            return View("~/Views/Authentication/RegisterView.cshtml");
        }
        public async Task<IActionResult> DoRegister(string username, string password, string hoten, string phoneNumber, string maNhanVien)
        {

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/Register";
            var accessToken = HttpContext.Session.GetString("JWTToken");
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hoten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                var registerModel = new RegisterModel
                {
                    UserName = username,
                    Password = password,
                    HoTen = hoten,
                    PhoneNumber = phoneNumber,
                    MaNhanVien = maNhanVien,
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");
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

                // Xử lý trường hợp đăng ký không thành công
                return Json(new
                {
                    isSuccess = response.Success,
                    Messages = response.Message
                });

            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new
                {
                    isSuccess = false,
                    Messages = "Đã xảy ra lỗi: " + ex.Message
                });
            }
        }
        public async Task<IEnumerable<object>> GetAllUser()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/GetNguoiDungs";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<NguoiDung>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
                ViewBag.UserDataSource = dataSource;
            }

            return dataSource;
        }
        public async Task<IActionResult> GetInfoUserById(string Id)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/GetNguoiDungById/{Id}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<NguoiDung>(HttpContext, apiUrl);

                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        UserName = item.UserName,
                        Password = item.Password,
                        HoTen = item.HoTen,
                        Email = item.Email,
                        DefaultKey = item.DefaultKey,
                        MaNhanVien = item.MaNhanVien,
                        IsAdmin = item.IsAdmin,
                        Status = item.Status,
                        IsActive = item.IsActive,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        CardID = item.CardID,
                        AvatarImg = item.AvatarImg,
                    }); ;

                }

            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> GetInfoUserByUserName(string username)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/GetNguoiDungByUserName/{username}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<NguoiDung>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item.Id,
                        UserName = item.UserName,
                        Password = Security.Crypt.ED.DecryptString(item.Password, item.DefaultKey),
                        HoTen = item.HoTen,
                        Email = item.Email,
                        DefaultKey = item.DefaultKey,
                        MaNhanVien = item.MaNhanVien,
                        IsAdmin = item.IsAdmin,
                        Status = item.Status,
                        IsActive = item.IsActive,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        CardID = item.CardID,
                        AvatarImg = item.AvatarImg,
                    }); ;
                }
            }

            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }

        public async Task<IActionResult> DoUpdate(int id, string? avatarImg, string? hoten, string? email, string? maNhanVien, string? phoneNumber, string? address, string? cardID, bool? isActive, int? status, string userName, string password)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/UpdateUser/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(hoten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin. Họ Tên"
                    });
                }
                // Tạo một đối tượng RegisterModel để lưu trữ các trường dữ liệu
                var updateModel = new RegisterModel
                {
                    UserName = userName,
                    Password = password,
                    AvatarImg = avatarImg,
                    HoTen = hoten,
                    Email = email,
                    MaNhanVien = maNhanVien,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    CardID = cardID,
                    IsActive = isActive,
                    //Status = status
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(updateModel), Encoding.UTF8, "application/json");
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
        //public async Task<IActionResult> UploadImage(int id, string filePath)
        //{
        //    try
        //    {
        //        // Đường dẫn đến tệp ảnh cần tải lên
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", filePath);

        //        // Tạo httpClient
        //        var httpClient = _httpClientFactory.CreateClient();

        //        // Tạo formData chứa tệp ảnh
        //        var formData = new MultipartFormDataContent();
        //        var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(imagePath));
        //        fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
        //        formData.Add(fileContent, "file", Path.GetFileName(imagePath));

        //        // Gửi yêu cầu POST đến API để tải lên tệp ảnh
        //        var response = await httpClient.PostAsync($"api/ImageUpload/upload", formData);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Xử lý phản hồi từ API nếu cần
        //            return Ok("Tải lên tệp ảnh thành công.");
        //        }
        //        else
        //        {
        //            // Xử lý lỗi nếu có lỗi xảy ra khi gửi yêu cầu
        //            return BadRequest("Đã xảy ra lỗi khi tải lên tệp ảnh.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý nếu có lỗi xảy ra khi xử lý tệp ảnh
        //        return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
        //    }
        //}
        public async Task<IActionResult> DoUpdateSetting(int id, string avatarImg, string hoten, string email, string maNhanVien, string phoneNumber, string address)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/UpdateUserSetting/{id}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(hoten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin. Họ Tên"
                    });
                }

                // Tạo một đối tượng RegisterModel để lưu trữ các trường dữ liệu
                var updateModel = new RegisterModel
                {
                    HoTen = hoten,
                    Email = email,
                    MaNhanVien = maNhanVien,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    AvatarImg = avatarImg
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(updateModel), Encoding.UTF8, "application/json");

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

        public async Task<IActionResult> DeleteUser(int id)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/DeleteUser/{id}";
            try
            {
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
        [CustomAuthorize(Fu = "Xác Thực", Func = "Khôi Phục Tài Khoản")]
        public async Task<IActionResult> RecoverAccoutView()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "Register");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Khôi phục tài khoản";
            return View();
        }
        public async Task<IActionResult> DoRecoverUser(string username, string newPassword)
        {

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/User/RecoverAccount/{username}";
            var accessToken = HttpContext.Session.GetString("JWTToken");

            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPassword))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin"
                    });
                }
                // Tạo một đối tượng RegisterModel để lưu trữ các trường dữ liệu
                var recoverModel = new RecoverPasswordModel
                {
                    NewPassword = newPassword
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(recoverModel), Encoding.UTF8, "application/json");
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
        public async Task<IActionResult> Role()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "Role");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            return View("~/Views/Authentication/Role.cshtml");
        }
        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Roles/GetAllRoles";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<Role>>(HttpContext, apiUrl);
            return dataSource;
        }
        public async Task<IActionResult> RoleCreatDefautNew()
        {
            try
            {
                var dataSource = await GetAllRoles();
                if (dataSource != null)
                {

                    var maxId = dataSource.Max(x => x.Id);
                    var id = maxId + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        Id = id,
                        //SuDung = true,
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
        public async Task<IActionResult> DoInsertRole(int ma, string ten)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Roles/Insert";
            try
            {
                if (ma == null || string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new Role
                {
                    Id = ma,
                    Name = ten
                    //SuDung = suDung,
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
        public async Task<IActionResult> GetsByMaRole(int ma)
        {
            if (ma == null)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Roles/GetsByMa/{ma}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<Role>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        //SuDung = item?.SuDung,
                        Id = item?.Id,
                        Name = item?.Name
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> DoUpDateRole(int ma, string? ten)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Roles/Update/{ma}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (ma == null || string.IsNullOrEmpty(ten))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new Role
                {
                    Id = ma,
                    Name = ten
                    //SuDung = suDung
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
        public async Task<IActionResult> DoDeleteRole(int ma)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Roles/Delete/{ma}";
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
        [CustomAuthorize(Fu = "Xác Thực", Func = "Phân Quyền")]
        public async Task<IActionResult> Decentralization()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "Decentralization");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Tạo Quyền";
            return View("~/Views/Authentication/Decentralization.cshtml");
        }
        public async Task<IEnumerable<object>> GetAllDecentralizations()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/GetAllsFullField";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetsByRoleIdDecentralization(int IdRole)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/GetAllsFullField/{IdRole}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        public async Task<IActionResult> DecentralizationCreatDefautNew()
        {
            try
            {
                var dataSource = await GetAllDecentralizations();
                if (dataSource != null)
                {

                    var maxId = dataSource.Max(x => (int?)((dynamic)x).Id);
                    var id = (maxId ?? 0) + 1;
                    return Json(new
                    {
                        isSuccess = true,
                        Id = id,
                        //SuDung = true,
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
        public async Task<IActionResult> DoInsertDecentralization(int roleId, string fu, string func, int status)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/InsertList";
            try
            {
                if (roleId == null || string.IsNullOrEmpty(fu) || string.IsNullOrEmpty(func) || status == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var jsonDataResult = GetJsonData();
                if (jsonDataResult is not JsonResult jsonResult)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không thể lấy dữ liệu JSON."
                    });
                }
                var jsonData = System.Text.Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonResult.Value)));
                // Deserialize JSON thành danh sách đối tượng FuFunc
                var fuFuncList = JsonConvert.DeserializeObject<List<FuFunc>>(jsonData);

                // Xây dựng bảng ánh xạ giữa fu và danh sách func
                var fuFuncMap = fuFuncList.GroupBy(ff => ff.Fu)
                                          .ToDictionary(group => group.Key, group => group.Select(ff => ff.Func).ToList());
                // Tạo danh sách đối tượng RolePermistion
                var rolePermissionList = new List<RolePermistion>();
                // Duyệt qua từng giá trị fu từ param
                var fuValues = fu.Split(',').Select(value => value.Trim());
                foreach (var fuValue in fuValues)
                {
                    // Kiểm tra xem có fu nào tương ứng trong bảng ánh xạ không
                    if (fuFuncMap.TryGetValue(fuValue, out var funcList))
                    {
                        //// Duyệt qua từng giá trị func từ param
                        var funcValues = func.Split(',').Select(value => value.Trim());

                        // Kiểm tra xem danh sách funcList có chứa ít nhất một giá trị nào đó từ funcValues không
                        if (funcList.Any(funcValues.Contains))
                        {
                            // Tìm ra tất cả các giá trị func từ funcValues mà có trong funcList
                            var matchingFuncValues = funcValues.Intersect(funcList);

                            foreach (var matchingFuncValue in matchingFuncValues)
                            {
                                var model = new RolePermistion
                                {
                                    RoleId = roleId,
                                    Fu = fuValue,
                                    Func = matchingFuncValue,
                                    CreatedDateTime = DateTime.Now,
                                    Status = status
                                };

                                rolePermissionList.Add(model);
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                isSuccess = false,
                                Messages = $"Không tìm thấy ánh xạ cho Fu: {fuValue}, Func: {string.Join(", ", funcList)}."
                            });
                        }
                    }
                    else
                    {
                        // Xử lý trường hợp không tìm thấy ánh xạ
                        return Json(new
                        {
                            isSuccess = false,
                            Messages = $"Không tìm thấy ánh xạ cho Fu: {fuValue}."
                        });
                    }
                }

                var jsonContent = new StringContent(JsonConvert.SerializeObject(rolePermissionList), Encoding.UTF8, "application/json");
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
        public class FuFunc
        {
            public string Fu { get; set; }
            public string Func { get; set; }
        }
        public async Task<IActionResult> GetsByMaDecentralization(int permistionId)
        {
            if (permistionId == null)
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
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/GetAllRolePermistionById/{permistionId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var item = await helper.GetAsync<RolePermistion>(HttpContext, apiUrl);
                if (item != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        Mesages = "Thành Công",
                        Id = item?.Id,
                        RoleId = item?.RoleId,
                        Fu = item?.Fu,
                        Func = item?.Func,
                        CreatedDateTime = item?.CreatedDateTime,
                        Status = item?.Status,
                    });
                }
            }
            return Json(new
            {
                isSuccess = false,
                Mesages = "Lỗi!"
            });
        }
        public async Task<IActionResult> DoUpDateDecentralization(int permistionId, int idRolePer, string fuPer, string funcPer, DateTime creatDatePer, int status)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/Update/{permistionId}";
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (permistionId == null || status == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new RolePermistion
                {
                    Id = permistionId,
                    RoleId = idRolePer,
                    Fu = fuPer,
                    Func = funcPer,
                    CreatedDateTime = creatDatePer,
                    Status = status
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
        public async Task<IActionResult> DoDeleteDecentralization(int permistionId)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/Delete/{permistionId}";
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
        public IActionResult GetJsonData()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "FuFunc.json");

            // Đọc nội dung của tệp JSON
            var jsonData = System.IO.File.ReadAllText(filePath);

            // Chuyển đổi chuỗi JSON thành danh sách đối tượng FuFunc
            var fuFuncList = JsonConvert.DeserializeObject<List<FuFunc>>(jsonData);

            // Trả về dữ liệu JSON dưới dạng JsonResult
            return Json(fuFuncList);
        }
        public async Task<IEnumerable<RolePermistion>> GetAllRolePermistion(int roleId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/GetAllRolePermistionByRoleId/{roleId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<RolePermistion>>(HttpContext, apiUrl);
            return dataSource;
        }

        public async Task<IActionResult> GetDataMultiSelectRolePermistion(int roleId)
        {
            var rolePermissions = await GetAllRolePermistion(roleId);
            var jsonResult = GetJsonData() as JsonResult;
            var fuFuncList = jsonResult?.Value as List<FuFunc>;

            if (fuFuncList != null)
            {
                // Lấy danh sách Fu và Func đã có từ RolePermistion
                var existingFuFunc = rolePermissions.Select(rp => new FuFunc { Fu = rp.Fu, Func = rp.Func }).ToList();

                // Loại bỏ các cặp Fu và Func đã có
                var filteredFuFunc = fuFuncList.Except(existingFuFunc, new FuFuncEqualityComparer()).ToList();

                return Json(filteredFuFunc);
            }
            // Xử lý trường hợp không lấy được giá trị hoặc có lỗi khác
            return Json(new { error = "Không thể lấy dữ liệu JSON." });
        }
        public class FuFuncEqualityComparer : IEqualityComparer<FuFunc>
        {
            public bool Equals(FuFunc x, FuFunc y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;

                return x.Fu == y.Fu && x.Func == y.Func;
            }

            public int GetHashCode(FuFunc obj)
            {
                if (obj == null)
                    return 0;

                return (obj.Fu?.GetHashCode() ?? 0) ^ (obj.Func?.GetHashCode() ?? 0);
            }
        }
        public async Task<IEnumerable<Role>> GetRolesFilterIsExst(int userId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Roles/GetRolesFilterIsExst/{userId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<Role>>(HttpContext, apiUrl);
            return dataSource;
        }
        [CustomAuthorize(Fu = "Xác Thực", Func = "Phân Quyền")]
        public async Task<IActionResult> GrantAccess()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "GrantAccess");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "Phân Quyền";
            return View("~/Views/Authentication/GrantAccess.cshtml");
        }

        public async Task<IEnumerable<object>> GetsByUserIdUserRole(int userId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/UserRole/GetAllsFullField/{userId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        public async Task<IActionResult> DoInsertUserRole(int userId, string listroleId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/UserRole/InsertList";
            try
            {
                if (userId == null || string.IsNullOrEmpty(listroleId))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }

                var userRoleList = new List<UserRole>();

                var roleValues = listroleId.Split(',').Select(value => value.Trim());

                foreach (var i in roleValues)
                {
                    var model = new UserRole
                    {
                        UserId = userId,
                        RoleId = int.Parse(i)
                    };

                    userRoleList.Add(model);
                }
                var jsonContent = new StringContent(JsonConvert.SerializeObject(userRoleList), Encoding.UTF8, "application/json");
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                if (response.Success)
                {
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
        public async Task<IEnumerable<object>> GetsByUserRoleId(int userRole)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/UserRole/GetAllsFullFieldById/{userRole}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        public async Task<IEnumerable<object>> GetsUserAreaByUserId(int userId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/UserArea/GetAllsFullField/{userId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            ViewBag.UserAreaDataSource = dataSource;
            return dataSource;
        }

        [HttpPost]
        public async Task<IActionResult> DoInsertUserData([FromBody] JObject data)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/UserArea/Insert";
            try
            {
                var item = data.ToObject<UserArea>();
                if (item != null)
                {

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var response = await helper.PostAsync(HttpContext, apiUrl, jsonContent);

                    if (response.Success)
                    {
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
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Không thể nhận dữ liệu."
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
        [HttpPost]
        public async Task<IActionResult> DoDeleteUserData([FromBody] int userDataId)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/UserArea/Delete/{userDataId}";
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
        public async Task<IActionResult> DoDeleteUserRole(int userRoleId)
        {
            try
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/UserRole/Delete/{userRoleId}";
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
