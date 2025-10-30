using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_PhieuCanThuMua_Server")] // This is the table name in the database
public partial class T_PhieuCanThuMua_Server
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    public int? STT { get; set; }

    public TimeOnly? Gio { get; set; }

    public DateOnly? NgayNguyenLieu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaHoSo { get; set; }

    [StringLength(200)]
    public string? TenNhanVien { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Nhom { get; set; }

    [StringLength(200)]
    public string? Xuong { get; set; }

    [StringLength(200)]
    public string? LoaiNguyenLieu { get; set; }

    [StringLength(200)]
    public string? TieuChuan { get; set; }

    [StringLength(200)]
    public string? Size { get; set; }

    [StringLength(200)]
    public string? BaoBi { get; set; }

    [StringLength(200)]
    public string? SanPham { get; set; }

    [StringLength(200)]
    public string? QuyTrinh { get; set; }

    [StringLength(200)]
    public string? PhuGia { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? MaKhachHang { get; set; }

    [StringLength(200)]
    public string? KhachHang { get; set; }

    [StringLength(200)]
    public string? KhangSinh { get; set; }

    [StringLength(200)]
    public string? ThongTinPhu { get; set; }

    [StringLength(200)]
    public string? CongDoan { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VoXoTB { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VoXoCD { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? VoXoCT { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TrongLuongTare { get; set; }

    public string? GhiChu { get; set; }
}
