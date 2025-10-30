using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Repos.A_Model;
using Models.Repos.Models;
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
            public string TrongLuongTPFormat => TrongLuongTP.ToString("N2", CultureInfo.InvariantCulture);
            public decimal DinhMucTP { get; set; }
            public string TrongLuongAndPersente => TrongLuongTP.ToString("N2") + $@" ({percentage}%)";
            public string TrongLuongAndDinhMucAnhPersente => TrongLuongTP.ToString("N2") + $@" - {DinhMucTP}" + $@" ({percentage}%)";
            public decimal TongTL { get; set; }
        }
        //[CustomAuthorize(Fu = "Dashboard", Func = "Tổng Hợp Thành Phẩm Fillet")]
        public IActionResult DBTongHopTPFilletPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamFilletWithDate(DateTime fromDate, DateTime toDate, string xuongId)
        {

            //var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopLoaiThanhPhamFilletDB/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            // danh sách tp dạt xẻ bướm
            var apiTongHopDinhMucXeBuomUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopDinhMucTPXeBuoms/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helperTongHopDinhMucXeBuom = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);


            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {

                    var itemBTPFillet = await TongHopTyLeThanhPhamBTPFilletWithDate(fromDate, toDate) as JsonResult;

                    dynamic data = itemBTPFillet.Value;
                    decimal tongTLBTPFillet = decimal.Parse(data.TongTLDaQuyVeCuaDatFillet);

                    //trọng lượng dạt xẻ bướm
                    var itemTongHopDinhMucXeBuom = await helperTongHopDinhMucXeBuom.GetAsync2<object>(HttpContext, apiTongHopDinhMucXeBuomUrl);

                    var trongLuongDat = itemTongHopDinhMucXeBuom.Cast<dynamic>().Select(x => new { DatXeBuom = (decimal)x.DatXeBuom }).FirstOrDefault();
                    var dinhMucMocRuot = itemTongHopDinhMucXeBuom.Cast<dynamic>().Select(x => new { DinhMucMocRuot = (decimal)x.DinhMucMocRuot }).FirstOrDefault();

                    //var trongLuongDat = itemTongHopDinhMucXeBuom.Cast<dynamic>()
                    //    .Select(x => new { DatXeBuom = x.DatXeBuom == null ? 0 : (decimal)x.DatXeBuom })
                    //    .FirstOrDefault();

                    //var dinhMucMocRuot = itemTongHopDinhMucXeBuom.Cast<dynamic>()
                    //    .Select(x => new { DinhMucMocRuot = x.DinhMucMocRuot == null ? 0 : (decimal)x.DinhMucMocRuot })
                    //    .FirstOrDefault();

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

                    //var trongluongCaTrang = itemDycs.FirstOrDefault(x => x.MaThanhPham == "CTR")?.DinhMuc ?? 0;
                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];
                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuongTra)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                        decimal trongLuongThanhPhamNhanItems = 0;


                        var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
                        using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl).Result;
                        if (tpfls != null)
                        {
                            var matchedTP = tpfls.FirstOrDefault(m => (string)m.Ma == (string)thanhPham.MaThanhPham);
                            if (matchedTP != null)
                            {
                                bool isDatFillet = matchedTP.IsDat == true && matchedTP.IsNguyenCon == false;
                                if (isDatFillet)
                                {
                                    decimal dat = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                                        .Select(x => (decimal)x.TrongLuongNhan)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                                    trongLuongThanhPhamNhanItems = dat ;//* (dinhMucMocRuot?.DinhMucMocRuot ?? 0);
                                }
                                else
                                {
                                    trongLuongThanhPhamNhanItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                                    .Select(x => (decimal)x.TrongLuongNhan)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                                }

                            }
                        }


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
                    decimal dinhMucFillet = (tongTrongLuong != 0) ? (tongTLBTPFillet / tongTrongLuong) * 1.006m : 0;
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        datas = listTyLeNameModel,
                        DinhMucFillet = dinhMucFillet.ToString("#,##0.000"),
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
                            .GroupBy(x => new { x.MaThanhPham, x.ThanhPhamName, x.IsTare })
                            .Select(
                                x => new
                                {
                                    MaThanhPham = x.Key.MaThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName,
                                    IsTare = x.Key.IsTare
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
                        //if (thanhPham.IsTare == false)
                        //{
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham && x.IsTare == thanhPham.IsTare)
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
                        //}
                        //else
                        //{
                        //    //lấy định mức từng loài thành phẩm

                        //    //lấy trọng lượng từng loại thành phẩm
                        //    var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham && x.IsTare == thanhPham.IsTare)
                        //    .Select(x => (decimal)x.TrongLuong)
                        //                                .DefaultIfEmpty(0)
                        //                                .Sum();


                        //    //tính phần trăm từng loại thành phẩm
                        //    var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        //    phamTram = Math.Round(phamTram, 2);

                        //    // add phàn trăm vào model
                        //    var newItem = new TyLeNameModel()
                        //    {
                        //        MaTP = thanhPham.MaThanhPham,
                        //        labels = thanhPham.ThanhPhamName,
                        //        percentage = double.Parse(phamTram.ToString()),
                        //        TrongLuongTP = trongLuongThanhPhamItems,
                        //    };
                        //    listTyLeNameModel.Add(newItem);
                        //}


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
        #region Thành Phẩm 2
        public IActionResult DBTongHopTP2PartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeSanPhamTP2WithDate(DateTime fromDate, DateTime toDate)
        {

            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetTongHopSanPhamThanhPham2/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}";
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
                            .GroupBy(x => new { x.MaLoaiThanhPham, x.TenThanhPham })
                            .Select(
                                x => new
                                {
                                    MaLoaiThanhPham = x.Key.MaLoaiThanhPham,
                                    TenThanhPham = x.Key.TenThanhPham
                                })
                            .OrderBy(x => x.MaLoaiThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = itemDycs.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();

                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {


                        var thanhPham = thanhPhams[i];
                        //if (thanhPham.IsTare == false)
                        //{
                        //lấy định mức từng loài thành phẩm

                        //lấy trọng lượng từng loại thành phẩm
                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaLoaiThanhPham == thanhPham.MaLoaiThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();


                        //tính phần trăm từng loại thành phẩm
                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaLoaiThanhPham,
                            labels = thanhPham.TenThanhPham,
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
        #endregion
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

            // danh sách tp dạt xẻ bướm
            var apiTongHopDinhMucXeBuomUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopDinhMucTPXeBuoms/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helperTongHopDinhMucXeBuom = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    //trọng lượng dạt xẻ bướm
                    var itemTongHopDinhMucXeBuom = await helperTongHopDinhMucXeBuom.GetAsync2<object>(HttpContext, apiTongHopDinhMucXeBuomUrl);

                    //var trongLuongDat = itemTongHopDinhMucXeBuom.Cast<dynamic>().Select(x => new { DatXeBuom = (decimal)x.DatXeBuom }).FirstOrDefault();
                    //var dinhMucMocRuot = itemTongHopDinhMucXeBuom.Cast<dynamic>().Select(x => new { DinhMucMocRuot = (decimal)x.DinhMucMocRuot }).FirstOrDefault();

                    var trongLuongDat = itemTongHopDinhMucXeBuom.Cast<dynamic>()
                        .Select(x => new { DatXeBuom = x.DatXeBuom == null ? 0 : (decimal)x.DatXeBuom })
                        .FirstOrDefault();

                    var dinhMucMocRuot = itemTongHopDinhMucXeBuom.Cast<dynamic>()
                        .Select(x => new { DinhMucMocRuot = x.DinhMucMocRuot == null ? 0 : (decimal)x.DinhMucMocRuot })
                        .FirstOrDefault();

                    var trongLuongBanDauChuaMocRuotCuaDatFillet = (trongLuongDat?.DatXeBuom ?? 0) * (dinhMucMocRuot?.DinhMucMocRuot ?? 0);

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
                    var trongLuongBTPFilletKhongDat = tongTrongLuong - (trongLuongDat?.DatXeBuom ?? 0);

                    var tongTrongLuongDaBaoGomDatDaTinhVeDungLuongChuaMocRuot = trongLuongBTPFilletKhongDat + trongLuongBanDauChuaMocRuotCuaDatFillet;
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
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuong.ToString("N2"),
                        TongTLDaQuyVeCuaDatFillet = tongTrongLuongDaBaoGomDatDaTinhVeDungLuongChuaMocRuot.ToString("N2"),
                        DatFillet = (trongLuongDat?.DatXeBuom.ToString("N2")) ?? "0.00",
                        DinhMucMocRuotXeBuom = (dinhMucMocRuot?.DinhMucMocRuot.ToString("N2")) ?? "0.00",
                        TrongLuongBanDauCuaDat = trongLuongBanDauChuaMocRuotCuaDatFillet.ToString("N2"),
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
        public async Task<IActionResult> TongHopTyLeThanhPhamBTPDinhHinhWithDate(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPDinhHinhs/GetTongHopThanhPhamsDB/{fromDate.ToString("yyyy-MM-dd")},{toDate.ToString("yyyy-MM-dd")},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            //var apiSanLuongTPFilletv2Url = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongSanLuongTP/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            //using var helperSanLuongTPFilletv2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            var apiDinhMucLangDaUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetDinhMucLangDa/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helperDinhMucLangDa = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {

                    //Sản Lượng TPFIlletv2
                    //var itemSanLuongTPFilletv2 = await helperSanLuongTPFilletv2.GetAsync2<object>(HttpContext, apiSanLuongTPFilletv2Url);

                    //var sanLuongTPFilletv2 = (itemSanLuongTPFilletv2 ?? new List<object>())
                    //    .Cast<dynamic>()
                    //    .Select(x => new
                    //    {
                    //        MaThanhPham = (string)x.MaThanhPham,
                    //        TrongLuongNhan = (decimal)x.TrongLuongNhan,
                    //        TrongLuongTra = (decimal)x.TrongLuongTra,
                    //        DinhMuc = (decimal)x.DinhMuc
                    //    })
                    //    .ToList();

                    //var trongLuongTraFilletv2 = sanLuongTPFilletv2.Select(x => x.TrongLuongTra).DefaultIfEmpty(0).Sum();


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
                    //var tongTrongLuongCaDa = itemDycs.Select(x => (decimal)(x.TrongLuongCaDa))
                    //                               .DefaultIfEmpty(0)
                    //                               .Sum();
                    //var trongLuongBTPDinhHinhNotCaDa = tongTrongLuong - tongTrongLuongCaDa;

                    var itemDinhMucLangDa = await helperDinhMucLangDa.GetAsync2<object>(HttpContext, apiDinhMucLangDaUrl);
                    var dinhmuclangdas = (itemDinhMucLangDa ?? new List<object>())
                        .Cast<dynamic>()
                        .Select(x => new
                        {
                            TrongLuongTraFL = (string)x.TrongLuongTraFL,
                            TrongLuongCaDa = x.TrongLuongCaDa != null ? (decimal)x.TrongLuongCaDa : 0m,
                            TrongLuongNhanDinhHinh = x.TrongLuongNhanDinhHinh != null ? (decimal)x.TrongLuongNhanDinhHinh : 0m,
                            DinhMucLangDa = x.DinhMucLangDa != null ? (decimal)x.DinhMucLangDa : 0m
                        });
                    var dinhMucLangDa = dinhmuclangdas.Select(x => x.DinhMucLangDa).FirstOrDefault(0);
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
                        DinhMucLangDa = dinhMucLangDa.ToString("#,##0.000"),
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
        public IActionResult DBTongHopThanhPhamBTPXeBuomPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamBTPXeBuomWithDate(DateTime fromDate, DateTime toDate)
        {
            
            // danh sách tp dạt nguyên con btpfillet
           


            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiBTPFilletv2Url = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetSanLuongDatNguyenConBTPFillets/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helperBTPFilletv2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);

            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopThanhPhamBTPXeBuomsDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {

                    var itemTPDatBTPFilletv2 = await helperBTPFilletv2.GetAsync2<object>(HttpContext, apiBTPFilletv2Url);
                    var itemTPDatBTP = itemTPDatBTPFilletv2.Cast<dynamic>()
                        .Select(x => new
                        {
                            MaLoaiThanhPham = (string)x.MaLoaiThanhPham,
                            ThanhPhamName = (string)x.ThanhPhamName,
                            TrongLuong = -(decimal)x.TrongLuong
                        })
                        .ToList();


                    var itemDycs = items.Cast<dynamic>().ToList();
                    var itemsNormalized = itemDycs.Select(x => new
                    {
                        MaLoaiThanhPham = (string)x.MaLoaiThanhPham,
                        ThanhPhamName = (string)x.ThanhPhamName,
                        TrongLuong = (decimal)x.TrongLuong
                    }).ToList();

                    // Gộp hai danh sách
                    var allItems = itemsNormalized.Concat(itemTPDatBTP).ToList();

                    
                    var thanhPhams = allItems.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaLoaiThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaLoaiThanhPham = x.Key.MaLoaiThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaLoaiThanhPham)
                            .ToList();


                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = allItems.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();

                    decimal trongLuongNguyenConNXB = 0;
                    decimal tongTrongLuongNguyenLieuXeBuom = 0;
                    //decimal trongLuongThanhPhamItems = 0;
                    //var dinhMucThanhPhamItems = ;
                    for (int i = 0; i < thanhPhams.Count; i++)
                    {
                        var thanhPham = thanhPhams[i];




                        var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
                        using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                        var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl).Result;
                        if (tpfls != null)
                        {
                            var matchedTP = tpfls.FirstOrDefault(m => (string)m.Ma == (string)thanhPham.MaLoaiThanhPham);
                            if (matchedTP != null)
                            {
                                bool isNguyenConNXB = matchedTP.IsNguyenConNXB == true && matchedTP.IsNguyenLieuXeBuom == true;
                                if (isNguyenConNXB)
                                {
                                    decimal trongLuongNguyenConNXBItem = itemDycs.Where(x => x.MaLoaiThanhPham == thanhPham.MaLoaiThanhPham)
                                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();
                                    trongLuongNguyenConNXB += trongLuongNguyenConNXBItem;
                                }
                            }
                        }

                        var trongLuongThanhPhamItems = allItems.Where(x => x.MaLoaiThanhPham == thanhPham.MaLoaiThanhPham)
                                    .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();




                        var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                        phamTram = Math.Round(phamTram, 2);

                        // add phàn trăm vào model
                        var newItem = new TyLeNameModel()
                        {
                            MaTP = thanhPham.MaLoaiThanhPham,
                            labels = thanhPham.ThanhPhamName,
                            percentage = double.Parse(phamTram.ToString()),
                            TrongLuongTP = trongLuongThanhPhamItems,
                            //DinhMucTP = dinhMucThanhPhamItems
                        };
                        listTyLeNameModel.Add(newItem);

                    }
                    tongTrongLuongNguyenLieuXeBuom = tongTrongLuong - trongLuongNguyenConNXB; // Không tính Nguyên Liệu NguyenCon NXB vào Nguyên Liệu Xẻ Bướm vì nó chỉ hiển thị nhờ dữ liệu ở đây và chuyển thẳng lên Xếp Khuôn
                    listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                    //var jsonData =JsonConvert.SerializeObject(listTyLeNameModel, Formatting.Indented);
                    return Json(new
                    {
                        TongTL = tongTrongLuongNguyenLieuXeBuom.ToString("N2"),
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
        public IActionResult DBTongHopThanhPhamTPXeBuomPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamTPXeBuomWithDate(DateTime fromDate, DateTime toDate)
        {
            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopThanhPhamTPXeBuomsDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);


            // danh sách tp dạt btpfillet
            var apiBTPFilletv2Url = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanBTPFilletv2/GetTongHopThanhPhamDatBTPFillets/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helperBTPFilletv2 = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);


            try
            {
                var listTyLeNameModel = new List<TyLeNameModel>();
                var items = await helper.GetAsync2<object>(HttpContext, apiUrl);
                if (items != null && items.Any())
                {
                    var itemTPDatBTPFilletv2 = await helperBTPFilletv2.GetAsync2<object>(HttpContext, apiBTPFilletv2Url);
                    var itemTPDatBTP = itemTPDatBTPFilletv2.Cast<dynamic>()
                        .Select(x => new
                        {
                            MaLoaiThanhPham = (string)x.MaLoaiThanhPham,
                            ThanhPhamName = (string)x.ThanhPhamName,
                            TrongLuong = -(decimal)x.TrongLuong
                        })
                        .ToList();




                    var itemDycs = items.Cast<dynamic>().ToList();
                    var itemsNormalized = itemDycs.Select(x => new
                    {
                        MaLoaiThanhPham = (string)x.MaLoaiThanhPham,
                        ThanhPhamName = (string)x.ThanhPhamName,
                        TrongLuong = (decimal)x.TrongLuong
                    }).ToList();

                    // Gộp hai danh sách
                    var allItems = itemsNormalized.Concat(itemTPDatBTP).ToList();


                    var thanhPhams = allItems.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaLoaiThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaLoaiThanhPham = x.Key.MaLoaiThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaLoaiThanhPham)
                            .ToList();
                    //Tính tổng trọng lượng của các loại thành phẩm
                    var tongTrongLuong = allItems.Select(x => (decimal)(x.TrongLuong))
                                                   .DefaultIfEmpty(0)
                                                   .Sum();

                    var apiBTPXeBuomUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopThanhPhamBTPXeBuomsDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
                    using var helperBTPXeBuom = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                    var itemBTPXeBuoms = await helperBTPXeBuom.GetAsync2<object>(HttpContext, apiBTPXeBuomUrl);

                    if (itemBTPXeBuoms != null && itemBTPXeBuoms.Any())
                    {
                        var itemDycBTPXeBuoms = itemBTPXeBuoms.Cast<dynamic>().ToList();

                        var thanhPhamBTPXeBuoms = itemDycBTPXeBuoms.Cast<dynamic>()
                            .ToList()
                            .GroupBy(x => new { x.MaLoaiThanhPham, x.ThanhPhamName })
                            .Select(
                                x => new
                                {
                                    MaLoaiThanhPham = x.Key.MaLoaiThanhPham,
                                    ThanhPhamName = x.Key.ThanhPhamName
                                })
                            .OrderBy(x => x.MaLoaiThanhPham)
                            .ToList();


                        var tongTrongLuongThanhPhamBTPXeBuom = itemDycBTPXeBuoms.Select(x => (decimal)(x.TrongLuong))
                                                       .DefaultIfEmpty(0)
                                                       .Sum();
                        decimal trongLuongNguyenConNXBBTPXeBuom = 0;
                        for (int i = 0; i < thanhPhamBTPXeBuoms.Count; i++)
                        {
                            var thanhPham = thanhPhamBTPXeBuoms[i];


                            var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
                            using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                            var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl).Result;
                            if (tpfls != null)
                            {
                                var matchedTP = tpfls.FirstOrDefault(m => (string)m.Ma == (string)thanhPham.MaLoaiThanhPham);
                                if (matchedTP != null)
                                {
                                    bool isNguyenConNXB = matchedTP.IsNguyenConNXB == true && matchedTP.IsNguyenLieuXeBuom == true;
                                    if (isNguyenConNXB)
                                    {
                                        decimal trongLuongNguyenConNXBItem = itemDycBTPXeBuoms.Where(x => x.MaLoaiThanhPham == thanhPham.MaLoaiThanhPham)
                                            .Select(x => (decimal)x.TrongLuong)
                                                        .DefaultIfEmpty(0)
                                                        .Sum();
                                        trongLuongNguyenConNXBBTPXeBuom += trongLuongNguyenConNXBItem;
                                    }
                                }
                            }
                        }

                        var trongLuongBTPXeBuom = tongTrongLuongThanhPhamBTPXeBuom - trongLuongNguyenConNXBBTPXeBuom;










                        decimal trongLuongTPXeBuom = 0; // Đã Móc Ruột
                        decimal trongLuongGiaoXeBuom = 0;
                        decimal trongLuongTPFilletv2s = 0;
                        decimal trongLuongNguyenCon = 0; // Nguyên Con
                        decimal trongLuongNguyenConNXB = 0;
                        //var dinhMucThanhPhamItems = ;
                        for (int i = 0; i < thanhPhams.Count; i++)
                        {
                            var thanhPham = thanhPhams[i];

                            var trongLuongThanhPhamItems = allItems.Where(x => x.MaLoaiThanhPham == thanhPham.MaLoaiThanhPham)
                            .Select(x => (decimal)x.TrongLuong)
                                                        .DefaultIfEmpty(0)
                                                        .Sum();

                            var apiTPFLUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/MaThanhPhamFillets/GetAlls";
                            using var helperTPFL = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
                            var tpfls = helperTPFL.GetAsync<IEnumerable<MaThanhPhamFillet>>(HttpContext, apiTPFLUrl).Result;

                            string labelText = thanhPham.ThanhPhamName;
                            if (tpfls != null)
                            {
                                var matchedTP = tpfls.FirstOrDefault(m => (string)m.Ma == (string)thanhPham.MaLoaiThanhPham);
                                if (matchedTP != null)
                                {
                                    bool isXeBuom = matchedTP.IsXeBuom == true;
                                    bool isNguyenCon = matchedTP.IsNguyenCon == true; // dạt da xấu không móc ruột
                                    bool isNotDat = matchedTP.IsDat != true;
                                    bool isNotNguyenLieuXeBuom = matchedTP.IsNguyenLieuXeBuom != true;
                                    bool isNotGiaoXeBuom = matchedTP.IsGiaoXepKhuon != true;
                                    bool isTPFilletv2 = matchedTP.IsDat == true && matchedTP.IsNguyenCon == false; // dạt xẻ bướm đã móc ruột
                                    bool isNguyenConNXB = matchedTP.IsNguyenConNXB == true; // dữ liệu để nhờ
                                    if (isTPFilletv2)
                                    {
                                        trongLuongTPFilletv2s += trongLuongThanhPhamItems; //TP Cân Chuyển Fillet = Dạt Xẻ Bướm Đã Móc Ruột
                                    }
                                    if (isXeBuom && isNotDat && isNotNguyenLieuXeBuom && isNotGiaoXeBuom && !isNguyenCon && !isNguyenConNXB)
                                    {
                                        trongLuongTPXeBuom += trongLuongThanhPhamItems; //TP Sau Móc Ruột
                                    }

                                    if (isNotGiaoXeBuom == false)
                                    {
                                        trongLuongGiaoXeBuom += trongLuongThanhPhamItems; // Giao Xếp Khuôn
                                    }
                                    if (isNguyenCon)
                                    {
                                        trongLuongNguyenCon += trongLuongThanhPhamItems; // Nguyên Con Chuyển Trả Fillet Không móc ruột
                                    }
                                    if (isNguyenConNXB)
                                    {
                                        trongLuongNguyenConNXB += trongLuongThanhPhamItems; // Nguyên Con NỘI TẠNG, KHÔNG TÍNH VÀO XẺ BƯỚM ĐƯỢC CHUYỂN THẲNG LÊN XẾP KHUÔN

                                        var dmNguyenConNXB = trongLuongNguyenConNXB == 0
                                        ? 0
                                        : trongLuongNguyenConNXBBTPXeBuom / trongLuongNguyenConNXB;

                                        labelText += $" - ĐM: {Math.Round(dmNguyenConNXB, 3)}";
                                    }
                                }
                            }



                            var phamTram = trongLuongThanhPhamItems / (tongTrongLuong) * 100;
                            phamTram = Math.Round(phamTram, 2);

                            // add phàn trăm vào model
                            var newItem = new TyLeNameModel()
                            {
                                MaTP = thanhPham.MaLoaiThanhPham,
                                labels = labelText,
                                percentage = double.Parse(phamTram.ToString()),
                                TrongLuongTP = trongLuongThanhPhamItems,
                                //DinhMucTP = dinhMucThanhPhamItems
                            };
                            listTyLeNameModel.Add(newItem);

                        }
                        listTyLeNameModel = listTyLeNameModel.OrderByDescending(x => x.TrongLuongTP).ToList();
                        var dinhMucNguyenConNXB = trongLuongNguyenConNXB == 0 ? 0 : trongLuongNguyenConNXBBTPXeBuom / trongLuongNguyenConNXB;
                        var tongTrongLuongMocRuot = trongLuongTPXeBuom + trongLuongTPFilletv2s; // trọng lượng đã móc ruột - dạt đã móc ruột

                        var dinhMucTPXeBuom_dm1 = trongLuongTPXeBuom == 0 ? 0 : (trongLuongBTPXeBuom + trongLuongNguyenCon) / trongLuongTPXeBuom; //Định Mức Móc Ruột = (BTPXeBuom(nguyên liệu vào xẻ bướm) - Nguyên Con(dạt da xấu)) / TP Sau Móc Ruột
                        var dinhMucTPXeBuom_dm2 = trongLuongGiaoXeBuom == 0 ? 0 : tongTrongLuongMocRuot / trongLuongGiaoXeBuom; //Định Mức Hao Hụt = (trọng lượng đã móc ruột - dạt đã móc ruột) / giao xếp khuôn
                        var dinhMucXeBuom = (dinhMucTPXeBuom_dm1 * dinhMucTPXeBuom_dm2); //Định Mức Xẻ Bướm = Định Mức Móc Ruột * Định Mức Hao Hụt
                        var dinhMucFindnal = dinhMucXeBuom * 1.006m;
                        return Json(new
                        {
                            TongTL = tongTrongLuongMocRuot.ToString("N2"), //tổng lượng đã móc ruột - dạt đã móc ruột, không tính giao xếp khuôn
                            DinhMucTPXeBuom_dm1 = dinhMucTPXeBuom_dm1.ToString("#,##0.000"), //Định Múc Móc Ruột
                            DinhMucTPXeBuom_dm2 = dinhMucTPXeBuom_dm2.ToString("#,##0.000"), //Định Mức Hao Hụt
                            dinhMucTPXeBuom = dinhMucXeBuom.ToString("#,##0.000"), //Định Mức Xẻ Bướm (định mức công đoạn)
                            dinhMucFindnal = dinhMucFindnal.ToString("#,##0.000"), //Định Mức công đoạn * 1.006(dm cắt tiết)
                            DinhMucNguyenConNXB = dinhMucNguyenConNXB.ToString("#,##0.000"),
                            datas = listTyLeNameModel,
                        });
                    }
                    return Json(new
                    {

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
        public IActionResult DBTongHopThanhPhamTPRaCoiPartialView()
        {
            return PartialView();
        }
        public async Task<IActionResult> TongHopTyLeThanhPhamTPRaCoiWithDate(DateTime fromDate, DateTime toDate)
        {
            var xuongId = HttpContext.Session.GetString("XuongId");
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanRaCois/GetTongHopThanhPhamRaCoisDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
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

                        var trongLuongThanhPhamItems = itemDycs.Where(x => x.MaThanhPham == thanhPham.MaThanhPham)
                        .Select(x => (decimal)x.TrongLuong)
                                                    .DefaultIfEmpty(0)
                                                    .Sum();


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




        public class ChartData
        {
            public string x { get; set; }
            public double? y { get; set; }
            public double? dm { get; set; }
            public double? dmc { get; set; }
            public string? ngay { get; set; }
        }
        public class ChartTyLeThoiGianVaDinhMuc
        {
            public string category { get; set; }
            public double Dat { get; set; }
            public double KhongDat { get; set; }
            public int soRoDat { get; set; }
            public int soRoKhongDat { get; set; }
        }

        public class ChartTyLeThoiGianVaDinhMucTheoChuyen
        {
            public string category { get; set; }
            public string GroupName { get; set; }
            public double TyLeDat { get; set; }
            public double TyLeKhongDat { get; set; }
            public double TyLeKhongDat2 { get; set; }
            public int SoRoDat { get; set; }
            public int SoRoKhongDat { get; set; }
            public int SoRoKhongDat2 { get; set; }
        }
        #region Tổng quan
        public IActionResult DBTongQuanPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongQuanData(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetTongQuanDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }

        public async Task<List<ChartData>> GetTongQuanForChart(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await TongQuanData(fromDate, toDate, xuongId);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();

                var chartData = new List<ChartData>();

                foreach (var item in dataList)
                {
                    chartData.Add(new ChartData
                    {
                        x = item.KV ?? "",
                        y = (double?)item.TLVao ?? 0,
                        dm = (double?)item.TLRa ?? 0
                    });
                }

                return chartData;
            }

            return new List<ChartData>();
        }
        public async Task<ActionResult> DataChartTongQuan(DateTime fromDate, DateTime toDate, string xuongId)
        {
            bool success = true;
            var dataSourceChart = await GetTongQuanForChart(fromDate, toDate, xuongId);

            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart
            };
            return Json(result);
        }

        public IActionResult DBTongQuanAdvanceView()
        {
            return View();
        }


        #endregion
        #region Định Mức Sửa Cá Theo Sản Phâm
        #region Định Hình
        public IActionResult DBDinhMucSuaCaTheoSanPhamPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> DinhMucSuaCaTheoSanPhamData(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopDinhMucTheoSanPham/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }

        public async Task<List<ChartData>> GetDinhMucSuaCaTheoSanPhamForChart(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await DinhMucSuaCaTheoSanPhamData(fromDate, toDate, xuongId);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();

                var chartData = new List<ChartData>();

                foreach (var item in dataList)
                {
                    chartData.Add(new ChartData
                    {
                        x = item.ThanhPhamName ?? "",
                        y = (double?)item.DinhMuc ?? 0,
                        dm = (double?)item.DinhMucChuan ?? 0,
                        ngay = (string?)item.Ngay
                    });
                }

                return chartData;
            }

            return new List<ChartData>();
        }
        public async Task<ActionResult> DataChartDinhMucSuaCaTheoSanPham(DateTime fromDate, DateTime toDate, string xuongId)
        {
            bool success = true;
            var dataSourceChart = await GetDinhMucSuaCaTheoSanPhamForChart(fromDate, toDate, xuongId);

            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart
            };
            return Json(result);
        }

        public IActionResult DBDinhMucSuaCaTheoSanPhamAdvanceView()
        {
            return View();
        }

        #endregion
        #region Fillet
        public IActionResult DBDinhMucFilletTheoSanPhamPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> DinhMucFilletTheoSanPhamData(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopDinhMucTheoSanPham/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }

        public async Task<List<ChartData>> GetDinhMucFilletTheoSanPhamForChart(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await DinhMucFilletTheoSanPhamData(fromDate, toDate, xuongId);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();

                var chartData = new List<ChartData>();

                foreach (var item in dataList)
                {
                    chartData.Add(new ChartData
                    {
                        x = item.ThanhPhamName ?? "",
                        y = (double?)item.DinhMuc ?? 0,
                        dm = (double?)item.DinhMucChuan ?? 0,
                        ngay = (string?)item.Ngay
                    });
                }

                return chartData;
            }

            return new List<ChartData>();
        }
        public async Task<ActionResult> DataChartDinhMucFilletTheoSanPham(DateTime fromDate, DateTime toDate, string xuongId)
        {
            bool success = true;
            var dataSourceChart = await GetDinhMucFilletTheoSanPhamForChart(fromDate, toDate, xuongId);

            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart
            };
            return Json(result);
        }

        public IActionResult DBDinhMucFilletTheoSanPhamAdvanceView()
        {
            return View();
        }
        #endregion
        #endregion
        #region Biểu đồ côt Năng Suất Theo Chuyền

        #region Định Hình
        public IActionResult DBTongHopDinhMucSanLuongTheoChuyenDinhHinhPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopDinhMucSanLuongTheoChuyenDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopNangSuatNhom/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        // Trọng Lượng Trung Bình của nhóm =  (Số lương nhân viên trong nhóm / tổng số nhân viên các nhóm) * Trọng Lương trả trong nhóm
        public async Task<List<ChartData>> GetTongHopDinhMucSanLuongTheoChuyenForChart(DateTime fromDate, DateTime toDate, string xuongId, double kyVong)
        {
            if (kyVong == 0 || kyVong == null)
            {
                kyVong = (double)AppViewModels.AppViewModel.Instance.ChiSoKyVongDH;
            }
            var dataSource = await TongHopDinhMucSanLuongTheoChuyenDinhHinh(fromDate, toDate, xuongId);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();
                var chartData = new List<ChartData>();

                foreach (var item in dataList)
                {
                    chartData.Add(new ChartData
                    {
                        x = item.Nhom ?? "",
                        y = (double?)item.NangSuat ?? 0,
                        dm = (double?)item.TongTrongLuong ?? 0,
                        ngay = (string?)item.Ngay
                    });
                }

                // nhóm lại theo ngày
                var dataByDate = dataList
                    .GroupBy(i => (string?)i.Ngay)
                    .ToList();

                // tính dữ liệu TB
                var averageRows = new List<ChartData>();

                foreach (var group in dataByDate)
                {
                    var ngay = group.Key ?? "";

                    var nhomSet = new HashSet<string>();
                    double tongNangSuat = 0;
                    double tongTrongLuong = 0;

                    foreach (var item in group)
                    {
                        if (item.Nhom != null)
                            nhomSet.Add((string)item.Nhom);

                        tongNangSuat += (double?)item.NangSuat ?? 0;
                        tongTrongLuong += (double?)item.TongTrongLuong ?? 0;
                    }

                    int soNhom = nhomSet.Count;

                    double trungBinhY = soNhom > 0 ? tongNangSuat / soNhom : 0;
                    double trungBinhDM = soNhom * tongTrongLuong;

                    averageRows.Add(new ChartData
                    {
                        x = "Kỳ Vọng",
                        y = kyVong,
                        dm = 0,
                        ngay = ngay
                    });

                    averageRows.Add(new ChartData
                    {
                        x = "TL Trung Bình",
                        y = trungBinhY,
                        dm = trungBinhDM,
                        ngay = ngay
                    });
                }

                // chèn vào đầu danh sách
                chartData.InsertRange(0, averageRows);

                return chartData;
            }

            return new List<ChartData>();
        }
        public async Task<ActionResult> DataChartTongHopDinhMucSanLuongTheoChuyenDinhHinh(DateTime fromDate, DateTime toDate, string xuongId, int kyVong)
        {

            bool success = true;
            var dataSourceChart = await GetTongHopDinhMucSanLuongTheoChuyenForChart(fromDate, toDate, xuongId, kyVong);
            double totalTrongLuong = dataSourceChart?
            .Where(item => item.x != "TL Trung Bình -Tỷ Lệ")
            .Sum(item => item.y ?? 0) ?? 0;
            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart,
                TotalTrongLuong = Math.Round(totalTrongLuong, 3).ToString("N2")
            };
            return Json(result);
        }
        public IActionResult DBTongHopDinhMucSanLuongTheoChuyenDinhHinhAdvanceView()
        {
            return View();
        }
        public IActionResult SetChiSoKyVongDH(decimal kyVong)
        {
            bool success = true;
            AppViewModels.AppViewModel.Instance.SetChiSoKyVongDH(kyVong);
            var result = new
            {
                Success = success,
                Messages = success ? "Đã set chỉ số kỳ vọng." : "Không thể set chỉ số kỳ vọng!",
            };
            return Json(result);
        }
        #endregion
        #region Fillet
        //Fillet
        public IActionResult DBTongHopDinhMucSanLuongTheoChuyenFilletPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopDinhMucSanLuongTheoChuyenFillet(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopNangSuatNhom/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        // Trọng Lượng Trung Bình của nhóm =  (Số lương nhân viên trong nhóm / tổng số nhân viên các nhóm) * Trọng Lương trả trong nhóm
        public async Task<List<ChartData>> GetTongHopDinhMucSanLuongTheoChuyenFilletForChart(DateTime fromDate, DateTime toDate, string xuongId, double kyVong)
        {
            if (kyVong == 0 || kyVong == null)
            {
                kyVong = (double)AppViewModels.AppViewModel.Instance.ChiSoKyVongFL;
            }
            var dataSource = await TongHopDinhMucSanLuongTheoChuyenFillet(fromDate, toDate, xuongId);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();
                var chartData = new List<ChartData>();

                foreach (var item in dataList)
                {
                    chartData.Add(new ChartData
                    {
                        x = item.Nhom ?? "",
                        y = (double?)item.NangSuat ?? 0,
                        dm = (double?)item.TongTrongLuong ?? 0,
                        ngay = (string?)item.Ngay
                    });
                }

                // nhóm lại theo ngày
                var dataByDate = dataList
                    .GroupBy(i => (string?)i.Ngay)
                    .ToList();

                // tính dữ liệu TB
                var averageRows = new List<ChartData>();

                foreach (var group in dataByDate)
                {
                    var ngay = group.Key ?? "";

                    var nhomSet = new HashSet<string>();
                    double tongNangSuat = 0;
                    double tongTrongLuong = 0;

                    foreach (var item in group)
                    {
                        if (item.Nhom != null)
                            nhomSet.Add((string)item.Nhom);

                        tongNangSuat += (double?)item.NangSuat ?? 0;
                        tongTrongLuong += (double?)item.TongTrongLuong ?? 0;
                    }

                    int soNhom = nhomSet.Count;

                    double trungBinhY = soNhom > 0 ? tongNangSuat / soNhom : 0;
                    double trungBinhDM = soNhom * tongTrongLuong;

                    averageRows.Add(new ChartData
                    {
                        x = "Kỳ Vọng",
                        y = kyVong,
                        dm = 0,
                        ngay = ngay
                    });

                    averageRows.Add(new ChartData
                    {
                        x = "TL Trung Bình",
                        y = trungBinhY,
                        dm = trungBinhDM,
                        ngay = ngay
                    });
                }

                // chèn vào đầu danh sách
                chartData.InsertRange(0, averageRows);

                return chartData;
            }

            return new List<ChartData>();
        }
        public async Task<ActionResult> DataChartTongHopDinhMucSanLuongTheoChuyenFillet(DateTime fromDate, DateTime toDate, string xuongId, int kyVong)
        {
            bool success = true;
            var dataSourceChart = await GetTongHopDinhMucSanLuongTheoChuyenFilletForChart(fromDate, toDate, xuongId, kyVong);
            double totalTrongLuong = dataSourceChart?.Sum(item => item.dm ?? 0) ?? 0;
            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart,
                TotalTrongLuong = Math.Round(totalTrongLuong, 3).ToString("N2")
            };
            return Json(result);
        }

        public IActionResult DBTongHopDinhMucSanLuongTheoChuyenFilletAdvanceView()
        {
            return View();
        }

        public IActionResult SetChiSoKyVongFL(decimal kyVong)
        {
            bool success = true;
            AppViewModels.AppViewModel.Instance.SetChiSoKyVongFL(kyVong);
            var result = new
            {
                Success = success,
                Messages = success ? "Đã set chỉ số kỳ vọng." : "Không thể set chỉ số kỳ vọng!",
            };
            return Json(result);
        }
        #endregion
        #endregion

        #region Biểu Đồ Sản Lượng Và Định Mức Từng Loại Thành Phẩm Theo Chuyền
        #region sửa cá
        public IActionResult DBTongHopSanLuongVaDinhMucThanhPhamTheoChuyenDinhHinhPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopSanLuongVaDinhMucThanhPhamTheoChuyenDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopSanLuongAndDinhMucThanhPhamTheoChuyen/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }

        public async Task<List<Dictionary<string, object>>> GetComboChartData_Pivot(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var rawData = await TongHopSanLuongVaDinhMucThanhPhamTheoChuyenDinhHinh(fromDate, toDate, xuongId);

            if (rawData == null || !rawData.Any())
                return new List<Dictionary<string, object>>();

            var list = rawData.Cast<dynamic>().ToList();

            // Group by Nhom
            var pivotResult = list
                .GroupBy(x => (string)x.Nhom)
                .Select(g =>
                {
                    var row = new Dictionary<string, object>
                    {
                        ["Nhom"] = g.Key,
                        ["TongTLTra"] = g.Sum(x => (double)(x.TLTra ?? 0))
                    };

                    foreach (var item in g)
                    {
                        string tpName = ((string)item.ThanhPhamName)?.Trim();
                        if (!string.IsNullOrEmpty(tpName))
                        {
                            string key = $"DinhMuc_{tpName}";
                            if (!row.ContainsKey(key))
                                row[key] = (decimal)(item.DinhMuc ?? 0);
                        }
                    }

                    return row;
                })
                .ToList();

            return pivotResult;
        }
        public async Task<ActionResult> DataChartTongHopSanLuongVaDinhMucThanhPhamTheoChuyenDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {

            var apiSoLuongNvUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetSanLuongTBTPDinhHinh/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helperSoLuongNv = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var itemSoLuong = await helperSoLuongNv.GetAsync2<object>(HttpContext, apiSoLuongNvUrl);


            var soLuongNV = (itemSoLuong ?? new List<object>())
                .Cast<dynamic>()
                .Select(x => new
                {
                    SanLuongTB = x.SanLuongTB != null ? (double)(x.SanLuongTB) : 0
                })
                .ToList();

            double soLuong = soLuongNV.FirstOrDefault()?.SanLuongTB ?? 0;


            var dataSourceChart = await GetComboChartData_Pivot(fromDate, toDate, xuongId);

            var result = new
            {
                Success = true,
                Messages = "Lấy dữ liệu thành công.",
                DataChart = dataSourceChart,
                TotalTrongLuong = Math.Round(soLuong, 3).ToString("N2")
            };

            return Json(result);
        }
        public IActionResult DBTongHopSanLuongVaDinhMucThanhPhamTheoChuyenDinhHinhAdvanceView()
        {
            return View();
        }
        #endregion
        #region Fillet
        public IActionResult DBTongHopSanLuongVaDinhMucThanhPhamTheoChuyenFilletPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopSanLuongVaDinhMucThanhPhamTheoChuyenFillet(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopSanLuongAndDinhMucThanhPhamTheoChuyen/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }

        public async Task<List<Dictionary<string, object>>> GetComboChartData_PivotFillet(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var rawData = await TongHopSanLuongVaDinhMucThanhPhamTheoChuyenFillet(fromDate, toDate, xuongId);

            if (rawData == null || !rawData.Any())
                return new List<Dictionary<string, object>>();

            var list = rawData.Cast<dynamic>().ToList();

            // Group by Nhom
            var pivotResult = list
                .GroupBy(x => (string)x.Nhom)
                .Select(g =>
                {
                    var row = new Dictionary<string, object>
                    {
                        ["Nhom"] = g.Key,
                        ["TongTLTra"] = g.Sum(x => (double)(x.TLTra ?? 0))
                    };

                    foreach (var item in g)
                    {
                        string tpName = ((string)item.ThanhPhamName)?.Trim();
                        if (!string.IsNullOrEmpty(tpName))
                        {
                            string key = $"DinhMuc_{tpName}";
                            if (!row.ContainsKey(key))
                                row[key] = (decimal)(item.DinhMuc ?? 0);
                        }
                    }

                    return row;
                })
                .ToList();

            return pivotResult;
        }
        public async Task<ActionResult> DataChartTongHopSanLuongVaDinhMucThanhPhamTheoChuyenFillet(DateTime fromDate, DateTime toDate, string xuongId)
        {

            var apiSoLuongNvUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetSanLuongTBTPFillet/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helperSoLuongNv = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var itemSoLuong = await helperSoLuongNv.GetAsync2<object>(HttpContext, apiSoLuongNvUrl);


            var soLuongNV = (itemSoLuong ?? new List<object>())
                .Cast<dynamic>()
                .Select(x => new
                {
                    SanLuongTB = x.SanLuongTB != null ? (double)(x.SanLuongTB) : 0
                })
                .ToList();

            double soLuong = soLuongNV.FirstOrDefault()?.SanLuongTB ?? 0;


            var dataSourceChart = await GetComboChartData_PivotFillet(fromDate, toDate, xuongId);

            var result = new
            {
                Success = true,
                Messages = "Lấy dữ liệu thành công.",
                DataChart = dataSourceChart,
                TotalTrongLuong = Math.Round(soLuong, 3).ToString("N2")
            };

            return Json(result);
        }
        public IActionResult DBTongHopSanLuongVaDinhMucThanhPhamTheoChuyenFilletAdvanceView()
        {
            return View();
        }
        #endregion
        #endregion
        #region Tỷ Lệ Thời Gian và đinh mức
        #region Định Hình
        public IActionResult DBTongHopTyLeThoiGianVaDinhMucDinhHinhPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopTongHopTyLeThoiGianVaDinhMucDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopTyLeThoiGianVaDinhMucDinhHinhDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        public async Task<IEnumerable<object>> TongHopTongHopTyLeThoiGianVaDinhMucForGridDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopTyLeThoiGianVaDinhMucForGridDinhHinhDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

            var groupedResult = dataSource
                .Cast<dynamic>()
                .GroupBy(x => (DateTime)x.Ngay)
                .Select(g => new
                {
                    Ngay = g.Key.ToString("yyyy-MM-dd"),
                    SoRoDauDinhMuc = g.FirstOrDefault(x => x.category == "DinhMuc")?.SoRoDat ?? 0,
                    TyLeDauDinhMuc = (g.FirstOrDefault(x => x.category == "DinhMuc")?.TyLeDat ?? 0) * 100,
                    SoRoRotDinhMuc = g.FirstOrDefault(x => x.category == "DinhMuc")?.SoRoKhongDat ?? 0,
                    TyLeRotDinhMuc = (g.FirstOrDefault(x => x.category == "DinhMuc")?.TyLeKhongDat ?? 0) * 100,
                    SoRoDatThoiGian = g.FirstOrDefault(x => x.category == "ThoiGian")?.SoRoDat ?? 0,
                    TyLeDatThoiGian = (g.FirstOrDefault(x => x.category == "ThoiGian")?.TyLeDat ?? 0) * 100,
                    SoRoRotThoiGian = g.FirstOrDefault(x => x.category == "ThoiGian")?.SoRoKhongDat ?? 0,
                    TyLeRotThoiGian = (g.FirstOrDefault(x => x.category == "ThoiGian")?.TyLeKhongDat ?? 0) * 100
                })
                .Select(r => new
                {
                    r.Ngay,
                    r.SoRoDauDinhMuc,
                    TyLeDauDinhMuc = $"{r.TyLeDauDinhMuc:0.00}%",
                    r.SoRoRotDinhMuc,
                    TyLeRotDinhMuc = $"{r.TyLeRotDinhMuc:0.00}%",
                    r.SoRoDatThoiGian,
                    TyLeDatThoiGian = $"{r.TyLeDatThoiGian:0.00}%",
                    r.SoRoRotThoiGian,
                    TyLeRotThoiGian = $"{r.TyLeRotThoiGian:0.00}%"
                })
                .ToList();


            return groupedResult;
        }
        // Trọng Lượng Trung Bình của nhóm =  (Số lương nhân viên trong nhóm / tổng số nhân viên các nhóm) * Trọng Lương trả trong nhóm
        public async Task<List<ChartTyLeThoiGianVaDinhMuc>> GetTongHopTongHopTyLeThoiGianVaDinhMucForChart(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await TongHopTongHopTyLeThoiGianVaDinhMucDinhHinh(fromDate, toDate, xuongId);
            var chartData = new List<ChartTyLeThoiGianVaDinhMuc>();

            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();


                foreach (var item in dataList)
                {
                    double soRoDat = (double)(item.SoRoDat ?? 0);
                    double soRoKhongDat = (double)(item.SoRoKhongDat ?? 0);
                    double dat = soRoDat > 0 ? Math.Round((double)item.TyLeDat * 100, 2) : 0;
                    double khongDat = soRoKhongDat > 0 ? Math.Round((double)item.TyLeKhongDat * 100, 2) : 0;

                    chartData.Add(new ChartTyLeThoiGianVaDinhMuc
                    {
                        category = item.category,
                        Dat = dat,
                        KhongDat = khongDat,
                        soRoDat = (int)soRoDat,
                        soRoKhongDat = (int)soRoKhongDat
                    });
                }
            }

            return chartData;
        }
        public async Task<ActionResult> DataChartTongHopTongHopTyLeThoiGianVaDinhMucDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {
            bool success = true;
            var dataSourceChart = await GetTongHopTongHopTyLeThoiGianVaDinhMucForChart(fromDate, toDate, xuongId);
            double tongSoRo = 0;
            if (dataSourceChart != null && dataSourceChart.Any())
            {
                var firstItem = dataSourceChart.First();
                tongSoRo = (double)(firstItem.soRoDat) + (double)(firstItem.soRoKhongDat);
            }
            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart,
                TotalRo = Math.Round(tongSoRo, 3).ToString("N2")
            };
            return Json(result);
        }
        public IActionResult DBTongHopTyLeThoiGianVaDinhMucDinhHinhAdvanceView()
        {
            return View();
        }
        #endregion
        #region Fillet
        public IActionResult DBTongHopTyLeThoiGianVaDinhMucFilletPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopTongHopTyLeThoiGianVaDinhMucFillet(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopTyLeThoiGianVaDinhMucFilletDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        // Trọng Lượng Trung Bình của nhóm =  (Số lương nhân viên trong nhóm / tổng số nhân viên các nhóm) * Trọng Lương trả trong nhóm
        public async Task<List<ChartTyLeThoiGianVaDinhMuc>> GetTongHopTongHopTyLeThoiGianVaDinhMucFilletForChart(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await TongHopTongHopTyLeThoiGianVaDinhMucFillet(fromDate, toDate, xuongId);
            var chartData = new List<ChartTyLeThoiGianVaDinhMuc>();

            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();


                foreach (var item in dataList)
                {
                    double soRoDat = (double)(item.SoRoDat ?? 0);
                    double soRoKhongDat = (double)(item.SoRoKhongDat ?? 0);
                    double dat = soRoDat > 0 ? Math.Round((double)item.TyLeDat * 100, 2) : 0;
                    double khongDat = soRoKhongDat > 0 ? Math.Round((double)item.TyLeKhongDat * 100, 2) : 0;

                    chartData.Add(new ChartTyLeThoiGianVaDinhMuc
                    {
                        category = item.category,
                        Dat = dat,
                        KhongDat = khongDat,
                        soRoDat = (int)soRoDat,
                        soRoKhongDat = (int)soRoKhongDat
                    });
                }
            }

            return chartData;
        }
        public async Task<ActionResult> DataChartTongHopTongHopTyLeThoiGianVaDinhMucFillet(DateTime fromDate, DateTime toDate, string xuongId)
        {
            bool success = true;
            var dataSourceChart = await GetTongHopTongHopTyLeThoiGianVaDinhMucFilletForChart(fromDate, toDate, xuongId);
            double tongSoRo = 0;
            if (dataSourceChart != null && dataSourceChart.Any())
            {
                var firstItem = dataSourceChart.First();
                tongSoRo = (double)(firstItem.soRoDat) + (double)(firstItem.soRoKhongDat);
            }
            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart,
                TotalRo = Math.Round(tongSoRo, 3).ToString("N2")
            };
            return Json(result);
        }


        public IActionResult DBTongHopTyLeThoiGianVaDinhMucFilletAdvanceView()
        {
            return View();
        }
        public async Task<IEnumerable<object>> TongHopTongHopTyLeThoiGianVaDinhMucForGridFillet(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFilletv2/GetTongHopTyLeThoiGianVaDinhMucForGridFilletDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);

            var groupedResult = dataSource
                .Cast<dynamic>()
                .GroupBy(x => (DateTime)x.Ngay)
                .Select(g => new
                {
                    Ngay = g.Key.ToString("yyyy-MM-dd"),
                    SoRoDauDinhMuc = g.FirstOrDefault(x => x.category == "DinhMuc")?.SoRoDat ?? 0,
                    TyLeDauDinhMuc = (g.FirstOrDefault(x => x.category == "DinhMuc")?.TyLeDat ?? 0) * 100,
                    SoRoRotDinhMuc = g.FirstOrDefault(x => x.category == "DinhMuc")?.SoRoKhongDat ?? 0,
                    TyLeRotDinhMuc = (g.FirstOrDefault(x => x.category == "DinhMuc")?.TyLeKhongDat ?? 0) * 100,
                    SoRoDatThoiGian = g.FirstOrDefault(x => x.category == "ThoiGian")?.SoRoDat ?? 0,
                    TyLeDatThoiGian = (g.FirstOrDefault(x => x.category == "ThoiGian")?.TyLeDat ?? 0) * 100,
                    SoRoRotThoiGian = g.FirstOrDefault(x => x.category == "ThoiGian")?.SoRoKhongDat ?? 0,
                    TyLeRotThoiGian = (g.FirstOrDefault(x => x.category == "ThoiGian")?.TyLeKhongDat ?? 0) * 100
                })
                .Select(r => new
                {
                    r.Ngay,
                    r.SoRoDauDinhMuc,
                    TyLeDauDinhMuc = $"{r.TyLeDauDinhMuc:0.00}%",
                    r.SoRoRotDinhMuc,
                    TyLeRotDinhMuc = $"{r.TyLeRotDinhMuc:0.00}%",
                    r.SoRoDatThoiGian,
                    TyLeDatThoiGian = $"{r.TyLeDatThoiGian:0.00}%",
                    r.SoRoRotThoiGian,
                    TyLeRotThoiGian = $"{r.TyLeRotThoiGian:0.00}%"
                })
                .ToList();


            return groupedResult;
        }
        #endregion
        #endregion

        #region Tỷ Lệ Thời Gian Và Định Mức Theo Chuyền
        #region Định Hình
        public IActionResult DBTongHopTyLeThoiGianVaDinhMucTheoChuyenDinhHinhPartialView()
        {
            return PartialView();
        }

        public async Task<IEnumerable<object>> TongHopTongHopTyLeThoiGianVaDinhMucTheoChuyenDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopTyLeThoiGianVaDinhMucDinhHinhTheoNhomDB/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        // Trọng Lượng Trung Bình của nhóm =  (Số lương nhân viên trong nhóm / tổng số nhân viên các nhóm) * Trọng Lương trả trong nhóm
        public async Task<List<ChartTyLeThoiGianVaDinhMucTheoChuyen>> GetTongHopTongHopTyLeThoiGianVaDinhMucTheoChuyenForChart(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await TongHopTongHopTyLeThoiGianVaDinhMucTheoChuyenDinhHinh(fromDate, toDate, xuongId);
            var chartData = new List<ChartTyLeThoiGianVaDinhMucTheoChuyen>();

            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();


                foreach (var item in dataList)
                {
                    double soRoDat = (double)(item.SoRoDat ?? 0);
                    double soRoKhongDat = (double)(item.SoRoKhongDat ?? 0);
                    double soRoKhongDat2 = (double)(item.SoRoKhongDat2 ?? 0);

                    double tyLeDat = soRoDat > 0 ? Math.Round((double)item.TyLeDat * 100, 2) : 0;
                    double tyLeKhongDat = soRoKhongDat > 0 ? Math.Round((double)item.TyLeKhongDat * 100, 2) : 0;
                    double tyLeKhongDat2 = soRoKhongDat2 > 0 ? Math.Round((double)item.TyLeKhongDat2 * 100, 2) : 0;

                    chartData.Add(new ChartTyLeThoiGianVaDinhMucTheoChuyen
                    {
                        GroupName = item.GroupName ?? "",
                        category = item.category,
                        TyLeDat = tyLeDat,
                        TyLeKhongDat = tyLeKhongDat,
                        TyLeKhongDat2 = tyLeKhongDat2,
                        SoRoDat = (int)soRoDat,
                        SoRoKhongDat = (int)soRoKhongDat,
                        SoRoKhongDat2 = (int)soRoKhongDat2
                    });
                }
            }

            return chartData;
        }
        public async Task<ActionResult> DataChartTongHopTongHopTyLeThoiGianVaDinhMucTheoChuyenDinhHinh(DateTime fromDate, DateTime toDate, string xuongId)
        {
            bool success = true;
            var dataSourceChart = await GetTongHopTongHopTyLeThoiGianVaDinhMucTheoChuyenForChart(fromDate, toDate, xuongId);
            double tongSoRo = 0;
            if (dataSourceChart != null && dataSourceChart.Any())
            {
                var firstItem = dataSourceChart.First();
                tongSoRo = (double)(firstItem.SoRoDat) + (double)(firstItem.SoRoKhongDat) + (double)(firstItem.SoRoKhongDat2);
            }
            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart,
                TotalRo = Math.Round(tongSoRo, 3).ToString("N2")
            };
            return Json(result);
        }
        #endregion
        #endregion

        #region Sản Lượng Sản Phẩm TP Xẻ Bướm
        public IActionResult DBTongSanLuongThanhPhamXeBuomPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongSanLuongThanhPhamXeBuomData(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPFillets/GetTongHopThanhPhamTPXeBuoms/{fromDate:yyyy-MM-dd},{toDate:yyyy-MM-dd},{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }
        // Trọng Lượng Trung Bình của nhóm =  (Số lương nhân viên trong nhóm / tổng số nhân viên các nhóm) * Trọng Lương trả trong nhóm
        public async Task<List<ChartData>> GetTongSanLuongThanhPhamXeBuomForChart(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await TongSanLuongThanhPhamXeBuomData(fromDate, toDate, xuongId);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();

                var groupedData = dataList
                .GroupBy(x => (string)x.ThanhPhamName)
                .Select(g => new ChartData
                {
                    x = g.Key ?? "",
                    y = g.Sum(item => (double?)item.TrongLuong ?? 0),
                    dm = g.Sum(item => (double?)item.SoRo ?? 0)
                })
                .OrderByDescending(cd => cd.y)
                .ToList();

                return groupedData;
            }

            return new List<ChartData>();
        }
        public async Task<ActionResult> DataChartTongSanLuongThanhPhamXeBuom(DateTime fromDate, DateTime toDate, string xuongId)
        {

            bool success = true;
            var dataSourceChart = await GetTongSanLuongThanhPhamXeBuomForChart(fromDate, toDate, xuongId);
            double totalTrongLuong = dataSourceChart?
            .Where(item => item.x != "Xẻ Bướm Giao Xếp Khuôn")
            .Sum(item => item.y ?? 0) ?? 0;
            var result = new
            {
                Success = success,
                Messages = success ? "Lấy dữ liệu thành công." : "Vui lòng kiểm tra lại!",
                DataChart = dataSourceChart,
                TotalTrongLuong = Math.Round(totalTrongLuong, 3).ToString("N2")
            };
            return Json(result);
        }
        public IActionResult DBTongSanLuongThanhPhamXeBuomAdvanceView()
        {
            return View();
        }
        #endregion

        #region Biểu đồ tỷ lệ thu hồi thành phẩm 2
        public IActionResult DBTyLeThuHoiThanhPham2PartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TyLeThuHoiThanhPham2(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanNguyenLieux/GetTyLeThuHoiThanhPham2/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }

        public async Task<List<Dictionary<string, object>>> GetComboChartData_PivotThanhPham2(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var rawData = await TyLeThuHoiThanhPham2(fromDate, toDate, xuongId);

            if (rawData == null || !rawData.Any())
                return new List<Dictionary<string, object>>();

            var list = rawData.Cast<dynamic>().ToList();

            // Group by MaLoaiThanhPham
            var pivotResult = list
                .GroupBy(x => (string)x.MaLoaiThanhPham)
                .Select(g =>
                {
                    var row = new Dictionary<string, object>
                    {
                        ["MaThanhPham"] = g.Key,
                        ["ThanhPhamName"] = g.FirstOrDefault()?.ThanhPhamName ?? "",
                        ["SanLuong"] = g.Sum(x => (double)(x.SanLuong ?? 0))
                    };

                    foreach (var item in g)
                    {
                        double tyLeThuHoi = (double)(item.TyLeThuHoi ?? 0) * 100;
                        if (tyLeThuHoi != 0)
                        {
                            string key = $"TyLeThuHoi_{tyLeThuHoi}";
                            if (!row.ContainsKey(key))
                                row[key] = Math.Round(tyLeThuHoi, 2);
                        }
                    }

                    return row;
                })
                .ToList();

            return pivotResult;
        }

        public async Task<ActionResult> DataChartTyLeThuHoiThanhPham2(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSourceChart = await GetComboChartData_PivotThanhPham2(fromDate, toDate, xuongId);

            // Tính tổng SanLuong
            double totalTrongLuong = dataSourceChart?
                .Where(item => item.ContainsKey("SanLuong"))
                .Sum(item => Convert.ToDouble(item["SanLuong"])) ?? 0;

            var result = new
            {
                Success = true,
                Messages = "Lấy dữ liệu thành công.",
                DataChart = dataSourceChart,
                TotalTrongLuong = Math.Round(totalTrongLuong, 3).ToString("N2")
            };

            return Json(result);
        }
        public IActionResult DBTyLeThuHoiThanhPham2AdvanceView()
        {
            return View();
        }
        #endregion

        #region biểu đồ toonge hợp tỷ lệ tăng trọng theo thành phẩm
        public class ChartComboTyLeTangTrongTheoThanhPhamRaCoi
        {
            public string MaThanhPham { get; set; }
            public string ThanhPhamName { get; set; }
            public double TongTrongLuongVao { get; set; }
            public double TongTrongLuongRa { get; set; }
            public double TyLeTangTrong { get; set; }
            public double DinhMucTangTrong { get; set; }
        }
        public IActionResult DBTongHopTyLeTangTrongTheoThanhPhamRaCoiXepKhuonPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopTyLeTangTrongTheoThanhPhamRaCoiXepKhuon(DateTime ngayNguyenLieu)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanRaCois/GetTongHopTyLeTangTrongCoiRaCoiDB/{ngayNguyenLieu:yyyy-MM-dd}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }


        public async Task<List<ChartComboTyLeTangTrongTheoThanhPhamRaCoi>> GetComboChartData_TongHopTyLeTangTrongRaCoiXepKhuon(DateTime ngayNguyenLieu)
        {
            var dataSource = await TongHopTyLeTangTrongTheoThanhPhamRaCoiXepKhuon(ngayNguyenLieu);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();

                var chartData = new List<ChartComboTyLeTangTrongTheoThanhPhamRaCoi>();

                foreach (var item in dataList)
                {
                    chartData.Add(new ChartComboTyLeTangTrongTheoThanhPhamRaCoi
                    {
                        ThanhPhamName = item.ThanhPhamName ?? "",
                        TongTrongLuongVao = (double?)item.TongTrongLuongVao ?? 0,
                        TongTrongLuongRa = (double?)item.TongTrongLuongRa ?? 0,
                        TyLeTangTrong = (double?)item.TyLeTangTrong ?? 0,
                        DinhMucTangTrong = (double?)item.DinhMucTangTrong ?? 0,
                    });
                }

                return chartData;
            }

            return new List<ChartComboTyLeTangTrongTheoThanhPhamRaCoi>();
        }
        public async Task<ActionResult> DataChartTongHopTyLeTangTrongRaCoiXepKhuon(DateTime ngayNguyenLieu)
        {
            var dataSourceChart = await GetComboChartData_TongHopTyLeTangTrongRaCoiXepKhuon(ngayNguyenLieu);
            var result = new
            {
                Success = true,
                Messages = "Lấy dữ liệu thành công.",
                DataChart = dataSourceChart,
            };

            return Json(result);
        }

        public IActionResult DBTongHopTyLeTangTrongTheoThanhPhamRaCoiXepKhuonAdvanceView()
        {
            return View();
        }
        #endregion


        #region biểu đồ hao hụt thành phẩm sửa cá


        public class ChartTongHopHaoHutThanhPhamSuaCa
        {
            public DateTime Ngay { get; set; }
            public string MaThanhPham { get; set; }
            public decimal TrongLuong { get; set; }
            public string Ten { get; set; }
            public decimal HaoHut { get; set; }
        }
        public IActionResult DBTongHopHaoHutThanhPhamSuaCaPartialView()
        {
            return PartialView();
        }
        public async Task<IEnumerable<object>> TongHopHaoHutThanhPhamSuaCa(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var apiUrl = $"{AppViewModels.AppViewModel.Instance.ApiHostUrl}/api/PhieuCanTPDinhHinhs/GetTongHopHaoHut/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}/{xuongId}";
            using var helper = new Middlewares.MethodRESTFulAPIHelpers(_httpClientFactory);
            var dataSource = await helper.GetAsync<IEnumerable<object>>(HttpContext, apiUrl);
            return dataSource;
        }


        public async Task<List<ChartTongHopHaoHutThanhPhamSuaCa>> GetChartData_TongHopHaoHutThanhPhamSuaCa(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSource = await TongHopHaoHutThanhPhamSuaCa(fromDate, toDate, xuongId);
            if (dataSource != null && dataSource.Any())
            {
                var dataList = dataSource.Cast<dynamic>().ToList();

                var chartData = new List<ChartTongHopHaoHutThanhPhamSuaCa>();

                foreach (var item in dataList)
                {
                    chartData.Add(new ChartTongHopHaoHutThanhPhamSuaCa
                    {
                        Ngay = item.Ngay,
                        MaThanhPham = item.MaThanhPham ?? "",
                        Ten = item.Ten ?? "",
                        TrongLuong = (decimal?)item.TrongLuong ?? 0,
                        HaoHut = (decimal?)item.HaoHut ?? 0
                    });
                }

                return chartData;
            }

            return new List<ChartTongHopHaoHutThanhPhamSuaCa>();
        }
        public async Task<ActionResult> DataChartTongHopHaoHutThanhPhamSuaCa(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dataSourceChart = await GetChartData_TongHopHaoHutThanhPhamSuaCa(fromDate, toDate, xuongId);
            var result = new
            {
                Success = true,
                Messages = "Lấy dữ liệu thành công.",
                DataChart = dataSourceChart,
            };

            return Json(result);
        }

        public IActionResult DBTongHopHaoHutThanhPhamSuaCaAdvanceView()
        {
            return View();
        }
        #endregion
    }
}
