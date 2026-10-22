Name: Exposure
Summary: Stay out longer on expeditions. The Exposure bar fills half as fast over time, running adds nothing, and searches add half, with real numbers on the bar, the cost of each search, warnings at 50, 75 and 90% and a trip summary. Presets from Vanilla to Hardcore, a setting for each kind of exposure, and danger that grows by day or location.

Description (BBCode):
Always running out of time on an expedition before you have seen half the map? Exposure slows down how fast the Exposure bar fills, so you can stay out longer and search more before the zombies find you.

[size=5]Main features[/size]
[list]
[*][b]Time exposure slowed[/b]: exposure from time spent out rises 2 times slower by default.[/*]
[*][b]Running is free[/b]: running adds no extra exposure by default, so it costs the same as walking.[/*]
[*][b]Searching and looting slowed[/b]: exposure added at once by searching, looting and events is halved by default.[/*]
[*][b]Nothing lost to rounding[/b]: when a halved amount leaves a fraction, it is kept and added with the next one.[/*]
[*][b]Only rises are touched[/b]: anything that lowers exposure keeps its full effect.[/*]
[*][b]Your own numbers[/b]: every setting can be changed in the config, from the game's own rate up to 100 times slower, with a separate setting for searching, picking up, breaking in and events.[/*]
[*][b]Presets[/b]: Vanilla (the game's own exposure), Relaxed (the defaults), Tourist (a fifth of the game's exposure) and Hardcore (a third faster than the game).[/*]
[*][b]Danger[/b]: make exposure rise faster as the days pass, or at chosen locations.[/*]
[*][b]Harder than the game, if you want[/b]: with AllowFasterThanGame on, a setting below 1 makes exposure rise faster than in the game, up to four times as fast.[/*]
[*][b]Real numbers on the bar[/b]: the Exposure bar shows its points (such as 62 / 100) and about how long until it is full at the current pace.[/*]
[*][b]Cost before you act[/b]: furniture menu buttons show how much exposure a search, open, pry or pick up will add, such as +25.[/*]
[*][b]Warnings[/b]: a notice and the game's alarm sound when exposure reaches 50, 75 and 90% of the bar. The percents can be set, and with Time Control the game can pause at one of them.[/*]
[*][b]Trip summary[/b]: once you are home, a notice with the exposure the trip added from each source, and what you brought home and what it is worth.[/*]
[/list]
Exposure only changes how fast the bar fills. It adds nothing to your save and does not edit game files. You can remove it at any time.

Text the mod shows in game is in English, or in Simplified Chinese when the game is set to Chinese.

[size=5]Known issues[/size]
[list]
[*]With a slowdown above 1, a small add from searching can show as 0; the fraction is added with the next one.[/*]
[*]The time to full is an estimate in real time, so it changes with game speed and pauses.[/*]
[*]Warning and trip summary notices need Exiled's Toolkit. Without it they are written to the log only; the warning sound still plays.[/*]
[*]Pausing at a warning needs Time Control with TimeControlOnExpeditions on, since the game cannot pause during an expedition by itself.[/*]
[*]No event in the current game adds exposure, so EventSlowdown changes nothing for now.[/*]
[/list]

[size=5]Requirements[/size]
[list]
[*]Survival Log 1.1.19131. A game update can break it until the mod is updated.[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]. Install it first.[/*]
[*]Optional: [url=https://www.nexusmods.com/survivallog/mods/61]Exiled's Toolkit[/url] 0.4.0 or newer. With it, settings changed on the Mod Manager's Mods screen take effect without restarting the game. Without it, they take effect at the next game start. It also shows the warning and trip summary notices.[/*]
[*]Optional: [url=https://www.nexusmods.com/survivallog/mods/47]Time Control[/url], with TimeControlOnExpeditions on, to pause the game at a warning.[/*]
[/list]

[size=5]Installation[/size]
[list=1]
[*]Install [url=https://www.nexusmods.com/survivallog/mods/12]BepInEx[/url] [i](if no other mods were installed before, start the game once so BepInEx finishes setup, then quit)[/i].[/*]
[*]Optional, to change settings without a restart: install Exiled's Toolkit the same way. Download its zip and extract it into the game folder.[/*]
[*]Download the zip from the Files tab.[/*]
[*]Extract it into the game folder (the folder with [font=Courier New]SurvivalLog.exe[/font]). The zip holds a BepInEx folder, so the DLL lands in the right place. When asked, merge the folders. The result should be:
[code]C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\Exposure.dll[/code][/*]
[/list]

[size=5]Uninstall[/size]
Delete [font=Courier New]Exposure.dll[/font] from the [font=Courier New]BepInEx\plugins[/font] folder.

[size=5]Configuration[/size]
To change settings, edit [font=Courier New]BepInEx\config\com.dev.survivallog.exposure.cfg[/font] while the game is closed, or do it in game on the Mod Manager's Mods screen, where changes take effect at once (see [url=https://github.com/AnExiledDev/SurvivalLog-Exposure/blob/main/CONFIG.md]CONFIG.md[/url] for every setting). Without Exiled's Toolkit installed, changes made on the Mods screen take effect at the next game start instead.
[list]
[*][font=Courier New]TimeSlowdown[/font] (default 2): how many times slower exposure from time rises.[/*]
[*][font=Courier New]RunningAddsExposure[/font] (default false): set to true to let running add exposure as in the game.[/*]
[*][font=Courier New]LootSlowdown[/font] (default 2): exposure from searching, picking up, breaking in and events is divided by this.[/*]
[*][font=Courier New]SearchSlowdown[/font], [font=Courier New]PickupSlowdown[/font], [font=Courier New]BreakInSlowdown[/font], [font=Courier New]EventSlowdown[/font] (default 0): a separate slowdown for each kind; 0 follows LootSlowdown.[/*]
[*][font=Courier New]Preset[/font] (default Custom): Vanilla, Relaxed, Tourist or Hardcore replace every other exposure and danger setting.[/*]
[*][font=Courier New]AllowFasterThanGame[/font] (default false): lets a setting below 1 make exposure rise faster than in the game.[/*]
[*]Danger: [font=Courier New]StartMultiplier[/font], [font=Courier New]EndMultiplier[/font], [font=Courier New]RampDays[/font] and [font=Courier New]Locations[/font] make exposure rise faster by day or location.[/*]
[*]Meter: [font=Courier New]ShowNumbers[/font], [font=Courier New]ShowTimeToFull[/font] and [font=Courier New]ShowCosts[/font] (all on by default).[/*]
[*]Warnings: [font=Courier New]Thresholds[/font] (default 50,75,90), [font=Courier New]Toast[/font], [font=Courier New]Sound[/font] and [font=Courier New]PauseAt[/font] (default 0, off).[/*]
[*]Trip: [font=Courier New]ShowSummary[/font] (default on).[/*]
[/list]

[size=5]Feedback[/size]
Found a bug or have an idea? Come say hi on the [url=https://discord.gg/trTsyxUFqg]Core Directive Discord[/url], or post in this page's Posts tab. For bugs, please include [font=Courier New]BepInEx\LogOutput.log[/font].
To reach me directly, message [b]exiled.dev[/b] on Discord; you can find me in the [url=https://discord.gg/MZGg4gQAUe]official Survival Log Discord[/url]. I'm also on X as [url=https://x.com/AnExiledDev]@AnExiledDev[/url].
Coming soon: a Survival Log wiki with detailed documentation for all of my mods.

[size=5]Source and license[/size]
Open source under MIT: [url=https://github.com/AnExiledDev/SurvivalLog-Exposure]github.com/AnExiledDev/SurvivalLog-Exposure[/url]

[line]
[size=5]简体中文[/size]

探索时还没逛完半张地图，暴露值就快满了？Exposure 会让暴露值涨得更慢，让你在被丧尸发现之前，能在外面待得更久、翻找更多地方。

[size=5]主要功能[/size]
[list]
[*][b]随时间上涨变慢[/b]：探索时随时间上涨的暴露值，默认只有原来的一半速度。[/*]
[*][b]跑步不额外增加[/b]：默认情况下跑步不会额外增加暴露值，跑和走的消耗一样。[/*]
[*][b]翻找和拾取减半[/b]：翻找、拾取和事件一次性增加的暴露值默认减半。[/*]
[*][b]零头不会丢[/b]：减半后剩下的小数部分会保留下来，加到下一次里。[/*]
[*][b]只减缓上涨[/b]：所有降低暴露值的效果都保持原样。[/*]
[*][b]数值可调[/b]：每项设置都能在配置中修改，从游戏原速到慢 100 倍都可以，翻找、拾取、撬开和事件各有单独设置。[/*]
[*][b]预设[/b]：Vanilla（游戏原本的暴露值）、Relaxed（默认设置）、Tourist（只有游戏的五分之一）和 Hardcore（比游戏快三分之一）。[/*]
[*][b]危险度[/b]：让暴露值随天数增加或在指定地点上涨更快。[/*]
[*][b]也可以比游戏更难[/b]：开启 AllowFasterThanGame 后，低于 1 的数值会让暴露值比游戏上涨更快，最快四倍速。[/*]
[*][b]暴露值显示数字[/b]：暴露值条显示具体点数（例如 62 / 100），以及按当前速度大约多久后涨满。[/*]
[*][b]行动前显示消耗[/b]：家具菜单的按钮上会显示翻找、开启、撬开或拾取会增加多少暴露值，例如 +25。[/*]
[*][b]提醒[/b]：暴露值达到上限的 50%、75% 和 90% 时显示通知并播放游戏的警报声。百分比可以修改，装了 Time Control 还能在其中一个百分比暂停游戏。[/*]
[*][b]探索总结[/b]：回到家后显示一条通知，列出这次探索中各来源增加的暴露值，以及带回家的物品和价值。[/*]
[/list]
Exposure 只改变暴露值上涨的快慢，不会往存档里添加任何内容，也不会修改游戏文件，随时可以移除。

模组在游戏中显示的文字为英文；游戏设为中文时显示简体中文。

[size=5]已知问题[/size]
[list]
[*]慢速倍数大于 1 时，一次很小的翻找可能显示为增加 0，剩下的小数会在下一次一起加上。[/*]
[*]涨满时间是按现实时间估算的，会随游戏速度和暂停变化。[/*]
[*]提醒和探索总结的通知需要 Exiled's Toolkit。没装的话只写入日志，警报声仍会播放。[/*]
[*]在提醒时暂停需要安装 Time Control 并开启 TimeControlOnExpeditions，因为游戏本身在探索中无法暂停。[/*]
[*]目前游戏中没有会增加暴露值的事件，所以 EventSlowdown 暂时没有效果。[/*]
[/list]

[size=5]前置要求[/size]
[list]
[*]Survival Log 1.1.19131。游戏更新后模组可能暂时失效，直到模组更新为止。[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]，请先安装。[/*]
[*]可选：[url=https://www.nexusmods.com/survivallog/mods/61]Exiled's Toolkit[/url] 0.4.0 或更新版本。装了它，在 Mod Manager 的「模组」界面改动的设置无需重启游戏即可生效；没装的话，改动在下次启动游戏时生效。提醒和探索总结的通知也需要它。[/*]
[*]可选：[url=https://www.nexusmods.com/survivallog/mods/47]Time Control[/url]，开启 TimeControlOnExpeditions 后可以在提醒时暂停游戏。[/*]
[/list]

[size=5]安装[/size]
[list=1]
[*]安装 [url=https://www.nexusmods.com/survivallog/mods/12]BepInEx[/url] [i]（如果之前没有装过其他模组，先启动一次游戏，让 BepInEx 完成初始化，然后退出）[/i]。[/*]
[*]可选（改动设置无需重启游戏）：用同样的方式安装 Exiled's Toolkit，下载它的压缩包并解压到游戏目录。[/*]
[*]在「文件」页下载压缩包。[/*]
[*]把压缩包解压到游戏目录（[font=Courier New]SurvivalLog.exe[/font] 所在的文件夹）。压缩包里带有 BepInEx 文件夹，DLL 会自动放到正确位置。如果提示合并文件夹，选择合并。解压后应为：
[code]C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\Exposure.dll[/code][/*]
[/list]

[size=5]卸载[/size]
从 [font=Courier New]BepInEx\plugins[/font] 文件夹中删除 [font=Courier New]Exposure.dll[/font]。

[size=5]配置[/size]
如需修改设置，可以在游戏内 Mod Manager 的「模组」界面修改（改动立即生效），也可以在游戏关闭时编辑 [font=Courier New]BepInEx\config\com.dev.survivallog.exposure.cfg[/font]。配置文件中每项设置都有中文说明，完整说明见 [url=https://github.com/AnExiledDev/SurvivalLog-Exposure/blob/main/CONFIG.md]CONFIG.md[/url]（英文）。没装 Exiled's Toolkit 的话，在「模组」界面的改动改为在下次启动游戏时生效。
[list]
[*][font=Courier New]TimeSlowdown[/font]（默认 2）：随时间上涨的暴露值慢几倍。[/*]
[*][font=Courier New]RunningAddsExposure[/font]（默认 false）：设为 true 后，跑步会和游戏原本一样增加暴露值。[/*]
[*][font=Courier New]LootSlowdown[/font]（默认 2）：翻找、拾取、撬开和事件增加的暴露值会除以这个数。[/*]
[*][font=Courier New]SearchSlowdown[/font]、[font=Courier New]PickupSlowdown[/font]、[font=Courier New]BreakInSlowdown[/font]、[font=Courier New]EventSlowdown[/font]（默认 0）：每类单独的慢速倍数，0 表示使用 LootSlowdown。[/*]
[*][font=Courier New]Preset[/font]（默认 Custom）：选择 Vanilla、Relaxed、Tourist 或 Hardcore 会替换其他所有暴露值和危险度设置。[/*]
[*][font=Courier New]AllowFasterThanGame[/font]（默认 false）：允许低于 1 的数值让暴露值比游戏上涨更快。[/*]
[*]危险度：[font=Courier New]StartMultiplier[/font]、[font=Courier New]EndMultiplier[/font]、[font=Courier New]RampDays[/font] 和 [font=Courier New]Locations[/font] 让暴露值随天数或地点上涨更快。[/*]
[*]显示：[font=Courier New]ShowNumbers[/font]、[font=Courier New]ShowTimeToFull[/font] 和 [font=Courier New]ShowCosts[/font]（默认全部开启）。[/*]
[*]提醒：[font=Courier New]Thresholds[/font]（默认 50,75,90）、[font=Courier New]Toast[/font]、[font=Courier New]Sound[/font] 和 [font=Courier New]PauseAt[/font]（默认 0，关闭）。[/*]
[*]探索总结：[font=Courier New]ShowSummary[/font]（默认开启）。[/*]
[/list]

[size=5]反馈[/size]
发现 bug 或有好点子？欢迎来 [url=https://discord.gg/trTsyxUFqg]Core Directive Discord[/url] 打个招呼，或在本页的「Posts」标签里发帖。报告 bug 时，请附上 [font=Courier New]BepInEx\LogOutput.log[/font]。
想直接联系我，可以在 Discord 上给 [b]exiled.dev[/b] 发消息，我在 [url=https://discord.gg/MZGg4gQAUe]Survival Log 官方 Discord[/url] 里。也可以在 X 上找我：[url=https://x.com/AnExiledDev]@AnExiledDev[/url]。
即将推出：Survival Log 维基，收录我所有模组的详细说明。

[size=5]源代码与许可[/size]
基于 MIT 许可开源：[url=https://github.com/AnExiledDev/SurvivalLog-Exposure]github.com/AnExiledDev/SurvivalLog-Exposure[/url]
