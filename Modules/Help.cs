
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NightRune.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("Help")]
        public async Task Pong()
        {
            var embed = new EmbedBuilder();

            embed.AddField($"Play <URL or search term>",
                $"Searches youtube for a song, or plays a URL from soundcloud, vimeo, youtube, twitch and bandcamp.")
                .AddField($"Stop",
                $"Stops the song and leaves the channel")
                .AddField($"Skip",
                $"Skips to the next song in the queue")
                .AddField($"Volume <1-150>",
                $"Volume from 0 to 150")
                .AddField($"Pause/Resume",
                $"Run these commands to pause and resume the music")
                .AddField($"Queue",
                $"Shows the number of tracks in the queue")
                .AddField($"Clear",
                $"Clears the tracks in the queue")
                .WithAuthor("Night Rune Help")
                .WithColor(Color.Blue)
                .WithTitle($"NightRune is at the moment very simple, it's a music bot!")
                .WithCurrentTimestamp()
                .Build();

            await ReplyAsync(embed: embed.Build());
        }
    }
}