// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross;
using ReactiveUI;

namespace CrissCross.Maui.UI.Gallery;

/// <summary>Reactive state and event flows used by the MAUI control gallery.</summary>
public sealed class GalleryViewModel : ReactiveObject, IDisposable
{
    /// <summary>Default page size used by the gallery pager.</summary>
    private const int PageSize = 10;

    /// <summary>Total items represented by the deterministic gallery data.</summary>
    private const int TotalItemCount = 42;

    /// <summary>Result count used when the gallery search has a submitted value.</summary>
    private const int FilteredItemCount = 7;

    /// <summary>Initial normalized command progress.</summary>
    private const double InitialProgress = 0.25;

    /// <summary>Number of hours represented by the default range sample.</summary>
    private const int DefaultRangeHours = -4;

    /// <summary>Duration of the deterministic visual QA operation.</summary>
    private static readonly TimeSpan OperationDelay = TimeSpan.FromMilliseconds(350);

    /// <summary>Tracks the cancellable visual QA operation.</summary>
    private CancellationTokenSource? _operationCancellation;

    /// <summary>Initializes a new instance of the <see cref="GalleryViewModel"/> class.</summary>
    public GalleryViewModel()
    {
        RunCommand = ReactiveCommand.CreateFromTask(RunAsync);
        CancelCommand = ReactiveCommand.Create(CancelOperation);
        SearchCommand = ReactiveCommand.Create<string>(Search);
        ClearSearchCommand = ReactiveCommand.Create(() => Search(string.Empty));
        RequestPageCommand = ReactiveCommand.Create<PageRequest>(request => PaginationState = new(request.PageIndex, request.PageSize, TotalItemCount));
        ApplyFiltersCommand = ReactiveCommand.Create(() => Search("active"));
        SelectChipCommand = ReactiveCommand.Create<string>(SelectChip);
        SelectSegmentCommand = ReactiveCommand.Create<string>(key => SegmentState = new(CreateSegments(), key));
        SelectStepCommand = ReactiveCommand.Create<string>(key => StepperState = new(CreateSteps(), key, StepperOrientation.Horizontal));
        ApplyRangeCommand = ReactiveCommand.Create<DateTimeRange>(range => DateRange = range);
        SetThemeCommand = ReactiveCommand.Create<string>(SetTheme);
        RestoreContentCommand = ReactiveCommand.Create(() => Search("restored"));
        UpdatePropertyCommand = ReactiveCommand.Create(() => Search("configuration"));
        RateCommand = ReactiveCommand.Create<int>(SetRating);
    }

    /// <summary>Gets the command button demonstration command.</summary>
    public ICommand RunCommand { get; }

    /// <summary>Gets the cancellation command for the active demonstration.</summary>
    public ICommand CancelCommand { get; }

    /// <summary>Gets the search submission command.</summary>
    public ICommand SearchCommand { get; }

    /// <summary>Gets the search clearing command.</summary>
    public ICommand ClearSearchCommand { get; }

    /// <summary>Gets the paging command.</summary>
    public ICommand RequestPageCommand { get; }

    /// <summary>Gets the filter apply command.</summary>
    public ICommand ApplyFiltersCommand { get; }

    /// <summary>Gets the chip selection command.</summary>
    public ICommand SelectChipCommand { get; }

    /// <summary>Gets the segment selection command.</summary>
    public ICommand SelectSegmentCommand { get; }

    /// <summary>Gets the step selection command.</summary>
    public ICommand SelectStepCommand { get; }

    /// <summary>Gets the date range application command.</summary>
    public ICommand ApplyRangeCommand { get; }

    /// <summary>Gets the common-theme selection command.</summary>
    public ICommand SetThemeCommand { get; }

    /// <summary>Gets the empty-state action command.</summary>
    public ICommand RestoreContentCommand { get; }

    /// <summary>Gets the property update command.</summary>
    public ICommand UpdatePropertyCommand { get; }

    /// <summary>Gets the command that observes rating selections from the MAUI rating control.</summary>
    public ICommand RateCommand { get; }

    /// <summary>Gets the current busy operation state.</summary>
    public BusyOperation? BusyOperation
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets a value indicating whether an operation is active.</summary>
    public bool IsBusy
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the command button state.</summary>
    public CommandButtonState CommandState
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the normalized command progress.</summary>
    public double? Progress
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the current search snapshot.</summary>
    public SearchQueryState SearchState
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = CreateSearchState(string.Empty);

    /// <summary>Gets the current pagination snapshot.</summary>
    public PaginationState PaginationState
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = new(0, PageSize, TotalItemCount);

    /// <summary>Gets the chip shown outside the chip group.</summary>
    public ChipModel HighlightChip { get; } = new("focus", "Selected chip");

    /// <summary>Gets the chip group state.</summary>
    public ChipGroupState ChipGroupState
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = new(CreateChips(), ChipGroupSelectionMode.Multiple);

    /// <summary>Gets the segmented selection state.</summary>
    public SegmentedSelectionState SegmentState
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = new(CreateSegments(), "cards");

    /// <summary>Gets the workflow step state.</summary>
    public StepperState StepperState
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = new(CreateSteps(), "review", StepperOrientation.Horizontal);

    /// <summary>Gets a validation summary state.</summary>
    public ValidationSummaryState ValidationState { get; } = new([new ValidationMessage("name", "Name", "A name is required.")]);

