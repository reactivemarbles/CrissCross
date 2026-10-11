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

/// <summary>Describes how retained plot data overflows are handled.</summary>
public enum ReactivePlotOverflowStrategy
{
    /// <summary>Drop the oldest retained points first.</summary>
    DropOldest,

    /// <summary>Drop the newest incoming points first.</summary>
    DropNewest,

    /// <summary>Keep only the latest visible points.</summary>
    KeepLatest,

    /// <summary>Reserve space for future blocking implementations; currently behaves as drop-oldest.</summary>
    BlockSource,
}
