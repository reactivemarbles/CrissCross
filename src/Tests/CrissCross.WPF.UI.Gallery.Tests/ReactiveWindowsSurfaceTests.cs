// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Exercises the Windows reactive compatibility assemblies.</summary>
public sealed class ReactiveWindowsSurfaceTests
{
    /// <summary>The expected number of entries in a newly cleared navigation history.</summary>
    private const int EmptyHistoryCount = 0;

    /// <summary>Verifies the reactive WPF and WinForms navigation hosts expose equivalent defaults.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveNavigationHosts_WhenConstructed_ExposeExpectedSetupAndHistoryDefaults()
    {
        var snapshot = await RunOnStaThreadAsync(ExerciseReactiveHosts);

        await Assert.That(snapshot.WpfRequiresSetup).IsFalse();
        await Assert.That(snapshot.WpfHostName).IsEqualTo("ReactiveWpf");
        await Assert.That(snapshot.WpfHistoryCount).IsEqualTo(EmptyHistoryCount);
        await Assert.That(snapshot.FormsRequiresSetup).IsTrue();
        await Assert.That(snapshot.FormsHostName).IsEqualTo("ReactiveForms");
        await Assert.That(snapshot.FormsHistoryCount).IsEqualTo(EmptyHistoryCount);
    }

    /// <summary>Verifies the reactive WebView2 wrapper projects safe pre-initialization state.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveWebView2_WhenConfiguredBeforeInitialization_RetainsSafeProperties()
    {
        var snapshot = await RunOnStaThreadAsync(ExerciseReactiveWebView);

        await Assert.That(snapshot.AllowExternalDrop).IsFalse();
        await Assert.That(snapshot.AutoDispose).IsFalse();
        await Assert.That(snapshot.ReloadRequiresInitialization).IsTrue();
        await Assert.That(snapshot.StopRequiresInitialization).IsTrue();
        await Assert.That(snapshot.Content).IsSameReferenceAs(snapshot.Overlay);
    }

    /// <summary>Exercises the reactive navigation hosts through their public history surface.</summary>
    /// <returns>The observable host state.</returns>
    private static ReactiveHostSnapshot ExerciseReactiveHosts()
    {
        using var wpfHost = new CrissCross.Reactive.WPF.ViewModelRoutedViewHost { HostName = "ReactiveWpf", NavigateBackIsEnabled = true, };
        wpfHost.ClearHistory();
        _ = wpfHost.NavigateBack();

        using var formsHost = new CrissCross.Reactive.WinForms.ViewModelRoutedViewHost { HostName = "ReactiveForms", NavigateBackIsEnabled = true, };
        formsHost.ClearHistory();
        _ = formsHost.NavigateBack();

        return new(
            wpfHost.RequiresSetup,
            wpfHost.HostName,
            wpfHost.NavigationStack.Count,
            formsHost.RequiresSetup,
            formsHost.HostName,
            formsHost.NavigationStack.Count);
    }

    /// <summary>Exercises safe reactive WebView wrapper configuration without starting Edge.</summary>
    /// <returns>The observable wrapper state.</returns>
    private static ReactiveWebViewSnapshot ExerciseReactiveWebView()
    {
        var overlay = new System.Windows.Controls.Border();
        using var browser = new CrissCross.Reactive.WPF.WebView2Wpf { AllowExternalDrop = false, AutoDispose = false, Content = overlay, };

        browser.GoBack();
        browser.GoForward();
        bool reloadRequiresInitialization = ThrowsBeforeCoreInitialization(browser.Reload);
        bool stopRequiresInitialization = ThrowsBeforeCoreInitialization(browser.Stop);

        return new(
            browser.AllowExternalDrop,
            browser.AutoDispose,
            reloadRequiresInitialization,
            stopRequiresInitialization,
            browser.Content,
            overlay);
    }

    /// <summary>Invokes an operation that requires the WebView2 core and captures its pre-init failure.</summary>
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

    /// <summary>Captures reactive platform host state.</summary>
    /// <param name="WpfRequiresSetup">Whether the WPF host requires setup.</param>
    /// <param name="WpfHostName">The WPF host name.</param>
    /// <param name="WpfHistoryCount">The WPF history count.</param>
    /// <param name="FormsRequiresSetup">Whether the WinForms host requires setup.</param>
    /// <param name="FormsHostName">The WinForms host name.</param>
    /// <param name="FormsHistoryCount">The WinForms history count.</param>
    private sealed record ReactiveHostSnapshot(
        bool WpfRequiresSetup,
        string WpfHostName,
        int WpfHistoryCount,
        bool FormsRequiresSetup,
        string FormsHostName,
        int FormsHistoryCount);

    /// <summary>Captures reactive WebView wrapper state.</summary>
    /// <param name="AllowExternalDrop">Whether external drop is allowed.</param>
    /// <param name="AutoDispose">Whether auto-dispose is enabled.</param>
    /// <param name="ReloadRequiresInitialization">Whether reload reports its core initialization requirement.</param>
    /// <param name="StopRequiresInitialization">Whether stop reports its core initialization requirement.</param>
    /// <param name="Content">The configured content.</param>
    /// <param name="Overlay">The expected content.</param>
    private sealed record ReactiveWebViewSnapshot(
        bool AllowExternalDrop,
        bool AutoDispose,
        bool ReloadRequiresInitialization,
        bool StopRequiresInitialization,
        object Content,
        object Overlay);
}
