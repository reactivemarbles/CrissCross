// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CrissCross.WPF.UI;

/// <summary>
/// Interaction logic for DateTimePicker.xaml.
/// </summary>
public partial class DateTimePicker : UserControl
{
    /// <summary>
    /// The selected date property.
    /// </summary>
    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
        nameof(SelectedDate),
        typeof(DateTime),
        typeof(DateTimePicker),
        new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// The display date start property.
    /// </summary>
    public static readonly DependencyProperty DisplayDateStartProperty =
        DependencyProperty.Register(
            nameof(DisplayDateStart),
            typeof(DateTime),
            typeof(DateTimePicker),
            new PropertyMetadata(DateTime.MinValue, DateChanged));

    /// <summary>
    /// The display date end property.
    /// </summary>
    public static readonly DependencyProperty DisplayDateEndProperty =
        DependencyProperty.Register(
            nameof(DisplayDateEnd),
            typeof(DateTime),
            typeof(DateTimePicker),
            new PropertyMetadata(DateTime.Now, DateChanged));

    private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimePicker"/> class.
    /// </summary>
    public DateTimePicker()
    {
        InitializeComponent();
        CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
        CalDisplay.SelectedDate = DateTime.Now;
    }

    /// <summary>
    /// Gets or sets the selected date.
    /// </summary>
    /// <value>
    /// The selected date.
    /// </value>
    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the display date start.
    /// </summary>
    /// <value>
    /// The display date start.
    /// </value>
    public DateTime DisplayDateStart
    {
        get => (DateTime)GetValue(DisplayDateStartProperty);
        set => SetValue(DisplayDateStartProperty, value);
    }

    /// <summary>
    /// Gets or sets the display date end.
    /// </summary>
    /// <value>
    /// The display date end.
    /// </value>
    public DateTime DisplayDateEnd
    {
        get => (DateTime)GetValue(DisplayDateEndProperty);
        set => SetValue(DisplayDateEndProperty, value);
    }

    private static void DateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DateTimePicker dateTimePicker)
        {
            if (dateTimePicker.DisplayDateStart > dateTimePicker.DisplayDateEnd)
            {
                dateTimePicker.DisplayDateEnd = dateTimePicker.DisplayDateStart;
            }

            if (dateTimePicker.DisplayDateStart > dateTimePicker.SelectedDate)
            {
                dateTimePicker.SelectedDate = dateTimePicker.DisplayDateStart;
            }

            if (dateTimePicker.DisplayDateEnd < dateTimePicker.SelectedDate)
            {
                dateTimePicker.SelectedDate = dateTimePicker.DisplayDateEnd;
            }

            dateTimePicker.CalDisplay_SelectedDatesChanged(null, EventArgs.Empty);
        }
    }

    private void CalDisplay_SelectedDatesChanged(object? sender, EventArgs e)
    {
        var hours = (Hours?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
        var minutes = (Min?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
        var timeSpan = TimeSpan.Parse(hours + ":" + minutes);
        if (CalDisplay.SelectedDate!.Value.Date == DateTime.Today.Date && timeSpan.CompareTo(DateTime.Now.TimeOfDay) < 0)
        {
            timeSpan = TimeSpan.FromHours(DateTime.Now.Hour + 1);
        }

        var date = CalDisplay.SelectedDate.Value.Date + timeSpan;
        DateDisplay.Text = date.ToString(DateTimeFormat);
        SelectedDate = date;
    }

    private void SaveTime_Click(object sender, RoutedEventArgs e)
    {
        CalDisplay_SelectedDatesChanged(SaveTime, EventArgs.Empty);
        PopUpCalendarButton.IsChecked = false;
    }

    private void Time_SelectionChanged(object sender, SelectionChangedEventArgs e) => CalDisplay_SelectedDatesChanged(sender, e);

    private void CalDisplay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (Mouse.Captured is CalendarItem)
        {
            Mouse.Capture(null);
        }
    }
}
