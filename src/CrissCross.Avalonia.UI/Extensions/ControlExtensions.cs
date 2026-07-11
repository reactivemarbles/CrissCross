// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace CrissCross.Avalonia.UI.Extensions;

/// <summary>Extension methods for <see cref="Control"/>.</summary>
public static class ControlExtensions
{
    /// <summary>Provides extension members for <see cref="Control"/>.</summary>
    /// <param name="control">The control to start from.</param>
    extension(Control control)
    {
        /// <summary>Finds a parent of the given type.</summary>
        /// <typeparam name="T">The type of parent to find.</typeparam>
        /// <returns>The parent of the specified type, or null if not found.</returns>
        public T? FindParent<T>()
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

        /// <summary>Finds a child of the given type.</summary>
        /// <typeparam name="T">The type of child to find.</typeparam>
        /// <returns>The child of the specified type, or null if not found.</returns>
        public T? FindChild<T>()
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

        /// <summary>Gets the bounds of the control relative to the root visual.</summary>
        /// <returns>The bounds in screen coordinates.</returns>
        public Rect GetBoundsRelativeToRoot()
        {
            ArgumentNullException.ThrowIfNull(control);

            var root = control.GetSelfAndVisualAncestors().LastOrDefault();
            if (root is null)
            {
                return default;
            }

            var transform = control.TransformToVisual((Visual)root);
            return transform.HasValue ? new Rect(control.Bounds.Size).TransformToAABB(transform.Value) : default;
        }
    }
}
