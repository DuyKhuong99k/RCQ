using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaSizeNguyenLieu))] // "MaSizeNguyenLieu
public partial class MaSizeNguyenLieu
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime MNgay { get; set; } = DateTime.Now;
}
