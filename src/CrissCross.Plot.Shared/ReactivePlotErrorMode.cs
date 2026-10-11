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

/// <summary>Describes how the binder surfaces invalid updates and source failures.</summary>
public enum ReactivePlotErrorMode
{
    /// <summary>Surface the error and stop the affected series.</summary>
    SurfaceAndStopSeries,

    /// <summary>Surface the error and allow retry-capable sources to continue.</summary>
    SurfaceAndContinueWithRetry,

    /// <summary>Ignore invalid update envelopes without mutating the chart.</summary>
    IgnoreInvalidUpdates,
}
