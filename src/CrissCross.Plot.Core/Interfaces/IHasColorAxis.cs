// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents an object with a color axis.</summary>
public interface IHasColorAxis
{
    /// <summary>Gets the colormap used by the color axis.</summary>
    IColormap Colormap { get; }

    /// <summary>Gets the coordinate range represented by the color axis.</summary>
    /// <returns>The color-axis range.</returns>
    Range GetRange();
}
