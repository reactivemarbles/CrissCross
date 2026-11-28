// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Autoscale axes that do not yet have limits set.</summary>
public class AutoScaleUnsetAxes : IRenderAction
{
    /// <summary>Adjusts unset axes according to visible plot data.</summary>
    /// <param name="rp">The render pack for the current render.</param>
    public void Render(RenderPack rp)
    {
        bool hasVisiblePlottables = false;
        foreach (IPlottable plottable in rp.Plot.PlottableList)
        {
            if (!plottable.IsVisible)
            {
                continue;
            }

            hasVisiblePlottables = true;
            break;
        }

        if (hasVisiblePlottables)
        {
            AutoscaleUnsetAxesToData(rp.Plot);
        }
        else
        {
            ApplyDefaultLimitsToUnsetAxes(rp.Plot);
        }
    }

    /// <summary>Applies default limits to axes that have no limits set.</summary>
    /// <param name="plot">The plot whose axes are updated.</param>
    private static void ApplyDefaultLimitsToUnsetAxes(Plot plot)
    {
        if (!plot.Axes.Bottom.Range.HasBeenSet)
        {
            plot.Axes.SetLimitsX(AxisLimits.Default);
        }

        if (plot.Axes.Left.Range.HasBeenSet)
        {
            return;
        }

        plot.Axes.SetLimitsY(AxisLimits.Default);
    }

    /// <summary>Autoscale unset axes using the plot's data.</summary>
    /// <param name="plot">The plot whose axes are updated.</param>
    private void AutoscaleUnsetAxesToData(Plot plot)
    {
        foreach (IPlottable plottable in plot.PlottableList)
        {
            bool shouldScaleXAxis = !plottable.Axes.XAxis.Range.HasBeenSet;
            bool shouldScaleYAxis = !plottable.Axes.YAxis.Range.HasBeenSet;
            if (shouldScaleXAxis || shouldScaleYAxis)
            {
                plot.Axes.AutoScale(
                    xAxis: plottable.Axes.XAxis,
                    yAxis: plottable.Axes.YAxis,
                    horizontal: shouldScaleXAxis,
                    vertical: shouldScaleYAxis);
            }
        }
    }
}
