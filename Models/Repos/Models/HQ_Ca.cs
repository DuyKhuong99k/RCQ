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
    [Table(nameof(HQ_Ca))]
    public partial class HQ_Ca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Ma { get; set; } = null!;

        [StringLength(200)]
        public string Ten { get; set; } = null!;
        
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime GioBatDau { get; set; } = DateTime.Now;
        
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime GioKetThuc { get; set; } = DateTime.Now;
        public bool IsQuaDem { get; set; } = false;
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime NgayGio { get; set; } = DateTime.Now;
    }
}
