// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Stores each plot axis range so the original limits can be restored later.</summary>
/// <param name="plot">The plot whose axis limits are stored.</param>
public class MultiAxisLimits(Plot plot)
{
    /// <summary>Gets the plot whose axis limits are stored.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is part of the existing API; changing it to a property would be binary-breaking.")]
    public readonly Plot Plot = plot;

    /// <summary>The original range for each axis on the plot.</summary>
    private readonly Dictionary<IAxis, CoordinateRange> _axisRanges = CreateInitialAxisRanges(plot);

    /// <summary>Set all axis limits to their original ranges.</summary>
    public void Recall()
    {
        foreach (var axisAndRange in _axisRanges)
        {
            axisAndRange.Key.Range.Set(axisAndRange.Value);
        }
    }

    /// <summary>Creates a snapshot of the current range for each plot axis.</summary>
    /// <param name="plot">The plot whose axis ranges are captured.</param>
    /// <returns>The initial range for each axis.</returns>
    private static Dictionary<IAxis, CoordinateRange> CreateInitialAxisRanges(Plot plot)
    {
        Dictionary<IAxis, CoordinateRange> ranges = [];
        foreach (IAxis axis in plot.Axes.GetAxes())
        {
            ranges.Add(axis, axis.Range.ToCoordinateRange);
        }

        return ranges;
    }
}
