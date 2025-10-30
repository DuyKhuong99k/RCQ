using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table(nameof(HQ_NhanVienTheoCa))]
    public partial class HQ_NhanVienTheoCa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string CaId { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string NhanVienId { get; set; } = null!;
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime NgayGio { get; set; } = DateTime.Now;
    }
}
