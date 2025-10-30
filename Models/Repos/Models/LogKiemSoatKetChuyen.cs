using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(LogKiemSoatKetChuyen))] // "LogKiemSoatKetChuyen
public partial class LogKiemSoatKetChuyen
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(500)]
    public string PCName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public int tab { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Action { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime NgayChuyen { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan GioChuyen { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDayTime { get; set; }
}
