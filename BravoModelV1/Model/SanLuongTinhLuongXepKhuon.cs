using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Model
{
    public class SanLuongTinhLuongXepKhuon
    {
        public decimal GioTyLe { get; set; }
        public DateTime Ngay { get; set; }

        public string MaCongViec { get; set; }
        public string TenCongViec { get; set; }

        public string MaNhanVien { get; set; }
        public string MaHoSo { get; set; }

        public string MaNhom { get; set; }


        public decimal SanLuongHuong { get; set; }

        public decimal SanLuongTrenGio { get; set; }

        public decimal SanLuongTru { get; set; }


        public decimal SoGio { get; set; }

        public decimal TyLeHuong { get; set; }

        public decimal TyLeTru { get; set; }

        public string CaId { get; set; }

        public string BravoId { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public bool IsChamCong { get; set; }
    }
}
