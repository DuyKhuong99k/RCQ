using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamChinhXepKhuon))] // "MaThanhPhamChinhXepKhuon
public partial class MaThanhPhamChinhXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    public double Min { get; set; }

    public double Max { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoId { get; set; }

    public int _type { get; set; }
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime MNgay { get; set; } = DateTime.Now;
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ThamSoTangTrong { get; set; } = 1M;

    [Column(TypeName = "decimal(18, 2)")] public decimal DinhMucTangTrong { get; set; } = 0M;
    public bool IsKhongThuc { get; set; }
}
