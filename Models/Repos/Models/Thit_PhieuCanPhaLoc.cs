using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("Thit_PhieuCanPhaLoc")] // This is the table name in the database
[PrimaryKey("STT", "MaMayCan", "Ngay", "MaXuong")]
public partial class Thit_PhieuCanPhaLoc
{
    [Key]
    public int STT { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiNguyenLieu { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiThanhPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVienPhucVu { get; set; }

    /// <summary>
    /// 0 nhap, 1 xuat
    /// </summary>
    public bool InOut { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ItemCode { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLoi { get; set; }

    public bool IsLoi { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    public string? GhiChu { get; set; }

    public bool IsNL { get; set; }

    public bool IsSX { get; set; }
}
