// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using ReactiveUI;
using Splat;
using CoreNavigationWindow = CrissCross.Avalonia.NavigationWindow;
using CoreRoutedViewHost = CrissCross.Avalonia.ViewModelRoutedViewHost;
using CoreTransitioningContentControl = CrissCross.Avalonia.ReactiveTransitioningContentControl;
using ReactiveNavigationResolution = CrissCross.Reactive.NavigationResolution;
using ReactiveNavigationWindow = CrissCross.Reactive.Avalonia.NavigationWindow;
using ReactiveRoutedViewHost = CrissCross.Reactive.Avalonia.ViewModelRoutedViewHost;
using ReactiveTransitioningContentControl = CrissCross.Reactive.Avalonia.ReactiveTransitioningContentControl;
using ReactiveUseNavigation = CrissCross.Reactive.IUseNavigation;

namespace CrissCross.NavigationView.Tests;

/// <summary>Covers Avalonia navigation window, routed host, transition, and theme resource behavior.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaNavigationHostTransitionCoverageTests
{
    /// <summary>The explicit host name used by the core navigation window.</summary>
    private const string CoreHostName = "core-window-host";

    /// <summary>The explicit host name used by the reactive navigation window.</summary>
    private const string ReactiveHostName = "reactive-window-host";

    /// <summary>The expected first-window history count after two navigation requests.</summary>
    private const int ExpectedSharedVisualHistoryCount = 2;

    /// <summary>Allows the short transition timer to finish on the native dispatcher.</summary>
    private static readonly TimeSpan AnimationCompletionWait = TimeSpan.FromMilliseconds(300);

    /// <summary>Verifies rapid content replacement completes without blocking or losing the latest visual.</summary>
    /// <param name="reactive">Whether to exercise the reactive package variant.</param>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task TransitioningControls_RapidReplacementDisplaysTheLatestContent(bool reactive)
    {
        ContentControl control = reactive ? new ReactiveTransitioningContentControl() : new CoreTransitioningContentControl();
        var lifetime = (IDisposable)control;
        var initial = new TextBlock { Text = "Initial content" };
        var latest = new TextBlock { Text = "Latest content" };
        control.Content = initial;
        var window = new Window { Content = control };
        var assemblyName = reactive ? "CrissCross.Avalonia.Reactive" : "CrissCross.Avalonia";
        window.Styles.Add((Styles)LoadStyles($"avares://{assemblyName}/Themes/Index.axaml"));
        try
        {
            window.Show();
            window.UpdateLayout();
            await Assert.That(initial.IsEffectivelyVisible).IsTrue();
            await Assert.That(TopLevel.GetTopLevel(initial)).IsSameReferenceAs(window);
            control.Content = new TextBlock { Text = "Intermediate content" };
            control.Content = latest;
            await Task.Delay(AnimationCompletionWait);
            window.UpdateLayout();
            await Assert.That(TopLevel.GetTopLevel(latest)).IsSameReferenceAs(window);
            await Assert.That(latest.IsEffectivelyVisible).IsTrue();
            await Assert.That(TopLevel.GetTopLevel(initial)).IsNull();
            await Assert.That(control is CoreTransitioningContentControl { IsDisposed: false } or ReactiveTransitioningContentControl { IsDisposed: false }).IsTrue();
            control.Content = new TextBlock { Text = "Dispose during transition" };
        }
        finally
        {
            lifetime.Dispose();
            await Task.Delay(AnimationCompletionWait);
            window.Close();
        }
    }

    /// <summary>Verifies named core and reactive navigation windows configure their routed frames.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task NavigationWindows_WhenInitializedWithNames_ConfigureMatchingNavigationFrames() => AvaloniaTestUiThread.RunAsync(static async () =>
    {
        var coreWindow = new TestCoreNavigationWindow { HostName = CoreHostName, NavigateBackIsEnabled = false };
        var reactiveWindow = new TestReactiveNavigationWindow { HostName = ReactiveHostName, NavigateBackIsEnabled = false };

        coreWindow.InitializeForTest();
        reactiveWindow.InitializeForTest();

        await Assert.That(coreWindow.NavigationFrame?.HostName).IsEqualTo(CoreHostName);
        await Assert.That(coreWindow.NavigationFrame?.Name).IsEqualTo(CoreHostName);
        await Assert.That(coreWindow.NavigationFrame?.NavigateBackIsEnabled ?? true).IsFalse();
        await Assert.That(reactiveWindow.NavigationFrame?.HostName).IsEqualTo(ReactiveHostName);
        await Assert.That(reactiveWindow.NavigationFrame?.Name).IsEqualTo(ReactiveHostName);
        await Assert.That(reactiveWindow.NavigationFrame?.NavigateBackIsEnabled ?? true).IsFalse();
    });

    /// <summary>Verifies unnamed core and reactive navigation windows allocate stable generated host names.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task NavigationWindows_WhenInitializedWithoutNames_AllocateGeneratedNavigationHostNames() => AvaloniaTestUiThread.RunAsync(static async () =>
    {
        var coreWindow = new TestCoreNavigationWindow();
        var reactiveWindow = new TestReactiveNavigationWindow();

        coreWindow.InitializeForTest();
        reactiveWindow.InitializeForTest();

        var coreHostName = ((IUseNavigation)coreWindow).Name;
        var reactiveHostName = ((ReactiveUseNavigation)reactiveWindow).Name;

        await Assert.That(coreHostName!).StartsWith("__crisscross_navhost_NavigationWindow_", StringComparison.Ordinal);
        await Assert.That(reactiveHostName!).StartsWith("__crisscross_navhost_NavigationWindow_", StringComparison.Ordinal);
        await Assert.That(coreWindow.NavigationFrame?.HostName).IsEqualTo(coreHostName);
        await Assert.That(reactiveWindow.NavigationFrame?.HostName).IsEqualTo(reactiveHostName);
    });

    /// <summary>Verifies closing one window that shares a visual frame alias keeps the other window's unique host channel registered.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task NavigationWindows_WithSharedVisualFrameAlias_CloseOneKeepsOtherHostRegistered() => AvaloniaTestUiThread.RunAsync(static async () =>
    {
        const string visualHostName = "mainNavHost";
        const string firstHostName = "mainNavHost-first";
        const string secondHostName = "mainNavHost-second";
        var firstWindow = new TestCoreNavigationWindow { HostName = firstHostName };
        var secondWindow = new TestCoreNavigationWindow { HostName = secondHostName };

        firstWindow.InitializeForTest();
        secondWindow.InitializeForTest();
        var firstFrame = firstWindow.NavigationFrame!;
        var secondFrame = secondWindow.NavigationFrame!;
        firstFrame.Name = visualHostName;
        firstFrame.HostName = visualHostName;
        secondFrame.Name = visualHostName;
        secondFrame.HostName = visualHostName;

        var previousMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        firstFrame.ViewLocator = new WindowNavigationViewLocator();
        secondFrame.ViewLocator = new WindowNavigationViewLocator();
        RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;

        try
        {
            AppLocator.CurrentMutable.UnregisterAll<WindowNavigationViewModel>();
            AppLocator.CurrentMutable.RegisterConstant(new WindowNavigationViewModel());
            firstWindow.SetMainNavigationHost(firstFrame);
            secondWindow.SetMainNavigationHost(secondFrame);

            firstWindow.NavigateToView(new NavigationKeyRequest<WindowNavigationViewModel>());
            secondWindow.CloseForTest();
            firstWindow.NavigateToView(new NavigationKeyRequest<WindowNavigationViewModel>());
            firstWindow.CloseForTest();

            await Assert.That(firstFrame.HostName).IsEqualTo(firstHostName);
            await Assert.That(secondFrame.HostName).IsEqualTo(secondHostName);
            await Assert.That(firstFrame.NavigationStack.Count).IsEqualTo(ExpectedSharedVisualHistoryCount);
            await Assert.That(secondFrame.NavigationStack).IsEmpty();
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = previousMainThreadScheduler;
            AppLocator.CurrentMutable.UnregisterAll<WindowNavigationViewModel>();
        }
    });

    /// <summary>Verifies routed hosts trim history while back navigation is disabled and publish their final state.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RoutedHosts_WhenBackNavigationIsDisabled_TrimHistoryAndReportNoBackTarget()
    {
        using var coreHost = new CoreRoutedViewHost { HostName = CoreHostName, NavigateBackIsEnabled = false };
        using var reactiveHost = new ReactiveRoutedViewHost { HostName = ReactiveHostName, NavigateBackIsEnabled = false };
        coreHost.NavigationStack.Add(typeof(CoreRoutedViewHost));
        coreHost.NavigationStack.Add(typeof(CoreNavigationWindow));
        reactiveHost.NavigationStack.Add(typeof(ReactiveRoutedViewHost));
        reactiveHost.NavigationStack.Add(typeof(ReactiveNavigationWindow));

        coreHost.Refresh();
        reactiveHost.Refresh();
        var coreBackTarget = coreHost.NavigateBack();
        var reactiveBackTarget = reactiveHost.NavigateBack();

        await Assert.That(coreHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(reactiveHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(coreBackTarget).IsNull();
        await Assert.That(reactiveBackTarget).IsNull();
        await Assert.That(coreHost.CanNavigateBack ?? true).IsFalse();
        await Assert.That(reactiveHost.CanNavigateBack ?? true).IsFalse();

        coreHost.ClearHistory();
        reactiveHost.ClearHistory();

        await Assert.That(coreHost.NavigationStack).IsEmpty();
        await Assert.That(reactiveHost.NavigationStack).IsEmpty();
    }

    /// <summary>Verifies both routed host variants reject absent resolved navigation requests before changing state.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RoutedHosts_WhenResolvedNavigationIsAbsent_ThrowArgumentNullException()
    {
        using var coreHost = new CoreRoutedViewHost { HostName = CoreHostName };
        using var reactiveHost = new ReactiveRoutedViewHost { HostName = ReactiveHostName };

        await Assert.That(() => coreHost.Navigate((NavigationResolution)null!)).Throws<ArgumentNullException>();
        await Assert.That(() => coreHost.NavigateAndReset((NavigationResolution)null!)).Throws<ArgumentNullException>();
        await Assert.That(() => reactiveHost.Navigate((ReactiveNavigationResolution)null!)).Throws<ArgumentNullException>();
        await Assert.That(() => reactiveHost.NavigateAndReset((ReactiveNavigationResolution)null!)).Throws<ArgumentNullException>();
        await Assert.That(coreHost.NavigationStack).IsEmpty();
        await Assert.That(reactiveHost.NavigationStack).IsEmpty();
    }

    /// <summary>Verifies transition controls handle content changes before attachment and remain safely disposable.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task TransitioningControls_WhenContentChangesWithoutVisualRoots_RemainUsableUntilDisposed()
    {
        var coreControl = new CoreTransitioningContentControl { Content = "core-before" };
        var reactiveControl = new ReactiveTransitioningContentControl { Content = "reactive-before" };

        coreControl.Content = "core-after";
        reactiveControl.Content = "reactive-after";

        await Assert.That(coreControl.Content).IsEqualTo("core-after");
        await Assert.That(reactiveControl.Content).IsEqualTo("reactive-after");
        await Assert.That(coreControl.IsDisposed).IsFalse();
        await Assert.That(reactiveControl.IsDisposed).IsFalse();

        coreControl.Dispose();
        reactiveControl.Dispose();

        await Assert.That(coreControl.IsDisposed).IsTrue();
        await Assert.That(reactiveControl.IsDisposed).IsTrue();
    }

    /// <summary>Verifies core, reactive, UI, and reactive UI style dictionaries can be loaded by public resource URI.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PublicThemeResources_WhenLoadedFromAssemblyUris_ReturnStyleCollections()
    {
        var coreStyles = LoadStyles("avares://CrissCross.Avalonia/Themes/Index.axaml");
        var reactiveStyles = LoadStyles("avares://CrissCross.Avalonia.Reactive/Themes/Index.axaml");
        var uiStyles = LoadStyles("avares://CrissCross.Avalonia.UI/Themes/FluentWindow.axaml");
        var reactiveUiStyles = LoadStyles("avares://CrissCross.Avalonia.UI.Reactive/Themes/FluentWindow.axaml");

        await Assert.That(coreStyles).IsTypeOf<Styles>();
        await Assert.That(reactiveStyles).IsTypeOf<Styles>();
        await Assert.That(uiStyles).IsTypeOf<Styles>();
        await Assert.That(reactiveUiStyles).IsTypeOf<Styles>();
    }

    /// <summary>Loads a public Avalonia style resource by its assembly URI.</summary>
    /// <param name="resourceUri">The public resource URI.</param>
    /// <returns>The resource declared by the URI.</returns>
    private static object LoadStyles(string resourceUri) => AvaloniaXamlLoader.Load(new(resourceUri));

    /// <summary>Simple view model used by the public navigation lifecycle regression.</summary>
    private sealed class WindowNavigationViewModel : RxObject;

    /// <summary>Simple view used by the public navigation lifecycle regression.</summary>
    private sealed class WindowNavigationView : Control, IViewFor<WindowNavigationViewModel>
    {
        /// <summary>Gets or sets the strongly typed view model.</summary>
        public WindowNavigationViewModel? ViewModel { get; set; }

        /// <summary>Gets or sets the weakly typed view model.</summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (WindowNavigationViewModel?)value;
        }
    }

    /// <summary>View locator used by the public navigation lifecycle regression.</summary>
    private sealed class WindowNavigationViewLocator : IViewLocator
    {
        /// <inheritdoc/>
        public IViewFor<TViewModel> ResolveView<TViewModel>()
            where TViewModel : class => (IViewFor<TViewModel>)(object)ResolveTypedView<TViewModel>();

        /// <inheritdoc/>
        public IViewFor<TViewModel> ResolveView<TViewModel>(string? contract)
            where TViewModel : class => (IViewFor<TViewModel>)(object)ResolveTypedView<TViewModel>();

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? instance) => ResolveUntypedView(instance);

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? instance, string? contract) => ResolveUntypedView(instance);

        /// <summary>Resolves a typed test view.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>The resolved view.</returns>
        private static WindowNavigationView ResolveTypedView<TViewModel>()
            where TViewModel : class => typeof(TViewModel) == typeof(WindowNavigationViewModel)
                ? new WindowNavigationView()
                : throw new InvalidOperationException($"Unsupported view model type {typeof(TViewModel).FullName}.");

        /// <summary>Resolves an untyped test view.</summary>
        /// <param name="instance">The view model.</param>
        /// <returns>The resolved view.</returns>
        private static WindowNavigationView? ResolveUntypedView(object? instance) =>
            instance is WindowNavigationViewModel ? new WindowNavigationView() : null;
    }

    /// <summary>Exposes protected core navigation-window initialization for verification.</summary>
    private sealed class TestCoreNavigationWindow : CoreNavigationWindow
    {
        /// <summary>Initializes the navigation window.</summary>
        public void InitializeForTest() => OnInitialized();

        /// <summary>Closes the navigation window for lifecycle tests.</summary>
        public void CloseForTest() => OnClosed(EventArgs.Empty);
    }

    /// <summary>Exposes protected reactive navigation-window initialization for verification.</summary>
    private sealed class TestReactiveNavigationWindow : ReactiveNavigationWindow
    {
        /// <summary>Initializes the navigation window.</summary>
        public void InitializeForTest() => OnInitialized();
    }
}
