// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Markup.Xaml;
using Avalonia.Styling;
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
public sealed class AvaloniaNavigationHostTransitionCoverageTests
{
    /// <summary>The explicit host name used by the core navigation window.</summary>
    private const string CoreHostName = "core-window-host";

    /// <summary>The explicit host name used by the reactive navigation window.</summary>
    private const string ReactiveHostName = "reactive-window-host";

    /// <summary>Verifies named core and reactive navigation windows configure their routed frames.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationWindows_WhenInitializedWithNames_ConfigureMatchingNavigationFrames()
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
    }

    /// <summary>Verifies unnamed core and reactive navigation windows allocate stable generated host names.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationWindows_WhenInitializedWithoutNames_AllocateGeneratedNavigationHostNames()
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
    }

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

    /// <summary>Exposes protected core navigation-window initialization for verification.</summary>
    private sealed class TestCoreNavigationWindow : CoreNavigationWindow
    {
        /// <summary>Initializes the navigation window.</summary>
        public void InitializeForTest() => OnInitialized();
    }

    /// <summary>Exposes protected reactive navigation-window initialization for verification.</summary>
    private sealed class TestReactiveNavigationWindow : ReactiveNavigationWindow
    {
        /// <summary>Initializes the navigation window.</summary>
        public void InitializeForTest() => OnInitialized();
    }
}
