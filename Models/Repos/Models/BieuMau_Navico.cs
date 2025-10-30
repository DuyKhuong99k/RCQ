using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BieuMau_Navico))]
public partial class BieuMau_Navico
{
    [Key]
    [StringLength(500)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    public DateOnly Ngay { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(500)]
    public string? BoPhan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaCongDoan { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal NguyenLieu_Ngay { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal NguyenLieu_Dem { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal ThanhPham_Ngay { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal ThanhPham_Dem { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVienCan { get; set; }

    [StringLength(500)]
    public string MaNhaMay { get; set; } = null!;

    public TimeOnly ThoiDiemGhiNhanSanLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKV { get; set; }
}
