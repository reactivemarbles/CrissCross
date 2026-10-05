// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Defines how a fixed-buffer stream advances across the plot.</summary>
public enum StreamerViewMode
{
    /// <summary>Moves existing samples left as new samples arrive at the right.</summary>
    ScrollLeft,

    /// <summary>Moves existing samples right as new samples arrive at the left.</summary>
    ScrollRight,

    /// <summary>Overwrites samples in place while the writing position moves left.</summary>
    WipeLeft,

    /// <summary>Overwrites samples in place while the writing position moves right.</summary>
    WipeRight,
}
