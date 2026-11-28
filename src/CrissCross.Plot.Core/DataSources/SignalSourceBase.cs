// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides shared coordinate and render-range behavior for signal sources.</summary>
public abstract class SignalSourceBase
{
    /// <summary>Gets or sets the horizontal coordinate distance between samples.</summary>
    public double Period { get; set; }

    /// <summary>Gets the number of signal samples.</summary>
    public abstract int Length { get; }

    /// <summary>Gets or sets the first sample index available for rendering.</summary>
    public int MinimumIndex { get; set; }

    /// <summary>Gets or sets the last sample index available for rendering.</summary>
    public int MaximumIndex { get; set; } = int.MaxValue;

    /// <summary>Gets the minimum visible sample index.</summary>
    public int MinRenderIndex => Math.Max(0, MinimumIndex);

    /// <summary>Gets the maximum visible sample index.</summary>
    public int MaxRenderIndex => Math.Min(Length - 1, MaximumIndex);

    /// <summary>Gets a value indicating whether pixel-overlap handling is enabled for signal rendering;/>.</summary>
    public bool UsePixelOverlap { get; }

    /// <summary>Gets or sets the horizontal coordinate offset.</summary>
    public double XOffset { get; set; }

    /// <summary>Gets or sets the vertical coordinate offset.</summary>
    public double YOffset { get; set; }

    /// <summary>Gets or sets the vertical coordinate scale.</summary>
    public double YScale { get; set; } = 1;

    /// <summary>Gets the sample index for a horizontal coordinate.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="visibleDataOnly">Whether to clamp the result to the visible sample range.</param>
    /// <returns>The corresponding sample index.</returns>
    public int GetIndex(double x, bool visibleDataOnly)
    {
        int i = (int)((x - XOffset) / Period);

        if (visibleDataOnly)
        {
            i = Math.Max(i, MinRenderIndex);
            i = Math.Min(i, MaxRenderIndex);
        }

        return i;
    }

    /// <summary>Determines whether a horizontal coordinate range overlaps available signal samples.</summary>
    /// <param name="xMin">The minimum horizontal coordinate.</param>
    /// <param name="xMax">The maximum horizontal coordinate.</param>
    /// <returns><see langword="true"/> when the range overlaps visible signal data.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The parameter names are retained for existing named-argument callers.")]
    public bool RangeContainsSignal(double xMin, double xMax)
    {
        int minimumIndex = GetIndex(xMin, false);
        int maximumIndex = GetIndex(xMax, false);
        return maximumIndex >= MinRenderIndex && minimumIndex <= MaxRenderIndex;
    }

    /// <summary>Gets the horizontal coordinate at the specified index.</summary>
    /// <param name="index">The signal index.</param>
    /// <returns>The horizontal coordinate.</returns>
    public double GetX(int index) => (index * Period) + XOffset;

    /// <summary>Gets the horizontal coordinate limits of the signal.</summary>
    /// <returns>The horizontal coordinate range.</returns>
    public CoordinateRange GetLimitsX() => GetLimits().Rect.XRange;

    /// <summary>Gets the vertical coordinate limits of the signal.</summary>
    /// <returns>The vertical coordinate range.</returns>
    public CoordinateRange GetLimitsY() => GetLimits().Rect.YRange;

    /// <summary>Gets the vertical signal limits within an index range.</summary>
    /// <param name="firstIndex">The first index, inclusive.</param>
    /// <param name="lastIndex">The last index, inclusive.</param>
    /// <returns>The minimum and maximum signal values in the range.</returns>
    public abstract SignalRangeY GetLimitsY(int firstIndex, int lastIndex);

    /// <summary>Gets the combined coordinate limits of the signal.</summary>
    /// <returns>The coordinate limits accounting for offsets and scaling.</returns>
    public AxisLimits GetLimits()
    {
        SignalRangeY rangeY = GetLimitsY(MinRenderIndex, MaxRenderIndex);

        return new(
            left: XOffset + (MinRenderIndex * Period),
            right: XOffset + (MaxRenderIndex * Period),
            bottom: (rangeY.Min * YScale) + YOffset,
            top: (rangeY.Max * YScale) + YOffset);
    }
}
