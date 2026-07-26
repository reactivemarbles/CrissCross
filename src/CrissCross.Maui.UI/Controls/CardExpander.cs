// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Provides a card-aligned expandable section composed from native MAUI layouts and commands.</summary>
public class CardExpander : ContentView
{
    /// <summary>Bindable property for <see cref="Header"/>.</summary>
    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header),
        typeof(View),
        typeof(CardExpander),
        propertyChanged: static (view, _, _) => ((CardExpander)view).Refresh());

    /// <summary>Bindable property for <see cref="ExpandedContent"/>.</summary>
    public static readonly BindableProperty ExpandedContentProperty = BindableProperty.Create(
        nameof(ExpandedContent),
        typeof(View),
        typeof(CardExpander),
        propertyChanged: static (view, _, _) => ((CardExpander)view).Refresh());

    /// <summary>Bindable property for <see cref="IsExpanded"/>.</summary>
    public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
        nameof(IsExpanded),
        typeof(bool),
        typeof(CardExpander),
        false,
        BindingMode.TwoWay,
        propertyChanged: static (view, _, _) => ((CardExpander)view).Refresh());

    /// <summary>Bindable property for <see cref="ContentPadding"/>.</summary>
    public static readonly BindableProperty ContentPaddingProperty = BindableProperty.Create(nameof(ContentPadding), typeof(Thickness), typeof(CardExpander), new Thickness(12));

    /// <summary>Spacing between header and expanded content.</summary>
    private const double ExpansionSpacing = 8;

    /// <summary>Hosts the header supplied by the caller.</summary>
    private readonly ContentView _headerHost = new() { HorizontalOptions = LayoutOptions.Fill };

    /// <summary>Hosts the supplementary content while it is expanded.</summary>
    private readonly ContentView _expandedHost = new();

    /// <summary>Initializes a new instance of the <see cref="CardExpander"/> class.</summary>
    public CardExpander()
    {
        var toggle = new TapGestureRecognizer();
        toggle.Tapped += (_, _) => IsExpanded = !IsExpanded;
        _headerHost.GestureRecognizers.Add(toggle);
        Content = new VerticalStackLayout { Spacing = ExpansionSpacing, Children = { _headerHost, _expandedHost } };
        Refresh();
    }

    /// <summary>Gets or sets the visual header that toggles the expanded region.</summary>
    public View? Header
    {
        get => (View?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>Gets or sets the optional visual content shown when <see cref="IsExpanded"/> is true.</summary>
    public View? ExpandedContent
    {
        get => (View?)GetValue(ExpandedContentProperty);
        set => SetValue(ExpandedContentProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the supplementary content is visible.</summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>Gets or sets the padding applied by a card-expander template to expanded content.</summary>
    public Thickness ContentPadding
    {
        get => (Thickness)GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    /// <summary>Updates the native composition from the immutable bindable-property snapshot.</summary>
    private void Refresh()
    {
        _headerHost.Content = Header ?? new Label { Text = "Details" };
        _expandedHost.Content = ExpandedContent;
        _expandedHost.Padding = ContentPadding;
        _expandedHost.IsVisible = IsExpanded && ExpandedContent is not null;
    }
}
