// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Storage;
using CrissCross.WPF.UI.Test.Models;
using CrissCross.WPF.UI.Test.ViewModels;
using CrissCross.WPF.UI.Test.Views;
using CrissCross.WPF.UI.Test.Views.Pages;

namespace CrissCross.WPF.UI.Test;

/// <summary>
/// Interaction logic for App.xaml.
/// </summary>
public partial class App
{
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureCrissCrossForPageNavigation<MainWindow, DashboardPage>()
        .ConfigureServices(
            (context, services) =>
            {
                services.AddSingleton<Tracker>();

                // Register Main window View Model.
                services.AddSingleton<MainWindowViewModel>();

                // Views and ViewModels
                services.AddSingleton<DashboardPage>().AddSingleton<DashboardViewModel>();
                services.AddSingleton<DataPage>().AddSingleton<DataViewModel>();
                services.AddSingleton<SettingsPage>().AddSingleton<SettingsViewModel>();
                services.AddSingleton<LoginPage>().AddSingleton<LoginViewModel>();

                // Configuration
                services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
            })
        .Build();

    private readonly Tracker? _tracker;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App() => _tracker = GetService<Tracker>();

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T? GetService<T>()
        where T : class => _host.Services.GetService(typeof(T)) as T;

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        _tracker?.Configure<MainWindow>()
                .Id(w => w.Name, $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
                .Properties(w => new { w.Height, w.Width, w.Left, w.Top, w.WindowState })
                .PersistOn(w => nameof(w.Closing))
                .StopTrackingOn(w => nameof(w.Closing));

        await _host.StartAsync();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}
