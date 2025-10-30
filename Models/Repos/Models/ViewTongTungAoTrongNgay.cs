using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("ViewTongTungAoTrongNgay")] // This is the table name in the database
[Keyless]
public partial class ViewTongTungAoTrongNgay
{
    public DateOnly? Ngày { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string Xưởng { get; set; } = null!;

    [Column("Nhà CC")]
    [StringLength(1)]
    [Unicode(false)]
    public string Nhà_CC { get; set; } = null!;

    [StringLength(50)]
    public string Ao { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string Cù { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string Ghe { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string Số { get; set; } = null!;

    [Column("Cá Mạnh", TypeName = "decimal(38, 2)")]
    public decimal Cá_Mạnh { get; set; }

    [Column("Ngộp Ao Ghe", TypeName = "decimal(38, 2)")]
    public decimal Ngộp_Ao_Ghe { get; set; }

    [Column("Ngộp Ao Xe", TypeName = "decimal(38, 2)")]
    public decimal Ngộp_Ao_Xe { get; set; }

    [Column("Tổng Hầm", TypeName = "decimal(38, 2)")]
    public decimal Tổng_Hầm { get; set; }

    [Column("Sống NX", TypeName = "decimal(38, 2)")]
    public decimal Sống_NX { get; set; }

    [Column("Ngộp Ghe NX", TypeName = "decimal(38, 2)")]
    public decimal Ngộp_Ghe_NX { get; set; }

    [Column("Ngộp Ghe Muối NX", TypeName = "decimal(38, 2)")]
    public decimal Ngộp_Ghe_Muối_NX { get; set; }

    [Column("Ghe Ngộp PP", TypeName = "decimal(38, 2)")]
    public decimal Ghe_Ngộp_PP { get; set; }

    [Column("Ngộp Ao Muối NX", TypeName = "decimal(38, 2)")]
    public decimal Ngộp_Ao_Muối_NX { get; set; }

    [Column("Ngộp Ao PP", TypeName = "decimal(38, 2)")]
    public decimal Ngộp_Ao_PP { get; set; }

    [Column("Dạt Nhỏ", TypeName = "decimal(38, 2)")]
    public decimal Dạt_Nhỏ { get; set; }

    [Column("Cá Tạp PP", TypeName = "decimal(38, 2)")]
    public decimal Cá_Tạp_PP { get; set; }

    [Column("Căn Tin", TypeName = "decimal(38, 2)")]
    public decimal Căn_Tin { get; set; }

    [Column("Ngộp Xe Muối NX", TypeName = "decimal(38, 2)")]
    public decimal Ngộp_Xe_Muối_NX { get; set; }

    [Column("Xe Ngộp PP", TypeName = "decimal(38, 2)")]
    public decimal Xe_Ngộp_PP { get; set; }

    [Column("Ghe Muối PP", TypeName = "decimal(38, 2)")]
    public decimal Ghe_Muối_PP { get; set; }

    [Column("Tổng Từng Xưởng", TypeName = "decimal(38, 2)")]
    public decimal? Tổng_Từng_Xưởng { get; set; }

    [Column("Tổng NX", TypeName = "decimal(38, 2)")]
    public decimal Tổng_NX { get; set; }

    [Column("Tổng Dạt", TypeName = "decimal(38, 2)")]
    public decimal Tổng_Dạt { get; set; }

    [Column("Hao Hụt", TypeName = "decimal(38, 2)")]
    public decimal? Hao_Hụt { get; set; }

    [Column("Tỷ Lệ", TypeName = "decimal(38, 6)")]
    public decimal? Tỷ_Lệ { get; set; }
}
