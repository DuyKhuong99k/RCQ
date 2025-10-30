

namespace BravoModelV1.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("PhieuCanSanXuat")]
    public partial class PhieuCanSanXuat
    {
        [Key]
        [StringLength(200)]
        public string ID { get; set; }

        public TimeSpan ThoiGian { get; set; }

        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }

        [Required]
        [StringLength(50)]
        public string MaCongDoan { get; set; }

        [Required]
        [StringLength(50)]
        public string MaSanPham { get; set; }

        public decimal SoLuong { get; set; }
        [Required]
        [StringLength(50)]
        public string MaKhachHang { get; set; }
        [Required]
        [StringLength(200)]
        public string TenKhachHang { get; set; }
        [Required]
        [StringLength(200)]
        public string TenCongDoan { get; set; }
        [Required]
        [StringLength(50)]
        public string MaXuong { get; set; }
        [Required]
        [StringLength(200)]
        public string TenXuong { get; set; }
        [Required]
        [StringLength(50)]
        public string MaChuyen { get; set; }
        [Required]
        [StringLength(200)]
        public string TenChuyen { get; set; }
        [Required]
        [StringLength(200)]
        public string TenSanPham { get; set; }
    }
}
