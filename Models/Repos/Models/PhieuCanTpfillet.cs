using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanTPFillet")]
[PrimaryKey("MaMayTinhCan", "MaUserCan", "ThoiGianCan", "Ngay")]
public partial class PhieuCanTPFillet
{
    [Key]
    [StringLength(50)]
    public string MaMayTinhCan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string MaUserCan { get; set; } = null!;

    [Key]
    [Column(TypeName = "time(7)")]
    public TimeSpan ThoiGianCan { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [StringLength(50)]
    public string? MaXuongSanXuat { get; set; }

    [StringLength(50)]
    public string? MSL { get; set; }

    [StringLength(50)]
    public string? MaLoaiCa { get; set; }

    [StringLength(50)]
    public string? MaLoaiThanhPham { get; set; }

    [StringLength(50)]
    public string? MaSize { get; set; }

    [StringLength(50)]
    public string? MaMau { get; set; }

    [StringLength(50)]
    public string? MaNhanVien { get; set; }

    [StringLength(100)]
    public string? HoVaTen { get; set; }

    [StringLength(50)]
    public string? MaTheTu { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TrongLuong { get; set; }

    public bool? SuDung { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(50)]
    public string? MaNhanVienPhucVu { get; set; }

    /// <summary>
    /// TP &amp; BTP
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string LoaiCan { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }
    
    [StringLength(50)]
    [Unicode(false)]
    public string? Id { get; set; }
}
