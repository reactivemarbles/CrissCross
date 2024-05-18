// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Set of static methods to operate on <see cref="SymbolRegular"/> and <see cref="SymbolFilled"/>.
/// </summary>
public static class SymbolGlyph
{
    /// <summary>
    /// If the icon is not found in some places, this one will be displayed.
    /// </summary>
    public const SymbolRegular DefaultIcon = SymbolRegular.BorderNone24;

    /// <summary>
    /// If the filled icon is not found in some places, this one will be displayed.
    /// </summary>
    public const SymbolFilled DefaultFilledIcon = SymbolFilled.BorderNone24;

    /// <summary>
    /// Finds icon based on name.
    /// </summary>
    /// <param name="name">Name of the icon.</param>
    /// <returns>Symbol Regular.</returns>
    public static SymbolRegular Parse(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return DefaultIcon;
        }

        try
        {
            return (SymbolRegular)Enum.Parse(typeof(SymbolRegular), name);
        }
        catch (Exception)
        {
#if DEBUG
            throw;
#else
            return DefaultIcon;
#endif
        }
    }

    /// <summary>
    /// Finds icon based on name.
    /// </summary>
    /// <param name="name">Name of the icon.</param>
    /// <returns>Symbol Regular.</returns>
    public static SymbolFilled ParseFilled(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return DefaultFilledIcon;
        }

        try
        {
            return (SymbolFilled)Enum.Parse(typeof(SymbolFilled), name);
        }
        catch (Exception)
        {
#if DEBUG
            throw;
#else
            return DefaultFilledIcon;
#endif
        }
    }
}
