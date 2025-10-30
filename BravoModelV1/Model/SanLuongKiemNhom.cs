using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoModelV1.Model
{
    public class SanLuongKiemNhom
    {
        public Models.Repos.Models. ToKiem ToKiem { get; set; }

        public decimal TongSanLuong { get; set; }

        public decimal TrongLuongTrenGio { get; set; }

        public decimal TongGio { get; set; }
    }
}
