using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanCaChet")] // "PhieuCanCaChet
[PrimaryKey("STT", "Ngay", "MaMayCan")]
public partial class PhieuCanCaChet
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
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThongKe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaAo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuongBinhQuan { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }
}
