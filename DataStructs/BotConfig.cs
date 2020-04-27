using System.Collections.Generic;

namespace NightRune.DataStructs
{
    public class BotConfig
    {
        public string DiscordToken { get; set; }
        public string DefaultPrefix { get; set; }
        public string GameStatus { get; set; }
        public List<ulong> BlacklistedChannels { get; set; }
    }
}