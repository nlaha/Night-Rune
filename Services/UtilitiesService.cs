using Discord;
using Discord.WebSocket;
using Serilog;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace NightRune.Services
{
    public class UtilitiesService
    {
        private DiscordSocketClient _client;
        public UtilitiesService(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task InitializeAsync()
        {
            // make our logger instance
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            return Task.CompletedTask;
        }

        public async Task<string> DeepFry(string filename, byte[] buffer)
        {
            Log.Information($"Deep Frying {filename}");

            SixLabors.ImageSharp.Image src_image;
            using (FileStream stream = File.OpenRead(filename))
            src_image = SixLabors.ImageSharp.Image.Load(stream);
            src_image.Mutate(x => x.Brightness(1.1f));
            src_image.Mutate(x => x.Contrast(5f));
            using (FileStream output = File.OpenWrite(filename))
                src_image.Save(output, new JpegEncoder());

            var result = filename;

            Log.Information("Frying Done!");

            return result;
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Log.Information(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
