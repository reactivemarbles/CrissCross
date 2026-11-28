// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Calculates the subplot rectangles for a multiplot.</summary>
public interface IMultiplotLayout
{
    /// <summary>Gets the rectangle of each subplot within the figure.</summary>
    /// <param name="subplots">The subplots to arrange.</param>
    /// <param name="figureRect">The rectangle available to the multiplot.</param>
    /// <returns>The rectangle for each subplot.</returns>
    PixelRect[] GetSubplotRectangles(SubplotCollection subplots, PixelRect figureRect);
}
