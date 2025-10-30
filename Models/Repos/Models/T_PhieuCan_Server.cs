using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_PhieuCan_Server")] // This is the table name in the database
public partial class T_PhieuCan_Server
{
    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    public int? STT { get; set; }

    public DateOnly? Ngay { get; set; }

    public TimeOnly? Gio { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? MaNhanVien { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? MaHoSo { get; set; }

    [StringLength(200)]
    public string? TenNhanVien { get; set; }

    [StringLength(200)]
    public string? Nhom { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? MaLo { get; set; }

    public DateOnly? NgayNguyenLieu { get; set; }

    [StringLength(200)]
    public string? SanPham { get; set; }

    [StringLength(200)]
    public string? QuyTrinh { get; set; }

    public bool? DatKhangSinh { get; set; }

    [StringLength(200)]
    public string? Size { get; set; }

    [StringLength(200)]
    public string? SizeTP { get; set; }

    [StringLength(200)]
    public string? CongViec { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? VoXo { get; set; }

    [StringLength(200)]
    public string? VoXoGiaoDong { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TrongLuongTare { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? MaKhachHang { get; set; }

    [StringLength(200)]
    public string? KhangSinh { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? ThongTinNguyenLieu { get; set; }

    public bool? PhanCo { get; set; }

    public int? SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TrongLuongDonVi { get; set; }

    public bool? CanTay { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(200)]
    public string? KhachHang { get; set; }

    [StringLength(200)]
    public string? PhuGia { get; set; }

    [StringLength(200)]
    public string? NhomHoaChat { get; set; }

    [StringLength(200)]
    public string? QuyCach { get; set; }

    [StringLength(200)]
    public string? ThongTinPhu { get; set; }

    [StringLength(200)]
    public string? GhiChu2 { get; set; }

    [StringLength(200)]
    public string? GhiChu3 { get; set; }
}
