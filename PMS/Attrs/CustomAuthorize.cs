
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using PMS.Controllers;

namespace PMS.Attrs
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        // Các tham số có tên tùy chỉnh cho chú thích
        public string Fu { get; set; } // Tên Controller

        public string Func { get; set; } // Tên Action

        private dbPMScontext _context;

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            // Người dùng đã đăng nhập chưa?
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(Fu) && !string.IsNullOrEmpty(Func))
                {
                    var loggedInUserRoles = context.HttpContext.User.Claims
                        .Where(c => c.Type == "RoleId")
                        .Select(c => c.Value);
                    var stringJson = context.HttpContext.Session.GetString("ListRolePermistionByRoleId");
                    List<RolePermistion> rolePermistions = JsonConvert.DeserializeObject<List<RolePermistion>>(stringJson);
                    var rolePermission = rolePermistions
                        .Where(x => x.Fu == Fu && x.Func == Func)
                        .OrderByDescending(x => x.CreatedDateTime)
                        .GroupBy(x => x.RoleId)
                        .Select(group => group.First())
                        .ToList();
                    var hasAccess = rolePermission.Any(x => loggedInUserRoles.Contains(x.RoleId.ToString()) && x.Status == 1);

                    if (!hasAccess)
                    {
                        var routeValues = new { action = "Unauthorized" }; // Khởi tạo route values với action là "PageNotFound"
                        context.Result = new RedirectToRouteResult("error", routeValues);
                        return;
                    }
                }
            }
            else
            {
                // Người dùng chưa đăng nhập
                context.Result = new RedirectToActionResult("Login", "Authentication", null);
                return;
            }
        }
    }
}
