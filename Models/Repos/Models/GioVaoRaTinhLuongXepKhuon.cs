using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(GioVaoRaTinhLuongXepKhuon))] // "GioVaoRaTinhLuongXepKhuon
[PrimaryKey("STT", "MaNhanVien", "Ngay", "MaCongViec", "MaXuong")]
public partial class GioVaoRaTinhLuongXepKhuon
{
    [Key]
    public int STT { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan GioVao { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan? GioRa { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaCa { get; set; } = null!;

    public DateTime CreateDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CreateBy { get; set; } = null!;

    public DateTime ModifyDateTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ModifyBy { get; set; } = null!;

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;
}
