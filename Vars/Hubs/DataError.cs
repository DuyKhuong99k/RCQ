using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class DataError
    {
        public string ErrorString { get; set; }
        public int ErrorCode { get; set; } = 0;
        public string Cmd { get; set; }
    }
}
