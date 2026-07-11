// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents NavigationView.</summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="INavigationView" />
public partial class NavigationView
{
    /// <summary>Attached property for <see cref="NavigationView"/>'s to get its parent.</summary>
    internal static readonly DependencyProperty NavigationParentProperty =
        DependencyProperty.RegisterAttached(
            nameof(NavigationParent),
            typeof(INavigationView),
            typeof(INavigationView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Gets the parent navigation view.</summary>
    internal INavigationView NavigationParent
    {
        get => (INavigationView)GetValue(NavigationParentProperty);
        private set => SetValue(NavigationParentProperty, value);
    }

    /// <summary>Gets the <see cref="NavigationView" /> parent view for its <see cref="INavigationViewItem" /> children.</summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="navigationItem">The navigation item.</param>
    /// <returns>
    /// Instance of the <see cref="NavigationView" /> or <see langword="null" />.
    /// </returns>
    internal static NavigationView? GetNavigationParent<T>(T navigationItem)
        where T : DependencyObject, INavigationViewItem =>
        navigationItem.GetValue(NavigationParentProperty) is NavigationView navigationView ? navigationView : null;
}
