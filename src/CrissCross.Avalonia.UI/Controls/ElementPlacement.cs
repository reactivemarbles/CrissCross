// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Decides where to put the element.</summary>
public enum ElementPlacement
{
    /// <summary>Puts the control element on the left.</summary>
    Left,

    /// <summary>Puts the control element on the right.</summary>
    Right
}
