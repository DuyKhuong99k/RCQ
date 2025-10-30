using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MayPhanCo))]
public partial class MayPhanCo
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }
}
