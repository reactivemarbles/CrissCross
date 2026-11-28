// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

/*
--- NOTICE BEGIN ---

MIT License

Copyright (c) 2017 Aloïs Deniel

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

--- NOTICE END ---
*/

namespace CrissCross.Plot.Palettes;

/// <summary>Provides the Microcharts palette.</summary>
public class Microcharts : IPalette
{
    /// <summary>Contains the hex values used to construct the palette.</summary>
    private static readonly string[] HexColors =
    {
        "#266489", "#68B9C0", "#90D585", "#F3C151", "#F37F64",
        "#424856", "#8F97A4", "#DAC096", "#76846E", "#DABFAF",
        "#A65B69", "#97A69D",
    };

    /// <summary>Gets the palette name.</summary>
    public string Name { get; } = "Microcharts";

    /// <summary>Gets the palette description.</summary>
    public string Description { get; } = "Soft color palette sourced from "
        + "the Microcharts project: https://github.com/microcharts-dotnet/Microcharts";

    /// <summary>Gets the palette colors.</summary>
    public Color[] Colors { get; } = Color.FromHex(HexColors);

    /// <summary>Gets the palette color at the specified index, wrapping at the end.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the requested index.</returns>
    public Color GetColor(int index) => Colors[index % Colors.Length];
}
