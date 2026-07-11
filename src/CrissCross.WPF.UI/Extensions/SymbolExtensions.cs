// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>Set of extensions for the enumeration of icons to facilitate their management and replacement.</summary>
public static class SymbolExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="icon">The icon value.</param>
    extension(SymbolFilled icon)
    {
        /// <summary>Replaces <see cref="SymbolFilled" /> with <see cref="SymbolRegular" />.</summary>
        /// <returns>SymbolRegular.</returns>
        public SymbolRegular Swap() =>
            SymbolGlyph.Parse(icon.ToString());

        /// <summary>Converts <see cref="SymbolFilled" /> to <see langword="string" /> based on the ID.</summary>
        /// <returns>A string.</returns>
        public string GetString() =>
            Encoding.Unicode.GetString(BitConverter.GetBytes((int)icon)).TrimEnd('\0');
    }

    /// <summary>Provides extension members.</summary>
    /// <param name="icon">The icon value.</param>
    extension(SymbolRegular icon)
    {
        /// <summary>Replaces <see cref="SymbolRegular" /> with <see cref="SymbolFilled" />.</summary>
        /// <returns>SymbolFilled.</returns>
        public SymbolFilled Swap() =>
            SymbolGlyph.ParseFilled(icon.ToString());

        /// <summary>Converts <see cref="SymbolRegular" /> to <see langword="string" /> based on the ID.</summary>
        /// <returns>A string.</returns>
        public string GetString() =>
            Encoding.Unicode.GetString(BitConverter.GetBytes((int)icon)).TrimEnd('\0');
    }
}
