// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Tests;

/// <summary>Exercises observable boundary behavior for core state models.</summary>
public sealed class CoreCoverageEdgeBehaviorTests
{
    /// <summary>Provides the duration used by range-boundary scenarios.</summary>
    private const int RangeDurationHours = 2;

    /// <summary>Provides an undefined preset for fallback-label behavior.</summary>
    private const int UndefinedPresetValue = 999;

    /// <summary>Provides an invalid negative page index.</summary>
    private const int NegativePageIndex = -4;

    /// <summary>Provides a page size used by paging scenarios.</summary>
    private const int PagingPageSize = 2;

    /// <summary>Provides the total item count used by paging scenarios.</summary>
    private const int TotalItemCount = 6;

    /// <summary>Provides a deliberately over-large requested page index.</summary>
    private const int OversizedPageIndex = 99;

    /// <summary>Provides the primary contract used by navigation scenarios.</summary>
    private const string PrimaryContract = "primary";

    /// <summary>Provides whitespace-padded contract text for normalization scenarios.</summary>
    private const string PaddedPrimaryContract = " primary ";

    /// <summary>Provides a second contract for distinct-contract scenarios.</summary>
    private const string SecondaryContract = "secondary";

    /// <summary>Provides the expected number of pending events delivered to a host signal.</summary>
    private const int ExpectedPendingEventCount = 2;

    /// <summary>Provides the expected stack size when the only entry cannot be navigated back from.</summary>
    private const int SingleNavigationEntryCount = 1;

    /// <summary>Provides the name for the source routed view model.</summary>
    private const string FromViewModelName = "from";

    /// <summary>Provides the name for the destination routed view model.</summary>
    private const string ToViewModelName = "to";

    /// <summary>Provides the category name used by property-group scenarios.</summary>
    private const string PropertyGroupName = "group";

    /// <summary>Provides the key used for unmatched lookup scenarios.</summary>
    private const string MissingKey = "missing";

    /// <summary>Provides the field key used by expression scenarios.</summary>
    private const string ExpressionFieldKey = "field";

    /// <summary>Provides a non-integral numeric value used by value-format scenarios.</summary>
    private const double NumericValue = 12.5D;

    /// <summary>Provides the display name used by validation scenarios.</summary>
    private const string InvalidDisplayName = "Invalid";

    /// <summary>Provides the display name used by Boolean value scenarios.</summary>
    private const string BooleanDisplayName = "Boolean";

    /// <summary>Provides a reusable first-item key for ordered-state scenarios.</summary>
    private const string FirstKey = "first";

    /// <summary>Provides the display name used by field-validation scenarios.</summary>
    private const string FieldDisplayName = "Field";

    /// <summary>Verifies range validity, endpoint inclusivity, labels, and maximum duration validation.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateTimeRange_BoundariesAndValidation_ProjectAllUserFacingStates()
    {
        var start = new DateTimeOffset(2026, 7, 26, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(RangeDurationHours);
        var incomplete = new DateTimeRange(start, null);
        var exceeded = new DateTimeRange(start, end, DateTimeRangePreset.Today, null, true, TimeSpan.FromHours(1));
        var exclusive = new DateTimeRange(start, end, DateTimeRangePreset.Today, null, false, null);
        var custom = new DateTimeRange(start, end, (DateTimeRangePreset)UndefinedPresetValue, "Custom report", true, null);

        await Assert.That(incomplete.ValidationMessage).IsEqualTo("Start and end are required.");
        await Assert.That(exceeded.ValidationMessage).IsEqualTo("Range exceeds the maximum allowed duration.");
        await Assert.That(exceeded.DisplayText).IsEqualTo("Today: invalid range");
        await Assert.That(exclusive.Contains(start)).IsTrue();
        await Assert.That(exclusive.Contains(end)).IsFalse();
        await Assert.That(custom.DisplayText).IsEqualTo("Custom report: 2026-07-26 08:00 - 2026-07-26 10:00");
    }

    /// <summary>Verifies null-object service resolution and all action availability combinations.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EmptyStateAndServiceProvider_ExposeOnlyConfiguredActions()
    {
        var command = new DelegateCommand();
        var noText = new EmptyStateModel("None", primaryActionCommand: command, secondaryActionCommand: command);
        var actions = new EmptyStateModel("None", primaryActionText: "Retry", primaryActionCommand: command, secondaryActionText: "Help", secondaryActionCommand: command);

        await Assert.That(EmptyServiceProvider.Instance.GetService(typeof(string))).IsNull();
        await Assert.That(noText.HasPrimaryAction).IsFalse();
        await Assert.That(noText.HasSecondaryAction).IsFalse();
        await Assert.That(actions.HasPrimaryAction).IsTrue();
        await Assert.That(actions.HasSecondaryAction).IsTrue();
    }

    /// <summary>Verifies busy and search projections at their invalid and plural boundaries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BusyAndSearch_ExposeInvalidProgressAndAllResultSummaries()
    {
        var belowRange = new BusyOperation("Load", progress: -0.1D);
        var aboveRange = new BusyOperation("Load", progress: 1.1D);
        var emptyTitle = new BusyOperation(" ");
        var unknown = new SearchQueryState();
        var single = new SearchQueryState(resultCount: 1);
        var plural = new SearchQueryState(resultCount: 2);

        await Assert.That(belowRange.IsDeterminate).IsFalse();
        await Assert.That(aboveRange.IsDeterminate).IsFalse();
        await Assert.That(emptyTitle.IsActive).IsFalse();
        await Assert.That(unknown.ResultSummary).IsEmpty();
        await Assert.That(single.ResultSummary).IsEqualTo("1 result");
        await Assert.That(plural.ResultSummary).IsEqualTo("2 results");
    }

    /// <summary>Verifies pagination clamping and segment selection fallback behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PaginationAndSegments_ClampAndResolveSelectedItems()
    {
        var first = new PaginationState(NegativePageIndex, 0, TotalItemCount);
        var last = new PaginationState(OversizedPageIndex, PagingPageSize, TotalItemCount);
        var selected = new SegmentedSelectionState([new SegmentItem("one", "One"), new SegmentItem("two", "Two", false)], "one");
        var missing = new SegmentedSelectionState([new SegmentItem("one", "One")], MissingKey);

        await Assert.That(first.PageIndex).IsEqualTo(0);
        await Assert.That(first.CreateRequest(-1).PageIndex).IsEqualTo(0);
        await Assert.That(last.PageIndex).IsEqualTo(PagingPageSize);
        await Assert.That(last.CreateRequest(OversizedPageIndex).PageIndex).IsEqualTo(PagingPageSize);
        await Assert.That(selected.HasSelection).IsTrue();
        await Assert.That(selected.GetItem(MissingKey)).IsNull();
        await Assert.That(missing.HasSelection).IsFalse();
    }

    /// <summary>Verifies step, theme, and validation display alternatives used by controls.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WorkflowThemeAndValidation_ExposeUnavailableAndFallbackStates()
    {
        var error = new ValidationMessage(null, " ", " Failure ", ValidationSeverity.Error, new DelegateCommand());
        var blocked = new StepDescriptor("blocked", null!, new StepDescriptorOptions { ValidationMessages = [error], CanEnter = true, CanLeave = false, });
        var empty = new StepperState([]);
        var unavailableHighContrast = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Dark, false);
        var system = new ThemePreferenceState(ThemeChoice.System, ThemeChoice.HighContrast, true);

        await Assert.That(error.HasField).IsFalse();
        await Assert.That(error.HasRemediation).IsTrue();
        await Assert.That(error.DisplayText).IsEqualTo("Failure");
        await Assert.That(blocked.IsBlocking).IsTrue();
        await Assert.That(blocked.IsAvailable).IsFalse();
        await Assert.That(blocked.DisplayTitle).IsEmpty();
        await Assert.That(empty.CurrentIndex).IsEqualTo(-1);
        await Assert.That(empty.ProgressText).IsEqualTo("No steps");
        await Assert.That(empty.CanFinish).IsFalse();
        await Assert.That(unavailableHighContrast.DisplayText).IsEqualTo("High contrast (using Dark)");
        await Assert.That(unavailableHighContrast.SupportsChoice(ThemeChoice.HighContrast)).IsFalse();
        await Assert.That(system.DisplayText).IsEqualTo("System (High contrast)");
    }

