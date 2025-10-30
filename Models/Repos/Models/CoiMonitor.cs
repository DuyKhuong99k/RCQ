using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(CoiMonitor))] // "CoiMonitor
public partial class CoiMonitor
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaCoi { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    public DateTime NgayGio { get; set; }

    public bool InLocked { get; set; }

    public bool OutLocked { get; set; }
    [Column(TypeName = "date")]
    public DateTime NgayNguyenLieu { get; set; }

    public DateTime? TimeROut { get; set; }
}
