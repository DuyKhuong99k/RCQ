using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PhieuCanBotCa")]
    public partial class PhieuCanBotCa
    {
        [Key]
        [StringLength(200)]
        public string ID { get; set; }

        public TimeSpan ThoiGian { get; set; }

        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }

        [Required]
        [StringLength(50)]
        public string MaKhachHang { get; set; }

        [Required]
        [StringLength(50)]
        public string MaSanPham { get; set; }

        public decimal SoLuong { get; set; }
        [Required]
        [StringLength(200)]
        public string TenKhachHang { get; set; }
        [Required]
        [StringLength(50)]
        public string MaPhuongTien { get; set; }
        [Required]
        [StringLength(200)]
        public string TenPhuongTien { get; set; }
        [Required]
        [StringLength(200)]
        public string TenSanPham { get; set; }
    }
}
