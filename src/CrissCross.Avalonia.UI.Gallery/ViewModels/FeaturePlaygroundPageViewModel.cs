// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CrissCross.Avalonia.UI;
using CrissCross.Avalonia.UI.Appearance;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>Reactive manual-QA view model for the gallery feature playground.</summary>
public sealed class FeaturePlaygroundPageViewModel : RxObject
{
    /// <summary>Default page size used by the pagination demo.</summary>
    private const int DefaultPageSize = 10;

    /// <summary>Sample total item count used by the pagination demo.</summary>
    private const int SampleTotalItemCount = 42;

    /// <summary>Sample filtered result count used after a non-empty search.</summary>
    private const int FilteredResultCount = 7;

    /// <summary>Interval in seconds for activation heartbeat messages.</summary>
    private const int ActivationHeartbeatSeconds = 5;

    /// <summary>Default range duration in hours.</summary>
    private const int DefaultRangeHours = 4;

    /// <summary>Initial command progress shown while the import demo runs.</summary>
    private const double InitialCommandProgress = 0.35;

    /// <summary>Completed command progress value.</summary>
    private const double CompletedCommandProgress = 1.0;

    /// <summary>Delay in milliseconds for the import command simulation.</summary>
    private const int ImportDelayMilliseconds = 250;

    /// <summary>Delay in milliseconds for the search command simulation.</summary>
    private const int SearchDelayMilliseconds = 150;

    /// <summary>Workflow key used by the review step.</summary>
    private const string ReviewStepKey = "review";

    /// <summary>Zero-based index of the review workflow step.</summary>
    private const int ReviewStepIndex = 2;

    /// <summary>Zero-based index of the publish workflow step.</summary>
    private const int PublishStepIndex = 3;

    /// <summary>Provides a caller-controlled clock for deterministic gallery diagnostics.</summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>Retains the user's active filters across searches and paging.</summary>
    private readonly List<FilterToken> _activeFilters =
    [
        new("area", FilterOperator.Equals, "north", "Area: North"),
        new("status", FilterOperator.NotEquals, "closed", "Status: Active"),
    ];

    /// <summary>Tracks whether the import command is running.</summary>
    private ObservableAsPropertyHelper<bool>? _isOperationRunning;

    /// <summary>Cancels the current import operation from the busy overlay.</summary>
    private Action? _cancelImport;

    /// <summary>Provides the _searchText member.</summary>
    private string? _searchText = "pump alarm";

    /// <summary>Provides the _searchState member.</summary>
    private SearchQueryState _searchState;

    /// <summary>Provides the _paginationState member.</summary>
    private PaginationState _paginationState;

    /// <summary>Provides the _currentRange member.</summary>
    private DateTimeRange _currentRange;

    /// <summary>Provides the _segmentState member.</summary>
    private SegmentedSelectionState _segmentState;

    /// <summary>Provides the _stepperState member.</summary>
    private StepperState _stepperState;

    /// <summary>Provides the _selectedTheme member.</summary>
    private ThemeChoice _selectedTheme;

    /// <summary>Provides the _themeState member.</summary>
    private ThemePreferenceState _themeState;

    /// <summary>Initializes a new instance of the <see cref="FeaturePlaygroundPageViewModel"/> class.</summary>
    public FeaturePlaygroundPageViewModel()
        : this(TimeProvider.System)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FeaturePlaygroundPageViewModel"/> class.</summary>
    /// <param name="timeProvider">The clock used for deterministic gallery diagnostics.</param>
    public FeaturePlaygroundPageViewModel(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
        DisplayName = "Reactive feature playground";
        _searchState = CreateSearchState(_searchText, false);
        _paginationState = new(1, DefaultPageSize, SampleTotalItemCount);
        _currentRange = CreateRange(_timeProvider.GetUtcNow());
        _segmentState = new(CreateSegments(), "table");
        _stepperState = new(CreateSteps(ReviewStepKey), ReviewStepKey, StepperOrientation.Horizontal);
        _themeState = CreateThemeState(_selectedTheme);

        RunImportCommand = ReactiveCommand.CreateFromTask(RunImportAsync);
        CancelImportCommand = ReactiveCommand.Create(CancelImport);
        SearchCommand = ReactiveCommand.CreateFromTask<string>(SearchAsync);
        ClearSearchCommand = ReactiveCommand.Create(ClearSearch);
        RemoveFilterCommand = ReactiveCommand.Create<FilterToken>(RemoveFilter);
        ClearFiltersCommand = ReactiveCommand.Create(ClearFilters);
        PageRequestCommand = ReactiveCommand.Create<PageRequest>(ApplyPageRequest);
        RangeChangedCommand = ReactiveCommand.Create<DateTimeRange>(ApplyRange);
        SegmentChangedCommand = ReactiveCommand.Create<string>(ApplySegment);
        StepRequestedCommand = ReactiveCommand.Create<string>(ApplyStep);
        ThemeChangedCommand = ReactiveCommand.Create<ThemeChoice>(ApplyTheme);
    }

