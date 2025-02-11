using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tcp
{
    public class ServerConfiguration
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public ServerConfiguration() { }
        public ServerConfiguration(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
