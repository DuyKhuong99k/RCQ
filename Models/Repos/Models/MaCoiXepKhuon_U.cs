using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Repos.A_Model;
using MvvmHelpers;

namespace Models.Repos.Models
{
    [Table(nameof(MaCoiXepKhuon_U))] // "MaCoiXepKhuon
    public partial class MaCoiXepKhuon_U
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaCoi { get; set; } = null!;

        [StringLength(200)]
        public string Ten { get; set; } = null!;

        public double TrongLuongMax { get; set; }

        public bool Tam { get; set; }
        [NotMapped]
        public ObservableRangeCollection<CoiTamChiTiet> CoiTamChiTiets { get; set; } = new();
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; } = DateTime.Now;
    }
}
