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
    [Table(nameof(CheckInOut))]
    public partial class CheckInOut
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(100)]
        [Unicode(false)]
        public string Id { get; set; } = "";

        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string MaChamCong { get; set; } = null!;
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime ThoiGian { get; set; }

        [Required] public int MaSoMay { get; set; } = 0;
        [Required]
        [StringLength(200)]
        public string TenMay { get; set; } = null!;
        [StringLength(500)]
        public string? GhiChu { get; set; }

        // GhiChu dành cho người dùng, không chứa metadata chấm công.
        [StringLength(20)]
        public string? TrangThai { get; set; }

        [StringLength(100)]
        public string? CongViec { get; set; }

        [Required]
        public int Isforget { get; set; } = 0;

        public bool CaDem { get; set; }

        [StringLength(100), Unicode(false)]
        public string? VaoId { get; set; }
        // Snapshot at event time. NULL means historical workshop is not known.
        [StringLength(100)]
        public string? MaXuong { get; set; }
        [StringLength(100)]
        public string? NguoiXacNhan { get; set; }
        [StringLength(500)]
        public string? LyDo { get; set; }

    }
}
