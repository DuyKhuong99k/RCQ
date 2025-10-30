using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamCaoThit))] // "MaThanhPhamCaoThit
public partial class MaThanhPhamCaoThit
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }
}
