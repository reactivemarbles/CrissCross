// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.MultiplotLayouts;

/// <summary>Arranges plots in vertically stacked rows with draggable dividers.</summary>
public class DraggableRows() : IMultiplotLayout
{
    /// <summary>Stores the height of each plot, except where the expanding plot overrides it.</summary>
    private readonly List<float> _plotHeights = [];

    /// <summary>Gets or sets the plot resized automatically to occupy the remaining space.</summary>
    public int ExpandingPlotIndex { get; set; }

    /// <summary>Gets or sets the minimum height permitted for a plot.</summary>
    public float MinimumHeight { get; set; } = 100;

    /// <summary>Gets or sets the distance in pixels for resizing when near a divider.</summary>
    public float SnapDistance { get; set; } = 5;

    /// <summary>Sets the initial height for each plot.</summary>
    /// <param name="heights">The initial plot heights, in order.</param>
    public void SetHeights(IEnumerable<float> heights)
    {
        _plotHeights.Clear();
        _plotHeights.AddRange(heights);
    }

    /// <summary>Resizes the plots to place a divider at the specified vertical pixel.</summary>
    /// <param name="index">The index of the divider to move.</param>
    /// <param name="yPixel">The requested vertical pixel position.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for existing named-argument callers.")]
    public void SetDivider(int index, float yPixel)
    {
        ArgumentChecks.ThrowIfNegative(index, nameof(index));
        ArgumentChecks.ThrowIfGreaterThanOrEqual(index, _plotHeights.Count - 1, nameof(index));

        float heightBeforeDivider = 0;
        for (int i = 0; i < index; i++)
        {
            heightBeforeDivider += _plotHeights[i];
        }

        int plotHeightAbove = index;
        int plotHeightBelow = index + 1;
        float combinedHeight = heightBeforeDivider + _plotHeights[plotHeightAbove] + _plotHeights[plotHeightBelow];
        float limit1 = heightBeforeDivider + MinimumHeight;
        float limit2 = combinedHeight - MinimumHeight;
        yPixel = NumericConversion.Clamp(yPixel, limit1, limit2) - heightBeforeDivider;

        float doubleHeight = _plotHeights[plotHeightAbove] + _plotHeights[plotHeightBelow];
        _plotHeights[plotHeightAbove] = yPixel;
        _plotHeights[plotHeightBelow] = doubleHeight - yPixel;
    }

    /// <summary>Returns the index of a divider near the specified vertical pixel.</summary>
    /// <param name="yPixel">The vertical pixel to compare with the dividers.</param>
    /// <returns>The nearby divider index, or <see langword="null"/> when none is close.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for existing named-argument callers.")]
    public int? GetDivider(float yPixel)
    {
        if (_plotHeights.Count < 2)
        {
            return null;
        }

        float[] dividerPositions = GetDividerPositions();
        for (int i = 0; i < dividerPositions.Length; i++)
        {
            float distance = Math.Abs(dividerPositions[i] - yPixel);
            if (distance < SnapDistance)
            {
                return i;
            }
        }

        return null;
    }

    /// <summary>Gets the subplot rectangles for the current figure size.</summary>
    /// <param name="subplots">The subplots to arrange.</param>
    /// <param name="figureRect">The available figure rectangle.</param>
    /// <returns>The rectangle for each subplot.</returns>
    public PixelRect[] GetSubplotRectangles(SubplotCollection subplots, PixelRect figureRect)
    {
        if (_plotHeights.Count != subplots.Count)
        {
            ResetLayout(figureRect, subplots.Count);
        }

        PixelRect[] rectangles = new PixelRect[subplots.Count];
        ResizeExpandingPlot(figureRect.Height);
        float[] dividers = GetDividerPositions();
        for (int i = 0; i < subplots.Count; i++)
        {
            bool isTopPlot = i == 0;
            bool isBottomPlot = i == subplots.Count - 1;
            rectangles[i] = new(
                left: figureRect.Left,
                right: figureRect.Right,
                bottom: isBottomPlot ? figureRect.Bottom : dividers[i],
                top: isTopPlot ? figureRect.Top : dividers[i - 1]);
        }

        return rectangles;
    }

    /// <summary>Gets the pixel positions of the dividers.</summary>
    /// <returns>The divider positions in plot order.</returns>
    private float[] GetDividerPositions()
    {
        if (_plotHeights.Count <= 1)
        {
            return _plotHeights.ToArray();
        }

        float[] positions = new float[_plotHeights.Count - 1];
        positions[0] = _plotHeights[0];
        for (int i = 1; i < positions.Length; i++)
        {
            positions[i] = positions[i - 1] + _plotHeights[i];
        }

        return positions;
    }

    /// <summary>Resizes the expanding plot to fill the remaining vertical space.</summary>
    /// <param name="totalHeight">The total vertical space available.</param>
    private void ResizeExpandingPlot(float totalHeight)
    {
        _plotHeights[ExpandingPlotIndex] = 0;
        float heightOfOtherPlots = 0;
        foreach (float height in _plotHeights)
        {
            heightOfOtherPlots += height;
        }

        _plotHeights[ExpandingPlotIndex] = totalHeight - heightOfOtherPlots;
    }

    /// <summary>Initializes equal heights for the requested number of plots.</summary>
    /// <param name="figureRect">The figure rectangle used to determine total height.</param>
    /// <param name="plotCount">The number of plots to initialize.</param>
    private void ResetLayout(PixelRect figureRect, int plotCount)
    {
        _plotHeights.Clear();
        for (int i = 0; i < plotCount; i++)
        {
            _plotHeights.Add(figureRect.Height / plotCount);
        }
    }
}
