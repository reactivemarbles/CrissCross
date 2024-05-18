// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a header for a group of menu items in a NavigationMenu.
/// </summary>
[ToolboxItem(true)]
[System.Drawing.ToolboxBitmap(typeof(NavigationViewItemHeader), "NavigationViewItemHeader.bmp")]
public class NavigationViewItemHeader : System.Windows.Controls.Control
{
    /// <summary>
    /// Property for <see cref="Text"/>.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(NavigationViewItemHeader),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(NavigationViewItemHeader),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets text presented in the header element.
    /// </summary>
    [Bindable(true)]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
