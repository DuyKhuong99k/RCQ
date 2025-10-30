using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_DinhMuc")] // Added
[PrimaryKey("STT", "Ngay", "Gio", "MaLo", "MaLoaiNguyenLieu", "MaSize", "MaThanhPham", "MaXuong", "CaTra", "MaKhuVuc")]
public partial class T_DinhMuc
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "datetime")]
    public DateTime Ngay { get; set; }

    [Key]
    public TimeOnly Gio { get; set; }

    [Key]
    [StringLength(50)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string MaLoaiNguyenLieu { get; set; } = null!;

    [Key]
    [StringLength(50)]
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

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;
}
