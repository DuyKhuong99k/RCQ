using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaMauCaGiongVungNuoi))] // "MaMauCaGiongVungNuoi
[PrimaryKey("Ngay", "MaGhe")]
public partial class MaMauCaGiongVungNuoi
{
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaGhe { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongDonVi { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; }
}
