using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(CongViecTinhLuongXepKhuonTheoLoaiThanhPham))] // "CongViecTinhLuongXepKhuonTheoLoaiThanhPham
[PrimaryKey("MaCongViec", "MaThanhPhamDinhHinh", "MaThanhPhamPhuXepKhuon", "MaThanhPhamChinhXepKhuon", "MaThanhPhamBlockXepKhuon", "MaThanhPhamKHCXepKhuon", "MaThanhPhamTaiChe", "MaThanhPhamSoChe", "MaCongViecTaiChe", "MaCongDoan", "MaChieuXaChinhXepKhuon")]
public partial class CongViecTinhLuongXepKhuonTheoLoaiThanhPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViec { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamDinhHinh { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamPhuXepKhuon { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamChinhXepKhuon { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamBlockXepKhuon { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamKHCXepKhuon { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamTaiChe { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamSoChe { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongViecTaiChe { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCongDoan { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaChieuXaChinhXepKhuon { get; set; } = null!;
}
