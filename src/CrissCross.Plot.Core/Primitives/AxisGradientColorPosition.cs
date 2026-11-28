// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines a color stop at a position in axis units.</summary>
/// <param name="color">The color at the axis position.</param>
/// <param name="position">The axis position of the color stop.</param>
public class AxisGradientColorPosition(Color color, double position)
{
    /// <summary>Gets or sets the color at this axis position.</summary>
    public Color Color { get; set; } = color;

    /// <summary>Gets or sets the axis position of this color stop.</summary>
    public double Position { get; set; } = position;
}
