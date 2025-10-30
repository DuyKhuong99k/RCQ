using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.EF
{
    [Table("PhieuCanKiemDinhHinh")]
    public partial class PhieuCanKiemDinhHinh
    {
        public int Id { get; set; }

        [Column(TypeName = "date")] public DateTime Ngay { get; set; }

        [Required][StringLength(50)] public string CaLamViec { get; set; }

        [StringLength(50)] public string MaNhanVien { get; set; }

        [StringLength(50)] public string MaSanPham { get; set; }

        [StringLength(50)] public string TenSanPham { get; set; }

        public decimal? TrongLuong { get; set; }

        [Required][StringLength(8)] public string KhuVuc { get; set; }

        [Column("_Status")] public int C_Status { get; set; }

        [Column(TypeName = "smalldatetime")] public DateTime? CreatedAt { get; set; }
    }
}
