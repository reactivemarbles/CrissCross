// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents active filter tokens associated with a <see cref="SearchBox"/>.</summary>
public class FilterBar : ItemsControl
{
    /// <summary>Property for <see cref="QueryState"/>.</summary>
    public static readonly DependencyProperty QueryStateProperty = DependencyProperty.Register(
        nameof(QueryState),
        typeof(SearchQueryState),
        typeof(FilterBar),
        new PropertyMetadata(null, OnQueryStateChanged));

    /// <summary>Property for <see cref="RemoveFilterCommand"/>.</summary>
    public static readonly DependencyProperty RemoveFilterCommandProperty = DependencyProperty.Register(
        nameof(RemoveFilterCommand),
        typeof(ICommand),
        typeof(FilterBar),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="ClearAllCommand"/>.</summary>
    public static readonly DependencyProperty ClearAllCommandProperty = DependencyProperty.Register(
        nameof(ClearAllCommand),
        typeof(ICommand),
        typeof(FilterBar),
        new PropertyMetadata(null));

    /// <summary>Gets or sets the aggregate search state that supplies active filter tokens.</summary>
    public SearchQueryState? QueryState
    {
        get => (SearchQueryState?)GetValue(QueryStateProperty);
        set => SetValue(QueryStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked for an individual removable filter token.</summary>
    public ICommand? RemoveFilterCommand
    {
        get => (ICommand?)GetValue(RemoveFilterCommandProperty);
        set => SetValue(RemoveFilterCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked to clear all active filters.</summary>
    public ICommand? ClearAllCommand
    {
        get => (ICommand?)GetValue(ClearAllCommandProperty);
        set => SetValue(ClearAllCommandProperty, value);
    }

    /// <summary>Provides the OnQueryStateChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnQueryStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not FilterBar filterBar || args.NewValue is not SearchQueryState state)
        {
            return;
        }

        filterBar.SetCurrentValue(ItemsSourceProperty, state.ActiveFilters);
    }
}
