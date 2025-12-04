// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// ContentDialogClosingEventArgs.
/// </summary>
public class ContentDialogClosingEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentDialogClosingEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="source">The source.</param>
    public ContentDialogClosingEventArgs(RoutedEvent routedEvent, object source)
        : base(routedEvent, source)
    {
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    public ContentDialogResult Result { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ContentDialogClosingEventArgs"/> is cancel.
    /// </summary>
    public bool Cancel { get; set; }
}
