using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(KNH_PhieuCan))] // "KNH_PhieuCan
[PrimaryKey("STT", "Ngay", "MayCan", "MaXuong")]
public partial class KNH_PhieuCan
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MayCan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThongTinSanPham { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuongKiemLai { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal SoLuongPhanTu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal PhuTroi { get; set; }

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
