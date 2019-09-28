using Discord;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace NightRune.Services
{
    public class MusicService
    {
        private LavaRestClient _lavaRestClient;
        private LavaSocketClient _lavaSocketClient;
        private DiscordSocketClient _client;
        public List<LavaTrack> QueueData = new List<LavaTrack>();

        public MusicService(LavaRestClient lavaRestClient, DiscordSocketClient client, LavaSocketClient lavaSocketClient)
        {
            _client = client;
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
        }

        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += TrackFinished;

            // make our logger instance
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            return Task.CompletedTask;
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await _lavaSocketClient.ConnectAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await _lavaSocketClient.DisconnectAsync(voiceChannel);

        public async Task<Embed> PlayAsync(string query, ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);
            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                var embed = new EmbedBuilder();

                embed.WithAuthor(_client.CurrentUser)
                    .WithColor(Color.Red)
                    .WithTitle("No Matches found!")
                    .WithDescription("Please enter a search term or a URL")
                    .WithCurrentTimestamp()
                    .Build();

                return embed.Build();
            }

            var track = results.Tracks.FirstOrDefault();

            if (_player.IsPlaying)
            {
                _player.Queue.Enqueue(track);
                QueueData.Add(track);
                var thumb = await track.FetchThumbnailAsync();

                var embed = new EmbedBuilder();

                embed.AddField($"Playing from {track.Provider}",
                    $"[Open Online]({track.Uri})!")
                    .WithAuthor(_client.CurrentUser)
                    .WithColor(Color.Blue)
                    .WithTitle($"Added {track.Title} to the queue")
                    .WithDescription($"Song Length: {track.Length.Hours}:{track.Length.Minutes}:{track.Length.Seconds} (H:M:S)")
                    .WithUrl(track.Uri.ToString())
                    .WithCurrentTimestamp()
                    .WithImageUrl(thumb)
                    .Build();

                return embed.Build();
            }
            else
            {
                await _player.PlayAsync(track);
                QueueData.Add(track);
                var thumb = await track.FetchThumbnailAsync();

                var embed = new EmbedBuilder();

                embed.AddField($"Playing from {track.Provider}",
                    $"[Open Online]({track.Uri})!")
                    .WithAuthor(_client.CurrentUser)
                    .WithColor(Color.Blue)
                    .WithTitle($"Now playing {track.Title}")
                    .WithDescription($"Song Length: {track.Length.Hours}:{track.Length.Minutes}:{track.Length.Seconds} (H:M:S)")
                    .WithUrl(track.Uri.ToString())
                    .WithCurrentTimestamp()
                    .WithImageUrl(thumb)
                    .Build();

                return embed.Build();
            }
        }
        public async Task<string> ClearQueue(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);

            _player.Queue.Clear();
            QueueData.Clear();
            return "Queue Cleared";
        }

        public async Task<string> StopAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null)
                return "Error with Player";
            await _player.StopAsync();
            return "Music Playback Stopped.";
        }

        public async Task<Embed> SkipAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null || _player.Queue.Items.Count() is 0) {
                var embedErr = new EmbedBuilder();

                embedErr.WithAuthor(_client.CurrentUser)
                    .WithColor(Color.Red)
                    .WithTitle($"\u2757 Nothing in the queue!")
                    .WithCurrentTimestamp()
                    .Build();

                return embedErr.Build();
            }

            var oldTrack = _player.CurrentTrack;
            await _player.SkipAsync();

            var embed = new EmbedBuilder();

            embed.WithAuthor(_client.CurrentUser)
                .WithColor(Color.Blue)
                .WithTitle($"\u23E9 Skiped: {oldTrack.Title} \nNow Playing: {_player.CurrentTrack.Title}")
                .WithDescription($"Song Length: {_player.CurrentTrack.Length.Hours}:{_player.CurrentTrack.Length.Minutes}:{_player.CurrentTrack.Length.Seconds} (H:M:S)")
                .WithCurrentTimestamp()
                .Build();

            return embed.Build();
        }

        public async Task<string> SetVolumeAsync(int vol, ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null)
                return "Player isn't playing.";

            if (vol > 150 || vol <= 2)
            {
                return "Please use a number between 2 - 150";
            }

            await _player.SetVolumeAsync(vol);
            return $"Volume set to: {vol}";
        }

        public async Task<string> PauseOrResumeAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null)
                return "Player isn't playing.";

            if (!_player.IsPaused)
            {
                await _player.PauseAsync();
                return "Player is Paused.";
            }
            else
            {
                await _player.ResumeAsync();
                return "Playback resumed.";
            }
        }

        public async Task<string> ResumeAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null)
                return "Player isn't playing.";

            if (_player.IsPaused)
            {
                await _player.ResumeAsync();
                return "Playback resumed.";
            }

            return "Player is not paused.";
        }

        public async Task<Embed> ShowQueue(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null || _player.Queue.Items.Count() is 0) {
                var embedErr = new EmbedBuilder();

                embedErr.WithAuthor(_client.CurrentUser)
                    .WithColor(Color.Red)
                    .WithTitle($"Current Queue")
                    .WithDescription($"Nothing is in the queue, run the play command to add some tracks!")
                    .WithCurrentTimestamp()
                    .Build();

                return embedErr.Build();
            }

            var embed = new EmbedBuilder();

            embed.WithAuthor(_client.CurrentUser)
                .WithColor(Color.Green)
                .WithTitle($"Current Queue")
                .WithCurrentTimestamp()
                .WithDescription($"Queue Length: {_player.Queue.Count}\n" +
                $"Queue Tracks:\n")
                .Build();

            foreach (var track in QueueData)
            {
                embed.AddField($"{track.Title}\n{track.Uri}", $"*Author:* {track.Author}\n" +
                    $"*Song Length:* {track.Length.Hours}:{track.Length.Minutes}:{track.Length.Seconds} (H:M:S)\n");
            }

            return embed.Build();
        }

        private async Task ClientReadyAsync()
        {
            await _lavaSocketClient.StartAsync(_client);
        }

        private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {
            if (!reason.ShouldPlayNext())
                return;

            if (!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await player.TextChannel.SendMessageAsync("There are no more tracks in the queue.");
                await _lavaSocketClient.DisconnectAsync(player.VoiceChannel);
                return;
            }

            await player.PlayAsync(nextTrack);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Log.Information(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}