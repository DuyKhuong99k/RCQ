using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaNetXepKhuon))] // "MaNetXepKhuon
public partial class MaNetXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public double TrongLuong { get; set; }

    public double BienDo { get; set; }

    public bool SuDung { get; set; }
}
