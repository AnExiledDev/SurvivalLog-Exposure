# Changelog

## 0.4.2

- A new install now starts at the game's exposure: `TimeSlowdown` 1, `RunningAddsExposure` true and `LootSlowdown` 1. Pick the `Relaxed` preset for the old defaults (half as fast, running adds none). Settings you already saved are kept.
- Made for Survival Log 1.1.20719.

## 0.4.1

- Now uses Exiled's Toolkit for its shared parts, so it needs Exiled's Toolkit 0.19.0 or newer.
- Made for Survival Log 1.1.20719.

## 0.4.0

- Each world can keep its own exposure settings: `Preset` and every setting in Exposure and Danger except `Locations`. With Exiled's Toolkit 0.17.0 or newer, pick them for a new world in Customizable Difficulty, or change them for the world you are playing on the Mod Manager's Mods screen. `Locations`, the meter, warnings and trip summary stay the same for every world.
- Made for Survival Log 1.1.20151.

## 0.3.1

- **Warning:** needs Exiled's Toolkit 0.11.0 or newer. Update the Toolkit first.
- Exiled's Toolkit now shows the warning and trip summary notices.
- Made for Survival Log 1.1.20151.

## 0.3.0

- The Exposure bar shows its points as numbers (such as 62 / 100) and about how long until it is full at the current pace.
- Furniture menu buttons show how much exposure a search, open, pry or pick up will add, such as +25.
- Warnings when exposure reaches 50, 75 and 90% of the bar, with the game's alarm sound and a notice. The percents can be set, and the game can pause at one of them when Time Control is installed with its expedition controls on.
- A trip summary once you are home: the exposure the trip added from each source, and how many items you brought home and what they are worth.
- Separate settings for searching, picking up, breaking in and events. Each one left at 0 follows `LootSlowdown`.
- Danger settings make exposure rise faster as the days pass, or at chosen locations.
- `AllowFasterThanGame` lets a setting below 1 make exposure rise faster than in the game, up to four times as fast.
- Presets: Vanilla, Relaxed, Tourist and Hardcore. The default, Custom, keeps your own settings.
- With the default settings, exposure rises exactly as in 0.2.1.
- Made for Survival Log 1.1.19131.

## 0.2.1

- After a game update, Mod Manager can tell you if this mod may need an update.
- Made for Survival Log 1.1.19131.

## 0.2.0

- Mod Manager's Mods screen links to this changelog.
- Settings changed on the Mod Manager's Mods screen take effect at once.
- Mod Manager's Mods screen shows the mod's name, description, category and links.
- Exiled's Toolkit 0.4.0 or newer is optional. With it, settings you change on the Mods screen take effect right away. Without it, they take effect the next time you start the game.

## 0.1.2

- First release.
- Exposure rises half as fast over time on expeditions.
- Running adds no exposure.
- Searching and looting add half as much exposure.
- Every amount can be set in the config.
