// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Text;

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
        //// It is possible that the alternative icon does not exist
        SymbolGlyph.ParseFilled(icon.ToString());

    /// <summary>
    /// Replaces <see cref="SymbolFilled" /> with <see cref="SymbolRegular" />.
    /// </summary>
    /// <param name="icon">The icon.</param>
    /// <returns>SymbolRegular.</returns>
    public static SymbolRegular Swap(this SymbolFilled icon) =>
        //// It is possible that the alternative icon does not exist
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
