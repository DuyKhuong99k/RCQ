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
    [Table(nameof(PhuongTienChoNguyenLieu_D))]
    public partial class PhuongTienChoNguyenLieu_D
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        public string MaPhuongTien { get; set; } = null!;

        [StringLength(500)]
        public string? Ten { get; set; }

        public bool? SuDung { get; set; }

        public bool IsHD { get; set; }

        public bool IsGhe { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string VungNuoiId { get; set; } = null!;

        [StringLength(50)]
        public string? SoGhe { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;
    }
}
