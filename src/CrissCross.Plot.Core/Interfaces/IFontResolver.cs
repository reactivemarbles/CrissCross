// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides functionality that converts a requested typeface into a physical font.</summary>
public interface IFontResolver
{
    /// <summary>Creates a typeface with the specified styles.</summary>
    /// <param name="fontName">The requested font family name.</param>
    /// <param name="weight">The requested font weight.</param>
    /// <param name="slant">The requested font slant.</param>
    /// <param name="spacing">The requested font spacing.</param>
    /// <returns>The matching typeface, or <see langword="null"/> if it cannot be created.</returns>
    SKTypeface? CreateTypeface(string fontName, FontWeight weight, FontSlant slant, FontSpacing spacing);

    /// <summary>Creates the typeface that most closely matches the requested family and style.</summary>
    /// <param name="fontName">The requested font family name.</param>
    /// <param name="bold">Whether bold text is requested.</param>
    /// <param name="italic">Whether italic text is requested.</param>
    /// <returns>The closest matching typeface, or <see langword="null"/> if none is available.</returns>
    SKTypeface? CreateTypeface(string fontName, bool bold, bool italic);
}
