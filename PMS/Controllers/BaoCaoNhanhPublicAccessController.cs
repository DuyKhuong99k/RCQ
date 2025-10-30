using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Models;
using System.Runtime.CompilerServices;
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
using AppViewModels;
using static PMS.Controllers.DatabaseManagementController;
using static PMS.Controllers.AuthenticationController;
using BravoModelV1;

namespace PMS.Controllers
{
    [Route("bao-cao-nhanh-public-access")]
    public class BaoCaoNhanhPublicAccessController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaoCaoNhanhPublicAccessController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [Route("view")]
        public IActionResult Index()
        {

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/XiNghieps/GetAllXuongNoCheckAuth";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var listXuong = helper.GetAsync<IEnumerable<XiNghiep>>(HttpContext, apiUrl);

            var rsListKhau = GetListKhau();
            ViewBag.listKhau = rsListKhau;
            ViewBag.listXuong = listXuong.Result.ToList(); ;
            return View();
        }

        public class ListKhauBaoCaoNhanh
        {
            public string TableName { get; set; }
            public string NameView { get; set; }
        }
        private List<ListKhauBaoCaoNhanh> GetListKhau()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonFuFunc", "ListKhauBaoCaoNhanh.json");

            // Đọc nội dung của tệp JSON
            var jsonData = System.IO.File.ReadAllText(filePath);

            // Chuyển đổi chuỗi JSON thành danh sách đối tượng ListKhauBaoCaoNhanh
            var listKhau = JsonConvert.DeserializeObject<List<ListKhauBaoCaoNhanh>>(jsonData);

            return listKhau;
        }

        public IActionResult GetViewByTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return BadRequest("Invalid table name");
            }

            // Tùy thuộc vào tableName, trả về partial view tương ứng
            switch (tableName)
            {
                case "PhieuCanPhuPhamv2":
                    return PartialView("_PhuPhamV2Partial");
                case "PhieuCanBTPFilletv2":
                    return PartialView("_BTPFilletV2Partial");
                case "PhieuCanLangDa":
                    return PartialView("_LangDaPartial");
                // Thêm các trường hợp khác tương tự
                default:
                    return PartialView("_DefaultPartial");
            }
        }

        public async Task<IEnumerable<object>> GetBaoCaoNhanhs(DateTime fromDate, DateTime toDate, string keyWord, int typeSearch, string khauValues, string xuongId)
        {
            IEnumerable<object> dataSource = ViewBag.dataSource;
            if (dataSource == null)
            {
                var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/BaoCaoNhanhs/GetsTongHopThanhPhamByNhanVienPublicAccess/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}/{keyWord}/{typeSearch}/{khauValues}/{xuongId}";
                using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                ViewBag.dataSource = await helper.GetAsync<object>(apiUrl);
                dataSource = ViewBag.dataSource;
            }
            return dataSource;
        }

        public class ChartData
        {
            public string x { get; set; }
            public double? y { get; set; }
            public double? dm { get; set; }
        }
        public async Task<List<ChartData>> GetBaoCaoNhanhsForChart(DateTime fromDate, DateTime toDate, string keyWord, int typeSearch, string khauValues, string xuongId)
        {
            // Gọi API để lấy dữ liệu từ phương thức GetBaoCaoNhanhs
            var dataSource = await GetBaoCaoNhanhs(fromDate, toDate, keyWord, typeSearch, khauValues, xuongId);

            // Kiểm tra xem có dữ liệu không
            if (dataSource != null && dataSource.Any())
            {
                // Chuyển đổi dữ liệu sang định dạng dùng cho biểu đồ
                var chartData = dataSource.Cast<dynamic>().Select(item => new ChartData
                {

                    x = item.ThanhPhamName ?? "",
                    y = (khauValues == "PhieuCanTPFilletv2" || khauValues == "PhieuCanTPDinhHinh")
                        ? (double?)item.TrongLuongTra ?? 0
                        : (double?)item.TrongLuong ?? 0,
                    dm = (double?)item.DinhMuc ?? 0,

                }).ToList();

                return chartData;
            }

            // Trả về danh sách rỗng nếu không có dữ liệu
            return new List<ChartData>();
        }

        [Route("reload")]
        public async Task<ActionResult> Reload(DateTime fromDate, DateTime toDate, string keyWord, int typeSearch, string khauValues, string xuongId)
        {
            IEnumerable<object> dataSource = null;
            IEnumerable<object> dataSourceChart = null;
            bool success = true;
            dataSource = await GetBaoCaoNhanhs(fromDate, toDate, keyWord, typeSearch, khauValues, xuongId);
            dataSourceChart = await GetBaoCaoNhanhsForChart(fromDate, toDate, keyWord, typeSearch, khauValues, xuongId);
            if (dataSource == null || !dataSource.Any())
            {
                success = false;
            }

            // Tính tổng trường TrongLuong hoặc TrongLuongTra dựa trên khauValues
            decimal totalTrongLuong = dataSource?.Sum(item =>
            {
                // Giả sử item là dynamic, bạn có thể sử dụng Reflection nếu không rõ kiểu
                if (khauValues == "PhieuCanTPFilletv2" || khauValues == "PhieuCanTPDinhHinh")
                {
                    var trongLuongTra = (item as dynamic)?.TrongLuongTra;
                    return trongLuongTra ;
                }
                else
                {
                    var trongLuong = (item as dynamic)?.TrongLuong;
                    return trongLuong;
                }
            }) ?? 0;

            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu Thành Công." : "Vui lòng kiểm tra lại!.",
                Data = dataSource,
                DataChart = dataSourceChart,
                TotalTrongLuong = totalTrongLuong
            };
            return Json(result);
        }

    }
}
