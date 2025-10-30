using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.SoketModels
{
    public class PhieuCan
    {
        public string Id { get; set; }
        public int STT { get; set; } = 0;
        public string NgayGio { get; set; }
        public string MayCan { get; set; }
        public string MaLo { get; set; }
        public string MaSize { get; set; }
        public string MaThanhPham { get; set; }
        public string MaNhanVien { get; set; }
        public decimal TrongLuongNhan { get; set; } = 0.000m;
        public decimal TrongLuongTra { get; set; } = 0.000m;
        public decimal DinhMucYeuCau { get; set; } = 0.000m;
        public decimal TrongLuongTare { get; set; } = 0.000m;
        public int ChiSanLuong { get; set; } = 0;
        public string MaNhanVienPhucVu { get; set; }
        public string MaNhanVienBanKiem { get; set; }
        public string TheId { get; set; }
        public string TheIdChucNang { get; set; }
        public string Ngay { get; set; }
        public int Status { get; set; } = 0;
    
    }
}
