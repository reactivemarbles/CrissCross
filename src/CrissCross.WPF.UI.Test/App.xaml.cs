// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
                // Register Main window View Model.
                services.AddSingleton<MainWindowViewModel>();

                // Views and ViewModels
                services.AddSingleton<DashboardPage>().AddSingleton<DashboardViewModel>();
                services.AddSingleton<DataPage>().AddSingleton<DataViewModel>();
                services.AddSingleton<SettingsPage>().AddSingleton<SettingsViewModel>();

                // Configuration
                services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
            })
        .Build();

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
    private async void OnStartup(object sender, StartupEventArgs e) => await _host.StartAsync();

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
