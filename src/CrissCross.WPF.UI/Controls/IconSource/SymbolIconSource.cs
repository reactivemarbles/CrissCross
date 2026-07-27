// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents an icon source that uses a glyph from the specified font.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SymbolIconSource : IconSource
{
    /// <summary>Property for <see cref="FontSize"/>.</summary>
    public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        nameof(FontSize),
        typeof(double),
        typeof(SymbolIconSource),
        new(SystemFonts.MessageFontSize));

    /// <summary>Property for <see cref="FontStyle"/>.</summary>
    public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
        nameof(FontStyle),
        typeof(FontStyle),
        typeof(SymbolIconSource),
        new(FontStyles.Normal));

    /// <summary>Property for <see cref="FontWeight"/>.</summary>
    public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
        nameof(FontWeight),
        typeof(FontWeight),
        typeof(SymbolIconSource),
        new(FontWeights.Normal));

    /// <summary>Property for <see cref="Symbol"/>.</summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        nameof(Symbol),
        typeof(SymbolRegular),
        typeof(SymbolIconSource),
        new(SymbolRegular.Empty));

    /// <summary>Property for <see cref="Filled"/>.</summary>
    public static readonly DependencyProperty FilledProperty = DependencyProperty.Register(
        nameof(Filled),
        typeof(bool),
        typeof(SymbolIconSource),
        new(false));

    /// <summary>Gets or sets the font size used to render the symbol.</summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>Gets or sets the font weight used to render the symbol.</summary>
    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    /// <summary>Gets or sets the font style used to render the symbol.</summary>
    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    /// <summary>Gets or sets displayed <see cref="SymbolRegular"/>.</summary>
    public SymbolRegular Symbol
    {
        get => (SymbolRegular)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>Gets or sets whether defines whether we should use the SymbolFilled.</summary>
    public bool Filled
    {
        get => (bool)GetValue(FilledProperty);
        set => SetValue(FilledProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Creates the icon element.</summary>
    /// <returns>
    /// A IconElement.
    /// </returns>
    public override IconElement CreateIconElement()
    {
        SymbolIcon symbolIcon = new(Symbol, FontSize, Filled);

        if (!FontSize.Equals(SystemFonts.MessageFontSize))
        {
            symbolIcon.FontSize = FontSize;
        }

        if (FontWeight != FontWeights.Normal)
        {
            symbolIcon.FontWeight = FontWeight;
        }

        if (FontStyle != FontStyles.Normal)
        {
            symbolIcon.FontStyle = FontStyle;
        }

        if (!Equals(Foreground, SystemColors.ControlTextBrush))
        {
            symbolIcon.Foreground = Foreground;
        }

        return symbolIcon;
    }
}
