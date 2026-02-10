// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.Builder;

namespace CrissCross.WPF.Test;

/// <summary>
/// Interaction logic for App.xaml.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        // This will prevent multiple instances of the application from running at the same time.
        Make.SingleInstance("MyUniqueAppName ddd81fc8-9107-4e33-b848-cac4c3ec3d2a");
        RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
        base.OnStartup(e);
    }
}
