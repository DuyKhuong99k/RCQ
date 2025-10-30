using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class ClientInfo
    {
        public string ConnectedId { get; set; }
        /// <summary>
        /// Id(tên) máy Cân
        /// </summary>
        public string Id { get; set; }
        public string? IPAddr { get; set; }

        public AppKV WKv { get; set; } = 0;
        public DateTime DateTimeConnected { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = false;
        public bool IsMayCan { get; set; } = false;
    }
}
