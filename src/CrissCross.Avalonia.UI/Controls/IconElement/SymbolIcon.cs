// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using CrissCross.Avalonia.UI.Extensions;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a text element containing an icon glyph.</summary>
public class SymbolIcon : FontIcon
{
    /// <summary>Property for <see cref="Symbol"/>.</summary>
    public static readonly StyledProperty<SymbolRegular> SymbolProperty =
        AvaloniaProperty.Register<SymbolIcon, SymbolRegular>(nameof(Symbol), SymbolRegular.Empty);

    /// <summary>Property for <see cref="Filled"/>.</summary>
    public static readonly StyledProperty<bool> FilledProperty =
        AvaloniaProperty.Register<SymbolIcon, bool>(nameof(Filled), false);

    /// <summary>Provides the SymbolIcon member.</summary>
    static SymbolIcon()
    {
        _ = SymbolProperty.Changed.AddClassHandler<SymbolIcon>((x, _) => x.OnGlyphChanged());
        _ = FilledProperty.Changed.AddClassHandler<SymbolIcon>((x, _) => x.OnFilledChanged());
    }

    /// <summary>Initializes a new instance of the <see cref="SymbolIcon"/> class.</summary>
    public SymbolIcon()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SymbolIcon"/> class.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="fontSize">Size of the font.</param>
    /// <param name="filled">if set to <c>true</c> use filled variant.</param>
    public SymbolIcon(SymbolRegular symbol, double fontSize = 14, bool filled = false)
    {
        Symbol = symbol;
        Filled = filled;
        FontSize = fontSize;
    }

    /// <summary>Gets or sets displayed <see cref="SymbolRegular"/>.</summary>
    public SymbolRegular Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether to use the filled version of the symbol.</summary>
    public bool Filled
    {
        get => GetValue(FilledProperty);
        set => SetValue(FilledProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        OnGlyphChanged();
    }

    /// <summary>Provides the OnFilledChanged member.</summary>
    private void OnFilledChanged() => OnGlyphChanged();

    /// <summary>Provides the OnGlyphChanged member.</summary>
    private void OnGlyphChanged()
    {
        if (Filled)
        {
            Glyph = Symbol.Swap().GetString();
        }
        else
        {
            Glyph = Symbol.GetString();
        }
    }
}
