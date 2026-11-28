// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActions;

/// <summary>Represents a key-released action.</summary>
/// <param name="Key">The key that was released.</param>
public record struct KeyUp(Key Key) : IUserAction
{
    /// <summary>Gets the device associated with the action.</summary>
    public readonly string Device => $"key {Key.Name}";

    /// <summary>Gets a description of the action.</summary>
    public readonly string Description => $"key {Key.Name} released";

    /// <summary>Gets or sets the time when the action occurred.</summary>
    public DateTime DateTime { get; set; } = UserActionTimestamp.UtcNow;
}
