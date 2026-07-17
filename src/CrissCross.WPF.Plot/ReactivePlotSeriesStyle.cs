// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Defines immutable styling for a normalized reactive plot series.</summary>
public sealed record ReactivePlotSeriesStyle
{
    /// <summary>Gets an optional hexadecimal or named series color.</summary>
    public string? Color { get; init; }

    /// <summary>Gets the rendered line width.</summary>
    public float LineWidth { get; init; } = 2;

    /// <summary>Gets the rendered marker size.</summary>
    public float MarkerSize { get; init; } = 5;

    /// <summary>Gets the visible line and marker mode.</summary>
    public PlotLineMode LineMode { get; init; } = PlotLineMode.LineOnly;

    /// <summary>Gets the area baseline mode.</summary>
    public PlotBaselineMode BaselineMode { get; init; }

    /// <summary>Gets the custom area baseline for <see cref="PlotBaselineMode.Custom"/>.</summary>
    public double Baseline { get; init; }

    /// <summary>Gets a value indicating whether the series is included in the plot legend.</summary>
    public bool ShowInLegend { get; init; } = true;
}