    /// <summary>Gets the empty-state model.</summary>
    public EmptyStateModel EmptyState { get; } = new("Nothing saved", "Restore sample content to continue.");

    /// <summary>Gets the reflection-free filter panel state.</summary>
    public DataFilterPanelState FilterPanelState { get; } = new();

    /// <summary>Gets the reflection-free property grid state.</summary>
    public PropertyGridState PropertyGridState { get; } = new();

    /// <summary>Gets the selected date range.</summary>
    public DateTimeRange DateRange
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = CreateDefaultRange();

    /// <summary>Gets the current theme preference state.</summary>
    public ThemePreferenceState ThemeState
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = new(ThemeChoice.Light);

    /// <summary>Gets the current theme description.</summary>
    public string ThemeDescription
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Light theme selected";

    /// <summary>Gets the selected gallery rating.</summary>
    public int Rating
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = 4;

    /// <summary>Gets the accessible gallery rating description.</summary>
    public string RatingDescription
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "4 of 5 stars selected";

    /// <inheritdoc />
    public void Dispose() => _operationCancellation?.Dispose();

    /// <summary>Creates the initial range without directly reading the machine clock.</summary>
    /// <returns>The default range sample.</returns>
    private static DateTimeRange CreateDefaultRange()
    {
        var now = TimeProvider.System.GetUtcNow();
        return new(now.AddHours(DefaultRangeHours), now, DateTimeRangePreset.Custom, "Last four hours");
    }

    /// <summary>Creates the search state displayed by search-related controls.</summary>
    /// <param name="text">The search text.</param>
    /// <returns>A search-state snapshot.</returns>
    private static SearchQueryState CreateSearchState(string? text) =>
        new(text, debouncedText: text, submittedText: text, resultCount: string.IsNullOrWhiteSpace(text) ? TotalItemCount : FilteredItemCount);

    /// <summary>Creates the selectable chip definitions.</summary>
    /// <returns>The chip definitions.</returns>
    private static IReadOnlyList<ChipModel> CreateChips() =>
    [
        new ChipModel("alarms", "Alarms"),
        new ChipModel("events", "Events"),
        new ChipModel("quality", "Quality"),
    ];

    /// <summary>Creates segmented view definitions.</summary>
    /// <returns>The segment definitions.</returns>
    private static IReadOnlyList<SegmentItem> CreateSegments() =>
    [new SegmentItem("table", "Table"), new SegmentItem("cards", "Cards"), new SegmentItem("timeline", "Timeline")];

    /// <summary>Creates workflow step definitions.</summary>
    /// <returns>The step definitions.</returns>
    private static IReadOnlyList<StepDescriptor> CreateSteps() =>
    [
        new StepDescriptor("connect", "Connect"),
        new StepDescriptor("review", "Review"),
        new StepDescriptor("publish", "Publish"),
    ];

    /// <summary>Runs a short cancellable operation for visual QA.</summary>
    /// <param name="cancellationToken">The reactive command cancellation token.</param>
    /// <returns>A task that completes when the demonstration completes.</returns>
    private async Task RunAsync(CancellationToken cancellationToken)
    {
        _operationCancellation?.Dispose();
        _operationCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        IsBusy = true;
        CommandState = CommandButtonState.Executing;
        Progress = InitialProgress;
        BusyOperation = new("Loading gallery samples", "The page remains interactive while state changes.", Progress, CancelCommand);

        try
        {
            await Task.Delay(OperationDelay, _operationCancellation.Token).ConfigureAwait(true);
            Progress = 1;
            CommandState = CommandButtonState.Succeeded;
            PaginationState = new(0, PageSize, TotalItemCount);
        }
        catch (OperationCanceledException)
        {
            CommandState = CommandButtonState.Cancelled;
        }
        finally
        {
            BusyOperation = null;
            Progress = null;
            IsBusy = false;
        }
    }

    /// <summary>Cancels the active visual-QA operation.</summary>
    private void CancelOperation() => _operationCancellation?.Cancel();

    /// <summary>Updates the reactive search and paging snapshots.</summary>
    /// <param name="text">The submitted search text.</param>
    private void Search(string? text)
    {
        SearchState = CreateSearchState(text);
        PaginationState = new(0, PageSize, string.IsNullOrWhiteSpace(text) ? TotalItemCount : FilteredItemCount);
    }

    /// <summary>Updates the group selection snapshot.</summary>
    /// <param name="key">The selected chip key.</param>
    private void SelectChip(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ChipGroupState = new(CreateChips(), ChipGroupSelectionMode.Multiple);
    }

    /// <summary>Applies a supported MAUI theme preference.</summary>
    /// <param name="themeText">The requested theme text.</param>
    private void SetTheme(string themeText)
    {
        var selected = string.Equals(themeText, nameof(ThemeChoice.Dark), StringComparison.OrdinalIgnoreCase)
            ? ThemeChoice.Dark
            : ThemeChoice.Light;
        ThemeState = new(selected);
        ThemeDescription = ThemeState.DisplayText;
        Application.Current!.UserAppTheme = selected == ThemeChoice.Dark ? AppTheme.Dark : AppTheme.Light;
    }

    /// <summary>Updates the observable rating description from the reactive command parameter.</summary>
    /// <param name="rating">The selected whole-star rating.</param>
    private void SetRating(int rating)
    {
        Rating = rating;
        RatingDescription = $"{rating} of 5 stars selected";
    }
}
