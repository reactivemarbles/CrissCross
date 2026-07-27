// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Splat;

namespace CrissCross.Reactive.Tests;

/// <summary>Exercises public routed-host behavior through the reactive shim.</summary>
public sealed class ReactiveRoutedHostBehaviorTests
{
    /// <summary>Provides the history count at which backward navigation becomes available.</summary>
    private const int BackNavigationHistoryCount = 2;

    /// <summary>Provides the expected pending-event count for a matching view and host result signal.</summary>
    private const int ExpectedPendingEventCount = 2;

    /// <summary>Provides the requested page size for value-object coverage.</summary>
    private const int PageSize = 10;

    /// <summary>Provides the padded navigation contract used for normalization coverage.</summary>
    private const string PaddedContract = " contract ";

    /// <summary>Provides the normalized navigation contract used for normalization coverage.</summary>
    private const string Contract = "contract";

    /// <summary>Provides an alternate contract used for normalization coverage.</summary>
    private const string OtherContract = "other";

    /// <summary>Provides the shared status field key.</summary>
    private const string StatusField = "status";

    /// <summary>Verifies registration, aliases, setup, history and back-navigation notifications.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RegisteredHost_UsesNavigationAndHostAliasesForHistoryAndBackState()
    {
        const string navigationName = "reactive-navigation";
        const string hostName = "reactive-host";
        using var navigation = new NavigationOwner(navigationName);
        using var hosted = new HostedNavigationOwner();
        using var host = new RecordingHost(hostName, true);
        using var model = new TestViewModel();
        var backStates = new List<bool>();

        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>();
        AppLocator.CurrentMutable.RegisterConstant(model);

        navigation.SetMainNavigationHost(host);
        using var subscription = hosted.CanNavigateBack(navigationName).Subscribe(backStates.Add);
        hosted.NavigateToView(new NavigationKeyRequest<TestViewModel> { Options = new NavigationRequestOptions { HostName = navigationName, }, });
        hosted.NavigateToView(new NavigationKeyRequest<TestViewModel> { Options = new NavigationRequestOptions { HostName = hostName, }, });

        await Assert.That(host.SetupCount).IsEqualTo(1);
        await Assert.That(host.NavigationStack.Count).IsEqualTo(BackNavigationHistoryCount);
        await Assert.That(host.CanNavigateBack).IsTrue();
        await Assert.That(backStates).Contains(false);

        _ = hosted.NavigateBack(hostName, "back parameter");
        hosted.ClearHistory(navigationName);

        await Assert.That(host.LastBackParameter).IsEqualTo("back parameter");
        await Assert.That(host.NavigationStack).IsEmpty();
    }

