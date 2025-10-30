using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.AppModel
{
    public class PhieuCanTongHopCoi
    {
        public string MaCoiChinh { get; set; }
        public string CoiChinhName { get; set; }
        public string MaMau { get; set; }
        public string MauName { get; set; }
        public string MaLo { get; set; }

        public string LoaiCaName { get; set; }

        public string ThanhPhamName { get; set; }

        public string ChieuXaName { get; set; }


        public string SizeName { get; set; }

        public string ChatLuongName { get; set; }

        public double TrongLuong { get; set; }

        public int SoRo { get; set; }

        public string MaXuong { get; set; }

        public bool ChuyenXuong { get; set; }

        public TimeSpan ThoiGianBatDauQuay { get; set; }
        public TimeSpan ThoiGianRaCoi { get; set; }
        public int ThoiGianQuay { get; set; }
        public DateTime Ngay { get; set; }
        public string MaNhanVien { get; set; }
        public DateTime NgayNguyenLieu { get; set; }
        public DateTime NgayBatDauQuay { get; set; }
        public DateTime NgayRaCoi { get; set; }
        public bool RaCoi { get; set; }
        public decimal TongTGQuay2 { get; set; }
    }
}
