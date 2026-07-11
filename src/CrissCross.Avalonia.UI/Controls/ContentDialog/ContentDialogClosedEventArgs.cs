// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Provides data for the <see cref="ContentDialog.Closed"/> event.</summary>
public class ContentDialogClosedEventArgs : RoutedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ContentDialogClosedEventArgs"/> class.</summary>
    /// <param name="result">The dialog result.</param>
    public ContentDialogClosedEventArgs(ContentDialogResult result)
    {
        Result = result;
    }

    /// <summary>Gets the dialog result.</summary>
    public ContentDialogResult Result { get; }
}
