using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    public class HQ_PhieuCanXuatNguyenLieu
    {
        [Key]
        public string Id { get; set; }
        public int STT { get; set; }
        public long IdPhieuCanNguyenLieu { get; set; }
        public string SoPhieuNhap { get; set; } = null!;
        public string SoPhieuXuat { get; set; } = null!;
        public string MaThuKho { get; set; } = null!;
        public long MaSanPham { get; set; }
        public decimal TrongLuongTong { get; set; }
        public decimal TrongLuongXe { get; set; }
        public decimal TrongLuongHang { get; set; }
        public long MaDonVi { get; set; }
        public string MaXuongXuatDen { get; set; } = null!;
        public long MaKho { get; set; }
    }
}
