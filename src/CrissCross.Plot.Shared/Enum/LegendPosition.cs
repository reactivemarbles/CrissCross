// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if PLOT_MAUI && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#elif PLOT_MAUI
namespace CrissCross.Maui.Plot;
#elif PLOT_AVALONIA && REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#elif PLOT_AVALONIA
namespace CrissCross.Avalonia.Plot;
#elif REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Specifies the position of a legend within a chart or graphical component.</summary>
/// <remarks>Use this enumeration to control where the legend is displayed relative to the chart area. The
/// available positions are Top, Right, and Left. The choice of position may affect the layout and readability of the
/// chart, especially when space is limited.</remarks>
public enum LegendPosition
{
    /// <summary>The top.</summary>
    Top,

    /// <summary>The right.</summary>
    Right,

    /// <summary>The left.</summary>
    Left,
}
