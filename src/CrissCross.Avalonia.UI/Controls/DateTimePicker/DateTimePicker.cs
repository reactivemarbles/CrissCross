// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that allows the user to select both a date and time.
/// </summary>
public class DateTimePicker : StackPanel
{
    /// <summary>
    /// Property for <see cref="SelectedDate"/>.
    /// </summary>
    public static readonly StyledProperty<DateTimeOffset?> SelectedDateProperty =
        AvaloniaProperty.Register<DateTimePicker, DateTimeOffset?>(nameof(SelectedDate));

    private readonly global::Avalonia.Controls.DatePicker _datePicker;
    private readonly global::Avalonia.Controls.TimePicker _timePicker;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimePicker"/> class.
    /// </summary>
    public DateTimePicker()
    {
        Orientation = Orientation.Horizontal;
        Spacing = 4;

        _datePicker = new global::Avalonia.Controls.DatePicker();
        _timePicker = new global::Avalonia.Controls.TimePicker();

        Children.Add(_datePicker);
        Children.Add(_timePicker);

        _datePicker.PropertyChanged += (s, e) =>
        {
            if (e.Property == global::Avalonia.Controls.DatePicker.SelectedDateProperty)
            {
                UpdateSelectedDate();
            }
        };

        _timePicker.PropertyChanged += (s, e) =>
        {
            if (e.Property == global::Avalonia.Controls.TimePicker.SelectedTimeProperty)
            {
                UpdateSelectedDate();
            }
        };
    }

    /// <summary>
    /// Gets or sets the selected date and time.
    /// </summary>
    public DateTimeOffset? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

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
