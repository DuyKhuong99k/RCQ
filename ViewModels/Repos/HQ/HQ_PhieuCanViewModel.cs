using AppModels;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using Vars.Hubs;
using Microsoft.Data.SqlClient;
using System.Linq;
using ToolsEx;
namespace ViewModels.Repos.HQ
{
    public partial class HQ_PhieuCanViewModel
    {
        private static HQ_PhieuCanViewModel instance;
        public static HQ_PhieuCanViewModel Instance => instance ??= new HQ_PhieuCanViewModel();
        private HQ_PhieuCanViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {

            }
        }
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetChiTiets<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNhanViens<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetTongHopNhanViens<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            //var gioBatDauCaNgay = HQ_CaViewModel.Instance.Items
            //    .Where(x => !x.IsQuaDem)
            //    .Select(x => x.GioBatDau)
            //    .FirstOrDefault();

            //// Kiểm tra nếu không tìm thấy giờ bắt đầu ca ngày
            //if (gioBatDauCaNgay == default(DateTime))
            //{
            //    throw new InvalidOperationException("Không tìm thấy giờ bắt đầu của ca ngày.");
            //}
            //// Chuyển giờ bắt đầu ca ngày sang kiểu float (giờ thập phân)
            //float gioBatDauCaNgayFloat = gioBatDauCaNgay.Hour + gioBatDauCaNgay.Minute / 60f;
            return dao.GetTongHopThanhPhams<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopThanhPhamDashboards<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            //var gioBatDauCaNgay = HQ_CaViewModel.Instance.Items
            //    .Where(x => !x.IsQuaDem)
            //    .Select(x => x.GioBatDau)
            //    .FirstOrDefault();

