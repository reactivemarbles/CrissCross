// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a label at a corner of a triangular plot and includes styling information.</summary>
/// <param name="position">The coordinate of the corner label.</param>
/// <param name="labelText">The text displayed by the label.</param>
public class TriangularAxisCornerLabel(Coordinates position, string labelText)
{
    /// <summary>Gets or sets the coordinate of the corner label.</summary>
    public Coordinates Position { get; set; } = position;

    /// <summary>Gets the style used to render the label.</summary>
    public LabelStyle LabelStyle { get; } = new() { Alignment = Alignment.MiddleCenter, };

    /// <summary>Gets or sets the text displayed by the label.</summary>
    public string? LabelText { get; set; } = labelText;
}
