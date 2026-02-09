// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Ala Pa**one color card.
/// </summary>
public class CardColor : global::Avalonia.Controls.Control
{
    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<CardColor, string>(
        nameof(Title), string.Empty);

    /// <summary>
    /// Property for <see cref="Subtitle"/>.
    /// </summary>
    public static readonly StyledProperty<string> SubtitleProperty = AvaloniaProperty.Register<CardColor, string>(
        nameof(Subtitle), string.Empty);

    /// <summary>
    /// Property for <see cref="SubtitleFontSize"/>.
    /// </summary>
    public static readonly StyledProperty<double> SubtitleFontSizeProperty = AvaloniaProperty.Register<CardColor, double>(
        nameof(SubtitleFontSize), 11.0d);

    /// <summary>
    /// Property for <see cref="Color"/>.
    /// </summary>
    public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<CardColor, Color>(
        nameof(Color), Color.FromArgb(0, 0, 0, 0));

    /// <summary>
    /// Property for <see cref="Brush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> BrushProperty = AvaloniaProperty.Register<CardColor, IBrush>(
        nameof(Brush), new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));

    /// <summary>
    /// Property for <see cref="CardBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> CardBrushProperty = AvaloniaProperty.Register<CardColor, IBrush>(
        nameof(CardBrush), new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));

    static CardColor()
    {
        SubtitleProperty.Changed.AddClassHandler<CardColor>((x, e) => x.OnSubtitlePropertyChanged());
        ColorProperty.Changed.AddClassHandler<CardColor>((x, e) => x.OnColorPropertyChanged());
        BrushProperty.Changed.AddClassHandler<CardColor>((x, e) => x.OnBrushPropertyChanged());
    }

    /// <summary>
    /// Gets or sets the main text displayed below the color.
    /// </summary>
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets text displayed under main <see cref="Title"/>.
    /// </summary>
    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size of <see cref="Subtitle"/>.
    /// </summary>
    public double SubtitleFontSize
    {
        get => GetValue(SubtitleFontSizeProperty);
        set => SetValue(SubtitleFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the displayed <see cref="CardBrush"/>.
    /// </summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the displayed <see cref="CardBrush"/>.
    /// </summary>
    public IBrush Brush
    {
        get => GetValue(BrushProperty);
        set => SetValue(BrushProperty, value);
    }

    /// <summary>
    /// Gets the brush used to render the background of the card.
    /// </summary>
    public IBrush CardBrush
    {
        get => GetValue(CardBrushProperty);
        private set => SetValue(CardBrushProperty, value);
    }

    /// <summary>
    /// Virtual method triggered when <see cref="Subtitle"/> is changed.
    /// </summary>
    protected virtual void OnSubtitlePropertyChanged()
    {
    }

    /// <summary>
    /// Virtual method triggered when <see cref="Color"/> is changed.
    /// </summary>
    protected virtual void OnColorPropertyChanged() => CardBrush = new SolidColorBrush(Color);

    /// <summary>
    /// Virtual method triggered when <see cref="Brush"/> is changed.
    /// </summary>
    protected virtual void OnBrushPropertyChanged() => CardBrush = Brush;
}
