using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_ChiTietBon")] // Added
public partial class T_ChiTietBon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaBon { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    public int Luot { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan GioBatDau { get; set; }

    [Column(TypeName = "date")]
    public DateTime? NgayKetThuc { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan? GioKetThuc { get; set; }

    public string? GhiChu { get; set; }

    public bool DaHoanThanh { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTP { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongConLai { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public bool DaChuyen { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaBonGoc { get; set; }
}
