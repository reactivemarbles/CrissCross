// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Test.Models;
using CrissCross.WPF.UI.Test.ViewModels;
using CrissCross.WPF.UI.Test.Views;
using CrissCross.WPF.UI.Test.Views.Pages;
using ReactiveUI.Builder;

namespace CrissCross.WPF.UI.Test;

/// <summary>Interaction logic for App.xaml.</summary>
public partial class App
{
    /// <summary>The application host and service provider.</summary>
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureCrissCrossForPageNavigation(new PageNavigationRegistration<MainWindow, DashboardPage>())
        .ConfigureServices(
            (context, services) =>
            {
                _ = services.AddSingleton<Tracker>();

                // Register Main window View Model.
                _ = services.AddSingleton<MainWindowViewModel>();

                // Views and ViewModels
                _ = services.AddSingleton<DashboardPage>().AddSingleton<DashboardViewModel>();
                _ = services.AddSingleton<DataPage>().AddSingleton<DataViewModel>();
                _ = services.AddSingleton<SettingsPage>().AddSingleton<SettingsViewModel>();
                _ = services.AddSingleton<LoginPage>().AddSingleton<LoginViewModel>();

                // Configuration
                _ = services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
            })
        .Build();

    /// <summary>Provides persisted window tracking.</summary>
    private readonly Tracker? _tracker;

    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    public App()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
        _tracker = GetService<Tracker>();
    }

    /// <summary>Gets registered service.</summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T? GetService<T>()
        where T : class => _host.Services.GetService(typeof(T)) as T;

    /// <summary>Occurs when the application is closing.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    /// <summary>Occurs when the application is loading.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        _tracker
            ?.Configure(new TrackingRequest<MainWindow>())
            .Id(
                w => w.Name,
                $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
            .Properties(w => ValueTuple.Create(w.Height, w.Width, w.Left, w.Top, w.WindowState))
            .PersistOn(w => nameof(w.Closing))
            .StopTrackingOn(w => nameof(w.Closing));

        await _host.StartAsync();
    }

    /// <summary>Occurs when an exception is thrown by an application but not handled.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more information, see the DispatcherUnhandledException API documentation.
    }
}
