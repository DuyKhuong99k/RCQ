using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DinhMucFillet))] // "DinhMucFillet
[PrimaryKey("STT", "Ngay", "Gio", "MaLo", "MaLoaiCa", "MaMau", "MaSize", "MaThanhPham", "MaXuong", "CaTra")]
public partial class DinhMucFillet
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [Key]
    [StringLength(50)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCa { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMau { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    public bool CaTra { get; set; }

    public double DinhMuc { get; set; }

    public bool SuDung { get; set; }
}
