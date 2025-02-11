using System.IO;
using System;
using Serilog.Core;
using Serilog.Events;
using Windows.Storage;
namespace TextToSpeech
{
    public class UwpFileSink : ILogEventSink
    {
        private readonly string _fileName;

        public UwpFileSink(string fileName)
        {
            _fileName = fileName;
        }

        public async void Emit(LogEvent logEvent)
        {
            try
            {
                var storageFile = await KnownFolders.DocumentsLibrary.CreateFileAsync(_fileName, CreationCollisionOption.OpenIfExists);
                var message = logEvent.RenderMessage();

                using (var stream = await storageFile.OpenStreamForWriteAsync())
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteLineAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Ignore exceptions
            }            
        }
    }
}