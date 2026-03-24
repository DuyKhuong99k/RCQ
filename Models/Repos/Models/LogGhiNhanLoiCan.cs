using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Models.Repos.Models
{
    [Table("PhieuCanTPDinhHinh")]
    [PrimaryKey("STT", "Ngay", "MaMayCan", "MaXuong")]
    public class LogGhiNhanLoiCan
    {
        [Key]
        public int STT { get; set; }

        [Key]
        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }

        [Column(TypeName = "time(7)")]
        public TimeSpan? Gio { get; set; }
        [Column(TypeName = "time(7)")]
        public TimeSpan? GioBTP { get; set; }

        [StringLength(50)]
        public string? MaUserCan { get; set; } = null!;

        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string MaMayCan { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? MaLoaiCa { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? MaMau { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? MaSize { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? MaThanhPham { get; set; } = null!;

        [StringLength(50)]
        public string? MaLo { get; set; } = null!;

        [StringLength(50)]
        public string? MaThe { get; set; } = null!;

        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TrongLuongNhan { get; set; }

        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TrongLuongTra { get; set; }

        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DinhMucThucTe { get; set; }

        [Column(TypeName = "decimal(18, 3)")]
        public decimal? DinhMucYeuCau { get; set; }

        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string MaXuong { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? MaNhanVien { get; set; } = null!;

        public bool? CaTra { get; set; }

        public int? STTBTP { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaMayCanBTP { get; set; }

        public string? GhiChu { get; set; }

        public bool? ChiSanLuong { get; set; }

        public bool? SuDung { get; set; }

        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TrongLuongTare { get; set; }

        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TrongLuongBu { get; set; }

        public bool? IsOffline { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaNhanVienPhucVu { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaNhanVienBanKiem { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Id { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? IdIn { get; set; }

        public string? ThongBaoLoi { get; set; }

    }
}
