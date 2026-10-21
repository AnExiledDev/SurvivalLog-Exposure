# Exposure

Slower exposure on expeditions.

On an expedition the Exposure bar fills as time passes, faster while you run, and in jumps when you search, loot or hit an event. When it is full, zombies find you. Exposure slows each of those down, so you can stay out longer and look around more before you have to leave.

## Features

- **Time exposure slowed**: exposure from time spent out rises 2 times slower by default.
- **Running is free**: running adds no extra exposure by default. Walking and running cost the same.
- **Searching and looting slowed**: exposure added at once by searching, looting and events is halved by default.
- **Nothing lost to rounding**: the game only adds whole points. When a halved amount has a fraction left over, it is kept and added to the next one, so over time you get exactly the share you set.
- **Only rises are touched**: anything that lowers exposure keeps its full effect.
- **Your own numbers**: each setting can be changed in the config, from the game's own rate up to 100 times slower.

Exposure only changes how fast the bar fills. It adds nothing to your save and does not edit game files. You can remove it at any time.

The mod shows no text of its own in game, so it works the same in English and Chinese.

## Known issues and limits

- With a slowdown above 1, a small add from searching can show as 0. The fraction is not lost: it is added with the next search or loot.
- Settings changed on the Mod Manager's Mods screen take effect at once with Exiled's Toolkit installed; a change made in the file needs a restart.

## Requirements

- Survival Log 1.1.19131. Built for this version; a game update can break it until the mod is updated.
- The [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12), the BepInEx 6 (IL2CPP) build for the game.
- Optional: [Exiled's Toolkit](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit) 0.4.0 or newer. With it, settings changed on the Mod Manager's Mods screen take effect without restarting the game. Without it, they take effect at the next game start.

## Install

1. Install the [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12) (if no other mods were installed before, start the game once so BepInEx finishes setup, then quit).
2. Optional, to change settings without a restart: install Exiled's Toolkit. Download `ExiledToolkit-x.y.z.zip` from [its latest release](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit/releases/latest) or from Nexus Mods, and extract it into the game folder (the folder with `SurvivalLog.exe`).
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

Check `BepInEx\LogOutput.log` for `Exposure 0.2.0 started`, then a line with your settings (for example `time /2, running off, loot /2`), which comes again each time you change one. Below it, `hooked ExploreManager.Update` and `hooked ExploreManager.AddExposure` mean both parts of the mod are working. A line starting with `could not hook` names the part that is off and what that means in game.

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

The unit tests cover the pure exposure rules and need nothing from the game:

```
dotnet test tests/Exposure.Tests
```

## License

MIT. Copyright (c) 2026 AnExiledDev. See [LICENSE](LICENSE).
