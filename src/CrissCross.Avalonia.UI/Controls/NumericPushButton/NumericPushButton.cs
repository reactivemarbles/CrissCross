// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a button with numeric increment/decrement functionality.
/// </summary>
public class NumericPushButton : Button
{
    /// <summary>
    /// Property for <see cref="NumericValue"/>.
    /// </summary>
    public static readonly StyledProperty<double> NumericValueProperty =
        AvaloniaProperty.Register<NumericPushButton, double>(nameof(NumericValue), 0.0);

    /// <summary>
    /// Gets or sets the numeric value.
    /// </summary>
    public double NumericValue
    {
        get => GetValue(NumericValueProperty);
        set => SetValue(NumericValueProperty, value);
    }
}
