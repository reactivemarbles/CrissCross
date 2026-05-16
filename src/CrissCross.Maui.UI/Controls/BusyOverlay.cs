// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Presents regional busy state over arbitrary content without replacing the content layout.
/// </summary>
public class BusyOverlay : ContentView
{
    /// <summary>
    /// Bindable property for <see cref="Operation"/>.
    /// </summary>
    public static readonly BindableProperty OperationProperty = BindableProperty.Create(
        nameof(Operation),
        typeof(BusyOperation),
        typeof(BusyOverlay),
        propertyChanged: OnOperationChanged);

    /// <summary>
    /// Bindable property for <see cref="IsBusy"/>.
    /// </summary>
    public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(
        nameof(IsBusy),
        typeof(bool),
        typeof(BusyOverlay));

    /// <summary>
    /// Gets or sets the busy operation displayed by the overlay.
    /// </summary>
    public BusyOperation? Operation
    {
        get => (BusyOperation?)GetValue(OperationProperty);
        set => SetValue(OperationProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the overlay is visible.
    /// </summary>
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    private static void OnOperationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BusyOverlay overlay)
        {
            overlay.SetValue(IsBusyProperty, newValue is BusyOperation { IsActive: true });
        }
    }
}
