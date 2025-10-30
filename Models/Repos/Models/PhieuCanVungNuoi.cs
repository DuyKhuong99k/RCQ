using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanVungNuoi")]
[PrimaryKey("STT", "Ngay", "MaMayCan")]
public partial class PhieuCanVungNuoi
{
    [Key]
    public int STT { get; set; }

    [Key]
    public DateOnly Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    public DateOnly NgayNhapXuong { get; set; }

    public TimeOnly Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaGhe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaAo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaCongDoan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThongKeDauAo { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TrongLuongTare { get; set; }

    public double TrongLuong { get; set; }
}
