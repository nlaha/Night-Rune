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

        public MidiSynth(UtilitiesService midiSynthService)
        {
            _utilitiesService = midiSynthService;
        }

        [Command("Fry")]
        public async Task Synth()
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

            await Context.Channel.SendFileAsync(await _utilitiesService.DeepFry(file, buffer));
            File.Delete(file);
        }

    }
}
