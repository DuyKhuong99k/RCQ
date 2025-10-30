using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.A_Model
{
    public class ProgressTPSoft
    {
        public DateTime dateTime { get; set; }
        public string MaXuong { get; set; }
        public IProgress<string> Progress { get; set; }
    }
}
