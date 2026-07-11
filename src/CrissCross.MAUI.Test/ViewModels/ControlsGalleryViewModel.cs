// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace CrissCross.MAUI.Test;

/// <summary>Reactive manual-QA view model for the gallery feature playground.</summary>
public sealed class ControlsGalleryViewModel : RxObject
{
    /// <summary>Provides the _isOperationRunning member.</summary>
    private readonly ObservableAsPropertyHelper<bool> _isOperationRunning;

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

    /// <summary>Provides the _chipGroupState member.</summary>
    private ChipGroupState _chipGroupState;

    /// <summary>Provides the _stepperState member.</summary>
    private StepperState _stepperState;

    /// <summary>Provides the _selectedTheme member.</summary>
    private ThemeChoice _selectedTheme = ThemeChoice.System;

    /// <summary>Provides the _themeState member.</summary>
    private ThemePreferenceState _themeState;

    /// <summary>Initializes a new instance of the <see cref="ControlsGalleryViewModel"/> class.</summary>
    public ControlsGalleryViewModel()
    {
        DisplayName = "Reactive feature playground";
        _searchState = CreateSearchState(_searchText, false);
        _paginationState = new(1, 10, 42);
        _currentRange = CreateRange(DateTimeOffset.Now);
        _segmentState = new(CreateSegments(), "table");
        _chipGroupState = new(CreateChips("alarms"), ChipGroupSelectionMode.Multiple);
        _stepperState = new(CreateSteps("review"), "review", StepperOrientation.Horizontal);
        _themeState = new(_selectedTheme, ThemeChoice.Dark, supportsHighContrast: true);

        RunImportCommand = ReactiveCommand.CreateFromTask(RunImportAsync);
        SearchCommand = ReactiveCommand.CreateFromTask<string>(SearchAsync);
        ClearSearchCommand = ReactiveCommand.Create(ClearSearch);
        PageRequestCommand = ReactiveCommand.Create<PageRequest>(ApplyPageRequest);
        RangeChangedCommand = ReactiveCommand.Create<DateTimeRange>(ApplyRange);
        SegmentChangedCommand = ReactiveCommand.Create<string>(ApplySegment);
        ChipChangedCommand = ReactiveCommand.Create<string>(ApplyChip);
        StepRequestedCommand = ReactiveCommand.Create<string>(ApplyStep);
        ThemeChangedCommand = ReactiveCommand.Create<ThemeChoice>(ApplyTheme);

        _isOperationRunning = RunImportCommand.IsExecuting
            .ToProperty(this, nameof(IsOperationRunning), scheduler: RxSchedulers.MainThreadScheduler);
    }

    /// <summary>Gets the async command used by the command button and busy overlay demos.</summary>
    public ReactiveCommand<Unit, Unit> RunImportCommand { get; }

    /// <summary>Gets the search submit command.</summary>
    public ReactiveCommand<string, Unit> SearchCommand { get; }

    /// <summary>Gets the search clear command.</summary>
    public ReactiveCommand<Unit, Unit> ClearSearchCommand { get; }

    /// <summary>Gets the page request command used by DataPager.</summary>
    public ReactiveCommand<PageRequest, Unit> PageRequestCommand { get; }

    /// <summary>Gets the date range command.</summary>
    public ReactiveCommand<DateTimeRange, Unit> RangeChangedCommand { get; }

    /// <summary>Gets the segmented selection command.</summary>
    public ReactiveCommand<string, Unit> SegmentChangedCommand { get; }

    /// <summary>Gets the chip selection command.</summary>
    public ReactiveCommand<string, Unit> ChipChangedCommand { get; }

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
    public bool IsOperationRunning => _isOperationRunning.Value;

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

