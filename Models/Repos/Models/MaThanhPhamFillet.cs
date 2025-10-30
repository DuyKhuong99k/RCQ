using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamFillet))] // "MaThanhPhamFillet
[PrimaryKey("MaCa", "Ma")]
public partial class MaThanhPhamFillet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCa { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public bool IsSoChe { get; set; }

    public bool IsCaMuoi { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMuc { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TrangThaiThanhPham { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongHienTai { get; set; }

    /// <summary>
    /// Don Vi Giay
    /// </summary>
    [Column(TypeName = "decimal(18, 3)")]
    public decimal ThoiGianTren1kgSeconds { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucHaoHut { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;

    public bool IsNotSetByTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ColorRGB { get; set; }

    public bool KhongPhanBietSize { get; set; }
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime MNgay { get; set; } = DateTime.Now;
    public bool IsXeBuom { get; set; } = false;
    public bool IsChuyenFillet { get; set; } = false;
    public bool IsNguyenLieuXeBuom { get; set; } = false;
    /// <summary>
    /// Dạt
    /// </summary>
    public bool IsDat { get; set; } = false;
    public bool IsGiaoXepKhuon { get; set; } = false;
    public bool IsNguyenCon { get; set; } = false;
    public bool IsNguyenConNXB { get; set; } = false;
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ThoiGianHT { get; set; } = 30M;
}
