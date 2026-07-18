// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Provides data for the <see cref="ContentDialog.Closing"/> event.</summary>
/// <param name="result">The dialog result.</param>
public class ContentDialogClosingEventArgs(ContentDialogResult result) : RoutedEventArgs
{
    /// <summary>Gets or sets the dialog result.</summary>
    public ContentDialogResult Result { get; set; } = result;

    /// <summary>Gets or sets a value indicating whether the dialog closing should be cancelled.</summary>
    public bool Cancel { get; set; }
}
