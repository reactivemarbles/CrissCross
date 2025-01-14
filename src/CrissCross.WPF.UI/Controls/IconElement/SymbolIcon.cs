// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using CrissCross.WPF.UI.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a text element containing an icon glyph.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(SymbolIcon), "SymbolIcon.bmp")]
public class SymbolIcon : FontIcon
{
    /// <summary>
    /// Property for <see cref="Symbol"/>.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
        nameof(Symbol),
        typeof(SymbolRegular),
        typeof(SymbolIcon),
        new PropertyMetadata(SymbolRegular.Empty, static (o, _) => ((SymbolIcon)o).OnGlyphChanged()));

    /// <summary>
    /// Property for <see cref="Filled"/>.
    /// </summary>
    public static readonly DependencyProperty FilledProperty = DependencyProperty.Register(
        nameof(Filled),
        typeof(bool),
        typeof(SymbolIcon),
        new PropertyMetadata(false, OnFilledChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolIcon"/> class.
    /// </summary>
    public SymbolIcon()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolIcon"/> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="fontSize">Size of the font.</param>
    /// <param name="filled">if set to <c>true</c> [filled].</param>
    public SymbolIcon(SymbolRegular symbol, double fontSize = 14, bool filled = false)
    {
        Symbol = symbol;
        Filled = filled;
        FontSize = fontSize;
    }

    /// <summary>
    /// Gets or sets displayed <see cref="SymbolRegular"/>.
    /// </summary>
    public SymbolRegular Symbol
    {
        get => (SymbolRegular)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether defines whether or not we should use the <see cref="SymbolFilled"/>.
    /// </summary>
    public bool Filled
    {
        get => (bool)GetValue(FilledProperty);
        set => SetValue(FilledProperty, value);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized" /> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized" /> is set to true internally.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        SetFontReference();
    }

    private static void OnFilledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (SymbolIcon)d;
        self.SetFontReference();
        self.OnGlyphChanged();
    }

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

    private void SetFontReference() =>
        SetResourceReference(FontFamilyProperty, Filled ? "FluentSystemIconsFilled" : "FluentSystemIcons");
}
