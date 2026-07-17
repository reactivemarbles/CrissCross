// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
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
