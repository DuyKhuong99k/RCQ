using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhuongTien_TaiTrong")]
[PrimaryKey("MaPhuongTien", "NgayApDung")]
public partial class PhuongTien_TaiTrong
{
    [Key]
    [StringLength(50)]
    public string MaPhuongTien { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime NgayApDung { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? TaiTrong { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }
}
