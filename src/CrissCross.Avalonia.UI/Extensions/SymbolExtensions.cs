// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>Extension methods for working with <see cref="Controls.SymbolRegular"/> and <see cref="Controls.SymbolFilled"/>.</summary>
public static class SymbolExtensions
{
    /// <summary>Provides extension members for <see cref="Controls.SymbolFilled"/>.</summary>
    /// <param name="symbol">The symbol.</param>
    extension(Controls.SymbolFilled symbol)
    {
        /// <summary>Converts the <see cref="Controls.SymbolFilled"/> to its Unicode string representation.</summary>
        /// <returns>The Unicode string representation of the symbol.</returns>
        public string GetString() =>
            char.ConvertFromUtf32((int)symbol);

        /// <summary>Swaps the <see cref="Controls.SymbolFilled"/> to its <see cref="Controls.SymbolRegular"/> equivalent.</summary>
        /// <returns>The regular version of the symbol if available, otherwise the same symbol value as regular.</returns>
        public Controls.SymbolRegular Swap() =>
            (Controls.SymbolRegular)(int)symbol;
    }

    /// <summary>Provides extension members for <see cref="Controls.SymbolRegular"/>.</summary>
    /// <param name="symbol">The symbol.</param>
    extension(Controls.SymbolRegular symbol)
    {
        /// <summary>Converts the <see cref="Controls.SymbolRegular"/> to its Unicode string representation.</summary>
        /// <returns>The Unicode string representation of the symbol.</returns>
        public string GetString() =>
            char.ConvertFromUtf32((int)symbol);

        /// <summary>Swaps the <see cref="Controls.SymbolRegular"/> to its <see cref="Controls.SymbolFilled"/> equivalent.</summary>
        /// <returns>The filled version of the symbol if available, otherwise the same symbol value as filled.</returns>
        public Controls.SymbolFilled Swap() =>
            (Controls.SymbolFilled)(int)symbol;
    }
}
