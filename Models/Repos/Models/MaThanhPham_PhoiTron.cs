using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPham_PhoiTron))] // "MaThanhPham_PhoiTron
[PrimaryKey("Ngay", "MaThanhPhamOrg", "MaThanhPhamDes", "MaKhuVuc", "MaXuong", "MaLo")]
public partial class MaThanhPham_PhoiTron
{
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamOrg { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamDes { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Column(TypeName = "numeric(18, 2)")]
    public decimal TyLe { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;

    public DateTime CreateDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ModifyBy { get; set; } = null!;

    public DateTime ModifyDateTime { get; set; }
}
