using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models
{ [Table("TrongLuongCoiTheoThanhPham")] 
    [PrimaryKey("MaCoi", "MaThanhPham", "MaXuong")]
    public partial class TrongLuongCoiTheoThanhPham
    {
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string MaCoi { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string MaThanhPham { get; set; } = null!;
        [StringLength(50)]
        [Unicode(false)]
        public string MaXuong { get; set; } = null!;
        public decimal TrongLuongMax { get; set; } = 500;
    }
    
}
