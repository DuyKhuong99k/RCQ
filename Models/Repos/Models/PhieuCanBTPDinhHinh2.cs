using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanBTPDinhHinh2")] // "PhieuCanBTPDinhHinh2
[PrimaryKey("STT", "Ngay", "MaMayCan", "MaXuong")]
public partial class PhieuCanBTPDinhHinh2
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
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVien { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaMayLangDa { get; set; } = null!;

    public double TrongLuong { get; set; }

    public bool IsEnabled { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    public bool CaTra { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }
}
