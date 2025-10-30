using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DanhSachToKiem))] // "DanhSachToKiem
[PrimaryKey("MaToKiem", "MaXuong", "MaNhanVien", "Ngay", "LoaiBoTri")]
public partial class DanhSachToKiem
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaToKiem { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    public int LoaiBoTri { get; set; }

    public double SoGio { get; set; }

    public double TyLe { get; set; }

    public double TyLeTru { get; set; }
}
