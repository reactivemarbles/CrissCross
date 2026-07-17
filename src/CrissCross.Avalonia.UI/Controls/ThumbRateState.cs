// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>States of the <see cref="ThumbRate"/> control.</summary>
public enum ThumbRateState
{
    /// <summary>No thumb has been clicked.</summary>
    None,

    /// <summary>The thumb up has been clicked.</summary>
    Liked,

    /// <summary>The thumb down has been clicked.</summary>
    Disliked
}
