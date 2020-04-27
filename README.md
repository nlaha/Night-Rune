<img src="https://raw.githubusercontent.com/nlaha/Night-Rune/master/Art/Icon.png" alt="NightRune" width="200"/></img>
# Night-Rune
[![Total alerts](https://img.shields.io/lgtm/alerts/g/nlaha/Night-Rune.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/nlaha/Night-Rune/alerts/)
[![Build Status](https://travis-ci.org/nlaha/Night-Rune.svg?branch=master)](https://travis-ci.org/nlaha/Night-Rune)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/68b44dbe12284da5993a30c6f7c93465)](https://www.codacy.com/manual/TheDekuTree/Night-Rune?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=nlaha/Night-Rune&amp;utm_campaign=Badge_Grade)

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
