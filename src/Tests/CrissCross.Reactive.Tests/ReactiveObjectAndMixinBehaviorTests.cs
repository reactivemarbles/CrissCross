// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace CrissCross.Reactive.Tests;

/// <summary>Exercises reactive-shim object and observable-mixin behavior through their public APIs.</summary>
public sealed class ReactiveObjectAndMixinBehaviorTests
{
    /// <summary>Provides the expected count for a pair of values.</summary>
    private const int PairCount = 2;

    /// <summary>Provides the expected number of distinct normalized contracts.</summary>
    private const int KnownContractCount = 3;

    /// <summary>Provides the page size used by value-object coverage.</summary>
    private const int PageSize = 5;

    /// <summary>Provides the highest valid page index for the constrained pagination test.</summary>
    private const int FinalPageIndex = 1;

    /// <summary>Provides the page size used when testing pagination clamping.</summary>
    private const int ConstrainedPageSize = 3;

    /// <summary>Provides the lower filter bound used by state projection coverage.</summary>
    private const int MinimumPriority = 1;

    /// <summary>Provides the upper filter bound used by state projection coverage.</summary>
    private const int MaximumPriority = 9;

    /// <summary>Provides the stable priority filter field key.</summary>
    private const string PriorityField = "priority";

    /// <summary>Provides a shared exception message.</summary>
    private const string Message = "message";

    /// <summary>Provides the padded contract used to verify normalization.</summary>
    private const string PaddedContract = " contract ";

    /// <summary>Provides the normalized navigation contract.</summary>
    private const string Contract = "contract";

    /// <summary>Verifies build-completion subscriptions replay, receive later signals, and honor disposal.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BuildCompletion_ReplaysSignalsAndStopsDisposedSubscriptions()
    {
        using var target = new DisposableProbe();
        var callbackCount = 0;
        var directCallbackCount = 0;
        var subscription = target.BuildCompleteDisposable(() => callbackCount++);
        var replayedCallbackCount = callbackCount;
        target.BuildComplete(() => directCallbackCount++);
        var replayedDirectCallbackCount = directCallbackCount;

        AppLocator.CurrentMutable.SetupComplete();

        var callbackCountAfterSetup = callbackCount;
        var directCallbackCountAfterSetup = directCallbackCount;
        await Assert.That(callbackCountAfterSetup).IsGreaterThan(replayedCallbackCount);
        await Assert.That(directCallbackCountAfterSetup).IsGreaterThan(replayedDirectCallbackCount);

        subscription.Dispose();
        AppLocator.CurrentMutable.SetupComplete();

        await Assert.That(callbackCount).IsEqualTo(callbackCountAfterSetup);
    }

