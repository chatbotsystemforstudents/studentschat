using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tcp;
using TextToSpeech.EndPointHandlers;

namespace TextToSpeech
{
    public static class EndPoints
    {
        public static void Register(Server server)
        {
            server.RegisterEndPoint("TextToSpeech", async (json) =>
            {
                //deserialize json to TextToSpeechEndPoint object
                var request=JsonConvert.DeserializeObject<Shared.Requests.TextToSpeechRequest>(json);
                if (request == null)
                {
                    Console.WriteLine("Invalid request.");
                    return;
                }
                await new TextToSpeechEndPointHandler(App.ServiceProvider.GetService<ILogger<TextToSpeechEndPointHandler>>()).Handle(new EndPointHandlers.TextToSpeechEndPoint(request.Text,request.Guid));
            });
        }
    }
}
