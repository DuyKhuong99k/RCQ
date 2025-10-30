using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MPG_CongThucChiTiet))]
public partial class MPG_CongThucChiTiet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaCongThuc { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TrongLuongDaCan { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TyLe { get; set; }

    public bool IsFull { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal PhuTroi { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal PhuTroiMax { get; set; }

    public bool SuDung { get; set; }

    public bool IsXacNhan { get; set; }
}
