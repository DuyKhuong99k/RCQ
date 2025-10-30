using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PD_PhieuXuatChiTiet")] // "PD_PhieuXuatChiTiet
[PrimaryKey("SoPhieu", "STT")]
public partial class PD_PhieuXuatChiTiet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string SoPhieu { get; set; } = null!;

    [Key]
    public int STT { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SanLuong { get; set; }

    public bool SuDung { get; set; }
}
