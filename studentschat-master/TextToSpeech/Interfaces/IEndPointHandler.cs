using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeech.Interfaces
{
    internal interface IEndPointHandler<EndPointData> where EndPointData : class
    {
        Task Handle(EndPointData data);
    }
}
