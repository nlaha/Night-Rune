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
    public class MidiSynth : ModuleBase<SocketCommandContext>
    {
        private UtilitiesService _utilitiesService;
        private DiscordSocketClient _client;

        public MidiSynth(UtilitiesService midiSynthService, DiscordSocketClient client)
        {
            _utilitiesService = midiSynthService;
            _client = client;
        }

        [Command("Fry")]
        [Summary("Deep fries your attached image")]
        public async Task Fry()
        {
            var attachments = Context.Message.Attachments;
      
            // Create a new WebClient instance.
            WebClient webClient = new WebClient();

            string file = attachments.ElementAt(0).Filename;
            string url = attachments.ElementAt(0).Url;

            // Download the resource and load the bytes into a buffer.
            byte[] buffer = webClient.DownloadData(url);

            // Encode the buffer into UTF-8
            string download = Encoding.UTF8.GetString(buffer);

            var user = Context.User as SocketGuildUser;

            // Save the unfried image as a file
            MemoryStream inStream = new MemoryStream();
            inStream.Write(buffer, 0, buffer.Length);
            FileStream inFileStream = new FileStream(file, FileMode.Create, System.IO.FileAccess.Write);
            inStream.WriteTo(inFileStream);
            inStream.Dispose();
            inFileStream.Dispose();

            // Fry the image
            await Context.Channel.SendFileAsync(await _utilitiesService.DeepFry(file, buffer));

            // Delete the file when we're done with it
            File.Delete(file);
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

            ReplyAsync($"{amount} Messages Purged");

        }

    }
}
