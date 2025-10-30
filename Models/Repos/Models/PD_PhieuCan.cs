using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PD_PhieuCan")] // "PD_PhieuCan
[PrimaryKey("STT", "Ngay", "MaMayCan", "MaXuong")]
public partial class PD_PhieuCan
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
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TrongLuongNguyenLieu { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TrongLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCoi { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaCongThuc { get; set; } = null!;

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLyDo { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime CreateDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;

    public DateTime ModifiedDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ModifiedBy { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    public bool Khoa { get; set; }

    public int Block { get; set; }
}
