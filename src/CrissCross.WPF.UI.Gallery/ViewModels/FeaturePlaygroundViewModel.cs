// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// Reactive manual-QA view model for the gallery feature playground.
/// </summary>
public sealed class FeaturePlaygroundViewModel : RxObject
{
    private readonly ObservableAsPropertyHelper<bool> _isOperationRunning;
    private string? _searchText = "pump alarm";
    private SearchQueryState _searchState;
    private BusyOperation? _currentOperation;
    private CommandButtonState _commandState;
    private double? _commandProgress;
    private PaginationState _paginationState;
    private DateTimeRange _currentRange;
    private SegmentedSelectionState _segmentState;
    private StepperState _stepperState;
    private ThemeChoice _selectedTheme = ThemeChoice.System;
    private ThemePreferenceState _themeState;
    private string _activationLog = "Not activated yet.";

    /// <summary>
    /// Initializes a new instance of the <see cref="FeaturePlaygroundViewModel"/> class.
    /// </summary>
    public FeaturePlaygroundViewModel()
    {
        DisplayName = "Reactive feature playground";
        _searchState = CreateSearchState(_searchText, false);
        _paginationState = new PaginationState(1, 10, 42);
        _currentRange = CreateRange(DateTimeOffset.Now);
        _segmentState = new SegmentedSelectionState(CreateSegments(), "table");
        _stepperState = new StepperState(CreateSteps("review"), "review", StepperOrientation.Horizontal);
        _themeState = new ThemePreferenceState(_selectedTheme, ThemeChoice.Dark, supportsHighContrast: true);

        RunImportCommand = ReactiveCommand.CreateFromTask(RunImportAsync);
        SearchCommand = ReactiveCommand.CreateFromTask(SearchAsync);
        ClearSearchCommand = ReactiveCommand.Create(ClearSearch);
        PageRequestCommand = ReactiveCommand.Create<PageRequest>(ApplyPageRequest);
        RangeChangedCommand = ReactiveCommand.Create<DateTimeRange>(ApplyRange);
        SegmentChangedCommand = ReactiveCommand.Create<string>(ApplySegment);
        StepRequestedCommand = ReactiveCommand.Create<string>(ApplyStep);
        ThemeChangedCommand = ReactiveCommand.Create<ThemeChoice>(ApplyTheme);

        _isOperationRunning = RunImportCommand.IsExecuting
            .ToProperty(this, nameof(IsOperationRunning), scheduler: RxSchedulers.MainThreadScheduler);
    }

    /// <summary>
    /// Gets the async command used by the command button and busy overlay demos.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RunImportCommand { get; }

    /// <summary>
    /// Gets the search submit command.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    /// <summary>
    /// Gets the search clear command.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ClearSearchCommand { get; }

    /// <summary>
    /// Gets the page request command used by DataPager.
    /// </summary>
    public ReactiveCommand<PageRequest, Unit> PageRequestCommand { get; }

    /// <summary>
    /// Gets the date range command.
    /// </summary>
    public ReactiveCommand<DateTimeRange, Unit> RangeChangedCommand { get; }

    /// <summary>
    /// Gets the segmented selection command.
    /// </summary>
    public ReactiveCommand<string, Unit> SegmentChangedCommand { get; }

    /// <summary>
    /// Gets the workflow step command.
    /// </summary>
    public ReactiveCommand<string, Unit> StepRequestedCommand { get; }

