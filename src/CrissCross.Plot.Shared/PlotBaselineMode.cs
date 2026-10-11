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

/// <summary>Specifies how an area series chooses its fill baseline.</summary>
public enum PlotBaselineMode
{
    /// <summary>Do not render a baseline fill.</summary>
    None,

    /// <summary>Fill to zero.</summary>
    Zero,

    /// <summary>Fill to <see cref="ReactivePlotSeriesStyle.Baseline"/>.</summary>
    Custom,
}
