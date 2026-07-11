// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays and edits a shared date/time range snapshot.</summary>
public class DateTimeRangePicker : ContentView
{
    /// <summary>Bindable property for <see cref="Range"/>.</summary>
    public static readonly BindableProperty RangeProperty = BindableProperty.Create(
        nameof(Range),
        typeof(DateTimeRange),
        typeof(DateTimeRangePicker));

    /// <summary>Bindable property for <see cref="ApplyRangeCommand"/>.</summary>
    public static readonly BindableProperty ApplyRangeCommandProperty = BindableProperty.Create(
        nameof(ApplyRangeCommand),
        typeof(ICommand),
        typeof(DateTimeRangePicker));

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public DateTimeRange? Range
    {
        get => (DateTimeRange?)GetValue(RangeProperty);
        set => SetValue(RangeProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? ApplyRangeCommand
    {
        get => (ICommand?)GetValue(ApplyRangeCommandProperty);
        set => SetValue(ApplyRangeCommandProperty, value);
    }
}