    /// <summary>Gets the async command used by the command button and busy overlay demos.</summary>
    public ReactiveCommand<Unit, Unit> RunImportCommand { get; }

    /// <summary>Gets the command that cancels the current import operation.</summary>
    public ReactiveCommand<Unit, Unit> CancelImportCommand { get; }

    /// <summary>Gets the search submit command.</summary>
    public ReactiveCommand<string, Unit> SearchCommand { get; }

    /// <summary>Gets the search clear command.</summary>
    public ReactiveCommand<Unit, Unit> ClearSearchCommand { get; }

    /// <summary>Gets the command that removes one active filter.</summary>
    public ReactiveCommand<FilterToken, Unit> RemoveFilterCommand { get; }

    /// <summary>Gets the command that clears removable filters while preserving the search text.</summary>
    public ReactiveCommand<Unit, Unit> ClearFiltersCommand { get; }

    /// <summary>Gets the page request command used by DataPager.</summary>
    public ReactiveCommand<PageRequest, Unit> PageRequestCommand { get; }

    /// <summary>Gets the date range command.</summary>
    public ReactiveCommand<DateTimeRange, Unit> RangeChangedCommand { get; }

    /// <summary>Gets the segmented selection command.</summary>
    public ReactiveCommand<string, Unit> SegmentChangedCommand { get; }

    /// <summary>Gets the workflow step command.</summary>
    public ReactiveCommand<string, Unit> StepRequestedCommand { get; }

    /// <summary>Gets the theme changed command.</summary>
    public ReactiveCommand<ThemeChoice, Unit> ThemeChangedCommand { get; }

