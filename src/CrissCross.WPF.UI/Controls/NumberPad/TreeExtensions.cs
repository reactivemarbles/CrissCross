// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Helper methods for UI-related tasks.</summary>
internal static class TreeExtensions
{
    /// <summary>Provides dependency object tree traversal extension members.</summary>
    /// <param name="source">The root element that marks the source of the search.</param>
    extension(DependencyObject source)
    {
        /// <summary>Provides the FindChildren member.</summary>
        /// <typeparam name="T">The type of the queried items.</typeparam>
        /// <returns>All descendants of <paramref name="source"/> that match the requested type.</returns>
        public IEnumerable<T> FindChildren<T>()
            where T : DependencyObject
        {
            if (source is null)
            {
                yield break;
            }

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

        /// <summary>
        /// This method is an alternative to WPF's <see cref="VisualTreeHelper.GetChild"/> method,
        /// which also supports content elements. Keep in mind that for content elements, this method
        /// falls back to the logical tree of the element.
        /// </summary>
        /// <returns>The submitted item's child elements, if available.</returns>
        public IEnumerable<DependencyObject> GetChildObjects()
        {
            if (source is null)
            {
                yield break;
            }

            if (source is ContentElement || source is FrameworkElement)
            {
                // use the logical tree for content / framework elements
                foreach (
                    var obj in from object obj in LogicalTreeHelper.GetChildren(source)
                    let depObj = obj as DependencyObject
                    where depObj is not null
                    select obj)
                {
                    yield return (DependencyObject)obj;
                }
            }
            else
            {
                // use the visual tree per default
                var count = VisualTreeHelper.GetChildrenCount(source);
                for (var i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(source, i);
                }
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's <see cref="VisualTreeHelper.GetParent"/> method,
        /// which also supports content elements. Keep in mind that for content element, this method
        /// falls back to the logical tree of the element!.
        /// </summary>
        /// <returns>The submitted item's parent, if available. Otherwise null.</returns>
        public DependencyObject? GetParentObject()
        {
            if (source is null)
            {
                return null;
            }

            // handle content elements separately
            if (source is ContentElement contentElement)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent is not null)
                {
                    return parent;
                }

                var fce = contentElement as FrameworkContentElement;
                return fce?.Parent;
            }

            // also try searching for parent in framework elements (such as DockPanel, etc)
            if (source is FrameworkElement frameworkElement)
            {
                var parent = frameworkElement.Parent;
                if (parent is not null)
                {
                    return parent;
                }
            }

            // if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(source);
        }

        /// <summary>Finds a parent of a given item on the visual tree.</summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <returns>
        /// The first parent item that matches the submitted type parameter. If not matching item can
        /// be found, a null reference is being returned.
        /// </returns>
        public T? TryFindParent<T>()
            where T : DependencyObject
        {
            // get parent item
            var parentObject = GetParentObject(source);

            // we've reached the end of the tree
            if (parentObject is null)
            {
                return null;
            }

            // check if the parent matches the type we're looking for
            var parent = parentObject as T;
            return parent ?? TryFindParent<T>(parentObject);

            // use recursion to proceed with next level
        }
    }

    /// <summary>Provides the TryFindFromPoint member.</summary>
    /// <typeparam name="T">
    /// The type of the element to be found on the visual tree of the element at the given location.
    /// </typeparam>
    /// <param name="reference">The main element which is used to perform hit testing.</param>
    /// <param name="point">The position to be evaluated on the origin.</param>
    /// <returns>A T.</returns>
    public static T? TryFindFromPoint<T>(UIElement reference, Point point)
        where T : DependencyObject
    {
        if (reference.InputHitTest(point) is not DependencyObject element)
        {
            return null;
        }

        return element is T t ? t : TryFindParent<T>(element);
    }
}
