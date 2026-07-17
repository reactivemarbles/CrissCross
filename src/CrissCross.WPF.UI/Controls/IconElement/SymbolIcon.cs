// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Extensions;
#else
using CrissCross.WPF.UI.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents a text element containing an icon glyph.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(SymbolIcon), "SymbolIcon.bmp")]
public partial class SymbolIcon : FontIcon
{
    /// <summary>Property for <see cref="Symbol"/>.</summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        nameof(Symbol),
        typeof(SymbolRegular),
        typeof(SymbolIcon),
        new PropertyMetadata(SymbolRegular.Empty, static (o, _) => ((SymbolIcon)o).OnGlyphChanged()));

    /// <summary>Property for <see cref="Filled"/>.</summary>
    public static readonly DependencyProperty FilledProperty = DependencyProperty.Register(
        nameof(Filled),
        typeof(bool),
        typeof(SymbolIcon),
        new PropertyMetadata(false, OnFilledChanged));

    /// <summary>Initializes a new instance of the <see cref="SymbolIcon"/> class.</summary>
    public SymbolIcon() { }

    /// <summary>Initializes a new instance of the <see cref="SymbolIcon"/> class.</summary>
    /// <param name="symbol">The symbol.</param>
    public SymbolIcon(SymbolRegular symbol)
        : this(symbol, DefaultFontSize, false) { }

    /// <summary>Initializes a new instance of the <see cref="SymbolIcon"/> class.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="fontSize">Size of the font.</param>
    public SymbolIcon(SymbolRegular symbol, double fontSize)
        : this(symbol, fontSize, false) { }

    /// <summary>Initializes a new instance of the <see cref="SymbolIcon"/> class.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="fontSize">Size of the font.</param>
    /// <param name="filled">if set to <c>true</c> [filled].</param>
    public SymbolIcon(SymbolRegular symbol, double fontSize, bool filled)
    {
        Symbol = symbol;
        Filled = filled;
        FontSize = fontSize;
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

    /// <summary>Raises the <see cref="E:System.Windows.FrameworkElement.Initialized" /> event. This method is invoked
    /// whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized" /> is set to true internally.</summary>
    /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        SetFontReference();
    }

    /// <summary>Provides the OnFilledChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnFilledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        var self = (SymbolIcon)d;
        self.SetFontReference();
        self.OnGlyphChanged();
    }

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

    /// <summary>Provides the SetFontReference member.</summary>
    private void SetFontReference() =>
        SetResourceReference(FontFamilyProperty, Filled ? "FluentSystemIconsFilled" : "FluentSystemIcons");
}
