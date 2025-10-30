using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhieuCanNguyenLieu")] // "PhieuCanNguyenLieu
[PrimaryKey("MaMayTinhCan", "MaUserCan", "ThoiGianCan")]
public partial class PhieuCanNguyenLieu
{
    [Key]
    [StringLength(50)]
    public string MaMayTinhCan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string MaUserCan { get; set; } = null!;

    [Key]
    public DateTime ThoiGianCan { get; set; }

    [Column(TypeName = "date")]
    public DateTime? Ngay { get; set; }

    [StringLength(50)]
    public string NhaCC { get; set; } = null!;

    [StringLength(50)]
    public string MSL { get; set; } = null!;

    [StringLength(50)]
    public string MaAo { get; set; } = null!;

    [StringLength(50)]
    public string MaPhuongTien { get; set; } = null!;

    [StringLength(50)]
    public string MaLoaiCa { get; set; } = null!;

    [StringLength(50)]
    public string MaLoaiThanhPham { get; set; } = null!;

    [StringLength(50)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    public string MaMau { get; set; } = null!;

    [StringLength(50)]
    public string MaBanCatTiet { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TrongLuong { get; set; }

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(50)]
    public string MaXuongSanXuat { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLeNuoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongOrg { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongTare { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Pheu { get; set; } = null!;

    public int Chuyen { get; set; }
}
