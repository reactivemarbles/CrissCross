// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides extension operations for plot controls.</summary>
public static class IPlotControlExtensions
{
    /// <summary>Provides plot hit-testing for a plot control.</summary>
    /// <param name="plotControl">The plot control to inspect.</param>
    extension(IPlotControl plotControl)
    {
        /// <summary>Gets the plot beneath the specified pixel position.</summary>
        /// <param name="pixel">The pixel position to inspect.</param>
        /// <returns>The plot at the position, or <see langword="null"/> when none is present.</returns>
        public Plot? GetPlotAtPixel(Pixel pixel) => plotControl.Multiplot.GetPlotAtPixel(pixel);
    }
}
