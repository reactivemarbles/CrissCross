// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a descriptor-driven filter editor surface for grids and lists.
/// </summary>
public class DataFilterPanel : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="FilterState"/>.
    /// </summary>
    public static readonly StyledProperty<DataFilterPanelState?> FilterStateProperty = AvaloniaProperty.Register<DataFilterPanel, DataFilterPanelState?>(nameof(FilterState));

    /// <summary>
    /// Property for <see cref="SearchText"/>.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty = AvaloniaProperty.Register<DataFilterPanel, string?>(nameof(SearchText));

    /// <summary>
    /// Property for <see cref="ResultCount"/>.
    /// </summary>
    public static readonly StyledProperty<int?> ResultCountProperty = AvaloniaProperty.Register<DataFilterPanel, int?>(nameof(ResultCount));

    /// <summary>
    /// Property for <see cref="SubmittedQueryState"/>.
    /// </summary>
    public static readonly StyledProperty<SearchQueryState?> SubmittedQueryStateProperty = AvaloniaProperty.Register<DataFilterPanel, SearchQueryState?>(nameof(SubmittedQueryState));

    /// <summary>
    /// Property for <see cref="ApplyFiltersCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> ApplyFiltersCommandProperty = AvaloniaProperty.Register<DataFilterPanel, ICommand?>(nameof(ApplyFiltersCommand));

    /// <summary>
    /// Property for <see cref="ClearFiltersCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> ClearFiltersCommandProperty = AvaloniaProperty.Register<DataFilterPanel, ICommand?>(nameof(ClearFiltersCommand));

    /// <summary>
    /// Property for <see cref="AddFilterCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> AddFilterCommandProperty = AvaloniaProperty.Register<DataFilterPanel, ICommand?>(nameof(AddFilterCommand));

    /// <summary>
    /// Property for <see cref="RemoveFilterCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> RemoveFilterCommandProperty = AvaloniaProperty.Register<DataFilterPanel, ICommand?>(nameof(RemoveFilterCommand));

    /// <summary>
    /// Initializes a new instance of the <see cref="DataFilterPanel"/> class.
    /// </summary>
    public DataFilterPanel()
    {
        ApplyCommand = new PanelCommand(ApplyFilters, () => FilterState?.CanApply == true);
        ClearCommand = new PanelCommand(ClearFilters, () => FilterState?.CanClear == true);
    }

    /// <summary>
    /// Gets or sets the shared filter panel state projected by the control.
    /// </summary>
    public DataFilterPanelState? FilterState
    {
        get => GetValue(FilterStateProperty);
        set => SetValue(FilterStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the query text included when applying filters.
    /// </summary>
    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the optional result count included with the emitted query state.
    /// </summary>
    public int? ResultCount
    {
        get => GetValue(ResultCountProperty);
        set => SetValue(ResultCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the latest query state emitted by the panel.
    /// </summary>
    public SearchQueryState? SubmittedQueryState
    {
        get => GetValue(SubmittedQueryStateProperty);
        set => SetValue(SubmittedQueryStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when filters are applied.
    /// </summary>
    public ICommand? ApplyFiltersCommand
    {
        get => GetValue(ApplyFiltersCommandProperty);
        set => SetValue(ApplyFiltersCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when filters are cleared.
    /// </summary>
    public ICommand? ClearFiltersCommand
    {
        get => GetValue(ClearFiltersCommandProperty);
        set => SetValue(ClearFiltersCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when a descriptor should add or edit a filter.
    /// </summary>
    public ICommand? AddFilterCommand
    {
        get => GetValue(AddFilterCommandProperty);
        set => SetValue(AddFilterCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when an active filter token should be removed.
    /// </summary>
    public ICommand? RemoveFilterCommand
    {
        get => GetValue(RemoveFilterCommandProperty);
        set => SetValue(RemoveFilterCommandProperty, value);
    }

    /// <summary>
    /// Gets the command that applies current filters.
    /// </summary>
    public ICommand ApplyCommand { get; }

    /// <summary>
    /// Gets the command that clears current filters.
    /// </summary>
    public ICommand ClearCommand { get; }

    /// <summary>
    /// Creates a search query snapshot from the current filter state.
    /// </summary>
    /// <returns>The query state snapshot.</returns>
    public SearchQueryState CreateQueryState() => FilterState?.ToSearchQueryState(SearchText, ResultCount) ?? new SearchQueryState(SearchText, submittedText: SearchText, resultCount: ResultCount);

    /// <summary>
    /// Applies the current filters and emits a query-state snapshot.
    /// </summary>
    public void ApplyFilters()
    {
        var queryState = CreateQueryState();
        SubmittedQueryState = queryState;

        if (ApplyFiltersCommand?.CanExecute(queryState) == true)
        {
            ApplyFiltersCommand.Execute(queryState);
        }
    }

    /// <summary>
    /// Clears current filters through the configured command hook.
    /// </summary>
    public void ClearFilters()
    {
        var state = FilterState;
        if (ClearFiltersCommand?.CanExecute(state) == true)
        {
            ClearFiltersCommand.Execute(state);
        }
    }

    private sealed class PanelCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public PanelCommand(Action execute, Func<bool> canExecute)
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
