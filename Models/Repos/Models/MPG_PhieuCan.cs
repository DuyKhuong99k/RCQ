using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("MPG_PhieuCan")]
[PrimaryKey("STT", "Ngay", "MayCan", "MaXuong")]
public partial class MPG_PhieuCan
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
    public string MaCongThucChiTiet { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal PhuTroi { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuong { get; set; }

    public int Block { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuongCongThuc { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
