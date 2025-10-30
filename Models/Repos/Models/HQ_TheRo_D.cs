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
    [Table(nameof(HQ_TheRo_D))]
    public class HQ_TheRo_D

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaThe { get; set; } = null!;

        [StringLength(50)]
        [Unicode(false)]
        public string ColorCode { get; set; } = null!;

        public DateTime CreatedDateTime { get; set; }

        public bool IsRach { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? MaThanhPhamDinhHinh { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime Ngay { get; set; } = DateTime.Now;

    }
}
