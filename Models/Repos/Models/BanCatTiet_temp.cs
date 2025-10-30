using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BanCatTiet_temp))]
public partial class BanCatTiet_temp
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }

    public int X1 { get; set; }

    public int Y1 { get; set; }

    public int X2 { get; set; }

    public int Y2 { get; set; }

    public int ColSpanX1 { get; set; }

    public int ColSpanX2 { get; set; }

    public bool IsShowX1 { get; set; }

    public bool IsShowX2 { get; set; }

    public bool IsDetect { get; set; }

    public bool IsFalse { get; set; }
}
