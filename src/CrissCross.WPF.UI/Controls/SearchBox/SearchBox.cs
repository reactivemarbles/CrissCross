// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents a lightweight search input surface that projects shared SearchQueryState.</summary>
public class SearchBox : Control
{
    /// <summary>Property for <see cref="Text"/>.</summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(SearchBox),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>Property for <see cref="PlaceholderText"/>.</summary>
    public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
        nameof(PlaceholderText),
        typeof(string),
        typeof(SearchBox),
        new PropertyMetadata("Search"));

    /// <summary>Property for <see cref="QueryState"/>.</summary>
    public static readonly DependencyProperty QueryStateProperty = DependencyProperty.Register(
        nameof(QueryState),
        typeof(SearchQueryState),
        typeof(SearchBox),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="SearchCommand"/>.</summary>
    public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register(
        nameof(SearchCommand),
        typeof(ICommand),
        typeof(SearchBox),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="ClearCommand"/>.</summary>
    public static readonly DependencyProperty ClearCommandProperty = DependencyProperty.Register(
        nameof(ClearCommand),
        typeof(ICommand),
        typeof(SearchBox),
        new PropertyMetadata(null));

    /// <summary>Gets or sets the current raw search text.</summary>
    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets or sets the placeholder text shown by the platform template.</summary>
    public string? PlaceholderText
    {
        get => (string?)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>Gets or sets the aggregate search state for result counts, active filters, and activity.</summary>
    public SearchQueryState? QueryState
    {
        get => (SearchQueryState?)GetValue(QueryStateProperty);
        set => SetValue(QueryStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked when the user submits the current search text.</summary>
    public ICommand? SearchCommand
    {
        get => (ICommand?)GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked when the user clears the current search text.</summary>
    public ICommand? ClearCommand
    {
        get => (ICommand?)GetValue(ClearCommandProperty);
        set => SetValue(ClearCommandProperty, value);
    }
}
