using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BoTriTinhLuonXepKhuon))]
[PrimaryKey("MaNhanVien", "Ngay", "MaCongViec", "MaXuong")]
public partial class BoTriTinhLuonXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public bool IsNhom { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TyLeHuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TyLeTru { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoGio { get; set; }
}
