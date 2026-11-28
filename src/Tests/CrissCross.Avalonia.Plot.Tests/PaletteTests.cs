// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies the built-in palette registry.</summary>
public sealed class PaletteTests
{
    /// <summary>Checks that every built-in palette is available without reflection-based discovery.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetPalettes_ReturnsEveryBuiltInPalette()
    {
        IPalette[] palettes = Palette.GetPalettes();
        string[] names = new string[palettes.Length];
        for (int i = 0; i < palettes.Length; i++)
        {
            names[i] = palettes[i].GetType().Name;
        }

        await Assert.That(names).IsEquivalentTo([
            "Amber",
            "Aurora",
            "Building",
            "Category10",
            "Category20",
            "ColorblindFriendly",
            "ColorblindFriendlyDark",
            "Dark",
            "DarkPastel",
            "Frost",
            "LightOcean",
            "LightSpectrum",
            "Microcharts",
            "Nero",
            "Nord",
            "Normal",
            "OneHalf",
            "OneHalfDark",
            "PastelWheel",
            "Penumbra",
            "PolarNight",
            "Redness",
            "SnowStorm",
            "SummerSplash",
            "Tsitsulin",
        ]);
    }
}
