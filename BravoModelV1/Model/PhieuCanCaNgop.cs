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

    [Table("PhieuCanCaNgop")]
    public partial class PhieuCanCaNgop
    {
        [Key]
        [StringLength(200)]
        public string ID { get; set; }

        public TimeSpan ThoiGian { get; set; }

        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }

        [Required]
        [StringLength(50)]
        public string MaNhaCungCap { get; set; }

        [Required]
        [StringLength(50)]
        public string MaVung { get; set; }

        [Required]
        [StringLength(50)]
        public string MaAo { get; set; }

        [Required]
        [StringLength(50)]
        public string MaCa { get; set; }

        public decimal SoLuong { get; set; }
        [Required]
        [StringLength(200)]
        public string TenNhaCungCap { get; set;}
        [Required]
        [StringLength(200)]
        public string TenPhuongTien { get; set; }
        [Required]
        [StringLength(200)]
        public string TenVungNuoi { get; set; }
        [Required]
        [StringLength(200)]
        public string TenAo { get; set; }
        [Required]
        [StringLength(200)]
        public string TenCa { get; set; }
        [Required]
        [StringLength(50)]
        public  string MaPhuongTien { get; set; }
    }
}
