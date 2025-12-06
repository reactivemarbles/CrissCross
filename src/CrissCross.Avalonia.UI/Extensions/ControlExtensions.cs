// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>
/// Extension methods for <see cref="Control"/>.
/// </summary>
public static class ControlExtensions
{
    /// <summary>
    /// Finds a parent of the given type.
    /// </summary>
    /// <typeparam name="T">The type of parent to find.</typeparam>
    /// <param name="control">The control to start from.</param>
    /// <returns>The parent of the specified type, or null if not found.</returns>
    public static T? FindParent<T>(this Control control)
        where T : Control
    {
        ArgumentNullException.ThrowIfNull(control);

        var parent = control.Parent;
        while (parent is not null)
        {
            if (parent is T typedParent)
            {
                return typedParent;
            }

            parent = parent.Parent;
        }

        return null;
    }

    /// <summary>
    /// Finds a child of the given type.
    /// </summary>
    /// <typeparam name="T">The type of child to find.</typeparam>
    /// <param name="control">The control to start from.</param>
    /// <returns>The child of the specified type, or null if not found.</returns>
    public static T? FindChild<T>(this Control control)
        where T : Control
    {
        ArgumentNullException.ThrowIfNull(control);

        if (control is not Panel panel)
        {
            return null;
        }

        foreach (var child in panel.Children)
        {
            if (child is T typedChild)
            {
                return typedChild;
            }

            if (child is Control childControl)
            {
                var result = childControl.FindChild<T>();
                if (result is not null)
                {
                    return result;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the bounds of the control relative to the root visual.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <returns>The bounds in screen coordinates.</returns>
    public static Rect GetBoundsRelativeToRoot(this Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        var root = control.GetVisualRoot();
        if (root is null)
        {
            return default;
        }

        var transform = control.TransformToVisual((Visual)root);
        if (transform.HasValue)
        {
            return new Rect(control.Bounds.Size).TransformToAABB(transform.Value);
        }

        return default;
    }
}
