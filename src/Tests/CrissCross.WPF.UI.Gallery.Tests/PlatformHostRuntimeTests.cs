// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Threading;
using CrissCross.WPF;
using ReactiveUI;
using FormsHost = CrissCross.WinForms.ViewModelRoutedViewHost;
using FormsUserControl = System.Windows.Forms.UserControl;
using WpfHost = CrissCross.WPF.ViewModelRoutedViewHost;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Exercises the Windows platform navigation hosts and WebView composition wrapper.</summary>
public sealed class PlatformHostRuntimeTests
{
    /// <summary>The assigned browser zoom factor.</summary>
    private const double BrowserZoomFactor = 1.25D;

    /// <summary>Verifies equivalent resolved-navigation behavior in the WPF and WinForms hosts.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationHosts_WhenResolvedViewsNavigate_MaintainHistoryAndContent()
    {
        var snapshot = await RunOnStaThreadAsync(ExerciseNavigationHosts);

        await Assert.That(snapshot.WpfCanNavigateBack).IsTrue();
        await Assert.That(snapshot.WpfContentViewModel).IsSameReferenceAs(snapshot.ExpectedViewModel);
        await Assert.That(snapshot.FormsCanNavigateBack).IsTrue();
        await Assert.That(snapshot.FormsContentViewModel).IsSameReferenceAs(snapshot.ExpectedViewModel);
    }

    /// <summary>Verifies that the WebView2 wrapper projects its safe pre-initialization property surface.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WebView2Wpf_BeforeCoreInitialization_RetainsConfigurationAndDisposes()
    {
        var snapshot = await RunOnStaThreadAsync(ExerciseWebView);

        await Assert.That(snapshot.AllowExternalDrop).IsFalse();
        await Assert.That(snapshot.AutoDispose).IsFalse();
        await Assert.That(snapshot.ReloadRequiresInitialization).IsTrue();
        await Assert.That(snapshot.StopRequiresInitialization).IsTrue();
        await Assert.That(snapshot.ZoomFactor).IsEqualTo(BrowserZoomFactor);
        await Assert.That(snapshot.Background).IsEqualTo(System.Drawing.Color.Navy);
        await Assert.That(snapshot.Foreground).IsEqualTo(System.Drawing.Color.White);
        await Assert.That(snapshot.Content).IsSameReferenceAs(snapshot.Overlay);
    }

    /// <summary>Exercises both platform navigation hosts.</summary>
    /// <returns>The observable navigation state.</returns>
    private static NavigationHostSnapshot ExerciseNavigationHosts()
    {
        IScheduler previousMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
        try
        {
            return ExerciseNavigationHostsSynchronously();
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = previousMainThreadScheduler;
        }
    }

    /// <summary>Exercises both platform hosts using the test-scoped immediate reactive scheduler.</summary>
    /// <returns>The observable navigation state.</returns>
    private static NavigationHostSnapshot ExerciseNavigationHostsSynchronously()
    {
        var firstViewModel = new TestViewModel();
        var secondViewModel = new TestViewModel();

        using var wpfHost = new WpfHost { HostName = "WpfGalleryTests", NavigateBackIsEnabled = true };
        new NavigationOwner(wpfHost.HostName).SetMainNavigationHost(wpfHost);
        wpfHost.Setup();
        wpfHost.Navigate(
            new NavigationResolution(
                firstViewModel,
                new WpfTestView(firstViewModel),
                null,
                "first",
                NavigationType.New));
        wpfHost.Navigate(
            new NavigationResolution(
                secondViewModel,
                new WpfTestView(secondViewModel),
                null,
                "second",
                NavigationType.New));
        DrainDispatcher();
        var wpfCanNavigateBack = wpfHost.CanNavigateBack == true;
        var wpfContentViewModel = ((WpfTestView)wpfHost.Content).ViewModel;

        using var formsHost = new FormsHost { HostName = "FormsGalleryTests", NavigateBackIsEnabled = true };
        new NavigationOwner(formsHost.HostName).SetMainNavigationHost(formsHost);
        NavigationResolution firstFormsResolution = new(
            firstViewModel,
            new FormsTestView(firstViewModel),
            null,
            "first",
            NavigationType.New);
        formsHost.Navigate(firstFormsResolution);
        NavigationResolution secondFormsResolution = new(
            secondViewModel,
            new FormsTestView(secondViewModel),
            null,
            "second",
            NavigationType.New);
        formsHost.Navigate(secondFormsResolution);
        DrainDispatcher();
        var formsCanNavigateBack = formsHost.CanNavigateBack == true;
        var formsContentViewModel = ((FormsTestView)formsHost.Content!).ViewModel;

        return new(
            secondViewModel,
            wpfCanNavigateBack,
            wpfContentViewModel,
            formsCanNavigateBack,
            formsContentViewModel);
    }

