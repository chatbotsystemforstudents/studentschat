using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tcp
{
    public class TcpRequest
    {
        public string EndPoint { get; set; }
        public string Json { get; set; }
        public TcpRequest() { }
        public TcpRequest(string endPoint, string json)
        {
            EndPoint = endPoint;
            Json = json;
        }
    }
}
