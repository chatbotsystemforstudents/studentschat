using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TextToSpeech.Interfaces;

namespace TextToSpeech.EndPointHandlers
{
    public class TextToSpeechEndPoint
    {
        public string Text { get;}
        public string Guid { get; set; }
        public TextToSpeechEndPoint(string text, string guid)
        {
            Text = text;
            Guid = guid;
        }
    }
    internal class TextToSpeechEndPointHandler : IEndPointHandler<TextToSpeechEndPoint>
    {
        private readonly ILogger<TextToSpeechEndPointHandler> _logger;
        public TextToSpeechEndPointHandler(ILogger<TextToSpeechEndPointHandler> logger)
        {
            _logger = logger;
        }
        public async Task Handle(TextToSpeechEndPoint data)
        {
            _logger.LogInformation($"Handling TextToSpeechEndPoint with text: {data.Text}");
            try
            {
                await new SpeechGenerator().GenerateAndSaveAudioAsync(data.Text, $"{data.Guid}.wav");
                _logger.LogInformation($"TextToSpeechEndPoint handled successfully");
            }catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling TextToSpeechEndPoint");
            }
        }
    }
}
