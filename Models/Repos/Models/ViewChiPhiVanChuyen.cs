using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("ViewChiPhiVanChuyen")] // This is the table name in the database
[Keyless]
public partial class ViewChiPhiVanChuyen
{
    [StringLength(1)]
    [Unicode(false)]
    public string TK { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string AT { get; set; } = null!;

    public DateOnly? Ngày { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string STT { get; set; } = null!;

    [StringLength(500)]
    public string? Ghe { get; set; }

    [Column("Số Ghe")]
    [StringLength(50)]
    public string Số_Ghe { get; set; } = null!;

    [Column("Tải Trọng")]
    [StringLength(1)]
    [Unicode(false)]
    public string Tải_Trọng { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Cù { get; set; }

    [Column("SL Hầm cá mạnh", TypeName = "decimal(18, 2)")]
    public decimal SL_Hầm_cá_mạnh { get; set; }

    [Column("SL NX", TypeName = "decimal(38, 2)")]
    public decimal SL_NX { get; set; }

    [Column("CL - (hao hụt)", TypeName = "decimal(38, 2)")]
    public decimal? CL____hao_hụt_ { get; set; }

    [Column("CL + (dư)", TypeName = "decimal(38, 2)")]
    public decimal? CL____dư_ { get; set; }

    [Column("TL Hao hụt từng ghe", TypeName = "decimal(38, 6)")]
    public decimal? TL_Hao_hụt_từng_ghe { get; set; }

    [Column("NAG Hầm", TypeName = "decimal(18, 2)")]
    public decimal NAG_Hầm { get; set; }

    [Column("NAG NX", TypeName = "decimal(38, 2)")]
    public decimal NAG_NX { get; set; }

    [Column("NAG Chênh Lệch", TypeName = "decimal(38, 2)")]
    public decimal? NAG_Chênh_Lệch { get; set; }

    [Column("NAX Hầm", TypeName = "decimal(18, 2)")]
    public decimal NAX_Hầm { get; set; }

    [Column("NAX NX", TypeName = "decimal(38, 2)")]
    public decimal NAX_NX { get; set; }

    [Column("NAX chênh Lệch", TypeName = "decimal(38, 2)")]
    public decimal? NAX_chênh_Lệch { get; set; }

    [Column("TCSL Hầm", TypeName = "decimal(20, 2)")]
    public decimal? TCSL_Hầm { get; set; }

    [Column("TCSL NX", TypeName = "decimal(38, 2)")]
    public decimal? TCSL_NX { get; set; }

    [Column("TCSL Chênh Lệch", TypeName = "decimal(38, 2)")]
    public decimal? TCSL_Chênh_Lệch { get; set; }

    [Column("Đơn Giá")]
    [StringLength(1)]
    [Unicode(false)]
    public string Đơn_Giá { get; set; } = null!;

    [Column("Thành Tiền")]
    [StringLength(1)]
    [Unicode(false)]
    public string Thành_Tiền { get; set; } = null!;

    [StringLength(50)]
    public string Ao { get; set; } = null!;

    [Column("Địa Chỉ Ao")]
    [StringLength(500)]
    public string? Địa_Chỉ_Ao { get; set; }
}
