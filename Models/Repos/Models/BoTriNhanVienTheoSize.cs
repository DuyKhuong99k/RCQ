using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BoTriNhanVienTheoSize))]
[PrimaryKey("Ngay", "MaNhanVien", "MaSize", "ThoiGianBatDau", "MaXuong")]
public partial class BoTriNhanVienTheoSize
{
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [Key]
    [Column(TypeName = "time(7)")]
    public TimeSpan ThoiGianBatDau { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public bool SuDung { get; set; }
}
