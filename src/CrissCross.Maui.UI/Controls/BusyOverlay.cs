// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Presents regional busy state over arbitrary content without replacing the content layout.</summary>
public class BusyOverlay : ContentView
{
    /// <summary>Bindable property for <see cref="Operation"/>.</summary>
    public static readonly BindableProperty OperationProperty = BindableProperty.Create(
        nameof(Operation),
        typeof(BusyOperation),
        typeof(BusyOverlay),
        propertyChanged: static (bindable, _, newValue) => OnOperationChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="IsBusy"/>.</summary>
    public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(
        nameof(IsBusy),
        typeof(bool),
        typeof(BusyOverlay));

    /// <summary>Gets or sets the busy operation displayed by the overlay.</summary>
    public BusyOperation? Operation
    {
        get => (BusyOperation?)GetValue(OperationProperty);
        set => SetValue(OperationProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the overlay is visible.</summary>
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    /// <summary>Runs the operation changed operation.</summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnOperationChanged(BindableObject bindable, object newValue)
    {
        if (bindable is not BusyOverlay overlay)
        {
            return;
        }

        overlay.SetValue(IsBusyProperty, newValue is BusyOperation { IsActive: true });
    }
}
