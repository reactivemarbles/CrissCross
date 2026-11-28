// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides extension operations for axis coordinate and pixel conversions.</summary>
public static class IAxesExtensions
{
    /// <summary>The divisor used to calculate half of a requested axis span.</summary>
    private const double SpanDivisor = 2;

    /// <summary>Provides span and path-conversion operations for an axis pair.</summary>
    /// <param name="axes">The axis pair used by these operations.</param>
    extension(IAxes axes)
    {
        /// <summary>Sets the horizontal axis range to the specified span around its current center.</summary>
        /// <param name="span">The desired horizontal axis span.</param>
        public void SetSpanX(double span)
        {
            double horizontalMinimum = axes.XAxis.Range.Center - (span / SpanDivisor);
            double horizontalMaximum = axes.XAxis.Range.Center + (span / SpanDivisor);
            axes.XAxis.Range.Set(horizontalMinimum, horizontalMaximum);
        }

        /// <summary>Sets the vertical axis range to the specified span around its current center.</summary>
        /// <param name="span">The desired vertical axis span.</param>
        public void SetSpanY(double span)
        {
            double verticalMinimum = axes.YAxis.Range.Center - (span / SpanDivisor);
            double verticalMaximum = axes.YAxis.Range.Center + (span / SpanDivisor);
            axes.YAxis.Range.Set(verticalMinimum, verticalMaximum);
        }

        /// <summary>Converts a coordinate path to a pixel path.</summary>
        /// <param name="path">The coordinate path to convert.</param>
        /// <returns>The converted pixel path.</returns>
        public PixelPath GetPixelPath(CoordinatePath path)
        {
            List<Pixel> pixels = [];
            foreach (Coordinates point in path.Points)
            {
                pixels.Add(axes.GetPixel(point));
            }

            Pixel[] pixelArray = pixels.ToArray();
            return path.Close ? PixelPath.Closed(pixelArray) : PixelPath.Open(pixelArray);
        }

        /// <summary>Converts coordinate paths to pixel paths.</summary>
        /// <param name="paths">The coordinate paths to convert.</param>
        /// <returns>The converted pixel paths.</returns>
        public PixelPath[] GetPixelPaths(CoordinatePath[] paths)
        {
            PixelPath[] pixelPaths = new PixelPath[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                pixelPaths[i] = axes.GetPixelPath(paths[i]);
            }

            return pixelPaths;
        }
    }
}
