// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Set of static methods to operate on <see cref="SymbolRegular"/> and <see cref="SymbolFilled"/>.</summary>
public static class SymbolGlyph
{
    /// <summary>If the icon is not found in some places, this one will be displayed.</summary>
    public static readonly SymbolRegular DefaultIcon = SymbolRegular.BorderNone24;

    /// <summary>If the filled icon is not found in some places, this one will be displayed.</summary>
    public static readonly SymbolFilled DefaultFilledIcon = SymbolFilled.BorderNone24;

    /// <summary>Finds icon based on name.</summary>
    /// <param name="name">Name of the icon.</param>
    /// <returns>Symbol Regular.</returns>
    public static SymbolRegular Parse(string name) => !string.IsNullOrEmpty(name) && Enum.TryParse<SymbolRegular>(name, out var symbol) ? symbol : DefaultIcon;

    /// <summary>Finds icon based on name.</summary>
    /// <param name="name">Name of the icon.</param>
    /// <returns>Symbol Regular.</returns>
    public static SymbolFilled ParseFilled(string name) => !string.IsNullOrEmpty(name) && Enum.TryParse<SymbolFilled>(name, out var symbol)
        ? symbol
        : DefaultFilledIcon;
}