    /// <summary>Verifies build-completion helpers reject missing targets and callbacks.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BuildCompletion_RejectsMissingTargetAndCallback()
    {
        IAmBuilt? missingTarget = null;
        using var target = new DisposableProbe();

        await Assert.That(() => missingTarget!.BuildCompleteDisposable(static () => { })).Throws<ArgumentNullException>();
        await Assert.That(() => target.BuildComplete(null!)).Throws<ArgumentNullException>();
        await Assert.That(() => target.BuildCompleteDisposable(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies matching handles null and empty lists, suppresses duplicates, and publishes transitions.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AnyMatch_HandlesNullEmptyDuplicateAndTransitioningInnerValues()
    {
        using var first = new StateSignal<int>(0);
        using var second = new StateSignal<int>(0);
        using var source = new StateSignal<IEnumerable<IObservable<int>>>(null!);
        var results = new List<bool>();
        using var subscription = source.AnyMatch(static value => value > 0).Subscribe(results.Add);

        source.OnNext([]);
        source.OnNext([first, second]);
        first.OnNext(0);
        first.OnNext(1);
        second.OnNext(1);
        first.OnNext(0);
        second.OnNext(0);

        await Assert.That(results).IsEquivalentTo([false, true, false]);
    }

    /// <summary>Verifies observable-list projection ignores null sources and replaces its current projection.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToListOfObservables_IgnoresNullSourceAndReplacesProjection()
    {
        using var first = new ObservableItem { Value = "first" };
        using var second = new ObservableItem { Value = "second" };
        using var source = new StateSignal<IEnumerable<ObservableItem>>(null!);
        var projectionCounts = new List<int>();
        using var subscription = source
            .ToListOfObservables(static item => item.Value)
            .Subscribe(values => projectionCounts.Add(CountValues(values)));

        source.OnNext([first]);
        source.OnNext([first, second]);

        await Assert.That(projectionCounts).IsEquivalentTo([FinalPageIndex, PairCount]);
    }

    /// <summary>Verifies finalizer-path disposal leaves managed disposable ownership intact.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RxObject_FinalizerPathDoesNotDisposeManagedResources()
    {
        using var target = new DisposableProbe();

        target.DisposeAsFinalizerWould();

        await Assert.That(target.IsDisposed).IsFalse();
    }

    /// <summary>Verifies default lifecycle hooks accept complete and pending navigation notifications.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RxObject_DefaultNavigationLifecycleHooksAcceptNotifications()
    {
        using var target = new DisposableProbe();
        using var lifetime = new CompositeDisposable();
        var completed = new ViewModelNavigationEventArgs(target, null, NavigationType.New, null, "reactive-host");
        var pending = new ViewModelNavigatingEventArgs(target, null, NavigationType.New, null, "reactive-host");

        target.WhenNavigatedFrom(completed);
        target.WhenNavigatedTo(completed, lifetime);
        target.WhenNavigating(pending);

        await Assert.That(target.IsDisposed).IsFalse();
    }

    /// <summary>Verifies forward-navigation boundaries and journal clearing use their documented empty state.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationJournal_HandlesForwardBoundariesAndClear()
    {
        var journal = new List<string> { "home" };
        var currentIndex = -1;

        var movesFromBeforeStart = NavigationJournal.TryMoveForward(journal, currentIndex, out var unchangedIndex, out var missingEntry);
        currentIndex = 0;
        var movesFromFinalEntry = NavigationJournal.TryMoveForward(journal, currentIndex, out var finalIndex, out var finalEntry);
        NavigationJournal.Clear(journal, ref currentIndex);

        await Assert.That(movesFromBeforeStart).IsFalse();
        await Assert.That(unchangedIndex).IsEqualTo(-1);
        await Assert.That(missingEntry).IsNull();
        await Assert.That(movesFromFinalEntry).IsFalse();
        await Assert.That(finalIndex).IsEqualTo(0);
        await Assert.That(finalEntry).IsNull();
        await Assert.That(journal).IsEmpty();
        await Assert.That(currentIndex).IsEqualTo(-1);
    }

    /// <summary>Verifies simple reactive value objects preserve their public default and exceptional states.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValueObjects_ProjectDelegatingConstructorsAndBoundaryStates()
    {
        var filters = new[]
        {
            new FilterToken("first", FilterOperator.Equals, "one", "First"),
            new FilterToken("second", FilterOperator.Contains, "two", "Second"),
        };
        var query = new SearchQueryState(" text ", filters: filters);
        var basicPage = new PageRequest(-1, 0);
        var sortedPage = new PageRequest(FinalPageIndex, PageSize, "name", true, query);
        var customOperators = new[] { FilterOperator.NotEquals };
        var descriptorWithOperators = new FilterDescriptor("state", "State", FilterEditorKind.Enum, customOperators);
        var descriptorWithChoices = new FilterDescriptor("kind", "Kind", FilterEditorKind.Enum, customOperators, ["a"]);
        var descriptorWithDefault = new FilterDescriptor(PriorityField, "Priority", FilterEditorKind.Number, customOperators, null, "high");
        var emptyGroup = new PropertyDescriptorGroup("General");
        var defaultStep = new StepDescriptor("step", "Step");
        var defaultChip = new ChipModel("chip", "Chip");
        var defaultChipGroup = new ChipGroupState([defaultChip]);
        var defaultSegments = new SegmentedSelectionState([]);
        var emptyPagination = new PaginationState(-1, 0, -1);
        var constrainedPagination = new PaginationState(int.MaxValue, ConstrainedPageSize, PageSize);
        var yesterdayRange = new DateTimeRange(null, null, DateTimeRangePreset.Yesterday);
        var fieldMessage = new ValidationMessage(" field ", " Field ", Message);
        var missingExpression = new FilterExpression(PriorityField, FilterOperator.Equals, null);
        var emptyProperty = new PropertyDescriptorModel("property", "Property");
        var emptyGrid = new PropertyGridState();
        var dateProperty = new PropertyDescriptorModel(
            "date",
            "Date",
            new PropertyDescriptorOptions { Value = new DateTime(2026, 7, 26, 14, 30, 0, DateTimeKind.Utc), });

        await Assert.That(basicPage.PageIndex).IsEqualTo(0);
        await Assert.That(basicPage.PageSize).IsEqualTo(1);
        await Assert.That(sortedPage.FilterSnapshotKey).IsEqualTo("first:Equals:one|second:Contains:two");
        await Assert.That(sortedPage.HasQuery).IsTrue();
        await Assert.That(sortedPage.DisplayText).IsEqualTo("Page 2, 5 per page");
        await Assert.That(descriptorWithOperators.DefaultOperator).IsEqualTo(FilterOperator.NotEquals);
        await Assert.That(descriptorWithChoices.HasChoices).IsTrue();
        await Assert.That(descriptorWithDefault.DefaultValue).IsEqualTo("high");
        await Assert.That(emptyGroup.Count).IsEqualTo(0);
        await Assert.That(emptyGroup.HasModifiedDescriptors).IsFalse();
        await Assert.That(defaultStep.ValidationMessages).IsEmpty();
        await Assert.That(defaultStep.IsBlocking).IsFalse();
        await Assert.That(defaultChipGroup.Chips).Count().IsEqualTo(1);
        await Assert.That(defaultSegments.HasSelection).IsFalse();
        await Assert.That(emptyPagination.SummaryText).IsEqualTo("No items");
        await Assert.That(constrainedPagination.PageIndex).IsEqualTo(FinalPageIndex);
        await Assert.That(constrainedPagination.CreateRequest(-1).PageIndex).IsEqualTo(0);
        await Assert.That(yesterdayRange.Label).IsEqualTo("Yesterday");
        await Assert.That(fieldMessage.HasField).IsTrue();
        await Assert.That(missingExpression.IsActive).IsFalse();
        await Assert.That(missingExpression.Key).IsEqualTo($"{PriorityField}:Equals:");
        await Assert.That(emptyProperty.HasValidationMessages).IsFalse();
        await Assert.That(emptyProperty.ValueDisplayText).IsEmpty();
        await Assert.That(emptyGrid.CanReset).IsFalse();
        await Assert.That(dateProperty.ValueDisplayText).IsEqualTo("2026-07-26 14:30");
        await Assert.That(EmptyServiceProvider.Instance.GetService(typeof(object))).IsNull();
        await Assert.That(filters).Count().IsEqualTo(PairCount);
    }

    /// <summary>Verifies navigation value objects retain normalized exception and equality details.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationValueObjects_ProjectExceptionAndLookupStates()
    {
        var inner = new InvalidOperationException("inner");
        var resolutionDefault = new NavigationResolutionException();
        var resolutionMessage = new NavigationResolutionException(Message);
        var resolutionInner = new NavigationResolutionException(Message, inner);
        var resolution = new NavigationResolutionException(
            NavigationSourceKind.View,
            typeof(ObservableItem),
            PaddedContract,
            [Contract, PaddedContract, null]);
        var registrationDefault = new NavigationRegistrationException();
        var registrationMessage = new NavigationRegistrationException(Message);
        var registrationInner = new NavigationRegistrationException(Message, inner);
        var registration = new NavigationRegistrationException(NavigationSourceKind.ViewModel, typeof(ObservableItem), PaddedContract);
        var first = new NavigationLookupKey(NavigationSourceKind.ViewModel, typeof(ObservableItem), Contract);
        var matching = new NavigationLookupKey(NavigationSourceKind.ViewModel, typeof(ObservableItem), Contract);
        var different = new NavigationLookupKey(NavigationSourceKind.View, typeof(ObservableItem), Contract);

        await Assert.That(resolutionDefault.SourceKey).IsEqualTo(typeof(object));
        await Assert.That(resolutionMessage.Message).IsEqualTo(Message);
        await Assert.That(resolutionInner.InnerException).IsEqualTo(inner);
        await Assert.That(resolution.Contract).IsEqualTo(PaddedContract);
        await Assert.That(resolution.KnownContracts).Count().IsEqualTo(KnownContractCount);
        await Assert.That(registrationDefault.ServiceType).IsEqualTo(typeof(object));
        await Assert.That(registrationMessage.Message).IsEqualTo(Message);
        await Assert.That(registrationInner.InnerException).IsEqualTo(inner);
        await Assert.That(registration.Contract).IsEqualTo(PaddedContract);
        await Assert.That(first == matching).IsTrue();
        await Assert.That(first != different).IsTrue();
        await Assert.That(first.Equals((object)matching)).IsTrue();
    }

    /// <summary>Verifies state projections cover their delegated overloads and multi-item display branches.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task StateProjections_CoverDelegatedOverloadsAndMultipleValues()
    {
        var descriptor = new FilterDescriptor(PriorityField, "Priority", FilterEditorKind.Number);
        var expressions = new[]
        {
            new FilterExpression(PriorityField, FilterOperator.GreaterThan, MinimumPriority),
            new FilterExpression(PriorityField, FilterOperator.LessThan, MaximumPriority),
        };
        var emptyPanel = new DataFilterPanelState();
        var descriptorPanel = new DataFilterPanelState([descriptor]);
        var expressionPanel = new DataFilterPanelState([descriptor], expressions);
        var dirtyPanel = new DataFilterPanelState([descriptor], expressions, true);
        var systemTheme = new ThemePreferenceState(ThemeChoice.System);
        var darkTheme = new ThemePreferenceState(ThemeChoice.Dark, ThemeChoice.Light);
        var idle = CommandButtonStatus.Idle(true);
        var succeeded = CommandButtonStatus.Succeeded(true);
        var cancelled = CommandButtonStatus.Cancelled(false);

        await Assert.That(emptyPanel.DescriptorCount).IsEqualTo(0);
        await Assert.That(descriptorPanel.DescriptorCount).IsEqualTo(1);
        await Assert.That(expressionPanel.SummaryText).IsEqualTo("2 active filters");
        await Assert.That(dirtyPanel.ToSearchQueryState().ActiveFilterCount).IsEqualTo(PairCount);
        await Assert.That(dirtyPanel.ToSearchQueryState(" value ").NormalizedText).IsEqualTo("value");
        await Assert.That(systemTheme.SystemChoice).IsEqualTo(ThemeChoice.Light);
        await Assert.That(darkTheme.EffectiveChoice).IsEqualTo(ThemeChoice.Dark);
        await Assert.That(idle.State).IsEqualTo(CommandButtonState.Idle);
        await Assert.That(succeeded.State).IsEqualTo(CommandButtonState.Succeeded);
        await Assert.That(cancelled.IsInteractive).IsFalse();
    }

    /// <summary>Verifies typed navigation requests and registrations expose their runtime keys and defaults.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TypedNavigationRequests_ProjectRuntimeKeysAndDefaults()
    {
        var keyRequest = new NavigationKeyRequest<ObservableItem>();
        var viewRequest = new ViewNavigationRequest<ObservableItem, ObservableView>();
        var viewModelRequest = new ViewModelNavigationRequest<ObservableItem, ObservableView>();
        var registration = new NavigationRegistration<ObservableItem, ObservableItem, ObservableView, ObservableView>(
            static _ => new ObservableItem(),
            static _ => new ObservableView());

        await Assert.That(keyRequest.Key).IsEqualTo(typeof(ObservableItem));
        await Assert.That(keyRequest.Options).IsNotNull();
        await Assert.That(viewRequest.ViewModelType).IsEqualTo(typeof(ObservableItem));
        await Assert.That(viewModelRequest.ViewType).IsEqualTo(typeof(ObservableView));
        await Assert.That(registration.ViewModelKey).IsEqualTo(typeof(ObservableItem));
        await Assert.That(registration.ViewKey).IsEqualTo(typeof(ObservableView));
    }

    /// <summary>Counts observable values without using a LINQ iterator.</summary>
    /// <typeparam name="T">The observable value type.</typeparam>
    /// <param name="values">The values to count.</param>
    /// <returns>The number of supplied values.</returns>
    private static int CountValues<T>(IEnumerable<IObservable<T>> values)
    {
        var count = 0;
        foreach (var _ in values)
        {
            count++;
        }

        return count;
    }

    /// <summary>Provides a property-notifying item for observable projection tests.</summary>
    private sealed class ObservableItem : RxObject
    {
        /// <summary>Gets or sets the projected value.</summary>
        public string? Value
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>Provides a paired view for typed navigation request coverage.</summary>
    private sealed class ObservableView : global::ReactiveUI.IViewFor<ObservableItem>
    {
        /// <inheritdoc/>
        public ObservableItem? ViewModel { get; set; }

        /// <inheritdoc/>
        object? global::ReactiveUI.IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ObservableItem?)value;
        }
    }

    /// <summary>Exposes protected disposal behavior for behavioral coverage.</summary>
    private sealed class DisposableProbe : RxObject
    {
        /// <summary>Runs the finalizer-equivalent disposal path.</summary>
        public void DisposeAsFinalizerWould() => Dispose(false);
    }
}
