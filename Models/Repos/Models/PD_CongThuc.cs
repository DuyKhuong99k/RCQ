using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PD_CongThuc")] // "PD_CongThuc
public partial class PD_CongThuc
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongNguyenLieu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaDonViTinh { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime CreateDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;

    public DateTime ModifiedDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ModifiedBy { get; set; } = null!;
}
