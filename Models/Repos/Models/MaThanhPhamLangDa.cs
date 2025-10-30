using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamLangDa))] // "MaThanhPhamLangDa
[PrimaryKey("MaCa", "Ma")]
public partial class MaThanhPhamLangDa
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCa { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoId { get; set; }
}
