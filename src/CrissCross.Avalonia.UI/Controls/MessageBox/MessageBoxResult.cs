// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Specifies identifiers to indicate the return value of a <see cref="MessageBox"/>.</summary>
public enum MessageBoxResult
{
    /// <summary>No button was tapped.</summary>
    None,

    /// <summary>The primary button was tapped by the user.</summary>
    Primary,

    /// <summary>The secondary button was tapped by the user.</summary>
    Secondary
}
