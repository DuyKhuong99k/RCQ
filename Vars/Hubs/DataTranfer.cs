using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class DataTranfer
    {
        public bool IsError { get; set; }=false;
        public string? Data { get; set; }
        public string? DataError { get; set; }
    }
}
