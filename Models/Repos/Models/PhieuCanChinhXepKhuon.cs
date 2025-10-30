using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanChinhXepKhuon")] // "PhieuCanChinhXepKhuon
[PrimaryKey("STT", "MaXuong", "MaMayCan", "NgayNguyenLieu")]
public partial class PhieuCanChinhXepKhuon
{
    [Key]
    public int STT { get; set; }

    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }
    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaCoiTam { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCoiChinh { get; set; }

    public bool DaQuay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan? ThoiGianBatDauQuay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan? ThoiGianRaCoi { get; set; }

    public bool Forced { get; set; }

    public int ThoiGianQuay { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSizeChinh { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaMau { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaChatLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamChinh { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVien { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhom { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaChieuXa { get; set; } = null!;

    public bool TaiChe { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    public string? GhiChu { get; set; }

    public int LuotQuay { get; set; }

    public bool ChuyenXuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVienPvPhanCo { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }

    [Key]
    [Column(TypeName = "date")]
    public DateTime NgayNguyenLieu { get; set; }
    [Column(TypeName = "date")]
    public DateTime? NgayRaCoi { get; set; }
    [Column(TypeName = "date")]
    public DateTime? NgayBatDauQuay { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MayQuay { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? IdMonitor { get; set; }
    
    [StringLength(50)]
    [Unicode(false)]
    public string? Id { get; set; }
}
