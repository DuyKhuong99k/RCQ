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
using Models.Repos.A_Model;
using DocumentFormat.OpenXml.Spreadsheet;
namespace PMS.Middlewares
{
    public static class AuthenticationHelpers
    {
        private static IHttpClientFactory _httpClientFactory;
        public static void Initialize(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public static ActionResult CheckAut(HttpContext context, ViewResult view, RedirectResult redirect, string reportStype = "")
        {
            var accsessToken = context.Session.GetString("JWTToken");
            var expiresJWTToken = context.Session.GetString("ExpiresJWTToken");

            if (string.IsNullOrWhiteSpace(accsessToken) || string.IsNullOrWhiteSpace(expiresJWTToken))
            {
                return redirect;
            }

            if (!DateTime.TryParse(expiresJWTToken, out DateTime expirationTimeUtc))
            {
                return redirect;
            }

            if (expirationTimeUtc < DateTime.UtcNow)
            {
                return redirect;
            }

            if (reportStype != "")
            {
                context.Session.SetString("reportType", reportStype);
            }
            return view;
        }
        public static bool CheckAut(HttpContext context, string reportStype = "")
        {
            var accsessToken = context.Session.GetString("JWTToken");
            var expiresJWTToken = context.Session.GetString("ExpiresJWTToken");
            if (string.IsNullOrWhiteSpace(accsessToken) || string.IsNullOrWhiteSpace(expiresJWTToken))
            {
                return false;
            }

            if (!DateTime.TryParse(expiresJWTToken, out DateTime expirationTimeUtc))
            {
                return false;
            }

            if (expirationTimeUtc < DateTime.UtcNow)
            {
                return false;
            }
            if (reportStype != "")
            {
                context.Session.SetString("reportType", reportStype);
            }
            return true;
        }
        //public static List<int> GetRoleIdsFromCookie(HttpContext httpContext)
        //{
        //    var userClaims = httpContext.User.Claims;
        //    var roleClaims = userClaims.Where(c => c.Type == "RoleId").ToList();
        //    return roleClaims.Select(c => int.Parse(c.Value)).ToList();
        //}
        public static List<int> GetRoleIdsFromCookie(HttpContext httpContext)
        {
            var userClaims = httpContext.User?.Claims ?? Enumerable.Empty<Claim>();

            var roleIds = userClaims
                .Where(c => c.Type == "RoleId" && int.TryParse(c.Value, out _))
                .Select(c => int.Parse(c.Value))
                .ToList();

            if (roleIds.Count == 0)
            {
                // Có thể log lỗi hoặc gán mặc định
                // Logger.Warn("Không tìm thấy RoleId trong claims.");
                // return new List<int> { 0 }; // Nếu bạn muốn tránh API lỗi
            }

            return roleIds;
        }
        public static async Task<List<RolePermistion>> GetRolePermistionsAsync(HttpContext httpContext, List<int> roleIds)
        {
            var roleIdsQueryParam = string.Join("&", roleIds.Select(id => $"roleIds={id}"));
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/GetRolePermistions?{roleIdsQueryParam}";

            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            var rolePermistions = await helper.GetAsync2<RolePermistion>(httpContext, apiUrl)
                                   ?? new List<RolePermistion>(); // <- Thêm dòng này

            string jsonString = JsonConvert.SerializeObject(rolePermistions);
            httpContext.Session.SetString("ListRolePermistionByRoleId", jsonString);

            return rolePermistions;
        }
        //public static async Task<List<RolePermistion>> GetRolePermistionsAsync(HttpContext httpContext, List<int> roleIds)
        //{
        //    // Chuyển danh sách roleIds thành một chuỗi query parameters
        //    var roleIdsQueryParam = string.Join("&", roleIds.Select(id => $"roleIds={id}"));

        //    // Tạo URL API với các query parameters
        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/RolePermistion/GetRolePermistions?{roleIdsQueryParam}";

        //    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

        //    // Gọi API sử dụng helper
        //    var rolePermistions = await helper.GetAsync2<RolePermistion>(httpContext, apiUrl);
        //    string jsonString = JsonConvert.SerializeObject(rolePermistions);
        //    httpContext.Session.SetString("ListRolePermistionByRoleId", jsonString.ToString());
        //    return rolePermistions;
        //}
        public static async Task<IEnumerable<KhuVuc>> GetKhuVucs(HttpContext httpContext)
        {
            IEnumerable<KhuVuc> dataSource = null;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/KhuVucs/Gets";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                dataSource = await helper.GetAsync<IEnumerable<KhuVuc>>(httpContext, apiUrl);
            }
            return dataSource;
        }

        public static async Task<string> GetNameNhanVienByMaNhanVien(HttpContext httpContext, string maNhanVien)
        {
            string dataSource = null;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNameNhanVienByMaNhanVien/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                dataSource = await helper.GetAsync<string>(httpContext, apiUrl);
            }
            return dataSource;
        }
        public static async Task<string> GetMaHoSoNhanVienByMaNhanVien(HttpContext httpContext, string maNhanVien)
        {
            string dataSource = null;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetMaHoSoNhanVienByMaNhanVien/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                dataSource = await helper.GetAsync<string>(httpContext, apiUrl);
            }
            return dataSource;
        }
    }
}
