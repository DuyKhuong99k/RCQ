using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_PhieuCan")] // This is the table name in the database
public partial class T_PhieuCan
{
    /// <summary>
    /// =yyyyMMdd.xinghiepId.PCName(no space and unicode).STT
    /// </summary>
    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    public int STT { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MayCan { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayNguyenLieu { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLoaiNguyenLieu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhomLo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVien { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaGroup { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLoaiKhuon { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaQuyCach { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongNhan { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal DinhMuc { get; set; }

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhaCungCap { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaPhuongTien { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLenhSanXuat { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVienPhucVu { get; set; }

    /// <summary>
    /// T: Tay, M: Máy
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCongViec { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVien2 { get; set; }

    public bool IsEnabled { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSanPham { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCongDoan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaBon { get; set; }

    public int Luot { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaQuyTrinh { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaDonHang { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKhachHang { get; set; }

    public bool IsDatKhangSinh { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKhangSinh { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal VoXo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThongTinPhu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaPhuGia { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaTrangThaiNguyenLieu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaQuyTrinhOrg { get; set; }

    public bool IsNgam { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal T { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaTyLeNhomHoaChat { get; set; }

    public bool IsCanTay { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongDonVi { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKhachHangOrg { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeOrg { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaPhuGiaOrg { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhomHoaChat { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeTP { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaPhieuPhanCo { get; set; }

    public string? GhiChu2 { get; set; }

    public string? GhiChu3 { get; set; }

    [StringLength(50)]
    public string? VoXo2 { get; set; }

    public int Status { get; set; }
}
