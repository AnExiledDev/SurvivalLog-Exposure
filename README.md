# Exposure

Stay out longer on expeditions by slowing down the Exposure bar.

A fresh install keeps the normal exposure, so the bar fills as fast as it always did until you pick the `Relaxed` or `Tourist` preset or set the slowdowns yourself. The numbers on the bar, the search costs, the warnings and the trip summary work from the start.

On an expedition the Exposure bar fills as time passes, faster while you run, and in jumps when you search, loot or hit an event. When it's full, zombies find you. Exposure lets you slow each of those down, so you can stay out longer and look around more before you have to leave. It also shows you what the bar is doing: real numbers, the cost of each search, warnings as it fills, and a summary of each trip.

## Features

- **Slower exposure over time**: you set how many times slower exposure rises while you're out (`TimeSlowdown`).
- **No exposure from running**: turn `RunningAddsExposure` off and running costs the same as walking.
- **Slower exposure from searching and looting**: the exposure added at once by searching, looting and events is divided by the number you set (`LootSlowdown`).
- **Nothing lost to rounding**: the game only adds whole points. When a slowed amount leaves a fraction, the fraction is kept and added to the next one, so over time you get exactly the share you set.
- **Only rises change**: anything that lowers exposure works the same as before.
- **A setting for each kind**: every slowdown goes from the game's rate up to 100 times slower, and searching, picking up, breaking in and events can each have their own.
- **Presets**: Default (the normal exposure), Relaxed (half the exposure, running adds none), Tourist (a fifth of the exposure, running adds none) and Hardcore (a third faster than normal).
- **Danger**: make exposure rise faster as the days pass, or at locations you pick.
- **Harder than normal, if you want**: with `AllowFasterThanGame` on, a setting below 1 makes exposure rise faster than normal, up to four times as fast.
- **Real numbers on the bar**: the Exposure bar shows its points (such as 62 / 100) and about how long until it's full at the current pace.
- **Costs on the buttons**: furniture menu buttons show how much exposure a search, open, pry or pick up will add, such as +25.
- **Warnings**: a notice and the game's alarm sound when exposure reaches 50, 75 and 90% of the bar. You can change the percents, and with Time Control the game can pause at one of them.
- **Trip summary**: once you're home, a notice shows the exposure the trip added from time, running, searching and looting, and events, plus what you brought home and what it's worth.

Exposure only changes how fast the bar fills and what it shows. It adds nothing to your save and doesn't edit game files, so you can remove it at any time.

Text the mod shows in game (the time to full, warnings and the trip summary) is in English, or in Simplified Chinese when the game is set to Chinese.

## Known issues and limits

- With a slowdown above 1, a small add from searching can show as 0. The fraction isn't lost. It's added with the next search or loot.
- The time to full is an estimate from the last few seconds, in real time, so it changes with game speed and pauses. It can briefly disagree with the game's bar, which redraws every 2 seconds.
- Costs only show on furniture buttons. Events and other jumps that don't come from a button show no cost.
- Pausing at a warning needs Time Control with `TimeControlOnExpeditions` on, because the game can't pause or unpause during an expedition by itself.
- No event in the current game adds exposure, so `EventSlowdown` does nothing for now.
- Settings changed on the Mod Manager's Mods screen take effect at once. A change made in the file needs a restart.

## Requirements

- Survival Log 1.1.20719. Made for this version, so a game update can break it until the mod is updated.
- The [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12), the BepInEx 6 (IL2CPP) build for the game.
- [Exiled's Toolkit](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit) 0.19.0 or newer. Exposure doesn't load without it. It shows the warning and trip summary notices as toasts, and it applies settings changed on the Mod Manager's Mods screen without a restart.
- Optional: [Time Control](https://www.nexusmods.com/survivallog/mods/47), with `TimeControlOnExpeditions` on, to pause the game at a warning.
- Optional: [Mod Manager](https://www.nexusmods.com/survivallog/mods/73). With it, you can change every setting in game on its Mods screen and see what each one does, without opening the config file.

## Install

1. Install the [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12) (if no other mods were installed before, start the game once so BepInEx finishes setup, then quit).
2. Install Exiled's Toolkit. Download `ExiledToolkit-x.y.z.zip` from [its latest release](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/61), and extract it into the game folder (the folder with `SurvivalLog.exe`).
3. Download `Exposure-x.y.z.zip` from the [latest release](https://github.com/AnExiledDev/SurvivalLog-Exposure/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/48).
4. Extract the zip into the game folder (the folder with `SurvivalLog.exe`). It holds a `BepInEx` folder, so the DLL lands in the right place. When asked, merge the folders. The result should be:
   `C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\Exposure.dll`

## Uninstall

Delete `Exposure.dll` from the `BepInEx\plugins` folder. For a clean slate, also delete `BepInEx\config\com.dev.survivallog.exposure.cfg`.

## Keys

None. The mod has no hotkeys or buttons, and it works on its own once installed.

## Configuration

Settings live in `BepInEx\config\com.dev.survivallog.exposure.cfg`, created the first time the game starts with the mod. Change them in game on the Mod Manager's Mods screen, where they take effect at once, or edit the file while the game is closed. See [CONFIG.md](CONFIG.md) for every setting. Each world can keep its own Exposure and Danger settings (all but `Locations`). Pick them for a new world in Customizable Difficulty, or change them for the world you're playing on the Mods screen.

## Troubleshooting

Check `BepInEx\LogOutput.log` for `Exposure 0.4.4 started` (with your version), then a line with your settings (for example `preset Custom: time /2, running off, loot /2, ...`), which shows up again each time you change one. Below it, each `hooked ...` line names a part of the mod that is working. At the start of each expedition a line gives the location id, the day and the danger, and at the end a `trip summary` line lists the exposure from each source. A line starting with `could not hook` names the part that is off and what that means in game.

## Feedback and bug reports

Found a bug, have an idea, or something feels off? Come tell me on the Core Directive Discord: https://discord.gg/trTsyxUFqg . You can also post in the Posts tab of the mod's Nexus Mods page. When reporting a bug, attach `BepInEx\LogOutput.log`.

To reach me directly, message `exiled.dev` on Discord. You can find me in the [official Survival Log Discord](https://discord.gg/MZGg4gQAUe). I'm also on X as [@AnExiledDev](https://x.com/AnExiledDev).

A Survival Log wiki with detailed documentation for all of my mods is coming soon.

## Build

This is a BepInEx 6 IL2CPP plugin. It compiles against BepInEx and the game's IL2CPP interop assemblies, so you need a game install with BepInEx set up and started once. Those assemblies come from the game and aren't part of this repo. You also need the .NET 8 SDK.

```
dotnet build src/Exposure -c Release -p:GameDir="C:\path\to\Survival Log"
```

`GameDir` is the game folder. The default in the project file is a local path you'll almost certainly need to override. The output DLL is at `src\Exposure\bin\Release\Exposure.dll`.

The unit tests cover the pure rules (exposure, danger, presets, warnings, the trip summary and the meter) and need nothing from the game:

```
dotnet test tests/Exposure.Tests
```

## License

MIT. Copyright (c) 2026 AnExiledDev. See [LICENSE](LICENSE).
