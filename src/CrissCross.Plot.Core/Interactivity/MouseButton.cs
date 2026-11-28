// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>Represents a physical button on a mouse.</summary>
/// <param name="name">The name of the mouse button.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1801", Justification = "The positional constructor parameter name is retained for source compatibility.")]
public readonly record struct MouseButton(string name)
{
    /// <summary>Gets the name of the mouse button.</summary>
    public string Name { get; } = name;
}
