using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(CongViecTinhLuongXepKhuonSanLuong))] // "CongViecTinhLuongXepKhuonSanLuong
[PrimaryKey("MaCongViec", "Ngay", "MaCa", "MaXuong")]
public partial class CongViecTinhLuongXepKhuonSanLuong
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCa { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SanLuong { get; set; }
}
