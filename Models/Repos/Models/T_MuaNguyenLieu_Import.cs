using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_MuaNguyenLieu_Import")] // Added
public partial class T_MuaNguyenLieu_Import
{
    [Key]
    public long Id { get; set; }

    [StringLength(300)]
    public string? CongTy { get; set; }

    [StringLength(300)]
    public string? Zone_XN { get; set; }

    [StringLength(300)]
    public string? HinhThucCan { get; set; }

    [StringLength(300)]
    public string? LoaiTom { get; set; }

    public DateTime? NgayNguyenLieu { get; set; }

    [StringLength(300)]
    public string? MaDaiLy { get; set; }

    [StringLength(300)]
    public string? SoLo { get; set; }

    [StringLength(300)]
    public string? GopLo { get; set; }

    [StringLength(300)]
    public string? TenLai { get; set; }

    [StringLength(300)]
    public string? NhomKS { get; set; }

    [StringLength(300)]
    public string? MaLoaiNguyenLieu { get; set; }

    [StringLength(300)]
    public string? MaSizeCo { get; set; }

    [StringLength(300)]
    public string? TenSizeCo { get; set; }

    [StringLength(300)]
    public string? SoCon_lb { get; set; }

    [StringLength(300)]
    public string? MaNgayNguyenLieu { get; set; }

    [StringLength(300)]
    public string? Con_lb { get; set; }

    public int? SoRo { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? KG_Loai1 { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? KG_Loai2 { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? KG_Dat { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TrongLuong { get; set; }

    [StringLength(300)]
    public string UserName { get; set; } = null!;

    [StringLength(300)]
    public string PCName { get; set; } = null!;

    public DateTime NgayImport { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? FileName { get; set; }
}
