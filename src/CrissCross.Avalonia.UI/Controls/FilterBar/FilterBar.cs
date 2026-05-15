// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents active filter tokens associated with a <see cref="SearchBox"/>.
/// </summary>
public class FilterBar : ItemsControl
{
    /// <summary>
    /// Property for <see cref="QueryState"/>.
    /// </summary>
    public static readonly StyledProperty<SearchQueryState?> QueryStateProperty = AvaloniaProperty.Register<FilterBar, SearchQueryState?>(nameof(QueryState));

    /// <summary>
    /// Property for <see cref="RemoveFilterCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> RemoveFilterCommandProperty = AvaloniaProperty.Register<FilterBar, ICommand?>(nameof(RemoveFilterCommand));

    /// <summary>
    /// Property for <see cref="ClearAllCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> ClearAllCommandProperty = AvaloniaProperty.Register<FilterBar, ICommand?>(nameof(ClearAllCommand));

    /// <summary>
    /// Gets or sets the aggregate search state that supplies active filter tokens.
    /// </summary>
    public SearchQueryState? QueryState
    {
        get => GetValue(QueryStateProperty);
        set => SetValue(QueryStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked for an individual removable filter token.
    /// </summary>
    public ICommand? RemoveFilterCommand
    {
        get => GetValue(RemoveFilterCommandProperty);
        set => SetValue(RemoveFilterCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked to clear all active filters.
    /// </summary>
    public ICommand? ClearAllCommand
    {
        get => GetValue(ClearAllCommandProperty);
        set => SetValue(ClearAllCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == QueryStateProperty && change.GetNewValue<SearchQueryState?>() is { } state)
        {
            ItemsSource = state.ActiveFilters;
        }
    }
}
