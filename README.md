# Exposure

Slower exposure on expeditions.

On an expedition the Exposure bar fills as time passes, faster while you run, and in jumps when you search, loot or hit an event. When it is full, zombies find you. Exposure slows each of those down, so you can stay out longer and look around more before you have to leave. It also shows you what the bar is doing: real numbers, the cost of each search, warnings as it fills, and a summary of each trip.

## Features

- **Time exposure slowed**: exposure from time spent out rises 2 times slower by default.
- **Running is free**: running adds no extra exposure by default. Walking and running cost the same.
- **Searching and looting slowed**: exposure added at once by searching, looting and events is halved by default.
- **Nothing lost to rounding**: the game only adds whole points. When a halved amount has a fraction left over, it is kept and added to the next one, so over time you get exactly the share you set.
- **Only rises are touched**: anything that lowers exposure keeps its full effect.
- **Your own numbers**: each setting can be changed in the config, from the game's own rate up to 100 times slower, with a separate setting for searching, picking up, breaking in and events.
- **Presets**: Vanilla (the game's own exposure), Relaxed (the defaults), Tourist (a fifth of the game's exposure) and Hardcore (a third faster than the game).
- **Danger**: make exposure rise faster as the days pass, or at chosen locations.
- **Harder than the game, if you want**: with `AllowFasterThanGame` on, a setting below 1 makes exposure rise faster than in the game, up to four times as fast.
- **Real numbers on the bar**: the Exposure bar shows its points (such as 62 / 100) and about how long until it is full at the current pace.
- **Cost before you act**: furniture menu buttons show how much exposure a search, open, pry or pick up will add, such as +25.
- **Warnings**: a notice and the game's alarm sound when exposure reaches 50, 75 and 90% of the bar. The percents can be set, and with Time Control the game can pause at one of them.
- **Trip summary**: once you are home, a notice with the exposure the trip added from time, running, searching and looting, and events, and what you brought home and what it is worth.

Exposure only changes how fast the bar fills. It adds nothing to your save and does not edit game files. You can remove it at any time.

Text the mod shows in game (the time to full, warnings and the trip summary) is in English, or in Simplified Chinese when the game is set to Chinese.

## Known issues and limits

- With a slowdown above 1, a small add from searching can show as 0. The fraction is not lost: it is added with the next search or loot.
- The time to full is an estimate from the last few seconds, in real time, so it changes with game speed and pauses. It can briefly disagree with the game's own bar, which redraws every 2 seconds.
- Costs are shown on furniture buttons only. Events and other jumps not tied to a button have no cost shown.
- Warning and trip summary notices need Exiled's Toolkit. Without it, they are written to the log only; the warning sound still plays.
- Pausing at a warning needs Time Control with `TimeControlOnExpeditions` on, since the game cannot pause or carry on during an expedition by itself.
- No event in the current game adds exposure, so `EventSlowdown` changes nothing for now.
- Settings changed on the Mod Manager's Mods screen take effect at once with Exiled's Toolkit installed; a change made in the file needs a restart.

## Requirements

- Survival Log 1.1.19611. Built for this version; a game update can break it until the mod is updated.
- The [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12), the BepInEx 6 (IL2CPP) build for the game.
- Optional: [Exiled's Toolkit](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit) 0.4.0 or newer. With it, settings changed on the Mod Manager's Mods screen take effect without restarting the game. Without it, they take effect at the next game start. It also shows the warning and trip summary notices.
- Optional: [Time Control](https://www.nexusmods.com/survivallog/mods/47), with `TimeControlOnExpeditions` on, to pause the game at a warning.

## Install

1. Install the [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12) (if no other mods were installed before, start the game once so BepInEx finishes setup, then quit).
2. Optional, to change settings without a restart: install Exiled's Toolkit. Download `ExiledToolkit-x.y.z.zip` from [its latest release](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/61), and extract it into the game folder (the folder with `SurvivalLog.exe`).
3. Download `Exposure-x.y.z.zip` from the [latest release](https://github.com/AnExiledDev/SurvivalLog-Exposure/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/48).
4. Extract the zip into the game folder (the folder with `SurvivalLog.exe`). It holds a `BepInEx` folder, so the DLL lands in the right place. When asked, merge the folders. The result should be:
   `C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\Exposure.dll`

## Uninstall

Delete `Exposure.dll` from the `BepInEx\plugins` folder. For a clean slate, also delete `BepInEx\config\com.dev.survivallog.exposure.cfg`.

## Keys

None. The mod has no hotkeys or buttons; it works on its own once installed.

## Configuration

Settings live in `BepInEx\config\com.dev.survivallog.exposure.cfg`, created the first time the game starts with the mod. Change them in game on the Mod Manager's Mods screen, where they take effect at once, or edit the file while the game is closed. See [CONFIG.md](CONFIG.md) for every setting. Without Exiled's Toolkit installed, changes made on the Mods screen take effect at the next game start instead.

## Troubleshooting

Check `BepInEx\LogOutput.log` for `Exposure 0.3.0 started`, then a line with your settings (for example `preset Custom: time /2, running off, loot /2, ...`), which comes again each time you change one. Below it, each `hooked ...` line names a part of the mod that is working. At the start of each expedition a line gives the location id, the day and the danger, and at the end a `trip summary` line lists the exposure from each source. A line starting with `could not hook` names the part that is off and what that means in game.

## Feedback and bug reports

Found a bug, have an idea, or something feels off? Come say so on the Core Directive Discord: https://discord.gg/trTsyxUFqg . You can also post in the Posts tab of the mod's Nexus Mods page. When reporting a bug, attach `BepInEx\LogOutput.log`.

To reach me directly, message `exiled.dev` on Discord; you can find me in the [official Survival Log Discord](https://discord.gg/MZGg4gQAUe). I'm also on X as [@AnExiledDev](https://x.com/AnExiledDev).

A Survival Log wiki with detailed documentation for all of my mods is coming soon.

## Build

This is a BepInEx 6 IL2CPP plugin. It compiles against BepInEx and the game's IL2CPP interop assemblies, so a game install with BepInEx set up and started once is required. Those assemblies come from the game and are not part of this repo. The .NET 8 SDK is required.

```
dotnet build src/Exposure -c Release -p:GameDir="C:\path\to\Survival Log"
```

`GameDir` is the game folder. The default in the project file is a local path you will almost certainly need to override. The output DLL is at `src\Exposure\bin\Release\Exposure.dll`.

The unit tests cover the pure rules (exposure, danger, presets, warnings, the trip summary and the meter) and need nothing from the game:

```
dotnet test tests/Exposure.Tests
```

## License

MIT. Copyright (c) 2026 AnExiledDev. See [LICENSE](LICENSE).
