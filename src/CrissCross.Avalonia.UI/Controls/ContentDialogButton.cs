// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Defines constants that specify the default button on a <see cref="ContentDialog"/>.</summary>
public enum ContentDialogButton
{
    /// <summary>The primary button is the default.</summary>
    Primary,

    /// <summary>The secondary button is the default.</summary>
    Secondary,

    /// <summary>The close button is the default.</summary>
    Close
}
