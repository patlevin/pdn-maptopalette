# Map To Palette - A Paint Dot NET Effect Plugin

## Purpose

This plugin maps the colours of an image, selection, or layer to the
currently selected colour palette in [Paint.NET](https://www.getpaint.net)(PDN).
This effect can be useful in artwork creation for consoles that
use fixed palettes such as the (S)NES, the Sega Master System,
or GameBoy Color. The effect can also be used to achieve a certain look.

## Why not just use an existing plugin

I created this plugin for personal purposes and decided to share it.
The reason I didn't just use existing plugins was the lack of [dithering](https://en.wikipedia.org/wiki/Dither) support. Since I use the plugin for
creating artwork for retro games, the quality improvement from
[error diffusion](https://en.wikipedia.org/wiki/Error_diffusion) seemed
well worth the effort.

## Installation

The plugin can be installed by copying the effect DLL and optionally the
sub-directories and localised resource DLLs therein to the _Effects_ folder
of your PDN installation directory.

You will find the plugin in the _Effects_ menu under the _Color_ sub-menu.

## Building from source

In order to build the plugin from source, you need a Microsoft Windows 7 or newer with PDN installed. Compilation was only tested on 64-bit Windows 10
v1709:

* [PDN](https://www.getpaint.net/download.html#download) v4.0.6 or later
* [.NET-Framework 4.7](https://www.microsoft.com/en-us/search/result.aspx?q=.net+framework+4.7)
* Visual Studio 2017 (Community Edition or better)
* C# Language Version 7.1 or later

In case your PDN installation is not located at _C:\Program Files\paint.net_, you need to change the reference to the proper location of the
assemblies on your machine.

Open the solution file in Visual Studio and rebuild to generate the plugin.

## Localisation

You can use the source code to localise and modify the texts used by the plugin.
To do that, you can either modify the file _Strings.resx_ directly or copy it
to _Strings.[ISO 639-1 language code].resx_ and translate the texts in the new file.

Compiling will then create a new resource DLL and a new locale directory, both of
which need to be copied to the PDN _effects_ folder to apply the changes.
Note that the language code you choose needs to match the locale of your operating system in order for the localised version to get applied - e.g.
_Strings.fr.resx_ for French texts only works if the UI language of your OS is also set to French.

Please refer to the [framework documentation](https://docs.microsoft.com/en-us/dotnet/standard/globalization-localization/)
for further details.

## License

Copyright (c) 2018 Patrick Levin

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.