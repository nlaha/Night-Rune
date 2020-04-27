using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NightRune.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NightRune.Modules
{
    public class Utilities : ModuleBase<SocketCommandContext>
    {
        private UtilitiesService _utilitiesService;
        private DiscordSocketClient _client;

        public Utilities(UtilitiesService utilitiesService, DiscordSocketClient client)
        {
            _utilitiesService = utilitiesService;
            _client = client;
        }

        [Command("Purge")]
        [Summary("Deletes set amount of messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(uint amount)
        {
            if (amount == 0 || amount.GetType() == null)
            {
                await ReplyAsync("Please specify a number of messages to delete");
            }

            var user = Context.User as SocketGuildUser;
            var channel = Context.Channel as SocketGuildChannel;
            var messages = await Context.Message.Channel.GetMessagesAsync((int)amount).FlattenAsync();

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            await ReplyAsync($"{amount} Messages Purged");

        }

    }
}
