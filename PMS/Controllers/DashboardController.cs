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
using static PMS.Controllers.HomeController;

namespace PMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> AddUserWaitView(string id, DateTime thoiGian, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/Dashboards/AddUserWaitView";
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(id))
                {
                    return Json(new
                    {
                        isSuccess = false,
                        Messages = "Vui lòng nhập đầy đủ thông tin."
                    });
                }
                var model = new ListUserWaitView
                {
                    Id = id,
                    ThoiGian = thoiGian,
                    XuongId = xuongId
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




       //[CustomAuthorize(Fu = "Dashboard", Func = "Tổng Hợp Thành Phẩm Định Hình")]
        public IActionResult DBTongHopTPDinhDinhHinh()
        {
            return PartialView();
        }
        //[CustomAuthorize(Fu = "Dashboard", Func = "Tổng Hợp Thành Phẩm Định Hình")]
        public IActionResult DBTongHopTPDinhHinh2View() { return PartialView(); }
        public async Task<IActionResult> TongHopTyLeThanhPhamWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopLoaiThanhPhams2DB/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var listTPTLModel = new List<TPTLModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)x.TrongLuongTra)
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuongTra)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuongNhan)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }

        public class TyLeNameModel
        {
            public string MaTP { get; set; }
            public string labels { get; set; }
            public double percentage { get; set; }
            public decimal TrongLuongTP { get; set; }
            public decimal DinhMucTP { get; set; }
            public decimal TongTL { get; set; }
        }
        //[CustomAuthorize(Fu = "Dashboard", Func = "Tổng Hợp Thành Phẩm Fillet")]
        public IActionResult DBTongHopTPFilletPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamFilletWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopLoaiThanhPhamFilletDB/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuongTra))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuongTra)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuongNhan)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPNguyenLieuPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamNguyenLieuWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetTongHopThanhPhamDashBoardDB/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();


                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPXepKhuonPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamXepKhuonWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanChinhXepKhuons/GetPhieuCanTongHopChinhXepKhuonsDB/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();


                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopGheVungNuoiDaiThanhSidePartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeGheVungNuoiDaiThanhSideWithDate(DateTime fromDate, DateTime toDate)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanVungNuoiDaiThanhSides/GetTongHopGheDB/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var ghes = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaGhe, x.TenGhe })
                            .Select(
                                x => new
                                {
                                    MaGhe = x.Key.MaGhe,
                                    TenGhe = x.Key.TenGhe
                                })
                            .OrderBy(x => x.MaGhe)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < ghes.Count; i++)
                    {
                        var ghe = ghes[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongGheItems = itemDycs.Where(x => x.MaGhe == ghe.MaGhe)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();


                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongGheItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = ghe.MaGhe,
                            labels = ghe.TenGhe,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongGheItems,
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopBTPFilletPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamBTPFilletWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPSoCheDinhHinhPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamSoCheDinhHinhWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanSoCheDinhHinhs/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPPhuXepKhuonPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamPhuXepKhuonWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuXepKhuons/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPXepKhuonBlockPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamXepKhuonBlockWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonBlocks/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPXepKhuonKHCPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamXepKhuonKHCWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanXepKhuonKHCs/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPTaiChePartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamTaiCheWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTaiChes/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPPhuPhamPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamPhuPhamWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanPhuPhams/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
        public IActionResult DBTongHopTPBTPDinhHinhPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamBTPDinhHinhWithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemDycs = items.Cast<dynamic>().ToList();
                    var thanhPhams = items.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();


                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        //var trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        //.Select(x => (decimal)x.TrongLuongNhan)
                        //                            .DefaultIfEmpty(0)
                        //                            .Sum();
                        //var dinhMucThanhPhamItems = Math.Round(trongLuongThanhPhamItems == 0 ? 0 : trongLuongThanhPhamNhanItems / trongLuongThanhPhamItems, 3);
                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                    });
                }

                return Json(new
                {

                });
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
