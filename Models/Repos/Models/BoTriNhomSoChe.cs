using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BoTriNhomSoChe))]
[PrimaryKey("MaNhanVien", "Ngay", "MaNhomSoChe", "MaXuong")]
public partial class BoTriNhomSoChe
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhomSoChe { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public double SoGio { get; set; }

    public double TyLeHuong { get; set; }

    public double TyLeTru { get; set; }
}
