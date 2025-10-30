using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanDinhHinh")] // "PhieuCanDinhHinh
public partial class PhieuCanDinhHinh
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [StringLength(50)]
    public string? CaLamViec { get; set; }

    [StringLength(50)]
    public string? MaNhanVien { get; set; }

    public bool? SuDung { get; set; }

    [StringLength(50)]
    public string? MaSanPham { get; set; }

    [StringLength(50)]
    public string? TenSanPham { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TrongLuongNhan { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TrongLuongTra { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DinhMucThucTe { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DinhMucYeuCau { get; set; }

    public bool? DanhGia { get; set; }

    public int? SoRo { get; set; }

    public int _Status { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? CreatedAt { get; set; }
}
