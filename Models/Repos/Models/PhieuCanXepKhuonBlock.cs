using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanXepKhuonBlock")]
[PrimaryKey("STT", "Ngay", "MaXuong", "MaMayCan")]
public partial class PhieuCanXepKhuonBlock
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

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    public string? GhiChu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaChatLuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNet { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaChieuXa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaMau { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaCongDoan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; }
}
