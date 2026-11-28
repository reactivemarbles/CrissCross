// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines interaction callbacks for handles displayed on a plot element.</summary>
public interface IHasInteractiveHandles
{
    /// <summary>Gets the interactive handle inside the specified rectangle.</summary>
    /// <param name="rect">The rectangle used to locate a handle.</param>
    /// <returns>The matching handle, or <see langword="null"/> if no handle is inside the rectangle.</returns>
    InteractiveHandle? GetHandle(CoordinateRect rect);

    /// <summary>Called when a handle is pressed, typically at the start of a drag.</summary>
    /// <param name="handle">The handle that was pressed.</param>
    void PressHandle(InteractiveHandle handle);

    /// <summary>Called when a handle is moved, typically during a drag.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the handle.</param>
    void MoveHandle(InteractiveHandle handle, Coordinates point);

    /// <summary>Called when a handle is released, typically at the end of a drag.</summary>
    /// <param name="handle">The handle that was released.</param>
    void ReleaseHandle(InteractiveHandle handle);
}
