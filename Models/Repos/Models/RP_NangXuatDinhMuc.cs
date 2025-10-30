using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RP_NangXuatDinhMuc")] // Added
[PrimaryKey("MaXuong", "MaLo", "Ngay", "Gio", "MaNhanVien", "ThanhPham", "Size", "CaTra")]
public partial class RP_NangXuatDinhMuc
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Key]
    public DateOnly Ngay { get; set; }

    [Key]
    public TimeOnly Gio { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [StringLength(200)]
    public string ThanhPham { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Size { get; set; } = null!;

    [Key]
    public bool CaTra { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DinhMucGio { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? NangXuatGio { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TLNhan { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TLTra { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DinhMuc { get; set; }
}
