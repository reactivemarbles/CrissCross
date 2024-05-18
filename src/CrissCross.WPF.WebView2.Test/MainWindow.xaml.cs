// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows;

namespace CrissCross.WPF.WebView2.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _clickedXTimes = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            ////WebView2Wpf.Dispose();
            base.OnClosing(e);
        }

        private void WindowsXp_Click(object sender, RoutedEventArgs e)
        {
            Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
            WebView2Wpf.Source = new System.Uri("https://support.microsoft.com/en-gb/windows/windows-xp-support-has-ended-47b944b8-f4d3-82f2-9acc-21c79ee6ef5e");
        }

        private void Windows7_Click(object sender, RoutedEventArgs e)
        {
            Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
            WebView2Wpf.Source = new System.Uri("https://support.microsoft.com/en-us/windows/windows-7-system-requirements-df0900f2-3513-a851-13e7-0d50bc24e15f");
        }

        private void Windows10_Click(object sender, RoutedEventArgs e)
        {
            Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
            WebView2Wpf.Source = new System.Uri("https://www.microsoft.com/en-gb/software-download/windows10");
        }

        private void ReactiveUI_Click(object sender, RoutedEventArgs e)
        {
            Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
            WebView2Wpf.Source = new System.Uri("https://www.reactiveui.net/");
        }
    }
}
