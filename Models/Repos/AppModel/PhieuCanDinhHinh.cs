using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.AppModel
{
    public class PhieuCanDinhHinh
    {
        public DateTime Ngay { get; set; }

        public string MaNhanVien { get; set; }

        public bool SuDung { get; set; } = true;

        public string MaSanPham { get; set; }

        public string TenSanPham { get; set; }

        public decimal TrongLuongNhan { get; set; }

        public decimal TrongLuongTra { get; set; }

        public decimal DinhMucThucTe { get; set; }

        public decimal DinhMucYeuCau { get; set; }

        public bool DanhGia { get; set; }

        public int SoRo { get; set; }

        public int _Status { get; set; } = 0;

        public string CaLamViec { get; set; }
    }
}
