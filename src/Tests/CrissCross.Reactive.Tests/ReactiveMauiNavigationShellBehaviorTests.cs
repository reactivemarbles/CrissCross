// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Reactive.MAUI;
using Microsoft.Maui.Controls;
using ReactiveUI;
using Splat;

namespace CrissCross.Reactive.Tests;

/// <summary>Exercises the reactive MAUI navigation shell without requiring platform page navigation.</summary>
public sealed class ReactiveMauiNavigationShellBehaviorTests
{
    /// <summary>Provides the shared navigation parameter used by overload tests.</summary>
    private const string NavigationParameter = "parameter";

    /// <summary>Provides the number of retained history items after refresh while back navigation is disabled.</summary>
    private const int RetainedHistoryCount = 1;

    /// <summary>Provides the history count used to verify refresh preserves enabled-back navigation.</summary>
    private const int PreservedHistoryCount = 2;

    /// <summary>Verifies setup rejects a shell that has no stable routing name.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Setup_RejectsBlankShellName()
    {
        using var shell = new NavigationShell();

        await Assert.That(() => shell.Setup()).Throws<ArgumentException>();
    }

    /// <summary>Verifies assigning a stable name registers the shell as its own routed navigation host.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NamedShell_RegistersAsRoutedHostAndExposesDefaultState()
    {
        const string shellName = "reactive-maui-shell-registration";
        using var shell = new NavigationShell { Name = shellName, };

        var isRegistered = ViewModelRoutedViewHostMixins.NavigationHost.TryGetValue(shellName, out var registeredHost);

        await Assert.That(shell.HostName).IsEqualTo(shellName);
        await Assert.That(shell.RequiresSetup).IsFalse();
        await Assert.That(shell.CanNavigateBack).IsFalse();
        await Assert.That(shell.NavigateBackIsEnabled).IsTrue();
        await Assert.That(shell.NavigationStack).IsEmpty();
        await Assert.That(isRegistered).IsTrue();
        await Assert.That(registeredHost).IsEqualTo(shell);
    }

    /// <summary>Verifies history refresh, clearing, and disabled back navigation publish the expected state.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HistoryOperations_RefreshClearAndPublishBackState()
    {
        const string shellName = "reactive-maui-shell-history";
        using var shell = new NavigationShell { Name = shellName, NavigateBackIsEnabled = false, };
        var backStates = new List<bool?>();
        using var subscription = shell.CanNavigateBackObservable.Subscribe(backStates.Add);
        shell.NavigationStack.Add(typeof(ReactiveMauiNavigationShellBehaviorTests));
        shell.NavigationStack.Add(typeof(NavigationShell));

        var backResult = shell.NavigateBack("ignored");
        shell.Refresh();
        shell.ClearHistory();
        _ = shell.NavigateBack();

        await Assert.That(shell.NavigationStack).IsEmpty();
        await Assert.That(backResult).IsNull();
        await Assert.That(backStates).Contains(true);
        await Assert.That(backStates).Contains(false);
    }

    /// <summary>Verifies refresh retains only the current entry while explicit history clearing removes it.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Refresh_WhenBackNavigationIsDisabled_RetainsOnlyCurrentHistoryEntry()
    {
        const string shellName = "reactive-maui-shell-refresh";
        using var shell = new NavigationShell { Name = shellName, NavigateBackIsEnabled = false, };
        shell.NavigationStack.Add(typeof(ReactiveMauiNavigationShellBehaviorTests));
        shell.NavigationStack.Add(typeof(NavigationShell));

        shell.Refresh();

        await Assert.That(shell.NavigationStack).Count().IsEqualTo(RetainedHistoryCount);
        await Assert.That(shell.NavigationStack[0]).IsEqualTo(typeof(NavigationShell));
    }

    /// <summary>Verifies refresh preserves history while the shell allows back navigation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Refresh_WhenBackNavigationIsEnabled_PreservesHistory()
    {
        const string shellName = "reactive-maui-shell-enabled-refresh";
        using var shell = new NavigationShell { Name = shellName, NavigateBackIsEnabled = true, };
        shell.NavigationStack.Add(typeof(ReactiveMauiNavigationShellBehaviorTests));
        shell.NavigationStack.Add(typeof(NavigationShell));

        shell.Refresh();

        await Assert.That(shell.NavigationStack).Count().IsEqualTo(PreservedHistoryCount);
    }

