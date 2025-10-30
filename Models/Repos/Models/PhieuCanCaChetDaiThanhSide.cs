using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanCaChetDaiThanhSide")] // "PhieuCanCaChetDaiThanhSide
[PrimaryKey("STT", "Ngay", "MaMayCan", "BiosId")]
public partial class PhieuCanCaChetDaiThanhSide
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    public string? GhiChu { get; set; }

    [StringLength(200)]
    public string TenLoaiCa { get; set; } = null!;

    [StringLength(200)]
    public string TenKhachHang { get; set; } = null!;

    [StringLength(200)]
    public string TenThongKe { get; set; } = null!;

    [StringLength(200)]
    public string TenAo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuongBinhQuan { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }

    [Key]
    [StringLength(500)]
    [Unicode(false)]
    public string BiosId { get; set; } = null!;

    [Column(TypeName = "time(7)")]
    public TimeSpan GioTai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayTai { get; set; }
}
