// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Specifies which visual parts of an XY series are visible.</summary>
public enum PlotLineMode
{
    /// <summary>Show the line and point markers.</summary>
    LineAndMarkers,

    /// <summary>Show only the line.</summary>
    LineOnly,

    /// <summary>Show only point markers.</summary>
    MarkersOnly,

    /// <summary>Hide the complete series while retaining it in the chart.</summary>
    Hidden,
}
