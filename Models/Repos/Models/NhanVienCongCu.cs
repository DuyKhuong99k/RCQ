using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhanVienCongCu")] // "NhanVienCongCu
[PrimaryKey("Ngay", "MaLo", "TabName", "MaNhanVien", "MaXuong")]
public partial class NhanVienCongCu
{
    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan Gio { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLo { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string TabName { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeXepKhuonChinh { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeXepKhuonKXL { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPhamXepKhuonChinh { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPhamXepKhuonKXL { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaChieuXaXepKhuonChinh { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaKhachHangXepKhuonKXL { get; set; }
}
