using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
namespace TextToSpeech
{
    public class SpeechGenerator
    {
        public async Task GenerateAndSaveAudioAsync(string text, string fileName)
        {
            var synthesizer = new SpeechSynthesizer();
            var greekVoice = SpeechSynthesizer.AllVoices.FirstOrDefault(v => v.DisplayName == "Microsoft Stefanos");

            if (greekVoice != null)
            {
                synthesizer.Voice = greekVoice;                

                var stream = await synthesizer.SynthesizeSsmlToStreamAsync(text);

                // Save the audio to a file in the local folder
                var storageFile = await KnownFolders.DocumentsLibrary.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using (var outputStream = await storageFile.OpenStreamForWriteAsync())
                {
                    await stream.AsStreamForRead().CopyToAsync(outputStream);
                }
            }
        }
    }
}

