using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.EF
{
    [Table("PhieuCanFillet")]
    public partial class PhieuCanFillet
    {
        public int Id { get; set; }

        [Column(TypeName = "date")] public DateTime Ngay { get; set; }

        [StringLength(50)] public string CaLamViec { get; set; }

        [StringLength(50)] public string MaNhanVien { get; set; }

        public bool? SuDung { get; set; }

        [StringLength(50)] public string MaSanPham { get; set; }

        [StringLength(50)] public string TenSanPham { get; set; }

        public decimal? TrongLuong { get; set; }

        public int? SoRo { get; set; }

        [Column("_Status")] public int C_Status { get; set; }

        [Column(TypeName = "smalldatetime")] public DateTime? CreatedAt { get; set; }
    }
}
