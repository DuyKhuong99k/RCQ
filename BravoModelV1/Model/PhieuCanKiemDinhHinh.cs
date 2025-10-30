using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Model
{
    public class PhieuCanKiemDinhHinh
    {
        public DateTime Ngay { get; set; }

        public string CaLamViec { get; set; }

        public string MaNhanVien { get; set; }

        public string MaSanPham { get; set; }

        public string TenSanPham { get; set; }

        public decimal TrongLuong { get; set; }

        public string KhuVuc { get; set; }

        public int _Status { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public bool IsChamCong { get; set; }
    }
}
