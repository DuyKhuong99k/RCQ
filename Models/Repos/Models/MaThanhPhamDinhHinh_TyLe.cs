using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaThanhPhamDinhHinh_TyLe))] // "MaThanhPhamDinhHinh_TyLe
public partial class MaThanhPhamDinhHinh_TyLe
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    public DateTime NgayGio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLeDau { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLeRot { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
