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
    [Table(nameof(HQ_Lo_D))]
    public partial class HQ_Lo_D
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        [Required]
        public required string LoId { get; set; }
        [Required]
        public DateOnly NgayNguyenLieu { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required]
        public bool SuDung { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime Ngay { get; set; } = DateTime.Now;
    }
}
