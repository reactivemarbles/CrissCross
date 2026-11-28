// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Synchronizes horizontal or vertical axis limits across multiple plots.</summary>
public class MultiplotSharedAxisManager : IMultiplotPreRenderAction
{
    // TODO: improve support for plots with non-standard axis limits
    /// <summary>The plots whose horizontal limits are synchronized.</summary>
    private readonly List<Plot> _plotsWithSharedX = [];

    /// <summary>The plots whose vertical limits are synchronized.</summary>
    private readonly List<Plot> _plotsWithSharedY = [];

    /// <summary>Links horizontal axis limits across the given plots.</summary>
    /// <param name="plots">The plots to synchronize horizontally.</param>
    public void ShareX(IEnumerable<Plot> plots)
    {
        _plotsWithSharedX.Clear();
        _plotsWithSharedX.AddRange(plots);

        // reset remembered axis limits to force realignment on the next render
        foreach (Plot plot in plots)
        {
            plot.RenderManager.ForgetLastRender();
        }
    }

    /// <summary>Links vertical axis limits across the given plots.</summary>
    /// <param name="plots">The plots to synchronize vertically.</param>
    public void ShareY(IEnumerable<Plot> plots)
    {
        _plotsWithSharedY.Clear();
        _plotsWithSharedY.AddRange(plots);

        // reset remembered axis limits to force realignment on the next render
        foreach (Plot plot in plots)
        {
            plot.RenderManager.ForgetLastRender();
        }
    }

    /// <summary>Updates the linked plots to match the first plot with changed limits.</summary>
    public void UpdateSharedPlotAxisLimits()
    {
        Plot? parentPlotX = GetFirstPlotWithChangedLimitsX();
        if (parentPlotX is not null)
        {
            AxisLimits parentLimits = parentPlotX.Axes.GetLimits();
            _plotsWithSharedX.ForEach(x => x.Axes.SetLimitsX(parentLimits));
        }

        Plot? parentPlotY = GetFirstPlotWithChangedLimitsY();
        if (parentPlotY is not null)
        {
            AxisLimits parentLimits = parentPlotY.Axes.GetLimits();
            _plotsWithSharedY.ForEach(x => x.Axes.SetLimitsY(parentLimits));
        }
    }

    /// <summary>Synchronizes linked limits before rendering.</summary>
    public void Invoke() => UpdateSharedPlotAxisLimits();

    /// <summary>Finds the first plot whose horizontal limits changed.</summary>
    /// <returns>The plot with changed limits, or <see langword="null"/> when none changed.</returns>
    private Plot? GetFirstPlotWithChangedLimitsX()
    {
        foreach (var plot in _plotsWithSharedX)
        {
            var oldRange = plot.Axes.GetLimits().HorizontalRange;
            var newRange = plot.LastRender.AxisLimits.HorizontalRange;
            if (oldRange != newRange)
            {
                return plot;
            }
        }

        return null;
    }

    /// <summary>Finds the first plot whose vertical limits changed.</summary>
    /// <returns>The plot with changed limits, or <see langword="null"/> when none changed.</returns>
    private Plot? GetFirstPlotWithChangedLimitsY()
    {
        foreach (var plot in _plotsWithSharedY)
        {
            var oldRange = plot.Axes.GetLimits().VerticalRange;
            var newRange = plot.LastRender.AxisLimits.VerticalRange;
            if (oldRange != newRange)
            {
                return plot;
            }
        }

        return null;
    }
}
