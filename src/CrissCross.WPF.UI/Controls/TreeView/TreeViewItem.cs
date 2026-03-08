// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extended <see cref="System.Windows.Controls.TreeViewItem"/> with <see cref="SymbolRegular"/> properties.
/// </summary>
public class TreeViewItem : System.Windows.Controls.TreeViewItem
{
    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(TreeViewItem),
        new PropertyMetadata(null, IconPropertyChanged));

    /// <summary>
    /// Identifies the IconVisibility dependency property, which determines whether the icon associated with the
    /// TreeViewItem is visible.
    /// </summary>
    /// <remarks>The IconVisibilityProperty is used to control the display state of the icon within a
    /// TreeViewItem. By default, the icon is collapsed. Setting this property to <see cref="Visibility.Visible"/> will
    /// display the icon, while <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Hidden"/> will hide it. This
    /// property can be used in styles, templates, or code to customize the appearance of TreeViewItem
    /// controls.</remarks>
    public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register(
        nameof(IconVisibility),
        typeof(Visibility),
        typeof(TreeViewItem),
        new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Gets or sets displayed <see cref="IconElement"/>.
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement?)GetValue(IconProperty);
        set
        {
            SetValue(IconVisibilityProperty, value == null ? Visibility.Collapsed : Visibility.Visible);
            SetValue(IconProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the visibility of the icon associated with the control.
    /// </summary>
    /// <remarks>A value of <see cref="Visibility.Visible"/> displays the icon, while <see
    /// cref="Visibility.Collapsed"/> hides it. Changing this property may affect the layout of the control.</remarks>
    public Visibility IconVisibility
    {
        get => (Visibility)GetValue(IconVisibilityProperty);
        set => SetValue(IconVisibilityProperty, value);
    }

    /// <inheritdoc/>
    protected override DependencyObject GetContainerForItemOverride() => new TreeViewItem();

    /// <inheritdoc/>
    protected override bool IsItemItsOwnContainerOverride(object item) => item is TreeViewItem;

    private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TreeViewItem item)
        {
            item.IconVisibility = e.NewValue == null ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
