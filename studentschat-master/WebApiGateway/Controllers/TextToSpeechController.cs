using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Shared.Requests;
using Tcp;
using WebApiGateway.Configuration;
using WebApiGateway.Models;

namespace WebApiGateway.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class TextToSpeechController : ControllerBase
    {
        private readonly ILogger<TextToSpeechController> _logger;
        private readonly Client _client;
        private readonly ServerConfiguration _speechServerConfiguration;
        private readonly AppConfig _appConfig;
        private static string _userChatHistory = string.Empty;
        private static string _agentChatHistory = string.Empty;
        public TextToSpeechController(ILogger<TextToSpeechController> logger,Client client,ServerConfiguration serverConfiguration,AppConfig appConfig)
        {
            _logger = logger;
            _client = client;
            _speechServerConfiguration = serverConfiguration;
            _appConfig = appConfig;
        }
        [HttpPost("GenerateLipSync", Name = "GenerateLipSync")]
        public async Task<ActionResult> Generate(Models.LipSyncRequest request)
        {
            string documentsPath = _appConfig.LipSyncOutputDir;
            string outputFileName = $"{request.Guid}.json"; 
            string audioFileName = $"{request.Guid}.wav"; 

            string outputFilePath = Path.Combine(documentsPath, outputFileName);
            string audioFilePath = Path.Combine(documentsPath, audioFileName);                                    
            _logger.LogInformation($"Generating lip sync for audio file: {audioFilePath} with destination file {outputFileName}");
            var processInfo = new ProcessStartInfo
            {
                FileName = "C:\\Rhubarb-Lip-Sync-1.13.0\\rhubarb", // Assuming rhubarb is in PATH, otherwise provide the full path
                Arguments = $"-f json -r phonetic -o \"{outputFilePath}\" \"{audioFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false, // Required for redirection
                CreateNoWindow = true // Avoid showing a console window
            };

            try
            {
                using (var process = Process.Start(processInfo))
                {
                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        return StatusCode(500, $"Error: {error}");
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating lip sync");
                return StatusCode(500, $"Exception: {ex.Message}");
            }
        }
        [HttpPost("GenerateSpeech", Name = "GenerateSpeech")]
        public async Task<ActionResult<TextToSpeechResponse>> Generate(Models.TextToSpeechRequest request)
        {
            _logger.LogInformation("Generating speech for text: {text}", request.Text);
            var guid = Guid.NewGuid().ToString();
            //TODO:properly await or queue the process
            ProcessText(request.Text,guid);            
            return Ok(new TextToSpeechResponse()
            {
                FileName = guid
            });
        }        
        private async Task ProcessText(string text,string guid)
        {            
            var responseFromChatGpt = await new ChatGptClient.Chat().Submit(text,_userChatHistory,_agentChatHistory);
            _logger.LogInformation("got response from chat: {response}", responseFromChatGpt);
            _userChatHistory += "\n" + text;
            _agentChatHistory += "\n" + responseFromChatGpt.ClearTextFormat;
            var responseFromSpeechServer=await _client.Send(new TcpRequest("TextToSpeech",
                JsonSerializer.Serialize(new Shared.Requests.TextToSpeechRequest
                {
                    Text = responseFromChatGpt.SsmlFormat,
                    Guid = guid
                }
               )),_speechServerConfiguration.IpAddress,_speechServerConfiguration.Port);
            if(responseFromSpeechServer)
                _logger.LogInformation("sent request to text to speech service for guid"+guid);
            else
                _logger.LogError("Error occurred while sending request to text to speech service for guid" + guid);
        }
    }
}
