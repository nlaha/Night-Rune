using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NightRune.Handlers;
using Serilog;
using System;
using System.Threading.Tasks;
using Victoria;

namespace NightRune.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly ServiceProvider _services;
        private readonly LavaNode _lavaNode;
        private readonly MusicService _audioService;
        private readonly UtilitiesService _utilitiesService;
        private readonly GlobalData _globalData;

        public DiscordService()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commandHandler = _services.GetRequiredService<CommandHandler>();
            _lavaNode = _services.GetRequiredService<LavaNode>();
            _globalData = _services.GetRequiredService<GlobalData>();
            _audioService = _services.GetRequiredService<MusicService>();
            _utilitiesService = _services.GetRequiredService<UtilitiesService>();

            SubscribeLavaLinkEvents();
            SubscribeDiscordEvents();
        }

        /* Initialize the Discord Client. */
        public async Task InitializeAsync()
        {
            Log.Information("Initialization Started");

            await InitializeGlobalDataAsync();

            await _client.LoginAsync(TokenType.Bot, GlobalData.Config.DiscordToken);
            await _client.StartAsync();

            await _audioService.InitializeAsync();
            await _utilitiesService.InitializeAsync();
            await _commandHandler.InitializeAsync();

            Log.Information("Initialization Complete");

            await Task.Delay(-1);
        }

        /* Hook Any Client Events Up Here. */
        private void SubscribeLavaLinkEvents()
        {
            _lavaNode.OnLog += LogAsync;
            _lavaNode.OnTrackEnded += _audioService.TrackEnded;
        }

        private void SubscribeDiscordEvents()
        {
            _client.Ready += ReadyAsync;
            _client.Log += LogAsync;
        }

        private async Task InitializeGlobalDataAsync()
        {
            await _globalData.InitializeAsync();
        }

        /* Used when the Client Fires the ReadyEvent. */
        private async Task ReadyAsync()
        {
            try
            {
                await _lavaNode.ConnectAsync();
                await _client.SetGameAsync(GlobalData.Config.GameStatus);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Source, ex.Message);
            }

        }

        /*Used whenever we want to log something to the Console. Log via Serilog*/
        private async Task LogAsync(LogMessage logMessage)
        {
            Log.Information(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        /* Configure our Services for Dependency Injection. */
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LavaNode>()
                .AddSingleton(new LavaConfig())
                .AddSingleton<MusicService>()
                .AddSingleton<UtilitiesService>()
                .AddSingleton<GlobalData>()
                .BuildServiceProvider();
        }
    }
}