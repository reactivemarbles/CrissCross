// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

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

    /// <summary>Gets every available palette in the assembly.</summary>
    /// <returns>The available palette instances.</returns>
    [RequiresUnreferencedCode("Discovers palettes by reflecting over the assembly types.")]
    public static IPalette[] GetPalettes() => TypeDiscovery.GetImplementations<IPalette>(typeof(Palette).Assembly);
}