    /// <summary>Verifies navigation value objects retain concrete runtime key and exception context.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationValueObjects_ExposeKeysEqualityAndExceptionOverloads()
    {
        var key = new NavigationKeyRequest<ViewModelRoutedViewHostMixinsTests.TestViewModel>();
        var viewModelRequest = new ViewModelNavigationRequest<TestNavigationViewModel, TestNavigationView>();
        var viewRequest = new ViewNavigationRequest<TestNavigationViewModel, TestNavigationView>();
        var lookup = new NavigationLookupKey(NavigationSourceKind.View, typeof(string), PrimaryContract);
        var sameLookup = new NavigationLookupKey(NavigationSourceKind.View, typeof(string), PrimaryContract);
        var differentLookup = new NavigationLookupKey(NavigationSourceKind.ViewModel, typeof(string), PrimaryContract);
        var inner = new InvalidOperationException("inner");
        var registrationException = new NavigationRegistrationException("registration", inner);
        var resolutionException = new NavigationResolutionException("resolution", inner);

        await Assert.That(key.Key).IsEqualTo(typeof(ViewModelRoutedViewHostMixinsTests.TestViewModel));
        await Assert.That(viewModelRequest.ViewType).IsEqualTo(typeof(TestNavigationView));
        await Assert.That(viewRequest.ViewModelType).IsEqualTo(typeof(TestNavigationViewModel));
        await Assert.That(lookup == sameLookup).IsTrue();
        await Assert.That(lookup != differentLookup).IsTrue();
        await Assert.That(lookup.Equals((object?)null)).IsFalse();
        await Assert.That(registrationException.InnerException).IsEqualTo(inner);
        await Assert.That(resolutionException.InnerException).IsEqualTo(inner);
    }

