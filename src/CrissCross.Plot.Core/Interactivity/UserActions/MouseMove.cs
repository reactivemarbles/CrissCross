// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActions;

/// <summary>Represents a mouse-move action.</summary>
/// <param name="Pixel">The pixel position of the mouse.</param>
public record struct MouseMove(Pixel Pixel) : IMouseAction
{
    /// <summary>Gets the device associated with the action.</summary>
    public readonly string Device => "mouse";

    /// <summary>Gets a description of the action.</summary>
    public readonly string Description => $"mouse moved to {Pixel}";

    /// <summary>Gets or sets the time when the action occurred.</summary>
    public DateTime DateTime { get; set; } = UserActionTimestamp.UtcNow;
}
