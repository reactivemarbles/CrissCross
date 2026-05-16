// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a themed alarm list surface.
/// </summary>
public class Alarms : System.Windows.Controls.Control
{
    /// <summary>
    /// Identifies the <see cref="AlarmItems"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlarmItemsProperty = DependencyProperty.Register(
        nameof(AlarmItems),
        typeof(object),
        typeof(Alarms),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the alarm items collection.
    /// </summary>
    public object? AlarmItems
    {
        get => GetValue(AlarmItemsProperty);
        set => SetValue(AlarmItemsProperty, value);
    }
}
