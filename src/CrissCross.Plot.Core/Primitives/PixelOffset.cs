// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>
/// Represents the distance from one pixel relative to another in pixel units.
/// Increasing X offset moves a pixel to the right.
/// Increasing Y offset moves a pixel downward.
/// </summary>
/// <param name="X">The horizontal offset in pixels.</param>
/// <param name="Y">The vertical offset in pixels.</param>
public readonly record struct PixelOffset(float X, float Y)
{
    /// <summary>Gets the zero offset.</summary>
    public static PixelOffset Zero => new(0, 0);

    /// <summary>Gets an offset with <see cref="float.NaN"/> components.</summary>
    public static PixelOffset NaN => new(float.NaN, float.NaN);

    /// <summary>Creates an offset with the specified vertical component.</summary>
    /// <param name="newY">The new vertical offset.</param>
    /// <returns>The updated offset.</returns>
    public PixelOffset WithY(float newY) => new(X, newY);

    /// <summary>Creates an offset with the specified horizontal component.</summary>
    /// <param name="newX">The new horizontal offset.</param>
    /// <returns>The updated offset.</returns>
    public PixelOffset WithX(float newX) => new(newX, Y);
}
