using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(ColorCode))] // "ColorCode
[PrimaryKey("Code", "Code2")]
public partial class ColorCode
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Code2 { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? ColorRGB { get; set; }
    [Required]
    [Column(TypeName = "datetime2(7)")] public DateTime MNgay { get; set; } = DateTime.Now;
}

