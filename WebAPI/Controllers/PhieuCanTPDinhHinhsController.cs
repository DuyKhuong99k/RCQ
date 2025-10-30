using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Models.Repos.SoketModels;
using ViewModels.Repos.HQ;
using Microsoft.AspNetCore.JsonPatch.Internal;
using AppViewModels;


namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhieuCanTPDinhHinhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanTPDinhHinhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PhieuCanTPDinhHinhs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuCanTPDinhHinh>>> GetPhieuCanTPDinhHinh()
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            return await _context.PhieuCanTPDinhHinh.ToListAsync();
        }

        // GET: api/PhieuCanTPDinhHinhs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhieuCanTPDinhHinh>> GetPhieuCanTPDinhHinh(int id)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var phieuCanTPDinhHinh = await _context.PhieuCanTPDinhHinh.FindAsync(id);

            if (phieuCanTPDinhHinh == null)
            {
                return NotFound();
            }

            return phieuCanTPDinhHinh;
        }


        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTietsFromdateTodate(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPDinhHinh.GetChiTiets<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanVien(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPDinhHinh.GetTongHopNhanViens<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate},{toDate},{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLoaiThanhPhams(DateTime fromDate, DateTime toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhams<object>(fromDate, toDate, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLoaiThanhPhams2(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhams<object>(date1, date2, xuongId);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLoaiThanhPhams2DB(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmDashBoard.ItemTongHopThanhPhamTPDinhHinh.OfType<dynamic>().Where(x => x.MaXuong == xuongId).ToList();// Sử dụng dynamic để cast các object
            return items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopLoaiThanhPhamsByMaTP(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhams<object>(date1, date2, xuongId);
            return items;
        }

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopDanhGiaDinhMucTrenChiTiet(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetChiTiets<object>(date1, date2, xuongId);

            if (AppViewModels.AppViewModel.Instance.ApDungTyLeDauRotQuaMuc)
            {
                var itemsTPDH_TL = Vm.VmPhieuCanTPDinhHinh_TyLe.GetsLast<MaThanhPhamDinhHinh_TyLe>(date2, xuongId);
                var itemsTPDH = Vm.VmThanhPhamDinhHinh.Gets<MaThanhPhamDinhHinh>();
                var listOfThanhPhamId = items.Cast<dynamic>()
                                    .ToList()
                                    .Select(x => x.MaThanhPham)
                                    .Distinct()
                                    .Cast<string>()
                                    .ToList();
                foreach (var thanhPhamId in listOfThanhPhamId)
                {
                    var thanhPhamTyLe = itemsTPDH_TL.FirstOrDefault(
                                           x => x.MaThanhPham == thanhPhamId &&
                                                x.MaXuong == xuongId);
                    var thanhPham = itemsTPDH.FirstOrDefault(x =>
                                           x.Ma == thanhPhamId);
                    if (thanhPham != null)
                    {
                        var tyLeDau = thanhPhamTyLe?.TyLeDau ?? thanhPham.TyLeDinhMucDau;
                        var tyLeRot = thanhPhamTyLe?.TyLeRot ?? thanhPham.TyLeDinhMucRot;
                        _ = items.Cast<dynamic>()
                                            .ToList()
                                            .Where(
                                                x =>
                                                    x.ChiSanLuong == false &&
                                                    x.MaThanhPham == thanhPhamId)
                                            .All(
                                                x =>
                                                {
                                                    var tyLe = (decimal)x.TrongLuongTra <= 0
                                                        ? 0
                                                        : Math.Abs((decimal)x.DinhMuc - (decimal)x.DinhMucChuan) /
                                                          (decimal)x.DinhMucChuan;
                                                    if (x.DanhGia == false)
                                                    {
                                                        if (tyLe > tyLeRot)
                                                        {
                                                            object objValue = 0;
                                                            x.TrongLuongTra = objValue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (tyLe > tyLeDau)
                                                        {
                                                            object objValue = Math.Round(
                                                                (decimal)x.TrongLuongNhan / (decimal)x.DinhMucChuan,
                                                                2);
                                                            x.TrongLuongTra = objValue;
                                                        }
                                                    }

                                                    return true;
                                                });
                    }
                }
            }
            if (AppViewModels.AppViewModel.Instance.IsShowMoney)
            {
                var listOfSanPhamIds = items.Cast<dynamic>()
                                    .ToList()
                                    .Select(x => x.MaSanPham)
                                    .Distinct()
                                    .Cast<string>()
                                    .ToList();
                var listOfDonGias = Vm.VmDG_DonGia.Gets<dynamic>(date2, true);
                var donGias = listOfDonGias.Where(x => listOfSanPhamIds.Contains(x.MaSanPham)).ToList();
                foreach (var item in donGias)
                {
                    if (item != null)
                    {
                        decimal donGia = item.DonGia;
                        decimal donGiaGiaCong = item.DonGiaGiaCong;
                        decimal heSo = item.HeSo;
                        decimal heSoRot = item.HeSoRot;
                        bool isUsedHeSoRot = item.IsUsedHeSoRot;
                        if (isUsedHeSoRot == false) heSoRot = 1;

                        switch (item.MaLoaiDonGia)
                        {
                            case "DG":
                                _ = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.DanhGia == item.DanhGia &&
                                             x.MaSize == item.MaSizeDinhHinh)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGia;
                                            x.ThanhTien = (dynamic)(donGia *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                break;
                            case "SP":
                                _ = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.IsGiaCong == false)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGia;
                                            x.ThanhTien = (dynamic)(donGia *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                _ = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.IsGiaCong == true)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGiaGiaCong;
                                            x.ThanhTien = (dynamic)(donGiaGiaCong *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                break;
                            case "DM":
                                _ = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.DinhMuc >= item.DinhMucDown &&
                                             x.DinhMuc <= item.DinhMucUp &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.ChiSanLuong == false &&
                                             x.IsGiaCong == false)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGia;
                                            x.ThanhTien = (dynamic)(donGia *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                _ = items.Cast<dynamic>()
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.DinhMuc >= item.DinhMucDown &&
                                             x.DinhMuc <= item.DinhMucUp &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.ChiSanLuong == false &&
                                             x.IsGiaCong == true)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGiaGiaCong;
                                            x.ThanhTien = (dynamic)(donGiaGiaCong *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                break;
                        }
                    }
                }
            }

            var dynamicItems = items.Cast<dynamic>().ToList();
            var itemTongHops = (from item in dynamicItems
                                group item by new
                                {
                                    item.MaNhanVien,
                                    item.MaHoSo,
                                    item.TenNhanVien,
                                    item.IsGiaCong,
                                    item.ThanhPhamName,
                                    item.Nhom,
                                    item.MaSanPham
                                }
                                into g
                                select new
                                {
                                    MaHoSo = (string)g.Key.MaHoSo,
                                    MaNhanVien = (string)g.Key.MaNhanVien,
                                    Nhom = (string)g.Key.Nhom,
                                    TenNhanVien = (string)g.Key.TenNhanVien,
                                    IsGiaCong = (bool)g.Key.IsGiaCong,
                                    ThanhPhamName = (string)g.Key.ThanhPhamName,
                                    MaSanPham = ((string)(g.Key.MaSanPham == null ? "" : g.Key.MaSanPham)).Replace(
                                        " ",
                                        "_"),
                                    SoRo = g.Count(),
                                    TrongLuongTra = g.Sum(x => (decimal)x.TrongLuongTra),
                                    TrongLuongTraTyLeOrg = g.Sum(x => (decimal)x.TrongLuongTraTyLeOrg),
                                    TrongLuongChucNang = g.Sum(x => (decimal)x.TrongLuongChucNang),
                                    DinhMuc = g.Average(x => (decimal)x.DinhMuc),
                                    ThanhTien = g.Sum(x => (decimal)x.ThanhTien)
                                }).ToList();



            return itemTongHops;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopDanhGiaDinhMuc(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetTongHopDanhGiaDinhMucs<dynamic>(date1, date2, xuongId);

            if (AppViewModel.Instance.IsShowMoney)
            {
                var listOfSanPhamIds = items
                                    .ToList()
                                    .Select(x => x.MaSanPham)
                                    .Distinct()
                                    .Cast<string>()
                                    .ToList();
                var listOfDonGias = Vm.VmDG_DonGia.Gets<dynamic>(date2, true);
                var donGias = listOfDonGias.Where(x => listOfSanPhamIds.Contains(x.MaSanPham)).ToList();
                foreach (var item in donGias)
                {
                    if (item != null)
                    {
                        decimal donGia = item.DonGia;
                        decimal donGiaGiaCong = item.DonGiaGiaCong;
                        decimal heSo = item.HeSo;
                        decimal heSoRot = item.HeSoRot;
                        bool isUsedHeSoRot = item.IsUsedHeSoRot;
                        if (isUsedHeSoRot == false) heSoRot = 1;

                        switch (item.MaLoaiDonGia)
                        {
                            case "DG":
                                _ = items
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             item.DanhGia == (x.DanhGia != 0) &&
                                             x.MaSize == item.MaSizeDinhHinh)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGia;
                                            x.ThanhTien = (dynamic)(donGia *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                break;
                            case "SP":
                                //_ = items.Cast<dynamic>()
                                //    .ToList()
                                //    .Where(
                                //        x => x.MaSanPham == item.MaSanPham && x.MaSizeDinhHinh == item.MaSize)
                                //    .All(
                                //        x =>
                                //        {
                                //            x.DonGia = (dynamic)item.DonGia;
                                //            x.ThanhTien = (dynamic)(donGia *
                                //                heSo *
                                //                heSoRot *
                                //                ((decimal)x.TrongLuongTra));
                                //            return true;
                                //        });
                                //break;
                                _ = items
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.IsGiaCong == false)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGia;
                                            x.ThanhTien = (dynamic)(donGia *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                _ = items
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.IsGiaCong == true)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGiaGiaCong;
                                            x.ThanhTien = (dynamic)(donGiaGiaCong *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                break;
                            case "DM":
                                //_ = items.Cast<dynamic>()
                                //    .ToList()
                                //    .Where(
                                //        x => x.MaSanPham == item.MaSanPham &&
                                //            x.DinhMuc >= item.DinhMucDown &&
                                //            x.DinhMuc <= item.DinhMucUp && x.MaSizeDinhHinh == item.MaSize && x.ChiSanLuong == false)
                                //    .All(
                                //        x =>
                                //        {
                                //            x.DonGia = (dynamic)item.DonGia;
                                //            x.ThanhTien = (dynamic)(donGia *
                                //                heSo *
                                //                heSoRot *
                                //                ((decimal)x.TrongLuongTra));
                                //            return true;
                                //        });
                                _ = items
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.DinhMuc >= item.DinhMucDown &&
                                             x.DinhMuc <= item.DinhMucUp &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.ChiSanLuong == false &&
                                             x.IsGiaCong == false)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGia;
                                            x.ThanhTien = (dynamic)(donGia *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                _ = items
                                    .ToList()
                                    .Where(
                                        x => x.MaSanPham == item.MaSanPham &&
                                             x.DinhMuc >= item.DinhMucDown &&
                                             x.DinhMuc <= item.DinhMucUp &&
                                             x.MaSize == item.MaSizeDinhHinh &&
                                             x.ChiSanLuong == false &&
                                             x.IsGiaCong == true)
                                    .All(
                                        x =>
                                        {
                                            x.DonGia = item.DonGiaGiaCong;
                                            x.ThanhTien = (dynamic)(donGiaGiaCong *
                                                heSo *
                                                heSoRot *
                                                (decimal)x.TrongLuongTra);
                                            return true;
                                        });
                                break;
                        }
                    }
                }
            }
            var _items = items.ToList().OrderByDescending(x => x.MaHoSo).Cast<object>().ToList();
            return _items;
        }

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanVienPhucVu(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetTongHopNhanVienPhucVus<dynamic>(date1, date2, xuongId);
            var _items = items
                    .ToList()
                    .OrderByDescending(x => x.MaHoSo)
                    .Cast<object>()
                    .ToList();
            return _items;
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopNhanVienBanKiem(string fromDate, string toDate, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetTongHopNhanVienBanKiems<dynamic>(date1, date2, xuongId);
            var _items = items
                    .ToList()
                    .OrderByDescending(x => x.MaHoSo)
                    .Cast<object>()
                    .ToList();
            return _items;
        }

        //[HttpGet("{fromDate}/{toDate}/{xuongId}")]
        //[Authorize]
        //public async Task<ActionResult<IEnumerable<object>>> CommandReLoadTongHopPivot()
        //{
        //    try
        //    {
        //        Application.Current.Dispatcher?.Invoke(
        //            () => { Items.Clear(); });
        //        var items = PhieuCanTPDinhHinhViewModel.Ins
        //            .GetTongHopDanhGiaDinhMucs<dynamic>(
        //                AppViewModel.Ins.FromDate,
        //                AppViewModel.Ins.DateReport,
        //                XiNghiepViewModel.Ins.XiNghiepSelectedItem?.Ma,
        //                AppViewModel.Ins.IsDinhMucBinhThuong,
        //                AppViewModel.Ins.IsCaTraChuyenDoi,
        //                AppViewModel.Ins.IsFloor);
        //        if (AppViewModel.Ins.IsShowMoney)
        //        {
        //            var listOfSanPhamIds = items
        //                .ToList()
        //                .Select(x => x.MaSanPham)
        //                .Distinct()
        //                .Cast<string>()
        //                .ToList();
        //            var listOfDonGias = DG_DonGiaViewModel.Ins
        //                .Gets<dynamic>(AppViewModel.Ins.DateReport, true);
        //            var donGias = listOfDonGias.Where(x => listOfSanPhamIds.Contains(x.MaSanPham)).ToList();

        //            foreach (var item in donGias)
        //                if (item != null)
        //                {
        //                    decimal donGia = item.DonGia;
        //                    decimal donGiaGiaCong = item.DonGiaGiaCong;
        //                    decimal heSo = item.HeSo;
        //                    decimal heSoRot = item.HeSoRot;
        //                    bool isUsedHeSoRot = item.IsUsedHeSoRot;
        //                    if (isUsedHeSoRot == false) heSoRot = 1;

        //                    switch (item.MaLoaiDonGia)
        //                    {
        //                        case "DG":
        //                            _ = items
        //                                .ToList()
        //                                .Where(
        //                                    x => x.MaSanPham == item.MaSanPham &&
        //                                         item.DanhGia == (x.DanhGia != 0) &&
        //                                         x.MaSize == item.MaSizeDinhHinh)
        //                                .All(
        //                                    x =>
        //                                    {
        //                                        x.DonGia = item.DonGia;
        //                                        x.ThanhTien = (dynamic)(donGia *
        //                                            heSo *
        //                                            heSoRot *
        //                                            (decimal)x.TrongLuongTra);
        //                                        return true;
        //                                    });
        //                            break;
        //                        case "SP":
        //                            //_ = items.Cast<dynamic>()
        //                            //    .ToList()
        //                            //    .Where(
        //                            //        x => x.MaSanPham == item.MaSanPham && x.MaSizeDinhHinh == item.MaSize)
        //                            //    .All(
        //                            //        x =>
        //                            //        {
        //                            //            x.DonGia = (dynamic)item.DonGia;
        //                            //            x.ThanhTien = (dynamic)(donGia *
        //                            //                heSo *
        //                            //                heSoRot *
        //                            //                ((decimal)x.TrongLuongTra));
        //                            //            return true;
        //                            //        });
        //                            //break;
        //                            _ = items
        //                                .ToList()
        //                                .Where(
        //                                    x => x.MaSanPham == item.MaSanPham &&
        //                                         x.MaSize == item.MaSizeDinhHinh &&
        //                                         x.IsGiaCong == false)
        //                                .All(
        //                                    x =>
        //                                    {
        //                                        x.DonGia = item.DonGia;
        //                                        x.ThanhTien = (dynamic)(donGia *
        //                                            heSo *
        //                                            heSoRot *
        //                                            (decimal)x.TrongLuongTra);
        //                                        return true;
        //                                    });
        //                            _ = items
        //                                .ToList()
        //                                .Where(
        //                                    x => x.MaSanPham == item.MaSanPham &&
        //                                         x.MaSize == item.MaSizeDinhHinh &&
        //                                         x.IsGiaCong == true)
        //                                .All(
        //                                    x =>
        //                                    {
        //                                        x.DonGia = item.DonGiaGiaCong;
        //                                        x.ThanhTien = (dynamic)(donGiaGiaCong *
        //                                            heSo *
        //                                            heSoRot *
        //                                            (decimal)x.TrongLuongTra);
        //                                        return true;
        //                                    });
        //                            break;
        //                        case "DM":
        //                            //_ = items.Cast<dynamic>()
        //                            //    .ToList()
        //                            //    .Where(
        //                            //        x => x.MaSanPham == item.MaSanPham &&
        //                            //            x.DinhMuc >= item.DinhMucDown &&
        //                            //            x.DinhMuc <= item.DinhMucUp && x.MaSizeDinhHinh == item.MaSize && x.ChiSanLuong == false)
        //                            //    .All(
        //                            //        x =>
        //                            //        {
        //                            //            x.DonGia = (dynamic)item.DonGia;
        //                            //            x.ThanhTien = (dynamic)(donGia *
        //                            //                heSo *
        //                            //                heSoRot *
        //                            //                ((decimal)x.TrongLuongTra));
        //                            //            return true;
        //                            //        });
        //                            _ = items
        //                                .ToList()
        //                                .Where(
        //                                    x => x.MaSanPham == item.MaSanPham &&
        //                                         x.DinhMuc >= item.DinhMucDown &&
        //                                         x.DinhMuc <= item.DinhMucUp &&
        //                                         x.MaSize == item.MaSizeDinhHinh &&
        //                                         x.ChiSanLuong == false &&
        //                                         x.IsGiaCong == false)
        //                                .All(
        //                                    x =>
        //                                    {
        //                                        x.DonGia = item.DonGia;
        //                                        x.ThanhTien = (dynamic)(donGia *
        //                                            heSo *
        //                                            heSoRot *
        //                                            (decimal)x.TrongLuongTra);
        //                                        return true;
        //                                    });
        //                            _ = items
        //                                .ToList()
        //                                .Where(
        //                                    x => x.MaSanPham == item.MaSanPham &&
        //                                         x.DinhMuc >= item.DinhMucDown &&
        //                                         x.DinhMuc <= item.DinhMucUp &&
        //                                         x.MaSize == item.MaSizeDinhHinh &&
        //                                         x.ChiSanLuong == false &&
        //                                         x.IsGiaCong == true)
        //                                .All(
        //                                    x =>
        //                                    {
        //                                        x.DonGia = item.DonGiaGiaCong;
        //                                        x.ThanhTien = (dynamic)(donGiaGiaCong *
        //                                            heSo *
        //                                            heSoRot *
        //                                            (decimal)x.TrongLuongTra);
        //                                        return true;
        //                                    });
        //                            break;
        //                    }
        //                }
        //        }

        //        var _items = items
        //            .ToList()
        //            .OrderByDescending(x => x.MaHoSo)
        //            .Cast<object>()
        //            .ToList();
        //        var dataTables = _items.ToDataTable();
        //        Application.Current.Dispatcher?.Invoke(
        //            () =>
        //            {
        //                PMSMSharedv1.ViewModel.BaoCaoViewModel.Ins.ReportDataTable = dataTables.Clone();
        //                //PMSMSharedv1.ViewModel.BaoCaoViewModel.Ins.ReportDataTable =
        //                //    _items.ToDataTable();
        //                //Items.Clear();
        //                //Items.AddRange(_items);
        //            });

        //        var numItem = dataTables.Rows.Count;
        //        var numSkip = 0;
        //        var numTake = 1000;
        //        var numLoop = 1 + numItem / numTake;
        //        for (var i = 0; i < numLoop; i++)
        //        {
        //            numSkip = numTake * i;
        //            Application.Current.Dispatcher?.Invoke(
        //                () =>
        //                {
        //                    var items = dataTables.AsEnumerable().Skip(numSkip).Take(numTake);
        //                    foreach (var dataRow in items)
        //                        PMSMSharedv1.ViewModel.BaoCaoViewModel.Ins.ReportDataTable.Rows.Add(
        //                            dataRow.ItemArray);
        //                });
        //            await Task.Delay(50);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //        //throw;
        //    }

        //}



        #region Xử Lý Phiếu Cân
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public IActionResult GetAllsWithDateAndXuong(DateTime dateTime, string xuongId)
        {
            var items = _context.PhieuCanTPDinhHinh.Where(x => x.Ngay == dateTime && x.MaXuong == xuongId).OrderByDescending(x => x.Ngay).ToList();

            return Ok(items);
        }
        [HttpGet("{dateTime}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanDinhHinh_XLPC(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetPhieuCanDinhHinh_XLPC<object>(date1, xuongId);
            return items;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Insert_XLPC(Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPDinhHinh>(dataT.Item1);
            //kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            // ... kiểm tra mã nhân viên đã tồn tại chưa ...
            if (_context.PhieuCanTPDinhHinh.Any(u => u.STT == model.STT && u.Ngay == model.Ngay && u.MaXuong == model.MaXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này đã tồn tại.",
                });
            }
            var newItem = new PhieuCanTPDinhHinh
            {
                STT = model.STT,
                STTBTP = model.STTBTP,
                MaLoaiCa = model.MaLoaiCa,
                CaTra = model.CaTra,
                DinhMucThucTe = model.DinhMucThucTe,
                Gio = model.Gio,
                Ngay = model.Ngay,
                MaLo = model.MaLo,
                MaSize = model.MaSize,
                MaMau = model.MaMau,
                MaThe = model.MaThe,
                TrongLuongNhan = model.TrongLuongNhan,
                MaXuong = model.MaXuong,
                MaThanhPham = model.MaThanhPham,
                MaUserCan = model.MaUserCan,
                MaMayCanBTP = model.MaMayCanBTP,
                MaMayCan = model.MaMayCan,
                MaNhanVien = model.MaNhanVien,
                DinhMucYeuCau = model.DinhMucYeuCau,
                SuDung = model.SuDung,
                ChiSanLuong = model.ChiSanLuong,
                TrongLuongTare = model.TrongLuongTare,
                TrongLuongTra = model.TrongLuongTra,
                TrongLuongBu = model.TrongLuongBu,
                IsOffline = model.IsOffline,
                GhiChu = model.GhiChu,
                MaNhanVienPhucVu = model.MaNhanVienPhucVu

            };
            _context.PhieuCanTPDinhHinh.Add(newItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu." + ex.Message.ToString(),
                    Errors = new List<string> { ex.Message.ToString() }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm thành công!"
            });
        }
        [HttpGet("{stt}/{ngay}/{maNhanVien}/{maMayCan}/{maXuong}")]
        [Authorize]
        public IActionResult GetsByMa(int stt, string ngay, string maNhanVien, string maMayCan, string maXuong)
        {
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = _context.PhieuCanTPDinhHinh.FirstOrDefault(x => x.STT == stt && x.Ngay == ngayConvert && x.MaNhanVien == maNhanVien && x.MaMayCan == maMayCan && x.MaXuong == maXuong);
            if (item == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Mã này không tồn tại!."
                });
            }
            return Ok(item);
        }

        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Update_XLPC(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPDinhHinh>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaLoaiCa,
                item.CaTra,
                item.DinhMucThucTe,
                item.Gio,
                item.MaLo,
                item.MaSize,
                item.MaMau,
                item.TrongLuongNhan,
                item.MaThanhPham,
                item.MaNhanVien,
                item.MaNhanVienPhucVu,
                item.TrongLuongTra,
                item.GhiChu
            }, options);

            item.MaLoaiCa = model.MaLoaiCa;
            item.CaTra = model.CaTra;
            item.DinhMucThucTe = model.DinhMucThucTe;
            item.Gio = model.Gio;
            item.MaLo = model.MaLo;
            item.MaSize = model.MaSize;
            item.MaMau = model.MaMau;
            item.TrongLuongNhan = model.TrongLuongNhan;
            item.MaThanhPham = model.MaThanhPham;
            item.MaNhanVien = model.MaNhanVien;
            item.MaNhanVienPhucVu = model.MaNhanVienPhucVu;
            item.TrongLuongTra = model.TrongLuongTra;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete_DinhHinh(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var oldData = JsonSerializer.Serialize(new
            {
                item.STT,
                item.STTBTP,
                item.TrongLuongNhan,
                item.TrongLuongTra,
                item.GhiChu
            }, options);

            var newItem = new PhieuCanTPDinhHinh
            {
                STT = model.STT,
                STTBTP = model.STTBTP,
                MaLoaiCa = item.MaLoaiCa,
                CaTra = item.CaTra,
                DinhMucThucTe = item.DinhMucThucTe,
                Gio = item.Gio,
                Ngay = item.Ngay,
                MaLo = item.MaLo,
                MaSize = item.MaSize,
                MaMau = item.MaMau,
                MaThe = item.MaThe,
                TrongLuongNhan = model.TrongLuongNhan,
                MaXuong = item.MaXuong,
                MaThanhPham = item.MaThanhPham,
                MaUserCan = item.MaUserCan,
                MaMayCanBTP = item.MaMayCanBTP,
                MaMayCan = item.MaMayCan,
                MaNhanVien = item.MaNhanVien,
                DinhMucYeuCau = item.DinhMucYeuCau,
                SuDung = item.SuDung,
                ChiSanLuong = item.ChiSanLuong,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuongTra = model.TrongLuongTra,
                TrongLuongBu = item.TrongLuongBu,
                IsOffline = item.IsOffline,
                MaNhanVienPhucVu = item.MaNhanVienPhucVu,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanTPDinhHinh.Add(newItem);
                    _context.PhieuCanTPDinhHinh.Remove(item);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> Delete_XLPC(int stt, string ngay, string maMayCan, string maXuong)
        {
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Bạn chưa chọn thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            _context.PhieuCanTPDinhHinh.Remove(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xóa.",
                    Errors = new List<string> { ex.Message }
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đã xoá!"
            });
        }

        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenXuong_DinhHinh(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var oldData = JsonSerializer.Serialize(new
            {
                item.MaXuong
            }, options);

            var newItem = new PhieuCanTPDinhHinh
            {
                STT = item.STT,
                STTBTP = item.STTBTP,
                MaLoaiCa = item.MaLoaiCa,
                CaTra = item.CaTra,
                DinhMucThucTe = item.DinhMucThucTe,
                Gio = item.Gio,
                Ngay = item.Ngay,
                MaLo = item.MaLo,
                MaSize = item.MaSize,
                MaMau = item.MaMau,
                MaThe = item.MaThe,
                TrongLuongNhan = item.TrongLuongNhan,
                MaXuong = model.MaXuong,
                MaThanhPham = item.MaThanhPham,
                MaUserCan = item.MaUserCan,
                MaMayCanBTP = item.MaMayCanBTP,
                MaMayCan = item.MaMayCan,
                MaNhanVien = item.MaNhanVien,
                DinhMucYeuCau = item.DinhMucYeuCau,
                SuDung = item.SuDung,
                ChiSanLuong = item.ChiSanLuong,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuongTra = item.TrongLuongTra,
                TrongLuongBu = item.TrongLuongBu,
                IsOffline = item.IsOffline,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanTPDinhHinh.Add(newItem);
                    _context.PhieuCanTPDinhHinh.Remove(item);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize_DinhHinh(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPDinhHinh>(dataT.Item1);
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }

            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var oldData = JsonSerializer.Serialize(new
            {
                item.MaSize
            }, options);

            var newItem = new PhieuCanTPDinhHinh
            {
                STT = item.STT,
                STTBTP = item.STTBTP,
                MaLoaiCa = item.MaLoaiCa,
                CaTra = item.CaTra,
                DinhMucThucTe = item.DinhMucThucTe,
                Gio = item.Gio,
                Ngay = item.Ngay,
                MaLo = item.MaLo,
                MaSize = model.MaSize,
                MaMau = item.MaMau,
                MaThe = item.MaThe,
                TrongLuongNhan = item.TrongLuongNhan,
                MaXuong = item.MaXuong,
                MaThanhPham = item.MaThanhPham,
                MaUserCan = item.MaUserCan,
                MaMayCanBTP = item.MaMayCanBTP,
                MaMayCan = item.MaMayCan,
                MaNhanVien = item.MaNhanVien,
                DinhMucYeuCau = item.DinhMucYeuCau,
                SuDung = item.SuDung,
                ChiSanLuong = item.ChiSanLuong,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuongTra = item.TrongLuongTra,
                TrongLuongBu = item.TrongLuongBu,
                IsOffline = item.IsOffline,
                GhiChu = item.GhiChu + "," + oldData + "," + model.GhiChu

            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.PhieuCanTPDinhHinh.Add(newItem);
                    _context.PhieuCanTPDinhHinh.Remove(item);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenSize_XLPC(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPDinhHinh>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaSize
            }, options);
            item.MaSize = model.MaSize;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        [HttpPost("{stt}/{ngay}/{maMayCan}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ChuyenThanhPham_XLPC(int stt, string ngay, string maMayCan, string maXuong, Tuple<string> dataT)
        {
            var model = JsonSerializer.Deserialize<PhieuCanTPDinhHinh>(dataT.Item1);
            // Kiểm tra xem ID người dùng được cập nhật có hợp lệ không
            if (string.IsNullOrEmpty(stt.ToString()) || string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maMayCan) || string.IsNullOrEmpty(maXuong))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Vui lòng cung cấp đủ thông tin!."
                });
            }
            DateTime ngayConvert = DateTime.ParseExact(ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var item = await _context.PhieuCanTPDinhHinh.FirstOrDefaultAsync(x => x.STT == stt && x.Ngay == ngayConvert && x.MaMayCan == maMayCan && x.MaXuong == maXuong);

            if (item == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Mục không tồn tại."
                });
            }

            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không và trả về danh sách lỗi nếu có.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ.",
                    Errors = errors
                });
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Lưu trạng thái cũ của đối tượng trước khi thay đổi
            var oldData = JsonSerializer.Serialize(new
            {
                item.MaThanhPham
            }, options);
            item.MaThanhPham = model.MaThanhPham;
            item.GhiChu = item.GhiChu + "," + oldData.ToString() + "," + model.GhiChu;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thông tin thành công!"
            });
        }
        #endregion

        #region Tính Lương
        [HttpGet("{dateTime}/{xuongId}/{khuVucId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopTinhLuongs2(string dateTime, string xuongId, string khuVucId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetPhieuCanTongHopTinhLuong2<object>(date1, xuongId, khuVucId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{khuVucId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHopTinhLuong(string dateTime, string xuongId, string khuVucId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.GetPhieuCanTongHopTinhLuong<object>(date1, xuongId, khuVucId);
            return items;
        }

        [HttpGet("{dateTime}/{xuongId}/{khuVucId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadPhieuCanTongHopTinhLuong(string dateTime, string xuongId, string khuVucId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanTPDinhHinh.LoadPhieuCanTongHopTinhLuong(date1, xuongId, khuVucId);
            return items;
        }

        [HttpPost("{dateTime}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> ExecuteTPSoftAction(string dateTime, string maXuong)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            // Tạo một tham chiếu đến SignalR Hub
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            try
            {
                // Giả lập báo cáo tiến trình bằng SignalR
                await hubContext.Clients.All.SendAsync("ReceiveProgress", "Khởi tạo TPSoft");
                await Task.Delay(50);

                var phieuCans = Vm.VmPhieuCanTPDinhHinh.GetsTPSoft(date1, maXuong);
                var phieuCanTPSoftServer = Vm.VmPMS_Record.Gets<BravoModelV1.EF.PMS_DataRecord>(date1, "01", maXuong);
                var idTPSofts = phieuCanTPSoftServer.Select(x => x.ID)
                    .DefaultIfEmpty(string.Empty)
                    .Distinct()
                    .ToList();

                var insItems = phieuCans.Where(x => !idTPSofts.Contains(x.ID)).ToList();
                var udaItems = phieuCans.Where(x => idTPSofts.Contains(x.ID)).ToList();
                udaItems.ForEach(x => x.DateSync = new DateTime(1900, 01, 01, 0, 0, 0));

                if (insItems.Any())
                {
                    var batches = Vm.VmPMS_Record.GetSqlsInBatches(insItems);
                    var rows = 0;

                    await hubContext.Clients.All.SendAsync("ReceiveProgress", "Bắt đầu thêm");
                    await Task.Delay(50);

                    foreach (var batche in batches)
                    {
                        rows += Vm.VmPMS_Record.Execute(batche);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Đã thực hiện {rows}/{insItems.Count}");
                        await Task.Delay(50);
                    }
                }

                if (udaItems.Any())
                {
                    var batches = Vm.VmPMS_Record.GetSqlsInBatches(udaItems);
                    var rows = 0;

                    await hubContext.Clients.All.SendAsync("ReceiveProgress", "Bắt đầu cập nhật");
                    await Task.Delay(50);
                    Vm.VmPMS_Record.Delete(udaItems.Select(x => x.ID).ToList());

                    foreach (var batche in batches)
                    {
                        rows += Vm.VmPMS_Record.Execute(batche);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Đã thực hiện {rows}/{udaItems.Count}");
                        await Task.Delay(50);
                    }
                }

                await hubContext.Clients.All.SendAsync("ReceiveProgress", "Thực hiện xong!");
                await Task.Delay(500);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $"Có lỗi xảy ra: {ex.Message}");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        [HttpPost("{dateTime}/{maXuong}")]
        [Authorize]
        public async Task<IActionResult> KetChuyen(string dateTime, string xuongId, Tuple<string> dataT)
        {

            try
            {
                string listPhieuCanTongHopDinhHinh = dataT.Item1;
                string[] jsonStrings = listPhieuCanTongHopDinhHinh.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                List<BravoModelV1.Model.PhieuCanTongHopTinhLuong> phieuCanTongHopTinhLuongSelectedItems = new List<BravoModelV1.Model.PhieuCanTongHopTinhLuong>();
                foreach (string jsonString in jsonStrings)
                {
                    var obj = JsonSerializer.Deserialize<BravoModelV1.Model.PhieuCanTongHopTinhLuong>(jsonString);
                    phieuCanTongHopTinhLuongSelectedItems.Add(obj);
                }

                var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                var phieuCanTongHopTinhLuongs = new List<BravoModelV1.Model.PhieuCanTongHopTinhLuong>();
                foreach (var phieuCanTongHopTinhLuongSelectedItem in phieuCanTongHopTinhLuongSelectedItems)
                {
                    phieuCanTongHopTinhLuongs.Add((BravoModelV1.Model.PhieuCanTongHopTinhLuong)phieuCanTongHopTinhLuongSelectedItem);
                }

                var items = Vm.VmPhieuCanTPDinhHinh.GetPhieuCanDinhHinhs(date1, phieuCanTongHopTinhLuongs);
                //var rl = MessageBox.Show($@"Thực hiện thao tác chuyển dữ liệu lên CSDL tính lương trên {items.Count} dòng dữ liệu?",
                //    "Thông Báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                //if (rl == MessageBoxResult.Yes)
                //{
                try
                {
                    //PhieuCanVm.IsBusy = true;
                    bool isF = false;
                    var sanPhamIds = items.Select(x => x.MaSanPham).Distinct().ToList();
                    if (sanPhamIds != null)
                    {
                        foreach (var item in sanPhamIds)
                        {
                            var log = Vm.VmLogKetChuyen.Get(date1, xuongId, item, 1);
                            if (log != null)
                            {
                                isF = true;
                                break;
                            }
                        }
                    }

                    if (isF == false)
                    {
                        var rows = 0;
                        await Task.Delay(1000);
                        await Task.Run(
                        () =>
                        {
                            rows = Vm.VmPhieuCanTPDinhHinh.InsertTL(items);
                            foreach (var item in sanPhamIds)
                            {
                                Vm.VmLogKetChuyen.Insert(
                                        new LogKetChuyenBravo()
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1,
                                            tab = 1,
                                            NgayChuyen = DateTime.Now
                                        });
                            }
                        });
                        await Task.Delay(1000);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");
                        //MessageBox.Show($@"Đã thực hiện trên {rows} dòng dữ liêu");

                    }
                    else
                    {
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Thao tác đã thực hiện, Không thể thực hiện lại!");
                        //MessageBox.Show("Thao tác đã thực hiện, Không thể thực hiện lại!");
                    }

                    //PhieuCanVm.IsBusy = false;
                    await ExecuteTPSoftAction(dateTime, xuongId);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thành công!"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }
            //}
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> SanLuongNhanVienKiem(string dateTime, string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.SanLuongNhanVienKiem(date1, xuongId);
            return items;
        }
        [HttpGet("{xuongId}")]
        [Authorize]
        public IActionResult GetNhomKiems(string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmNhanVien.GetNhomKiems(xuongId);
            return Ok(items);
        }
        [HttpGet("{dateTime}/{xuongId}/{type}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> ReloadDanhSachMacDinhCaiDatNhanVienTL(string dateTime, string xuongId, int type)
        {
            if (_context.NhanVienDaiThanh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.ReloadDanhSachMacDinhCaiDatNhanVienTL(date1, xuongId, type);
            return items;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> ReloadDanhSachMacDinhCaiDatNhanVienTL2()
        {
            if (_context.NhanVienDaiThanh == null)
            {
                return NotFound();
            }
            var items = Vm.VmNhanVien.NhanViensKiems;
            return items;
        }
        [HttpGet("{nhomKiem}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadGridNhanVienDaiThanhTo(string nhomKiem)
        {
            if (_context.NhanVienDaiThanh == null)
            {
                return NotFound();
            }
            var items = Vm.VmNhanVien.LoadGridNhanVienDaiThanhTo(nhomKiem);
            return items;
        }

        [HttpPost("{nhomKiem}")]
        [Authorize]
        public async Task<IActionResult> MoveTo(string nhomKiem, Tuple<string> dataT)
        {
            try
            {
                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmNhanVien.MoveTo(nhomKiem, dataT);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                }); ;
            }

        }
        [HttpPost("{nhomKiem}")]
        [Authorize]
        public async Task<IActionResult> RemoveTo(string nhomKiem, Tuple<string> dataT)
        {
            try
            {
                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmNhanVien.RemoveTo(nhomKiem, dataT);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                }); ;
            }

        }
        [HttpPost("{dateTime}/{xuongId}/{type}")]
        [Authorize]
        public async Task<IActionResult> SaveCaiDatNhanVien(string dateTime, string xuongId, int type)
        {
            try
            {
                if (_context.DanhSachToKiem == null)
                {
                    return NotFound();
                }
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                Vm.VmNhanVien.SaveCaiDatNhanVienKiem(date1, xuongId, type);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                }); ;
            }

        }

        [HttpPost("{nhomKiem}")]
        [Authorize]
        public async Task<IActionResult> AddNhanVienDaiThanhTo(string nhomKiem, Tuple<string> dataT)
        {
            try
            {
                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmNhanVien.AddNhanVienDaiThanhTo(nhomKiem, dataT);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                }); ;
            }

        }
        public async Task<IActionResult> KetChuyenSanLuongKiem(string dateTime, string xuongId, Tuple<string> dataT)
        {
            try
            {
                var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                var sanPhamBravo = Vm.VmSanPhamBravo.GetSanPhamKiem();

                string listSanLuongNhanVienKiemSelectedItems = dataT.Item1;
                string[] sanLuongNhanVienKiems = listSanLuongNhanVienKiemSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var sanLuongNhanVienKiemSelectedItems = new List<BravoModelV1.Model.SanLuongNhanVienKiem>();
                foreach (var sanLuongNhanVienKiemSelectedItem in sanLuongNhanVienKiems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = sanLuongNhanVienKiemSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var sanLuongNhanVienKiem = JsonSerializer.Deserialize<BravoModelV1.Model.SanLuongNhanVienKiem>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (sanLuongNhanVienKiem != null)
                    {
                        sanLuongNhanVienKiemSelectedItems.Add((BravoModelV1.Model.SanLuongNhanVienKiem)sanLuongNhanVienKiem);
                    }
                }
                var _items = sanLuongNhanVienKiemSelectedItems.Cast<BravoModelV1.Model.SanLuongNhanVienKiem>().ToList();
                var items = Vm.VmPhieuCanTPDinhHinh.GetPhieuCanKiemDinhHinhs(_items, sanPhamBravo, 2, date1);
                try
                {
                    //PhieuCanVm.IsBusy = true;
                    bool isF = false;
                    //foreach(var phieuCanDinhHinh in itemsP)
                    //{
                    //    var ps = items.Where(x => x.MaNhanVien == phieuCanDinhHinh.MaNhanVien);
                    //    if(ps.Any())
                    //    {
                    //        isF = true;
                    //        break;
                    //    }
                    //}
                    var sanPhamIds = items.Select(x => x.MaSanPham).Distinct().ToList();
                    if (sanPhamIds != null)
                    {
                        foreach (var item in sanPhamIds)
                        {
                            var log = Vm.VmLogKetChuyen.Get(date1, xuongId, item, 2);
                            if (log != null)
                            {
                                isF = true;
                                break;
                            }
                        }
                    }

                    if (isF == false)
                    {
                        var rows = 0;
                        await Task.Delay(1000);
                        await Task.Run(
                            () =>
                            {
                                rows = Vm.VmPhieuCanTPDinhHinh.Insert(items);
                                foreach (var item in sanPhamIds)
                                {
                                    Vm.VmLogKetChuyen.Insert(
                                        new LogKetChuyenBravo()
                                        {
                                            Gio = DateTime.Now.TimeOfDay,
                                            MaSanPham = item,
                                            MaXuong = xuongId,
                                            Ngay = date1.Date,
                                            tab = 2,
                                            NgayChuyen = DateTime.Now
                                        });
                                }
                            });
                        await Task.Delay(1000);
                        // MessageBox.Show($@"Đã thực hiện trên {rows} dòng dữ liêu");
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");

                    }
                    else
                    {
                        //MessageBox.Show("Thao tác đã thực hiện, Không thể thực hiện lại!");
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Thao tác đã thực hiện, Không thể thực hiện lại!");

                    }
                    //return Ok(new ApiResponse
                    //{
                    //    Success = true,
                    //    Message = "Thành công!"
                    //});
                    //PhieuCanVm.IsBusy = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi.",
                        Errors = new List<string> { ex.Message }
                    });
                }


                //else
                //{
                //    //MessageBox.Show("Không có dữ liệu");
                //    await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Không có dữ liệu!");

                //}
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        #region tính lương phục vụ
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> ReloadSanLuongPhucVu(string dateTime, string xuongId)
        {
            if (_context.NhanVienDaiThanh == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.ReloadSanLuongPhucVu(date1, xuongId);
            return items;
        }
        [HttpGet("{xuongId}")]
        [Authorize]
        public IActionResult GetNhomKiemsWithType2(string xuongId)
        {
            if (_context.PhieuCanTPDinhHinh == null)
            {
                return NotFound();
            }
            var items = Vm.VmNhanVien.GetNhomKiemsWithType2(xuongId);
            return Ok(items);
        }

        public async Task<IActionResult> KetChuyenSanLuongPhucVu(string dateTime, string xuongId, Tuple<string> dataT)
        {
            var hubContext = HttpContext.RequestServices.GetService<IHubContext<Hub.ProgressHub>>();
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            try
            {

                string listSanLuongNhanVienKiemSelectedItems = dataT.Item1;
                string[] sanLuongNhanVienKiems = listSanLuongNhanVienKiemSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var sanLuongNhanVienKiemSelectedItems = new List<BravoModelV1.Model.SanLuongNhanVienKiem>();
                foreach (var sanLuongNhanVienKiemSelectedItem in sanLuongNhanVienKiems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = sanLuongNhanVienKiemSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var sanLuongNhanVienKiem = JsonSerializer.Deserialize<BravoModelV1.Model.SanLuongNhanVienKiem>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (sanLuongNhanVienKiem != null)
                    {
                        sanLuongNhanVienKiemSelectedItems.Add((BravoModelV1.Model.SanLuongNhanVienKiem)sanLuongNhanVienKiem);
                    }
                }
                var _items = sanLuongNhanVienKiemSelectedItems.Cast<BravoModelV1.Model.SanLuongNhanVienKiem>().ToList();
                var sanPhamBraVo = Vm.VmSanPhamBravo.GetSanPhamPhucVu();
                var items = Vm.VmPhieuCanTPDinhHinh.GetPhieuCanKiemDinhHinhs(_items, sanPhamBraVo, 5, date1);
                try
                {
                    var itemsP = Vm.VmPhieuCanTPDinhHinh.GetPhieuCanKiemDinhHinhs(date1, sanPhamBraVo.Id);
                    bool isF = false;
                    //foreach(var phieuCanDinhHinh in itemsP)
                    //{
                    //    var ps = items.Where(x => x.MaNhanVien == phieuCanDinhHinh.MaNhanVien);
                    //    if(ps.Any())
                    //    {
                    //        isF = true;
                    //        break;
                    //    }
                    //}
                    var sanPhamIds = items.Select(x => x.MaSanPham).Distinct().ToList();
                    if (sanPhamIds != null)
                    {
                        foreach (var item in sanPhamIds)
                        {
                            var log = Vm.VmLogKetChuyen.Get(date1, xuongId, item, 5);
                            if (log != null)
                            {
                                isF = true;
                                break;
                            }
                        }
                    }

                    if (isF == false)
                    {
                        var rows = 0;
                        await Task.Delay(1000);
                        await Task.Run(
                           () =>
                           {
                               rows = Vm.VmPhieuCanTPDinhHinh.Insert(items);
                               foreach (var item in sanPhamIds)
                               {
                                   Vm.VmLogKetChuyen.Insert(
                                       new LogKetChuyenBravo()
                                       {
                                           Gio = DateTime.Now.TimeOfDay,
                                           MaSanPham = item,
                                           MaXuong = xuongId,
                                           Ngay = date1.Date,
                                           NgayChuyen = DateTime.Now,
                                           tab = 5
                                       });
                               }
                           });
                        await Task.Delay(1000);
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Đã thực hiện trên {rows} dòng dữ liêu");

                    }
                    else
                    {
                        await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Thao tác đã thực hiện, Không thể thực hiện lại!");

                    }
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thành công!"
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // throw;
                    //MessageBox.Show(ex.Message);
                    await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Lỗi" + ex.ToString());
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi.",
                        Errors = new List<string> { ex.Message }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                await hubContext.Clients.All.SendAsync("ReceiveProgress", $@"Lỗi" + ex.ToString());
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        #endregion
        #endregion
    }
}
