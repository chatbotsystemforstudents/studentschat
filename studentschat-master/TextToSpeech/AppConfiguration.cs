using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace TextToSpeech
{
    internal class AppConfiguration
    {
        public static async Task<T> LoadAsync<T>(string fileName)
        {
            try
            {
                // Locate the file in the app package
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{fileName}"));

                // Read the file content
                var fileContent = await FileIO.ReadTextAsync(file);

                // Deserialize the JSON content to a strongly-typed object
                return JsonSerializer.Deserialize<T>(fileContent);
            }catch (Exception ex)
            {
                throw new Exception($"Error loading configuration file: {fileName}", ex);
            }
        }
    }
}
