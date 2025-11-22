using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table("HQ_PhieuCanXuatNguyenLieu")]
    public class HQ_PhieuCanXuatNguyenLieu
    {
        [Key]
        [Column("Id", TypeName = "varchar(50)")]
        public string Id { get; set; }

        [Column("STT", TypeName = "int")]
        public int STT { get; set; }

        [Column("IdPhieuCanNguyenLieu", TypeName = "bigint")]
        public long IdPhieuCanNguyenLieu { get; set; }

        [Column("SoPhieuNhap", TypeName = "varchar(MAX)")]
        public string SoPhieuNhap { get; set; }

        [Column("SoPhieuXuat", TypeName = "varchar(MAX)")]
        public string SoPhieuXuat { get; set; }

        [Column("MaThuKho", TypeName = "varchar(50)")]
        public string MaThuKho { get; set; }

        [Column("MaSanPham", TypeName = "bigint")]
        public long MaSanPham { get; set; }

        [Column("TrongLuongTong", TypeName = "decimal(18, 3)")]
        public decimal TrongLuongTong { get; set; }

        [Column("TrongLuongXe", TypeName = "decimal(18, 3)")]
        public decimal TrongLuongXe { get; set; }

        [Column("TrongLuongHang", TypeName = "decimal(18, 3)")]
        public decimal TrongLuongHang { get; set; }

        [Column("MaDonVi", TypeName = "bigint")]
        public long MaDonVi { get; set; }

        [Column("MaXuongXuatDen", TypeName = "varchar(50)")]
        public string MaXuongXuatDen { get; set; }

        [Column("MaKho", TypeName = "bigint")]
        public long MaKho { get; set; }
    }
}
