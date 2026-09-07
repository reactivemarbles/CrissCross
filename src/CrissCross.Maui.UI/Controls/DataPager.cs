// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Layouts;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Represents a paging/navigation surface for local or remote MAUI data sources.</summary>
public class DataPager : ContentView
{
    /// <summary>Bindable property for <see cref="PaginationState"/>.</summary>
    public static readonly BindableProperty PaginationStateProperty = BindableProperty.Create(
        nameof(PaginationState),
        typeof(PaginationState),
        typeof(DataPager),
        propertyChanged: static (bindable, _, _) => OnPaginationStateChanged(bindable));

    /// <summary>Bindable property for <see cref="CurrentRequest"/>.</summary>
    public static readonly BindableProperty CurrentRequestProperty = BindableProperty.Create(
        nameof(CurrentRequest),
        typeof(PageRequest),
        typeof(DataPager));

    /// <summary>Bindable property for <see cref="PageRequestCommand"/>.</summary>
    public static readonly BindableProperty PageRequestCommandProperty = BindableProperty.Create(
        nameof(PageRequestCommand),
        typeof(ICommand),
        typeof(DataPager),
        propertyChanged: static (bindable, _, _) => OnPaginationStateChanged(bindable));

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

    /// <summary>Provides horizontal padding for pager buttons.</summary>
    private const double ButtonHorizontalPadding = 10;

    /// <summary>Provides vertical padding for pager buttons.</summary>
    private const double ButtonVerticalPadding = 6;

    /// <summary>Provides the minimum width for pager buttons.</summary>
    private const double ButtonMinimumWidth = 72;

    /// <summary>Provides vertical padding for the composed pager surface.</summary>
    private const double RootVerticalPadding = 4;

    /// <summary>Provides spacing for the composed pager surface.</summary>
    private const double LayoutSpacing = 6;

    /// <summary>Displays the current item range.</summary>
    private readonly Label _summaryLabel = CreateLabel("No items", isEmphasized: true);

    /// <summary>Displays the current page number and total pages.</summary>
    private readonly Label _statusLabel = CreateLabel("Page 1 of 1", isEmphasized: false);

    /// <summary>Requests the first page.</summary>
    private readonly Button _firstButton = CreateButton("First");

    /// <summary>Requests the previous page.</summary>
    private readonly Button _previousButton = CreateButton("Previous");

    /// <summary>Requests the next page.</summary>
    private readonly Button _nextButton = CreateButton("Next");

    /// <summary>Requests the last page.</summary>
    private readonly Button _lastButton = CreateButton("Last");

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

        _firstButton.Command = FirstPageCommand;
        _previousButton.Command = PreviousPageCommand;
        _nextButton.Command = NextPageCommand;
        _lastButton.Command = LastPageCommand;
        Content = CreateLayout();
        ApplyState();
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
        var maxPageIndex = state is null ? int.MaxValue : Math.Max(0, state.TotalPages - 1);
        var clampedPageIndex = Math.Clamp(pageIndex, 0, maxPageIndex);
        var pageSize = state?.PageSize ?? DefaultPageSize;
        return new(clampedPageIndex, pageSize, SortKey, SortDescending, QueryState);
    }

    /// <summary>Emits a page request for the specified zero-based page index.</summary>
    /// <param name="pageIndex">The requested page index.</param>
    public void MoveToPage(int pageIndex)
    {
        var request = CreateRequest(pageIndex);
        CurrentRequest = request;

        if (PageRequestCommand?.CanExecute(request) == true)
        {
            PageRequestCommand.Execute(request);
        }

        ApplyState();
    }

    /// <summary>Creates a pager navigation button.</summary>
    /// <param name="text">The button text.</param>
    /// <returns>The configured button.</returns>
    private static Button CreateButton(string text)
    {
        var button = new Button { Text = text, Padding = new(ButtonHorizontalPadding, ButtonVerticalPadding), MinimumWidthRequest = ButtonMinimumWidth };
        button.SetDynamicResource(Button.BackgroundColorProperty, "CrissCrossAccentColor");
        button.SetDynamicResource(Button.TextColorProperty, "CrissCrossAccentTextColor");
        button.Margin = new(0, 0, LayoutSpacing, LayoutSpacing);
        return button;
    }

    /// <summary>Creates a pager display label.</summary>
    /// <param name="text">The label text.</param>
    /// <param name="isEmphasized">A value indicating whether the label should use bold text.</param>
    /// <returns>The configured label.</returns>
    private static Label CreateLabel(string text, bool isEmphasized)
    {
        var label = new Label { Text = text, FontAttributes = isEmphasized ? FontAttributes.Bold : FontAttributes.None };
        label.SetDynamicResource(Label.TextColorProperty, "CrissCrossTextColor");
        return label;
    }

    /// <summary>Applies bindable property changes to the pager presentation.</summary>
    /// <param name="bindable">The bindable control.</param>
    private static void OnPaginationStateChanged(BindableObject bindable)
    {
        if (bindable is not DataPager pager)
        {
            return;
        }

        pager.ApplyState();
    }

    /// <summary>Creates the native MAUI pager layout.</summary>
    /// <returns>The composed pager view.</returns>
    private VerticalStackLayout CreateLayout()
    {
        var root = new VerticalStackLayout { Spacing = LayoutSpacing, Padding = new(0, RootVerticalPadding) };
        var buttonRow = new FlexLayout { Direction = FlexDirection.Row, Wrap = FlexWrap.Wrap, AlignItems = FlexAlignItems.Center };
        buttonRow.Children.Add(_firstButton);
        buttonRow.Children.Add(_previousButton);
        buttonRow.Children.Add(_nextButton);
        buttonRow.Children.Add(_lastButton);
        root.Children.Add(_summaryLabel);
        root.Children.Add(_statusLabel);
        root.Children.Add(buttonRow);
        return root;
    }

    /// <summary>Applies the current pagination state to labels, buttons, and command availability.</summary>
    private void ApplyState()
    {
        var state = PaginationState;
        _summaryLabel.Text = state?.SummaryText ?? "No items";
        _statusLabel.Text = state is null ? "Page 1 of 1" : $"Page {state.PageNumber} of {state.TotalPages}";
        _firstButton.IsEnabled = FirstPageCommand.CanExecute(null);
        _previousButton.IsEnabled = PreviousPageCommand.CanExecute(null);
        _nextButton.IsEnabled = NextPageCommand.CanExecute(null);
        _lastButton.IsEnabled = LastPageCommand.CanExecute(null);

        if (FirstPageCommand is PageNavigationCommand firstCommand)
        {
            firstCommand.RaiseCanExecuteChanged();
        }

        if (PreviousPageCommand is PageNavigationCommand previousCommand)
        {
            previousCommand.RaiseCanExecuteChanged();
        }

        if (NextPageCommand is PageNavigationCommand nextCommand)
        {
            nextCommand.RaiseCanExecuteChanged();
        }

        if (LastPageCommand is not PageNavigationCommand lastCommand)
        {
            return;
        }

        lastCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Command wrapper for page navigation actions.</summary>
    /// <param name="execute">The execute action.</param>
    /// <param name="canExecute">The can execute predicate.</param>
    private sealed class PageNavigationCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        /// <summary>Stores the command availability predicate.</summary>
        private readonly Func<bool> _canExecute = canExecute;

        /// <summary>Stores the command action.</summary>
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
        }

        /// <summary>Raises command availability changes after pager state changes.</summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
