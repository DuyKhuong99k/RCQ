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
    [Table(nameof(HQ_Lo))]
    public partial class HQ_Lo
    {
        [Key]  
        [StringLength(50)]
        [Unicode(false)]
        [Required]
        public required string Id { get; set; }
        [Required]
        public DateOnly NgayNguyenLieu { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required]
        public bool SuDung { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;

    }
}
