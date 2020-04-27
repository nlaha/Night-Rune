using System;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NightRune.Services;

namespace NightRune
{
    class Program
    {
        /* Keep This File Super Simple. (This Method Requires C# 7.2 or Higher!) */
        private static Task Main()
            => new DiscordService().InitializeAsync();
    }
}
