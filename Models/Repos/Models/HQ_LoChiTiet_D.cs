using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table(nameof(HQ_LoChiTiet_D))]
    public partial class HQ_LoChiTiet_D
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public required string LoChiTietId { get; set; }

        [Required]
        [StringLength(200)]
        [Unicode(true)]
        public required string Ten { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        [Required]
        public required string LoId { get; set; }
       

        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal TyLe { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime Ngay { get; set; } = DateTime.Now;
    }
}
