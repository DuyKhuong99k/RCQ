using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanSoCheDinhHinh2")] // "PhieuCanSoCheDinhHinh2
[PrimaryKey("STT", "Ngay", "MaXuong", "MaMayCan")]
public partial class PhieuCanSoCheDinhHinh2
{
    [Key]
    public int STT { get; set; }

    [Key]
    public DateOnly Ngay { get; set; }

    public TimeOnly Gio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [StringLength(1)]
    [Unicode(false)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaMayCan { get; set; } = null!;

    public double TrongLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaMayLangDa { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaUserCan { get; set; } = null!;

    [StringLength(500)]
    public string? GhiChu { get; set; }
}
