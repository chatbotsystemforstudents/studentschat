using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tcp
{
    public class TcpResponse
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public string Response { get; set; }
        public TcpResponse() { }               
    }
}
