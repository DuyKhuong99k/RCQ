using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamXepKhuonKHC))] // "MaThanhPhamXepKhuonKHC
public partial class MaThanhPhamXepKhuonKHC
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    public double Min { get; set; }

    public double Max { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoId { get; set; }
}