    /// <summary>Gets the chip group state.</summary>
    public ChipGroupState ChipGroupState
    {
        get => _chipGroupState;
        private set => this.RaiseAndSetIfChanged(ref _chipGroupState, value);
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
            ThemeState = new(value, ThemeChoice.Dark, supportsHighContrast: true);
        }
    }

    /// <summary>Gets the current theme state.</summary>
    public ThemePreferenceState ThemeState
    {
        get => _themeState;
        private set => this.RaiseAndSetIfChanged(ref _themeState, value);
    }

    /// <summary>Gets deterministic platform notes for manual QA.</summary>
    public string PlatformNotes { get; } = GetPlatformNotes();

    /// <summary>Gets activation/disposal trace text.</summary>
    public string ActivationLog
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }
= "Not activated yet.";

    /// <inheritdoc/>
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(disposables);

        ActivationLog = $"Activated {DateTimeOffset.Now:HH:mm:ss} from {e.From?.Name ?? "<cold start>"}.";
        _ = Observable.Interval(TimeSpan.FromSeconds(5), RxSchedulers.TaskpoolScheduler)
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

    /// <summary>Gets platform-specific notes for the current device.</summary>
    /// <returns>The platform notes.</returns>
    private static string GetPlatformNotes()
    {
        if (OperatingSystem.IsAndroid())
        {
            return "Android: touch-first layout, Material handlers, compact spacing.";
        }

        if (OperatingSystem.IsWindows())
        {
            return "Windows: desktop windowing with pointer and keyboard QA.";
        }

        return OperatingSystem.IsIOS()
            ? "iOS: safe-area and touch interaction QA."
            : "MAUI: platform-specific handlers are active for the current device.";
    }

    /// <summary>Provides the CreateSearchState member.</summary>
    /// <param name="text">The text value.</param>
    /// <param name="isSearching">The isSearching value.</param>
    /// <returns>The result.</returns>
    private static SearchQueryState CreateSearchState(string? text, bool isSearching) => new(
        text,
        debouncedText: text?.Trim(),
        submittedText: text?.Trim(),
        isSearching: isSearching,
        resultCount: string.IsNullOrWhiteSpace(text) ? 42 : 7,
        filters:
        [
            new FilterToken("area", FilterOperator.Equals, "north", "Area: North"),
            new FilterToken("status", FilterOperator.NotEquals, "closed", "Status: Active")
        ]);

    /// <summary>Provides the CreateRange member.</summary>
    /// <param name="now">The now value.</param>
    /// <returns>The result.</returns>
    private static DateTimeRange CreateRange(DateTimeOffset now) => new(now.AddHours(-4), now, DateTimeRangePreset.Custom, "Last four hours");

    /// <summary>Provides the CreateSegments member.</summary>
    /// <returns>The result.</returns>
    private static IReadOnlyList<SegmentItem> CreateSegments() =>
    [
        new SegmentItem("table", "Table"),
        new SegmentItem("cards", "Cards"),
        new SegmentItem("timeline", "Timeline")
    ];

    /// <summary>Provides the CreateChips member.</summary>
    /// <param name="selectedKey">The selectedKey value.</param>
    /// <returns>The result.</returns>
    private static IReadOnlyList<ChipModel> CreateChips(string selectedKey) =>
    [
        new ChipModel("alarms", "Alarms", isSelected: selectedKey == "alarms"),
        new ChipModel("events", "Events", isSelected: selectedKey == "events"),
        new ChipModel("quality", "Quality", isSelected: selectedKey == "quality")
    ];

    /// <summary>Provides the CreateSteps member.</summary>
    /// <param name="currentKey">The currentKey value.</param>
    /// <returns>The result.</returns>
    private static IReadOnlyList<StepDescriptor> CreateSteps(string currentKey) =>
    [
        new StepDescriptor("connect", "Connect", currentKey == "connect" ? StepStatus.Active : StepStatus.Completed),
        new StepDescriptor("query", "Query", currentKey == "query" ? StepStatus.Active : StepStatus.Completed),
        new StepDescriptor("review", "Review", currentKey == "review" ? StepStatus.Active : StepStatus.Pending),
        new StepDescriptor("publish", "Publish", StepStatus.Pending, canEnter: currentKey == "review")
    ];

    /// <summary>Provides the RunImportAsync member.</summary>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task RunImportAsync(CancellationToken cancellationToken)
    {
        CommandState = CommandButtonState.Executing;
        CommandProgress = 0.35;
        CurrentOperation = new("Loading deterministic sample data", "Simulates a cancellable import without network access.", CommandProgress, ClearSearchCommand);

        await Task.Delay(250, cancellationToken).ConfigureAwait(true);

        CommandProgress = 1.0;
        CurrentOperation = null;
        CommandState = CommandButtonState.Succeeded;
        PaginationState = new(0, 10, 42);
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Provides the SearchAsync member.</summary>
    /// <param name="submittedText">The submittedText value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SearchAsync(string submittedText, CancellationToken cancellationToken)
    {
        SearchText = submittedText;
        SearchState = CreateSearchState(SearchText, true);
        await Task.Delay(150, cancellationToken).ConfigureAwait(true);
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Provides the ClearSearch member.</summary>
    private void ClearSearch()
    {
        SearchText = string.Empty;
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Provides the ApplyPageRequest member.</summary>
    /// <param name="request">The request value.</param>
    private void ApplyPageRequest(PageRequest request) => PaginationState = new(request.PageIndex, request.PageSize, 42);

    /// <summary>Provides the ApplyRange member.</summary>
    /// <param name="range">The range value.</param>
    private void ApplyRange(DateTimeRange range) => CurrentRange = range ?? CreateRange(DateTimeOffset.Now);

    /// <summary>Provides the ApplySegment member.</summary>
    /// <param name="key">The key value.</param>
    private void ApplySegment(string key) => SegmentState = new(CreateSegments(), key);

    /// <summary>Provides the ApplyChip member.</summary>
    /// <param name="key">The key value.</param>
    private void ApplyChip(string key) => ChipGroupState = new(CreateChips(key), ChipGroupSelectionMode.Multiple);

    /// <summary>Provides the ApplyStep member.</summary>
    /// <param name="key">The key value.</param>
    private void ApplyStep(string key) => StepperState = new(CreateSteps(key), key, StepperOrientation.Horizontal);

    /// <summary>Provides the ApplyTheme member.</summary>
    /// <param name="choice">The choice value.</param>
    private void ApplyTheme(ThemeChoice choice) => SelectedTheme = choice;
}
