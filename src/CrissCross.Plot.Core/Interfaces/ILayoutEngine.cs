// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>
/// This interface describes a class that decides how to lay-out a collection of panels around the
/// edges of a figure and create a final layout containing size and position of all panels
/// and also the size and position of the data area.
/// </summary>
public interface ILayoutEngine
{
    /// <summary>Calculates the layout of a figure and its panels.</summary>
    /// <param name="figureRect">The pixel rectangle available for the figure.</param>
    /// <param name="plot">The plot whose panels are arranged.</param>
    /// <param name="paint">The paint used to measure text and decorations.</param>
    /// <returns>The calculated layout.</returns>
    Layout GetLayout(PixelRect figureRect, Plot plot, Paint paint);
}
