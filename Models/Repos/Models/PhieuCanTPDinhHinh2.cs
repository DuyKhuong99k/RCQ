using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanTPDinhHinh2")]
[PrimaryKey("STT", "Ngay", "MaMayCan", "MaXuong")]
public partial class PhieuCanTPDinhHinh2
{
    [Key]
    public int STT { get; set; }

    [Key]
    public DateOnly Ngay { get; set; }

    public TimeOnly Gio { get; set; }

    [StringLength(50)]
    public string MaUserCan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    [StringLength(1)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(1)]
    public string MaMau { get; set; } = null!;

    [StringLength(1)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    public string MaLo { get; set; } = null!;

    [StringLength(50)]
    public string MaThe { get; set; } = null!;

    public double TrongLuongNhan { get; set; }

    public double TrongLuongTra { get; set; }

    public double DinhMucThucTe { get; set; }

    public double DinhMucYeuCau { get; set; }

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

    [StringLength(500)]
    public string? GhiChu { get; set; }
}
