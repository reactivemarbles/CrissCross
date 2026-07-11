// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Presents regional busy state over arbitrary content without replacing the content layout.</summary>
public class BusyOverlay : System.Windows.Controls.ContentControl
{
    /// <summary>Property for <see cref="Operation"/>.</summary>
    public static readonly DependencyProperty OperationProperty = DependencyProperty.Register(
        nameof(Operation),
        typeof(BusyOperation),
        typeof(BusyOverlay),
        new PropertyMetadata(null, OnOperationChanged));

    /// <summary>Property for <see cref="IsBusy"/>.</summary>
    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        nameof(IsBusy),
        typeof(bool),
        typeof(BusyOverlay),
        new PropertyMetadata(false));

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

    /// <summary>Provides the OnOperationChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnOperationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not BusyOverlay overlay)
        {
            return;
        }

        overlay.SetCurrentValue(IsBusyProperty, args.NewValue is BusyOperation operation && operation.IsActive);
    }
}
