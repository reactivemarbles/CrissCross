// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays a profile image or accessible initials when no image is available.</summary>
public class PersonPicture : ContentView
{
    /// <summary>Bindable property for <see cref="DisplayName"/>.</summary>
    public static readonly BindableProperty DisplayNameProperty = BindableProperty.Create(
        nameof(DisplayName),
        typeof(string),
        typeof(PersonPicture),
        propertyChanged: static (view, _, _) => ((PersonPicture)view).Refresh());

    /// <summary>Bindable property for <see cref="Initials"/>.</summary>
    public static readonly BindableProperty InitialsProperty = BindableProperty.Create(
        nameof(Initials),
        typeof(string),
        typeof(PersonPicture),
        propertyChanged: static (view, _, _) => ((PersonPicture)view).Refresh());

    /// <summary>Bindable property for <see cref="Source"/>.</summary>
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(ImageSource),
        typeof(PersonPicture),
        propertyChanged: static (view, _, _) => ((PersonPicture)view).Refresh());

    /// <summary>Displays the supplied profile image.</summary>
    private readonly Image _image = new() { Aspect = Aspect.AspectFill };

    /// <summary>Displays the initials fallback.</summary>
    private readonly Label _initials = new() { HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

    /// <summary>Initializes a new instance of the <see cref="PersonPicture"/> class.</summary>
    public PersonPicture()
    {
        Content = new Grid { Children = { _initials, _image } };
        Refresh();
    }

    /// <summary>Gets or sets the display name from which fallback initials are derived.</summary>
    public string? DisplayName
    {
        get => (string?)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    /// <summary>Gets or sets explicit initials used when <see cref="Source"/> is unavailable.</summary>
    public string? Initials
    {
        get => (string?)GetValue(InitialsProperty);
        set => SetValue(InitialsProperty, value);
    }

    /// <summary>Gets or sets the optional profile image source.</summary>
    public ImageSource? Source
    {
        get => (ImageSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>Creates at most two initials without culture-sensitive casing transformations.</summary>
    /// <param name="name">The display name.</param>
    /// <returns>The fallback initials.</returns>
    private static string CreateInitials(string? name)
    {
        var words = name?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        return words.Length switch
        {
            0 => string.Empty,
            1 => words[0][..1].ToUpperInvariant(),
            _ => string.Concat(words[0][..1], words[^1][..1]).ToUpperInvariant(),
        };
    }

    /// <summary>Updates the fallback presentation from the immutable bindable-property snapshot.</summary>
    private void Refresh()
    {
        _image.Source = Source;
        _image.IsVisible = Source is not null;
        _initials.Text = string.IsNullOrWhiteSpace(Initials) ? CreateInitials(DisplayName) : Initials;
        _initials.IsVisible = Source is null;
        SemanticProperties.SetDescription(this, DisplayName ?? _initials.Text);
    }
}
