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

namespace PMS.Controllers
{
    public class HelpersController : Controller
    {
        private readonly IHubContext<PMS.Hubs.ProgressHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public HelpersController(IHubContext<PMS.Hubs.ProgressHub> hubContext, IHttpClientFactory httpClientFactory)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> GetNameNhanVienByMaNhanVien(string maNhanVien)
        {
            string dataSource = null;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetNameNhanVienByMaNhanVien/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                dataSource = await helper.GetAsync<string>(HttpContext, apiUrl);
            }
            return dataSource;
        }
        public async Task<string> GetMaHoSoNhanVienByMaNhanVien(HttpContext httpContext, string maNhanVien)
        {
            string dataSource = null;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/NhanVienDaiThanhs/GetMaHoSoNhanVienByMaNhanVien/{maNhanVien}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                dataSource = await helper.GetAsync<string>(HttpContext, apiUrl);
            }
            return dataSource;
        }
    }
}
