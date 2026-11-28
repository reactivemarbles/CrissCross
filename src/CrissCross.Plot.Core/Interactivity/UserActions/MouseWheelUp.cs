// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActions;

/// <summary>Represents an upward mouse wheel action.</summary>
/// <param name="Pixel">The pixel associated with the mouse action.</param>
public record struct MouseWheelUp(Pixel Pixel) : IMouseButtonAction
{
    /// <summary>Gets the mouse wheel button associated with this action.</summary>
    public readonly MouseButton Button => StandardMouseButtons.Wheel;

    /// <summary>Gets whether this action represents a button press.</summary>
    public readonly bool IsPressed => true;

    /// <summary>Gets the input device description.</summary>
    public readonly string Device => "mouse wheel";

    /// <summary>Gets the action description.</summary>
    public readonly string Description => "mouse wheel up";

    /// <summary>Gets or sets the UTC time when this action was recorded.</summary>
    public DateTime DateTime { get; set; } = UserActionTimestamp.UtcNow;
}
