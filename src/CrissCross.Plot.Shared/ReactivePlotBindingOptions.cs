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

/// <summary>Binding options for reactive chart updates.</summary>
public sealed record ReactivePlotBindingOptions
{
    /// <summary>Gets the scheduler used to subscribe to source streams when specified.</summary>
    public IScheduler? SourceScheduler { get; init; }

    /// <summary>Gets the scheduler used to marshal adapter mutations.</summary>
    public IScheduler? UiScheduler { get; init; }

    /// <summary>Gets the optional time window used to coalesce high-frequency updates.</summary>
    public TimeSpan? BatchWindow { get; init; }

    /// <summary>Gets the maximum number of updates combined into a single adapter mutation.</summary>
    public int MaxBatchSize { get; init; } = 1;

    /// <summary>Gets the optional maximum number of visible points retained per series.</summary>
    public int? MaxVisiblePoints { get; init; }

    /// <summary>Gets the number of configured Y axes that updates may target.</summary>
    public int MaxAxisCount { get; init; } = 16;

    /// <summary>Gets the overflow strategy used when maximum visible points is exceeded.</summary>
    public ReactivePlotOverflowStrategy OverflowStrategy { get; init; } = ReactivePlotOverflowStrategy.DropOldest;

    /// <summary>Gets the error mode used for source and validation failures.</summary>
    public ReactivePlotErrorMode ErrorMode { get; init; } = ReactivePlotErrorMode.SurfaceAndStopSeries;
}
