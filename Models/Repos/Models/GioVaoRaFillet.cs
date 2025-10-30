using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(GioVaoRaFillet))] // "GioVaoRaFillet
[PrimaryKey("STT", "MaNhanVien", "Ngay", "MaCongViec", "MaXuong")]
public partial class GioVaoRaFillet
{
    [Key]
    public int STT { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan GioVao { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan? GioRa { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