            //// Kiểm tra nếu không tìm thấy giờ bắt đầu ca ngày
            //if (gioBatDauCaNgay == default(DateTime))
            //{
            //    throw new InvalidOperationException("Không tìm thấy giờ bắt đầu của ca ngày.");
            //}
            //// Chuyển giờ bắt đầu ca ngày sang kiểu float (giờ thập phân)
            //float gioBatDauCaNgayFloat = gioBatDauCaNgay.Hour + gioBatDauCaNgay.Minute / 60f;
            return dao.GetTongHopThanhPhamDashboards<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopNhanVienTheoCas<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetTongHopNhanVienTheoCas<T>(fromDate, toDate, xuongId);
        }

        public List<T> GetTongHopSanLuong<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetTongHopSanLuong3<T>(fromDate, toDate, xuongId);
        }


        public class TongHopTinhLuongDto
        {
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string MA_DAI_DIEN { get; set; }
            public string CaLamViec { get; set; }
            public decimal TongTrongLuong { get; set; }
            public string MaLoaiNguyenLieu { get; set; }
            public string TenLoaiNguyenLieu { get; set; }
            public string MaSize { get; set; }
            public string TenSize { get; set; }
            public string MaThanhPham { get; set; }
            public string TenThanhPham { get; set; }
            public string MaSanPham { get; set; }
            public string SanPhamName { get; set; }
            public string TenNhom { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public List<TongHopTinhLuongDto> GetTongHopTinhLuongs(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            var items = dao.GetTongHopSanLuong3<dynamic>(fromDate, toDate, xuongId);

            var nhomList = HQ_NhomViewModel.Instance.Gets<HQ_Nhom>()
                .Select(x => new { x.Id, x.Ten }).Distinct().ToDictionary(n => n.Id, n => n.Ten);

            // var donGiaList = DG_DonGiaViewModel.Instance.Gets<DG_DonGia>()
            //     .Select(x=>new {x.MaSanPham,x.DonGia}).ToDictionary(d => d.MaSanPham, d => d.DonGia);

            var result = items
            .GroupBy(x => new
            {
                x.MaNhanVien,
                x.TenNhanVien,
                x.MA_DAI_DIEN,
                x.CaLamViec,
                x.MaLoaiNguyenLieu,
                x.TenLoaiNguyenLieu,
                x.MaSize,
                x.TenSize,
                x.MaThanhPham,
                x.TenThanhPham,
                x.MaSanPham,
                x.SanPhamName
            })
            .Select(g => new TongHopTinhLuongDto
            {
                MaNhanVien = g.Key.MaNhanVien ?? "",
                TenNhanVien = g.Key.TenNhanVien ?? "",
                MA_DAI_DIEN = g.Key.MA_DAI_DIEN ?? "",
                CaLamViec = g.Key.CaLamViec ?? "",
                TongTrongLuong = g.Sum(i => (decimal)i.TongTrongLuong),
                MaLoaiNguyenLieu = g.Key.MaLoaiNguyenLieu ?? "",
                TenLoaiNguyenLieu = g.Key.TenLoaiNguyenLieu ?? "",
                MaSize = g.Key.MaSize ?? "",
                TenSize = g.Key.TenSize ?? "",
                MaThanhPham = g.Key.MaThanhPham ?? "", // Nếu null thì thay bằng chuỗi rỗng
                TenThanhPham = g.Key.TenThanhPham ?? "",
                MaSanPham = g.Key.MaSanPham ?? "",
                SanPhamName = g.Key.SanPhamName ?? "",
                TenNhom = nhomList.ContainsKey(g.Key.MA_DAI_DIEN) ? nhomList[g.Key.MA_DAI_DIEN] : "Không xác định",
                DonGia = 0,//donGiaList.ContainsKey(g.Key.MaSanPham ?? "") ? donGiaList[g.Key.MaSanPham ?? ""] : 0,
                ThanhTien = 0,//g.Sum(i => (decimal)i.TongTrongLuong) * (donGiaList.ContainsKey(g.Key.MaSanPham ?? "") ? donGiaList[g.Key.MaSanPham ?? ""] : 0)
            })
            .ToList();


            return result;
        }
        public class TongHopThongKeSanXuatDto
        {
            public DateTime Ngay { get; set; }
            public string CaLamViec { get; set; }
            public string MA_DAI_DIEN { get; set; }
            public string TenNhom { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string MaSanPham { get; set; }
            public string SanPhamName { get; set; }
            public decimal TongTrongLuong { get; set; }
            public string GioBatDau { get; set; }
            public string GioKetThuc { get; set; }
            public double TongGio { get; set; }
        }
        public List<TongHopThongKeSanXuatDto> GetTongHopThongKeSanXuatTemp(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            var items = dao.GetTongHopSanLuong3<dynamic>(fromDate, toDate, xuongId);

            var nhomList = HQ_NhomViewModel.Instance.Gets<HQ_Nhom>()
                .ToDictionary(n => n.Id, n => n.Ten);
            var _items = new List<TongHopThongKeSanXuatDto>();
            foreach (var item in items)
            {
                
            }
            var result = items
                .GroupBy(x => new
                {
                    x.NgayLamViec,
                    x.CaLamViec,
                    x.MA_DAI_DIEN,
                    x.MaNhanVien,
                    x.TenNhanVien,
                    x.MaSanPham,
                    x.SanPhamName,
                    x.MaThanhPham
                })
                .Select(g => new TongHopThongKeSanXuatDto
                {
                    Ngay = g.Key.NgayLamViec,
                    CaLamViec = g.Key.CaLamViec ?? "",
                    MA_DAI_DIEN = g.Key.MA_DAI_DIEN ?? "",
                    TenNhom = nhomList.ContainsKey(g.Key.MA_DAI_DIEN) ? nhomList[g.Key.MA_DAI_DIEN] : "Không xác định",
                    MaNhanVien = g.Key.MaNhanVien ?? "",
                    TenNhanVien = g.Key.TenNhanVien ?? "",
                    MaSanPham = g.Key.MaSanPham ?? "",
                    SanPhamName = g.Key.SanPhamName ?? "",
                    TongTrongLuong = g.Sum(i => (decimal)i.TongTrongLuong),
                    GioBatDau = g.Min(i => ((DateTime)i.GioBatDau).ToString("HH:mm")),
                    GioKetThuc = g.Max(i => ((DateTime)i.GioKetThuc).ToString("HH:mm")),
                    TongGio = Math.Round((g.Max(i => (DateTime)i.GioKetThuc) - g.Min(i => (DateTime)i.GioBatDau)).TotalHours, 2)

                })
                .ToList();

            return result;
        }
        public List<TongHopThongKeSanXuatDto> GetTongHopThongKeSanXuat(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);

            var items = dao.GetTongHopSanLuong3<dynamic>(fromDate, toDate, xuongId);

            var nhomList = HQ_NhomViewModel.Instance.Gets<HQ_Nhom>()
                .ToDictionary(n => n.Id, n => n.Ten);

            var result = new List<TongHopThongKeSanXuatDto>();

            var groups = items.GroupBy(x => new
            {
                x.NgayLamViec,
                x.CaLamViec,
                x.MaNhanVien,
                x.TenNhanVien,
                x.MaSanPham,
                x.SanPhamName,
                x.MA_DAI_DIEN
            });

            foreach (var group in groups)
            {
                var itemsSapXepTheoThoiGianBatDau = group.OrderBy(i => (DateTime)i.GioBatDau).ToList();

                DateTime? currentBatDau = null;
                DateTime? currentKetThuc = null;
                decimal tongTL = 0;
                var listTungConViec = new List<dynamic>();

                foreach (var item in itemsSapXepTheoThoiGianBatDau)
                {
                    // Lấy thời gian baatsd đau kết thức từng dòng
                    DateTime batDau = (DateTime)item.GioBatDau;
                    DateTime ketThuc = (DateTime)item.GioKetThuc;
                    decimal trongLuong = (decimal)item.TongTrongLuong;

                    // công việc trước không có thời gian kết thúc hoặc thời gian bắt đầu của dòng hiện tại cách thời gian kết thúc của cong việc trước không quá 2h
                    if (currentKetThuc == null || (batDau - currentKetThuc.Value).TotalHours <= 2)
                    {
                        
                        if (currentBatDau == null)
                        { 
                            currentBatDau = batDau;
                        }
                        // Cập nhật thời gian kết thúc nếu thời gian kết thúc của dòng hiện tại mhor hơn
                        if (currentKetThuc == null || ketThuc > currentKetThuc)
                        {
                             currentKetThuc = ketThuc;
                        }
                           
                        tongTL += trongLuong;
                        
                        listTungConViec.Add(item);
                    }
                    else
                    {
                        // Kết thúc ccong việc trước
                        result.Add(new TongHopThongKeSanXuatDto
                        {
                            Ngay = group.Key.NgayLamViec, 
                            CaLamViec = group.Key.CaLamViec ?? "", 
                            MA_DAI_DIEN = group.Key.MA_DAI_DIEN ?? "", 
                            TenNhom = nhomList.ContainsKey(group.Key.MA_DAI_DIEN) ? nhomList[group.Key.MA_DAI_DIEN] : "Không xác định", 
                            MaNhanVien = group.Key.MaNhanVien ?? "", 
                            TenNhanVien = group.Key.TenNhanVien ?? "", 
                            MaSanPham = group.Key.MaSanPham ?? "", 
                            SanPhamName = group.Key.SanPhamName ?? "",
                            TongTrongLuong = tongTL, 
                            GioBatDau = currentBatDau?.ToString("HH:mm") ?? "", 
                            GioKetThuc = currentKetThuc?.ToString("HH:mm") ?? "", 
                            TongGio = currentBatDau != null && currentKetThuc != null ? Math.Round((currentKetThuc.Value - currentBatDau.Value).TotalHours, 2) : 0 
                        });

                        // khời tạo lại công việc mới
                        currentBatDau = batDau; 
                        currentKetThuc = ketThuc; 
                        tongTL = trongLuong; 
                        listTungConViec.Clear(); // claer dòng trước để tạo mới công việc
                        listTungConViec.Add(item); // Thêm bảnvào vào cocng việc mới    
                    }
                }

                // kết quả
                if (listTungConViec.Any())
                {
                    result.Add(new TongHopThongKeSanXuatDto
                    {
                        Ngay = group.Key.NgayLamViec,
                        CaLamViec = group.Key.CaLamViec ?? "",
                        MA_DAI_DIEN = group.Key.MA_DAI_DIEN ?? "",
                        TenNhom = nhomList.ContainsKey(group.Key.MA_DAI_DIEN) ? nhomList[group.Key.MA_DAI_DIEN] : "Không xác định",
                        MaNhanVien = group.Key.MaNhanVien ?? "",
                        TenNhanVien = group.Key.TenNhanVien ?? "",
                        MaSanPham = group.Key.MaSanPham ?? "",
                        SanPhamName = group.Key.SanPhamName ?? "",
                        TongTrongLuong = tongTL,
                        GioBatDau = currentBatDau?.ToString("HH:mm") ?? "",
                        GioKetThuc = currentKetThuc?.ToString("HH:mm") ?? "",
                        TongGio = currentBatDau != null && currentKetThuc != null ? Math.Round((currentKetThuc.Value - currentBatDau.Value).TotalHours, 2) : 0
                    });
                }
            }
            
            return result.OrderBy(r => DateTime.Parse(r.GioBatDau)).ToList(); //sắp xếp theo thời gian bắt đầu
        }


        //public List<TongHopThongKeSanXuatDto> GetTongHopThongKeSanXuat(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        //{
        //    var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
        //    var items = dao.GetTongHopSanLuong4<dynamic>(fromDate, toDate, xuongId);

        //    // Tạo từ điển chứa tên nhóm
        //    var nhomList = HQ_NhomViewModel.Instance.Gets<HQ_Nhom>()
        //        .ToDictionary(n => n.Id, n => n.Ten);

        //    // Chuyển đổi items thành danh sách TongHopThongKeSanXuatDto
        //    var result = items
        //        .Select(item => new TongHopThongKeSanXuatDto
        //        {
        //            Ngay = item.Ngay, // DateTime từ SQL (NgayLamViec)
        //            CaLamViec = item.CaLamViec?.ToString() ?? "",
        //            MA_DAI_DIEN = item.MA_DAI_DIEN?.ToString() ?? "",
        //            TenNhom = item.MA_DAI_DIEN != "000" && nhomList.ContainsKey(item.MA_DAI_DIEN?.ToString())
        //                ? nhomList[item.MA_DAI_DIEN.ToString()]
        //                : "Không xác định",
        //            MaNhanVien = item.MaNhanVien?.ToString() ?? "",
        //            TenNhanVien = item.TenNhanVien?.ToString() ?? "",
        //            MaSanPham = item.MaSanPham?.ToString() ?? "",
        //            SanPhamName = item.SanPhamName?.ToString() ?? "Không xác định",
        //            TongTrongLuong = Convert.ToDecimal(item.TongTrongLuong),
        //            GioBatDau = item.GioBatDau is DateTime gioBatDau
        //                ? gioBatDau.ToString("HH:mm")
        //                : item.GioBatDau?.ToString() ?? "00:00",
        //            GioKetThuc = item.GioKetThuc is DateTime gioKetThuc
        //                ? gioKetThuc.ToString("HH:mm")
        //                : item.GioKetThuc?.ToString() ?? "00:00",
        //            TongGio = item.GioKetThuc is DateTime ketThuc && item.GioBatDau is DateTime batDau
        //                ? Math.Round((ketThuc - batDau).TotalHours, 2)
        //                : Convert.ToDouble(item.TongGio)
        //        })
        //        .OrderBy(x => x.MaNhanVien)
        //        .ThenBy(x => DateTime.Parse(x.Ngay.ToString("yyyy-MM-dd") + " " + x.GioBatDau))
        //        .ToList();

        //    // Điều chỉnh GioBatDau để khớp với GioKetThuc của bản ghi trước cho cùng MaNhanVien
        //    for (int i = 1; i < result.Count; i++)
        //    {
        //        if (result[i].MaNhanVien == result[i - 1].MaNhanVien &&
        //            result[i].Ngay == result[i - 1].Ngay &&
        //            result[i].CaLamViec == result[i - 1].CaLamViec)
        //        {
        //            result[i].GioBatDau = result[i - 1].GioKetThuc;
        //            // Tính lại TongGio
        //            var batDau = DateTime.Parse(result[i].Ngay.ToString("yyyy-MM-dd") + " " + result[i].GioBatDau);
        //            var ketThuc = DateTime.Parse(result[i].Ngay.ToString("yyyy-MM-dd") + " " + result[i].GioKetThuc);
        //            result[i].TongGio = Math.Round((ketThuc - batDau).TotalHours, 2);
        //        }
        //    }

        //    return result;
        //}

        //public List<T> GetTongHopTinhLuongs<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        //{
        //    var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);


        //    DateTime toDateWithTime = toDate.Date.Add(DateTime.Now.TimeOfDay);
        //    // Lấy giờ bắt đầu của ca ngày từ danh sách ca làm việc
        //    var gioBatDauCaNgay = HQ_CaViewModel.Instance.Items
        //        .Where(x => !x.IsQuaDem)
        //        .Select(x => x.GioBatDau)
        //        .FirstOrDefault();

        //    // Kiểm tra nếu không tìm thấy giờ bắt đầu ca ngày
        //    if (gioBatDauCaNgay == default(DateTime))
        //    {
        //        throw new InvalidOperationException("Không tìm thấy giờ bắt đầu của ca ngày.");
        //    }

        //    // Chuyển giờ bắt đầu ca ngày sang kiểu float (giờ thập phân)
        //    float gioBatDauCaNgayFloat = gioBatDauCaNgay.Hour + gioBatDauCaNgay.Minute / 60f;

        //    var items = dao.GetTongHopTinhLuongs<T>(fromDate, toDate, xuongId, gioBatDauCaNgayFloat);
        //    return items;
        //    ///////////////////////////ĐÂY LÀ CODE CHO SỬ DỤNG THẺ NHÓM ĐỂ TÍNH SL CỦA TOÀN BỘ NHÂN VIÊN TRONG THẺ NHÓM////////////////////////
        //    //if (items != null)
        //    //{
        //    //    var nhanVienIsNhoms = NhanVienViewModel.Instance.GetDanhSachNhanVienIsNhoms<NhanVienDaiThanh>().ToList();
        //    //    // Lọc các phần tử trong items dựa trên mã nhân viên thuộc nhóm
        //    //    var filterNhanVienNhoms = items
        //    //    .Where(item =>
        //    //    {
        //    //        dynamic dynamicItem = item;
        //    //        // Ép kiểu dynamicItem.MaNhanVien thành string và kiểm tra trong danh sách MaNhanVien
        //    //        return nhanVienIsNhoms.Select(nv => nv.MaNhanVien).Contains((string)dynamicItem.MaNhanVien);
        //    //    })
        //    //    .GroupBy(item =>
        //    //    {
        //    //        dynamic dynamicItem = item;
        //    //        return new { MaNhanVien = (string)dynamicItem.MaNhanVien, MaThanhPham = (string)dynamicItem.MaThanhPham };
        //    //    })
        //    //    .Select(group =>
        //    //    {
        //    //        dynamic firstItem = group.First();
        //    //        return new
        //    //        {
        //    //            //Ngay = firstItem.Ngay,
        //    //            CaId = firstItem.CaId,
        //    //            //CaName = firstItem.CaName,
        //    //            MaNhanVien = group.Key.MaNhanVien,
        //    //            MaHoSo = firstItem.MaHoSo,
        //    //            NhanVienName = firstItem.NhanVienName,
        //    //            MaThanhPham = group.Key.MaThanhPham,
        //    //            ThanhPhamName = firstItem.ThanhPhamName,
        //    //            MaSize = firstItem.MaSize,
        //    //            SizeName = firstItem.SizeName,
        //    //            MaLoaiNguyenLieu = firstItem.MaLoaiNguyenLieu,
        //    //            LoaiNguyenLieuName = firstItem.LoaiNguyenLieuName,
        //    //            MaSanPhamTinhLuong = firstItem.MaSanPhamTinhLuong,
        //    //            TenSanPhamTinhLuong = firstItem.TenSanPhamTinhLuong,
        //    //            DonGia = (decimal?)(firstItem.DonGia ?? 0),
        //    //            ThanhTien = (decimal?)(firstItem.ThanhTien ?? 0),
        //    //            TrongLuong = (decimal)firstItem.TrongLuong,
        //    //            //TrongLuong = group.Sum(item => (decimal)((dynamic)item).TrongLuong)
        //    //        };
        //    //    })
        //    //    .ToList();
        //    //    // Lấy danh sách mã nhân viên từ filterNhanVienNhoms
        //    //    var filteredNameNhanViens = filterNhanVienNhoms.Select(item => ((dynamic)item).NhanVienName).Distinct().ToList();
        //    //    foreach (var nameNhanVienfiltered in filteredNameNhanViens)
        //    //    {
        //    //        var maNhombyNameNhanVien = HQ_NhomViewModel.Instance
        //    //        .Gets<HQ_Nhom>()
        //    //        .Where(item => StringExtensions.NonUnicode(nameNhanVienfiltered).ToUpper() == StringExtensions.NonUnicode(item.Ten).ToUpper())
        //    //        .Select(item => item.Id)
        //    //        .ToList();
        //    //        foreach (var stringmaNhombyNameNhanVien in maNhombyNameNhanVien)
        //    //        {
        //    //            // danh sách nhân viên nhóm
        //    //            var danhSachNhanVienTheoMaNhom = HQ_NhanVienTheoNhomViewModel.Instance.Gets<HQ_NhanVienTheoNhom>(toDate)
        //    //            .Where(item => item.MaNhom.Trim().ToUpper() == stringmaNhombyNameNhanVien.ToString().Trim().ToUpper())
        //    //            .Select(item => new
        //    //            {
        //    //                MaNhom = item.MaNhom,
        //    //                MaNhanVien = item.MaNhanVien,
        //    //                HeSo = item.HeSo
        //    //            })
        //    //            .ToList();

        //    //            // tính toonge hệ số nhóm
        //    //            var tongHeSoTheoNhom = danhSachNhanVienTheoMaNhom
        //    //                .GroupBy(item => item.MaNhom)
        //    //                .Select(group => new
        //    //                {
        //    //                    MaNhom = group.Key,
        //    //                    TongHeSo = group.Sum(x => x.HeSo)
        //    //                })
        //    //                .ToList();

        //    //            // lấy thông tin phiếu cân gốc để khởi tạo thông tin cho phiếu cân nhân viên trong nhóm
        //    //            var danhSachThongTinPhieuCanByTheNhom = filterNhanVienNhoms
        //    //                .Join(HQ_NhomViewModel.Instance.Gets<HQ_Nhom>(),
        //    //                    nhanviennhom => StringExtensions.NonUnicode(nhanviennhom.NhanVienName).Trim().ToUpper(),
        //    //                    nhom => StringExtensions.NonUnicode(nhom.Ten).Trim().ToUpper(),
        //    //                    (nhanviennhom, nhom) => new
        //    //                    {
        //    //                        nhom.Id,
        //    //                        nhanviennhom.MaThanhPham,
        //    //                        nhanviennhom.MaLoaiNguyenLieu,
        //    //                        nhanviennhom.MaSize,
        //    //                        nhanviennhom.MaSanPhamTinhLuong,
        //    //                        nhanviennhom.DonGia,
        //    //                        nhanviennhom.ThanhTien,
        //    //                        nhanviennhom.TrongLuong,

        //    //                    }).ToList();


        //    //            // Tạo list danh sách Phân Bổ Trọng Lượng cho từng nhân viên trong nhóm đã tìm được
        //    //            var danhSachNhanVienWithData = danhSachNhanVienTheoMaNhom
        //    //                .Join(
        //    //                    tongHeSoTheoNhom, // Danh sách tổng hệ số theo nhóm
        //    //                    nhanVien => nhanVien.MaNhom,
        //    //                    tongHeSo => tongHeSo.MaNhom,
        //    //                    (nhanVien, tongHeSo) => new
        //    //                    {
        //    //                        nhanVien.MaNhom,
        //    //                        nhanVien.MaNhanVien,
        //    //                        nhanVien.HeSo,
        //    //                        TongHeSoNhom = tongHeSo.TongHeSo // Tổng hệ số của nhóm
        //    //                    }
        //    //                )

        //    //                .Join(
        //    //                    danhSachThongTinPhieuCanByTheNhom,
        //    //                    nhanVien => nhanVien.MaNhom,
        //    //                    trongLuong => trongLuong.Id,
        //    //                    (nhanVien, trongLuong) => new
        //    //                    {
        //    //                        nhanVien.MaNhom,
        //    //                        nhanVien.MaNhanVien,
        //    //                        nhanVien.HeSo,
        //    //                        nhanVien.TongHeSoNhom,
        //    //                        trongLuong.MaThanhPham,
        //    //                        trongLuong.MaLoaiNguyenLieu,
        //    //                        trongLuong.MaSize,
        //    //                        trongLuong.MaSanPhamTinhLuong,
        //    //                        trongLuong.DonGia,
        //    //                        trongLuong.ThanhTien,
        //    //                        //trongLuong.TangCa,
        //    //                        trongLuong.TrongLuong, // Tổng trọng lượng của nhóm
        //    //                        TrongLuongPhanBo = Math.Round((trongLuong.TrongLuong / nhanVien.TongHeSoNhom) * nhanVien.HeSo, 2),
        //    //                        TienPhanBo = Math.Round(((decimal)trongLuong.ThanhTien / nhanVien.TongHeSoNhom) * nhanVien.HeSo, 2)
        //    //                    }
        //    //                )
        //    //                .Join(
        //    //                    NhanVienViewModel.Instance.GetNhanVienDaiThanhs(xuongId),
        //    //                    nhanVien => nhanVien.MaNhanVien,
        //    //                    daiThanh => daiThanh.MaNhanVien,
        //    //                    (nhanVien, daiThanh) => new
        //    //                    {
        //    //                        nhanVien.MaNhom,
        //    //                        nhanVien.MaNhanVien,
        //    //                        nhanVien.HeSo,
        //    //                        nhanVien.TongHeSoNhom,
        //    //                        nhanVien.TrongLuong,
        //    //                        nhanVien.TrongLuongPhanBo,
        //    //                        nhanVien.MaThanhPham,
        //    //                        nhanVien.MaLoaiNguyenLieu,
        //    //                        nhanVien.MaSize,
        //    //                        nhanVien.MaSanPhamTinhLuong,
        //    //                        nhanVien.DonGia,
        //    //                        nhanVien.TienPhanBo,
        //    //                        //nhanVien.TangCa,
        //    //                        daiThanh.MaHoSo,
        //    //                        daiThanh.Name
        //    //                    }
        //    //                )
        //    //                .Join(
        //    //                    HQ_NhomViewModel.Instance.Gets<HQ_Nhom>(),
        //    //                    nhanVien => nhanVien.MaNhom,
        //    //                    nhom => nhom.Id,
        //    //                    (nhanVien, nhom) => new
        //    //                    {
        //    //                        nhanVien.MaNhom,
        //    //                        nhanVien.MaNhanVien,
        //    //                        nhanVien.HeSo,
        //    //                        nhanVien.TongHeSoNhom,
        //    //                        nhanVien.TrongLuong,
        //    //                        //nhanVien.TangCa,
        //    //                        nhanVien.TrongLuongPhanBo,
        //    //                        nhanVien.MaThanhPham,
        //    //                        nhanVien.MaLoaiNguyenLieu,
        //    //                        nhanVien.MaSize,
        //    //                        nhanVien.MaSanPhamTinhLuong,
        //    //                        nhanVien.DonGia,
        //    //                        nhanVien.TienPhanBo,
        //    //                        nhanVien.MaHoSo,
        //    //                        nhanVien.Name,

        //    //                        NhomName = nhom.Ten
        //    //                    }
        //    //                )

        //    //                .ToList();
        //    //            // Cập nhật dữ liệu vào danh sách items
        //    //            var updatedItems = danhSachNhanVienWithData.Select(nhanVien => new
        //    //            {
        //    //                //CaId = nhanVien.CaId,
        //    //                //CaName = nhanVien.CaName,
        //    //                MaNhanVien = nhanVien.MaNhanVien,
        //    //                MaHoSo = nhanVien.MaHoSo,
        //    //                NhanVienName = nhanVien.Name,
        //    //                MaNhom = nhanVien.MaNhom,
        //    //                NhomName = nhanVien.NhomName,
        //    //                MaThanhPham = nhanVien.MaThanhPham,
        //    //                ThanhPhamName = filterNhanVienNhoms.FirstOrDefault(item =>
        //    //                    ((dynamic)item).MaThanhPham == nhanVien.MaThanhPham)?.ThanhPhamName,
        //    //                MaLoaiNguyenLieu = nhanVien.MaLoaiNguyenLieu,
        //    //                LoaiNguyenLieuName = filterNhanVienNhoms.FirstOrDefault(item =>
        //    //                    ((dynamic)item).MaLoaiNguyenLieu == nhanVien.MaLoaiNguyenLieu)?.LoaiNguyenLieuName,
        //    //                MaSize = nhanVien.MaSize,
        //    //                SizeName = filterNhanVienNhoms.FirstOrDefault(item =>
        //    //                    ((dynamic)item).MaSize == nhanVien.MaSize)?.SizeName,
        //    //                MaSanPhamTinhLuong = nhanVien.MaSanPhamTinhLuong,
        //    //                TenSanPhamTinhLuong = filterNhanVienNhoms.FirstOrDefault(item =>
        //    //                    ((dynamic)item).MaSanPhamTinhLuong == nhanVien.MaSanPhamTinhLuong)?.TenSanPhamTinhLuong,
        //    //                //TangCa = nhanVien.TangCa,
        //    //                DonGia = nhanVien.DonGia,
        //    //                ThanhTien = nhanVien.TienPhanBo,
        //    //                TrongLuong = nhanVien.TrongLuongPhanBo
        //    //            }).ToList();

        //    //            items.AddRange(updatedItems.Cast<T>());
        //    //            // Loại bỏ các phiếu có trong filterNhanVienNhoms
        //    //            var maNhanViensFilter = filterNhanVienNhoms.Select(x => ((dynamic)x).MaNhanVien).ToList();
        //    //            items.RemoveAll(item =>
        //    //            {
        //    //                dynamic dynamicItem = item;
        //    //                return maNhanViensFilter.Contains((string)dynamicItem.MaNhanVien);
        //    //            });
        //    //        }

        //    //    }
        //        //return items;

        //    //}
        //    //return new List<T>(); ;

        //}
        ////public List<T> GetTongHopThongKeSanXuats<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        ////{
        ////    DateTime toDateWithTime = toDate.Date.Add(DateTime.Now.TimeOfDay);

        ////    var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);

        ////    // Lấy giờ bắt đầu của ca ngày từ danh sách ca làm việc
        ////    var gioBatDauCaNgay = HQ_CaViewModel.Instance.Items
        ////        .Where(x => !x.IsQuaDem)
        ////        .Select(x => x.GioBatDau)
        ////        .FirstOrDefault();

        ////    // Kiểm tra nếu không tìm thấy giờ bắt đầu ca ngày
        ////    if (gioBatDauCaNgay == default(DateTime))
        ////    {
        ////        throw new InvalidOperationException("Không tìm thấy giờ bắt đầu của ca ngày.");
        ////    }

        ////    // Chuyển giờ bắt đầu ca ngày sang kiểu float (giờ thập phân)
        ////    float gioBatDauCaNgayFloat = gioBatDauCaNgay.Hour + gioBatDauCaNgay.Minute / 60f;


        ////    var items = dao.GetTongHopThongKeSanXuats<T>(toDate, xuongId, gioBatDauCaNgayFloat);

        ////    // Kiểm tra và trả về danh sách
        ////    if (items != null)
        ////    {

        ////        var nhanVienIsNhoms = NhanVienViewModel.Instance.GetDanhSachNhanVienIsNhoms<NhanVienDaiThanh>().ToList();

        ////        // Lọc các phần tử trong items dựa trên mã nhân viên thuộc nhóm
        ////        var filterNhanVienNhoms = items
        ////        .Where(item =>
        ////        {
        ////            dynamic dynamicItem = item;
        ////            // Ép kiểu dynamicItem.MaNhanVien thành string và kiểm tra trong danh sách MaNhanVien
        ////            return nhanVienIsNhoms.Select(nv => nv.MaNhanVien).Contains((string)dynamicItem.MaNhanVien);
        ////        })
        ////        .GroupBy(item =>
        ////        {
        ////            dynamic dynamicItem = item;
        ////            return new { MaNhanVien = (string)dynamicItem.MaNhanVien, MaThanhPham = (string)dynamicItem.MaThanhPham };
        ////        })
        ////        .Select(group =>
        ////        {
        ////            dynamic firstItem = group.First();
        ////            return new
        ////            {
        ////                Ngay = firstItem.Ngay,
        ////                CaId = firstItem.CaId,
        ////                CaName = firstItem.CaName,
        ////                MaNhanVien = group.Key.MaNhanVien,
        ////                MaHoSo = firstItem.MaHoSo,
        ////                NhanVienName = firstItem.NhanVienName,
        ////                MaNhom = firstItem.MaNhom,
        ////                NhomName = firstItem.NhomName,
        ////                MaThanhPham = group.Key.MaThanhPham,
        ////                ThanhPhamName = firstItem.ThanhPhamName,
        ////                TrongLuongTong = group.Sum(item => (decimal)((dynamic)item).TrongLuong),
        ////                GioBatDau = firstItem.GioBatDau,
        ////                GioKetThuc = firstItem.GioKetThuc
        ////            };
        ////        })
        ////        .ToList();

        ////        // Lấy danh sách mã nhóm từ filterNhanVienNhoms
        ////        var filteredMaNhoms = filterNhanVienNhoms.Select(item => ((dynamic)item).MaNhom).Distinct().ToList();
        ////        // Lấy danh sách mã nhân viên từ filterNhanVienNhoms
        ////        var filteredMaNhanViens = filterNhanVienNhoms.Select(item => ((dynamic)item).MaNhanVien).Distinct().ToList();

        ////        // Lọc danh sách nhân viên theo mã nhóm, loại bỏ những nhân viên thuộc filterNhanVienNhoms
        ////        var danhSachNhanVienTheoMaNhom = HQ_NhanVienTheoNhomViewModel.Instance.Gets<HQ_NhanVienTheoNhom>(toDate)
        ////            .Where(item => filteredMaNhoms.Contains(item.MaNhom) && !filteredMaNhanViens.Contains(item.MaNhanVien))
        ////            .Select(item => new
        ////            {
        ////                MaNhom = item.MaNhom,
        ////                MaNhanVien = item.MaNhanVien,
        ////                HeSo = item.HeSo
        ////            })
        ////            .ToList();
        ////        var tongHeSoTheoNhom = danhSachNhanVienTheoMaNhom
        ////            .GroupBy(item => item.MaNhom)
        ////            .Select(group => new
        ////            {
        ////                MaNhom = group.Key,
        ////                TongHeSo = group.Sum(x => x.HeSo)
        ////            })
        ////            .ToList();
        ////        // Tính tổng trọng lượng theo nhóm từ filterNhanVienNhoms
        ////        var tongTrongLuongTheoNhom = filterNhanVienNhoms
        ////        .GroupBy(item => ((dynamic)item).MaNhom)
        ////        .Select(group => new
        ////        {
        ////            MaNhom = group.Key,
        ////            TongTrongLuong = group.Sum(x => (decimal)((dynamic)x).TrongLuongTong)
        ////        })
        ////        .ToList();


        ////        var danhSachNhanVienWithData = danhSachNhanVienTheoMaNhom
        ////            .Join(
        ////                tongHeSoTheoNhom, // Danh sách tổng hệ số theo nhóm
        ////                nhanVien => nhanVien.MaNhom,
        ////                tongHeSo => tongHeSo.MaNhom,
        ////                (nhanVien, tongHeSo) => new
        ////                {
        ////                    nhanVien.MaNhom,
        ////                    nhanVien.MaNhanVien,
        ////                    nhanVien.HeSo,
        ////                    TongHeSoNhom = tongHeSo.TongHeSo // Tổng hệ số của nhóm
        ////                }
        ////            )
        ////            .Join(
        ////                tongTrongLuongTheoNhom,
        ////                nhanVien => nhanVien.MaNhom,
        ////                trongLuong => trongLuong.MaNhom,
        ////                (nhanVien, trongLuong) => new
        ////                {
        ////                    nhanVien.MaNhom,
        ////                    nhanVien.MaNhanVien,
        ////                    nhanVien.HeSo,
        ////                    nhanVien.TongHeSoNhom,
        ////                    trongLuong.TongTrongLuong, // Tổng trọng lượng của nhóm
        ////                    TrongLuongPhanBo = Math.Round((nhanVien.HeSo / nhanVien.TongHeSoNhom) * trongLuong.TongTrongLuong, 2)
        ////                }
        ////            )
        ////            .Join(
        ////                NhanVienViewModel.Instance.GetNhanVienDaiThanhs(xuongId),
        ////                nhanVien => nhanVien.MaNhanVien,
        ////                daiThanh => daiThanh.MaNhanVien,
        ////                (nhanVien, daiThanh) => new
        ////                {
        ////                    nhanVien.MaNhom,
        ////                    nhanVien.MaNhanVien,
        ////                    nhanVien.HeSo,
        ////                    nhanVien.TongHeSoNhom,
        ////                    nhanVien.TongTrongLuong,
        ////                    nhanVien.TrongLuongPhanBo,
        ////                    daiThanh.MaHoSo,
        ////                    daiThanh.Name
        ////                }
        ////            )
        ////            .Join(
        ////                HQ_NhomViewModel.Instance.Gets<HQ_Nhom>(),
        ////                nhanVien => nhanVien.MaNhom,
        ////                nhom => nhom.Id,
        ////                (nhanVien, nhom) => new
        ////                {
        ////                    nhanVien.MaNhom,
        ////                    nhanVien.MaNhanVien,
        ////                    nhanVien.HeSo,
        ////                    nhanVien.TongHeSoNhom,
        ////                    nhanVien.TongTrongLuong,
        ////                    nhanVien.TrongLuongPhanBo,
        ////                    nhanVien.MaHoSo,
        ////                    nhanVien.Name,
        ////                    NhomName = nhom.Ten
        ////                }
        ////            )
        ////            .Join(
        ////                HQ_NhanVienTheoCaViewModel.Instance.Gets<HQ_NhanVienTheoCa>(toDateWithTime),
        ////                nhanVien => nhanVien.MaNhanVien,
        ////                nhanVienTheoCa => nhanVienTheoCa.NhanVienId,
        ////                (nhanVien, nhanVienTheoCa) => new
        ////                {
        ////                    nhanVien.MaNhom,
        ////                    nhanVien.MaNhanVien,
        ////                    nhanVien.HeSo,
        ////                    nhanVien.TongHeSoNhom,
        ////                    nhanVien.TongTrongLuong,
        ////                    nhanVien.TrongLuongPhanBo,
        ////                    nhanVien.MaHoSo,
        ////                    nhanVien.Name,
        ////                    nhanVien.NhomName,
        ////                    CaId = nhanVienTheoCa.CaId
        ////                }
        ////            )
        ////            .Join(
        ////                HQ_CaViewModel.Instance.Gets<HQ_Ca>(),
        ////                nhanvien => int.Parse(nhanvien.CaId),
        ////                ca => ca.Id,
        ////                (nhanVien, ca) => new
        ////                {
        ////                    nhanVien.MaNhom,
        ////                    nhanVien.MaNhanVien,
        ////                    nhanVien.HeSo,
        ////                    nhanVien.TongHeSoNhom,
        ////                    nhanVien.TongTrongLuong,
        ////                    nhanVien.TrongLuongPhanBo,
        ////                    nhanVien.MaHoSo,
        ////                    nhanVien.Name,
        ////                    nhanVien.NhomName,
        ////                    nhanVien.CaId,
        ////                    CaName = ca.Ten // Lấy tên ca
        ////                }
        ////            )
        ////            .ToList();

        ////        // Cập nhật dữ liệu vào danh sách items
        ////        var updatedItems = danhSachNhanVienWithData.Select(nhanVien => new
        ////        {
        ////            Ngay = filterNhanVienNhoms.FirstOrDefault(item => ((dynamic)item).MaNhom == nhanVien.MaNhom)?.Ngay,
        ////            CaId = nhanVien.CaId,
        ////            CaName = nhanVien.CaName,
        ////            MaNhanVien = nhanVien.MaNhanVien,
        ////            MaHoSo = nhanVien.MaHoSo,
        ////            NhanVienName = nhanVien.Name,
        ////            MaNhom = nhanVien.MaNhom,
        ////            NhomName = nhanVien.NhomName,
        ////            MaThanhPham = filterNhanVienNhoms.FirstOrDefault(item => ((dynamic)item).MaNhom == nhanVien.MaNhom)?.MaThanhPham,
        ////            ThanhPhamName = filterNhanVienNhoms.FirstOrDefault(item => ((dynamic)item).MaNhom == nhanVien.MaNhom)?.ThanhPhamName,
        ////            TrongLuong = nhanVien.TrongLuongPhanBo,
        ////            GioBatDau = filterNhanVienNhoms.FirstOrDefault(item => ((dynamic)item).MaNhom == nhanVien.MaNhom)?.GioBatDau,
        ////            GioKetThuc = filterNhanVienNhoms.FirstOrDefault(item => ((dynamic)item).MaNhom == nhanVien.MaNhom)?.GioKetThuc
        ////        }).ToList();


        ////        items.AddRange(updatedItems.Cast<T>());
        ////        // Loại bỏ các phiếu có trong filterNhanVienNhoms
        ////        var maNhanViensFilter = filterNhanVienNhoms.Select(x => ((dynamic)x).MaNhanVien).ToList();
        ////        items.RemoveAll(item =>
        ////        {
        ////            dynamic dynamicItem = item;
        ////            return maNhanViensFilter.Contains((string)dynamicItem.MaNhanVien);
        ////        });

        ////        return items;
        ////    }

        ////    return new List<T>(); ;
        ////}
        public List<T> GetTongHopThongKeSanXuats<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            DateTime toDateWithTime = toDate.Date.Add(DateTime.Now.TimeOfDay);

            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);

            // Lấy giờ bắt đầu của ca ngày từ danh sách ca làm việc
            var gioBatDauCaNgay = HQ_CaViewModel.Instance.Items
                .Where(x => !x.IsQuaDem)
                .Select(x => x.GioBatDau)
                .FirstOrDefault();

            // Kiểm tra nếu không tìm thấy giờ bắt đầu ca ngày
            if (gioBatDauCaNgay == default(DateTime))
            {
                throw new InvalidOperationException("Không tìm thấy giờ bắt đầu của ca ngày.");
            }

            // Chuyển giờ bắt đầu ca ngày sang kiểu float (giờ thập phân)
            float gioBatDauCaNgayFloat = gioBatDauCaNgay.Hour + gioBatDauCaNgay.Minute / 60f;


            var items = dao.GetTongHopThongKeSanXuats<T>(toDate, xuongId, gioBatDauCaNgayFloat);

            // Kiểm tra và trả về danh sách
            if (items != null)
            {

                var nhanVienIsNhoms = NhanVienViewModel.Instance.GetDanhSachNhanVienIsNhoms<NhanVienDaiThanh>().ToList();

                // Lọc các phần tử trong items dựa trên mã nhân viên thuộc nhóm
                var filterNhanVienNhoms = items
                .Where(item =>
                {
                    dynamic dynamicItem = item;
                    // Ép kiểu dynamicItem.MaNhanVien thành string và kiểm tra trong danh sách MaNhanVien
                    return nhanVienIsNhoms.Select(nv => nv.MaNhanVien).Contains((string)dynamicItem.MaNhanVien);
                })
                .GroupBy(item =>
                {
                    dynamic dynamicItem = item;
                    return new { MaNhanVien = (string)dynamicItem.MaNhanVien, MaThanhPham = (string)dynamicItem.MaThanhPham };
                })
                .Select(group =>
                {
                    dynamic firstItem = group.First();
                    return new
                    {
                        Ngay = firstItem.Ngay,
                        CaId = firstItem.CaId,
                        CaName = firstItem.CaName,
                        MaNhanVien = group.Key.MaNhanVien,
                        MaHoSo = firstItem.MaHoSo,
                        NhanVienName = firstItem.NhanVienName,
                        //MaNhom = firstItem.MaNhom,
                        //NhomName = firstItem.NhomName,
                        MaThanhPham = group.Key.MaThanhPham,
                        ThanhPhamName = firstItem.ThanhPhamName,
                        MaSanPhamTinhLuong = firstItem.MaSanPhamTinhLuong,
                        SanPhamTinhLuongName = firstItem.SanPhamTinhLuongName,
                        TrongLuong = (decimal)firstItem.TrongLuong,
                        GioBatDau = firstItem.GioBatDau,
                        GioKetThuc = firstItem.GioKetThuc
                    };
                })
                .ToList();

                // Lấy danh sách mã nhóm từ filterNhanVienNhoms
                //var filteredMaNhoms = filterNhanVienNhoms.Select(item => ((dynamic)item).MaNhom).Distinct().ToList();

                // Lấy danh sách mã nhân viên từ filterNhanVienNhoms
                var filteredNameNhanViens = filterNhanVienNhoms.Select(item => ((dynamic)item).NhanVienName).Distinct().ToList();
                foreach (var nameNhanVienfiltered in filteredNameNhanViens)
                {
                    var maNhombyNameNhanVien = HQ_NhomViewModel.Instance
                    .Gets<HQ_Nhom>()
                    .Where(item => StringExtensions.NonUnicode(nameNhanVienfiltered).ToUpper() == StringExtensions.NonUnicode(item.Ten).ToUpper())
                    .Select(item => item.Id)
                    .ToList();
                    foreach (var stringmaNhombyNameNhanVien in maNhombyNameNhanVien)
                    {
                        // danh sách nhân viên nhóm
                        var danhSachNhanVienTheoMaNhom = HQ_NhanVienTheoNhomViewModel.Instance.Gets<HQ_NhanVienTheoNhom>(toDate)
                        .Where(item => item.MaNhom.Trim().ToUpper() == stringmaNhombyNameNhanVien.ToString().Trim().ToUpper())
                        .Select(item => new
                        {
                            MaNhom = item.MaNhom,
                            MaNhanVien = item.MaNhanVien,
                            HeSo = item.HeSo
                        })
                        .ToList();
                        // tính toonge hệ số nhóm
                        var tongHeSoTheoNhom = danhSachNhanVienTheoMaNhom
                            .GroupBy(item => item.MaNhom)
                            .Select(group => new
                            {
                                MaNhom = group.Key,
                                TongHeSo = group.Sum(x => x.HeSo)
                            })
                            .ToList();
                        // lấy thông tin phiếu cân gốc để khởi tạo thông tin cho phiếu cân nhân viên trong nhóm
                        var danhSachThongTinPhieuCanByTheNhom = filterNhanVienNhoms
                            .Join(HQ_NhomViewModel.Instance.Gets<HQ_Nhom>(),
                                nhanviennhom => StringExtensions.NonUnicode(nhanviennhom.NhanVienName).Trim().ToUpper(),
                                nhom => StringExtensions.NonUnicode(nhom.Ten).Trim().ToUpper(),
                                (nhanviennhom, nhom) => new
                                {
                                    nhom.Id,
                                    nhanviennhom.MaThanhPham,
                                    nhanviennhom.MaSanPhamTinhLuong,
                                    nhanviennhom.SanPhamTinhLuongName,
                                    nhanviennhom.TrongLuong,
                                    nhanviennhom.Ngay,
                                    nhanviennhom.GioBatDau,
                                    nhanviennhom.GioKetThuc

                                }).ToList();


                        var danhSachNhanVienWithData = danhSachNhanVienTheoMaNhom
                            .Join(
                                tongHeSoTheoNhom, // Danh sách tổng hệ số theo nhóm
                                nhanVien => nhanVien.MaNhom,
                                tongHeSo => tongHeSo.MaNhom,
                                (nhanVien, tongHeSo) => new
                                {
                                    nhanVien.MaNhom,
                                    nhanVien.MaNhanVien,
                                    nhanVien.HeSo,
                                    TongHeSoNhom = tongHeSo.TongHeSo // Tổng hệ số của nhóm
                                }
                            )
                            .Join(
                                danhSachThongTinPhieuCanByTheNhom,
                                nhanVien => nhanVien.MaNhom,
                                trongLuong => trongLuong.Id,
                                (nhanVien, trongLuong) => new
                                {
                                    nhanVien.MaNhom,
                                    nhanVien.MaNhanVien,
                                    nhanVien.HeSo,
                                    nhanVien.TongHeSoNhom,
                                    trongLuong.MaThanhPham,
                                    trongLuong.MaSanPhamTinhLuong,
                                    trongLuong.SanPhamTinhLuongName,
                                    trongLuong.TrongLuong, // Tổng trọng lượng của nhóm
                                    trongLuong.Ngay,
                                    trongLuong.GioBatDau,
                                    trongLuong.GioKetThuc,
                                    TrongLuongPhanBo = Math.Round((trongLuong.TrongLuong / nhanVien.TongHeSoNhom) * nhanVien.HeSo, 2),
                                }
                            )
                            .Join(
                                NhanVienViewModel.Instance.GetNhanVienDaiThanhs(xuongId),
                                nhanVien => nhanVien.MaNhanVien,
                                daiThanh => daiThanh.MaNhanVien,
                                (nhanVien, daiThanh) => new
                                {
                                    nhanVien.Ngay,
                                    nhanVien.MaNhom,
                                    nhanVien.MaNhanVien,
                                    nhanVien.HeSo,
                                    nhanVien.TongHeSoNhom,
                                    nhanVien.TrongLuong,
                                    nhanVien.TrongLuongPhanBo,
                                    nhanVien.MaThanhPham,
                                    nhanVien.MaSanPhamTinhLuong,
                                    nhanVien.GioBatDau,
                                    nhanVien.GioKetThuc,
                                    daiThanh.MaHoSo,
                                    daiThanh.Name
                                }
                            )
                            .Join(
                                HQ_NhomViewModel.Instance.Gets<HQ_Nhom>(),
                                nhanVien => nhanVien.MaNhom,
                                nhom => nhom.Id,
                                (nhanVien, nhom) => new
                                {
                                    nhanVien.Ngay,
                                    nhanVien.MaNhom,
                                    nhanVien.MaNhanVien,
                                    nhanVien.HeSo,
                                    nhanVien.TongHeSoNhom,
                                    nhanVien.TrongLuong,
                                    nhanVien.TrongLuongPhanBo,
                                    nhanVien.MaThanhPham,
                                    nhanVien.MaSanPhamTinhLuong,
                                    nhanVien.MaHoSo,
                                    nhanVien.Name,
                                    nhanVien.GioBatDau,
                                    nhanVien.GioKetThuc,
                                    NhomName = nhom.Ten
                                }
                            )
                            //.Join(
                            //    HQ_NhanVienTheoCaViewModel.Instance.Gets<HQ_NhanVienTheoCa>(toDateWithTime),
                            //    nhanVien => nhanVien.MaNhanVien,
                            //    nhanVienTheoCa => nhanVienTheoCa.NhanVienId,
                            //    (nhanVien, nhanVienTheoCa) => new
                            //    {
                            //        nhanVien.MaNhom,
                            //        nhanVien.MaNhanVien,
                            //        nhanVien.HeSo,
                            //        nhanVien.TongHeSoNhom,
                            //        nhanVien.TrongLuong,
                            //        nhanVien.TrongLuongPhanBo,
                            //        nhanVien.MaThanhPham,
                            //        nhanVien.MaHoSo,
                            //        nhanVien.Name,
                            //        nhanVien.NhomName,
                            //        CaId = nhanVienTheoCa.CaId
                            //    }
                            //)
                            //.Join(
                            //    HQ_CaViewModel.Instance.Gets<HQ_Ca>(),
                            //    nhanvien => int.Parse(nhanvien.CaId),
                            //    ca => ca.Id,
                            //    (nhanVien, ca) => new
                            //    {
                            //        nhanVien.MaNhom,
                            //        nhanVien.MaNhanVien,
                            //        nhanVien.HeSo,
                            //        nhanVien.TongHeSoNhom,
                            //        nhanVien.TrongLuong,
                            //        nhanVien.TrongLuongPhanBo,
                            //        nhanVien.MaThanhPham,
                            //        nhanVien.MaHoSo,
                            //        nhanVien.Name,
                            //        nhanVien.NhomName,
                            //        nhanVien.CaId,
                            //        CaName = ca.Ten // Lấy tên ca
                            //    }
                            //)
                            .ToList();

                        // Cập nhật dữ liệu vào danh sách items
                        var updatedItems = danhSachNhanVienWithData.Select(nhanVien => new
                        {
                            Ngay = nhanVien.Ngay,
                            //CaId = nhanVien.CaId,
                            //CaName = nhanVien.CaName,
                            MaNhanVien = nhanVien.MaNhanVien,
                            MaHoSo = nhanVien.MaHoSo,
                            NhanVienName = nhanVien.Name,
                            MaNhom = nhanVien.MaNhom,
                            NhomName = nhanVien.NhomName,
                            MaThanhPham = nhanVien.MaThanhPham,
                            ThanhPhamName = filterNhanVienNhoms.FirstOrDefault(item =>
                                ((dynamic)item).MaThanhPham == nhanVien.MaThanhPham)?.ThanhPhamName,
                            MaSanPhamTinhLuong = nhanVien.MaSanPhamTinhLuong,
                            SanPhamTinhLuongName = filterNhanVienNhoms.FirstOrDefault(item =>
                                ((dynamic)item).MaSanPhamTinhLuong == nhanVien.MaSanPhamTinhLuong)?.SanPhamTinhLuongName,
                            TrongLuong = nhanVien.TrongLuongPhanBo,
                            GioBatDau = nhanVien.GioBatDau,
                            GioKetThuc = nhanVien.GioKetThuc
                        }).ToList();

                        items.AddRange(updatedItems.Cast<T>());
                        // Loại bỏ các phiếu có trong filterNhanVienNhoms
                        var maNhanViensFilter = filterNhanVienNhoms.Select(x => ((dynamic)x).MaNhanVien).ToList();
                        items.RemoveAll(item =>
                        {
                            dynamic dynamicItem = item;
                            return maNhanViensFilter.Contains((string)dynamicItem.MaNhanVien);
                        });
                    }

                }



                return items;
            }

            return new List<T>(); ;
        }



        public List<T> GetChiTietXLPCs<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetChiTietXLPCs<T>(dateTime, xuongId);
        }
        public List<T> GetPhieuCanUpdateXLPCs<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetPhieuCanUpdateXLPCs<T>(dateTime, xuongId);
        }
        public List<T> GetPhieuCanDeleteXLPCs<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetPhieuCanDeleteXLPCs<T>(dateTime, xuongId);
        }

        public T? Get<T>(string id, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Get<T>(id);
        }

        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan();
            return dao.GetsLast<T>(dateTime, num);
        }
        public int Insert<T>(T item, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Insert(item);
        }
        public int Update<T>(T item, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Update(item);
        }
        public int Delete<T>(T item, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Delete(item);
        }
        //(Id, STT, NgayGio, MayCan, MaLo, MaSize, MaThanhPham, MaNhanVien, TrongLuongNhan, TrongLuongTra, TrongLuongTare, TheId, TheChucNang, Ngay, MaLoaiNguyenLieu);
        public Models.Repos.Models.HQ_PhieuCan CreateNew(string id, int stt, string malo, string maloainguyenlieu, string masize, string mathanhpham, string manhanvien, decimal trongluongnhan, decimal trongluongtra, decimal trongluongtare, string theid, string thechucnang, DateTime ngayGio, string maycan, string maxuong)
        {
            return new HQ_PhieuCan()
            {
                Id = id,
                MaLo = malo,
                MaLoaiNguyenLieu = maloainguyenlieu,
                MaSize = masize,
                MaThanhPham = mathanhpham,
                MayCan = maycan,
                TheId = theid,
                TheIdNhanVien = theid,
                NgayGio = ngayGio,
                ChiSanLuong = true,
                GhiChu = "",
                MaNhanVien = manhanvien,
                MaNhanVienBanKiem = "",
                MaNhanVienPhucVu = "",
                MaXuong = maxuong,
                Ngay = DateOnly.FromDateTime(ngayGio),
                STT = stt,
                Status = 1,
                TrongLuong = trongluongnhan,
                TrongLuongTare = trongluongtare,

            };

        }
    }
}
