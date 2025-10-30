using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanPhuPham")] // "PhieuCanPhuPham
[PrimaryKey("MaMayTinhCan", "MaUserCan", "ThoiGianCan", "NgayCan")]
public partial class PhieuCanPhuPham
{
    [Key]
    [StringLength(50)]
    public string MaMayTinhCan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string MaUserCan { get; set; } = null!;

    [Key]
    [Column(TypeName = "datetime")]
    public DateTime ThoiGianCan { get; set; }

    [Column(TypeName = "date")]
    public DateTime? Ngay { get; set; }

    [StringLength(50)]
    public string? NhaMuaHang { get; set; }

    [StringLength(50)]
    public string? MSL { get; set; }

    [StringLength(50)]
    public string? MaPhuongTien { get; set; }

    [StringLength(50)]
    public string? MaLoaiCa { get; set; }

    [StringLength(50)]
    public string? MaLoaiThanhPham { get; set; }

    [StringLength(50)]
    public string? MaSize { get; set; }

    [StringLength(50)]
    public string? MaMau { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TrongLuong { get; set; }

    public bool? SuDung { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(50)]
    public string? MaXuongSanXuat { get; set; }

    [Key]
    [Column(TypeName = "datetime")]
    public DateTime NgayCan { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }
    
    [StringLength(50)]
    [Unicode(false)]
    public string? Id { get; set; }
}
