// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Value indicating the general character set for a given character.</summary>
internal enum CharacterType
{
    /// <summary>Indicates we could not match the character set.</summary>
    Other = 0,

    /// <summary>Member of the Latin character set.</summary>
    Standard = 1,

    /// <summary>Member of a symbolic character set.</summary>
    Symbolic = 2,

    /// <summary>Member of a character set which supports glyphs.</summary>
    Glyph = 3,
}
