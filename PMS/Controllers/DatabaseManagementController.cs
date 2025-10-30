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

namespace PMS.Controllers
{
    public class DatabaseManagementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DatabaseManagementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Quản Lý Dữ Liệu")]
        public IActionResult Index()
        {
            ViewBag.TitlePage = "Quản Lý CSDL";
            return View("~/Views/DatabaseManagement/DatabaseManagementView.cshtml");
        }
        //public class TableName
        //{
        //    public string tableName { get; set; }
        //}
        public class TableNameJsonData
        {
            public List<string> TableName { get; set; }
        }

        public List<string> GetJsonData()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "TableName.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var jsonData = System.IO.File.ReadAllText(filePath);

            // Deserialize dữ liệu thành đối tượng TableNameJsonData
            var tableNameData = JsonConvert.DeserializeObject<TableNameJsonData>(jsonData);

            // Trả về danh sách TableName từ đối tượng TableNameJsonData
            return tableNameData?.TableName;
        }

        public async Task<IEnumerable<object>> GetAllsFullField()
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;

            if (dataSource == null)
            {
                // Lấy danh sách TableName từ JSON
                var tableNameList = GetJsonData();


                if (tableNameList != null)
                {
                    var allTableInfo = new List<object>();

                    foreach (var tableName in tableNameList)
                    {
                        var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TableInfos/GetAllsFullField/{tableName}";
                        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var tableInfos = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

                        // Thêm tất cả các TableInfo từ API vào danh sách chung
                        allTableInfo.AddRange(tableInfos);
                    }

                    ViewBag.dataSource = allTableInfo;
                    dataSource = ViewBag.dataSource;
                }
                else
                {
                    return Enumerable.Empty<object>();
                }
            }

            return dataSource;
        }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Quản Lý Dữ Liệu")]
        public async Task<IActionResult> BackupDatabase()
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TableInfos/BackupDatabase";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var item = await helper.GetAsync<ApiResponse>(HttpContext, apiUrl);
            if (item != null)
            {
                return Json(new
                {
                    isSuccess = item?.Success,
                    Mesages = item?.Message,

                });
            }
            return Json(new
            {
                isSuccess = item?.Success,
                Mesages = item?.Message,
            });
        }
        [CustomAuthorize(Fu = "Cài Đặt", Func = "Quản Lý Dữ Liệu")]
        public async Task<IActionResult> DeleteOldData(string tableName, int dateKeep)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/TableInfos/DeleteOldData/{tableName}/{dateKeep}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            string postData = $"tableName={tableName}&dateKeep={dateKeep}";
            var item = await helper.PostAsync(HttpContext, apiUrl, null);
            if (item != null)
            {
                return Json(new
                {
                    isSuccess = item?.Success,
                    Mesages = item?.Message,

                });
            }
            return Json(new
            {
                isSuccess = item?.Success,
                Mesages = item?.Message,
            });
        }
       

    }
}
