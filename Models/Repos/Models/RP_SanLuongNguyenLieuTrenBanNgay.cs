using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RP_SanLuongNguyenLieuTrenBanNgay")] // Added
[PrimaryKey("Ngay", "KhuVuc", "GioR", "MaLo", "MaXuong")]
public partial class RP_SanLuongNguyenLieuTrenBanNgay
{
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string KhuVuc { get; set; } = null!;

    [Key]
    [Column(TypeName = "time(7)")]
    public TimeSpan GioR { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTrongLuong { get; set; }
}
