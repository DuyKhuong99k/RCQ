using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table(nameof(HQ_TheTu_U))]
    public partial class HQ_TheTu_U
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        public string MaTheTu { get; set; } = null!;

        [StringLength(50)]
        public string? MaNhanVien { get; set; }

        public DateTime NgayGio { get; set; }

        [StringLength(50)]
        public string PCName { get; set; } = null!;
        [NotMapped]
        public string? NhanVienName { get; set; }
        [NotMapped]
        public string? MaSo { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public  DateTime Ngay { get; set; } = DateTime.Now;
    }
}
