using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.AppModel
{
    public class PhieuCanTongHop
    {
        public string MaLo { get; set; }

        public string LoaiCaName { get; set; }

        public string ThanhPhamName { get; set; }

        public string SizeName { get; set; }

        public string ChieuXaName { get; set; }
        public string ChatLuongName { get; set; }

        public string MauName { get; set; }

        public decimal TrongLuong { get; set; }

        public string MaXuong { get; set; }

        public bool ChuyenXuong { get; set; }
        public int STT { get; set; }
    }
}