    /// <summary>Verifies registration validates required factories and exposes typed public keys.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationRegistration_RequiresFactoriesAndProjectsLookupTypes()
    {
        var registration = new NavigationRegistration<
            TestNavigationViewModel,
            TestNavigationViewModel,
            TestNavigationView,
            TestNavigationView>(
            static _ => new(),
            static _ => new());

        await Assert.That(registration.ViewModelKey).IsEqualTo(typeof(TestNavigationViewModel));
        await Assert.That(registration.ViewKey).IsEqualTo(typeof(TestNavigationView));
        await Assert.That(static () => new NavigationRegistration<
                TestNavigationViewModel,
                TestNavigationViewModel,
                TestNavigationView,
                TestNavigationView>(null!, static _ => new()))
            .Throws<ArgumentNullException>();
        await Assert.That(static () => new NavigationRegistration<
                TestNavigationViewModel,
                TestNavigationViewModel,
                TestNavigationView,
                TestNavigationView>(static _ => new(), null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies navigation failure constructors keep stable defaults, details, and unique contracts.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationExceptions_ProjectDefaultMessageAndDetailedFailureContracts()
    {
        var defaultRegistration = new NavigationRegistrationException();
        var messageRegistration = new NavigationRegistrationException("message");
        var detailedRegistration = new NavigationRegistrationException(NavigationSourceKind.View, typeof(string), PaddedPrimaryContract);
        var defaultResolution = new NavigationResolutionException();
        var messageResolution = new NavigationResolutionException("message");
        var detailedResolution = new NavigationResolutionException(
            NavigationSourceKind.View,
            typeof(string),
            PaddedPrimaryContract,
            [null, PaddedPrimaryContract, PrimaryContract, SecondaryContract, SecondaryContract]);

        await Assert.That(defaultRegistration.ServiceType).IsEqualTo(typeof(object));
        await Assert.That(messageRegistration.SourceKind).IsEqualTo(NavigationSourceKind.ViewModel);
        await Assert.That(detailedRegistration.Contract).IsEqualTo(PaddedPrimaryContract);
        await Assert.That(detailedRegistration.Message).Contains("View navigation registration");
        await Assert.That(defaultResolution.SourceKey).IsEqualTo(typeof(object));
        await Assert.That(messageResolution.KnownContracts).IsEmpty();
        await Assert.That(detailedResolution.Contract).IsEqualTo(PaddedPrimaryContract);
        await Assert.That(detailedResolution.KnownContracts).IsEquivalentTo([null, PaddedPrimaryContract, PrimaryContract, SecondaryContract]);
        await Assert.That(static () => new NavigationRegistrationException(NavigationSourceKind.View, null!, null)).Throws<ArgumentNullException>();
        await Assert.That(static () => new NavigationResolutionException(NavigationSourceKind.View, null!, null, [])).Throws<ArgumentNullException>();
        await Assert.That(static () => new NavigationResolutionException(NavigationSourceKind.View, typeof(string), null, null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies host convenience navigation delegates preserve contracts and reset behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HostNavigationConvenienceMethods_DelegateToHostWithDefaultParameters()
    {
        var host = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost("convenience");
        var viewModel = new ViewModelRoutedViewHostMixinsTests.TestViewModel("convenience");

        host.Navigate(viewModel);
        host.Navigate(viewModel, PrimaryContract);
        host.NavigateAndReset(viewModel);
        host.NavigateAndReset(viewModel, SecondaryContract);
        _ = host.NavigateBack();

        await Assert.That(host.LastContract).IsEqualTo(SecondaryContract);
        await Assert.That(host.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Verifies registered views receive matching navigation completion and pending events.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationViewCallbacks_ForwardMatchingCompletedAndPendingLifecycleEvents()
    {
        const string hostName = "callback-host";
        using var from = new RoutableViewModel(FromViewModelName);
        using var to = new RoutableViewModel(ToViewModelName);
        using var unrelated = new UnrelatedRoutableViewModel();
        using var fromView = new ViewModelRoutedViewHostMixinsTests.TestView(from);
        using var toView = new ViewModelRoutedViewHostMixinsTests.TestView(to);
        var completedFromHandlers = 0;
        var completedToHandlers = 0;
        var navigatingHandlers = 0;
        var pendingForHost = 0;
        ViewModelRoutedViewHostMixins.ResultNavigating[hostName] = new();
        using var pendingSubscription = ViewModelRoutedViewHostMixins.ResultNavigating[hostName]
            .Subscribe(_ => pendingForHost++);

        fromView.WhenNavigatedFrom(_ => completedFromHandlers++);
        toView.WhenNavigatedTo((_, _) => completedToHandlers++);
        fromView.WhenNavigating(args =>
        {
            navigatingHandlers++;
            return args;
        });

        var completed = new ViewModelNavigationEventArgs(from, to, NavigationType.New, toView, hostName);
        var navigating = new ViewModelNavigatingEventArgs(from, to, NavigationType.New, toView, hostName);
        var entering = new ViewModelNavigatingEventArgs(null, to, NavigationType.New, toView, hostName);
        var unrelatedEvent = new ViewModelNavigatingEventArgs(unrelated, unrelated, NavigationType.New, toView, hostName);
        var missingResultEvent = new ViewModelNavigatingEventArgs(from, to, NavigationType.New, toView, "missing-result-host");
        ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(completed);
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(navigating);
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(entering);
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(unrelatedEvent);
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(missingResultEvent);
        ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(null!);

        await Assert.That(completedFromHandlers).IsEqualTo(1);
        await Assert.That(completedToHandlers).IsEqualTo(1);
        await Assert.That(from.NavigatedFromCount).IsEqualTo(1);
        await Assert.That(to.NavigatedToCount).IsEqualTo(1);
        await Assert.That(navigatingHandlers).IsEqualTo(ExpectedPendingEventCount);
        await Assert.That(from.NavigatingCount).IsEqualTo(ExpectedPendingEventCount);
        await Assert.That(pendingForHost).IsEqualTo(ExpectedPendingEventCount);
    }

    /// <summary>Verifies host setup assigns a fallback key and invokes required initialization once.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationHostSetup_UsesFallbackNameAndRunsRequiredSetup()
    {
        using var navigation = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(null);
        var host = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(string.Empty) { RequiresSetup = true, };

        navigation.SetMainNavigationHost(host);

        await Assert.That(host.Name).StartsWith("__crisscross_host_");
        await Assert.That(host.SetupCallCount).IsEqualTo(1);
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost.ContainsKey(host.Name)).IsTrue();
    }

    /// <summary>Verifies a renamed host is found by its current name and cached as an alias.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NamedHostOperations_ResolveRenamedHostAndCacheAlias()
    {
        const string originalHostName = "original-host";
        const string renamedHostName = "renamed-host";
        using var navigation = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(originalHostName);
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
        var host = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(originalHostName);
        navigation.SetMainNavigationHost(host);
        host.Name = renamedHostName;

        consumer.ClearHistory(renamedHostName);

        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost.ContainsKey(renamedHostName)).IsTrue();
        await Assert.That(ReferenceEquals(ViewModelRoutedViewHostMixins.NavigationHost[renamedHostName], host)).IsTrue();
    }

    /// <summary>Verifies runtime view-model navigation is a no-op when no matching service is registered.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RuntimeViewNavigation_LeavesHostUntouchedWhenServiceIsUnavailable()
    {
        const string hostName = "runtime-host";
        using var navigation = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(hostName);
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
        var host = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(hostName);
        Splat.AppLocator.CurrentMutable.UnregisterAll<ViewModelRoutedViewHostMixinsTests.TestViewModel>();
        navigation.SetMainNavigationHost(host);

        consumer.NavigateToView(
            typeof(ViewModelRoutedViewHostMixinsTests.TestViewModel),
            new NavigationRequestOptions { HostName = hostName, });

        await Assert.That(host.NavigationStack).IsEmpty();
    }

    /// <summary>Verifies parameterless hosted-navigation shortcuts delegate to the default registered host.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HostedNavigationShortcuts_UseDefaultHostForSetupHistoryAndRuntimeViewRequests()
    {
        const string hostName = "hosted-shortcuts";
        using var setup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(hostName);
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
        var host = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(hostName);
        setup.SetMainNavigationHost(host);
        host.NavigationStack.Add(typeof(ViewModelRoutedViewHostMixinsTests.TestViewModel));

        using var setupSubscription = consumer.WhenSetup().Subscribe(static _ => { });
        using var backSubscription = consumer.CanNavigateBack().Subscribe(static _ => { });
        _ = consumer.NavigateBack();
        consumer.ClearHistory();
        consumer.NavigateToView(typeof(ViewModelRoutedViewHostMixinsTests.TestViewModel));

        await Assert.That(host.NavigationStack.Count).IsLessThanOrEqualTo(SingleNavigationEntryCount);
    }

    /// <summary>Verifies primary-navigation runtime shortcuts preserve overload delegation and unavailable-service behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PrimaryNavigationRuntimeShortcuts_DelegateToCurrentHost()
    {
        const string hostName = "primary-shortcuts";
        using var setup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(hostName);
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestViewModel(hostName);
        var host = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(hostName);
        setup.SetMainNavigationHost(host);
        host.NavigationStack.Add(typeof(ViewModelRoutedViewHostMixinsTests.TestViewModel));

        consumer.NavigateBack((object?)"parameter");
        consumer.NavigateToView(typeof(ViewModelRoutedViewHostMixinsTests.TestViewModel));

        await Assert.That(host.NavigationStack.Count).IsEqualTo(SingleNavigationEntryCount);
    }

    /// <summary>Verifies constructor shims and fallback formatting paths retain stable state-model projections.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CoreValueObjectShims_ExposeFallbackAndEmptyStates()
    {
        var chipGroup = new ChipGroupState([]);
        var yesterdayRange = new DateTimeRange(null, null, DateTimeRangePreset.Yesterday);
        var unknownDescriptor = new FilterDescriptor("unknown", "Unknown", (FilterEditorKind)UndefinedPresetValue);
        var nullExpression = new FilterExpression(ExpressionFieldKey, FilterOperator.Equals, null);
        var blankExpression = new FilterExpression(ExpressionFieldKey, FilterOperator.Equals, " ");
        var request = new PageRequest(0, 1, null, false);
        var group = new PropertyDescriptorGroup(PropertyGroupName, []);
        var descriptor = new PropertyDescriptorModel("key", "Key");
        var grid = new PropertyGridState([descriptor], " ", false);
        var segments = new SegmentedSelectionState([]);
        var defaultStep = new StepDescriptor("step", "Step");
        var emptyStepper = new StepperState([], null);
        var theme = new ThemePreferenceState((ThemeChoice)UndefinedPresetValue, ThemeChoice.Light);

        await Assert.That(chipGroup.Chips).IsEmpty();
        await Assert.That(yesterdayRange.Label).IsEqualTo("Yesterday");
        await Assert.That(unknownDescriptor.DefaultOperator).IsEqualTo(FilterOperator.Equals);
        await Assert.That(nullExpression.IsActive).IsFalse();
        await Assert.That(blankExpression.IsActive).IsFalse();
        await Assert.That(request.FilterSnapshotKey).IsEmpty();
        await Assert.That(group.Count).IsEqualTo(0);
        await Assert.That(descriptor.HasValidationMessages).IsFalse();
        await Assert.That(grid.HasSearch).IsFalse();
        await Assert.That(grid.CanReset).IsFalse();
        await Assert.That(segments.Items).IsEmpty();
        await Assert.That(defaultStep.ValidationMessages).IsEmpty();
        await Assert.That(emptyStepper.CurrentIndex).IsEqualTo(-1);
        await Assert.That(theme.DisplayText).IsEqualTo("System");
    }

    /// <summary>Verifies values with stable keys and unmatched lookups retain deterministic fallback results.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CoreValueObjects_ExposeKeysAndNoMatchFallbacks()
    {
        var expression = new FilterExpression(ExpressionFieldKey, FilterOperator.Contains, "value");
        var firstToken = new FilterDescriptor(FirstKey, "First", FilterEditorKind.Text).CreateToken("one");
        var secondToken = new FilterDescriptor("second", "Second", FilterEditorKind.Text).CreateToken("two");
        var request = new PageRequest(0, 1, null, false, new SearchQueryState(filters: [firstToken, secondToken]));
        var descriptor = new PropertyDescriptorModel("key", "Key");
        var group = new PropertyDescriptorGroup(PropertyGroupName, [descriptor]);
        var unmatchedStepper = new StepperState([new StepDescriptor("step", "Step")], MissingKey);

        await Assert.That(expression.Key).Contains($"{ExpressionFieldKey}:Contains:value");
        await Assert.That(request.FilterSnapshotKey).Contains("|");
        await Assert.That(group.HasModifiedDescriptors).IsFalse();
        await Assert.That(unmatchedStepper.CurrentIndex).IsEqualTo(0);
    }

    /// <summary>Verifies runtime navigation-key overloads reject resolution when no navigator is registered.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RuntimeNavigationKeys_RequireARegisteredNavigatorForHostedAndPrimaryNavigation()
    {
        const string hostedName = "runtime-key-hosted";
        const string primaryName = "runtime-key-primary";
        using var hostedSetup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(hostedName);
        using var hostedConsumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
        var hostedHost = new ViewModelRoutedViewHostMixinsTests.TestResolvedViewModelRoutedViewHost(hostedName);
        using var primarySetup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(primaryName);
        using var primaryConsumer = new ViewModelRoutedViewHostMixinsTests.TestViewModel(primaryName);
        var primaryHost = new ViewModelRoutedViewHostMixinsTests.TestResolvedViewModelRoutedViewHost(primaryName);
        Splat.AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        Splat.AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        hostedSetup.SetMainNavigationHost(hostedHost);
        primarySetup.SetMainNavigationHost(primaryHost);

        await Assert.That(() => hostedConsumer.NavigateTo(typeof(TestNavigationViewModel))).Throws<InvalidOperationException>();
        await Assert.That(() => primaryConsumer.NavigateTo(typeof(TestNavigationViewModel))).Throws<InvalidOperationException>();
    }

    /// <summary>Verifies state models expose every interactive, active, and display alternative.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CoreStateModels_ProjectAllInteractiveAndFormattingAlternatives()
    {
        var command = new DelegateCommand();
        var disabledChip = new ChipModel("disabled", "Disabled", new ChipModelOptions { IsEnabled = false, IsSelected = true, IsRemovable = true, RemoveCommand = command, SelectCommand = command, });
        var removableChip = new ChipModel("removable", "Removable", new ChipModelOptions { IsRemovable = true, RemoveCommand = command, });
        var passiveChip = new ChipModel("passive", "Passive", new ChipModelOptions());
        var start = new DateTimeOffset(2026, 7, 26, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(RangeDurationHours);
        var reversed = new DateTimeRange(start: end, end: start);
        var inclusive = new DateTimeRange(start, end, DateTimeRangePreset.Custom, null, true, null);
        var noCommandAction = new EmptyStateModel("None", primaryActionText: "Retry");
        var descriptor = new FilterDescriptor("date", "Date", FilterEditorKind.Date, null, [], null, false);
        var activeExpression = new FilterExpression("date", FilterOperator.Equals, start);
        var inactiveExpression = new FilterExpression("date", FilterOperator.Equals, " ");
        var readyPanel = new DataFilterPanelState([descriptor], [activeExpression, inactiveExpression], true, false);
        var applyingPanel = new DataFilterPanelState([descriptor], [activeExpression], true, true);

        await Assert.That(disabledChip.IsInteractive).IsFalse();
        await Assert.That(disabledChip.CanRemove).IsFalse();
        await Assert.That(removableChip.CanRemove).IsTrue();
        await Assert.That(removableChip.IsInteractive).IsTrue();
        await Assert.That(passiveChip.IsInteractive).IsFalse();
        await Assert.That(reversed.ValidationMessage).IsEqualTo("Start must be before or equal to end.");
        await Assert.That(reversed.Contains(start)).IsFalse();
        await Assert.That(inclusive.Contains(end)).IsTrue();
        await Assert.That(noCommandAction.HasPrimaryAction).IsFalse();
        await Assert.That(descriptor.HasChoices).IsFalse();
        await Assert.That(descriptor.CreateDisplayText(FilterOperator.Equals, start)).Contains("2026-07-26 08:00");
        await Assert.That(readyPanel.CanApply).IsTrue();
        await Assert.That(readyPanel.CanClear).IsTrue();
        await Assert.That(applyingPanel.CanApply).IsFalse();
        await Assert.That(applyingPanel.CanClear).IsFalse();
        await Assert.That(activeExpression.ToToken().DisplayText).Contains("date Equals");
    }

    /// <summary>Verifies descriptor, workflow, and summary models retain their null and value alternatives.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DescriptorWorkflowAndSummaryModels_ProjectAllValueAlternatives()
    {
        var command = new DelegateCommand();
        var blocking = new ValidationMessage(ExpressionFieldKey, FieldDisplayName, InvalidDisplayName);
        var pending = new ValidationMessage(ExpressionFieldKey, FieldDisplayName, "Checking", ValidationSeverity.Pending);
        var nullValue = new PropertyDescriptorModel("null", "Null", new PropertyDescriptorOptions { Value = null, OriginalValue = null, });
        var dateValue = new PropertyDescriptorModel("date", "Date", new PropertyDescriptorOptions { Value = new DateTime(2026, 7, 26, 8, 0, 0, DateTimeKind.Utc), });
        var booleanValue = new PropertyDescriptorModel("boolean", BooleanDisplayName, new PropertyDescriptorOptions { Value = true, OriginalValue = false, ResetCommand = command, });
        var numericValue = new PropertyDescriptorModel("number", "Number", new PropertyDescriptorOptions { Value = NumericValue, });
        var invalidDescriptor = new PropertyDescriptorModel(
            "invalid",
            InvalidDisplayName,
            new PropertyDescriptorOptions { ValidationMessages = [blocking], });
        var invalidGroup = new PropertyDescriptorGroup(InvalidDisplayName, [invalidDescriptor]);
        var available = new StepDescriptor("available", "Available", new StepDescriptorOptions { Status = StepStatus.Active, IsOptional = true, });
        var completed = new StepDescriptor("completed", "Completed", new StepDescriptorOptions { Status = StepStatus.Completed, });
        var workflow = new StepperState([completed, available], "available", StepperOrientation.Vertical);
        var summary = new ValidationSummaryState([blocking, pending]);

        await Assert.That(nullValue.ValueDisplayText).IsEmpty();
        await Assert.That(dateValue.ValueDisplayText).IsEqualTo("2026-07-26 08:00");
        await Assert.That(booleanValue.ValueDisplayText).IsEqualTo("True");
        await Assert.That(numericValue.ValueDisplayText).IsEqualTo("12.5");
        await Assert.That(booleanValue.CanReset).IsTrue();
        await Assert.That(invalidGroup.HasValidationErrors).IsTrue();
        await Assert.That(available.DisplayTitle).IsEqualTo("Available (optional)");
        await Assert.That(available.IsAvailable).IsTrue();
        await Assert.That(workflow.CanGoPrevious).IsTrue();
        await Assert.That(workflow.CanGoNext).IsFalse();
        await Assert.That(workflow.CanFinish).IsTrue();
        await Assert.That(workflow.GetStep(MissingKey)).IsNull();
        await Assert.That(summary.GetMessagesForField(" ")).IsEmpty();
        await Assert.That(summary.FirstError).IsEqualTo(blocking);
    }

    /// <summary>Verifies runtime key navigation retries view resolution after a view-model resolution miss.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RuntimeNavigationKeys_FallBackFromViewModelToViewResolution()
    {
        const string hostName = "resolution-fallback";
        using var setup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(hostName);
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
        var host = new ViewModelRoutedViewHostMixinsTests.TestResolvedViewModelRoutedViewHost(hostName);
        var navigator = new ViewFallbackNavigator();
        Splat.AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        Splat.AppLocator.CurrentMutable.RegisterConstant<IBidirectionalNavigator>(navigator);
        setup.SetMainNavigationHost(host);

        try
        {
            consumer.NavigateTo(typeof(TestNavigationView), new NavigationRequestOptions { HostName = hostName, });

            await Assert.That(host.LastResolution).IsNotNull();
            await Assert.That(host.LastResolution!.View).IsTypeOf<TestNavigationView>();
            await Assert.That(navigator.ViewModelRequestCount).IsEqualTo(1);
            await Assert.That(navigator.ViewRequestCount).IsEqualTo(1);
        }
        finally
        {
            Splat.AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        }
    }

    /// <summary>Verifies runtime key navigation propagates unexpected navigator failures.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RuntimeNavigationKeys_PropagateUnexpectedNavigatorFailures()
    {
        const string hostName = "resolution-unexpected";
        using var setup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(hostName);
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
        var host = new ViewModelRoutedViewHostMixinsTests.TestResolvedViewModelRoutedViewHost(hostName);
        var navigator = new ViewFallbackNavigator { UseUnexpectedError = true, };
        Splat.AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        Splat.AppLocator.CurrentMutable.RegisterConstant<IBidirectionalNavigator>(navigator);
        setup.SetMainNavigationHost(host);

        try
        {
            await Assert.That(() => consumer.NavigateTo(typeof(TestNavigationView), new NavigationRequestOptions { HostName = hostName, }))
                .Throws<InvalidOperationException>();
        }
        finally
        {
            Splat.AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        }
    }

    /// <summary>Verifies the remaining public value-state alternatives used by templates and persistence.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CoreValueStates_ExposeRemainingPublicAlternatives()
    {
        var command = new DelegateCommand();
        var descriptor = new FilterDescriptor("choice", "Choice", FilterEditorKind.Text, null, ["one"], null, false);
        var panelWithoutEdits = new DataFilterPanelState([descriptor], [], false, false);
        var panelWithoutFilters = new DataFilterPanelState([descriptor], [], true, false);
        var lookupWithNoContract = new NavigationLookupKey(NavigationSourceKind.View, typeof(string), null);
        var request = new NavigationRequest(NavigationSourceKind.View, null, typeof(string), null, null, NavigationType.New, CancellationToken.None);
        var middlePage = new PaginationState(1, PagingPageSize, TotalItemCount);
        var emptyText = new PropertyDescriptorModel("empty", "Empty", new PropertyDescriptorOptions { Value = " ", });
        var falseValue = new PropertyDescriptorModel("false", "False", new PropertyDescriptorOptions { Value = false, });
        var modified = new PropertyDescriptorModel("modified", "Modified", new PropertyDescriptorOptions { Value = "new", OriginalValue = "old", ResetCommand = command, });
        var grid = new PropertyGridState([modified], "modified", false);
        var query = new SearchQueryState(" query ", filters: []);
        var optional = new StepDescriptor("optional", "Optional", new StepDescriptorOptions { IsOptional = false, Status = StepStatus.Skipped, });
        var first = new StepDescriptor(FirstKey, "First", new StepDescriptorOptions { Status = StepStatus.Active, CanLeave = false, });
        var second = new StepDescriptor("second", "Second");
        var stepper = new StepperState([first, second], FirstKey);
        var highContrast = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Dark, true);
        var warning = new ValidationMessage(ExpressionFieldKey, FieldDisplayName, "Warning", ValidationSeverity.Warning);
        var unknownSeverity = new ValidationMessage(ExpressionFieldKey, FieldDisplayName, "Other", (ValidationSeverity)UndefinedPresetValue);
        var summary = new ValidationSummaryState([warning, unknownSeverity]);

        await Assert.That(descriptor.HasChoices).IsTrue();
        await Assert.That(descriptor.CreateDisplayText(FilterOperator.Equals, null)).EndsWith(" ");
        await Assert.That(panelWithoutEdits.CanApply).IsFalse();
        await Assert.That(panelWithoutFilters.CanClear).IsFalse();
        _ = lookupWithNoContract.GetHashCode();
        await Assert.That(request.Contract).IsNull();
        await Assert.That(middlePage.CanGoFirst).IsTrue();
        await Assert.That(middlePage.CanGoNext).IsTrue();
        await Assert.That(emptyText.HasValue).IsFalse();
        await Assert.That(falseValue.ValueDisplayText).IsEqualTo("False");
        await Assert.That(modified.IsModified).IsTrue();
        await Assert.That(grid.CanCommit).IsTrue();
        await Assert.That(query.HasQuery).IsTrue();
        await Assert.That(query.IsFiltered).IsFalse();
        await Assert.That(optional.IsComplete).IsTrue();
        await Assert.That(stepper.CanGoNext).IsFalse();
        await Assert.That(highContrast.IsHighContrastEffective).IsTrue();
        await Assert.That(highContrast.SupportsChoice(ThemeChoice.HighContrast)).IsTrue();
        await Assert.That(summary.WarningCount).IsEqualTo(1);
        await Assert.That(summary.SummaryText).IsEqualTo("1 warning");
    }

    /// <summary>Verifies observable list helpers ignore null list snapshots before valid values arrive.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ObservableListHelpers_IgnoreNullSnapshots()
    {
        using var source = new StateSignal<IEnumerable<IObservable<int>>?>(null);
        var resultCount = 0;
        using var subscription = source.AnyMatch(static value => value > 0).Subscribe(_ => resultCount++);

        source.OnNext(null!);

        await Assert.That(resultCount).IsEqualTo(0);
    }

    /// <summary>Verifies host operations provide the documented failure when no host has been registered.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HostedNavigation_RequiresAtLeastOneRegisteredHost()
    {
        var hosts = new Dictionary<string, IViewModelRoutedViewHost>();
        foreach (var host in ViewModelRoutedViewHostMixins.NavigationHost)
        {
            hosts.Add(host.Key, host.Value);
        }

        ViewModelRoutedViewHostMixins.NavigationHost.Clear();

        try
        {
            using var consumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
            await Assert.That(() => consumer.ClearHistory("missing-host")).Throws<InvalidOperationException>();
        }
        finally
        {
            foreach (var host in hosts)
            {
                ViewModelRoutedViewHostMixins.NavigationHost[host.Key] = host.Value;
            }
        }
    }

    /// <summary>Verifies setup observers safely ignore absent hosts and absent setup signals.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task HostedSetupObservers_IgnoreUnavailableHostsAndSignals()
    {
        const string configuredHostName = "observer-host";
        const string manuallyRegisteredHostName = "observer-manual";
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestHostedViewModel();
        using var nullHostSubscription = consumer.CanNavigateBack(null!).Subscribe(static _ => { });
        using var missingHostSubscription = consumer.WhenSetup(MissingKey).Subscribe(static _ => { });
        ViewModelRoutedViewHostMixins.NavigationHost[manuallyRegisteredHostName] =
            new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(manuallyRegisteredHostName);
        using var missingSignalSubscription = consumer.WhenSetup(manuallyRegisteredHostName).Subscribe(static _ => { });
        using var setup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(configuredHostName);
        var configuredHost = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(configuredHostName);

        try
        {
            Splat.AppLocator.CurrentMutable.SetupComplete();
            setup.SetMainNavigationHost(configuredHost);
            await Assert.That(configuredHost.Name).IsEqualTo(configuredHostName);
        }
        finally
        {
            _ = ViewModelRoutedViewHostMixins.NavigationHost.Remove(manuallyRegisteredHostName);
        }
    }

    /// <summary>Verifies primary setup observers safely ignore null host names and unavailable setup signals.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PrimarySetupObservers_IgnoreUnavailableHostsAndSignals()
    {
        const string configuredHostName = "primary-observer-host";
        const string manuallyRegisteredHostName = "primary-observer-manual";
        using var unnamedConsumer = new ViewModelRoutedViewHostMixinsTests.TestViewModel(null);
        using var nullHostSubscription = unnamedConsumer.CanNavigateBack().Subscribe(static _ => { });
        using var nullNameSetupSubscription = unnamedConsumer.WhenSetup().Subscribe(static _ => { });
        ViewModelRoutedViewHostMixins.NavigationHost[manuallyRegisteredHostName] =
            new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(manuallyRegisteredHostName);
        using var manualConsumer = new ViewModelRoutedViewHostMixinsTests.TestViewModel(manuallyRegisteredHostName);
        using var missingSignalSubscription = manualConsumer.WhenSetup().Subscribe(static _ => { });
        using var setup = new ViewModelRoutedViewHostMixinsTests.TestSetNavigationViewModel(configuredHostName);
        var configuredHost = new ViewModelRoutedViewHostMixinsTests.TestViewModelRoutedViewHost(configuredHostName);

        try
        {
            Splat.AppLocator.CurrentMutable.SetupComplete();
            setup.SetMainNavigationHost(configuredHost);
            await Assert.That(configuredHost.Name).IsEqualTo(configuredHostName);
        }
        finally
        {
            _ = ViewModelRoutedViewHostMixins.NavigationHost.Remove(manuallyRegisteredHostName);
        }
    }

    /// <summary>Verifies primary setup observers tolerate an empty host registry.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PrimarySetupObservers_IgnoreAnEmptyHostRegistry()
    {
        var hosts = new Dictionary<string, IViewModelRoutedViewHost>();
        foreach (var host in ViewModelRoutedViewHostMixins.NavigationHost)
        {
            hosts.Add(host.Key, host.Value);
        }

        ViewModelRoutedViewHostMixins.NavigationHost.Clear();
        using var consumer = new ViewModelRoutedViewHostMixinsTests.TestViewModel("unregistered-host");
        using var backSubscription = consumer.CanNavigateBack().Subscribe(static _ => { });
        using var setupSubscription = consumer.WhenSetup().Subscribe(static _ => { });

        try
        {
            Splat.AppLocator.CurrentMutable.SetupComplete();
            await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost).IsEmpty();
        }
        finally
        {
            foreach (var host in hosts)
            {
                ViewModelRoutedViewHostMixins.NavigationHost[host.Key] = host.Value;
            }
        }
    }

    /// <summary>Provides a command that is available and has no side effects.</summary>
    private sealed class DelegateCommand : System.Windows.Input.ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => true;

        /// <inheritdoc/>
        public void Execute(object? parameter) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Provides a concrete view model for generic navigation value objects.</summary>
    private sealed class TestNavigationViewModel : RxObject;

    /// <summary>Provides a concrete view for generic navigation value objects.</summary>
    private sealed class TestNavigationView : ReactiveUI.IViewFor<TestNavigationViewModel>
    {
        /// <inheritdoc/>
        public TestNavigationViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? ReactiveUI.IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestNavigationViewModel?)value;
        }
    }

    /// <summary>Provides a routed view model that records lifecycle callbacks.</summary>
    /// <param name="name">The stable routed view-model name.</param>
    private sealed class RoutableViewModel(string name) : RxObject, INotifiyRoutableViewModel
    {
        /// <inheritdoc/>
        string? IUseHostedNavigation.Name => name;

        /// <summary>Gets the number of completed-from callbacks.</summary>
        public int NavigatedFromCount { get; private set; }

        /// <summary>Gets the number of completed-to callbacks.</summary>
        public int NavigatedToCount { get; private set; }

        /// <summary>Gets the number of pending-navigation callbacks.</summary>
        public int NavigatingCount { get; private set; }

        /// <inheritdoc/>
        public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e) => NavigatedFromCount++;

        /// <inheritdoc/>
        public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables) => NavigatedToCount++;

        /// <inheritdoc/>
        public override void WhenNavigating(IViewModelNavigatingEventArgs e) => NavigatingCount++;
    }

    /// <summary>Provides a routed view model of a distinct runtime type for callback-filter scenarios.</summary>
    private sealed class UnrelatedRoutableViewModel : RxObject
    {
        /// <inheritdoc/>
        public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e) { }

        /// <inheritdoc/>
        public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables) { }

