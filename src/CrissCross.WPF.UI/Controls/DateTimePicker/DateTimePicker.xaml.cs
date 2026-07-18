// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Interaction logic for DateTimePicker.xaml.</summary>
public partial class DateTimePicker : UserControl
{
    /// <summary>The selected date property.</summary>
    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
        nameof(SelectedDate),
        typeof(DateTimeOffset),
        typeof(DateTimePicker),
        new FrameworkPropertyMetadata(DateTimeOffset.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>The display date start property.</summary>
    public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(
        nameof(DisplayDateStart),
        typeof(DateTimeOffset),
        typeof(DateTimePicker),
        new PropertyMetadata(DateTimeOffset.MinValue, DateChanged));

    /// <summary>The display date end property.</summary>
    public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(
        nameof(DisplayDateEnd),
        typeof(DateTimeOffset),
        typeof(DateTimePicker),
        new PropertyMetadata(DateTimeOffset.Now, DateChanged));

    /// <summary>Provides the DateTimeFormat member.</summary>
    private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

    /// <summary>Initializes a new instance of the <see cref="DateTimePicker"/> class.</summary>
    public DateTimePicker()
    {
        InitializeComponent();
        CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
        CalDisplay.SelectedDate = DateTime.UtcNow;
    }

    /// <summary>Gets or sets the selected date.</summary>
    /// <value>
    /// The selected date.
    /// </value>
    public DateTimeOffset SelectedDate
    {
        get => (DateTimeOffset)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>Gets or sets the display date start.</summary>
    /// <value>
    /// The display date start.
    /// </value>
    public DateTimeOffset DisplayDateStart
    {
        get => (DateTimeOffset)GetValue(DisplayDateStartProperty);
        set => SetValue(DisplayDateStartProperty, value);
    }

    /// <summary>Gets or sets the display date end.</summary>
    /// <value>
    /// The display date end.
    /// </value>
    public DateTimeOffset DisplayDateEnd
    {
        get => (DateTimeOffset)GetValue(DisplayDateEndProperty);
        set => SetValue(DisplayDateEndProperty, value);
    }

    /// <summary>Provides the SaveTime_Click member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected void SaveTime_Click(object sender, RoutedEventArgs e)
    {
        CalDisplay_SelectedDatesChanged(SaveTime, EventArgs.Empty);
        PopUpCalendarButton.IsChecked = false;
    }

    /// <summary>Provides the DateChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void DateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not DateTimePicker dateTimePicker)
        {
            return;
        }

        dateTimePicker.CoerceDisplayDateRange();
        dateTimePicker.CoerceSelectedDate();

        dateTimePicker.CalDisplay_SelectedDatesChanged(null, EventArgs.Empty);
    }

    /// <summary>Determines whether two values represent the same calendar date.</summary>
    /// <param name="left">The first date.</param>
    /// <param name="right">The second date.</param>
    /// <returns><c>true</c> when the calendar dates match; otherwise, <c>false</c>.</returns>
    private static bool IsSameDate(DateTime left, DateTime right) =>
        left.Year == right.Year && left.Month == right.Month && left.Day == right.Day;

    /// <summary>Ensures the display date range is valid.</summary>
    private void CoerceDisplayDateRange() =>
        DisplayDateEnd = DisplayDateStart > DisplayDateEnd ? DisplayDateStart : DisplayDateEnd;

    /// <summary>Ensures the selected date remains within the display range.</summary>
    private void CoerceSelectedDate()
    {
        SelectedDate = DisplayDateStart > SelectedDate ? DisplayDateStart : SelectedDate;
        SelectedDate = DisplayDateEnd < SelectedDate ? DisplayDateEnd : SelectedDate;
    }

    /// <summary>Provides the CalDisplay_SelectedDatesChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void CalDisplay_SelectedDatesChanged(object? sender, EventArgs e)
    {
        var timeSpan = CoerceSelectedTime(GetSelectedTime());
        if (CalDisplay.SelectedDate is not { } selectedDate)
        {
            return;
        }

        var date =
            new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 0, 0, 0, selectedDate.Kind)
            + timeSpan;
        DateDisplay.Text = date.ToString(DateTimeFormat);
        SelectedDate = new(date);
    }

    /// <summary>Gets the selected time.</summary>
    /// <returns>The selected time.</returns>
    private TimeSpan GetSelectedTime()
    {
        var hours = (Hours?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
        var minutes = (Min?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
        return TimeSpan.Parse(hours + ":" + minutes);
    }

    /// <summary>Coerces the selected time when today's selected time has passed.</summary>
    /// <param name="timeSpan">The selected time.</param>
    /// <returns>The coerced selected time.</returns>
    private TimeSpan CoerceSelectedTime(TimeSpan timeSpan) =>
        ShouldUseSelectedTime(timeSpan) ? timeSpan : TimeSpan.FromHours(DateTime.Now.Hour + 1);

    /// <summary>Determines whether the selected time can be used as-is.</summary>
    /// <param name="timeSpan">The selected time.</param>
    /// <returns><c>true</c> when the selected time can be used as-is; otherwise, <c>false</c>.</returns>
    private bool ShouldUseSelectedTime(TimeSpan timeSpan) =>
        !IsSameDate(CalDisplay.SelectedDate!.Value, DateTime.Today) || timeSpan.CompareTo(DateTime.Now.TimeOfDay) >= 0;

    /// <summary>Provides the Time_SelectionChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void Time_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        CalDisplay_SelectedDatesChanged(sender, e);

    /// <summary>Provides the CalDisplay_PreviewMouseUp member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void CalDisplay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (Mouse.Captured is not CalendarItem)
        {
            return;
        }

        _ = Mouse.Capture(null);
    }
}
