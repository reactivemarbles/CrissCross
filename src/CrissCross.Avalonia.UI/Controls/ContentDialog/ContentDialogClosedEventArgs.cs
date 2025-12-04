// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the <see cref="ContentDialog.Closed"/> event.
/// </summary>
public class ContentDialogClosedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentDialogClosedEventArgs"/> class.
    /// </summary>
    /// <param name="result">The dialog result.</param>
    public ContentDialogClosedEventArgs(ContentDialogResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the dialog result.
    /// </summary>
    public ContentDialogResult Result { get; }
}
