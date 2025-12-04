// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that allows a user to pick a date from a calendar display.
/// </summary>
public class CalendarDatePicker : global::Avalonia.Controls.Button
{
    /// <summary>
    /// Property for <see cref="IsCalendarOpen"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsCalendarOpenProperty = AvaloniaProperty.Register<CalendarDatePicker, bool>(
        nameof(IsCalendarOpen), false);

    /// <summary>
    /// Property for <see cref="IsTodayHighlighted"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsTodayHighlightedProperty = AvaloniaProperty.Register<CalendarDatePicker, bool>(
        nameof(IsTodayHighlighted), false);

    /// <summary>
    /// Property for <see cref="Date"/>.
    /// </summary>
    public static readonly StyledProperty<DateTime?> DateProperty = AvaloniaProperty.Register<CalendarDatePicker, DateTime?>(
        nameof(Date), null);

    /// <summary>
    /// Property for <see cref="FirstDayOfWeek"/>.
    /// </summary>
    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty = AvaloniaProperty.Register<CalendarDatePicker, DayOfWeek>(
        nameof(FirstDayOfWeek), DayOfWeek.Sunday);

    private Popup? _popup;

    /// <summary>
    /// Gets or sets a value indicating whether the current date is highlighted.
    /// </summary>
    public bool IsTodayHighlighted
    {
        get => GetValue(IsTodayHighlightedProperty);
        set => SetValue(IsTodayHighlightedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the calendar view of the <see cref="CalendarDatePicker"/> is currently shown.
    /// </summary>
    public bool IsCalendarOpen
    {
        get => GetValue(IsCalendarOpenProperty);
        set => SetValue(IsCalendarOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the day that is considered the beginning of the week.
    /// </summary>
    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    /// <summary>
    /// Gets or sets the date currently set in the calendar picker.
    /// </summary>
    public DateTime? Date
    {
        get => GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    /// <inheritdoc />
    protected override void OnClick()
    {
        base.OnClick();

        InitializePopup();

        SetCurrentValue(IsCalendarOpenProperty, !IsCalendarOpen);
    }

    /// <summary>
    /// Called when [popup opened].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected virtual void OnPopupOpened(object? sender, EventArgs e)
    {
        if (sender is not Popup popup)
        {
            return;
        }

        if (popup.Child is null)
        {
            return;
        }

        _ = popup.Child?.Focus();
    }

    /// <summary>
    /// Called when [selected dates changed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (IsCalendarOpen)
        {
            SetCurrentValue(IsCalendarOpenProperty, false);
        }
    }

    private void InitializePopup()
    {
        if (_popup is not null)
        {
            return;
        }

        var calendar = new global::Avalonia.Controls.Calendar();
        calendar.Bind(
            global::Avalonia.Controls.Calendar.SelectedDateProperty,
            new Binding(nameof(Date))
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });
        calendar.Bind(
            global::Avalonia.Controls.Calendar.IsTodayHighlightedProperty,
            new Binding(nameof(IsTodayHighlighted))
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });
        calendar.Bind(
            global::Avalonia.Controls.Calendar.FirstDayOfWeekProperty,
            new Binding(nameof(FirstDayOfWeek))
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });

        calendar.SelectedDatesChanged += OnSelectedDatesChanged;

        _popup = new Popup
        {
            PlacementTarget = this,
            Placement = PlacementMode.Bottom,
            Child = calendar,
            Focusable = false,
            IsOpen = false,
            VerticalOffset = 1D,
        };

        _popup.Bind(
            Popup.IsOpenProperty,
            new Binding(nameof(IsCalendarOpen))
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });
    }
}
