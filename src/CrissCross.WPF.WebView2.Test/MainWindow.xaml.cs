// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace CrissCross.WPF.WebView2.Test;

/// <summary>Interaction logic for MainWindow.xaml.</summary>
public partial class MainWindow : Window
{
    /// <summary>Counts how many sample navigation buttons have been clicked.</summary>
    private int _clickedXTimes;

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow() => InitializeComponent();

    /// <summary>Handles the Windows XP sample link click.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void WindowsXp_Click(object sender, RoutedEventArgs e)
    {
        Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
        WebView2Wpf.Source = new(
            "https://support.microsoft.com/en-gb/windows/"
                + "windows-xp-support-has-ended-47b944b8-f4d3-82f2-9acc-21c79ee6ef5e");
    }

    /// <summary>Handles the Windows 7 sample link click.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void Windows7_Click(object sender, RoutedEventArgs e)
    {
        Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
        WebView2Wpf.Source = new(
            "https://support.microsoft.com/en-us/windows/"
                + "windows-7-system-requirements-df0900f2-3513-a851-13e7-0d50bc24e15f");
    }

    /// <summary>Handles the Windows 10 sample link click.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void Windows10_Click(object sender, RoutedEventArgs e)
    {
        Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
        WebView2Wpf.Source = new("https://www.microsoft.com/en-gb/software-download/windows10");
    }

    /// <summary>Handles the ReactiveUI sample link click.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void ReactiveUI_Click(object sender, RoutedEventArgs e)
    {
        Greeting.Text = $"Hello CrissCross {_clickedXTimes++}";
        WebView2Wpf.Source = new("https://www.reactiveui.net/");
    }
}
