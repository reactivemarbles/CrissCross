// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents a themed alarm list surface.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Alarms : System.Windows.Controls.Control
{
    /// <summary>Identifies the <see cref="AlarmItems"/> dependency property.</summary>
    public static readonly DependencyProperty AlarmItemsProperty = DependencyProperty.Register(
        nameof(AlarmItems),
        typeof(object),
        typeof(Alarms),
        new(null));

    /// <summary>Gets or sets the alarm items collection.</summary>
    public object? AlarmItems
    {
        get => GetValue(AlarmItemsProperty);
        set => SetValue(AlarmItemsProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
