using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhanBoNhanVienTheoCongViecPhuFillet")] // "PhanBoNhanVienTheoCongViecPhuFillet
[PrimaryKey("MaNhanVien", "MaCongViecPhuFillet", "Ngay", "MaXuong")]
public partial class PhanBoNhanVienTheoCongViecPhuFillet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViecPhuFillet { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public double TyLeHuong { get; set; }

    public double TyLeTru { get; set; }
}
