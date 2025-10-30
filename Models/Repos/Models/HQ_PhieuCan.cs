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
    [Table(nameof(HQ_PhieuCan))]
    public partial class HQ_PhieuCan
    {
        [Key]
        [StringLength(100)]
        [Unicode(false)]
        public required string Id { get; set; }
        [Required]
        public int STT {get; set; }
        [Required]
        public DateOnly Ngay { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime NgayGio {get; set; } = DateTime.Now;
        [Required]
        [StringLength(50)]
        public required string MayCan { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public required string MaLo { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public required string MaSize { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public required string MaThanhPham { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public required string MaLoaiNguyenLieu { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public  string? MaNhanVien { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? MaNhanVienPhucVu { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? MaNhanVienBanKiem { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal TrongLuong { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal TrongLuongTare { get; set; } = 0;
        [Required]
        public bool ChiSanLuong { get; set; } = true;
        [Required]
        public int Status { get; set; } = 0;
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public required string TheId { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public required string TheIdNhanVien { get; set; }
        public string? GhiChu { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string MaXuong { get; set; }
    }

}
