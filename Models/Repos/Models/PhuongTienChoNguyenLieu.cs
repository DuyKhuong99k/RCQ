using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhuongTienChoNguyenLieu")]
public partial class PhuongTienChoNguyenLieu
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }

    public bool IsHD { get; set; }

    public bool IsGhe { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string VungNuoiId { get; set; } = null!;

    [StringLength(50)]
    public string? SoGhe { get; set; }
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime MNgay { get; set; } = DateTime.Now;
}
