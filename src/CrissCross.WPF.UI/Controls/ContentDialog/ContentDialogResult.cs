// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Specifies identifiers to indicate the return value of a <see cref="ContentDialog"/>.</summary>
public enum ContentDialogResult
{
    /// <summary>No button was tapped.</summary>
    None,

    /// <summary>The primary button was tapped by the user.</summary>
    Primary,

    /// <summary>The secondary button was tapped by the user.</summary>
    Secondary,
}
