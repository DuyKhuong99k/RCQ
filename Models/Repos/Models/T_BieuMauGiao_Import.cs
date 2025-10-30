using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_BieuMauGiao_Import")] // Added
public partial class T_BieuMauGiao_Import
{
    [Key]
    public long Id { get; set; }

    [StringLength(300)]
    public string? MaKhachHang { get; set; }

    [StringLength(300)]
    public string? KhangSinh { get; set; }

    [StringLength(300)]
    public string? ThongTinPhuGia { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? CoVoXo { get; set; }

    [StringLength(300)]
    public string? MapppingSize_HLSO { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal? TrongLuongGiao { get; set; }

    public int? SoLuongBo_Phi { get; set; }

    public int? SoLuong { get; set; }

    [StringLength(300)]
    public string? NguoiGiao { get; set; }

    [StringLength(300)]
    public string? DonViNhan { get; set; }

    [StringLength(300)]
    public string? NguoiNhan { get; set; }

    [StringLength(300)]
    public string? BoPhan { get; set; }

    [StringLength(300)]
    public string? NguoiVanChuyen { get; set; }

    [StringLength(300)]
    public string? TaiXe_BienSoXe { get; set; }

    [StringLength(300)]
    public string? GhiChu { get; set; }

    [StringLength(300)]
    public string UserName { get; set; } = null!;

    [StringLength(300)]
    public string PCName { get; set; } = null!;

    public DateTime NgayImport { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? FileName { get; set; }

    [StringLength(300)]
    public string? MaBang { get; set; }

    public DateTime? NgayLap { get; set; }

    [StringLength(300)]
    public string? SoSeal { get; set; }

    [StringLength(300)]
    public string? DonVi { get; set; }

    [StringLength(300)]
    public string? NgayNguyenLieu { get; set; }

    [StringLength(300)]
    public string? HinhThucCan { get; set; }

    [StringLength(300)]
    public string? LoaiTom { get; set; }

    [StringLength(300)]
    public string? ChungNhan { get; set; }

    [StringLength(300)]
    public string? SizeTPBaoBi { get; set; }

    [StringLength(300)]
    public string? QTMatHang { get; set; }

    [StringLength(300)]
    public string? LoaiQuyTrinh { get; set; }

    [StringLength(300)]
    public string? GhiChuPhuGia { get; set; }
}
