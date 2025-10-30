using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PD_PhieuNhap")] // "PD_PhieuNhap
[PrimaryKey("STT", "Ngay", "PCName", "MaXuong")]
public partial class PD_PhieuNhap
{
    [StringLength(50)]
    [Unicode(false)]
    public string SoPhieu { get; set; } = null!;

    [Key]
    public int STT { get; set; }

    [Key]
    public DateOnly Ngay { get; set; }

    [Key]
    [StringLength(50)]
    public string PCName { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public bool SuDung { get; set; }

    public DateTime CreateDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;

    public DateTime ModifiedDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ModifiedBy { get; set; } = null!;

    public bool NhapTra { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;
}
