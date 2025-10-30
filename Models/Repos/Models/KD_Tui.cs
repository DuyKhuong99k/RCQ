using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(KD_Tui))] // "KD_Tui
public partial class KD_Tui
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaDonHang { get; set; } = null!;

    [Column(TypeName = "decimal(18, 5)")]
    public decimal PhuTroi { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal PhuTroiMax { get; set; }

    [Column(TypeName = "decimal(18, 5)")]
    public decimal TongTrongLuong { get; set; }

    public bool SuDung { get; set; }

    public DateOnly CreateDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaQuyCach { get; set; } = null!;
}
