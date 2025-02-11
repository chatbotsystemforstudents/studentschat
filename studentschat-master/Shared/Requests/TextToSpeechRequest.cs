using System;

namespace Shared.Requests
{
    public class TextToSpeechRequest:IRequest
    {
        public string Text { get; set; }
        public string Guid { get; set; }
    }
}
