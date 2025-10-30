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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

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

    }
}
