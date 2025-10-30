using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_NhomLo")]
public partial class T_NhomLo
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public bool SuDung { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "date")]
    public DateTime? NgayKetThuc { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayNguyenLieu { get; set; }

    public bool DaKetThuc { get; set; }

    public string? GhiChu { get; set; }
}
