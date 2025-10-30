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
    [Table(nameof(HQ_ColorCode_D))]
    public partial class HQ_ColorCode_D
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Code { get; set; } = null!;

        [StringLength(200)]
        public string Name { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string Code2 { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string? ColorRGB { get; set; }
        [Required]
        public DateOnly MNgay { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime Ngay { get; set; } = DateTime.Now;
    }
}
