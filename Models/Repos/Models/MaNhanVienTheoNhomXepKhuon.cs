using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaNhanVienTheoNhomXepKhuon))] // "MaNhanVienTheoNhomXepKhuon
[PrimaryKey("MaNhanVien", "MaNhom", "Ngay")]
public partial class MaNhanVienTheoNhomXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhom { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TyLeHuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TyLeTru { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoGio { get; set; }
}
