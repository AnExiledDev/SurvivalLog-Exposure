Name: Exposure
Summary: Slower exposure on expeditions: time exposure rises half as fast, running adds none, and searching and looting add half. Every amount can be set. 探索时暴露值上涨更慢：随时间上涨减半，跑步不再额外增加，翻找和拾取增加的暴露值减半，数值均可调整。

Description (BBCode):
Always running out of time on an expedition before you have seen half the map? Exposure slows down how fast the Exposure bar fills, so you can stay out longer and search more before the zombies find you.

[size=5]Main features[/size]
[list]
[*][b]Time exposure slowed[/b]: exposure from time spent out rises 2 times slower by default.[/*]
[*][b]Running is free[/b]: running adds no extra exposure by default, so it costs the same as walking.[/*]
[*][b]Searching and looting slowed[/b]: exposure added at once by searching, looting and events is halved by default.[/*]
[*][b]Nothing lost to rounding[/b]: when a halved amount leaves a fraction, it is kept and added with the next one.[/*]
[*][b]Only rises are touched[/b]: anything that lowers exposure keeps its full effect.[/*]
[*][b]Your own numbers[/b]: every setting can be changed in the config, from the game's own rate up to 100 times slower.[/*]
[/list]
Exposure only changes how fast the bar fills. It adds nothing to your save and does not edit game files. You can remove it at any time.

The mod shows no text of its own in game, so it works the same in English and Chinese.

[size=5]Known issues[/size]
With a slowdown above 1, a small add from searching can show as 0; the fraction is added with the next one.

[size=5]Requirements[/size]
[list]
[*]Survival Log 1.1.18902. A game update can break it until the mod is updated.[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]. Install it first.[/*]
[/list]

[size=5]Installation[/size]
[list=1]
[*]Install [url=https://www.nexusmods.com/survivallog/mods/12]BepInEx[/url] [i](if no other mods were installed before, start the game once so BepInEx finishes setup, then quit)[/i].[/*]
[*]Download the zip from the Files tab.[/*]
[*]Extract it into the game folder (the folder with [font=Courier New]SurvivalLog.exe[/font]). The zip holds a BepInEx folder, so the DLL lands in the right place. When asked, merge the folders. The result should be:
[code]C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\Exposure.dll[/code][/*]
[/list]

[size=5]Uninstall[/size]
Delete [font=Courier New]Exposure.dll[/font] from the [font=Courier New]BepInEx\plugins[/font] folder.

[size=5]Configuration[/size]
To change settings, edit [font=Courier New]BepInEx\config\com.dev.survivallog.exposure.cfg[/font] while the game is closed (see [url=https://github.com/AnExiledDev/SurvivalLog-Exposure/blob/main/CONFIG.md]CONFIG.md[/url] for every setting). Changes apply at the next game start.
[list]
[*][font=Courier New]TimeSlowdown[/font] (default 2): how many times slower exposure from time rises.[/*]
[*][font=Courier New]RunningAddsExposure[/font] (default false): set to true to let running add exposure as in the game.[/*]
[*][font=Courier New]LootSlowdown[/font] (default 2): exposure from searching, looting and events is divided by this.[/*]
[/list]

[size=5]Feedback[/size]
Found a bug or have an idea? Come say hi on the [url=https://discord.gg/trTsyxUFqg]Core Directive Discord[/url], or leave a comment here. For bugs, please include [font=Courier New]BepInEx\LogOutput.log[/font].

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
[*][b]数值可调[/b]：每项设置都能在配置中修改，从游戏原速到慢 100 倍都可以。[/*]
[/list]
Exposure 只改变暴露值上涨的快慢，不会往存档里添加任何内容，也不会修改游戏文件，随时可以移除。

模组在游戏中不显示任何自己的文字，所以英文和中文下效果完全一样。

[size=5]已知问题[/size]
慢速倍数大于 1 时，一次很小的翻找可能显示为增加 0，剩下的小数会在下一次一起加上。

[size=5]前置要求[/size]
[list]
[*]Survival Log 1.1.18902。游戏更新后模组可能暂时失效，直到模组更新为止。[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]，请先安装。[/*]
[/list]

[size=5]安装[/size]
[list=1]
[*]安装 [url=https://www.nexusmods.com/survivallog/mods/12]BepInEx[/url] [i]（如果之前没有装过其他模组，先启动一次游戏，让 BepInEx 完成初始化，然后退出）[/i]。[/*]
[*]在「文件」页下载压缩包。[/*]
[*]把压缩包解压到游戏目录（[font=Courier New]SurvivalLog.exe[/font] 所在的文件夹）。压缩包里带有 BepInEx 文件夹，DLL 会自动放到正确位置。如果提示合并文件夹，选择合并。解压后应为：
[code]C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\Exposure.dll[/code][/*]
[/list]

[size=5]卸载[/size]
从 [font=Courier New]BepInEx\plugins[/font] 文件夹中删除 [font=Courier New]Exposure.dll[/font]。

[size=5]配置[/size]
如需修改设置，请在游戏关闭时编辑 [font=Courier New]BepInEx\config\com.dev.survivallog.exposure.cfg[/font]。配置文件中每项设置都有中文说明，完整说明见 [url=https://github.com/AnExiledDev/SurvivalLog-Exposure/blob/main/CONFIG.md]CONFIG.md[/url]（英文）。修改会在下次启动游戏时生效。
[list]
[*][font=Courier New]TimeSlowdown[/font]（默认 2）：随时间上涨的暴露值慢几倍。[/*]
[*][font=Courier New]RunningAddsExposure[/font]（默认 false）：设为 true 后，跑步会和游戏原本一样增加暴露值。[/*]
[*][font=Courier New]LootSlowdown[/font]（默认 2）：翻找、拾取和事件增加的暴露值会除以这个数。[/*]
[/list]

[size=5]反馈[/size]
发现 bug 或有好点子？欢迎来 [url=https://discord.gg/trTsyxUFqg]Core Directive Discord[/url] 打个招呼，或在这里留言。报告 bug 时，请附上 [font=Courier New]BepInEx\LogOutput.log[/font]。

[size=5]源代码与许可[/size]
基于 MIT 许可开源：[url=https://github.com/AnExiledDev/SurvivalLog-Exposure]github.com/AnExiledDev/SurvivalLog-Exposure[/url]
