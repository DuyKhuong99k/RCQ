using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table("HQ_PhieuCanNhapNguyenLieu")]
    public class HQ_PhieuCanNhapNguyenLieu
    {
        [Key]
        [Column("Id", TypeName = "varchar(50)")]
        public string Id { get; set; }                           

        [Column("STT", TypeName = "int")]
        public int STT { get; set; }                           

        [Column("IdPhieuCanNguyenLieu", TypeName = "bigint")]
        public long IdPhieuCanNguyenLieu { get; set; }         

        [Column("SoPhieuCanNhap", TypeName = "varchar(MAX)")]
        public string SoPhieuCanNhap { get; set; }             

        [Column("SoLanSua", TypeName = "int")]
        public int SoLanSua { get; set; }                      

        [Column("MaKho", TypeName = "bigint")]
        public long MaKho { get; set; }                         

        [Column("TaiXe", TypeName = "nvarchar(50)")]
        [MaxLength(50)]
        public string? TaiXe { get; set; }                      

        [Column("CCCD", TypeName = "nvarchar(50)")]
        [MaxLength(500)]
        public string? CCCD { get; set; }                       

        [Column("SDT", TypeName = "nvarchar(50)")]
        [MaxLength(50)]
        public string? SDT { get; set; }                        

        [Column("NoiDungGiaoNhan", TypeName = "nvarchar(MAX)")]
        public string? NoiDungGiaoNhan { get; set; }            

        [Column("MaPhuongTien", TypeName = "nvarchar(50)")]
        [MaxLength(50)]
        public string? MaPhuongTien { get; set; }               

        [Column("MaSanPham", TypeName = "bigint")]
        public long MaSanPham { get; set; }                     

        [Column("TrongLuongTong", TypeName = "decimal(18,3)")]
        public decimal TrongLuongTong { get; set; }             

        [Column("TrongLuongXe", TypeName = "decimal(18,3)")]
        public decimal TrongLuongXe { get; set; }               

        [Column("TrongLuongHang", TypeName = "decimal(18,3)")]
        public decimal TrongLuongHang { get; set; }             

        [Column("MaDonVi", TypeName = "bigint")]
        public long MaDonVi { get; set; }                       

        [Column("MaChatLuong", TypeName = "bigint")]
        public long MaChatLuong { get; set; }                   

        [Column("CanHang", TypeName = "int")]
        public int CanHang { get; set; }                        

        [Column("TruBi", TypeName = "bit")]
        public bool TruBi { get; set; }   
        
        [Column("MaQuyCach", TypeName = "bigint")]
        public long MaQuyCach { get; set; }

        [Column("IsPhanLoaiNguyenLieu", TypeName = "bit")]
        public bool IsPhanLoaiNguyenLieu { get; set; }  

        [Column("TenKhachHangCoDinh", TypeName = "nvarchar(500)")]
        [MaxLength(500)]
        public string? TenKhachHangCoDinh { get; set; }

    }
}
