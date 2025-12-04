// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the <see cref="ContentDialog.Closing"/> event.
/// </summary>
public class ContentDialogClosingEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentDialogClosingEventArgs"/> class.
    /// </summary>
    /// <param name="result">The dialog result.</param>
    public ContentDialogClosingEventArgs(ContentDialogResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Gets or sets the dialog result.
    /// </summary>
    public ContentDialogResult Result { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog closing should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }
}
