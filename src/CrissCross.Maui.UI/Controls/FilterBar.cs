// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Displays active search/filter tokens with clear and remove command hooks.
/// </summary>
public class FilterBar : ContentView
{
    /// <summary>
    /// Bindable property for <see cref="SearchState"/>.
    /// </summary>
    public static readonly BindableProperty SearchStateProperty = BindableProperty.Create(
        nameof(SearchState),
        typeof(SearchQueryState),
        typeof(FilterBar));

    /// <summary>
    /// Bindable property for <see cref="ClearFiltersCommand"/>.
    /// </summary>
    public static readonly BindableProperty ClearFiltersCommandProperty = BindableProperty.Create(
        nameof(ClearFiltersCommand),
        typeof(ICommand),
        typeof(FilterBar));

    /// <summary>
    /// Gets or sets the shared CrissCross state projected by this control.
    /// </summary>
    public SearchQueryState? SearchState
    {
        get => (SearchQueryState?)GetValue(SearchStateProperty);
        set => SetValue(SearchStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked by the control surface.
    /// </summary>
    public ICommand? ClearFiltersCommand
    {
        get => (ICommand?)GetValue(ClearFiltersCommandProperty);
        set => SetValue(ClearFiltersCommandProperty, value);
    }
}
