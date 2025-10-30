using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Repos.A_Model;
using Newtonsoft.Json;
using PMS.Attrs;
using PMS.Models;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace PMS.Controllers
{

    public class DashboardHQController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DashboardHQController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {

            return View();
        }

        //public async Task<IEnumerable<object>> GetTongHopThanhPhams(string xuongId)
        //{
        //    DateTime? fromDate = null;
        //    DateTime? toDate = null;
        //    //DateTime? fromDate = new DateTime(2025, 02, 22, 0, 0, 0);
        //    //DateTime? toDate = new DateTime(2025, 02, 22, 23, 59, 59);
        //    if (fromDate == null) fromDate = DateTime.Now;
        //    if (toDate == null) toDate = DateTime.Now;
        //    if (fromDate == null) fromDate = DateTime.Now;
        //    if (toDate == null) toDate = DateTime.Now;
        //    IEnumerable<object> dataSource = ViewBag.dataSourceChart;
        //    //if (dataSource == null)
        //    //{
        //        var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhamDashboards/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //        ViewBag.dataSourceChart = await helper.GetAsync<object>(HttpContext, apiUrl);
        //        dataSource = ViewBag.dataSourceChart;
        //    //}
        //    return dataSource;
        //}
        public async Task<IEnumerable<object>> GetTongHopThanhPhams(string xuongId)
        {
            // Cố định ngày từ 00:00 đến 23:59 hôm nay
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            // Gọi API lấy dữ liệu
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhamDashboards/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        //public async Task<IEnumerable<object>> GetTongHopThanhPham2s(string xuongId)
        //{
        //    //DateTime? fromDate = new DateTime(2025, 02, 22, 0, 0, 0);
        //    //DateTime? toDate = new DateTime(2025, 02, 22, 23, 59, 59);

        //    var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhamDashboards/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";

        //    using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //    var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

        //    return dataSource ?? Enumerable.Empty<IEnumerable<object>>();
        //}
        //public async Task<IEnumerable<object>> GetTongHopThanhPhamGrids(string xuongId)
        //{
        //    DateTime? fromDate = null;
        //    DateTime? toDate = null;
        //    //DateTime? fromDate = new DateTime(2025, 02, 22, 0, 0, 0);
        //    //DateTime? toDate = new DateTime(2025, 02, 22, 23, 59, 59);
        //    if (fromDate == null) fromDate = DateTime.Now;
        //    if (toDate == null) toDate = DateTime.Now;
        //    //if (fromDate == null) fromDate = DateTime.Now;
        //    //if (toDate == null) toDate = DateTime.Now;
        //    IEnumerable<object> dataSource = ViewBag.dataSource;
        //    if (dataSource == null)
        //    {
        //        var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhamDashboardForGrids/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
        //        using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
        //        ViewBag.dataSource = await helper.GetAsync<object>(HttpContext, apiUrl);
        //        dataSource = ViewBag.dataSource;
        //    }
        //    return dataSource;
        //}
        public async Task<IEnumerable<object>> GetTongHopThanhPhamGrids(string xuongId)
        {
            // Cố định ngày từ 00:00 đến 23:59 hôm nay
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhamDashboardForGrids/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }


        public async Task<IActionResult> GetDanhSachThanhPhamTrongLuong(string xuongId)
        {

            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/HQ_PhieuCans/GetTongHopThanhPhamDashboards/{fromDate?.ToString("yyyy-MM-dd")}/{toDate?.ToString("yyyy-MM-dd")}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
            if (items != null && items.Any())
            {
                var danhSachThanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName, x.TrongLuong })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName,
                                    TrongLuong = x.Key.TrongLuong
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();


                return Json(new
                {
                    Success = true,
                    datas = danhSachThanhPhams,
                });
            }
            return Json(new
            {
                Success = false,
            });

        }

        public class ChartData
        {
            public string x { get; set; }
            public double? y { get; set; }
            //public double? dm { get; set; }
            //public double? dmc { get; set; }
        }
        public async Task<List<ChartData>> GetBaoCaoNhanhsForChart(string xuongId)
        {
            // Gọi API để lấy dữ liệu từ phương thức GetBaoCaoNhanhs
            var dataSource = await GetTongHopThanhPhams(xuongId);

            // Kiểm tra xem có dữ liệu không
            if (dataSource != null && dataSource.Any())
            {
                // Chuyển đổi dữ liệu sang định dạng dùng cho biểu đồ
                var chartData = dataSource.Cast<dynamic>().Select(item => new ChartData
                {

                    x = item.ThanhPhamName ?? "",
                    y = (double?)item.TrongLuong ?? 0,
                    //dm = (double?)item.DinhMuc ?? 0,
                    //dmc = (double?)item.DinhMucChuan ?? 0

                }).ToList();

                return chartData;
            }

            // Trả về danh sách rỗng nếu không có dữ liệu
            return new List<ChartData>();
        }

        //public async Task<ActionResult> Reload(string xuongId)
        //{
        //    DateTime? fromDate = null;
        //    DateTime? toDate = null;
        //    //DateTime? fromDate = new DateTime(2025, 02, 22, 0, 0, 0);
        //    //DateTime? toDate = new DateTime(2025, 02, 22, 23, 59, 59);
        //    if (fromDate == null) fromDate = DateTime.Now;
        //    if (toDate == null) toDate = DateTime.Now;


        //    IEnumerable<object> dataSource = null;
        //    IEnumerable<ChartData> dataSourceChart = null;
        //    IEnumerable<object> dataSourceChiTiet = null;
        //    bool success = true;


        //    dataSource = await GetTongHopThanhPhamGrids(xuongId);
        //    ViewBag.dataSourceTongHop = dataSource;


        //    dataSourceChart = await GetBaoCaoNhanhsForChart(xuongId);
        //    if (dataSource == null || !dataSource.Any())
        //    {
        //        success = false;
        //    }

        //    // Tính tổng trường TrongLuong hoặc TrongLuongTra dựa trên khauValues
        //    double totalTrongLuong = dataSourceChart?.Sum(item => item.y ?? 0) ?? 0;
        //    var result = new
        //    {
        //        Success = success,
        //        Messages = success ? "Lấy dữ liệu Thành Công." : "Vui lòng kiểm tra lại!.",
        //        Data = dataSource,
        //        DataChart = dataSourceChart,
        //        TotalTrongLuong = Math.Round(totalTrongLuong, 3)
        //    };
        //    return Json(result);
        //}
        public async Task<ActionResult> Reload(string xuongId)
        {
            // Cố định ngày từ 00:00 đến 23:59 hôm nay
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            bool success = true;

            // Lấy dữ liệu tổng hợp
            var dataSource = await GetTongHopThanhPhamGrids(xuongId);

            // Lấy dữ liệu biểu đồ
            var dataSourceChart = await GetBaoCaoNhanhsForChart(xuongId);

            if (dataSource == null || !dataSource.Any())
            {
                success = false;
            }

            // Tính tổng trọng lượng từ dữ liệu biểu đồ
            double totalTrongLuong = dataSourceChart?.Sum(item => item.y ?? 0) ?? 0;

            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                Data = dataSource,
                DataChart = dataSourceChart,
                TotalTrongLuong = Math.Round(totalTrongLuong, 3)
            };

            return Json(result);
        }

    }
}
