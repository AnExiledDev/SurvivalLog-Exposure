Name: Exposure
Summary: Stay out longer on expeditions. The Exposure bar fills half as fast over time, running adds nothing extra, and searching and looting add half as much, so you can search more of the map before the zombies find you. Every rate can be set, from the game's own up to 100 times slower, and anything that lowers exposure still works in full.

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
[*]Survival Log 1.1.19131. A game update can break it until the mod is updated.[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]. Install it first.[/*]
[*]Optional: [url=https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit]Exiled's Toolkit[/url] 0.4.0 or newer. With it, settings changed on the Mod Manager's Mods screen take effect without restarting the game. Without it, they take effect at the next game start.[/*]
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
[*][font=Courier New]LootSlowdown[/font] (default 2): exposure from searching, looting and events is divided by this.[/*]
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
[*][b]数值可调[/b]：每项设置都能在配置中修改，从游戏原速到慢 100 倍都可以。[/*]
[/list]
Exposure 只改变暴露值上涨的快慢，不会往存档里添加任何内容，也不会修改游戏文件，随时可以移除。

模组在游戏中不显示任何自己的文字，所以英文和中文下效果完全一样。

[size=5]已知问题[/size]
慢速倍数大于 1 时，一次很小的翻找可能显示为增加 0，剩下的小数会在下一次一起加上。

[size=5]前置要求[/size]
[list]
[*]Survival Log 1.1.19131。游戏更新后模组可能暂时失效，直到模组更新为止。[/*]
[*][url=https://www.nexusmods.com/survivallog/mods/12]BepInEx for Survival Log[/url]，请先安装。[/*]
[*]可选：[url=https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit]Exiled's Toolkit[/url] 0.4.0 或更新版本。装了它，在 Mod Manager 的「模组」界面改动的设置无需重启游戏即可生效；没装的话，改动在下次启动游戏时生效。[/*]
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
[*][font=Courier New]LootSlowdown[/font]（默认 2）：翻找、拾取和事件增加的暴露值会除以这个数。[/*]
[/list]

[size=5]反馈[/size]
发现 bug 或有好点子？欢迎来 [url=https://discord.gg/trTsyxUFqg]Core Directive Discord[/url] 打个招呼，或在本页的「Posts」标签里发帖。报告 bug 时，请附上 [font=Courier New]BepInEx\LogOutput.log[/font]。
想直接联系我，可以在 Discord 上给 [b]exiled.dev[/b] 发消息，我在 [url=https://discord.gg/MZGg4gQAUe]Survival Log 官方 Discord[/url] 里。也可以在 X 上找我：[url=https://x.com/AnExiledDev]@AnExiledDev[/url]。
即将推出：Survival Log 维基，收录我所有模组的详细说明。

[size=5]源代码与许可[/size]
基于 MIT 许可开源：[url=https://github.com/AnExiledDev/SurvivalLog-Exposure]github.com/AnExiledDev/SurvivalLog-Exposure[/url]
