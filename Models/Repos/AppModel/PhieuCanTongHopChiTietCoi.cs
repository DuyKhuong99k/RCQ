using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.AppModel
{
    public class PhieuCanTongHopChiTietCoi
    {
        public string ChatLuongName { get; set; }

        public string ChieuXaName { get; set; }

        public bool ChuyenXuong { get; set; }

        public bool Forced { get; set; }

        public int LanQuay { get; set; }

        public string LoaiCaName { get; set; }

        public string MaCoiChinh { get; set; }

        public string MaLo { get; set; }

        public string MauName { get; set; }

        public string MaXuong { get; set; }

        public string SizeName { get; set; }

        public int SoLanQuay { get; set; }

        public int SoRo { get; set; }

        public string ThanhPhamName { get; set; }

        public TimeSpan ThoiGianBatDauQuay { get; set; }

        public int ThoiGianQuay { get; set; }

        public int ThoiGianQuayThucTe { get; set; }

        public TimeSpan ThoiGianRaCoi { get; set; }

        public double TrongLuong { get; set; }
    }
}
