// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the <see cref="ContentDialog.ButtonClicked"/> event.
/// </summary>
public class ContentDialogButtonClickEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentDialogButtonClickEventArgs"/> class.
    /// </summary>
    /// <param name="button">The button that was clicked.</param>
    public ContentDialogButtonClickEventArgs(ContentDialogButton button)
    {
        Button = button;
    }

    /// <summary>
    /// Gets or sets the button that was clicked.
    /// </summary>
    public ContentDialogButton Button { get; set; }
}
