// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Layouts;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays active search/filter tokens with clear and remove command hooks.</summary>
public class FilterBar : ContentView
{
    /// <summary>Bindable property for <see cref="SearchState"/>.</summary>
    public static readonly BindableProperty SearchStateProperty = BindableProperty.Create(
        nameof(SearchState),
        typeof(SearchQueryState),
        typeof(FilterBar),
        propertyChanged: static (bindable, _, newValue) => OnSearchStateChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="ClearFiltersCommand"/>.</summary>
    public static readonly BindableProperty ClearFiltersCommandProperty = BindableProperty.Create(
        nameof(ClearFiltersCommand),
        typeof(ICommand),
        typeof(FilterBar),
        propertyChanged: static (bindable, _, _) => OnCommandChanged(bindable));

    /// <summary>Bindable property for <see cref="RemoveFilterCommand"/>.</summary>
    public static readonly BindableProperty RemoveFilterCommandProperty = BindableProperty.Create(
        nameof(RemoveFilterCommand),
        typeof(ICommand),
        typeof(FilterBar),
        propertyChanged: static (bindable, _, _) => OnCommandChanged(bindable));

    /// <summary>Semantic text color resource key.</summary>
    private const string TextColorResourceKey = "CrissCrossTextColor";

    /// <summary>Semantic accent color resource key.</summary>
    private const string AccentColorResourceKey = "CrissCrossAccentColor";

    /// <summary>Semantic accent text color resource key.</summary>
    private const string AccentTextColorResourceKey = "CrissCrossAccentTextColor";

    /// <summary>Semantic subtle surface color resource key.</summary>
    private const string SubtleSurfaceColorResourceKey = "CrissCrossSubtleSurfaceColor";

    /// <summary>Horizontal action padding.</summary>
    private const double ActionPaddingHorizontal = 10D;

    /// <summary>Vertical action padding.</summary>
    private const double ActionPaddingVertical = 6D;

    /// <summary>Minimum action width.</summary>
    private const double ActionMinimumWidth = 96D;

    /// <summary>Root layout spacing.</summary>
    private const double RootSpacing = 8D;

    /// <summary>Root vertical padding.</summary>
    private const double RootPaddingVertical = 4D;

    /// <summary>Chip layout spacing.</summary>
    private const double ChipSpacing = 6D;

    /// <summary>Chip horizontal padding.</summary>
    private const double ChipPaddingHorizontal = 10D;

    /// <summary>Chip vertical padding.</summary>
    private const double ChipPaddingVertical = 4D;

    /// <summary>Displays the current active-filter summary.</summary>
    private readonly Label _summaryLabel = CreateLabel("No active filters", FontAttributes.Bold);

    /// <summary>Hosts active filter chip rows.</summary>
    private readonly FlexLayout _chipHost = new() { Direction = FlexDirection.Row, Wrap = FlexWrap.Wrap, AlignItems = FlexAlignItems.Center };

    /// <summary>Clears all active filters.</summary>
    private readonly Button _clearButton = CreateActionButton("Clear filters");

    /// <summary>Initializes a new instance of the <see cref="FilterBar"/> class.</summary>
    public FilterBar()
    {
        Content = CreateLayout();
        ApplyState(null);
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public SearchQueryState? SearchState
    {
        get => (SearchQueryState?)GetValue(SearchStateProperty);
        set => SetValue(SearchStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? ClearFiltersCommand
    {
        get => (ICommand?)GetValue(ClearFiltersCommandProperty);
        set => SetValue(ClearFiltersCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked when a removable filter token is removed.</summary>
    public ICommand? RemoveFilterCommand
    {
        get => (ICommand?)GetValue(RemoveFilterCommandProperty);
        set => SetValue(RemoveFilterCommandProperty, value);
    }

    /// <summary>Invokes the clear-filters command with the current search state.</summary>
    /// <returns><c>true</c> when the clear command was executed.</returns>
    public bool ClearFilters()
    {
        var state = SearchState;
        if (ClearFiltersCommand?.CanExecute(state) != true)
        {
            return false;
        }

        ClearFiltersCommand.Execute(state);
        return true;
    }

    /// <summary>Invokes the remove-filter command for a token.</summary>
    /// <param name="token">The token to remove.</param>
    /// <returns><c>true</c> when the remove command was executed.</returns>
    public bool RemoveFilter(FilterToken token)
    {
        if (!token.IsRemovable || RemoveFilterCommand?.CanExecute(token) != true)
        {
            return false;
        }

        RemoveFilterCommand.Execute(token);
        return true;
    }

    /// <summary>Creates a label using semantic theme color resources.</summary>
    /// <param name="text">The label text.</param>
    /// <param name="fontAttributes">The font attributes.</param>
    /// <returns>The configured label.</returns>
    private static Label CreateLabel(string text, FontAttributes fontAttributes)
    {
        var label = new Label { Text = text, FontAttributes = fontAttributes, VerticalTextAlignment = TextAlignment.Center };
        label.SetDynamicResource(Label.TextColorProperty, TextColorResourceKey);
        return label;
    }

    /// <summary>Creates an action button using semantic theme color resources.</summary>
    /// <param name="text">The button text.</param>
    /// <returns>The configured button.</returns>
    private static Button CreateActionButton(string text)
    {
        var button = new Button { Text = text, Padding = new(ActionPaddingHorizontal, ActionPaddingVertical), MinimumWidthRequest = ActionMinimumWidth };
        button.SetDynamicResource(Button.BackgroundColorProperty, AccentColorResourceKey);
        button.SetDynamicResource(Button.TextColorProperty, AccentTextColorResourceKey);
        return button;
    }

    /// <summary>Runs when the search state changes.</summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnSearchStateChanged(BindableObject bindable, object? newValue)
    {
        if (bindable is not FilterBar filterBar)
        {
            return;
        }

        filterBar.ApplyState(newValue as SearchQueryState);
    }

    /// <summary>Runs when command availability changes.</summary>
    /// <param name="bindable">The bindable object.</param>
    private static void OnCommandChanged(BindableObject bindable)
    {
        if (bindable is not FilterBar filterBar)
        {
            return;
        }

        filterBar.ApplyState(filterBar.SearchState);
    }

    /// <summary>Gets the summary text for the current filters.</summary>
    /// <param name="filterCount">The active filter count.</param>
    /// <param name="resultSummary">The optional result summary.</param>
    /// <returns>The formatted summary text.</returns>
    private static string GetSummaryText(int filterCount, string? resultSummary) => resultSummary is { Length: > 0 }
        ? $"{filterCount} active filters, {resultSummary}"
        : $"{filterCount} active filters";

    /// <summary>Creates the root layout.</summary>
    /// <returns>The root layout.</returns>
    private VerticalStackLayout CreateLayout()
    {
        var root = new VerticalStackLayout { Spacing = RootSpacing, Padding = new(0D, RootPaddingVertical) };
        root.Children.Add(_summaryLabel);
        root.Children.Add(_chipHost);
        root.Children.Add(_clearButton);
        return root;
    }

    /// <summary>Applies search state to native child presenters.</summary>
    /// <param name="state">The search state.</param>
    private void ApplyState(SearchQueryState? state)
    {
        var filters = state?.ActiveFilters ?? [];
        _chipHost.Children.Clear();
        _summaryLabel.Text = filters.Count == 0
            ? "No active filters"
            : GetSummaryText(filters.Count, state?.ResultSummary);
        _clearButton.Command = ClearFiltersCommand;
        _clearButton.CommandParameter = state;
        _clearButton.IsEnabled = filters.Count > 0 && ClearFiltersCommand?.CanExecute(state) == true;

        foreach (var token in filters)
        {
            _chipHost.Children.Add(CreateChip(token));
        }

        SemanticProperties.SetDescription(this, _summaryLabel.Text);
    }

    /// <summary>Creates a native chip row for one active filter token.</summary>
    /// <param name="token">The token to present.</param>
    /// <returns>The chip view.</returns>
    private HorizontalStackLayout CreateChip(FilterToken token)
    {
        var label = CreateLabel(token.DisplayText, FontAttributes.None);
        var removeButton = CreateActionButton("Remove");
        removeButton.Command = RemoveFilterCommand;
        removeButton.CommandParameter = token;
        removeButton.IsEnabled = token.IsRemovable && RemoveFilterCommand?.CanExecute(token) == true;

        var chip = new HorizontalStackLayout { Spacing = ChipSpacing, Padding = new(ChipPaddingHorizontal, ChipPaddingVertical) };
        chip.SetDynamicResource(VisualElement.BackgroundColorProperty, SubtleSurfaceColorResourceKey);
        chip.Children.Add(label);
        chip.Children.Add(removeButton);
        SemanticProperties.SetDescription(chip, token.DisplayText);
        return chip;
    }
}
