// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
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
