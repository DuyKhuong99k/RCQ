using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamSoCheDinhHinh_Color))] // "MaThanhPhamSoCheDinhHinh_Color
[PrimaryKey("ColorCode", "Ngay", "MaLo", "MaXuong")]
public partial class MaThanhPhamSoCheDinhHinh_Color
{
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string ColorCode { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
