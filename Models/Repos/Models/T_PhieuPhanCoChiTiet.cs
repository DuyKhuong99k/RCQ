using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_PhieuPhanCoChiTiet")] // This is the table name in the database
[PrimaryKey("STT", "MaPhieuPhanCo")]
public partial class T_PhieuPhanCoChiTiet
{
    [Key]
    public int STT { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaPhieuPhanCo { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSize { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaQuyTrinh { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaPhuGia { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKhachHang { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKhangSinh { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? VoXo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThongTinPhu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaTrangThaiNguyenLieu { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TyLe { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal GramCuoi { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal GramDau { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCongDoan { get; set; }

    [StringLength(200)]
    public string? VoXo2 { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong2 { get; set; }
}
