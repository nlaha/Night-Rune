using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.EventArgs;
using Victoria.Enums;
using Victoria.Responses.Rest;

namespace NightRune.Handlers
{
    public static class EmbedHandler
    {
        /* This file is where we can store all the Embed Helper Tasks (So to speak). 
         We wrap all the creations of new EmbedBuilder's in a Task.Run to allow us to stick with Async calls. 
         All the Tasks here are also static which means we can call them from anywhere in our program. */
        public static async Task<Discord.Embed> CreateBasicEmbed(string title, string description, Color color)
        {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Discord.Embed> CreateLyricEmbed(string title, string description, Color color)
        {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Discord.Embed> CreateSongQueueEmbed(LavaTrack song)
        {
            var artwork = await song.FetchArtworkAsync();
            Color color = Color.Blue;

            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle("Added: " + song.Title)
                .WithDescription("Author: " + song.Author + "\nDuration: " + song.Duration.ToString("c") + " (HH:MM:SS)")
                .WithUrl(song.Url)
                .WithImageUrl(artwork)
                .WithColor(color)
                .WithAuthor("[NightRune] Music Bot")
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Discord.Embed> CreateSongPlayEmbed(LavaTrack song)
        {
            var artwork = await song.FetchArtworkAsync();
            Color color = Color.Blue;

            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle("Now Playing: " + song.Title)
                .WithDescription("Author: " + song.Author + "\nDuration: " + song.Duration.ToString("c") + " (HH:MM:SS)")
                .WithUrl(song.Url)
                .WithImageUrl(artwork)
                .WithColor(color)
                .WithAuthor("[NightRune] Music Bot")
                .WithCurrentTimestamp().Build()));
            return embed;
        }

        public static async Task<Embed> CreateErrorEmbed(string source, string error)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($"ERROR OCCURED FROM - {source}")
                .WithDescription($"**Error Deaitls**: \n{error}")
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp().Build());
            return embed;
        }
    }
}
