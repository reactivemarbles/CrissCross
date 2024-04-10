// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CrissCross.WPF.UI
{
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

        private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimePicker"/> class.
        /// </summary>
        public DateTimePicker()
        {
            InitializeComponent();
            CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
            CalDisplay.SelectedDate = DateTime.Now.AddDays(1);

            ////BitmapSource ConvertGDI_To_WPF(Bitmap bm)
            ////{
            ////    BitmapSource? bms = null;
            ////    var h_bm = IntPtr.Zero;
            ////    h_bm = bm.GetHbitmap();
            ////    bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ////    bms.Freeze();
            ////    h_bm = IntPtr.Zero;
            ////    return bms;
            ////}

            ////Bitmap bitmap1 = Properties.Resources.DateTimePicker;
            ////bitmap1.MakeTransparent(System.Drawing.Color.Black);
            ////CalIco.Source = ConvertGDI_To_WPF(bitmap1);
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

        private void CalDisplay_SelectedDatesChanged(object sender, EventArgs e)
        {
            var hours = (Hours?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
            var minutes = (Min?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
            var timeSpan = TimeSpan.Parse(hours + ":" + minutes);
            if (CalDisplay.SelectedDate.Value.Date == DateTime.Today.Date && timeSpan.CompareTo(DateTime.Now.TimeOfDay) < 0)
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

        private void Time_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalDisplay_SelectedDatesChanged(sender, e);
        }

        private void CalDisplay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // that it's not necessary to click twice after opening the calendar  https://stackoverflow.com/q/6024372
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }
    }
}
