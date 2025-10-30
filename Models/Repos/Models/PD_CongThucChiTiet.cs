using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PD_CongThucChiTiet")] // "PD_CongThucChiTiet
[PrimaryKey("STT", "Ngay", "MaCongThuc")]
public partial class PD_CongThucChiTiet
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongThuc { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal SanLuong { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLe { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal BienDo { get; set; }
}
