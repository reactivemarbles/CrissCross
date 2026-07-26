// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays a named colour swatch using native MAUI composition.</summary>
public class CardColor : ContentView
{
    /// <summary>Bindable property for <see cref="Title"/>.</summary>
    public static readonly BindableProperty TitleProperty = CreateProperty(nameof(Title), string.Empty);

    /// <summary>Bindable property for <see cref="Subtitle"/>.</summary>
    public static readonly BindableProperty SubtitleProperty = CreateProperty(nameof(Subtitle), string.Empty);

    /// <summary>Bindable property for <see cref="Color"/>.</summary>
    public static readonly BindableProperty ColorProperty = CreateProperty(nameof(Color), Colors.Transparent);

    /// <summary>Spacing between visual card elements.</summary>
    private const double ContentSpacing = 4;

    /// <summary>Displays the colour swatch.</summary>
    private readonly Border _swatch = new Border { HeightRequest = 48, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 } };

    /// <summary>Displays the title.</summary>
    private readonly Label _title = new() { FontAttributes = FontAttributes.Bold };

    /// <summary>Displays the subtitle.</summary>
    private readonly Label _subtitle = new();

    /// <summary>Initializes a new instance of the <see cref="CardColor"/> class.</summary>
    public CardColor()
    {
        Content = new VerticalStackLayout { Spacing = ContentSpacing, Children = { _swatch, _title, _subtitle } };
        Refresh();
    }

    /// <summary>Gets or sets the main text displayed under the swatch.</summary>
    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Gets or sets the supporting text displayed under the title.</summary>
    public string? Subtitle
    {
        get => (string?)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>Gets or sets the displayed swatch colour.</summary>
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>Creates a bindable property that refreshes the native composition.</summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="name">The public property name.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The bindable property definition.</returns>
    private static BindableProperty CreateProperty<T>(string name, T defaultValue) => BindableProperty.Create(
        name,
        typeof(T),
        typeof(CardColor),
        defaultValue,
        propertyChanged: static (view, _, _) => ((CardColor)view).Refresh());

    /// <summary>Updates the visible and accessible snapshot.</summary>
    private void Refresh()
    {
        _swatch.Background = new SolidColorBrush(Color);
        _title.Text = Title;
        _subtitle.Text = Subtitle;
        _subtitle.IsVisible = !string.IsNullOrWhiteSpace(Subtitle);
        SemanticProperties.SetDescription(this, $"{Title}: {Subtitle} {Color}");
    }
}
