using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanTPFilletv2")]
[PrimaryKey("STT", "Ngay", "MaMayCan", "MaXuong")]
public partial class PhieuCanTPFilletv2
{
    [Key]
    public int STT { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    public string MaUserCan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaMau { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    public string MaLo { get; set; } = null!;

    [StringLength(50)]
    public string MaThe { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongNhan { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongTra { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DinhMucThucTe { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DinhMucYeuCau { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    public bool CaTra { get; set; }

    public int? STTBTP { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaMayCanBTP { get; set; }

    [Unicode(false)]
    public string? GhiChu { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongTare { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVienPhucVu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaBan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ThePhieuSanLuongId { get; set; }
    [StringLength(100)]
    [Unicode(false)]
    public string? Id { get; set; }
    [StringLength(100)]
    [Unicode(false)]
    public string? IdIn { get; set; }
}
