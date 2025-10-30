using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_LenhSanXuat")] // Added
public partial class T_LenhSanXuat
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public bool HoanThanh { get; set; }

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }
}
