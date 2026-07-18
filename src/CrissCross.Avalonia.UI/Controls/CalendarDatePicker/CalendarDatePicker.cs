// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a control that allows a user to pick a date from a calendar display.</summary>
public class CalendarDatePicker : global::Avalonia.Controls.Button
{
    /// <summary>Property for <see cref="IsCalendarOpen"/>.</summary>
    public static readonly StyledProperty<bool> IsCalendarOpenProperty = AvaloniaProperty.Register<
        CalendarDatePicker,
        bool
    >(nameof(IsCalendarOpen), false);

    /// <summary>Property for <see cref="IsTodayHighlighted"/>.</summary>
    public static readonly StyledProperty<bool> IsTodayHighlightedProperty = AvaloniaProperty.Register<
        CalendarDatePicker,
        bool
    >(nameof(IsTodayHighlighted), true);

    /// <summary>Property for <see cref="Date"/>.</summary>
    public static readonly StyledProperty<DateTimeOffset?> DateProperty = AvaloniaProperty.Register<
        CalendarDatePicker,
        DateTimeOffset?
    >(nameof(Date));

    /// <summary>Property for <see cref="FirstDayOfWeek"/>.</summary>
    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty = AvaloniaProperty.Register<
        CalendarDatePicker,
        DayOfWeek
    >(nameof(FirstDayOfWeek), DayOfWeek.Sunday);

    /// <summary>Provides the _popup member.</summary>
    private Popup? _popup;

    /// <summary>Provides the _calendar member.</summary>
    private global::Avalonia.Controls.Calendar? _calendar;

    /// <summary>Gets or sets a value indicating whether the current date is highlighted.</summary>
    public bool IsTodayHighlighted
    {
        get => GetValue(IsTodayHighlightedProperty);
        set => SetValue(IsTodayHighlightedProperty, value);
    }

    /// <summary>Gets or sets whether the calendar view of the CalendarDatePicker is currently shown.</summary>
    public bool IsCalendarOpen
    {
        get => GetValue(IsCalendarOpenProperty);
        set => SetValue(IsCalendarOpenProperty, value);
    }

    /// <summary>Gets or sets the day that is considered the beginning of the week.</summary>
    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    /// <summary>Gets or sets the date currently set in the calendar picker.</summary>
    public DateTimeOffset? Date
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

    /// <inheritdoc />
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Initialize popup after the control is loaded to ensure visual tree is ready
        InitializePopup();
    }

    /// <summary>Called when [selected dates changed].</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_calendar?.SelectedDate is not null)
        {
            var selectedDate = DateTime.SpecifyKind(_calendar.SelectedDate.Value, DateTimeKind.Unspecified);
            SetCurrentValue(
                DateProperty,
                new DateTimeOffset(selectedDate, TimeZoneInfo.Local.GetUtcOffset(selectedDate)));
        }

        if (!IsCalendarOpen)
        {
            return;
        }

        SetCurrentValue(IsCalendarOpenProperty, false);
    }

    /// <summary>Provides the InitializePopup member.</summary>
    private void InitializePopup()
    {
        if (_popup is not null)
        {
            return;
        }

        var isTodayHighlighted = IsTodayHighlighted;
        var firstDayOfWeek = FirstDayOfWeek;

        _calendar = new global::Avalonia.Controls.Calendar
        {
            IsTodayHighlighted = isTodayHighlighted,
            FirstDayOfWeek = firstDayOfWeek,
            SelectedDate = Date?.DateTime,
        };

        var calendar = _calendar;
        _ = this.GetObservable(DateProperty).Subscribe(SyncCalendarDate);
        _ = this.GetObservable(IsTodayHighlightedProperty).Subscribe(SyncCalendarTodayHighlight);
        _ = calendar
            .GetObservable(global::Avalonia.Controls.Calendar.IsTodayHighlightedProperty)
            .Subscribe(SyncPickerTodayHighlight);
        _ = this.GetObservable(FirstDayOfWeekProperty).Subscribe(SyncCalendarFirstDayOfWeek);
        _ = calendar
            .GetObservable(global::Avalonia.Controls.Calendar.FirstDayOfWeekProperty)
            .Subscribe(SyncPickerFirstDayOfWeek);

        _calendar.SelectedDatesChanged += OnSelectedDatesChanged;

        _popup = new Popup
        {
            PlacementTarget = this,
            Placement = PlacementMode.Bottom,
            Child = _calendar,
            IsLightDismissEnabled = true,
            IsOpen = false,
            VerticalOffset = 1D,
        };

        var popup = _popup;
        _ = this.GetObservable(IsCalendarOpenProperty).Subscribe(SyncPopupOpenState);
        _ = popup.GetObservable(Popup.IsOpenProperty).Subscribe(SyncPickerOpenState);

        // Add popup to visual tree
        var adornerLayer = AdornerLayer.GetAdornerLayer(this);
        if (adornerLayer is not null)
        {
            adornerLayer.Children.Add(_popup);
        }
        else if (this.GetVisualParent() is Panel parent)
        {
            // Fallback: add to parent panel
            parent.Children.Add(_popup);
        }
    }

    /// <summary>Synchronizes the calendar date from the picker.</summary>
    /// <param name="value">The selected date.</param>
    private void SyncCalendarDate(DateTimeOffset? value)
    {
        if (_calendar?.SelectedDate == value?.DateTime)
        {
            return;
        }

        _calendar!.SelectedDate = value?.DateTime;
    }

    /// <summary>Synchronizes the calendar highlight setting from the picker.</summary>
    /// <param name="value">The highlight setting.</param>
    private void SyncCalendarTodayHighlight(bool value)
    {
        if (_calendar?.IsTodayHighlighted == value)
        {
            return;
        }

        _calendar!.IsTodayHighlighted = value;
    }

    /// <summary>Synchronizes the picker highlight setting from the calendar.</summary>
    /// <param name="value">The highlight setting.</param>
    private void SyncPickerTodayHighlight(bool value)
    {
        if (IsTodayHighlighted == value)
        {
            return;
        }

        SetCurrentValue(IsTodayHighlightedProperty, value);
    }

    /// <summary>Synchronizes the calendar first day of week from the picker.</summary>
    /// <param name="value">The first day of week.</param>
    private void SyncCalendarFirstDayOfWeek(DayOfWeek value)
    {
        if (_calendar?.FirstDayOfWeek == value)
        {
            return;
        }

        _calendar!.FirstDayOfWeek = value;
    }

    /// <summary>Synchronizes the picker first day of week from the calendar.</summary>
    /// <param name="value">The first day of week.</param>
    private void SyncPickerFirstDayOfWeek(DayOfWeek value)
    {
        if (FirstDayOfWeek == value)
        {
            return;
        }

        SetCurrentValue(FirstDayOfWeekProperty, value);
    }

    /// <summary>Synchronizes the popup open state from the picker.</summary>
    /// <param name="value">The open state.</param>
    private void SyncPopupOpenState(bool value)
    {
        if (_popup?.IsOpen == value)
        {
            return;
        }

        _popup!.IsOpen = value;
    }

    /// <summary>Synchronizes the picker open state from the popup.</summary>
    /// <param name="value">The open state.</param>
    private void SyncPickerOpenState(bool value)
    {
        if (IsCalendarOpen == value)
        {
            return;
        }

        SetCurrentValue(IsCalendarOpenProperty, value);
    }
}
