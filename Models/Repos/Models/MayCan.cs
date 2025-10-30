using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using Vars;

namespace Models.Repos.Models;

[Table(nameof(MayCan))] // "MayCan
public class MayCan
{
    private DateTime _ngayNguyenLieu;
    public AppType AType { get; set; } = AppType._default;
    [NotMapped] public string? BanId { get; set; }
    [NotMapped] public bool CaTra { get; set; }
    [NotMapped] public int? ChuyenNL { get; set; } = 0;
    [NotMapped] public string? ColorString { get; set; }

    [NotMapped]
    [StringLength(100)]
    [Unicode(false)]
    public string? ConnectionId { get; set; }


    [NotMapped] public DateTime DateTimeConnected { get; set; } = DateTime.Now;

    [StringLength(500)] public string DisplayName { get; set; }

    /// <summary>
    ///     Id(tên) máy Cân
    /// </summary>
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; }

    [NotMapped] public string? IdIn { get; set; }
    [NotMapped] public string? IdMonitor { get; set; }
    public int Idx { get; set; }


    [StringLength(200)] public string? IPAddr { get; set; }
    [NotMapped] public bool IsActive { get; set; } = false;

    [NotMapped] public bool IsConnected { get; set; } = false;
    [NotMapped] public bool IsExpand { get; set; } = false;
    [NotMapped] public bool IsHuman { get; set; }
    [NotMapped] public bool IsKhongTheDauVao { get; set; }
    [NotMapped] public bool IsNapThe { get; set; }
    [NotMapped] public bool IsNhanVienCongCu { get; set; }
    [NotMapped] public bool IsStated { get; set; }
    public bool IsSuDungMauThanhPham { get; set; }

    [NotMapped] public ObservableRangeCollection<object> Items { get; set; } = new();


    [NotMapped] public string? MaAo { get; set; }
    [NotMapped] public string? MaChatLuong { get; set; }
    [NotMapped] public string? MaChieuXa { get; set; }
    [NotMapped] public string? MaCoi { get; set; }

    [NotMapped] public string? MaCoiTam { get; set; }
    [NotMapped] public string? MaHoSo { get; set; }
    [NotMapped] public string? MaKhachHang { get; set; }

    [NotMapped] public string MaLo { get; set; }
    [NotMapped] public string? MaLoaiCa { get; set; }
    [NotMapped] public string? MaLoaiNguyenLieu { get; set; }
    [NotMapped] public string? MaMau { get; set; }
    [NotMapped] public string? MaNhaCungCap { get; set; }
    [NotMapped] public string? MaNhanVien { get; set; }
    [NotMapped] public string? MaNhanVienPhucVu { get; set; }
    [NotMapped] public string? MaPhuongTien { get; set; }
    [NotMapped] public string MaSize { get; set; }
    [NotMapped] public string MaThanhPham { get; set; }
    [StringLength(50)] [Unicode(false)] public string MaXuong { get; set; }
    [NotMapped] public string? MayCanIdBTP { get; set; }

    /// <summary>
    ///     "DESKTOP": Máy cân chạy nền window
    ///     "BOARD": Máy cân chạy trên board
    ///     "NONE": Chưa xác định
    /// </summary>
    [StringLength(100)]
    [Unicode(false)]
    public string MType { get; set; } = "NONE";

    [NotMapped] public string NapTheId { get; set; } = "";

    [NotMapped]
    public DateTime NgayNguyenLieu
    {
        get => _ngayNguyenLieu;
        set
        {
            _ngayNguyenLieu = value;
            NgayNguyenLieuDateTimeChanged = DateTime.Now;
        }
    }

    [NotMapped] public DateTime NgayNguyenLieuDateTimeChanged { get; set; } = DateTime.Now;
    [NotMapped] public string? NhanVienName { get; set; }
    public string Par1 { get; set; }
    public string Par2 { get; set; }
    public string Par3 { get; set; }
    public string Par4 { get; set; }
    [NotMapped] public string? Pheu { get; set; }
    public string SCode { get; set; }
    [NotMapped] public int? STTBTP { get; set; }
    [NotMapped] public string SubTitle => $@"{DisplayName} - {MaXuong} - {MType} ";
    [NotMapped] public string? The { get; set; }
    [NotMapped] public DateTime? TheAddDateTime { get; set; }
    [NotMapped] public string? TheNhanVien { get; set; }
    [NotMapped] public DateTime? TheNhanVienAddDateTime { get; set; }
    [NotMapped] public DateTime? TheNhanVienPhucVuAddDateTime { get; set; }
    [NotMapped] public string? ThePhieuSanLuongId { get; set; }
    [NotMapped] public string? TheRo { get; set; }
    [NotMapped] public DateTime? TheRoAddDateTime { get; set; }
    [NotMapped] public string? TheView { get; set; }
    [NotMapped] public TimeSpan ThoiGianBTP { get; set; } = new(0, 0, 0);
    [NotMapped] public string? ThongBao { get; set; }

    [NotMapped] public int TongSoRo { get; set; }
    [NotMapped] public decimal TongTrongLuong { get; set; } = decimal.Zero;
    [NotMapped] public decimal TrongLuong { get; set; }

    [NotMapped] public decimal TrongLuongNhan { get; set; }
    [NotMapped] public decimal? TrongLuongTare { get; set; } = 0;
    [NotMapped] public decimal TyLeNuoc { get; set; }
    [NotMapped] public bool IsChiSangLuong { get; set; } = false;
    [NotMapped] public decimal? ThamSoTangTrong { get; set; } = 0M;
    [NotMapped] public string? MaNet { get; set; }
    [NotMapped] public string? MaCongViec { get; set; }
    [NotMapped] public string? ErrorString {get; set; }
    [NotMapped] public bool IsXacDinhLoaiThanhPham { get; set; } = false;
    public AppKV WKv { get; set; } = 0;

    public void CopyFrom(MayCan other)
    {
        if (other == null) return;

        // Các trường cơ bản
        DisplayName = other.DisplayName;
        SCode = other.SCode;
        Par1 = other.Par1;
        Par2 = other.Par2;
        Par3 = other.Par3;
        Par4 = other.Par4;
        Idx = other.Idx;

        

        // Các trường runtime
        // ConnectionId = other.ConnectionId;
        // IsConnected = other.IsConnected;
        // IPAddr = other.IPAddr;
        // DateTimeConnected = other.DateTimeConnected;
        // IsActive = other.IsActive;
        // IsExpand = other.IsExpand;
        if (WKv != other.WKv ||  AType != other.AType)
        {
            IsSuDungMauThanhPham = other.IsSuDungMauThanhPham;
            MaLo = other.MaLo;
            MaThanhPham = other.MaThanhPham;
            MaSize = other.MaSize;
            TrongLuongTare = other.TrongLuongTare;
            TrongLuong = other.TrongLuong;
            IsStated = other.IsStated;
            TrongLuongNhan = other.TrongLuongNhan;
            TheRo = other.TheRo;
            TheRoAddDateTime = other.TheRoAddDateTime;
            TheNhanVienAddDateTime = other.TheNhanVienAddDateTime;
            TheNhanVienPhucVuAddDateTime = other.TheNhanVienPhucVuAddDateTime;
            TheNhanVien = other.TheNhanVien;
            MaNhanVien = other.MaNhanVien;
            MaHoSo = other.MaHoSo;
            MaNhanVienPhucVu = other.MaNhanVienPhucVu;
            The = other.The;
            TheAddDateTime = other.TheAddDateTime;
            TheView = other.TheView;

            TongSoRo = other.TongSoRo;
            TongTrongLuong = other.TongTrongLuong;
            // Items.Clear();
            // foreach (var otherItem in other.Items)
            // {
            //     Items.Add(otherItem);
            // }

            STTBTP = other.STTBTP;
            MayCanIdBTP = other.MayCanIdBTP;
            BanId = other.BanId;
            ThePhieuSanLuongId = other.ThePhieuSanLuongId;
            IdIn = other.IdIn;
            ThongBao = other.ThongBao;
            ColorString = other.ColorString;
            MaLoaiCa = other.MaLoaiCa;
            MaMau = other.MaMau;
            CaTra = other.CaTra;
            IsNapThe = other.IsNapThe;
            NapTheId = other.NapTheId;
            MaChieuXa = other.MaChieuXa;
            MaChatLuong = other.MaChatLuong;
            IdMonitor = other.IdMonitor;
            MaCoi = other.MaCoi;
            MaCoiTam = other.MaCoiTam;
            NgayNguyenLieu = other.NgayNguyenLieu;
            NhanVienName = other.NhanVienName;
            MaAo = other.MaAo;
            MaNhaCungCap = other.MaNhaCungCap;
            MaPhuongTien = other.MaPhuongTien;
            ChuyenNL = other.ChuyenNL;
            Pheu = other.Pheu;
            TyLeNuoc = other.TyLeNuoc;
            MaKhachHang = other.MaKhachHang;
            MaLoaiNguyenLieu = other.MaLoaiNguyenLieu;
            IsKhongTheDauVao = other.IsKhongTheDauVao;
            ThoiGianBTP = other.ThoiGianBTP;
            IsNhanVienCongCu = other.IsNhanVienCongCu;
            IsHuman = other.IsHuman;
        }
        MaXuong = other.MaXuong;
        MType = other.MType;
        WKv = other.WKv;
        AType = other.AType;
    }
}