    /// <summary>Gets or sets the search text.</summary>
    public string? SearchText
    {
        get => _searchText;
        set
        {
            if (string.Equals(_searchText, value, StringComparison.Ordinal))
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref _searchText, value);
            SearchState = CreateSearchState(value, false);
        }
    }

    /// <summary>Gets the aggregate search state.</summary>
    public SearchQueryState SearchState
    {
        get => _searchState;
        private set => this.RaiseAndSetIfChanged(ref _searchState, value);
    }

    /// <summary>Gets the active busy operation, when any.</summary>
    public BusyOperation? CurrentOperation
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets a value indicating whether the async import command is executing.</summary>
    public bool IsOperationRunning => GetIsOperationRunningValue();

    /// <summary>Gets the command button visual state.</summary>
    public CommandButtonState CommandState
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets normalized command progress.</summary>
    public double? CommandProgress
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets current pagination state.</summary>
    public PaginationState PaginationState
    {
        get => _paginationState;
        private set => this.RaiseAndSetIfChanged(ref _paginationState, value);
    }

    /// <summary>Gets the current date/time range.</summary>
    public DateTimeRange CurrentRange
    {
        get => _currentRange;
        private set => this.RaiseAndSetIfChanged(ref _currentRange, value);
    }

    /// <summary>Gets the segmented control state.</summary>
    public SegmentedSelectionState SegmentState
    {
        get => _segmentState;
        private set => this.RaiseAndSetIfChanged(ref _segmentState, value);
    }

    /// <summary>Gets the stepper workflow state.</summary>
    public StepperState StepperState
    {
        get => _stepperState;
        private set => this.RaiseAndSetIfChanged(ref _stepperState, value);
    }

    /// <summary>Gets or sets the selected theme.</summary>
    public ThemeChoice SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme == value)
            {
                return;
            }

            _ = this.RaiseAndSetIfChanged(ref _selectedTheme, value);
            ThemeState = CreateThemeState(value);
        }
    }

    /// <summary>Gets the current theme state.</summary>
    public ThemePreferenceState ThemeState
    {
        get => _themeState;
        private set => this.RaiseAndSetIfChanged(ref _themeState, value);
    }

    /// <summary>Gets deterministic platform notes for manual QA.</summary>
    public string PlatformNotes { get; } =
        "Avalonia demonstrates desktop cross-platform rendering, NavigationWindow hosting, and shared Fluent-style "
        + "resources.";

    /// <summary>Gets activation/disposal trace text.</summary>
    public string ActivationLog
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Not activated yet.";

    /// <inheritdoc/>
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(disposables);

        ActivationLog = $"Activated {GetCurrentTimeText()} from {e.From?.Name ?? "<cold start>"}.";
        _ = Observable
            .Interval(TimeSpan.FromSeconds(ActivationHeartbeatSeconds), RxSchedulers.TaskpoolScheduler)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(OnActivationHeartbeat)
            .DisposeWith(disposables);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isOperationRunning?.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Provides the CreateRange member.</summary>
    /// <param name="now">The now value.</param>
    /// <returns>The result.</returns>
    private static DateTimeRange CreateRange(DateTimeOffset now) =>
        new(now.AddHours(-DefaultRangeHours), now, DateTimeRangePreset.Custom, "Last four hours");

    /// <summary>Provides the CreateSegments member.</summary>
    /// <returns>The result.</returns>
    private static IReadOnlyList<SegmentItem> CreateSegments() =>
        [new SegmentItem("table", "Table"), new SegmentItem("cards", "Cards"), new SegmentItem("timeline", "Timeline")];

    /// <summary>Provides the CreateSteps member.</summary>
    /// <param name="currentKey">The currentKey value.</param>
    /// <returns>The result.</returns>
    private static IReadOnlyList<StepDescriptor> CreateSteps(string currentKey)
    {
        var currentIndex = currentKey switch
        {
            "connect" => 0,
            "query" => 1,
            ReviewStepKey => ReviewStepIndex,
            "publish" => PublishStepIndex,
            _ => ReviewStepIndex,
        };

        return
        [
            new StepDescriptor(
                "connect",
                "Connect",
                new StepDescriptorOptions { Status = GetStepStatus(0, currentIndex) }),
            new StepDescriptor(
                "query",
                "Query",
                new StepDescriptorOptions { Status = GetStepStatus(1, currentIndex) }),
            new StepDescriptor(
                ReviewStepKey,
                "Review",
                new StepDescriptorOptions { Status = GetStepStatus(ReviewStepIndex, currentIndex) }),
            new StepDescriptor(
                "publish",
                "Publish",
                new StepDescriptorOptions { Status = GetStepStatus(PublishStepIndex, currentIndex) }),];
    }

    /// <summary>Resolves a workflow status from the active and candidate step positions.</summary>
    /// <param name="stepIndex">The candidate step index.</param>
    /// <param name="currentIndex">The active step index.</param>
    /// <returns>The status projected by the gallery workflow.</returns>
    private static StepStatus GetStepStatus(int stepIndex, int currentIndex)
    {
        if (stepIndex == currentIndex)
        {
            return StepStatus.Active;
        }

        return stepIndex < currentIndex ? StepStatus.Completed : StepStatus.Pending;
    }

    /// <summary>Provides the CreateThemeState member.</summary>
    /// <param name="selectedChoice">The selectedChoice value.</param>
    /// <returns>The result.</returns>
    private static ThemePreferenceState CreateThemeState(ThemeChoice selectedChoice) =>
        new(selectedChoice, GetSystemThemeChoice(), supportsHighContrast: true);

    /// <summary>Provides the GetSystemThemeChoice member.</summary>
    /// <returns>The result.</returns>
    private static ThemeChoice GetSystemThemeChoice() =>
        new ThemeService().GetSystemTheme() switch
        {
            ApplicationTheme.Dark => ThemeChoice.Dark,
            ApplicationTheme.HighContrast => ThemeChoice.HighContrast,
            _ => ThemeChoice.Light,
        };

    /// <summary>Provides the CreateSearchState member.</summary>
    /// <param name="text">The text value.</param>
    /// <param name="isSearching">The isSearching value.</param>
    /// <returns>The result.</returns>
    private SearchQueryState CreateSearchState(string? text, bool isSearching) =>
        new(
            text,
            debouncedText: text?.Trim(),
            submittedText: text?.Trim(),
            isSearching: isSearching,
            resultCount: string.IsNullOrWhiteSpace(text) ? SampleTotalItemCount : FilteredResultCount,
            filters: _activeFilters.ToArray());

    /// <summary>Gets the lazily initialized command execution state.</summary>
    /// <returns>Whether the import command is executing.</returns>
    private bool GetIsOperationRunningValue()
    {
        _isOperationRunning ??= RunImportCommand.IsExecuting.ToProperty(
            this,
            nameof(IsOperationRunning),
            scheduler: RxSchedulers.MainThreadScheduler);
        return _isOperationRunning.Value;
    }

    /// <summary>Provides the RunImportAsync member.</summary>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task RunImportAsync(CancellationToken cancellationToken)
    {
        using var importCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _cancelImport = importCancellation.Cancel;
        CommandState = CommandButtonState.Executing;
        CommandProgress = InitialCommandProgress;
        CurrentOperation = new(
            "Loading deterministic sample data",
            "Simulates a cancellable import without network access.",
            CommandProgress,
            CancelImportCommand);

        try
        {
            await Task.Delay(ImportDelayMilliseconds, importCancellation.Token).ConfigureAwait(true);
            CommandProgress = CompletedCommandProgress;
            CommandState = CommandButtonState.Succeeded;
            PaginationState = new(0, DefaultPageSize, SampleTotalItemCount);
            SearchState = CreateSearchState(SearchText, false);
        }
        catch (OperationCanceledException) when (importCancellation.IsCancellationRequested)
        {
            CommandProgress = null;
            CommandState = CommandButtonState.Cancelled;
        }
        finally
        {
            CurrentOperation = null;
            _cancelImport = null;
        }
    }

    /// <summary>Requests cancellation without changing the current query or filter state.</summary>
    private void CancelImport() => _cancelImport?.Invoke();

    /// <summary>Provides the SearchAsync member.</summary>
    /// <param name="submittedText">The submittedText value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SearchAsync(string submittedText, CancellationToken cancellationToken)
    {
        SearchText = submittedText;
        SearchState = CreateSearchState(SearchText, true);
        await Task.Delay(SearchDelayMilliseconds, cancellationToken).ConfigureAwait(true);
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Provides the ClearSearch member.</summary>
    private void ClearSearch()
    {
        SearchText = string.Empty;
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Removes the requested token and republishes the immutable query state.</summary>
    /// <param name="token">The requested filter token.</param>
    private void RemoveFilter(FilterToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        _ = _activeFilters.RemoveAll(candidate => candidate.IsRemovable && candidate.Key == token.Key);
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Clears removable filters while preserving the current query.</summary>
    private void ClearFilters()
    {
        _ = _activeFilters.RemoveAll(static token => token.IsRemovable);
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Provides the ApplyPageRequest member.</summary>
    /// <param name="request">The request value.</param>
    private void ApplyPageRequest(PageRequest request) =>
        PaginationState = new(request.PageIndex, request.PageSize, SampleTotalItemCount);

    /// <summary>Provides the ApplyRange member.</summary>
    /// <param name="range">The range value.</param>
    private void ApplyRange(DateTimeRange range) => CurrentRange = range ?? CreateRange(_timeProvider.GetUtcNow());

    /// <summary>Updates the activation log for a scheduled heartbeat.</summary>
    /// <param name="value">The heartbeat sequence value.</param>
    private void OnActivationHeartbeat(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ActivationLog = $"Still active {GetCurrentTimeText()}; dispose this page by navigating away.";
    }

    /// <summary>Formats the injected current time for the gallery activation trace.</summary>
    /// <returns>The localized current time.</returns>
    private string GetCurrentTimeText() => _timeProvider.GetLocalNow().ToString("T", CultureInfo.CurrentCulture);

    /// <summary>Provides the ApplySegment member.</summary>
    /// <param name="key">The key value.</param>
    private void ApplySegment(string key) => SegmentState = new(CreateSegments(), key);

    /// <summary>Provides the ApplyStep member.</summary>
    /// <param name="key">The key value.</param>
    private void ApplyStep(string key) => StepperState = new(CreateSteps(key), key, StepperOrientation.Horizontal);

    /// <summary>Provides the ApplyTheme member.</summary>
    /// <param name="choice">The choice value.</param>
    private void ApplyTheme(ThemeChoice choice) => SelectedTheme = choice;
}
