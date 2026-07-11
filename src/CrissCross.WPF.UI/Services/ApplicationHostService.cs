// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;
using Page = System.Windows.Controls.Page;
using Window = System.Windows.Window;

namespace CrissCross.WPF.UI;

/// <summary>Managed host of the application.</summary>
/// <typeparam name="TWindow">The application Window.</typeparam>
/// <typeparam name="TPage">The TPage type.</typeparam>
/// <seealso cref="IHostedService" />
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationHostService{TWindow , TPage}" /> class.
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
public sealed class ApplicationHostService<TWindow, TPage>(IServiceProvider serviceProvider) : IHostedService
    where TWindow : Window
    where TPage : Page
{
    /// <summary>Stores the _navigationWindow value.</summary>
    private INavigationWindow? _navigationWindow;

    /// <summary>Triggered when the application host is ready to start the service.</summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken) => await HandleActivationAsync();

    /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;

    /// <summary>Creates main window during activation.</summary>
    /// <returns>The result.</returns>
    private async Task HandleActivationAsync()
    {
        await Task.CompletedTask;

        if (!Application.Current.Windows.OfType<TWindow>().Any())
        {
            _navigationWindow = (serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow) ?? throw new InvalidOperationException("Navigation Window not registered.");

            _navigationWindow.ShowWindow();

            _ = _navigationWindow.Navigate(typeof(TPage));
        }

        await Task.CompletedTask;
    }
}
