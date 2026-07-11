// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.Builder;

namespace CrissCross.WPF.Test;

/// <summary>Interaction logic for App.xaml.</summary>
public partial class App : Application
{
    /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        // This will prevent multiple instances of the application from running at the same time.
        Make.SingleInstance("MyUniqueAppName ddd81fc8-9107-4e33-b848-cac4c3ec3d2a");
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
        base.OnStartup(e);
    }
}