    /// <summary>Verifies resolved navigation APIs reject missing resolutions before attempting platform navigation.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_RejectsMissingResolution()
    {
        const string shellName = "reactive-maui-shell-resolution-guard";
        using var shell = new NavigationShell { Name = shellName, };

        await Assert.That(() => shell.Navigate((NavigationResolution)null!)).Throws<ArgumentNullException>();
        await Assert.That(() => shell.NavigateAndReset((NavigationResolution)null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies shell page projection preserves pages and ignores non-page values.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PageProjection_PreservesPagesAndRejectsNonPages()
    {
        var page = new ContentPage();
        object nonPage = new();
        using var shell = new NavigationShellProbe();

        await Assert.That(shell.ProjectPage(page)).IsEqualTo(page);
        await Assert.That(shell.ProjectPage(nonPage)).IsNull();
    }

    /// <summary>Verifies all public navigation overloads publish safely when view resolution is intentionally unavailable.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationOverloads_WithNamedHost_PublishRequestsAndAssignResolvedViews()
    {
        const string shellName = "reactive-maui-shell-overloads";
        using var shell = new NavigationShell { Name = shellName, ViewLocator = null };
        var viewModel = new TestViewModel();
        var view = new TestView();
        var resolution = new NavigationResolution(viewModel, view, "resolved", NavigationParameter, NavigationType.New);

        shell.Navigate(viewModel, "typed", NavigationParameter);
        shell.Navigate((IRxObject)viewModel, "runtime", NavigationParameter);
        shell.NavigateAndReset(viewModel, "typed-reset", NavigationParameter);
        shell.NavigateAndReset((IRxObject)viewModel, "runtime-reset", NavigationParameter);
        shell.Navigate(resolution);
        shell.NavigateAndReset(resolution);

        await Assert.That(view.ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(shell.NavigationStack).Count().IsEqualTo(RetainedHistoryCount);
        await Assert.That(shell.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
    }

    /// <summary>Verifies enabled back navigation publishes a request for a populated history stack.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigateBack_WithEnabledPopulatedHistory_PublishesAndUpdatesState()
    {
        const string shellName = "reactive-maui-shell-back";
        using var shell = new NavigationShell { Name = shellName, CanNavigateBack = true, NavigateBackIsEnabled = true };
        using var unregistered = new UnregisteredBackViewModel();
        shell.NavigationStack.Add(unregistered.GetType());
        shell.NavigationStack.Add(typeof(NavigationShell));

        var result = shell.NavigateBack(NavigationParameter);

        await Assert.That(result).IsNull();
        await Assert.That(shell.CanNavigateBack).IsTrue();
    }

    /// <summary>Verifies shell disposal is idempotent.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Dispose_WhenRepeated_RemainsSafe()
    {
        var shell = new NavigationShell();

        shell.Dispose();
        shell.Dispose();

        await Assert.That(shell.NavigationStack).IsEmpty();
    }

    /// <summary>Verifies setup processes accepted resolved navigation, emits current targets, and resets history.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Setup_AcceptedResolvedNavigation_EmitsTargetsAndResetsHistory()
    {
        const string shellName = "reactive-maui-shell-accepted-navigation";
        using var shell = new NavigationShell { Name = shellName, };
        using var firstView = new TestNavigationView();
        using var secondView = new TestNavigationView();
        using var resetView = new TestNavigationView();
        using var first = new TestViewModel();
        using var second = new TestViewModel();
        using var reset = new TestViewModel();
        var currentTargets = new List<INotifiyRoutableViewModel>();
        using var subscription = shell.CurrentViewModel.Subscribe(currentTargets.Add);

        shell.Setup();
        shell.Navigate(new(first, firstView, null, "first", NavigationType.New));
        shell.Navigate(new(second, secondView, null, "second", NavigationType.New));
        shell.NavigateAndReset(new(reset, resetView, null, "reset", NavigationType.New));

        await Assert.That(currentTargets).Contains(first);
        await Assert.That(currentTargets).Contains(second);
        await Assert.That(currentTargets).Contains(reset);
        await Assert.That(shell.NavigationStack).Count().IsEqualTo(RetainedHistoryCount);
        await Assert.That(shell.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
        await Assert.That(resetView.ViewModel).IsSameReferenceAs(reset);
    }

    /// <summary>Verifies setup honors cancelled routed requests without adding an entry to navigation history.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Setup_CancelledResolvedNavigation_DoesNotChangeHistory()
    {
        const string shellName = "reactive-maui-shell-cancelled-navigation";
        ViewModelRoutedViewHostMixins.ResultNavigating[shellName] = new();
        using var cancellation = ViewModelRoutedViewHostMixins.ResultNavigating[shellName]
            .Subscribe(static request => request.Cancel = true);
        using var shell = new NavigationShell { Name = shellName, };
        using var view = new TestNavigationView();
        using var viewModel = new TestViewModel();

        shell.Setup();
        shell.Navigate(new(viewModel, view, null, "cancel", NavigationType.New));

        await Assert.That(shell.NavigationStack).IsEmpty();
        await Assert.That(view.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>Verifies back navigation restores the registered preceding target and trims its history entry.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Setup_BackNavigation_RestoresPreviousRegisteredTarget()
    {
        const string shellName = "reactive-maui-shell-back-navigation";
        using var shell = new NavigationShell { Name = shellName, };
        using var firstView = new TestNavigationView();
        using var secondView = new TestNavigationView();
        using var first = new TestViewModel();
        using var second = new TestViewModel();
        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>();
        AppLocator.CurrentMutable.RegisterConstant(first);

        try
        {
            shell.Setup();
            shell.Navigate(new(first, firstView, null, "first", NavigationType.New));
            shell.Navigate(new(second, secondView, null, "second", NavigationType.New));

            var restored = shell.NavigateBack("back");

            await Assert.That(restored).IsSameReferenceAs(first);
            await Assert.That(shell.NavigationStack).Count().IsEqualTo(RetainedHistoryCount);
            await Assert.That(shell.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
            await Assert.That(firstView.ViewModel).IsSameReferenceAs(first);
        }
        finally
        {
            AppLocator.CurrentMutable.UnregisterAll<TestViewModel>();
        }
    }

    /// <summary>Verifies views that own pending navigation setup publish through the shared pending signal.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_WithSetupNavigatingView_PublishesPendingRequest()
    {
        const string shellName = "reactive-maui-shell-pending-navigation";
        using var shell = new NavigationShell { Name = shellName, };
        using var view = new TestNavigationView { ISetupNavigating = true, };
        using var viewModel = new TestViewModel();
        var requests = new List<IViewModelNavigatingEventArgs>();
        using var subscription = ViewModelRoutedViewHostMixins.SetWhenNavigating.Subscribe(requests.Add);

        shell.Navigate(new(viewModel, view, null, "pending", NavigationType.New));

        await Assert.That(requests).Count().IsEqualTo(RetainedHistoryCount);
        await Assert.That(requests[0].To).IsSameReferenceAs(viewModel);
        await Assert.That(requests[0].View).IsSameReferenceAs(view);
    }

    /// <summary>Exposes protected page projection behavior for direct behavioral testing.</summary>
    private sealed class NavigationShellProbe : NavigationShell
    {
        /// <summary>Projects an object to a page using the shell implementation.</summary>
        /// <param name="value">The value to project.</param>
        /// <returns>The supplied page, when the value is a page.</returns>
        public Page? ProjectPage(object value) => ToPage(value);
    }

    /// <summary>Provides a routed view model for shell navigation.</summary>
    private sealed class TestViewModel : RxObject;

    /// <summary>Provides an intentionally unregistered back-navigation target.</summary>
    private sealed class UnregisteredBackViewModel : RxObject;

    /// <summary>Provides a resolved view without requiring a platform page host.</summary>
    private sealed class TestView : IViewFor<TestViewModel>
    {
        /// <inheritdoc/>
        public TestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    /// <summary>Provides a MAUI page that participates in the routed navigation lifecycle.</summary>
    private sealed class TestNavigationView : ContentPage, IViewFor<TestViewModel>, INotifiyNavigation
    {
        /// <inheritdoc/>
        public bool ISetupNavigatedTo { get; set; }

        /// <inheritdoc/>
        public bool ISetupNavigatedFrom { get; set; }

        /// <inheritdoc/>
        public bool ISetupNavigating { get; set; }

        /// <inheritdoc/>
        public CompositeDisposable CleanUp { get; } = new();

        /// <inheritdoc/>
        public TestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }

        /// <inheritdoc/>
        public void Dispose() => CleanUp.Dispose();
    }
}
