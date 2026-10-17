# Exposure settings

Settings live in `BepInEx\config\com.dev.survivallog.exposure.cfg`, created the first time the game starts with the mod. Edit the file while the game is closed; changes apply at the next game start. Delete the file to get the defaults back.

## Exposure

| Setting | Default | What it does |
| --- | --- | --- |
| `TimeSlowdown` | `2` | On an expedition, exposure from time spent out rises this many times slower. `1` is the game's rate, `2` is half as fast. From 1 to 100. |
| `RunningAddsExposure` | `false` | When `false`, running on an expedition adds no exposure on top of time, so running costs the same as walking. When `true`, running adds exposure as in the game. |
| `LootSlowdown` | `2` | Exposure added at once by searching, looting and events is divided by this. `1` is the game's amount, `2` is half. From 1 to 100. |

`TimeSlowdown` and `LootSlowdown` take whole or decimal numbers (such as `1.5`). A value below 1 counts as 1 and a value above 100 counts as 100, so the mod never makes exposure rise faster than the game does.

The game only adds whole points of exposure. When `LootSlowdown` leaves a fraction (5 divided by 2 is 2.5), the whole part is added now and the fraction is kept for the next add, so a search can show 2 and the next one 3.

None of the settings change anything that lowers exposure; those keep the game's full effect.
