using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models
{
    [Table(nameof(HQ_PhieuThongKeSanXuat))]
    public partial class HQ_PhieuThongKeSanXuat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string SoChungTu { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Ca { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaTo { get; set; }
        [StringLength(200)]
        [Unicode(true)]
        public string TenTo { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaNhanVien { get; set; }
        [StringLength(200)]
        [Unicode(true)]
        public string TenNhanVien { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaCongViec { get; set; }
        [StringLength(200)]
        [Unicode(true)]
        public string TenCongViec { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal SanLuong { get; set; }
        [Column(TypeName = "datetime2(7)")]
        public DateTime GioBatDau { get; set; }
        [Column(TypeName = "datetime2(7)")]
        public DateTime GioKetThuc { get; set; }
        [Column(TypeName = "datetime2(7)")]
        public DateTime NgayGioTao { get; set; } = DateTime.Now;
    }
}
