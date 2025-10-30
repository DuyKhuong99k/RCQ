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
    [Table(nameof(HQ_ThanhPham))]
    public partial class HQ_ThanhPham
    {
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string Id { get; set; } = null!;
        [Required]
        [StringLength(200)]
        [Unicode(true)]
        public string Ten { get; set; } = null!;

        [Required] public bool SuDung { get; set; } = false;
        [Required] [Column(TypeName = "datetime2(7)")] public DateTime MNgay { get; set; } = DateTime.Now;
        [Required] 
        [Column(TypeName = "decimal(18,3)")]
        public decimal Max {get; set; } = 100;
        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal Min { get; set; } = 0;
    }
    
}
