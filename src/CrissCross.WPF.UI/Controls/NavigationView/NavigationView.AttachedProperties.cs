// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigationView.
/// </summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="INavigationView" />
public partial class NavigationView
{
    /// <summary>
    /// The header content property.
    /// </summary>
    public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.RegisterAttached(
        "HeaderContent",
        typeof(object),
        typeof(FrameworkElement),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Gets the content of the header.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>An object.</returns>
    public static object? GetHeaderContent(FrameworkElement target) => target?.GetValue(HeaderContentProperty);

    /// <summary>
    /// Sets the content of the header.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="headerContent">Content of the header.</param>
    public static void SetHeaderContent(FrameworkElement target, object headerContent) =>
        target?.SetValue(HeaderContentProperty, headerContent);
}
