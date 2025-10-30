using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_PhieuYeuCau")] // This is the table name in the database
public partial class T_PhieuYeuCau
{
    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

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

    public int TrangThai { get; set; }

    public DateTime NgayGioTao { get; set; }

    [StringLength(50)]
    public string NguoiTao { get; set; } = null!;

    [StringLength(200)]
    public string PCName { get; set; } = null!;

    public string? GhiChu { get; set; }

    public int STT { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? MaNhomYeuCau { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCongDoan { get; set; }
}
