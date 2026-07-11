// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CrissCross.WPF.UI;

/// <summary>Interaction logic for DateTimePicker.xaml.</summary>
public partial class DateTimePicker : UserControl
{
    /// <summary>The selected date property.</summary>
    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
        nameof(SelectedDate),
        typeof(DateTime),
        typeof(DateTimePicker),
        new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>The display date start property.</summary>
    public static readonly DependencyProperty DisplayDateStartProperty =
        DependencyProperty.Register(
            nameof(DisplayDateStart),
            typeof(DateTime),
            typeof(DateTimePicker),
            new PropertyMetadata(DateTime.MinValue, DateChanged));

    /// <summary>The display date end property.</summary>
    public static readonly DependencyProperty DisplayDateEndProperty =
        DependencyProperty.Register(
            nameof(DisplayDateEnd),
            typeof(DateTime),
            typeof(DateTimePicker),
            new PropertyMetadata(DateTime.Now, DateChanged));

    /// <summary>Provides the DateTimeFormat member.</summary>
    private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

    /// <summary>Initializes a new instance of the <see cref="DateTimePicker"/> class.</summary>
    public DateTimePicker()
    {
        InitializeComponent();
        CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
        CalDisplay.SelectedDate = DateTime.Now;
    }

    /// <summary>Gets or sets the selected date.</summary>
    /// <value>
    /// The selected date.
    /// </value>
    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>Gets or sets the display date start.</summary>
    /// <value>
    /// The display date start.
    /// </value>
    public DateTime DisplayDateStart
    {
        get => (DateTime)GetValue(DisplayDateStartProperty);
        set => SetValue(DisplayDateStartProperty, value);
    }

    /// <summary>Gets or sets the display date end.</summary>
    /// <value>
    /// The display date end.
    /// </value>
    public DateTime DisplayDateEnd
    {
        get => (DateTime)GetValue(DisplayDateEndProperty);
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
        if (d is not DateTimePicker dateTimePicker)
        {
            return;
        }

        dateTimePicker.CoerceDisplayDateRange();
        dateTimePicker.CoerceSelectedDate();

        dateTimePicker.CalDisplay_SelectedDatesChanged(null, EventArgs.Empty);
    }

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
        var date = CalDisplay.SelectedDate.Value.Date + timeSpan;
        DateDisplay.Text = date.ToString(DateTimeFormat);
        SelectedDate = date;
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
        ShouldUseSelectedTime(timeSpan)
            ? timeSpan
            : TimeSpan.FromHours(DateTime.Now.Hour + 1);

    /// <summary>Determines whether the selected time can be used as-is.</summary>
    /// <param name="timeSpan">The selected time.</param>
    /// <returns><c>true</c> when the selected time can be used as-is; otherwise, <c>false</c>.</returns>
    private bool ShouldUseSelectedTime(TimeSpan timeSpan) =>
        CalDisplay.SelectedDate!.Value.Date != DateTime.Today.Date ||
        timeSpan.CompareTo(DateTime.Now.TimeOfDay) >= 0;

    /// <summary>Provides the Time_SelectionChanged member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void Time_SelectionChanged(object sender, SelectionChangedEventArgs e) => CalDisplay_SelectedDatesChanged(sender, e);

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
