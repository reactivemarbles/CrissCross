// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Provides data for the <see cref="ContentDialog.ButtonClicked"/> event.</summary>
/// <param name="button">The button that was clicked.</param>
public class ContentDialogButtonClickEventArgs(ContentDialogButton button) : RoutedEventArgs
{
    /// <summary>Gets or sets the button that was clicked.</summary>
    public ContentDialogButton Button { get; set; } = button;
}
