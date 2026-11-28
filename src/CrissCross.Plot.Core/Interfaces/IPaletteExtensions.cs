// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides convenience operations for palettes.</summary>
public static class IPaletteExtensions
{
    /// <summary>Provides sampling and color transformation operations for a palette.</summary>
    /// <param name="palette">The palette used by these operations.</param>
    extension(IPalette palette)
    {
        /// <summary>Gets the specified number of colors from the palette.</summary>
        /// <param name="count">The number of colors to retrieve.</param>
        /// <returns>The colors in palette order, wrapping when necessary.</returns>
        public Color[] GetColors(int count)
        {
            ArgumentChecks.ThrowIfNegative(count, nameof(count));
            Color[] colors = new Color[count];
            for (int i = 0; i < count; i++)
            {
                colors[i] = palette.GetColor(i);
            }

            return colors;
        }

        /// <summary>Creates a palette with every color inverted.</summary>
        /// <returns>The palette containing inverted colors.</returns>
        public IPalette Inverted()
        {
            Color[] sourceColors = palette.Colors;
            Color[] invertedColors = new Color[sourceColors.Length];
            for (int i = 0; i < sourceColors.Length; i++)
            {
                invertedColors[i] = sourceColors[i].Inverted();
            }

            return new Palettes.Custom(invertedColors, $"{palette.Name} (inverted)", palette.Description);
        }

        /// <summary>Creates a palette with every color's hue inverted.</summary>
        /// <returns>The palette containing hue-inverted colors.</returns>
        public IPalette InvertedHue()
        {
            Color[] sourceColors = palette.Colors;
            Color[] invertedColors = new Color[sourceColors.Length];
            for (int i = 0; i < sourceColors.Length; i++)
            {
                invertedColors[i] = sourceColors[i].InvertedHue();
            }

            return new Palettes.Custom(invertedColors, $"{palette.Name} (inverted hue)", palette.Description);
        }

        /// <summary>Gets the number of colors in the palette.</summary>
        /// <returns>The number of colors.</returns>
        public int Count() => palette.Colors.Length;
    }
}
