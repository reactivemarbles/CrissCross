// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Primitives;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a collection of gauge controls for data visualization.</summary>
public class Gauges : TemplatedControl
{
    /// <summary>Property for <see cref="Value"/>.</summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<Gauges, double>(nameof(Value), 0.0);

    /// <summary>Gets or sets the gauge value.</summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
