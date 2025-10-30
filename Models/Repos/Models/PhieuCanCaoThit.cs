using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanCaoThit")] // "PhieuCanCaoThit
[PrimaryKey("STT", "Ngay", "MaMayCan", "MaXuong")]
public partial class PhieuCanCaoThit
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaQuyCach { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongSec { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucThucTe { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucYeuCau { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    public string? GhiChu { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongTare { get; set; }
}
