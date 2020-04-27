using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Serilog;

namespace NightRune.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider services)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _cmdService.Log += LogAsync;
            _client.MessageReceived += HandleMessageAsync;

            // make our logger instance
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            var argPos = 0;
            if (socketMessage.Author.IsBot) return;

            var userMessage = socketMessage as SocketUserMessage;
            if (userMessage is null)
                return;

            // If we don't have a prefix, return
            if (!(userMessage.HasCharPrefix('!', ref argPos)) || userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _cmdService.ExecuteAsync(context, argPos, _services);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Log.Information(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}