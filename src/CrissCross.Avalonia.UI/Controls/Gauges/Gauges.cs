// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a collection of gauge controls for data visualization.
/// </summary>
public class Gauges : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<Gauges, double>(nameof(Value), 0.0);

    /// <summary>
    /// Gets or sets the gauge value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
