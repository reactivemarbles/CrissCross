// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Calculates the layout of legend items.</summary>
public interface ILegendLayout
{
    /// <summary>Gets the layout for the specified legend items.</summary>
    /// <param name="legend">The legend being laid out.</param>
    /// <param name="items">The items to arrange.</param>
    /// <param name="maxSize">The maximum available size.</param>
    /// <param name="paint">The paint used to measure text.</param>
    /// <returns>The calculated legend layout.</returns>
    LegendLayout GetLayout(Legend legend, LegendItem[] items, PixelSize maxSize, Paint paint);
}
