// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides color sampling from a colormap.</summary>
public interface IColormap
{
    /// <summary>Gets the human-readable name of this colormap.</summary>
    string Name { get; }

    /// <summary>Gets the color at a position on this colormap, clamping positions outside the range.</summary>
    /// <param name="position">The fractional distance along the colormap.</param>
    /// <returns>The sampled color, or transparent when the position is NaN.</returns>
    Color GetColor(double position);
}
