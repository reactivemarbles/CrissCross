// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>
/// Represents a single key on a keyboard that may be pressed and held.
/// Keys are tracked by <see cref="KeyboardState"/> and <see cref="IUserActionResponse"/>
/// classes can see which keys are pressed when they are executed.
/// </summary>
public readonly record struct Key
{
    /// <summary>Initializes a new instance of the <see cref="Key"/> struct.</summary>
    /// <param name="name">The name that identifies the key.</param>
    public Key(string name)
    {
        Name = name.ToLower();
    }

    /// <summary>Gets a name that uniquely identifies a specific key.</summary>
    public string Name { get; }
}
