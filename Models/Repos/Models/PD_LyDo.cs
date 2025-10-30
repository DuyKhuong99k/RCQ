using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PD_LyDo")] // "PD_LyDo
public partial class PD_LyDo
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string Ten { get; set; } = null!;

    public string DienGiai { get; set; } = null!;

    public string? GhiChu { get; set; }
}
