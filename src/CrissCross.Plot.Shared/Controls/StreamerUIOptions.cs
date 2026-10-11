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

/// <summary>Configures scaling and point retention for a <see cref="StreamerUI"/>.</summary>
public sealed class StreamerUIOptions
{
    /// <summary>Gets a value indicating whether automatic axis scaling is enabled.</summary>
    public bool AutoScale { get; init; } = true;

    /// <summary>Gets a value indicating whether manual axis scaling is enabled.</summary>
    public bool ManualScale { get; init; }

    /// <summary>Gets a value indicating whether the displayed point count is fixed.</summary>
    public bool FixedPoints { get; init; }
}
