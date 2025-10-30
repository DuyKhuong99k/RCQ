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
    [Table("PhieuCanRaCoi")] // "PhieuCanRaCoi
    public partial class PhieuCanRaCoi
    {
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        [Required]
        public string Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string IdMonitor { get; set; }
        [Column(TypeName = "date")]
        public DateTime Ngay { get; set; }
        public TimeSpan Gio { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaXuong { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MayCan { get; set; }
        [Column(TypeName = "date")]
        public DateTime NgayNguyenLieu { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal TrongLuong { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal TrongLuongTare { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaLo { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaThanhPham { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaSize { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaChieuXa { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaChatLuong { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaNhanVien { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaCoi { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaThe { get; set; }
        public string? GhiChu { get; set; }
        public int STT { get; set; }
    }
}
