// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Clock system.</summary>
public enum ClockIdentifier
{
    /// <summary>The clock12 hour.</summary>
    Clock12Hour,

    /// <summary>The clock24 hour.</summary>
    Clock24Hour,
}
