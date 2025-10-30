using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Table("ViewBaoCaoKeToanTheoNhaCungCap")]
[Keyless]
public partial class ViewBaoCaoKeToanTheoNhaCungCap
{
    public DateOnly? Ngày { get; set; }

    [StringLength(50)]
    public string Xưởng { get; set; } = null!;

    [Column("Nhà CC")]
    [StringLength(500)]
    public string? Nhà_CC { get; set; }

    [StringLength(50)]
    public string MaAo { get; set; } = null!;

    [Column("Sống NX", TypeName = "decimal(38, 2)")]
    public decimal? Sống_NX { get; set; }

    [Column("Ngộp Ghe NX", TypeName = "decimal(38, 2)")]
    public decimal? Ngộp_Ghe_NX { get; set; }

    [Column("Ngộp Ghe Muối NX", TypeName = "decimal(38, 2)")]
    public decimal? Ngộp_Ghe_Muối_NX { get; set; }

    [Column("Ghe Ngộp PP", TypeName = "decimal(38, 2)")]
    public decimal? Ghe_Ngộp_PP { get; set; }

    [Column("Ngộp Ao Muối NX", TypeName = "decimal(38, 2)")]
    public decimal? Ngộp_Ao_Muối_NX { get; set; }

    [Column("Ngộp Ao PP", TypeName = "decimal(38, 2)")]
    public decimal? Ngộp_Ao_PP { get; set; }

    [Column("Dạt Nhỏ", TypeName = "decimal(38, 2)")]
    public decimal? Dạt_Nhỏ { get; set; }

    [Column("Cá Tạp PP", TypeName = "decimal(38, 2)")]
    public decimal? Cá_Tạp_PP { get; set; }

    [Column("Căn Tin", TypeName = "decimal(38, 2)")]
    public decimal? Căn_Tin { get; set; }

    [Column("Ngộp Xe Muối NX", TypeName = "decimal(38, 2)")]
    public decimal? Ngộp_Xe_Muối_NX { get; set; }

    [Column("Xe Ngộp PP", TypeName = "decimal(38, 2)")]
    public decimal? Xe_Ngộp_PP { get; set; }

    [Column("Ghe Muối PP", TypeName = "decimal(38, 2)")]
    public decimal? Ghe_Muối_PP { get; set; }

    [Column("Tổng Từng Xưởng", TypeName = "decimal(38, 2)")]
    public decimal? Tổng_Từng_Xưởng { get; set; }

    [Column("Tổng NX", TypeName = "decimal(38, 2)")]
    public decimal? Tổng_NX { get; set; }

    [Column("Tổng Dạt", TypeName = "decimal(38, 2)")]
    public decimal? Tổng_Dạt { get; set; }
}
