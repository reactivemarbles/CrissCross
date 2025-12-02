// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Set of extensions for the enumeration of icons to facilitate their management and replacement.
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    /// Replaces <see cref="SymbolRegular" /> with <see cref="SymbolFilled" />.
    /// </summary>
    /// <param name="icon">The icon.</param>
    /// <returns>SymbolFilled.</returns>
    public static SymbolFilled Swap(this SymbolRegular icon) =>
        SymbolGlyph.ParseFilled(icon.ToString());

    /// <summary>
    /// Replaces <see cref="SymbolFilled" /> with <see cref="SymbolRegular" />.
    /// </summary>
    /// <param name="icon">The icon.</param>
    /// <returns>SymbolRegular.</returns>
    public static SymbolRegular Swap(this SymbolFilled icon) =>
        SymbolGlyph.Parse(icon.ToString());

    /// <summary>
    /// Converts <see cref="SymbolRegular" /> to <see langword="string" /> based on the ID.
    /// </summary>
    /// <param name="icon">The icon.</param>
    /// <returns>A string.</returns>
    public static string GetString(this SymbolRegular icon) =>
        Encoding.Unicode.GetString(BitConverter.GetBytes((int)icon)).TrimEnd('\0');

    /// <summary>
    /// Converts <see cref="SymbolFilled" /> to <see langword="string" /> based on the ID.
    /// </summary>
    /// <param name="icon">The icon.</param>
    /// <returns>A string.</returns>
    public static string GetString(this SymbolFilled icon) =>
        Encoding.Unicode.GetString(BitConverter.GetBytes((int)icon)).TrimEnd('\0');
}
