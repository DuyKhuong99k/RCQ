using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanVungNuoiDaiThanhSide")]
[PrimaryKey("STT", "Ngay", "MaMayCan", "BiosId")]
public partial class PhieuCanVungNuoiDaiThanhSide
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

    [Key]
    [StringLength(500)]
    [Unicode(false)]
    public string BiosId { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime NgayTai { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan GioTai { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    public string? GhiChu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCaDaiThanhId { get; set; } = null!;

    [StringLength(300)]
    public string TenLoaiCa { get; set; } = null!;

    public bool CanLai { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaGhe { get; set; } = null!;

    [StringLength(500)]
    public string TenGhe { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string TenAo { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string TenCongDoan { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string? TenThongKeDauAo { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TrongLuongTare { get; set; }

    public double TrongLuong { get; set; }

    public bool IsTap { get; set; }
}
