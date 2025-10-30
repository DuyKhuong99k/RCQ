using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Keyless]
[Table("Sheet1$")]
public partial class Sheet1_
{
    [Column("Tên bảng chẻ cở")]
    [StringLength(255)]
    public string? Tên_bảng_chẻ_cở { get; set; }

    [Column("Số con vỏ xô")]
    public double? Số_con_vỏ_xô { get; set; }

    [Column("Quy trình")]
    [StringLength(255)]
    public string? Quy_trình { get; set; }

    [Column("Size tính lương")]
    [StringLength(255)]
    public string? Size_tính_lương { get; set; }

    [Column("Công đoạn tính lương")]
    [StringLength(255)]
    public string? Công_đoạn_tính_lương { get; set; }
}
