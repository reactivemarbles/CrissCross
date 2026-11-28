// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a control handle used to interact with a plottable.</summary>
/// <param name="parent">The plottable that owns this handle.</param>
/// <param name="cursor">The cursor to display while interacting with the handle.</param>
/// <param name="index">The index identifying this handle among its parent's handles.</param>
public class InteractiveHandle(IHasInteractiveHandles parent, Cursor cursor, int index = 0)
{
    /// <summary>Gets the plottable this handle belongs to. This is useful when events pass handles themselves.</summary>
    public IHasInteractiveHandles Parent { get; } = parent;

    /// <summary>
    /// Gets the index that uniquely identifies this handle.
    /// This value is only useful for interactive objects with multiple handles.
    /// </summary>
    public int Index { get; } = index;

    /// <summary>Gets cursor the control should use when interacting with this handle.</summary>
    public Cursor Cursor { get; } = cursor;

    /// <summary>Returns a string identifying the handle's parent and index.</summary>
    /// <returns>A description of this handle.</returns>
    public override string ToString() => $"{Parent.GetType()} handle {Index}";
}
