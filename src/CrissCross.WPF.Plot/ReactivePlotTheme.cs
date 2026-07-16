// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Defines colors used by a complete plot surface.</summary>
public sealed record ReactivePlotTheme
{
    /// <summary>Gets the default dark chart theme.</summary>
    public static ReactivePlotTheme Dark { get; } = new();

    /// <summary>Gets the default light chart theme.</summary>
    public static ReactivePlotTheme Light { get; } =
        new()
        {
            FigureBackground = "#F7F7F7",
            DataBackground = "#FFFFFF",
            Axis = "#202020",
            Grid = "#D8D8D8",
            LegendBackground = "#FFFFFF",
            LegendForeground = "#202020",
        };

    /// <summary>Gets the plot figure background color.</summary>
    public string FigureBackground { get; init; } = "#252526";

    /// <summary>Gets the plot data background color.</summary>
    public string DataBackground { get; init; } = "#252526";

    /// <summary>Gets the axis and label color.</summary>
    public string Axis { get; init; } = "#D7D7D7";

    /// <summary>Gets the major gridline color.</summary>
    public string Grid { get; init; } = "#404040";

    /// <summary>Gets the legend background color.</summary>
    public string LegendBackground { get; init; } = "#404040";

    /// <summary>Gets the legend foreground and outline color.</summary>
    public string LegendForeground { get; init; } = "#D7D7D7";
}
