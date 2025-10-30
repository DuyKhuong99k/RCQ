using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RP_SanLuongDenThoiDiem")] // Added
[PrimaryKey("KhuVuc", "MaLo", "MaXuong", "Ngay", "ThanhPham", "Size")]
public partial class RP_SanLuongDenThoiDiem
{
    [Key]
    [StringLength(50)]
    public string KhuVuc { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    public DateOnly Ngay { get; set; }

    [Key]
    [StringLength(200)]
    public string ThanhPham { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string Size { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? DinhMucLangDa { get; set; }

    [StringLength(200)]
    public string? MaNhom { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTrognLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? MaxTongTrongLuong { get; set; }

    [StringLength(200)]
    public string? ThanhPhamId { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal? Tyle { get; set; }
}
