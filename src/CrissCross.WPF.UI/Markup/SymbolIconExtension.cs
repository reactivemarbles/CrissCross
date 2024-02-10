// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Windows.Markup;

namespace CrissCross.WPF.UI.Markup;

/// <summary>
/// Custom <see cref="MarkupExtension"/> which can provide <see cref="SymbolIcon"/>.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:Button
///     Appearance="Primary"
///     Content="WPF UI button with font icon"
///     Icon="{ui:SymbolIcon Symbol=Fluent24}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:Button Icon="{ui:SymbolIcon Fluent24}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:HyperlinkButton Icon="{ui:SymbolIcon Fluent24}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:TitleBar Icon="{ui:SymbolIcon Fluent24}" /&gt;
/// </code>
/// </example>
[ContentProperty(nameof(Symbol))]
[MarkupExtensionReturnType(typeof(SymbolIcon))]
public class SymbolIconExtension : MarkupExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolIconExtension"/> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public SymbolIconExtension(SymbolRegular symbol) => Symbol = symbol;

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolIconExtension"/> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    public SymbolIconExtension(string symbol) => Symbol = (SymbolRegular)Enum.Parse(typeof(SymbolRegular), symbol);

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolIconExtension"/> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="filled">if set to <c>true</c> [filled].</param>
    public SymbolIconExtension(SymbolRegular symbol, bool filled)
        : this(symbol) => Filled = filled;

    /// <summary>
    /// Gets or sets the symbol.
    /// </summary>
    /// <value>
    /// The symbol.
    /// </value>
    [ConstructorArgument("symbol")]
    public SymbolRegular Symbol { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="SymbolIconExtension"/> is filled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if filled; otherwise, <c>false</c>.
    /// </value>
    [ConstructorArgument("filled")]
    public bool Filled { get; set; }

    /// <summary>
    /// Gets or sets the size of the font.
    /// </summary>
    /// <value>
    /// The size of the font.
    /// </value>
    public double FontSize { get; set; }

    /// <summary>
    /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>
    /// The object value to set on the property where the extension is applied.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var symbolIcon = new SymbolIcon { Symbol = Symbol, Filled = Filled };

        if (FontSize > 0)
        {
            symbolIcon.FontSize = FontSize;
        }

        return symbolIcon;
    }
}
