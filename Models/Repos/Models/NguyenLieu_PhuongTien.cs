using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NguyenLieu_PhuongTien")] // "NguyenLieu_PhuongTien
public partial class NguyenLieu_PhuongTien
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    [Column(TypeName = "numeric(18, 0)")]
    public decimal? TaiTrong { get; set; }

    public bool? SuDung { get; set; }

    public bool? HienThi { get; set; }
}
