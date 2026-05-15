// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a paging/navigation surface for local or remote data sources.
/// </summary>
public class DataPager : Control
{
    /// <summary>
    /// Property for <see cref="PaginationState"/>.
    /// </summary>
    public static readonly DependencyProperty PaginationStateProperty = DependencyProperty.Register(
        nameof(PaginationState),
        typeof(PaginationState),
        typeof(DataPager),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="CurrentRequest"/>.
    /// </summary>
    public static readonly DependencyProperty CurrentRequestProperty = DependencyProperty.Register(
        nameof(CurrentRequest),
        typeof(PageRequest),
        typeof(DataPager),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="PageRequestCommand"/>.
    /// </summary>
    public static readonly DependencyProperty PageRequestCommandProperty = DependencyProperty.Register(
        nameof(PageRequestCommand),
        typeof(ICommand),
        typeof(DataPager),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="SortKey"/>.
    /// </summary>
    public static readonly DependencyProperty SortKeyProperty = DependencyProperty.Register(
        nameof(SortKey),
        typeof(string),
        typeof(DataPager),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="SortDescending"/>.
    /// </summary>
    public static readonly DependencyProperty SortDescendingProperty = DependencyProperty.Register(
        nameof(SortDescending),
        typeof(bool),
        typeof(DataPager),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="QueryState"/>.
    /// </summary>
    public static readonly DependencyProperty QueryStateProperty = DependencyProperty.Register(
        nameof(QueryState),
        typeof(SearchQueryState),
        typeof(DataPager),
        new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPager"/> class.
    /// </summary>
    public DataPager()
    {
        FirstPageCommand = new PageNavigationCommand(() => MoveToPage(0), () => PaginationState?.CanGoFirst == true);
        PreviousPageCommand = new PageNavigationCommand(() => MoveToPage((PaginationState?.PageIndex ?? 0) - 1), () => PaginationState?.CanGoPrevious == true);
        NextPageCommand = new PageNavigationCommand(() => MoveToPage((PaginationState?.PageIndex ?? 0) + 1), () => PaginationState?.CanGoNext == true);
        LastPageCommand = new PageNavigationCommand(() => MoveToPage((PaginationState?.TotalPages ?? 1) - 1), () => PaginationState?.CanGoLast == true);
    }

    /// <summary>
    /// Gets or sets the shared pagination state projected by the control.
    /// </summary>
    public PaginationState? PaginationState
    {
        get => (PaginationState?)GetValue(PaginationStateProperty);
        set => SetValue(PaginationStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the latest page request emitted by the control.
    /// </summary>
    public PageRequest? CurrentRequest
    {
        get => (PageRequest?)GetValue(CurrentRequestProperty);
        set => SetValue(CurrentRequestProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when a page request is emitted.
    /// </summary>
    public ICommand? PageRequestCommand
    {
        get => (ICommand?)GetValue(PageRequestCommandProperty);
        set => SetValue(PageRequestCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the sort key included with emitted page requests.
    /// </summary>
    public string? SortKey
    {
        get => (string?)GetValue(SortKeyProperty);
        set => SetValue(SortKeyProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether emitted page requests sort descending.
    /// </summary>
    public bool SortDescending
    {
        get => (bool)GetValue(SortDescendingProperty);
        set => SetValue(SortDescendingProperty, value);
    }

    /// <summary>
    /// Gets or sets the search/filter state snapshot included with emitted page requests.
    /// </summary>
    public SearchQueryState? QueryState
    {
        get => (SearchQueryState?)GetValue(QueryStateProperty);
        set => SetValue(QueryStateProperty, value);
    }

    /// <summary>
    /// Gets the command that requests the first page.
    /// </summary>
    public ICommand FirstPageCommand { get; }

    /// <summary>
    /// Gets the command that requests the previous page.
    /// </summary>
    public ICommand PreviousPageCommand { get; }

    /// <summary>
    /// Gets the command that requests the next page.
    /// </summary>
    public ICommand NextPageCommand { get; }

    /// <summary>
    /// Gets the command that requests the last page.
    /// </summary>
    public ICommand LastPageCommand { get; }

    /// <summary>
    /// Creates a page request for the specified zero-based page index.
    /// </summary>
    /// <param name="pageIndex">The requested page index.</param>
    /// <returns>The page request snapshot.</returns>
    public PageRequest CreateRequest(int pageIndex)
    {
        var state = PaginationState;
        var clampedPageIndex = state is null ? Math.Max(0, pageIndex) : Math.Clamp(pageIndex, 0, state.TotalPages - 1);
        var pageSize = state?.PageSize ?? 20;
        return new PageRequest(clampedPageIndex, pageSize, SortKey, SortDescending, QueryState);
    }

    /// <summary>
    /// Emits a page request for the specified zero-based page index.
    /// </summary>
    /// <param name="pageIndex">The requested page index.</param>
    public void MoveToPage(int pageIndex)
    {
        var request = CreateRequest(pageIndex);
        CurrentRequest = request;

        if (PageRequestCommand?.CanExecute(request) == true)
        {
            PageRequestCommand.Execute(request);
        }
    }

    private sealed class PageNavigationCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public PageNavigationCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute();
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
