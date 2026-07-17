// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Represents a paging/navigation surface for local or remote MAUI data sources.</summary>
public class DataPager : ContentView
{
    /// <summary>Bindable property for <see cref="PaginationState"/>.</summary>
    public static readonly BindableProperty PaginationStateProperty = BindableProperty.Create(
        nameof(PaginationState),
        typeof(PaginationState),
        typeof(DataPager));

    /// <summary>Bindable property for <see cref="CurrentRequest"/>.</summary>
    public static readonly BindableProperty CurrentRequestProperty = BindableProperty.Create(
        nameof(CurrentRequest),
        typeof(PageRequest),
        typeof(DataPager));

    /// <summary>Bindable property for <see cref="PageRequestCommand"/>.</summary>
    public static readonly BindableProperty PageRequestCommandProperty = BindableProperty.Create(
        nameof(PageRequestCommand),
        typeof(ICommand),
        typeof(DataPager));

    /// <summary>Bindable property for <see cref="SortKey"/>.</summary>
    public static readonly BindableProperty SortKeyProperty = BindableProperty.Create(
        nameof(SortKey),
        typeof(string),
        typeof(DataPager));

    /// <summary>Bindable property for <see cref="SortDescending"/>.</summary>
    public static readonly BindableProperty SortDescendingProperty = BindableProperty.Create(
        nameof(SortDescending),
        typeof(bool),
        typeof(DataPager));

    /// <summary>Bindable property for <see cref="QueryState"/>.</summary>
    public static readonly BindableProperty QueryStateProperty = BindableProperty.Create(
        nameof(QueryState),
        typeof(SearchQueryState),
        typeof(DataPager));

    /// <summary>Default page size used when no pagination state has been supplied.</summary>
    private const int DefaultPageSize = 20;

    /// <summary>Initializes a new instance of the <see cref="DataPager"/> class.</summary>
    public DataPager()
    {
        FirstPageCommand = new PageNavigationCommand(() => MoveToPage(0), () => PaginationState?.CanGoFirst == true);
        PreviousPageCommand = new PageNavigationCommand(
            () => MoveToPage((PaginationState?.PageIndex ?? 0) - 1),
            () => PaginationState?.CanGoPrevious == true);
        NextPageCommand = new PageNavigationCommand(
            () => MoveToPage((PaginationState?.PageIndex ?? 0) + 1),
            () => PaginationState?.CanGoNext == true);
        LastPageCommand = new PageNavigationCommand(
            () => MoveToPage((PaginationState?.TotalPages ?? 1) - 1),
            () => PaginationState?.CanGoLast == true);
    }

    /// <summary>Gets or sets the shared pagination state projected by the control.</summary>
    public PaginationState? PaginationState
    {
        get => (PaginationState?)GetValue(PaginationStateProperty);
        set => SetValue(PaginationStateProperty, value);
    }

    /// <summary>Gets or sets the latest page request emitted by the control.</summary>
    public PageRequest? CurrentRequest
    {
        get => (PageRequest?)GetValue(CurrentRequestProperty);
        set => SetValue(CurrentRequestProperty, value);
    }

    /// <summary>Gets or sets the command invoked when a page request is emitted.</summary>
    public ICommand? PageRequestCommand
    {
        get => (ICommand?)GetValue(PageRequestCommandProperty);
        set => SetValue(PageRequestCommandProperty, value);
    }

    /// <summary>Gets or sets the sort key included with emitted page requests.</summary>
    public string? SortKey
    {
        get => (string?)GetValue(SortKeyProperty);
        set => SetValue(SortKeyProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether emitted page requests sort descending.</summary>
    public bool SortDescending
    {
        get => (bool)GetValue(SortDescendingProperty);
        set => SetValue(SortDescendingProperty, value);
    }

    /// <summary>Gets or sets the search/filter state snapshot included with emitted page requests.</summary>
    public SearchQueryState? QueryState
    {
        get => (SearchQueryState?)GetValue(QueryStateProperty);
        set => SetValue(QueryStateProperty, value);
    }

    /// <summary>Gets the command that requests the first page.</summary>
    public ICommand FirstPageCommand { get; }

    /// <summary>Gets the command that requests the previous page.</summary>
    public ICommand PreviousPageCommand { get; }

    /// <summary>Gets the command that requests the next page.</summary>
    public ICommand NextPageCommand { get; }

    /// <summary>Gets the command that requests the last page.</summary>
    public ICommand LastPageCommand { get; }

    /// <summary>Creates a page request for the specified zero-based page index.</summary>
    /// <param name="pageIndex">The requested page index.</param>
    /// <returns>The page request snapshot.</returns>
    public PageRequest CreateRequest(int pageIndex)
    {
        var state = PaginationState;
        var clampedPageIndex = state is null ? Math.Max(0, pageIndex) : Math.Clamp(pageIndex, 0, state.TotalPages - 1);
        var pageSize = state?.PageSize ?? DefaultPageSize;
        return new PageRequest(clampedPageIndex, pageSize, SortKey, SortDescending, QueryState);
    }

    /// <summary>Emits a page request for the specified zero-based page index.</summary>
    /// <param name="pageIndex">The requested page index.</param>
    public void MoveToPage(int pageIndex)
    {
        var request = CreateRequest(pageIndex);
        CurrentRequest = request;

        if (PageRequestCommand?.CanExecute(request) != true)
        {
            return;
        }

        PageRequestCommand.Execute(request);
    }

    /// <summary>Command wrapper for page navigation actions.</summary>
    /// <param name="execute">The execute action.</param>
    /// <param name="canExecute">The can execute predicate.</param>
    private sealed class PageNavigationCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        /// <summary>Stores the can execute predicate.</summary>
        private readonly Func<bool> _canExecute = canExecute;

        /// <summary>Stores the execute action.</summary>
        private readonly Action _execute = execute;

        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc />
        public bool CanExecute(object? parameter) => _canExecute();

        /// <inheritdoc />
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            _execute();
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
