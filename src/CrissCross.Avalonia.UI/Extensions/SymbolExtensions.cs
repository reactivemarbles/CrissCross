// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>
/// Extension methods for working with <see cref="Controls.SymbolRegular"/> and <see cref="Controls.SymbolFilled"/>.
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    /// Converts the <see cref="Controls.SymbolRegular"/> to its Unicode string representation.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The Unicode string representation of the symbol.</returns>
    public static string GetString(this Controls.SymbolRegular symbol) =>
        char.ConvertFromUtf32((int)symbol);

    /// <summary>
    /// Converts the <see cref="Controls.SymbolFilled"/> to its Unicode string representation.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The Unicode string representation of the symbol.</returns>
    public static string GetString(this Controls.SymbolFilled symbol) =>
        char.ConvertFromUtf32((int)symbol);

    /// <summary>
    /// Swaps the <see cref="Controls.SymbolRegular"/> to its <see cref="Controls.SymbolFilled"/> equivalent.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The filled version of the symbol if available, otherwise the same symbol value as filled.</returns>
    public static Controls.SymbolFilled Swap(this Controls.SymbolRegular symbol) =>
        (Controls.SymbolFilled)(int)symbol;

    /// <summary>
    /// Swaps the <see cref="Controls.SymbolFilled"/> to its <see cref="Controls.SymbolRegular"/> equivalent.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The regular version of the symbol if available, otherwise the same symbol value as regular.</returns>
    public static Controls.SymbolRegular Swap(this Controls.SymbolFilled symbol) =>
        (Controls.SymbolRegular)(int)symbol;
}
