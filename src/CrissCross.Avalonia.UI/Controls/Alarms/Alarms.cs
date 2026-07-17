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

/// <summary>Represents an alarm management control.</summary>
public class Alarms : TemplatedControl
{
    /// <summary>Property for <see cref="AlarmItems"/>.</summary>
    public static readonly StyledProperty<object?> AlarmItemsProperty =
        AvaloniaProperty.Register<Alarms, object?>(nameof(AlarmItems));

    /// <summary>Gets or sets the alarm items collection.</summary>
    public object? AlarmItems
    {
        get => GetValue(AlarmItemsProperty);
        set => SetValue(AlarmItemsProperty, value);
    }
}
