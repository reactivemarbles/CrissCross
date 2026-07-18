// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Specifies how remaining space is distributed.</summary>
public enum SpacingMode
{
    /// <summary>Spacing is disabled and all items will be arranged as closely as possible.</summary>
    None,

    /// <summary>Provides the Uniform member.</summary>
    Uniform,

    /// <summary>Provides the BetweenItemsOnly member.</summary>
    BetweenItemsOnly,

    /// <summary>The remaining space is evenly distributed between start and end of each row.</summary>
    StartAndEndOnly,
}
