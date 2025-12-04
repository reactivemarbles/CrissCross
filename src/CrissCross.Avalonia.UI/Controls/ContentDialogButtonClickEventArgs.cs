// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// ContentDialogButtonClickEventArgs.
/// </summary>
public class ContentDialogButtonClickEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentDialogButtonClickEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="source">The source.</param>
    public ContentDialogButtonClickEventArgs(RoutedEvent routedEvent, object source)
        : base(routedEvent, source)
    {
    }

    /// <summary>
    /// Gets the button.
    /// </summary>
    public ContentDialogButton Button { get; init; }
}
