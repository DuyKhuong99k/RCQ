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
    [Table(nameof(HQ_NhanVienTheoNhom))]
    public class HQ_NhanVienTheoNhom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string MaNhanVien { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string MaNhom { get; set; }
        [Column(TypeName = "datetime2(7)")] public DateTime NgayGioBatDau { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")] public decimal HeSo { get; set; } = 1;
    }
}
