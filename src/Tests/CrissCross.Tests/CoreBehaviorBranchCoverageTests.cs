// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.Tests;

/// <summary>Exercises public edge behavior that feeds core branch coverage.</summary>
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class CoreBehaviorBranchCoverageTests
{
    /// <summary>Provides the created-date field key used by filter scenarios.</summary>
    private const string CreatedKey = "created";

    /// <summary>Provides the display name used by created-date filter scenarios.</summary>
    private const string CreatedDisplayName = "Created";

    /// <summary>Provides a whitespace-padded navigation contract.</summary>
    private const string DetailContract = " detail ";

    /// <summary>Provides a key that should not match lookups.</summary>
    private const string MissingKey = "missing";

    /// <summary>Provides the name field key used by filter scenarios.</summary>
    private const string NameKey = "name";

    /// <summary>Provides the pending step key.</summary>
    private const string PendingKey = "pending";

    /// <summary>Provides the review category and step key.</summary>
    private const string ReviewKey = "review";

    /// <summary>Provides the status field key.</summary>
    private const string StatusKey = "status";

    /// <summary>Provides the summary field and step key.</summary>
    private const string SummaryKey = "summary";

    /// <summary>Provides the summary display name used by descriptor scenarios.</summary>
    private const string SummaryDisplayName = "Summary";

    /// <summary>Provides the trimmed title key expected from descriptor construction.</summary>
    private const string TitleKey = "title";

    /// <summary>Provides a common object value for token scenarios.</summary>
    private const string ValueText = "value";

    /// <summary>Provides the expected count for single-item assertions.</summary>
    private const int ExpectedSingleCount = 1;

    /// <summary>Provides a count used for date ranges and result summaries.</summary>
    private const int ExpectedTwoCount = 2;

    /// <summary>Provides the zero-based last page index for a five-item collection with two-item pages.</summary>
    private const int LastPageIndex = 2;

    /// <summary>Provides the minimum clamped page size.</summary>
    private const int OneItemPageSize = 1;

    /// <summary>Provides a standard two-item page size.</summary>
    private const int TwoItemPageSize = 2;

    /// <summary>Provides the total item count used by paging boundary tests.</summary>
    private const int TotalItemCount = 5;

    /// <summary>Provides an undefined enum value used by fallback formatting tests.</summary>
    private const int UndefinedEnumValue = 999;

    /// <summary>Gets a debugger-safe representation of this test fixture.</summary>
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Verifies disposal paths stay idempotent and protected false-dispose leaves owned disposables alive.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DisposalLifetime_PublicAndProtectedPathsRemainIdempotent()
    {
        var rxObject = new ExposedDisposeRxObject();
        var wasDisposed = false;
        rxObject.GetDisposables().Add(new ActionDisposable(() => wasDisposed = true));

        rxObject.DisposeFromTest(false);
        var disposedAfterFalsePath = rxObject.IsDisposed;

        rxObject.DisposeFromTest(true);
        rxObject.DisposeFromTest(true);

        await Assert.That(disposedAfterFalsePath).IsFalse();
        await Assert.That(wasDisposed).IsTrue();
        await Assert.That(rxObject.IsDisposed).IsTrue();
    }

    /// <summary>Verifies event-signal subscriptions remove handlers exactly once and stop forwarding after disposal.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EventSignals_UnsubscribeOnceAndStopForwardingAfterDisposal()
    {
        var source = new EventSource();
        var received = new List<EventArgs>();
        var observable = EventSignal.From<EventArgs>(source.AddHandler, source.RemoveHandler);

        var subscription = observable.Subscribe(received.Add);
        source.Raise(EventArgs.Empty);
        subscription.Dispose();
        subscription.Dispose();
        source.Raise(EventArgs.Empty);

        await Assert.That(source.AddCount).IsEqualTo(ExpectedSingleCount);
        await Assert.That(source.RemoveCount).IsEqualTo(ExpectedSingleCount);
        await Assert.That(received.Count).IsEqualTo(ExpectedSingleCount);
    }

    /// <summary>Verifies date range projections for reversed ranges and inclusive/exclusive boundaries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateRanges_ProjectReversedAndBoundaryContainmentStates()
    {
        var start = new DateTimeOffset(2026, 9, 6, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(ExpectedTwoCount);
        var reversedStart = end;
        var reversedEnd = start;
        var reversed = new DateTimeRange(reversedStart, reversedEnd, DateTimeRangePreset.Custom, ReviewKey);
        var inclusive = new DateTimeRange(
            start,
            end,
            DateTimeRangePreset.Today,
            null,
            true,
            TimeSpan.FromHours(ExpectedTwoCount));
        var exclusive = new DateTimeRange(start, end, DateTimeRangePreset.Yesterday, null, false, null);

        await Assert.That(reversed.HasValue).IsTrue();
        await Assert.That(reversed.IsReversed).IsTrue();
        await Assert.That(reversed.ExceedsMaximumDuration).IsFalse();
        await Assert.That(reversed.Duration).IsEqualTo(TimeSpan.Zero);
        await Assert.That(reversed.ValidationMessage).IsEqualTo("Start must be before or equal to end.");
        await Assert.That(reversed.DisplayText).IsEqualTo("review: invalid range");
        await Assert.That(inclusive.Contains(end)).IsTrue();
        await Assert.That(exclusive.Contains(start.AddTicks(-ExpectedSingleCount))).IsFalse();
        await Assert.That(exclusive.Contains(end)).IsFalse();
    }

    /// <summary>Verifies search, pagination, and theme boundary states with nullable or unsupported inputs.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SharedStates_ProjectNullableEmptyAndUnsupportedBoundaries()
    {
        var emptySearch = new SearchQueryState();
        var filteredSearch = new SearchQueryState(
            " query ",
            resultCount: ExpectedTwoCount,
            filters: [new FilterToken(StatusKey, FilterOperator.Equals, ValueText, "Status equals value")]);
        var emptyPage = new PaginationState(0, 0, 0);
        var lastPage = new PaginationState(LastPageIndex, TwoItemPageSize, TotalItemCount);
        var unsupportedHighContrast = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Dark, false);
        var unknownTheme = new ThemePreferenceState((ThemeChoice)UndefinedEnumValue, (ThemeChoice)UndefinedEnumValue, false);

        await Assert.That(emptySearch.NormalizedText).IsEmpty();
        await Assert.That(emptySearch.HasQuery).IsFalse();
        await Assert.That(emptySearch.ActiveFilters.Count).IsEqualTo(0);
        await Assert.That(filteredSearch.NormalizedText).IsEqualTo("query");
        await Assert.That(filteredSearch.IsFiltered).IsTrue();
        await Assert.That(filteredSearch.ResultSummary).IsEqualTo("2 results");
        await Assert.That(emptyPage.PageSize).IsEqualTo(OneItemPageSize);
        await Assert.That(emptyPage.FirstItemNumber).IsEqualTo(0);
        await Assert.That(emptyPage.LastItemNumber).IsEqualTo(0);
        await Assert.That(emptyPage.SummaryText).IsEqualTo("No items");
        await Assert.That(lastPage.FirstItemNumber).IsEqualTo(TotalItemCount);
        await Assert.That(lastPage.LastItemNumber).IsEqualTo(TotalItemCount);
        await Assert.That(lastPage.CanGoNext).IsFalse();
        await Assert.That(lastPage.CanGoLast).IsFalse();
        await Assert.That(unsupportedHighContrast.SupportsChoice(ThemeChoice.HighContrast)).IsFalse();
        await Assert.That(unsupportedHighContrast.DisplayText).IsEqualTo("High contrast (using Dark)");
        await Assert.That(unknownTheme.EffectiveChoice).IsEqualTo(ThemeChoice.Light);
        await Assert.That(unknownTheme.DisplayText).IsEqualTo("System");
    }

    /// <summary>Verifies filter descriptors and expressions expose fallback operator, value formatting, and activity behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Filters_ProjectFallbackOperatorsValueFormatsAndActivityStates()
    {
        var when = new DateTimeOffset(2026, 9, 6, 8, 0, 0, TimeSpan.Zero);
        var explicitDescriptor = new FilterDescriptor(
            CreatedKey,
            CreatedDisplayName,
            FilterEditorKind.DateTime,
            [FilterOperator.NotEquals],
            [],
            null,
            false);
        var fallbackDescriptor = new FilterDescriptor(StatusKey, "Status", (FilterEditorKind)UndefinedEnumValue);
        var disabledExpression = new FilterExpression(NameKey, FilterOperator.Contains, ValueText, NameKey, false);
        var whitespaceExpression = new FilterExpression(NameKey, FilterOperator.Contains, "   ");
        var objectExpression = new FilterExpression(CreatedKey, FilterOperator.Equals, when);
        var fallbackToken = new FilterExpression(StatusKey, (FilterOperator)UndefinedEnumValue, ValueText).ToToken();
        var describedToken = objectExpression.ToToken(explicitDescriptor);

        await Assert.That(explicitDescriptor.DefaultOperator).IsEqualTo(FilterOperator.NotEquals);
        await Assert.That(explicitDescriptor.HasChoices).IsFalse();
        await Assert.That(explicitDescriptor.SupportsOperator(FilterOperator.Contains)).IsFalse();
        await Assert.That(explicitDescriptor.CreateDisplayText(FilterOperator.Equals, when)).IsEqualTo("Created equals 2026-09-06 08:00");
        await Assert.That(explicitDescriptor.CreateDisplayText((FilterOperator)UndefinedEnumValue, null)).IsEqualTo("Created 999 ");
        await Assert.That(fallbackDescriptor.DefaultOperator).IsEqualTo(FilterOperator.Equals);
        await Assert.That(disabledExpression.IsActive).IsFalse();
        await Assert.That(whitespaceExpression.IsActive).IsFalse();
        await Assert.That(objectExpression.IsActive).IsTrue();
        await Assert.That(fallbackToken.DisplayText).IsEqualTo("status 999 value");
        await Assert.That(describedToken.Operator).IsEqualTo(FilterOperator.Equals);
        await Assert.That(describedToken.DisplayText).IsEqualTo("Created equals 2026-09-06 08:00");
    }

    /// <summary>Verifies property-grid descriptors and groups preserve fallback state and summary behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PropertyGrid_ProjectFallbackGroupsAndCommitBoundaries()
    {
        var resetCommand = new TestCommand();
        var general = new PropertyDescriptorModel(
            " title ",
            " Title ",
            new PropertyDescriptorOptions { Value = string.Empty, OriginalValue = string.Empty, TemplateKey = "   ", Choices = null, ValidationMessages = null });
        var modified = new PropertyDescriptorModel(
            StatusKey,
            "Status",
            new PropertyDescriptorOptions { Category = ReviewKey, Value = true, OriginalValue = false, ResetCommand = resetCommand });
        var invalid = new PropertyDescriptorModel(
            SummaryKey,
            SummaryDisplayName,
            new PropertyDescriptorOptions { Value = new DateTimeOffset(2026, 9, 6, 8, 0, 0, TimeSpan.Zero), ValidationMessages = [new ValidationMessage(SummaryKey, SummaryDisplayName, "Required")] });
        var emptyGroup = new PropertyDescriptorGroup("   ");
        var modifiedGroup = new PropertyDescriptorGroup(ReviewKey, [modified]);
        var state = new PropertyGridState([general, modified, invalid], "TRUE", false);
        var committingState = new PropertyGridState([modified], null, true);

        await Assert.That(general.Key).IsEqualTo(TitleKey);
        await Assert.That(general.Category).IsEqualTo("General");
        await Assert.That(general.TemplateKey).IsNull();
        await Assert.That(general.HasValue).IsFalse();
        await Assert.That(general.HasChoices).IsFalse();
        await Assert.That(general.IsModified).IsFalse();
        await Assert.That(general.ValueDisplayText).IsEmpty();
        await Assert.That(modified.ValueDisplayText).IsEqualTo("True");
        await Assert.That(invalid.ValueDisplayText).IsEqualTo("2026-09-06 08:00");
        await Assert.That(emptyGroup.Name).IsEqualTo("General");
        await Assert.That(emptyGroup.HasModifiedDescriptors).IsFalse();
        await Assert.That(emptyGroup.HasValidationErrors).IsFalse();
        await Assert.That(modifiedGroup.HasModifiedDescriptors).IsTrue();
        await Assert.That(state.VisibleDescriptorCount).IsEqualTo(ExpectedSingleCount);
        await Assert.That(state.SummaryText).IsEqualTo("3 properties, 1 invalid");
        await Assert.That(state.CanCommit).IsFalse();
        await Assert.That(state.CanReset).IsTrue();
        await Assert.That(committingState.CanReset).IsFalse();
        await Assert.That(state.GetDescriptor(MissingKey)).IsNull();
    }

    /// <summary>Verifies step descriptors and steppers expose blocking, skipped, and missing-current boundaries.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Stepper_ProjectBlockingSkippedAndMissingCurrentBoundaries()
    {
        var first = new StepDescriptor(
            PendingKey,
            "Pending",
            new StepDescriptorOptions { CanLeave = false });
        var blockedByValidation = new StepDescriptor(
            ReviewKey,
            "Review",
            new StepDescriptorOptions { ValidationMessages = [new ValidationMessage(ReviewKey, "Review", "Fix review")] });
        var skipped = new StepDescriptor(
            SummaryKey,
            SummaryDisplayName,
            new StepDescriptorOptions { Status = StepStatus.Skipped, IsOptional = true });
        var state = new StepperState([first, blockedByValidation, skipped], MissingKey, StepperOrientation.Vertical);
        var finalBlockedState = new StepperState([blockedByValidation], ReviewKey);

        await Assert.That(first.IsBlocking).IsFalse();
        await Assert.That(first.IsAvailable).IsTrue();
        await Assert.That(blockedByValidation.IsBlocking).IsTrue();
        await Assert.That(blockedByValidation.IsAvailable).IsFalse();
        await Assert.That(skipped.IsComplete).IsTrue();
        await Assert.That(skipped.DisplayTitle).IsEqualTo($"{SummaryDisplayName} (optional)");
        await Assert.That(state.CurrentKey).IsEqualTo(PendingKey);
        await Assert.That(state.CurrentIndex).IsEqualTo(0);
        await Assert.That(state.Orientation).IsEqualTo(StepperOrientation.Vertical);
        await Assert.That(state.BlockingStepCount).IsEqualTo(ExpectedSingleCount);
        await Assert.That(state.CanGoPrevious).IsFalse();
        await Assert.That(state.CanGoNext).IsFalse();
        await Assert.That(state.CanFinish).IsFalse();
        await Assert.That(state.GetStep(MissingKey)).IsNull();
        await Assert.That(finalBlockedState.CanFinish).IsFalse();
    }

    /// <summary>Verifies navigation value objects normalize empty contracts and preserve runtime keys.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationValueObjects_ProjectNullContractsAndRuntimeKeys()
    {
        var viewModel = new TestNavigationViewModel();
        var view = new TestNavigationView { ViewModel = viewModel };
        var request = new NavigationRequest(
            NavigationSourceKind.ViewModel,
            viewModel,
            typeof(TestNavigationViewModel),
            "   ",
            null,
            NavigationType.New,
            default);
        var lookup = new NavigationLookupKey(
            NavigationSourceKind.ViewModel,
            typeof(TestNavigationViewModel),
            null);
        var sameLookup = new NavigationLookupKey(
            NavigationSourceKind.ViewModel,
            typeof(TestNavigationViewModel),
            null);
        var differentLookup = new NavigationLookupKey(
            NavigationSourceKind.View,
            typeof(TestNavigationViewModel),
            null);
        var resolution = new NavigationResolution(viewModel, view, DetailContract, ValueText, NavigationType.Refresh);
        var typedResolution = new NavigationResolution<TestNavigationViewModel, TestNavigationView>(
            viewModel,
            view,
            "   ",
            null,
            NavigationType.New);

        await Assert.That(request.Contract).IsNull();
        await Assert.That(request.Parameter).IsNull();
        await Assert.That(lookup.Equals((object?)sameLookup)).IsTrue();
        await Assert.That(lookup == sameLookup).IsTrue();
        await Assert.That(lookup != differentLookup).IsTrue();
        await Assert.That(lookup.GetHashCode()).IsEqualTo(sameLookup.GetHashCode());
        await Assert.That(resolution.Contract).IsEqualTo(DetailContract);
        await Assert.That(resolution.Parameter).IsEqualTo(ValueText);
        await Assert.That(typedResolution.Contract).IsNull();
        await Assert.That(typedResolution.Parameter).IsNull();
    }

    /// <summary>Exposes protected disposal for lifecycle branch testing.</summary>
    private sealed class ExposedDisposeRxObject : RxObject
    {
        /// <summary>Gets the disposable collection owned by the object.</summary>
        /// <returns>The owned disposable collection.</returns>
        public CompositeDisposable GetDisposables() => Disposables;

        /// <summary>Invokes the protected dispose path with the supplied flag.</summary>
        /// <param name="disposing">A value indicating whether managed resources should be disposed.</param>
        public void DisposeFromTest(bool disposing) => Dispose(disposing);
    }

    /// <summary>Provides a delegate-backed disposable for disposal assertions.</summary>
    /// <param name="onDispose">The callback to invoke on disposal.</param>
    private sealed class ActionDisposable(Action onDispose) : IDisposable
    {
        /// <summary>Invokes the configured disposal callback.</summary>
        public void Dispose() => onDispose();
    }

    /// <summary>Provides a counting event source for event-signal subscription tests.</summary>
    private sealed class EventSource
    {
        /// <summary>Stores the subscribed event handlers.</summary>
        private EventHandler<EventArgs>? _raised;

        /// <summary>Gets the number of handler add operations.</summary>
        public int AddCount { get; private set; }

        /// <summary>Gets the number of handler remove operations.</summary>
        public int RemoveCount { get; private set; }

        /// <summary>Adds a handler and records the subscription.</summary>
        /// <param name="handler">The handler to add.</param>
        public void AddHandler(EventHandler<EventArgs> handler)
        {
            AddCount++;
            _raised += handler;
        }

        /// <summary>Removes a handler and records the unsubscription.</summary>
        /// <param name="handler">The handler to remove.</param>
        public void RemoveHandler(EventHandler<EventArgs> handler)
        {
            RemoveCount++;
            _raised -= handler;
        }

        /// <summary>Raises the event with the supplied arguments.</summary>
        /// <param name="args">The event arguments.</param>
        public void Raise(EventArgs args) => _raised?.Invoke(this, args);
    }

    /// <summary>Provides a simple command used by immutable state snapshots.</summary>
    private sealed class TestCommand : System.Windows.Input.ICommand
    {
        /// <summary>Occurs when command availability changes.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Determines whether the command can execute.</summary>
        /// <param name="parameter">The optional command parameter.</param>
        /// <returns><c>true</c>.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Executes the command and raises the availability event.</summary>
        /// <param name="parameter">The optional command parameter.</param>
        public void Execute(object? parameter) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Provides a navigation view model for resolution value-object tests.</summary>
    private sealed class TestNavigationViewModel : RxObject;

    /// <summary>Provides a navigation view for typed and untyped resolution tests.</summary>
    private sealed class TestNavigationView : IViewFor<TestNavigationViewModel>
    {
        /// <inheritdoc />
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestNavigationViewModel?)value;
        }

        /// <inheritdoc />
        public TestNavigationViewModel? ViewModel { get; set; }
    }
}
