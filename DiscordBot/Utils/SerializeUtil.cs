using System.IO;
using System.Threading.Tasks;
using DiscordBot.Services.Logging;
using Newtonsoft.Json;
using Discord;

namespace DiscordBot.Utils
{
    public static class SerializeUtil
    {
        public static T DeserializeFile<T>(string path, bool newFileIfNotExists = true) where T : new()
        {
            // Check if file exists,
            if (!File.Exists(path))
            {
                if (newFileIfNotExists)
                {
                    LoggingService.LogToConsole($@"Deserialized File at '{path}' does not exist, attempting to generate new file.",
                        LogSeverity.Warning);
                    var deserializedItem = new T();
                    File.WriteAllText(path, JsonConvert.SerializeObject(deserializedItem));
                }
                else
                {
                    LoggingService.LogToConsole($@"Deserialized File at '{path}' does not exist.", LogSeverity.Error);
                }
            }

            using var file = File.OpenText(path);
            var content = JsonConvert.DeserializeObject<T>(file.ReadToEnd()) ?? new T();
            return content;
        }

        /// <summary> Tests objectToSerialize to confirm not null before saving it to path. </summary>
        public static bool SerializeFile<T>(string path, T objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                LoggingService.LogToConsole($"Object `{path}` passed into SerializeFile is null, ignoring save request.",
                    LogSeverity.Warning);
                return false;
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(objectToSerialize));
            return true;
        }

        public static async Task<bool> SerializeFileAsync<T>(string path, T objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                LoggingService.LogToConsole($"Object `{path}` passed into SerializeFile is null, ignoring save request.",
                    LogSeverity.Warning);
                return false;
            }
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(objectToSerialize));
            return true;
        }
    }
}