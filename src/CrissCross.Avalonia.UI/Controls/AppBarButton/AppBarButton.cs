// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a compact command button with a circular icon surface and caption.</summary>
public class AppBarButton : global::Avalonia.Controls.Button
{
    /// <summary>Property for <see cref="BackgroundGlyph"/>.</summary>
    public static readonly StyledProperty<string> BackgroundGlyphProperty =
        AvaloniaProperty.Register<AppBarButton, string>(nameof(BackgroundGlyph), string.Empty);

    /// <summary>Property for <see cref="BackgroundGlyphFontSize"/>.</summary>
    public static readonly StyledProperty<double> BackgroundGlyphFontSizeProperty =
        AvaloniaProperty.Register<AppBarButton, double>(nameof(BackgroundGlyphFontSize), 33D);

    /// <summary>Property for <see cref="ElipseDiameter"/>.</summary>
    public static readonly StyledProperty<double> ElipseDiameterProperty =
        AvaloniaProperty.Register<AppBarButton, double>(nameof(ElipseDiameter), 40D);

    /// <summary>Property for <see cref="ForegroundGlyph"/>.</summary>
    public static readonly StyledProperty<string> ForegroundGlyphProperty =
        AvaloniaProperty.Register<AppBarButton, string>(nameof(ForegroundGlyph), string.Empty);

    /// <summary>Property for <see cref="ForegroundGlyphColor"/>.</summary>
    public static readonly StyledProperty<IBrush?> ForegroundGlyphColorProperty =
        AvaloniaProperty.Register<AppBarButton, IBrush?>(nameof(ForegroundGlyphColor));

    /// <summary>Property for <see cref="ForegroundGlyphFontSize"/>.</summary>
    public static readonly StyledProperty<double> ForegroundGlyphFontSizeProperty =
        AvaloniaProperty.Register<AppBarButton, double>(nameof(ForegroundGlyphFontSize), 33D);

    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly StyledProperty<SymbolRegular> IconProperty =
        AvaloniaProperty.Register<AppBarButton, SymbolRegular>(nameof(Icon), SymbolRegular.Empty);

    /// <summary>Property for <see cref="IconData"/>.</summary>
    public static readonly StyledProperty<Geometry?> IconDataProperty =
        AvaloniaProperty.Register<AppBarButton, Geometry?>(nameof(IconData));

    /// <summary>Property for <see cref="IconHeight"/>.</summary>
    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<AppBarButton, double>(nameof(IconHeight), 25D);

    /// <summary>Property for <see cref="IconWidth"/>.</summary>
    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<AppBarButton, double>(nameof(IconWidth), 25D);

    /// <summary>Property for <see cref="IsIconFilled"/>.</summary>
    public static readonly StyledProperty<bool> IsIconFilledProperty =
        AvaloniaProperty.Register<AppBarButton, bool>(nameof(IsIconFilled));

    /// <summary>Gets or sets the background glyph.</summary>
    public string BackgroundGlyph
    {
        get => GetValue(BackgroundGlyphProperty);
        set => SetValue(BackgroundGlyphProperty, value);
    }

    /// <summary>Gets or sets the background glyph font size.</summary>
    public double BackgroundGlyphFontSize
    {
        get => GetValue(BackgroundGlyphFontSizeProperty);
        set => SetValue(BackgroundGlyphFontSizeProperty, value);
    }

    /// <summary>Gets or sets the circular icon surface diameter.</summary>
    /// <remarks>The spelling is retained for source compatibility with the WPF AppBarButton.</remarks>
    public double ElipseDiameter
    {
        get => GetValue(ElipseDiameterProperty);
        set => SetValue(ElipseDiameterProperty, value);
    }

    /// <summary>Gets or sets the foreground glyph.</summary>
    public string ForegroundGlyph
    {
        get => GetValue(ForegroundGlyphProperty);
        set => SetValue(ForegroundGlyphProperty, value);
    }

    /// <summary>Gets or sets the foreground glyph brush.</summary>
    public IBrush? ForegroundGlyphColor
    {
        get => GetValue(ForegroundGlyphColorProperty);
        set => SetValue(ForegroundGlyphColorProperty, value);
    }

    /// <summary>Gets or sets the foreground glyph font size.</summary>
    public double ForegroundGlyphFontSize
    {
        get => GetValue(ForegroundGlyphFontSizeProperty);
        set => SetValue(ForegroundGlyphFontSizeProperty, value);
    }

    /// <summary>Gets or sets the native Fluent symbol displayed by the button.</summary>
    public SymbolRegular Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets custom vector geometry displayed by the button.</summary>
    public Geometry? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    /// <summary>Gets or sets the icon height.</summary>
    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    /// <summary>Gets or sets the icon width.</summary>
    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the filled Fluent symbol variant is displayed.</summary>
    public bool IsIconFilled
    {
        get => GetValue(IsIconFilledProperty);
        set => SetValue(IsIconFilledProperty, value);
    }
}
