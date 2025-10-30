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
    [Table(nameof(HQ_MapSanPhamTinhLuong))]
    public partial class HQ_MapSanPhamTinhLuong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MaSanPham { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string MaThanhPham { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string MaSize { get; set; } = null!;
        [Column(TypeName = "datetime2(7)")]
        public DateTime NgayGio { get; set; } = DateTime.Now;
        [StringLength(50)]
        [Unicode(false)]
        public string MaLoaiNguyenLieu { get; set; } = null!;
    }
}
