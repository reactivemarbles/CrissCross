// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Layout;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a control that allows the user to select both a date and time.</summary>
public class DateTimePicker : StackPanel
{
    /// <summary>Property for <see cref="SelectedDate"/>.</summary>
    public static readonly StyledProperty<DateTimeOffset?> SelectedDateProperty =
        AvaloniaProperty.Register<DateTimePicker, DateTimeOffset?>(nameof(SelectedDate));

    /// <summary>Provides the _datePicker member.</summary>
    private readonly global::Avalonia.Controls.DatePicker _datePicker;

    /// <summary>Provides the _timePicker member.</summary>
    private readonly global::Avalonia.Controls.TimePicker _timePicker;

    /// <summary>Initializes a new instance of the <see cref="DateTimePicker"/> class.</summary>
    public DateTimePicker()
    {
        const double defaultSpacing = 4.0;

        Orientation = Orientation.Horizontal;
        Spacing = defaultSpacing;

        _datePicker = new();
        _timePicker = new();

        Children.Add(_datePicker);
        Children.Add(_timePicker);

        _datePicker.PropertyChanged += (s, e) =>
        {
            if (e.Property != global::Avalonia.Controls.DatePicker.SelectedDateProperty)
            {
                return;
            }

            UpdateSelectedDate();
        };

        _timePicker.PropertyChanged += (s, e) =>
        {
            if (e.Property != global::Avalonia.Controls.TimePicker.SelectedTimeProperty)
            {
                return;
            }

            UpdateSelectedDate();
        };
    }

    /// <summary>Gets or sets the selected date and time.</summary>
    public DateTimeOffset? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>Provides the UpdateSelectedDate member.</summary>
    private void UpdateSelectedDate()
    {
        if (_datePicker.SelectedDate.HasValue && _timePicker.SelectedTime.HasValue)
        {
            var date = _datePicker.SelectedDate.Value;
            var time = _timePicker.SelectedTime.Value;
            SelectedDate = new DateTimeOffset(
                date.Year,
                date.Month,
                date.Day,
                time.Hours,
                time.Minutes,
                time.Seconds,
                date.Offset);
        }
        else
        {
            SelectedDate = null;
        }
    }
}
