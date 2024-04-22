// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Helper methods for UI-related tasks.
/// </summary>
internal static class TreeHelper
{
    /// <summary>
    /// Analyzes both visual and logical tree in order to find all elements of a given type that
    /// are descendants of the <paramref name="source"/> item.
    /// </summary>
    /// <typeparam name="T">The type of the queried items.</typeparam>
    /// <param name="source">
    /// The root element that marks the source of the search. If the source is already of the
    /// requested type, it will not be included in the result.
    /// </param>
    /// <returns>All descendants of <paramref name="source"/> that match the requested type.</returns>
    public static IEnumerable<T> FindChildren<T>(this DependencyObject source)
        where T : DependencyObject
    {
        if (source != null)
        {
            foreach (var child in GetChildObjects(source))
            {
                // analyze if children match the requested type
                if (child is T t)
                {
                    yield return t;
                }

                // recurse tree
                foreach (var descendant in FindChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }
    }

    /// <summary>
    /// This method is an alternative to WPF's <see cref="VisualTreeHelper.GetChild"/> method,
    /// which also supports content elements. Keep in mind that for content elements, this method
    /// falls back to the logical tree of the element.
    /// </summary>
    /// <param name="parent">The item to be processed.</param>
    /// <returns>The submitted item's child elements, if available.</returns>
    public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent)
    {
        if (parent == null)
        {
            yield break;
        }

        if (parent is ContentElement || parent is FrameworkElement)
        {
            // use the logical tree for content / framework elements
            foreach (var obj in from object obj in LogicalTreeHelper.GetChildren(parent) let depObj = obj as DependencyObject where depObj != null select obj)
            {
                yield return (DependencyObject)obj;
            }
        }
        else
        {
            // use the visual tree per default
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                yield return VisualTreeHelper.GetChild(parent, i);
            }
        }
    }

    /// <summary>
    /// This method is an alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method,
    /// which also supports content elements. Keep in mind that for content element, this method
    /// falls back to the logical tree of the element!.
    /// </summary>
    /// <param name="child">The item to be processed.</param>
    /// <returns>The submitted item's parent, if available. Otherwise null.</returns>
    public static DependencyObject? GetParentObject(this DependencyObject child)
    {
        if (child == null)
        {
            return null;
        }

        // handle content elements separately
        if (child is ContentElement contentElement)
        {
            var parent = ContentOperations.GetParent(contentElement);
            if (parent != null)
            {
                return parent;
            }

            var fce = contentElement as FrameworkContentElement;
            return fce?.Parent;
        }

        // also try searching for parent in framework elements (such as DockPanel, etc)
        if (child is FrameworkElement frameworkElement)
        {
            var parent = frameworkElement.Parent;
            if (parent != null)
            {
                return parent;
            }
        }

        // if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
        return VisualTreeHelper.GetParent(child);
    }

    /// <summary>
    /// Tries to locate a given item within the visual tree, starting with the dependency object
    /// at a given position.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the element to be found on the visual tree of the element at the given location.
    /// </typeparam>
    /// <param name="reference">The main element which is used to perform hit testing.</param>
    /// <param name="point">The position to be evaluated on the origin.</param>
    /// <returns>A T.</returns>
    public static T? TryFindFromPoint<T>(UIElement reference, Point point)
        where T : DependencyObject => reference.InputHitTest(point) is not DependencyObject element ? (T?)null : element is T t ? t : TryFindParent<T>(element);

    /// <summary>
    /// Finds a parent of a given item on the visual tree.
    /// </summary>
    /// <typeparam name="T">The type of the queried item.</typeparam>
    /// <param name="child">A direct or indirect child of the queried item.</param>
    /// <returns>
    /// The first parent item that matches the submitted type parameter. If not matching item can
    /// be found, a null reference is being returned.
    /// </returns>
    public static T? TryFindParent<T>(this DependencyObject child)
        where T : DependencyObject
    {
        // get parent item
        var parentObject = GetParentObject(child);

        // we've reached the end of the tree
        if (parentObject == null)
        {
            return null;
        }

        // check if the parent matches the type we're looking for
        var parent = parentObject as T;
        return parent ?? TryFindParent<T>(parentObject);

        // use recursion to proceed with next level
    }
}
