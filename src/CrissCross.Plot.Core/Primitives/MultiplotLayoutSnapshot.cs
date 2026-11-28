// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Stores layout rectangles from the most recent multiplot render.</summary>
public class MultiplotLayoutSnapshot
{
    /// <summary>Maps each rendered plot to its last layout snapshot.</summary>
    private readonly Dictionary<Plot, SubplotLayoutSnapshot> _snapshots = [];

    /// <summary>Records the layout and axis limits for a rendered subplot.</summary>
    /// <param name="plot">The rendered plot.</param>
    /// <param name="subplotRect">The subplot rectangle in pixels.</param>
    public void Remember(Plot plot, PixelRect subplotRect)
    {
        var axisLimits = plot.RenderManager.LastRender.AxisLimits;
        _snapshots[plot] = new(axisLimits, subplotRect);
    }

    /// <summary>Gets the rectangle used for the plot's most recent render.</summary>
    /// <param name="plot">The plot to look up.</param>
    /// <returns>The last render rectangle, or <see langword="null"/> if it was not recorded.</returns>
    public PixelRect? GetLastRenderRectangle(Plot plot) => _snapshots.TryGetValue(plot, out SubplotLayoutSnapshot snapshot) ? snapshot.Rect : null;

    /// <summary>Removes the stored layout for a plot.</summary>
    /// <param name="plot">The plot to remove.</param>
    public void Forget(Plot plot) => _ = _snapshots.Remove(plot);

    /// <summary>Removes all stored layouts.</summary>
    public void Reset() => _snapshots.Clear();

    /// <summary>Returns the plot beneath a pixel according to the last render.</summary>
    /// <param name="pixel">The pixel position to inspect.</param>
    /// <returns>The plot beneath the pixel, or <see langword="null"/> if no rendered plot contains it.</returns>
    public Plot? GetPlotAtPixel(Pixel pixel)
    {
        foreach (var entry in _snapshots)
        {
            if (entry.Value.Rect.Contains(pixel))
            {
                return entry.Key;
            }
        }

        return null;
    }

    /// <summary>Stores the limits and rectangle for one rendered subplot.</summary>
    /// <param name="Limits">The axis limits captured during rendering.</param>
    /// <param name="Rect">The rendered subplot rectangle.</param>
    private readonly record struct SubplotLayoutSnapshot(AxisLimits Limits, PixelRect Rect);
}
