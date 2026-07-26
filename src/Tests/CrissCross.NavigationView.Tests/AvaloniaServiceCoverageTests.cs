// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI;
using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises navigation and snackbar service guard behavior without an application host.</summary>
public sealed class AvaloniaServiceCoverageTests
{
    /// <summary>Verifies services report absent host controls through their documented guards.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Services_WhenHostsAreNotConfigured_ThrowDocumentedExceptions()
    {
        var navigation = new NavigationService(new NullServiceProvider());
        var snackbar = new SnackbarService();
        var contentDialog = new ContentDialogService();

        await Assert.That(navigation.GetNavigationControl).Throws<ArgumentNullException>();
        await Assert.That(navigation.GoBack).Throws<ArgumentNullException>();
        await Assert.That(navigation.GoForward).Throws<ArgumentNullException>();
        await Assert.That(() => navigation.Navigate(typeof(global::Avalonia.Controls.TextBlock))).Throws<ArgumentNullException>();
        await Assert.That(snackbar.GetSnackbarPresenter).Throws<InvalidOperationException>();
        await Assert
            .That(() => snackbar.Show("title", "message", ControlAppearance.Primary, null, TimeSpan.Zero))
            .Throws<ArgumentNullException>();
        await Assert.That(contentDialog.GetContentPresenter).Throws<ArgumentNullException>();
        var dialog = new ContentDialog();
        await Assert
            .That(() => contentDialog.ShowAsync(dialog, CancellationToken.None))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies the theme service exposes the documented unsupported system-accent result.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ThemeService_WhenSettingSystemAccent_ReturnsFalse()
    {
        var service = new ThemeService();

        await Assert.That(service.SetSystemAccent()).IsFalse();
    }

    /// <summary>Provides a service provider that resolves no services.</summary>
    private sealed class NullServiceProvider : IServiceProvider
    {
        /// <inheritdoc/>
        public object? GetService(Type serviceType) => null;
    }
}
