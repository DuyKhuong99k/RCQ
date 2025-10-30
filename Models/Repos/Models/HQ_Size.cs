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
    [Table(nameof(HQ_Size))]
    public partial class HQ_Size
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
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;
    }
}
