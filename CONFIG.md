# Exposure settings

Settings live in `BepInEx\config\com.dev.survivallog.exposure.cfg`, created the first time the game starts with the mod. Change them in game on the Mod Manager's Mods screen, where they take effect at once, or edit the file while the game is closed. Delete the file to get the defaults back. Each world can keep its own Exposure and Danger settings (all but `Locations`). Pick them for a new world in Customizable Difficulty, or change them for the world you're playing on the Mods screen.

## Exposure

| Setting | Default | What it does |
| --- | --- | --- |
| `Preset` | `Custom` | A ready-made set of settings: `Custom`, `Default`, `Relaxed`, `Tourist` or `Hardcore` (see below). Any preset other than `Custom` replaces every other setting in Exposure and Danger. |
| `TimeSlowdown` | `1` | On an expedition, exposure from time spent out rises this many times slower. `1` is the game's rate, `2` is half as fast. |
| `RunningAddsExposure` | `true` | When `false`, running on an expedition adds no exposure on top of time, so running costs the same as walking. When `true`, running adds exposure like it normally does. |
| `LootSlowdown` | `1` | Exposure added at once by searching, picking up, breaking in and events is divided by this, unless that kind has its own setting below. `1` is the game's amount, `2` is half. |
| `SearchSlowdown` | `0` | Exposure added by searching and looking over furniture is divided by this. `0` uses `LootSlowdown`. |
| `PickupSlowdown` | `0` | Exposure added by picking up and taking out supplies is divided by this. `0` uses `LootSlowdown`. |
| `BreakInSlowdown` | `0` | Exposure added by prying and forcing doors and locks, unlocking, tearing down and swiping cards is divided by this. `0` uses `LootSlowdown`. |
| `EventSlowdown` | `0` | Exposure added by events and story rewards on an expedition is divided by this. `0` uses `LootSlowdown`. No event in the current game adds exposure, so for now this changes nothing. |
| `AllowFasterThanGame` | `false` | When `true`, a slowdown below 1 makes exposure rise faster than in the game, down to `0.25`, which is four times as fast. When `false`, no setting makes exposure rise faster than in the game. |

The slowdowns take whole or decimal numbers (such as `1.5`) from 1 to 100, or from 0.25 with `AllowFasterThanGame` on. A value past either end counts as that end.

The game only adds whole points of exposure. When a slowdown leaves a fraction (5 divided by 2 is 2.5), the whole part is added now and the fraction is kept for the next add, so a search can show 2 and the next one 3.

The settings only change how fast exposure rises. Anything that lowers exposure still lowers it by the full amount.

### Presets

| Preset | Time | Running | Searching, picking up, breaking in, events |
| --- | --- | --- | --- |
| `Default` | normal rate | adds exposure | normal amount |
| `Relaxed` | half as fast | adds none | half |
| `Tourist` | a fifth as fast | adds none | a fifth |
| `Hardcore` | a third faster than the game | adds exposure at the game's rate | a third more than the game |

The default `Custom` settings are the same as `Default`. Presets ignore the Danger settings.

## Danger

Danger makes exposure rise faster on top of the Exposure settings. It is used with the `Custom` preset only. Without `AllowFasterThanGame`, danger can bring exposure back up to the game's rate but never past it.

| Setting | Default | What it does |
| --- | --- | --- |
| `StartMultiplier` | `1` | How many times faster exposure rises on day 1. `1` changes nothing, `2` is twice as fast, `0.5` is half as fast. From 0.1 to 10. |
| `EndMultiplier` | `1` | How many times faster exposure rises once day `RampDays` is reached. Before that day, the multiplier climbs evenly from `StartMultiplier` on day 1. From 0.1 to 10. |
| `RampDays` | `30` | The day on which `EndMultiplier` is reached. `1` uses `EndMultiplier` from the first day. |
| `Locations` | empty | Extra multipliers for single locations, written as `id=multiplier` and separated by commas, such as `1002=1.5, 1005=2`. Ids: 1000 Hardware Store, 1002 Abandoned School, 1003 Tech Company, 1004 Deserted Supermarket, 1005 Abandoned Hospital. Applied on top of the day multiplier. |

The log shows the location id and day at the start of each expedition (`expedition at location 1002 on day 4, danger x1.2`).

## Meter

| Setting | Default | What it does |
| --- | --- | --- |
| `ShowNumbers` | `true` | Show the exposure bar's points as numbers, such as `62 / 100`, in place of the percent. |
| `ShowTimeToFull` | `true` | Under the bar, show about how long until it is full at the current pace, in real time. Hidden while the bar is not rising. |
| `ShowCosts` | `true` | On an expedition, show how much exposure each search, open, pry or pick up adds, such as `+25`, on its button in the furniture menu. |

## Warnings

| Setting | Default | What it does |
| --- | --- | --- |
| `Thresholds` | `50,75,90` | Warn when exposure first reaches each of these percents of the bar. Whole numbers from 1 to 100, separated by commas. Each warns once per trip. Leave empty for no warnings. |
| `Toast` | `true` | Show a notice when a warning percent is reached. Every warning is also written to the log. |
| `Sound` | `true` | Play the game's exposure alarm, the zombie roar, when a warning percent is reached. |
| `PauseAt` | `0` | Pause the game when exposure first reaches this percent. `0` is off. The game can't pause or unpause during an expedition by itself, so this needs Time Control with `TimeControlOnExpeditions` on. Without it the game doesn't pause, and the warning tells you why. |

## Trip

| Setting | Default | What it does |
| --- | --- | --- |
| `ShowSummary` | `true` | Once you are home from an expedition, show a notice with the exposure the trip added from each source, and how many items you brought home and what they are worth. The summary is always written to the log. |