        /// <inheritdoc/>
        public override void WhenNavigating(IViewModelNavigatingEventArgs e) { }
    }

    /// <summary>Provides a navigator that exercises runtime view fallback behavior.</summary>
    private sealed class ViewFallbackNavigator : IBidirectionalNavigator
    {
        /// <summary>Gets or sets whether runtime view-model resolution fails unexpectedly.</summary>
        public bool UseUnexpectedError { get; set; }

        /// <summary>Gets the number of view-model resolution attempts.</summary>
        public int ViewModelRequestCount { get; private set; }

        /// <summary>Gets the number of view resolution attempts.</summary>
        public int ViewRequestCount { get; private set; }

        /// <inheritdoc/>
        public IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(ViewModelNavigationRequest<TViewModel, TView> request)
            where TViewModel : class, IRxObject
            where TView : class, ReactiveUI.IViewFor<TViewModel> =>
            Observable.Throw<NavigationResolution<TViewModel, TView>>(new NavigationResolutionException("Typed navigation is unavailable."));

        /// <inheritdoc/>
        public IObservable<NavigationResolution> NavigateViewModel(Type viewModelKey, NavigationRequestOptions options)
        {
            ViewModelRequestCount++;
            return Observable.Throw<NavigationResolution>(UseUnexpectedError
                ? new InvalidOperationException("Unexpected navigation failure.")
                : new NavigationResolutionException("View-model navigation is unavailable."));
        }

        /// <inheritdoc/>
        public IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(ViewNavigationRequest<TViewModel, TView> request)
            where TViewModel : class, IRxObject
            where TView : class, ReactiveUI.IViewFor<TViewModel> =>
            NavigateViewModel<TViewModel, TView>(null!);

        /// <inheritdoc/>
        public IObservable<NavigationResolution> NavigateView(Type viewKey, NavigationRequestOptions options)
        {
            ViewRequestCount++;
            return Observable.Return(new NavigationResolution(new TestNavigationViewModel(), new TestNavigationView(), options.Contract, options.Parameter, NavigationType.New));
        }
    }
}
