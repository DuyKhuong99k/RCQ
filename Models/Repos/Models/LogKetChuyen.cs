using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(LogKetChuyen))] // "LogKetChuyen
[PrimaryKey("STT", "MaKhuVuc", "Ngay", "MaXuong")]
public partial class LogKetChuyen
{
    [Key]
    public int STT { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [StringLength(500)]
    public string PCName { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
