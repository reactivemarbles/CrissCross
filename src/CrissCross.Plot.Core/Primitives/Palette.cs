// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides built-in palette access and custom palette creation.</summary>
public static class Palette
{
    /// <summary>Gets or sets the default palette.</summary>
    public static IPalette Default { get; set; } = new Palettes.Category10();

    /// <summary>Creates a custom palette from hexadecimal color values.</summary>
    /// <param name="hexColors">The colors represented as hexadecimal strings.</param>
    /// <returns>The custom palette.</returns>
    public static IPalette FromColors(string[] hexColors) => new Palettes.Custom(hexColors, string.Empty, string.Empty);

    /// <summary>Creates a custom palette from color values.</summary>
    /// <param name="colors">The colors in the palette.</param>
    /// <returns>The custom palette.</returns>
    public static IPalette FromColors(Color[] colors) => new Palettes.Custom(colors, string.Empty, string.Empty);

    /// <summary>Gets every built-in palette.</summary>
    /// <returns>The available palette instances.</returns>
    public static IPalette[] GetPalettes() =>
    [
        new Palettes.Amber(),
        new Palettes.Aurora(),
        new Palettes.Building(),
        new Palettes.Category10(),
        new Palettes.Category20(),
        new Palettes.ColorblindFriendly(),
        new Palettes.ColorblindFriendlyDark(),
        new Palettes.Dark(),
        new Palettes.DarkPastel(),
        new Palettes.Frost(),
        new Palettes.LightOcean(),
        new Palettes.LightSpectrum(),
        new Palettes.Microcharts(),
        new Palettes.Nero(),
        new Palettes.Nord(),
        new Palettes.Normal(),
        new Palettes.OneHalf(),
        new Palettes.OneHalfDark(),
        new Palettes.PastelWheel(),
        new Palettes.Penumbra(),
        new Palettes.PolarNight(),
        new Palettes.Redness(),
        new Palettes.SnowStorm(),
        new Palettes.SummerSplash(),
        new Palettes.Tsitsulin(),
    ];
}
