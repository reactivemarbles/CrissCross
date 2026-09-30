// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
            static (context, services) =>
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
        _tracker = _host.Services.GetService<Tracker>();
    }

    /// <summary>Occurs when the application is closing.</summary>
    /// <param name="e">The event data.</param>
    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
        base.OnExit(e);
    }

    /// <summary>Occurs when the application is loading.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        _tracker
            ?.Configure(new TrackingRequest<MainWindow>())
            .Id(
                static w => w.Name,
                $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
            .Properties(static w => ValueTuple.Create(w.Height, w.Width, w.Left, w.Top, w.WindowState))
            .PersistOn(static w => nameof(w.Closing))
            .StopTrackingOn(static w => nameof(w.Closing));

        await _host.StartAsync();
    }
}