    /// <summary>Verifies typed host extensions forward contracts and reset history.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TypedHostExtensions_ForwardContractParameterAndResolvedServices()
    {
        const string contract = "reactive-contract";
        var parameter = new object();
        using var host = new RecordingHost("typed-host", false);
        using var model = new TestViewModel();
        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>(contract);
        AppLocator.CurrentMutable.RegisterConstant(model, contract);

        host.Navigate(model, contract);
        host.NavigateAndReset(model);
        host.Navigate(new NavigationKeyRequest<TestViewModel> { Options = new NavigationRequestOptions { Contract = contract, Parameter = parameter, }, });
        host.NavigateAndReset(new NavigationKeyRequest<TestViewModel> { Options = new NavigationRequestOptions { Contract = contract, Parameter = parameter, }, });

        await Assert.That(host.LastViewModel).IsEqualTo(model);
        await Assert.That(host.LastContract).IsEqualTo(contract);
        await Assert.That(host.LastParameter).IsEqualTo(parameter);
        await Assert.That(host.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(host.NavigateBack()).IsNull();
    }

    /// <summary>Verifies public routed navigation validates required inputs and registered hosts.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RoutedNavigation_RejectsNullInputsAndMissingRegistrations()
    {
        ISetNavigation? setNavigation = null;
        IUseHostedNavigation? hostedNavigation = null;
        IViewModelRoutedViewHost? host = null;
        using var owner = new NavigationOwner("unregistered-reactive-host");

        await Assert.That(() => setNavigation!.SetMainNavigationHost(new RecordingHost("ignored", false)))
            .Throws<ArgumentNullException>();
        await Assert.That(() => owner.SetMainNavigationHost(host!)).Throws<ArgumentNullException>();
        await Assert.That(() => hostedNavigation!.ClearHistory("missing")).Throws<ArgumentNullException>();
        await Assert.That(() => owner.NavigateToView<TestViewModel>(null!)).Throws<ArgumentNullException>();
        await Assert.That(() => owner.NavigateToView(typeof(TestViewModel), null!)).Throws<ArgumentNullException>();
        await Assert.That(() => owner.ClearHistory()).Throws<KeyNotFoundException>();

        using var registeredHost = new RecordingHost("unregistered-reactive-host", false);
        owner.SetMainNavigationHost(registeredHost);

        await Assert.That(() => owner.NavigateToView(null!, new())).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies navigation views dispatch matching completion callbacks and renew per-view lifetimes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationViews_DispatchMatchingCompletionCallbacksAndRenewNewViewLifetime()
    {
        const string hostName = "callback-reactive-host";
        using var from = new RoutableViewModel("from-reactive");
        using var to = new RoutableViewModel("to-reactive");
        using var fromView = new NavigationView(from);
        using var toView = new NavigationView(to);
        var fromHandlerCount = 0;
        var toHandlerCount = 0;
        CompositeDisposable? firstLifetime = null;
        CompositeDisposable? secondLifetime = null;

        fromView.WhenNavigatedFrom(_ => fromHandlerCount++);
        toView.WhenNavigatedTo((_, lifetime) =>
        {
            toHandlerCount++;
            if (firstLifetime is null)
            {
                firstLifetime = lifetime;
                return;
            }

            secondLifetime = lifetime;
        });

        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(
            new ViewModelNavigationEventArgs(from, to, NavigationType.New, toView, hostName));
        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(
            new ViewModelNavigationEventArgs(to, from, NavigationType.New, fromView, hostName));

        await Assert.That(fromHandlerCount).IsGreaterThan(0);
        await Assert.That(toHandlerCount).IsGreaterThan(0);
        await Assert.That(from.NavigatedFromCount).IsGreaterThan(0);
        await Assert.That(to.NavigatedToCount).IsGreaterThan(0);
        await Assert.That(firstLifetime).IsNotNull();
        await Assert.That(secondLifetime).IsNotNull();
    }

    /// <summary>Verifies pending navigation invokes matching views and routes host-scoped results.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationViews_DispatchPendingCallbacksAndPublishHostResults()
    {
        const string hostName = "pending-reactive-host";
        using var from = new RoutableViewModel("pending-from");
        using var to = new RoutableViewModel("pending-to");
        using var view = new NavigationView(from);
        var handlerCount = 0;
        var resultCount = 0;
        ViewModelRoutedViewHostMixins.ResultNavigating[hostName] = new();
        using var resultSubscription = ViewModelRoutedViewHostMixins.ResultNavigating[hostName]
            .Subscribe(_ => resultCount++);

        view.WhenNavigating(args =>
        {
            handlerCount++;
            args.Cancel = true;
            return args;
        });

        var matching = new ViewModelNavigatingEventArgs(from, to, NavigationType.New, view, hostName);
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(matching);
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(
            new ViewModelNavigatingEventArgs(null, to, NavigationType.New, view, hostName));
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(null!);

        await Assert.That(handlerCount).IsGreaterThan(0);
        await Assert.That(from.NavigatingCount).IsGreaterThan(0);
        await Assert.That(matching.Cancel).IsTrue();
        await Assert.That(resultCount).IsEqualTo(ExpectedPendingEventCount);
    }

    /// <summary>Verifies primary and hosted overloads forward reset, type, and back-navigation parameters.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PrimaryAndHostedNavigation_ForwardAllPublicViewModelOverloads()
    {
        const string hostName = "overloads-reactive-host";
        const string contract = "overloads-reactive-contract";
        var parameter = new object();
        using var owner = new NavigationOwner(hostName);
        using var hosted = new HostedNavigationOwner();
        using var host = new RecordingHost(hostName, false);
        using var model = new TestViewModel();

        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>(contract);
        AppLocator.CurrentMutable.RegisterConstant(model, contract);
        owner.SetMainNavigationHost(host);

        owner.NavigateToView(new NavigationKeyRequest<TestViewModel> { Options = new NavigationRequestOptions { Contract = contract, Parameter = parameter, }, });
        owner.NavigateToView(typeof(TestViewModel), new NavigationRequestOptions { Contract = contract, Parameter = parameter, });
        hosted.NavigateToView(typeof(TestViewModel), new NavigationRequestOptions { HostName = hostName, Contract = contract, Parameter = parameter, });
        hosted.NavigateToViewAndClearHistory(new NavigationKeyRequest<TestViewModel> { Options = new NavigationRequestOptions { HostName = hostName, Contract = contract, Parameter = parameter, }, });
        owner.NavigateBack(parameter);
        _ = hosted.NavigateBack(hostName);

        await Assert.That(host.LastViewModel).IsEqualTo(model);
        await Assert.That(host.LastContract).IsEqualTo(contract);
        await Assert.That(host.LastParameter).IsEqualTo(parameter);
        await Assert.That(host.LastBackParameter).IsNull();
        await Assert.That(host.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Verifies runtime view-model navigation safely ignores an unregistered service.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RuntimeViewModelNavigation_UnregisteredServiceLeavesHostUntouched()
    {
        const string hostName = "unresolved-reactive-host";
        using var owner = new NavigationOwner(hostName);
        using var host = new RecordingHost(hostName, false);

        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>();
        owner.SetMainNavigationHost(host);
        owner.NavigateToView(typeof(TestViewModel));

        await Assert.That(host.LastViewModel).IsNull();
        await Assert.That(host.NavigationStack).IsEmpty();
    }

    /// <summary>Verifies typed navigator extensions preserve runtime keys and request options.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TypedNavigatorExtensions_ForwardViewModelAndViewKeys()
    {
        const string contract = "typed-navigator-contract";
        var parameter = new object();
        var navigator = new RecordingNavigator();
        var options = new NavigationRequestOptions { Contract = contract, Parameter = parameter, };

        using var viewModelSubscription = navigator
            .NavigateViewModel(new NavigationKeyRequest<TestViewModel> { Options = options, })
            .Subscribe(static _ => { });
        using var viewSubscription = navigator
            .NavigateView(new NavigationKeyRequest<NavigationView> { Options = options, })
            .Subscribe(static _ => { });

        await Assert.That(navigator.LastViewModelKey).IsEqualTo(typeof(TestViewModel));
        await Assert.That(navigator.LastViewKey).IsEqualTo(typeof(NavigationView));
        await Assert.That(navigator.LastOptions).IsEqualTo(options);
    }

    /// <summary>Verifies resolved primary navigation obtains a registered ViewModel/View pair.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_UsesRegisteredNavigatorAndForwardsContractAndParameter()
    {
        const string hostName = "resolved-reactive-host";
        const string contract = "resolved-reactive-contract";
        var parameter = new object();
        var registry = new NavigationRegistry();
        using var owner = new NavigationOwner(hostName);
        using var host = new ResolvedRecordingHost(hostName, false);

        RegisterResolvedPair(registry, contract);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        AppLocator.CurrentMutable.RegisterConstant<INavigationRegistry>(registry);
        IBidirectionalNavigator navigator = registry.CreateNavigator();
        AppLocator.CurrentMutable.RegisterConstant(navigator);
        owner.SetMainNavigationHost(host);

        owner.NavigateTo(new NavigationKeyRequest<ResolvedViewModel> { Options = new NavigationRequestOptions { Contract = contract, Parameter = parameter, }, });

        await Assert.That(host.LastResolution).IsNotNull();
        await Assert.That(host.LastResolution!.Contract).IsEqualTo(contract);
        await Assert.That(host.LastResolution.Parameter).IsEqualTo(parameter);
        await Assert.That(host.LastResolution.ViewModel).IsAssignableTo<ResolvedViewModel>();
        await Assert.That(host.LastResolution.View).IsAssignableTo<ResolvedView>();
    }

    /// <summary>Verifies resolved navigation creates a navigator from a registered registry when no navigator is supplied.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_UsesNavigationRegistryWhenNavigatorIsNotRegistered()
    {
        const string hostName = "registry-resolved-reactive-host";
        var registry = new NavigationRegistry();
        using var owner = new NavigationOwner(hostName);
        using var host = new ResolvedRecordingHost(hostName, false);

        RegisterResolvedPair(registry, null);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        AppLocator.CurrentMutable.RegisterConstant<INavigationRegistry>(registry);
        owner.SetMainNavigationHost(host);

        owner.NavigateTo(new NavigationKeyRequest<ResolvedViewModel>());

        await Assert.That(host.LastResolution).IsNotNull();
        await Assert.That(host.LastResolution!.ViewModel).IsAssignableTo<ResolvedViewModel>();
    }

    /// <summary>Verifies primary and hosted runtime navigation-key overloads resolve registered pairs.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RuntimeNavigationKeys_ResolveThroughPrimaryAndHostedOverloads()
    {
        const string hostName = "runtime-key-reactive-host";
        const string contract = "runtime-key-contract";
        var parameter = new object();
        var registry = new NavigationRegistry();
        using var owner = new NavigationOwner(hostName);
        using var hosted = new HostedNavigationOwner();
        using var host = new ResolvedRecordingHost(hostName, false);

        RegisterResolvedPair(registry, contract);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        IBidirectionalNavigator navigator = registry.CreateNavigator();
        AppLocator.CurrentMutable.RegisterConstant(navigator);
        owner.SetMainNavigationHost(host);

        owner.NavigateTo(typeof(ResolvedViewModel), new NavigationRequestOptions { Contract = contract, Parameter = parameter, });
        hosted.NavigateTo(typeof(ResolvedView), new NavigationRequestOptions { HostName = hostName, Contract = contract, Parameter = parameter, });

        await Assert.That(host.LastResolution).IsNotNull();
        await Assert.That(host.LastResolution!.Contract).IsEqualTo(contract);
        await Assert.That(host.LastResolution.Parameter).IsEqualTo(parameter);
        await Assert.That(host.LastResolution.View).IsAssignableTo<ResolvedView>();
    }

    /// <summary>Verifies blank registration names receive a stable generated host key and remain usable.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BlankHostRegistration_GeneratesUsableHostNameAndRunsRequiredSetup()
    {
        using var owner = new NavigationOwner(string.Empty);
        using var host = new RecordingHost(string.Empty, true);

        owner.SetMainNavigationHost(host);
        using var generatedNameOwner = new NavigationOwner(host.Name);
        generatedNameOwner.ClearHistory();
        generatedNameOwner.NavigateBack();

        await Assert.That(host.Name).IsNotEmpty();
        await Assert.That(host.SetupCount).IsEqualTo(1);
        await Assert.That(host.LastBackParameter).IsNull();
    }

    /// <summary>Verifies resolved navigation falls back from a ViewModel lookup to a registered view key.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_ViewKeyFallsBackFromViewModelLookup()
    {
        const string hostName = "resolved-view-key-reactive-host";
        var registry = new NavigationRegistry();
        using var owner = new NavigationOwner(hostName);
        using var host = new ResolvedRecordingHost(hostName, false);

        RegisterResolvedPair(registry, null);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        AppLocator.CurrentMutable.RegisterConstant<INavigationRegistry>(registry);
        IBidirectionalNavigator navigator = registry.CreateNavigator();
        AppLocator.CurrentMutable.RegisterConstant(navigator);
        owner.SetMainNavigationHost(host);

        owner.NavigateTo(new NavigationKeyRequest<ResolvedView>());

        await Assert.That(host.LastResolution).IsNotNull();
        await Assert.That(host.LastResolution!.View).IsAssignableTo<ResolvedView>();
    }

    /// <summary>Verifies resolution APIs reject hosts that cannot accept resolved navigation pairs.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_NonResolvedHostThrowsDescriptiveInvalidOperation()
    {
        const string hostName = "non-resolved-reactive-host";
        using var owner = new NavigationOwner(hostName);
        using var host = new RecordingHost(hostName, false);

        owner.SetMainNavigationHost(host);

        await Assert.That(() => owner.NavigateTo(new NavigationKeyRequest<ResolvedViewModel>()))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies a host renamed after registration can be resolved through its current name.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RenamedHost_ResolvesCurrentNameAndCreatesNavigationAlias()
    {
        const string initialName = "initial-reactive-host";
        const string renamedHostName = "renamed-reactive-host";
        var parameter = new object();
        using var owner = new NavigationOwner(initialName);
        using var hosted = new HostedNavigationOwner();
        using var host = new RecordingHost(initialName, false);

        owner.SetMainNavigationHost(host);
        host.Name = renamedHostName;
        _ = hosted.NavigateBack(renamedHostName, parameter);

        await Assert.That(host.LastBackParameter).IsEqualTo(parameter);
    }

    /// <summary>Verifies a new navigation event disposes the previous view-scoped lifetime.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NewNavigation_DisposesPriorViewLifetimeBeforeInvokingNextView()
    {
        const string hostName = "lifetime-reactive-host";
        using var from = new RoutableViewModel("lifetime-from");
        using var to = new RoutableViewModel("lifetime-to");
        using var view = new NavigationView(to);
        var firstLifetimeResource = new TrackingDisposable();
        var callbackCount = 0;

        view.WhenNavigatedTo((_, lifetime) =>
        {
            callbackCount++;
            if (callbackCount != 1)
            {
                return;
            }

            lifetime.Add(firstLifetimeResource);
        });

        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(
            new ViewModelNavigationEventArgs(from, to, NavigationType.New, view, hostName));
        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(
            new ViewModelNavigationEventArgs(from, to, NavigationType.New, view, hostName));

        await Assert.That(callbackCount).IsGreaterThan(1);
        await Assert.That(firstLifetimeResource.DisposeCount).IsEqualTo(1);
    }

    /// <summary>Verifies setup observables emit for a same-name registered host without alias assumptions.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HostSetup_EmitsForPrimaryHostNameAndCanNavigateBackStartsFalse()
    {
        const string hostName = "setup-reactive-host";
        using var owner = new NavigationOwner(hostName);
        using var hosted = new HostedNavigationOwner();
        using var host = new RecordingHost(hostName, false);
        var setupValues = new List<bool>();
        var backValues = new List<bool>();

        using var setupSubscription = hosted.WhenSetup(hostName).Subscribe(setupValues.Add);
        using var backSubscription = hosted.CanNavigateBack(hostName).Subscribe(backValues.Add);
        owner.SetMainNavigationHost(host);

        await Assert.That(setupValues).Contains(true);
        await Assert.That(backValues).Contains(false);
    }

    /// <summary>Verifies all navigation callback registration APIs reject null handlers.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationCallbackRegistration_RejectsNullHandlers()
    {
        using var model = new RoutableViewModel("null-handler-reactive");
        using var view = new NavigationView(model);

        await Assert.That(() => view.WhenNavigatedFrom(null!)).Throws<ArgumentNullException>();
        await Assert.That(() => view.WhenNavigatedTo(null!)).Throws<ArgumentNullException>();
        await Assert.That(() => view.WhenNavigating(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies repeat registration reuses setup infrastructure while replacing the active host.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RepeatHostRegistration_ReusesSetupAndCleanupInfrastructure()
    {
        const string hostName = "reuse-reactive-host";
        using var owner = new NavigationOwner(hostName);
        using var firstHost = new RecordingHost(hostName, false);
        using var replacementHost = new RecordingHost(hostName, false);

        owner.SetMainNavigationHost(firstHost);
        var originalSetupSubject = ViewModelRoutedViewHostMixins.WhenSetupSubjects[hostName];
        var originalDisposable = ViewModelRoutedViewHostMixins.CurrentViewDisposable[hostName];
        var originalResultSignal = ViewModelRoutedViewHostMixins.ResultNavigating[hostName];
        owner.SetMainNavigationHost(replacementHost);

        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost[hostName]).IsEqualTo(replacementHost);
        await Assert.That(ViewModelRoutedViewHostMixins.WhenSetupSubjects[hostName]).IsEqualTo(originalSetupSubject);
        await Assert.That(ViewModelRoutedViewHostMixins.CurrentViewDisposable[hostName]).IsEqualTo(originalDisposable);
        await Assert.That(ViewModelRoutedViewHostMixins.ResultNavigating[hostName]).IsEqualTo(originalResultSignal);
    }

    /// <summary>Verifies resolved navigation reports a missing navigator after all locator fallbacks are removed.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_RequiresRegisteredNavigatorOrRegistry()
    {
        const string hostName = "missing-navigator-reactive-host";
        using var owner = new NavigationOwner(hostName);
        using var host = new ResolvedRecordingHost(hostName, false);

        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        owner.SetMainNavigationHost(host);

        await Assert.That(() => owner.NavigateTo(new NavigationKeyRequest<ResolvedViewModel>()))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies disposing a navigation view removes callback subscriptions from shared signals.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DisposedNavigationView_DoesNotReceiveLaterNavigationCallbacks()
    {
        const string hostName = "disposed-view-reactive-host";
        using var from = new RoutableViewModel("disposed-from-reactive");
        using var to = new RoutableViewModel("disposed-to-reactive");
        var callbackCount = 0;
        var view = new NavigationView(to);

        view.WhenNavigatedTo((_, _) => callbackCount++);
        view.Dispose();
        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(
            new ViewModelNavigationEventArgs(from, to, NavigationType.New, view, hostName));

        await Assert.That(callbackCount).IsEqualTo(0);
    }

    /// <summary>Verifies backward navigation reuses the active view lifetime instead of disposing it.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BackNavigation_ReusesExistingViewLifetime()
    {
        const string hostName = "back-lifetime-reactive-host";
        using var from = new RoutableViewModel("back-lifetime-from");
        using var to = new RoutableViewModel("back-lifetime-to");
        using var view = new NavigationView(to);
        CompositeDisposable? initialLifetime = null;
        CompositeDisposable? returnedLifetime = null;

        view.WhenNavigatedTo((args, lifetime) =>
        {
            if (args.NavigationType == NavigationType.New)
            {
                initialLifetime = lifetime;
                return;
            }

            returnedLifetime = lifetime;
        });

        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(
            new ViewModelNavigationEventArgs(from, to, NavigationType.New, view, hostName));
        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(
            new ViewModelNavigationEventArgs(to, to, NavigationType.Back, view, hostName));

        await Assert.That(initialLifetime).IsNotNull();
        await Assert.That(returnedLifetime).IsEqualTo(initialLifetime);
    }

    /// <summary>Verifies pending navigation still invokes the matching view when no host result signal exists.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PendingNavigation_WithoutHostResultSignalStillInvokesViewModel()
    {
        const string hostName = "unobserved-pending-reactive-host";
        using var from = new RoutableViewModel("unobserved-pending-from");
        using var to = new RoutableViewModel("unobserved-pending-to");
        using var view = new NavigationView(from);
        var handlerCount = 0;

        _ = ViewModelRoutedViewHostMixins.ResultNavigating.Remove(hostName);
        view.WhenNavigating(args =>
        {
            handlerCount++;
            return args;
        });
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(
            new ViewModelNavigatingEventArgs(from, to, NavigationType.New, view, hostName));

        await Assert.That(handlerCount).IsGreaterThan(0);
        await Assert.That(from.NavigatingCount).IsGreaterThan(0);
    }

    /// <summary>Verifies primary navigation setup publishes backward-navigation changes and default helpers.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PrimaryNavigation_SetupPublishesBackStateAndDefaultHistoryHelpers()
    {
        const string hostName = "primary-state-reactive-host";
        using var owner = new NavigationOwner(hostName);
        using var hosted = new HostedNavigationOwner();
        using var host = new RecordingHost(hostName, false);
        using var first = new TestViewModel();
        using var second = new TestViewModel();
        var backStates = new List<bool>();

        AppLocator.CurrentMutable.SetupComplete();
        using var subscription = owner.CanNavigateBack().Subscribe(backStates.Add);
        owner.SetMainNavigationHost(host);
        host.Navigate(first, null, null);
        host.Navigate(second, null, null);
        owner.NavigateBack();
        _ = hosted.NavigateBack(hostName);
        hosted.ClearHistory(hostName);
        owner.ClearHistory();

        await Assert.That(backStates).Contains(false);
        await Assert.That(host.NavigationStack).IsEmpty();
    }

    /// <summary>Verifies navigation and filter value objects retain fallback, normalization, and display states.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationAndFilterValueObjects_ProjectFallbackAndDistinctStates()
    {
        var lookup = new NavigationLookupKey(NavigationSourceKind.View, typeof(string), null);
        var sameLookup = new NavigationLookupKey(NavigationSourceKind.View, typeof(string), null);
        var otherLookup = new NavigationLookupKey(NavigationSourceKind.ViewModel, typeof(string), null);
        var registration = new NavigationRegistrationException("registration");
        var resolution = new NavigationResolutionException(
            NavigationSourceKind.View,
            typeof(string),
            PaddedContract,
            [null, Contract, PaddedContract, OtherContract, OtherContract]);
        var inactiveExpression = new FilterExpression(StatusField, FilterOperator.Equals, " ", "Status");
        var activeExpression = new FilterExpression(StatusField, FilterOperator.Equals, "Open");
        var token = activeExpression.ToToken();
        var query = new SearchQueryState(" ", resultCount: 3, filters: [token]);
        var request = new PageRequest(0, PageSize, null, false, query);

        await Assert.That(lookup == sameLookup).IsTrue();
        await Assert.That(lookup != otherLookup).IsTrue();
        await Assert.That(lookup.Equals((object?)null)).IsFalse();
        await Assert.That(registration.ServiceType).IsEqualTo(typeof(object));
        await Assert.That(resolution.Contract).IsEqualTo(PaddedContract);
        await Assert.That(resolution.KnownContracts).IsEquivalentTo([null, Contract, PaddedContract, OtherContract]);
        await Assert.That(inactiveExpression.IsActive).IsFalse();
        await Assert.That(activeExpression.IsActive).IsTrue();
        await Assert.That(token.DisplayText).Contains("status");
        await Assert.That(query.ResultSummary).IsEqualTo("3 results");
        await Assert.That(request.FilterSnapshotKey).IsEqualTo(token.Key);
    }

    /// <summary>Verifies hosted resolution wrappers forward runtime and generic keys to a resolved host.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HostedResolutionWrappers_ResolveViewModelAndViewKeys()
    {
        const string hostName = "hosted-resolution-reactive-host";
        const string contract = "hosted-resolution-reactive-contract";
        var parameter = new object();
        var registry = new NavigationRegistry();
        using var owner = new NavigationOwner(hostName);
        using var hosted = new HostedNavigationOwner();
        using var host = new ResolvedRecordingHost(hostName, false);

        RegisterResolvedPair(registry, contract);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        AppLocator.CurrentMutable.RegisterConstant<INavigationRegistry>(registry);
        IBidirectionalNavigator navigator = registry.CreateNavigator();
        AppLocator.CurrentMutable.RegisterConstant(navigator);
        owner.SetMainNavigationHost(host);

        hosted.NavigateTo(new NavigationKeyRequest<ResolvedView> { Options = new NavigationRequestOptions { HostName = hostName, Contract = contract, Parameter = parameter, }, });
        await Assert.That(host.LastResolution).IsNotNull();
        await Assert.That(host.LastResolution!.View).IsAssignableTo<ResolvedView>();

        hosted.NavigateTo(typeof(ResolvedViewModel), new NavigationRequestOptions { HostName = hostName, Contract = contract, Parameter = parameter, });

        await Assert.That(host.LastResolution!.ViewModel).IsAssignableTo<ResolvedViewModel>();
        await Assert.That(host.LastResolution.Contract).IsEqualTo(contract);
        await Assert.That(host.LastResolution.Parameter).IsEqualTo(parameter);
    }

    /// <summary>Verifies resolution retries the view lookup and reports an unknown contract when both lookups fail.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigation_UnknownContractReportsResolutionFailureAfterViewFallback()
    {
        const string hostName = "unknown-contract-reactive-host";
        const string knownContract = "known-reactive-contract";
        const string unknownContract = "unknown-reactive-contract";
        var registry = new NavigationRegistry();
        using var owner = new NavigationOwner(hostName);
        using var host = new ResolvedRecordingHost(hostName, false);

        RegisterResolvedPair(registry, knownContract);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        AppLocator.CurrentMutable.RegisterConstant<INavigationRegistry>(registry);
        IBidirectionalNavigator navigator = registry.CreateNavigator();
        AppLocator.CurrentMutable.RegisterConstant(navigator);
        owner.SetMainNavigationHost(host);

        await Assert.That(() => owner.NavigateTo(
                new NavigationKeyRequest<ResolvedViewModel> { Options = new NavigationRequestOptions { Contract = unknownContract, }, }))
            .Throws<NavigationResolutionException>();
    }

    /// <summary>Verifies unknown hosted setup requests stay silent while backward state retains its false default.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task UnknownHostedSetup_StaysSilentAndBackStateStartsFalse()
    {
        const string knownHostName = "known-setup-reactive-host";
        const string unknownHostName = "unknown-setup-reactive-host";
        using var owner = new NavigationOwner(knownHostName);
        using var hosted = new HostedNavigationOwner();
        using var host = new RecordingHost(knownHostName, false);
        var setupValues = new List<bool>();
        var backValues = new List<bool>();

        using var setupSubscription = hosted.WhenSetup(unknownHostName).Subscribe(setupValues.Add);
        using var backSubscription = hosted.CanNavigateBack(unknownHostName).Subscribe(backValues.Add);
        owner.SetMainNavigationHost(host);

        await Assert.That(setupValues).IsEmpty();
        await Assert.That(backValues).IsEquivalentTo([false]);
    }

    /// <summary>Verifies primary-host setup and back-state observables follow the registered primary key.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PrimaryHostSetupAndBackState_UseTheExactRegisteredNavigationKey()
    {
        const string hostName = "primary-observable-host";
        using var owner = new NavigationOwner(hostName);
        using var host = new RecordingHost(hostName, false);
        using var first = new TestViewModel();
        using var second = new TestViewModel();
        var setupStates = new List<bool>();
        var backStates = new List<bool>();
        using var setupSubscription = owner.WhenSetup().Subscribe(setupStates.Add);
        using var backSubscription = owner.CanNavigateBack().Subscribe(backStates.Add);

        AppLocator.CurrentMutable.SetupComplete();
        owner.SetMainNavigationHost(host);
        host.Navigate(first, null, null);
        host.Navigate(second, null, null);

        await Assert.That(setupStates).Contains(true);
        await Assert.That(backStates).Contains(false);
        await Assert.That(backStates).Contains(true);
    }

    /// <summary>Verifies a renamed host without a matching setup signal does not publish a stale setup event.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PrimarySetup_RenamedHostWithoutSetupSignalStaysSilent()
    {
        const string initialHostName = "primary-setup-initial-host";
        const string renamedHostName = "primary-setup-renamed-host";
        using var owner = new NavigationOwner(initialHostName);
        using var host = new RecordingHost(initialHostName, false);
        var setupStates = new List<bool>();

        owner.SetMainNavigationHost(host);
        host.Name = renamedHostName;
        using var subscription = owner.WhenSetup().Subscribe(setupStates.Add);
        AppLocator.CurrentMutable.SetupComplete();

        await Assert.That(setupStates).IsEmpty();
    }

    /// <summary>Registers the standard resolved view-model/view pair with explicitly typed factories.</summary>
    /// <param name="registry">The registry receiving the pair.</param>
    /// <param name="contract">The optional registration contract.</param>
    private static void RegisterResolvedPair(NavigationRegistry registry, string? contract)
    {
        Func<IServiceProvider, ResolvedViewModel> createViewModel = static _ => new();
        Func<IServiceProvider, ResolvedView> createView = static _ => new();
        _ = registry.Register(createViewModel, createView, contract);
    }

    /// <summary>Provides a primary navigation owner.</summary>
    /// <param name="name">The navigation host name.</param>
    private sealed class NavigationOwner(string name) : RxObject, IUseNavigation, ISetNavigation
    {
        string? IUseNavigation.Name => name;

        string? ISetNavigation.Name => name;
    }

    /// <summary>Provides a named navigation owner.</summary>
    private sealed class HostedNavigationOwner : RxObject;

    /// <summary>Provides a navigable view model.</summary>
    private sealed class TestViewModel : RxObject;

    /// <summary>Provides a registered routed view model.</summary>
    private sealed class ResolvedViewModel : RxObject;

    /// <summary>Provides the paired registered routed view.</summary>
    private sealed class ResolvedView : global::ReactiveUI.IViewFor<ResolvedViewModel>
    {
        /// <inheritdoc/>
        public ResolvedViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? global::ReactiveUI.IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ResolvedViewModel?)value;
        }
    }

    /// <summary>Tracks disposable lifetime cleanup.</summary>
    private sealed class TrackingDisposable : IDisposable
    {
        /// <summary>Gets the number of disposal calls.</summary>
        public int DisposeCount { get; private set; }

        /// <inheritdoc/>
        public void Dispose() => DisposeCount++;
    }

    /// <summary>Records runtime bidirectional navigation requests.</summary>
    private sealed class RecordingNavigator : IBidirectionalNavigator
    {
        /// <summary>Gets the last view-model key.</summary>
        public Type? LastViewModelKey { get; private set; }

        /// <summary>Gets the last view key.</summary>
        public Type? LastViewKey { get; private set; }

        /// <summary>Gets the last options.</summary>
        public NavigationRequestOptions? LastOptions { get; private set; }

        /// <inheritdoc/>
        public IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
            ViewModelNavigationRequest<TViewModel, TView> request)
            where TViewModel : class, IRxObject
            where TView : class, global::ReactiveUI.IViewFor<TViewModel> => Observable.Empty<NavigationResolution<TViewModel, TView>>();

        /// <inheritdoc/>
        public IObservable<NavigationResolution> NavigateViewModel(Type viewModelKey, NavigationRequestOptions options)
        {
            LastViewModelKey = viewModelKey;
            LastOptions = options;
            return Observable.Empty<NavigationResolution>();
        }

        /// <inheritdoc/>
        public IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
            ViewNavigationRequest<TViewModel, TView> request)
            where TViewModel : class, IRxObject
            where TView : class, global::ReactiveUI.IViewFor<TViewModel>
        {
            ViewModelNavigationRequest<TViewModel, TView> viewModelRequest = new();
            return NavigateViewModel(viewModelRequest);
        }

        /// <inheritdoc/>
        public IObservable<NavigationResolution> NavigateView(Type viewKey, NavigationRequestOptions options)
        {
            LastViewKey = viewKey;
            LastOptions = options;
            return Observable.Empty<NavigationResolution>();
        }
    }

    /// <summary>Records routed view-model lifecycle callbacks.</summary>
    /// <param name="name">The stable routed name.</param>
    private sealed class RoutableViewModel(string name) : RxObject, INotifiyRoutableViewModel
    {
        /// <summary>Gets the number of completion-from callbacks.</summary>
        public int NavigatedFromCount { get; private set; }

        /// <summary>Gets the number of completion-to callbacks.</summary>
        public int NavigatedToCount { get; private set; }

        /// <summary>Gets the number of pending-navigation callbacks.</summary>
        public int NavigatingCount { get; private set; }

        string? IUseHostedNavigation.Name => name;

        /// <inheritdoc/>
        public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e) => NavigatedFromCount++;

        /// <inheritdoc/>
        public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables) => NavigatedToCount++;

        /// <inheritdoc/>
        public override void WhenNavigating(IViewModelNavigatingEventArgs e) => NavigatingCount++;
    }

    /// <summary>Provides a notification-capable routed view.</summary>
    /// <param name="viewModel">The routed view model.</param>
    private sealed class NavigationView(object? viewModel) : INotifiyNavigation, global::ReactiveUI.IViewFor
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
        public object? ViewModel { get; set; } = viewModel;

        /// <inheritdoc/>
        public void Dispose() => CleanUp.Dispose();
    }

    /// <summary>Records a hosted navigation interaction.</summary>
    /// <param name="name">The host name.</param>
    /// <param name="requiresSetup">Whether setup must run during registration.</param>
    private class RecordingHost(string name, bool requiresSetup) : IViewModelRoutedViewHost, IDisposable
    {
        /// <summary>Publishes changes to the backward-navigation state.</summary>
        private readonly StateSignal<bool?> _canNavigateBack = new(false);

        /// <summary>Gets the recorded navigation entries.</summary>
        public ObservableCollection<Type?> NavigationStack { get; } = [];

        /// <summary>Gets the number of setup calls.</summary>
        public int SetupCount { get; private set; }

        /// <summary>Gets the last back-navigation parameter.</summary>
        public object? LastBackParameter { get; private set; }

        /// <summary>Gets the last view model.</summary>
        public IRxObject? LastViewModel { get; private set; }

        /// <summary>Gets the last contract.</summary>
        public string? LastContract { get; private set; }

        /// <summary>Gets the last parameter.</summary>
        public object? LastParameter { get; private set; }

        /// <inheritdoc/>
        public IObservable<INotifiyRoutableViewModel> CurrentViewModel => Observable.Empty<INotifiyRoutableViewModel>();

        /// <inheritdoc/>
        public IObservable<bool?> CanNavigateBackObservable => _canNavigateBack;

        /// <inheritdoc/>
        public bool? CanNavigateBack
        {
            get => _canNavigateBack.Value;
            set => _canNavigateBack.OnNext(value);
        }

        /// <inheritdoc/>
        public bool? NavigateBackIsEnabled { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; } = name;

        /// <inheritdoc/>
        public string HostName
        {
            get => Name;
            set => Name = value;
        }

        /// <inheritdoc/>
        public bool RequiresSetup => requiresSetup;

        /// <inheritdoc/>
        public void ClearHistory()
        {
            NavigationStack.Clear();
            _canNavigateBack.OnNext(false);
        }

        /// <inheritdoc/>
        public void Setup() => SetupCount++;

        /// <inheritdoc/>
        public void Navigate<T>(T viewModel, string? contract, object? parameter)
            where T : class, IRxObject => Navigate((IRxObject)viewModel, contract, parameter);

        /// <inheritdoc/>
        public void Navigate(IRxObject viewModel, string? contract, object? parameter)
        {
            LastViewModel = viewModel;
            LastContract = contract;
            LastParameter = parameter;
            NavigationStack.Add(viewModel.GetType());
            _canNavigateBack.OnNext(NavigationStack.Count > 1);
        }

        /// <inheritdoc/>
        public void NavigateAndReset<T>(T viewModel, string? contract, object? parameter)
            where T : class, IRxObject => NavigateAndReset((IRxObject)viewModel, contract, parameter);

        /// <inheritdoc/>
        public void NavigateAndReset(IRxObject viewModel, string? contract, object? parameter)
        {
            ClearHistory();
            Navigate(viewModel, contract, parameter);
        }

        /// <inheritdoc/>
        public IRxObject? NavigateBack(object? parameter)
        {
            LastBackParameter = parameter;
            if (NavigationStack.Count > 1)
            {
                NavigationStack.RemoveAt(NavigationStack.Count - 1);
            }

            _canNavigateBack.OnNext(NavigationStack.Count > 1);
            return null;
        }

        /// <inheritdoc/>
        public void Refresh()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases resources owned by the recording host.</summary>
        /// <param name="disposing">Whether managed resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _canNavigateBack.Dispose();
        }
    }

    /// <summary>Records resolution-based routed-host navigation.</summary>
    /// <param name="name">The host name.</param>
    /// <param name="requiresSetup">Whether setup must run during registration.</param>
    private sealed class ResolvedRecordingHost(string name, bool requiresSetup) : RecordingHost(name, requiresSetup), IResolvedViewModelRoutedViewHost
    {
        /// <summary>Gets the last resolution.</summary>
        public NavigationResolution? LastResolution { get; private set; }

        /// <inheritdoc/>
        public void Navigate(NavigationResolution resolution) => LastResolution = resolution;

        /// <inheritdoc/>
        public void NavigateAndReset(NavigationResolution resolution) => Navigate(resolution);
    }
}
