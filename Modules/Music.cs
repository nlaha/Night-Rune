using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NightRune.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria.Entities;

namespace NightRune.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;

        public Music(MusicService musicService)
        {
            _musicService = musicService;
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
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
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
                await _musicService.LeaveAsync(user.VoiceChannel);
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
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"now connected to {user.VoiceChannel.Name}");

                var results = await _musicService.GetTracksAsync(query, Context.Guild.Id);

                // Check if we get any results
                if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
                {
                    var embed = new EmbedBuilder();

                    embed.WithAuthor(Context.User)
                        .WithColor(Color.Red)
                        .WithTitle("No Matches found!")
                        .WithDescription("Please enter a search term or a URL")
                        .WithCurrentTimestamp()
                        .Build();

                    await ReplyAsync(embed: embed.Build());
                }

                List<LavaTrack> trackList = results.Tracks.ToList();
                var resultEmbed = new EmbedBuilder();

                // build the search result track list embed
                resultEmbed.WithAuthor(Context.User)
                    .WithColor(Color.Green)
                    .WithTitle($"Search Results")
                    .WithCurrentTimestamp()
                    .WithDescription($"Total number of Results: {trackList.Count}\nShowing the top 5\n" +
                    $"Please respond with 1 to 5 to select a track:\n");

                for (int i = 0; i < trackList.Count; i++)
                {
                    if (i < 5)
                    {
                        var ListEmote = "📕";

                        switch (i)
                        {
                            case 0:
                                ListEmote = "📕";
                                break;
                            case 1:
                                ListEmote = "📗";
                                break;
                            case 2:
                                ListEmote = "📘";
                                break;
                            case 3:
                                ListEmote = "📙";
                                break;
                            case 4:
                                ListEmote = "📒";
                                break;
                        }


                        var ctrack = trackList[i];
                        // Show the track info in the list
                        resultEmbed.AddField($"[{ListEmote}]: {ctrack.Title}\n{ctrack.Uri}", $"*Author:* {ctrack.Author}\n" +
                        $"*Song Length:* {ctrack.Length.Hours}:{ctrack.Length.Minutes}:{ctrack.Length.Seconds} (H:M:S)\n");
                    }
                }
                IUserMessage resultMessage = await ReplyAsync(embed: resultEmbed.Build());
                
                var emoji1 = new Emoji("📕");
                var emoji2 = new Emoji("📗");
                var emoji3 = new Emoji("📘");
                var emoji4 = new Emoji("📙");
                var emoji5 = new Emoji("📒");
                /*
                var emoji1 = Emote.Parse("<:one:>");
                var emoji2 = Emote.Parse("<:two:>");
                var emoji3 = Emote.Parse("<:three:>");
                var emoji4 = Emote.Parse("<:four:>");
                var emoji5 = Emote.Parse("<:five:>");
                */
                IEmote[] oneToFiveEmojis = new IEmote[5] { emoji1, emoji2, emoji3, emoji4, emoji5 };

                _musicService.resetTracks();
                await resultMessage.AddReactionsAsync(oneToFiveEmojis);

                await ReplyAsync(embed: await _musicService.PlayAsync(trackList, Context.Guild.Id));

            }
        }

        [Command("Stop")]
        public async Task Stop() {
            await ReplyAsync(await _musicService.StopAsync(Context.Guild.Id));
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("I can't leave the voice channel because you aren't in it!");
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);
                await ReplyAsync($"I have left the channel: \"{user.VoiceChannel.Name}\"");
            }
        }

        [Command("Queue")]
        public async Task Queue()
            => await ReplyAsync(embed: await _musicService.ShowQueue(Context.Guild.Id));

        [Command("Clear")]
        public async Task Clear()
            => await ReplyAsync(await _musicService.ClearQueue(Context.Guild.Id));

        [Command("Skip")]
        public async Task Skip()
            => await ReplyAsync(embed: await _musicService.SkipAsync(Context.Guild.Id));

        [Command("Volume")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol, Context.Guild.Id));

        [Command("Pause")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseOrResumeAsync(Context.Guild.Id));

        [Command("Resume")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync(Context.Guild.Id));
    }
}