using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TheThanhPham")] // This is the table name in the database
public partial class TheThanhPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPham { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeDinhHinh { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuongTare { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaMayLangDa { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPhamFillet { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPhamPhuPham { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeFillet { get; set; }

    public bool? IsZero { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLoaiNguyenLieu { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string? MaSize { get; set; }

    public bool? IsRestart { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaQuyTrinhT { get; set; }

    public int STTPhieuPhanCoChiTietT { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCoiXepKhuon { get; set; }
    [NotMapped]
    public string? ThanhPhamDHName { get; set; }
    [NotMapped]
    public string? ThanhPhamName { get; set; }
    [NotMapped]
    public string? SizeName { get; set; }
    [NotMapped]
    public string? SizeDHName { get; set; }
    [NotMapped]
    public string? ThanhPhamFilletName { get; set; }
    [NotMapped]
    public string? SizeFilletName { get; set; }
    [NotMapped]
    public string? ThanhPhamPhuPhamName { get; set; }
    [NotMapped]
    public string? QuyTrinhName { get; set; }
    [NotMapped]
    public string? CoiXepKhuonName { get; set; }
    [Column(TypeName = "datetime2(7)")]
    public DateTime NgayGio { get; set; } = DateTime.Now;

    public int LoLevel { get; set; } = 0;
    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizeChinhXepKhuon { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPhamChinhXepKhuon { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string? MaChatLuongChinhXepKhuon { get; set; } // MaChatLuongXepKhuon
    [StringLength(50)]
    [Unicode(false)]
    public string? MaSizePhuXepKhuon { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPhamPhuXepKhuon { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string? MaChatLuongPhuXepKhuon { get; set; } // Không sử dụng, không có
    [StringLength(50)]
    [Unicode(false)]
    public string? MaChieuXa { get; set; }
    [NotMapped]
    public string? ThanhPhamChinhXepKhuonName { get; set; }
    [NotMapped]
    public string? SizeChinhXepKhuonName { get; set; }
    [NotMapped]
    public string? ChatLuongChinhXepKhuonName { get; set; }
    [NotMapped]
    public string? ThanhPhamPhuXepKhuonName { get; set; }
    [NotMapped]
    public string? SizePhuXepKhuonName { get; set; }
    [NotMapped]
    public string? ChatLuongPhuXepKhuonName { get; set; }
    [NotMapped]
    public string? LoaiNguyenLieuName { get; set; } 
    [NotMapped]
    public string? ChieuXaName { get; set; } 
}
