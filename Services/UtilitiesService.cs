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
using NightRune.Services;

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

        private Task LogAsync(LogMessage logMessage)
        {
            Log.Information(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
