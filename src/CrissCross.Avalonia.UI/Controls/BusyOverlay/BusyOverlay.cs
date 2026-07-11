// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia;
using Avalonia.Controls;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Presents regional busy state over arbitrary content without replacing the content layout.</summary>
public class BusyOverlay : ContentControl
{
    /// <summary>Property for <see cref="Operation"/>.</summary>
    public static readonly StyledProperty<BusyOperation?> OperationProperty = AvaloniaProperty.Register<BusyOverlay, BusyOperation?>(
        nameof(Operation));

    /// <summary>Property for <see cref="IsBusy"/>.</summary>
    public static readonly StyledProperty<bool> IsBusyProperty = AvaloniaProperty.Register<BusyOverlay, bool>(
        nameof(IsBusy));

    /// <summary>Gets or sets the busy operation displayed by the overlay.</summary>
    public BusyOperation? Operation
    {
        get => GetValue(OperationProperty);
        set => SetValue(OperationProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the overlay is visible.</summary>
    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property != OperationProperty)
        {
            return;
        }

        SetCurrentValue(IsBusyProperty, change.GetNewValue<BusyOperation?>() is { IsActive: true });
    }
}
