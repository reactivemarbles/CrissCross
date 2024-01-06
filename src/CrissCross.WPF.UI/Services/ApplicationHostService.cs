// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace CrissCross.WPF.UI;
#pragma warning disable CA1812
/// <summary>
/// Managed host of the application.
/// </summary>
/// <typeparam name="TWindow">The application Window.</typeparam>
/// <typeparam name="TPage">The Start Page.</typeparam>
/// <seealso cref="IHostedService" />
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationHostService{TWindow , TPage}" /> class.
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
internal class ApplicationHostService<TWindow, TPage>(IServiceProvider serviceProvider) : IHostedService
    where TWindow : Window
    where TPage : Page
{
    private INavigationWindow? _navigationWindow;

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken) => await HandleActivationAsync();

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;

    /// <summary>
    /// Creates main window during activation.
    /// </summary>
    private async Task HandleActivationAsync()
    {
        await Task.CompletedTask;

        if (!Application.Current.Windows.OfType<TWindow>().Any())
        {
            _navigationWindow = (serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
            _navigationWindow!.ShowWindow();

            _navigationWindow.Navigate(typeof(TPage));
        }

        await Task.CompletedTask;
    }
}
#pragma warning restore CA1812
