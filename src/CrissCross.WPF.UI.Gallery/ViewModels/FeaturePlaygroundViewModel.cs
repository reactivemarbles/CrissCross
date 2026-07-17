// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrissCross.WPF.UI;
using CrissCross.WPF.UI.Appearance;
using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>Reactive manual-QA view model for the gallery feature playground.</summary>
public sealed class FeaturePlaygroundViewModel : RxObject
{
    /// <summary>The initial page index.</summary>
    private const int SamplePageIndex = 1;

    /// <summary>The number of entries on a sample page.</summary>
    private const int SamplePageSize = 10;

    /// <summary>The total sample result count.</summary>
    private const int SampleResultCount = 42;

    /// <summary>The result count shown after searching.</summary>
    private const int SearchResultCount = 7;

    /// <summary>The interval between activation refreshes.</summary>
    private const int ActivationRefreshSeconds = 5;

    /// <summary>The default range offset in hours.</summary>
    private const int DefaultRangeHours = -4;

    /// <summary>The initial command progress.</summary>
    private const double InitialCommandProgress = 0.35;

    /// <summary>The simulated import delay in milliseconds.</summary>
    private const int ImportDelayMilliseconds = 250;

    /// <summary>The simulated search delay in milliseconds.</summary>
    private const int SearchDelayMilliseconds = 150;

    /// <summary>Workflow key used by the review step.</summary>
    private const string ReviewStepKey = "review";

    /// <summary>Tracks whether the import command is running.</summary>
    private ObservableAsPropertyHelper<bool>? _isOperationRunning;

    /// <summary>Stores the current search text.</summary>
    private string? _searchText = "pump alarm";

    /// <summary>Stores the current search state.</summary>
    private SearchQueryState _searchState;

    /// <summary>Stores the current pagination state.</summary>
    private PaginationState _paginationState;

    /// <summary>Stores the current date/time range.</summary>
    private DateTimeRange _currentRange;

    /// <summary>Stores the current segment state.</summary>
    private SegmentedSelectionState _segmentState;

    /// <summary>Stores the current stepper state.</summary>
    private StepperState _stepperState;

    /// <summary>Stores the selected theme choice.</summary>
    private ThemeChoice _selectedTheme = ThemeChoice.System;

    /// <summary>Stores the current theme preference state.</summary>
    private ThemePreferenceState _themeState;

    /// <summary>Initializes a new instance of the <see cref="FeaturePlaygroundViewModel"/> class.</summary>
    public FeaturePlaygroundViewModel()
    {
        DisplayName = "Reactive feature playground";
        _searchState = CreateSearchState(_searchText, false);
        _paginationState = new(SamplePageIndex, SamplePageSize, SampleResultCount);
        _currentRange = CreateRange(DateTimeOffset.Now);
        _segmentState = new(CreateSegments(), "table");
        _stepperState = new(CreateSteps(ReviewStepKey), ReviewStepKey, StepperOrientation.Horizontal);
        _themeState = CreateThemeState(_selectedTheme);

        RunImportCommand = ReactiveCommand.CreateFromTask(RunImportAsync);
        SearchCommand = ReactiveCommand.CreateFromTask<string>(SearchAsync);
        ClearSearchCommand = ReactiveCommand.Create(ClearSearch);
        PageRequestCommand = ReactiveCommand.Create<PageRequest>(ApplyPageRequest);
        RangeChangedCommand = ReactiveCommand.Create<DateTimeRange>(ApplyRange);
        SegmentChangedCommand = ReactiveCommand.Create<string>(ApplySegment);
        StepRequestedCommand = ReactiveCommand.Create<string>(ApplyStep);
        ThemeChangedCommand = ReactiveCommand.Create<ThemeChoice>(ApplyTheme);
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
        "WPF demonstrates Windows desktop theme/high-contrast support, view-model navigation, "
        + "and breadcrumb view navigation.";

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

        ActivationLog = $"Activated {DateTimeOffset.Now:HH:mm:ss} from {e.From?.Name ?? "<cold start>"}.";
        _ = Observable
            .Interval(TimeSpan.FromSeconds(ActivationRefreshSeconds), RxSchedulers.TaskpoolScheduler)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ =>
                ActivationLog = $"Still active {DateTimeOffset.Now:HH:mm:ss}; dispose this page by navigating away.")
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

    /// <summary>Creates the aggregate search state.</summary>
    /// <param name="text">The current search text.</param>
    /// <param name="isSearching">A value indicating whether search is active.</param>
    /// <returns>The search state.</returns>
    private static SearchQueryState CreateSearchState(string? text, bool isSearching) =>
        new(
            text,
            debouncedText: text?.Trim(),
            submittedText: text?.Trim(),
            isSearching: isSearching,
            resultCount: string.IsNullOrWhiteSpace(text) ? SampleResultCount : SearchResultCount,
            filters:
            [
                new FilterToken("area", FilterOperator.Equals, "north", "Area: North"),
                new FilterToken("status", FilterOperator.NotEquals, "closed", "Status: Active"),]);

