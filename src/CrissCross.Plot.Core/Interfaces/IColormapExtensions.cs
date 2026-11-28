// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Colormaps;

namespace CrissCross.Plot;

/// <summary>Provides sampling and transformation operations for colormaps.</summary>
public static class IColormapExtensions
{
    /// <summary>The fractional midpoint of a colormap.</summary>
    private const double MidpointFraction = 0.5;

    /// <summary>Provides sampling and conversion operations for a colormap.</summary>
    /// <param name="colormap">The colormap used by the operations.</param>
    extension(IColormap colormap)
    {
        /// <summary>Gets the color for an item at an index in a collection.</summary>
        /// <param name="index">The item index.</param>
        /// <param name="count">The number of items in the collection.</param>
        /// <param name="startFraction">The first fraction sampled from the colormap.</param>
        /// <param name="endFraction">The final fraction sampled from the colormap.</param>
        /// <returns>The color assigned to the indexed item.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Existing optional parameters are retained for source and binary compatibility.")]
        public Color GetColor(int index, int count, double startFraction = 0, double endFraction = 1)
        {
            if (count == 1)
            {
                return colormap.GetColor(MidpointFraction);
            }

            double fraction = (double)index / (count - 1);
            double fractionRange = endFraction - startFraction;
            fraction = (fraction * fractionRange) + startFraction;
            return colormap.GetColor(fraction);
        }

        /// <summary>Gets the color at a position normalized to the specified range.</summary>
        /// <param name="position">The position to sample.</param>
        /// <param name="range">The range used to normalize the position.</param>
        /// <returns>The color at the normalized position.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleSharp",
            "SST1473",
            Justification = "Exact equality detects a zero-width range; a tolerance would incorrectly collapse narrow but valid ranges.")]
        public Color GetColor(double position, Range range)
        {
            if (double.IsNaN(position))
            {
                return Colors.Transparent;
            }

            if (range.Min.Equals(range.Max))
            {
                return colormap.GetColor(0);
            }

            double normalizedPosition = range.Normalize(position, true);
            return colormap.GetColor(normalizedPosition);
        }

        /// <summary>Creates a colormap with the color order reversed.</summary>
        /// <returns>The reversed colormap.</returns>
        public IColormap Reversed() => new Reversed(colormap);

        /// <summary>Gets colors evenly spaced along the colormap.</summary>
        /// <param name="count">The number of colors to get from the colormap.</param>
        /// <param name="minFraction">The starting fraction in the colormap range.</param>
        /// <param name="maxFraction">The ending fraction in the colormap range.</param>
        /// <returns>The sampled colors.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Existing optional parameters are retained for source and binary compatibility.")]
        public Color[] GetColors(int count, double minFraction = 0, double maxFraction = 1)
        {
            if (double.IsInfinity(minFraction) || double.IsNaN(minFraction))
            {
                throw new ArgumentException($"{nameof(minFraction)} must be a real number", nameof(minFraction));
            }

            if (double.IsInfinity(maxFraction) || double.IsNaN(maxFraction))
            {
                throw new ArgumentException($"{nameof(maxFraction)} must be a real number", nameof(maxFraction));
            }

            if (count == 0)
            {
                return [];
            }

            if (count == 1)
            {
                return [colormap.GetColor(0)];
            }

            maxFraction = NumericConversion.Clamp(maxFraction, 0, 1);
            minFraction = NumericConversion.Clamp(minFraction, 0, maxFraction);
            double fractionStep = (maxFraction - minFraction) / (count - 1);
            Color[] colors = new Color[count];
            for (int i = 0; i < count; i++)
            {
                colors[i] = colormap.GetColor((i * fractionStep) + minFraction);
            }

            return colors;
        }

        /// <summary>Creates an image showing the colormap horizontally.</summary>
        /// <param name="height">The image height in pixels.</param>
        /// <param name="width">The image width in pixels.</param>
        /// <returns>The horizontal colormap image.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Existing optional parameters are retained for source and binary compatibility.")]
        public Image GetImageHorizontal(int height = 1, int width = 256)
        {
            using SKBitmap bitmap = new(width, height);
            using SKCanvas canvas = new(bitmap);
            using Paint paint = Paint.NewDisposablePaint();
            paint.IsStroke = true;
            paint.IsAntialias = false;
            paint.StrokeWidth = 1;

            for (int x = 0; x < width; x++)
            {
                double fraction = (double)x / (width - 1);
                paint.Color = colormap.GetColor(fraction);
                PixelLine line = new(x, 0, x, height);
                Drawing.DrawLine(canvas, paint, line);
            }

            return new(bitmap);
        }

        /// <summary>Creates an image showing the colormap vertically.</summary>
        /// <param name="height">The image height in pixels.</param>
        /// <param name="width">The image width in pixels.</param>
        /// <returns>The vertical colormap image.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Existing optional parameters are retained for source and binary compatibility.")]
        public Image GetImageVertical(int height = 256, int width = 1)
        {
            using SKBitmap bitmap = new(width, height);
            using SKCanvas canvas = new(bitmap);
            using Paint paint = Paint.NewDisposablePaint();
            paint.IsStroke = true;
            paint.IsAntialias = false;
            paint.StrokeWidth = 1;

            for (int y = 0; y < height; y++)
            {
                double fraction = 1.0 - ((double)y / (height - 1));
                paint.Color = colormap.GetColor(fraction);
                PixelLine line = new(0, y, width, y);
                Drawing.DrawLine(canvas, paint, line);
            }

            return new(bitmap);
        }

        /// <summary>Creates a colormap with its colors inverted.</summary>
        /// <returns>The inverted colormap.</returns>
        public IColormap Invert() => new Inverted(colormap);

        /// <summary>Creates a colormap with its hue inverted.</summary>
        /// <returns>The hue-inverted colormap.</returns>
        public IColormap InvertHue() => new InvertedHue(colormap);
    }
}
