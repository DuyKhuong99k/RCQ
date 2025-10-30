using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaLoaiCaVungNuoi))] // "MaLoaiCaVungNuoi
public partial class MaLoaiCaVungNuoi
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool CanLai { get; set; }

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? DaiThanhId { get; set; }

    public bool IsTap { get; set; }
}
