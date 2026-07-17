// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Extensions;
#else
namespace CrissCross.Avalonia.UI.Extensions;
#endif

/// <summary>Extension methods for <see cref="Control"/>.</summary>
public static class ControlExtensions
{
    /// <summary>Provides extension members for <see cref="Control"/>.</summary>
    /// <param name="control">The control to start from.</param>
    extension(Control control)
    {
        /// <summary>Finds a parent of the given type.</summary>
        /// <param name="controlType">The type of parent to find.</param>
        /// <returns>The parent of the specified type, or null if not found.</returns>
        public Control? FindParent(Type controlType)
        {
            ArgumentNullException.ThrowIfNull(control);
            ArgumentNullException.ThrowIfNull(controlType);

            var parent = control.Parent;
            while (parent is not null)
            {
                if (parent is Control typedParent && controlType.IsInstanceOfType(typedParent))
                {
                    return typedParent;
                }

                parent = parent.Parent;
            }

            return null;
        }

        /// <summary>Finds a child of the given type.</summary>
        /// <param name="controlType">The type of child to find.</param>
        /// <returns>The child of the specified type, or null if not found.</returns>
        public Control? FindChild(Type controlType)
        {
            ArgumentNullException.ThrowIfNull(control);
            ArgumentNullException.ThrowIfNull(controlType);

            if (control is not Panel panel)
            {
                return null;
            }

            foreach (var child in panel.Children)
            {
                if (controlType.IsInstanceOfType(child))
                {
                    return child;
                }

                if (child is Control childControl)
                {
                    var result = childControl.FindChild(controlType);
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
