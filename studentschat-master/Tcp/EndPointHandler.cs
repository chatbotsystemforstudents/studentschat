using System;
using System.Threading.Tasks;

namespace Tcp
{
    public class EndPointHandler
    {
        public string EndPoint { get; private set; }
        public Func<string,Task> Handler { get; private set; }
        public EndPointHandler(string endPoint, Func<string, Task> handler)
        {
            EndPoint = endPoint;
            Handler = handler;
        }
    }
}
