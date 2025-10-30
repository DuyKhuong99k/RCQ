using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(KNH_ThongTinSanPham))] // "KNH_ThongTinSanPham
public partial class KNH_ThongTinSanPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal PhuTroi { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal SoLuongPhanTu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaQuyCach { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }

    /// <summary>
    /// Thuoc Tinh, Gia Tri
    /// </summary>
    public string? JsonValue { get; set; }

    public DateTime CreateDateTime { get; set; }
}
