using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface ISlmpConfig
    {
        /// <summary>
        /// IP address of the target SLMP-compatible device
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// The port that SLMP server is configured to run on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Connection timeout.
        /// </summary>
        public int ConnTimeout { get; set; }

        /// <summary>
        /// Receive timeout.
        /// </summary>
        public int RecvTimeout { get; set; }
        /// Send timeout.
        /// </summary>
        public int SendTimeout { get; set; } 

        /// <summary>
        /// Initialize a new `SlmpConfig` class
        /// </summary>
        public void Initialize(string address, int port);
    }
}
