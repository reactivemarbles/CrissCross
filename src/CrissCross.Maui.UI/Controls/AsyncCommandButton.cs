// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Represents a command button with an optional cancellation command for long-running async operations.</summary>
public class AsyncCommandButton : CommandButton
{
    /// <summary>Bindable property for <see cref="CancelCommand"/>.</summary>
    public static readonly BindableProperty CancelCommandProperty = BindableProperty.Create(
        nameof(CancelCommand),
        typeof(ICommand),
        typeof(AsyncCommandButton));

    /// <summary>Bindable property for <see cref="CancelText"/>.</summary>
    public static readonly BindableProperty CancelTextProperty = BindableProperty.Create(
        nameof(CancelText),
        typeof(string),
        typeof(AsyncCommandButton),
        "Cancel");

    /// <summary>Gets or sets the command used by templates to cancel the current operation.</summary>
    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    /// <summary>Gets or sets the text displayed for the cancellation affordance.</summary>
    public string? CancelText
    {
        get => (string?)GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }
}
