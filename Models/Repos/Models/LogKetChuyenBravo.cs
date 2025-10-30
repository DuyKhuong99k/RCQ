using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(LogKetChuyenBravo))] // "LogKetChuyenBravo
[PrimaryKey("MaSanPham", "Ngay", "MaXuong", "tab")]
public partial class LogKetChuyenBravo
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime NgayChuyen { get; set; }

    [Key]
    public int tab { get; set; }
}
