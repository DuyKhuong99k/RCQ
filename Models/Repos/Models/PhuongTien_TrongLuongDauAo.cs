using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PhuongTien_TrongLuongDauAo")]
[PrimaryKey("MaPhuongTien", "Ngay", "MaAo", "MaNhaCungCap", "Chuyen")]
public partial class PhuongTien_TrongLuongDauAo
{
    [Key]
    [StringLength(50)]
    public string MaPhuongTien { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }

    [Key]
    [StringLength(50)]
    public string MaAo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CaManh { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CaNgopAoGhe { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CaNgopAoXe { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TongHam { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CaNgayTruoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CaConLai { get; set; }

    [StringLength(50)]
    public string ThuKy { get; set; } = null!;

    [StringLength(50)]
    public string ApTai { get; set; } = null!;

    public int STTChuyen { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLeMoi { get; set; }

    public string? GhiChu { get; set; }

    [Column(TypeName = "time(7)")]
    public TimeSpan GioXuatPhat { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayXuatPhat { get; set; }

    [Column(TypeName = "date")]
    public DateTime NgayBatCa { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDateTime { get; set; }

    [StringLength(500)]
    public string CreateBy { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ModifiedDateTime { get; set; }

    [StringLength(500)]
    public string ModifiedBy { get; set; } = null!;

    public bool IsVungNuoiBlocked { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhaCungCap { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CaNgopAoBanNgoai { get; set; }

    [Key]
    public int Chuyen { get; set; }
}