    /// <summary>
    /// Gets the theme changed command.
    /// </summary>
    public ReactiveCommand<ThemeChoice, Unit> ThemeChangedCommand { get; }

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    public string? SearchText
    {
        get => _searchText;
        set
        {
            if (string.Equals(_searchText, value, StringComparison.Ordinal))
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _searchText, value);
            SearchState = CreateSearchState(value, false);
        }
    }

    /// <summary>
    /// Gets the aggregate search state.
    /// </summary>
    public SearchQueryState SearchState
    {
        get => _searchState;
        private set => this.RaiseAndSetIfChanged(ref _searchState, value);
    }

    /// <summary>
    /// Gets the active busy operation, when any.
    /// </summary>
    public BusyOperation? CurrentOperation
    {
        get => _currentOperation;
        private set => this.RaiseAndSetIfChanged(ref _currentOperation, value);
    }

    /// <summary>
    /// Gets a value indicating whether the async import command is executing.
    /// </summary>
    public bool IsOperationRunning => _isOperationRunning.Value;

    /// <summary>
    /// Gets the command button visual state.
    /// </summary>
    public CommandButtonState CommandState
    {
        get => _commandState;
        private set => this.RaiseAndSetIfChanged(ref _commandState, value);
    }

    /// <summary>
    /// Gets normalized command progress.
    /// </summary>
    public double? CommandProgress
    {
        get => _commandProgress;
        private set => this.RaiseAndSetIfChanged(ref _commandProgress, value);
    }

    /// <summary>
    /// Gets current pagination state.
    /// </summary>
    public PaginationState PaginationState
    {
        get => _paginationState;
        private set => this.RaiseAndSetIfChanged(ref _paginationState, value);
    }

    /// <summary>
    /// Gets the current date/time range.
    /// </summary>
    public DateTimeRange CurrentRange
    {
        get => _currentRange;
        private set => this.RaiseAndSetIfChanged(ref _currentRange, value);
    }

    /// <summary>
    /// Gets the segmented control state.
    /// </summary>
    public SegmentedSelectionState SegmentState
    {
        get => _segmentState;
        private set => this.RaiseAndSetIfChanged(ref _segmentState, value);
    }

    /// <summary>
    /// Gets the stepper workflow state.
    /// </summary>
    public StepperState StepperState
    {
        get => _stepperState;
        private set => this.RaiseAndSetIfChanged(ref _stepperState, value);
    }

    /// <summary>
    /// Gets or sets the selected theme.
    /// </summary>
    public ThemeChoice SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _selectedTheme, value);
            ThemeState = new ThemePreferenceState(value, ThemeChoice.Dark, supportsHighContrast: true);
        }
    }

    /// <summary>
    /// Gets the current theme state.
    /// </summary>
    public ThemePreferenceState ThemeState
    {
        get => _themeState;
        private set => this.RaiseAndSetIfChanged(ref _themeState, value);
    }

    /// <summary>
    /// Gets deterministic platform notes for manual QA.
    /// </summary>
    public string PlatformNotes { get; } = "WPF demonstrates Windows desktop theme/high-contrast support, view-model navigation, and breadcrumb view navigation.";

    /// <summary>
    /// Gets activation/disposal trace text.
    /// </summary>
    public string ActivationLog
    {
        get => _activationLog;
        private set => this.RaiseAndSetIfChanged(ref _activationLog, value);
    }

    /// <inheritdoc/>
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(disposables);

        ActivationLog = $"Activated {DateTimeOffset.Now:HH:mm:ss} from {e.From?.Name ?? "<cold start>"}.";
        Observable.Interval(TimeSpan.FromSeconds(5), RxSchedulers.TaskpoolScheduler)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ => ActivationLog = $"Still active {DateTimeOffset.Now:HH:mm:ss}; dispose this page by navigating away.")
            .DisposeWith(disposables);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isOperationRunning.Dispose();
        }

        base.Dispose(disposing);
    }

    private static SearchQueryState CreateSearchState(string? text, bool isSearching) => new(
        text,
        debouncedText: text?.Trim(),
        submittedText: text?.Trim(),
        isSearching: isSearching,
        resultCount: string.IsNullOrWhiteSpace(text) ? 42 : 7,
        filters: new[]
        {
            new FilterToken("area", FilterOperator.Equals, "north", "Area: North"),
            new FilterToken("status", FilterOperator.NotEquals, "closed", "Status: Active")
        });

    private static DateTimeRange CreateRange(DateTimeOffset now) => new(now.AddHours(-4), now, DateTimeRangePreset.Custom, "Last four hours");

    private static IReadOnlyList<SegmentItem> CreateSegments() =>
    [
        new SegmentItem("table", "Table"),
        new SegmentItem("cards", "Cards"),
        new SegmentItem("timeline", "Timeline")
    ];

    private static IReadOnlyList<StepDescriptor> CreateSteps(string currentKey) =>
    [
        new StepDescriptor("connect", "Connect", currentKey == "connect" ? StepStatus.Active : StepStatus.Completed),
        new StepDescriptor("query", "Query", currentKey == "query" ? StepStatus.Active : StepStatus.Completed),
        new StepDescriptor("review", "Review", currentKey == "review" ? StepStatus.Active : StepStatus.Pending),
        new StepDescriptor("publish", "Publish", StepStatus.Pending, canEnter: currentKey == "review")
    ];

    private async Task RunImportAsync(CancellationToken cancellationToken)
    {
        CommandState = CommandButtonState.Executing;
        CommandProgress = 0.35;
        CurrentOperation = new BusyOperation("Loading deterministic sample data", "Simulates a cancellable import without network access.", CommandProgress, ClearSearchCommand);

        await Task.Delay(250, cancellationToken).ConfigureAwait(true);

        CommandProgress = 1.0;
        CurrentOperation = null;
        CommandState = CommandButtonState.Succeeded;
        PaginationState = new PaginationState(0, 10, 42);
        SearchState = CreateSearchState(SearchText, false);
    }

    private async Task SearchAsync(CancellationToken cancellationToken)
    {
        SearchState = CreateSearchState(SearchText, true);
        await Task.Delay(150, cancellationToken).ConfigureAwait(true);
        SearchState = CreateSearchState(SearchText, false);
    }

    private void ClearSearch()
    {
        SearchText = string.Empty;
        SearchState = CreateSearchState(SearchText, false);
    }

    private void ApplyPageRequest(PageRequest request) => PaginationState = new PaginationState(request.PageIndex, request.PageSize, 42);

    private void ApplyRange(DateTimeRange range) => CurrentRange = range ?? CreateRange(DateTimeOffset.Now);

    private void ApplySegment(string key) => SegmentState = new SegmentedSelectionState(CreateSegments(), key);

    private void ApplyStep(string key) => StepperState = new StepperState(CreateSteps(key), key, StepperOrientation.Horizontal);

    private void ApplyTheme(ThemeChoice choice) => SelectedTheme = choice;
}
