## Overview
[![Build & Publish Release](https://github.com/Jump-King-Multiplayer/Myra/actions/workflows/build-and-publish-release.yml/badge.svg?branch=master)](https://github.com/Jump-King-Multiplayer/Myra/actions/workflows/build-and-publish-release.yml)
[![Build & Publish Beta](https://github.com/Jump-King-Multiplayer/Myra/actions/workflows/build-and-publish-beta.yml/badge.svg)](https://github.com/Jump-King-Multiplayer/Myra/actions/workflows/build-and-publish-beta.yml)

Myra is UI Library for [MonoGame](http://www.monogame.net/), [FNA](https://github.com/FNA-XNA/FNA) and [Stride](https://github.com/stride3d/stride).

## Features
* **Rich Set of Widgets.** Myra has following widgets: Button, CheckBox, ComboBox, ListBox, TabControl, Grid, Image, Menu, ProgressBar, ScrollPane, SplitPane(with arbitrary number of splitters), Slider, TextBlock, TextField, SpinButton, Tree, Window, Dialog, FileDialog, ColorPickerDialog and PropertyGrid.
* **MML(Myra Markup Language).** XML based declarative language to describe UI ([example](/samples/Myra.Samples.AllWidgets/allControls.xmmp)).
* **Skinning.**  The default skin(it had been borrowed from [VisUI](https://github.com/kotcrab/vis-ui)) could be replaced with a custom skin loaded from the XML ([example](/samples/Myra.Samples.CustomUIStylesheet/Resources/ui_stylesheet.xmms)).
* **MyraPad.** Standalone WYSIWYG MML based UI designer.
* **Myra.PlatformAgnostic.** Version of the library that could be used in any C# game engine.

## Jump King Multiplayer Clone differences
* Added the ability to use custom widgets when using MML. MyraPad has not been updated to support this however.
* Fixed various bugs related to MML.

## Demo
If you would like to see Myra in action, download the binary release(Myra.v.v.v.v.zip from the latest release at [Releases](https://github.com/Jump-King-Multiplayer/Myra/releases)), unpack it and run samples(should be runnable on Linux too through Mono).

## Documentation
[https://github.com/Jump-King-Multiplayer/Myra/wiki](https://github.com/Jump-King-Multiplayer/Myra/wiki)

## Support
Use following resources if you need help with Myra or have other questions:
* [Myra Discord](https://discord.gg/ZeHxhCY)
* [Myra Topic at MonoGame Community Forum](http://community.monogame.net/t/myra-ui-library-for-the-monogame)
* [Myra Topic at gamedev.ru (Russian)](https://gamedev.ru/code/forum/?id=241617)

## Building From Source Code
1. Clone this repo.
2. Open a solution from the "build" folder.

## Gallery
All Widgets Sample
![](/images/AllWidgetsSample.png)

Commodore 64 Skin
![](/images/CustomStylesheetSample.png)

MyraPad
![](/images/MyraPad.png)

## Credits
* [MonoGame](http://www.monogame.net/)
* [FNA](https://github.com/FNA-XNA/FNA)
* [Stride](https://github.com/stride3d/stride)
* [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended)
* [VisUI](https://github.com/kotcrab/vis-editor/wiki/VisUI)
* [LibGDX](http://libgdx.badlogicgames.com/)
* [Cyotek.Drawing.BitmapFont](https://github.com/cyotek/Cyotek.Drawing.BitmapFont)
* [stb](https://github.com/nothings/stb)
* [TextCopy](https://github.com/SimonCropp/TextCopy)
