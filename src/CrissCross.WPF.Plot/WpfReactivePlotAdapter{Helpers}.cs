// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Plottables;

namespace CrissCross.WPF.Plot;

/// <summary>Applies normalized reactive plot updates to WPF plot UI elements.</summary>
internal sealed partial class WpfReactivePlotAdapter
{
    /// <summary>Clears a SignalXY UI element.</summary>
    /// <param name="signalXy">The SignalXY UI element.</param>
    private static void ClearSignalXy(SignalXY_UI signalXy) =>
        signalXy.PlotLine!.Data = new SignalXYSourceDoubleArray([], []);

    /// <summary>Completes and disposes a subject.</summary>
    /// <typeparam name="T">The subject value type.</typeparam>
    /// <param name="subject">The subject to dispose.</param>
    private static void DisposeSubject<T>(Signal<T>? subject)
    {
        if (subject is null)
        {
            return;
        }

        subject.OnCompleted();
        subject.Dispose();
    }

    /// <summary>Creates a horizontal-step scatter line.</summary>
    /// <param name="plot">The destination plot.</param>
    /// <param name="x">The rendered X values.</param>
    /// <param name="y">The rendered Y values.</param>
    /// <returns>The created scatter line.</returns>
    private static Scatter CreateStepLine(ScottPlot.Plot plot, double[] x, double[] y)
    {
        var scatter = plot.Add.ScatterLine(x, y);
        scatter.ConnectStyle = ConnectStyle.StepHorizontal;
        return scatter;
    }

    /// <summary>Creates an area series with optional baseline styling.</summary>
    /// <param name="plot">The destination plot.</param>
    /// <param name="x">The rendered X values.</param>
    /// <param name="y">The rendered Y values.</param>
    /// <param name="style">Optional series styling.</param>
    /// <returns>The created area scatter.</returns>
    private static Scatter CreateArea(
        ScottPlot.Plot plot,
        double[] x,
        double[] y,
        ReactivePlotSeriesStyle? style)
    {
        var scatter = plot.Add.ScatterLine(x, y);
        var baselineMode = style?.BaselineMode ?? PlotBaselineMode.Zero;
        scatter.FillY = baselineMode != PlotBaselineMode.None;
        scatter.FillYValue = baselineMode == PlotBaselineMode.Custom ? style!.Baseline : 0;
        return scatter;
    }

    /// <summary>Converts normalized X values to values rendered by ScottPlot.</summary>
    /// <param name="x">The normalized X values.</param>
    /// <param name="axisKind">The X-axis interpretation.</param>
    /// <returns>The rendered X values.</returns>
    private static double[] ConvertXValues(IReadOnlyList<double> x, PlotXAxisKind axisKind)
    {
        var converted = new double[x.Count];
        if (axisKind != PlotXAxisKind.Ticks)
        {
            for (var i = 0; i < x.Count; i++)
            {
                converted[i] = x[i];
            }

            return converted;
        }

        for (var i = 0; i < x.Count; i++)
        {
            converted[i] = new DateTime(Convert.ToInt64(x[i]), DateTimeKind.Local).ToOADate();
        }

        return converted;
    }

    /// <summary>Sets a legend label on a supported snapshot plottable.</summary>
    /// <param name="plottable">The plottable to update.</param>
    /// <param name="name">The series name.</param>
    /// <param name="showInLegend">Whether the label is visible in the legend.</param>
    private static void SetLegendText(IPlottable plottable, string name, bool showInLegend)
    {
        var legendText = showInLegend ? name : string.Empty;
        switch (plottable)
        {
            case Scatter scatter:
            {
                scatter.LegendText = legendText;
                break;
            }

            case BarPlot bars:
            {
                bars.LegendText = legendText;
                break;
            }

            case LollipopPlot stem:
            {
                stem.LegendText = legendText;
                break;
            }

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(plottable),
                    plottable,
                    "The plottable does not support legend text.");
        }
    }

    /// <summary>Resolves a named or hexadecimal series color.</summary>
    /// <param name="colorName">The color value.</param>
    /// <returns>The ScottPlot color.</returns>
    private static Color ResolveColor(string colorName)
    {
        if (colorName.StartsWith("#", StringComparison.Ordinal))
        {
            return Color.FromHex(colorName);
        }

        var drawingColor = System.Drawing.Color.FromName(colorName);
        var isUnknownColor =
            drawingColor.A == 0
            && !string.Equals(
                colorName,
                nameof(System.Drawing.Color.Transparent),
                StringComparison.OrdinalIgnoreCase);
        return isUnknownColor ? Colors.White : Color.FromColor(drawingColor);
    }
}
