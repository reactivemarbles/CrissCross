// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a lightweight search input surface that projects shared <see cref="SearchQueryState"/>.</summary>
public class SearchBox : TemplatedControl
{
    /// <summary>Property for <see cref="Text"/>.</summary>
    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<SearchBox, string?>(nameof(Text), string.Empty, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Property for <see cref="PlaceholderText"/>.</summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty = AvaloniaProperty.Register<SearchBox, string?>(nameof(PlaceholderText), "Search");

    /// <summary>Property for <see cref="QueryState"/>.</summary>
    public static readonly StyledProperty<SearchQueryState?> QueryStateProperty = AvaloniaProperty.Register<SearchBox, SearchQueryState?>(nameof(QueryState));

    /// <summary>Property for <see cref="SearchCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> SearchCommandProperty = AvaloniaProperty.Register<SearchBox, ICommand?>(nameof(SearchCommand));

    /// <summary>Property for <see cref="ClearCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> ClearCommandProperty = AvaloniaProperty.Register<SearchBox, ICommand?>(nameof(ClearCommand));

    /// <summary>Gets or sets the current raw search text.</summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets or sets the placeholder text shown by the platform template.</summary>
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>Gets or sets the aggregate search state for result counts, active filters, and activity.</summary>
    public SearchQueryState? QueryState
    {
        get => GetValue(QueryStateProperty);
        set => SetValue(QueryStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked when the user submits the current search text.</summary>
    public ICommand? SearchCommand
    {
        get => GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked when the user clears the current search text.</summary>
    public ICommand? ClearCommand
    {
        get => GetValue(ClearCommandProperty);
        set => SetValue(ClearCommandProperty, value);
    }
}
