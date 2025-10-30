using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_PhieuCanThuMua")] // This is the table name in the database
public partial class T_PhieuCanThuMua
{
    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiNguyenLieu { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaTieuChuan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaBaoBi { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaQuyTrinh { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaPhuGia { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhachHang { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhangSinh { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThongTinPhu { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string VoXoTB { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string VoXoCD { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string VoXoCT { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TyLe { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GramCuoi { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GramDau { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal NhuCau { get; set; }

    public DateTime NgayGioTao { get; set; }

    [StringLength(50)]
    public string MaUserCan { get; set; } = null!;

    [StringLength(200)]
    public string MayCan { get; set; } = null!;

    public string? GhiChu { get; set; }

    public int STT { get; set; }

    public DateOnly Ngay { get; set; }

    public TimeOnly Gio { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string MaPhieuYeuCau { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuongTare { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    public int Status { get; set; }

    public DateOnly? NgayNguyenLieu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhomLo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCongDoan { get; set; }
}
