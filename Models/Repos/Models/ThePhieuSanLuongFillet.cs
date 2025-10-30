using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("ThePhieuSanLuongFillet")] // This is the table name in the database
public partial class ThePhieuSanLuongFillet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaMau { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    public string MaLo { get; set; } = null!;

    public bool CaTra { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public int STT_PC { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MayCan_PC { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongTare { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaBan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    public bool IsDone { get; set; }

    public int STT_PC_BTP { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MayCan_PC_BTP { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongNhan { get; set; }

    public string? GhiChu { get; set; }
    [StringLength(100)]
    [Unicode(false)]
    public string? IdIn { get; set; }
}
