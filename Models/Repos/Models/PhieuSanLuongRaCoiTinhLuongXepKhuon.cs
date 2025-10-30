using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuSanLuongRaCoiTinhLuongXepKhuon")]
[PrimaryKey("STT", "Ngay", "MaXuong", "MaMayCan")]
public partial class PhieuSanLuongRaCoiTinhLuongXepKhuon
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayThem { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan GioRaCoi { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaCoi { get; set; } = null!;

    public int LuotRaCoi { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhom { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; }

    public string? GhiChu { get; set; }
}
