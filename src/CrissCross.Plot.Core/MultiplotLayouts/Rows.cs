// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.MultiplotLayouts;

/// <summary>Arranges subplots in equally sized rows.</summary>
public class Rows : IMultiplotLayout
{
    /// <summary>Gets equally sized subplot rectangles arranged vertically.</summary>
    /// <param name="subplots">The subplots to arrange.</param>
    /// <param name="figureRect">The rectangle available to the multiplot.</param>
    /// <returns>The subplot rectangles in row order.</returns>
    public PixelRect[] GetSubplotRectangles(SubplotCollection subplots, PixelRect figureRect)
    {
        PixelRect[] rectangles = new PixelRect[subplots.Count];

        double fractionPerRow = 1.0 / subplots.Count;
        for (int i = 0; i < subplots.Count; i++)
        {
            FractionRect fr = new(0, fractionPerRow * i, 1, fractionPerRow);
            rectangles[i] = fr.GetPixelRect(figureRect);
        }

        return rectangles;
    }
}
