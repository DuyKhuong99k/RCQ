using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class DataLite
    {
        public string Id { get; set; }
        public string TheId { get; set; }
        public decimal TrongLuong { get; set; }
        public decimal TrongLuongTare { get; set;}
        public string NgayGio { get; set; }
        public string Stated {get; set; }
        public bool IsWaiting { get; set; } = false;
    }
}
