// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Defines colors used by a complete plot surface.</summary>
public sealed record ReactivePlotTheme
{
    /// <summary>Stores the light-theme foreground color.</summary>
    private const string Ink = "#202020";

    /// <summary>Stores the light-theme surface color.</summary>
    private const string White = "#FFFFFF";

    /// <summary>Gets the default dark chart theme.</summary>
    public static ReactivePlotTheme Dark { get; } = new();

    /// <summary>Gets the default light chart theme.</summary>
    public static ReactivePlotTheme Light { get; } = new() { FigureBackground = "#F7F7F7", DataBackground = White, Axis = Ink, Grid = "#D8D8D8", LegendBackground = White, LegendForeground = Ink };

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
