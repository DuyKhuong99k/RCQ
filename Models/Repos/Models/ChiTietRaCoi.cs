using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(ChiTietRaCoi))] // "ChiTietRaCoi
public partial class ChiTietRaCoi
{
    [Key]
    public long Id { get; set; }

    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaCoi { get; set; } = null!;

    public int Luot { get; set; }

    public bool IsDone { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayNguyenLieu { get; set; }
}
