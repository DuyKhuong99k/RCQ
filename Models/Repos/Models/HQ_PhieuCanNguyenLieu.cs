using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    public class HQ_PhieuCanNguyenLieu
    {
        [Key]
        public string Id { get; set; }
        public int STT { get; set; }
        public DateOnly Ngay { get; set; }
        public DateTime NgayGio { get; set; }
        public string MayCan { get; set; } = null!;
        public string MaLo { get; set; } = null!;
        public string? GhiChu { get; set; } = null!;
        public string MaXuong { get; set; } = null!;
    }
}
