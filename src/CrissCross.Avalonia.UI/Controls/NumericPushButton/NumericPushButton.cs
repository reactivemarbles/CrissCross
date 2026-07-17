// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a button with numeric increment/decrement functionality.</summary>
public class NumericPushButton : Button
{
    /// <summary>Property for <see cref="NumericValue"/>.</summary>
    public static readonly StyledProperty<double> NumericValueProperty =
        AvaloniaProperty.Register<NumericPushButton, double>(nameof(NumericValue), 0.0);

    /// <summary>Gets or sets the numeric value.</summary>
    public double NumericValue
    {
        get => GetValue(NumericValueProperty);
        set => SetValue(NumericValueProperty, value);
    }
}
