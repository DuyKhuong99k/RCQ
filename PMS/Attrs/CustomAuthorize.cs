
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.EntityFrameworkCore;
//using Models.Repos;
//using Models.Repos.Models;
//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using PMS.Controllers;

//namespace PMS.Attrs
//{
//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
//    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
//    {
//        // Các tham số có tên tùy chỉnh cho chú thích
//        public string Fu { get; set; } // Tên Controller

//        public string Func { get; set; } // Tên Action

//        private dbPMScontext _context;

//        public async void OnAuthorization(AuthorizationFilterContext context)
//        {
//            // Người dùng đã đăng nhập chưa?
//            if (context.HttpContext.User.Identity is { IsAuthenticated: true })
//            {
//                if (!string.IsNullOrEmpty(Fu) && !string.IsNullOrEmpty(Func))
//                {
//                    var loggedInUserRoles = context.HttpContext.User.Claims
//                        .Where(c => c.Type == "RoleId")
//                        .Select(c => c.Value);
//                    var hasAccess = false;
//                    var stringJson = context.HttpContext.Session.GetString("ListRolePermistionByRoleId");
//                    if (stringJson != null)
//                    {
//                        var rolePermistions = JsonConvert.DeserializeObject<List<RolePermistion>>(stringJson);
//                        if (rolePermistions != null)
//                        {
//                            var rolePermission = rolePermistions
//                                .Where(x => x.Fu == Fu && x.Func == Func)
//                                .OrderByDescending(x => x.CreatedDateTime)
//                                .GroupBy(x => x.RoleId)
//                                .Select(group => group.First())
//                                .ToList();
//                            hasAccess = rolePermission.Any(x => loggedInUserRoles.Contains(x.RoleId.ToString()) && x.Status == 1);
//                        }

//                    }

//                    if (hasAccess) return;
//                    var routeValues = new { action = "Unauthorized" }; // Khởi tạo route values với action là "PageNotFound"
//                    context.Result = new RedirectToRouteResult("error", routeValues);
//                }
//            }
//            else
//            {
//                // Người dùng chưa đăng nhập
//                context.Result = new RedirectToActionResult("Login", "Authentication", null);
//                return;
//            }
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System.Security.Claims;
using Models.Repos.Models;

namespace PMS.Attrs
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public string Fu { get; set; } // Controller name
        public string Func { get; set; } // Action name

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Kiểm tra người dùng đã đăng nhập chưa
            if (user.Identity is not { IsAuthenticated: true })
            {
                context.Result = new RedirectToActionResult("Login", "Authentication", null);
                return;
            }

            if (string.IsNullOrEmpty(Fu) || string.IsNullOrEmpty(Func))
            {
                // Không kiểm tra quyền nếu chưa cấu hình đúng
                return;
            }

            // Lấy RoleId từ Claim
            var roleIdClaims = user.Claims
                .Where(c => c.Type == "RoleId" && int.TryParse(c.Value, out _))
                .Select(c => int.Parse(c.Value))
                .ToList();

            if (!roleIdClaims.Any())
            {
                context.Result = new RedirectToActionResult("Login", "Authentication", null);
                return;
            }

            // Lấy danh sách quyền từ session
            var json = context.HttpContext.Session.GetString("ListRolePermistionByRoleId");
            if (string.IsNullOrEmpty(json))
            {
                context.Result = new RedirectToActionResult("Login", "Authentication", null);
                return;
            }

            var rolePermissions = JsonConvert.DeserializeObject<List<RolePermistion>>(json);
            if (rolePermissions == null)
            {
                context.Result = new RedirectToActionResult("Login", "Authentication", null);
                return;
            }

            // Kiểm tra quyền
            var matchedPermissions = rolePermissions
                .Where(p => p.Fu == Fu && p.Func == Func)
                .OrderByDescending(p => p.CreatedDateTime)
                .GroupBy(p => p.RoleId)
                .Select(g => g.First())
                .ToList();

            bool hasAccess = matchedPermissions.Any(p =>
                roleIdClaims.Contains(p.RoleId) && p.Status == 1);

            if (!hasAccess)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "Unauthorized" }
                });
            }
        }
    }
}

