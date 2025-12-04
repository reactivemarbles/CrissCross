// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an alarm management control.
/// </summary>
public class Alarms : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="AlarmItems"/>.
    /// </summary>
    public static readonly StyledProperty<object?> AlarmItemsProperty =
        AvaloniaProperty.Register<Alarms, object?>(nameof(AlarmItems));

    /// <summary>
    /// Gets or sets the alarm items collection.
    /// </summary>
    public object? AlarmItems
    {
        get => GetValue(AlarmItemsProperty);
        set => SetValue(AlarmItemsProperty, value);
    }
}