    /// <summary>Creates the default date/time range.</summary>
    /// <param name="now">The current time.</param>
    /// <returns>The default date/time range.</returns>
    private static DateTimeRange CreateRange(DateTimeOffset now) =>
        new(now.AddHours(DefaultRangeHours), now, DateTimeRangePreset.Custom, "Last four hours");

    /// <summary>Creates the segmented control items.</summary>
    /// <returns>The segment items.</returns>
    private static IReadOnlyList<SegmentItem> CreateSegments() =>
        [new SegmentItem("table", "Table"), new SegmentItem("cards", "Cards"), new SegmentItem("timeline", "Timeline")];

    /// <summary>Creates the workflow steps.</summary>
    /// <param name="currentKey">The active step key.</param>
    /// <returns>The workflow steps.</returns>
    private static IReadOnlyList<StepDescriptor> CreateSteps(string currentKey) =>
        [
            new StepDescriptor(
                "connect",
                "Connect",
                new StepDescriptorOptions
                {
                    Status = currentKey == "connect" ? StepStatus.Active : StepStatus.Completed,
                }),
            new StepDescriptor(
                "query",
                "Query",
                new StepDescriptorOptions
                {
                    Status = currentKey == "query" ? StepStatus.Active : StepStatus.Completed,
                }),
            new StepDescriptor(
                ReviewStepKey,
                "Review",
                new StepDescriptorOptions
                {
                    Status = currentKey == ReviewStepKey ? StepStatus.Active : StepStatus.Pending,
                }),
            new StepDescriptor(
                "publish",
                "Publish",
                new StepDescriptorOptions { Status = StepStatus.Pending, CanEnter = currentKey == ReviewStepKey }),];

    /// <summary>Creates the theme preference state.</summary>
    /// <param name="selectedChoice">The selected theme choice.</param>
    /// <returns>The theme preference state.</returns>
    private static ThemePreferenceState CreateThemeState(ThemeChoice selectedChoice) =>
        new(selectedChoice, GetSystemThemeChoice(), supportsHighContrast: true);

    /// <summary>Gets the current system theme choice.</summary>
    /// <returns>The current system theme choice.</returns>
    private static ThemeChoice GetSystemThemeChoice() =>
        new ThemeService().GetSystemTheme() switch
        {
            ApplicationTheme.Dark => ThemeChoice.Dark,
            ApplicationTheme.HighContrast => ThemeChoice.HighContrast,
            _ => ThemeChoice.Light,
        };

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

    /// <summary>Runs the sample import operation.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task RunImportAsync(CancellationToken cancellationToken)
    {
        CommandState = CommandButtonState.Executing;
        CommandProgress = InitialCommandProgress;
        CurrentOperation = new(
            "Loading deterministic sample data",
            "Simulates a cancellable import without network access.",
            CommandProgress,
            ClearSearchCommand);

        await Task.Delay(ImportDelayMilliseconds, cancellationToken).ConfigureAwait(true);

        CommandProgress = 1.0;
        CurrentOperation = null;
        CommandState = CommandButtonState.Succeeded;
        PaginationState = new(0, SamplePageSize, SampleResultCount);
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Runs the sample search operation.</summary>
    /// <param name="submittedText">The submitted search text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SearchAsync(string submittedText, CancellationToken cancellationToken)
    {
        SearchText = submittedText;
        SearchState = CreateSearchState(SearchText, true);
        await Task.Delay(SearchDelayMilliseconds, cancellationToken).ConfigureAwait(true);
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Clears the search state.</summary>
    private void ClearSearch()
    {
        SearchText = string.Empty;
        SearchState = CreateSearchState(SearchText, false);
    }

    /// <summary>Applies a page request.</summary>
    /// <param name="request">The page request.</param>
    private void ApplyPageRequest(PageRequest request) =>
        PaginationState = new(request.PageIndex, request.PageSize, SampleResultCount);

    /// <summary>Applies a date/time range.</summary>
    /// <param name="range">The date/time range.</param>
    private void ApplyRange(DateTimeRange range) => CurrentRange = range ?? CreateRange(DateTimeOffset.Now);

    /// <summary>Applies the selected segment.</summary>
    /// <param name="key">The selected segment key.</param>
    private void ApplySegment(string key) => SegmentState = new(CreateSegments(), key);

    /// <summary>Applies the requested step.</summary>
    /// <param name="key">The requested step key.</param>
    private void ApplyStep(string key) => StepperState = new(CreateSteps(key), key, StepperOrientation.Horizontal);

    /// <summary>Applies the selected theme.</summary>
    /// <param name="choice">The selected theme choice.</param>
    private void ApplyTheme(ThemeChoice choice) => SelectedTheme = choice;
}
