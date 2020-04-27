using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NightRune.Services;
using Victoria;
using Victoria.Enums;

namespace NightRune.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;
        private readonly LavaNode _lavaNode;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        public Music(MusicService musicService, LavaNode lavaNode)
        {
            _musicService = musicService;
            _lavaNode = lavaNode;
        }

        [Command("Join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("You need to connect to a voice channel.");
                return;
            }
            else
            {
                await _musicService.ConnectAsync(Context.Guild, user.VoiceState, Context.Channel as ITextChannel);
                await ReplyAsync($"now connected to {user.VoiceChannel.Name}");
            }
        }

        [Command("Leave")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("Please join the channel the bot is in to make it leave.");
            }
            else
            {
                await _musicService.LeaveAsync(user.Guild);
                await ReplyAsync($"Bot has now left {user.VoiceChannel.Name}");
            }
        }

        [Command("Play")]
        public async Task Play([Remainder]string query)
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("You need to connect to a voice channel.");
                return;
            }
            else
            {
                await _musicService.ConnectAsync(Context.Guild, user.VoiceState, Context.Channel as ITextChannel);
                await ReplyAsync($"now connected to {user.VoiceChannel.Name}");
                await ReplyAsync(embed: await _musicService.PlayAsync(query, Context.Guild, user));
            }
        }

        [Command("Stop")]
        public async Task Stop() {
            await ReplyAsync(embed: await _musicService.StopAsync(Context.Guild));
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("I can't leave the voice channel because you aren't in it!");
            }
            else
            {
                await _musicService.LeaveAsync(user.Guild);
                await ReplyAsync($"I have left the channel: \"{user.VoiceChannel.Name}\"");
            }
        }

        [Command("Queue")]
        public async Task Queue()
            => await ReplyAsync(embed: await _musicService.ShowQueue(Context.Guild));

        [Command("Clear")]
        public async Task Clear()
            => await ReplyAsync(embed: await _musicService.ClearQueue(Context.Guild));

        [Command("Skip")]
        public async Task Skip()
            => await ReplyAsync(embed: await _musicService.SkipAsync(Context.Guild));

        [Command("Volume")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol, Context.Guild));

        [Command("Pause")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseAsync(Context.Guild));

        [Command("Resume")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync(Context.Guild));

        [Command("Lyrics", RunMode = RunMode.Async)]
        public async Task Lyrics()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("I'm not playing any tracks.");
                return;
            }

            var lyrics = await player.Track.FetchLyricsFromOVHAsync();
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync($"No lyrics found for {player.Track.Title}");
                return;
            }

            var splitLyrics = lyrics.Split('\n');
            var stringBuilder = new StringBuilder();
            foreach (var line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            await ReplyAsync("Lyrics for:\n" + player.Track.Title + "\n" + $"```{stringBuilder}```");
        }
    }
}