    /// <summary>Runs queued reactive host notifications on the current Windows dispatcher.</summary>
    private static void DrainDispatcher() =>
        Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ApplicationIdle);

    /// <summary>Exercises safe WebView wrapper configuration without starting the Edge runtime.</summary>
    /// <returns>The observable wrapper state.</returns>
    private static WebViewSnapshot ExerciseWebView()
    {
        var overlay = new System.Windows.Controls.Border();
        using var browser = new WebView2Wpf
        {
            AllowExternalDrop = false,
            AutoDispose = false,
            Content = overlay,
            DefaultBackgroundColor = System.Drawing.Color.Navy,
            DesignModeForegroundColor = System.Drawing.Color.White,
            ZoomFactor = BrowserZoomFactor,
        };

        browser.GoBack();
        browser.GoForward();
        bool reloadRequiresInitialization = ThrowsBeforeCoreInitialization(browser.Reload);
        bool stopRequiresInitialization = ThrowsBeforeCoreInitialization(browser.Stop);

        return new(
            browser.AllowExternalDrop,
            browser.AutoDispose,
            reloadRequiresInitialization,
            stopRequiresInitialization,
            browser.ZoomFactor,
            browser.DefaultBackgroundColor,
            browser.DesignModeForegroundColor,
            browser.Content,
            overlay);
    }

    /// <summary>Invokes an operation that requires the WebView2 core and captures its documented pre-init failure.</summary>
    /// <param name="operation">The WebView2 operation.</param>
    /// <returns><c>true</c> when the operation requires core initialization.</returns>
    private static bool ThrowsBeforeCoreInitialization(Action operation)
    {
        try
        {
            operation();
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    /// <summary>Runs work on a Windows STA thread.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The platform action.</param>
    /// <returns>A task that completes with the action result.</returns>
    private static Task<TResult> RunOnStaThreadAsync<TResult>(Func<TResult> action)
    {
        var completion = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(
            () =>
            {
                try
                {
                    completion.SetResult(action());
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }

    /// <summary>Provides a concrete routed view model.</summary>
    private sealed class TestViewModel : RxObject;

    /// <summary>Provides a WPF view for resolved navigation.</summary>
    /// <param name="viewModel">The initial view model.</param>
    private sealed class WpfTestView(TestViewModel viewModel) : System.Windows.Controls.UserControl, IViewFor<TestViewModel>
    {
        /// <inheritdoc/>
        public TestViewModel? ViewModel { get; set; } = viewModel;

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>Provides a WinForms view for resolved navigation.</summary>
    /// <param name="viewModel">The initial view model.</param>
    private sealed class FormsTestView(TestViewModel viewModel) : FormsUserControl, IViewFor<TestViewModel>
    {
        /// <inheritdoc/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestViewModel? ViewModel { get; set; } = viewModel;

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>Provides the navigation-owner contract required to register platform hosts.</summary>
    /// <param name="Name">The navigation host name.</param>
    private sealed record NavigationOwner(string? Name) : ISetNavigation;

    /// <summary>Captures cross-platform navigation results.</summary>
    /// <param name="ExpectedViewModel">The view model expected to remain active.</param>
    /// <param name="WpfCanNavigateBack">Whether WPF reported back navigation.</param>
    /// <param name="WpfContentViewModel">The WPF content view model.</param>
    /// <param name="FormsCanNavigateBack">Whether WinForms reported back navigation.</param>
    /// <param name="FormsContentViewModel">The WinForms content view model.</param>
    private sealed record NavigationHostSnapshot(
        TestViewModel ExpectedViewModel,
        bool WpfCanNavigateBack,
        TestViewModel? WpfContentViewModel,
        bool FormsCanNavigateBack,
        TestViewModel? FormsContentViewModel);

    /// <summary>Captures the safe WebView wrapper surface.</summary>
    /// <param name="AllowExternalDrop">Whether external drop remains enabled.</param>
    /// <param name="AutoDispose">Whether automatic disposal remains enabled.</param>
    /// <param name="ReloadRequiresInitialization">Whether reload reports its core initialization requirement.</param>
    /// <param name="StopRequiresInitialization">Whether stop reports its core initialization requirement.</param>
    /// <param name="ZoomFactor">The configured zoom factor.</param>
    /// <param name="Background">The configured background.</param>
    /// <param name="Foreground">The configured design-mode foreground.</param>
    /// <param name="Content">The configured overlay content.</param>
    /// <param name="Overlay">The expected overlay.</param>
    private sealed record WebViewSnapshot(
        bool AllowExternalDrop,
        bool AutoDispose,
        bool ReloadRequiresInitialization,
        bool StopRequiresInitialization,
        double ZoomFactor,
        Color Background,
        Color Foreground,
        object Content,
        object Overlay);
}
