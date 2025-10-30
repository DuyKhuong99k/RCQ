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
    [Table(nameof(MaChatLuongXepKhuon_U))] // "MaChatLuongXepKhuon
    public partial class MaChatLuongXepKhuon_U
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaChatLuong { get; set; } = null!;

        [StringLength(200)] public string Ten { get; set; } = null!;

        public bool SuDung { get; set; }

        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;
    }
}
