<img src="https://raw.githubusercontent.com/nlaha/Night-Rune/master/Art/Icon.png" alt="NightRune" width="200"/></img>
# Night-Rune
[![Total alerts](https://img.shields.io/lgtm/alerts/g/nlaha/Night-Rune.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/nlaha/Night-Rune/alerts/)
[![Build Status](https://travis-ci.org/nlaha/Night-Rune.svg?branch=master)](https://travis-ci.org/nlaha/Night-Rune)

A speedy discord bot for your server

### Publish
    dotnet publish -c Release -r <(windows/linux)-x64> -o <my output directory>
   
### Hosting
There must be a Lavalink server present on localhost:2333
and a config.json with the bot token next to the executable

    {
      "DiscordToken": "mytoken",
      "DefaultPrefix": "!",
      "GameStatus": "Playing Music",
      "BlacklistedChannels": []
    }
