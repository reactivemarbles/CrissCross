// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides indexed access to a collection of colors.</summary>
public interface IPalette
{
    /// <summary>Gets all colors in this palette.</summary>
    Color[] Colors { get; }

    /// <summary>Gets the display name of this palette.</summary>
    string Name { get; }

    /// <summary>Gets additional information about this palette, such as its source.</summary>
    string Description { get; }

    /// <summary>Gets the color at an index, wrapping when the index exceeds the palette size.</summary>
    /// <param name="index">The color index.</param>
    /// <returns>The color at the specified index.</returns>
    Color GetColor(int index);
}
