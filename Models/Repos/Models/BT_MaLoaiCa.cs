using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BT_MaLoaiCa))] // "BT_MaLoaiCa
public partial class BT_MaLoaiCa
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }
}
