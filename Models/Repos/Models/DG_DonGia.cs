using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DG_DonGia))] // "DG_DonGia
public partial class DG_DonGia
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
    public string MaSanPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiDonGia { get; set; } = null!;

    public bool DanhGia { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucDown { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DinhMucUp { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal HeSo { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DonGia { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;

    public DateTime CreateDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedDateTime { get; set; }

    public string? GhiChu { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal HeSoRot { get; set; }

    public bool IsUsedHeSoRot { get; set; }

    public int Range { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeDinhHinh { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal DonGiaGiaCong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaXepHang { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPham { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? LoaiCan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeFillet { get; set; }
}
