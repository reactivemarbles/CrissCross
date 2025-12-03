// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Specifies identifiers to indicate the return value of a <see cref="ContentDialog"/>.
/// </summary>
public enum ContentDialogResult
{
    /// <summary>
    /// No button was tapped.
    /// </summary>
    None,

    /// <summary>
    /// The primary button was tapped by the user.
    /// </summary>
    Primary,

    /// <summary>
    /// The secondary button was tapped by the user.
    /// </summary>
    Secondary
}
