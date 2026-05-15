// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Represents a search input that projects a shared <see cref="SearchQueryState"/> snapshot.
/// </summary>
public class SearchBox : SearchBar
{
    /// <summary>
    /// Bindable property for <see cref="SearchState"/>.
    /// </summary>
    public static readonly BindableProperty SearchStateProperty = BindableProperty.Create(
        nameof(SearchState),
        typeof(SearchQueryState),
        typeof(SearchBox),
        propertyChanged: OnSearchStateChanged);

    /// <summary>
    /// Bindable property for <see cref="SubmitCommand"/>.
    /// </summary>
    public static readonly BindableProperty SubmitCommandProperty = BindableProperty.Create(
        nameof(SubmitCommand),
        typeof(ICommand),
        typeof(SearchBox));

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchBox"/> class.
    /// </summary>
    public SearchBox() => SearchButtonPressed += OnSearchButtonPressed;

    /// <summary>
    /// Gets or sets the shared search state snapshot.
    /// </summary>
    public SearchQueryState? SearchState
    {
        get => (SearchQueryState?)GetValue(SearchStateProperty);
        set => SetValue(SearchStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when a search is submitted.
    /// </summary>
    public ICommand? SubmitCommand
    {
        get => (ICommand?)GetValue(SubmitCommandProperty);
        set => SetValue(SubmitCommandProperty, value);
    }

    /// <summary>
    /// Submits the current query text through <see cref="SubmitCommand"/>.
    /// </summary>
    /// <returns><c>true</c> when the command was invoked; otherwise, <c>false</c>.</returns>
    public bool SubmitSearch()
    {
        var normalizedText = (Text ?? SearchState?.Text ?? string.Empty).Trim();
        if (SubmitCommand?.CanExecute(normalizedText) != true)
        {
            return false;
        }

        SubmitCommand.Execute(normalizedText);
        return true;
    }

    private static void OnSearchStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchBox searchBox && newValue is SearchQueryState state)
        {
            searchBox.Text = state.Text;
        }
    }

    private void OnSearchButtonPressed(object? sender, EventArgs e) => SubmitSearch();
}
